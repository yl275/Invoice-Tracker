using InvoiceSystem.Application.DTOs.Team;
using InvoiceSystem.Application.Interfaces;
using InvoiceSystem.Application.Interfaces.Repositories;
using InvoiceSystem.Application.Interfaces.Services;
using InvoiceSystem.Domain.Entities;

namespace InvoiceSystem.Application.Services;

public class TeamService : ITeamService
{
    private readonly ITeamRepository _teamRepository;
    private readonly IUserContext _userContext;

    public TeamService(ITeamRepository teamRepository, IUserContext userContext)
    {
        _teamRepository = teamRepository;
        _userContext = userContext;
    }

    public async Task<IReadOnlyList<TeamDto>> ListMyTeamsAsync()
    {
        if (!_userContext.HasUser) return Array.Empty<TeamDto>();
        var ids = _userContext.TeamIds;
        if (ids.Count == 0) return Array.Empty<TeamDto>();
        var teams = await _teamRepository.GetByIdsAsync(ids);
        return teams.Select(t => new TeamDto { Id = t.Id, Name = t.Name, CreatedAt = t.CreatedAt }).ToList();
    }

    public async Task<TeamDto?> GetTeamAsync(Guid teamId)
    {
        if (!_userContext.HasUser || !_userContext.TeamIds.Contains(teamId)) return null;
        var team = await _teamRepository.GetByIdAsync(teamId);
        return team == null ? null : new TeamDto { Id = team.Id, Name = team.Name, CreatedAt = team.CreatedAt };
    }

    public async Task<TeamDto?> CreateTeamAsync(string name)
    {
        if (!_userContext.HasUser) return null;
        var team = Team.Create(name.Trim(), _userContext.UserId!);
        await _teamRepository.AddTeamAsync(team);
        return new TeamDto { Id = team.Id, Name = team.Name, CreatedAt = team.CreatedAt };
    }

    /// <summary>Leave a team. Fails if this is the user's last team (keeps personal data safe).</summary>
    public async Task<(bool Ok, string? Error)> LeaveTeamAsync(Guid teamId)
    {
        if (!_userContext.HasUser || !_userContext.TeamIds.Contains(teamId))
            return (false, null);
        if (_userContext.TeamIds.Count <= 1)
            return (false, "Cannot leave your last team. You must always have at least one workspace.");
        var team = await _teamRepository.GetByIdAsync(teamId);
        if (team == null) return (false, null);
        team.RemoveMember(_userContext.UserId!);
        await _teamRepository.SaveChangesAsync();
        return (true, null);
    }

    public async Task<TeamInvitationDto?> InviteAsync(Guid teamId, string email, string baseUrl)
    {
        if (!_userContext.HasUser || !_userContext.TeamIds.Contains(teamId))
            return null;
        var team = await _teamRepository.GetByIdAsync(teamId);
        if (team == null) return null;
        var invitation = TeamInvitation.Create(teamId, email.Trim(), _userContext.UserId!);
        await _teamRepository.AddInvitationAsync(invitation);
        var link = $"{baseUrl.TrimEnd('/')}/invite?token={invitation.Token}";
        return new TeamInvitationDto
        {
            Id = invitation.Id,
            Email = invitation.Email,
            ExpiresAt = invitation.ExpiresAt,
            InviteLink = link
        };
    }

    public async Task<bool> AcceptInvitationAsync(string token, string currentUserEmail)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(currentUserEmail)) return false;
        var invitation = await _teamRepository.GetInvitationByTokenAsync(token);
        if (invitation == null || !invitation.IsValid) return false;
        var email = currentUserEmail.Trim().ToLowerInvariant();
        if (invitation.Email != email) return false;

        var team = await _teamRepository.GetByIdAsync(invitation.TeamId);
        if (team == null) return false;
        team.AddMember(_userContext.UserId!, TeamRole.Member);
        invitation.MarkAccepted();
        await _teamRepository.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<TeamMemberDto>> ListMembersAsync(Guid teamId)
    {
        if (!_userContext.HasUser || !_userContext.TeamIds.Contains(teamId)) return Array.Empty<TeamMemberDto>();
        var team = await _teamRepository.GetByIdAsync(teamId);
        if (team == null) return Array.Empty<TeamMemberDto>();
        return team.Members.Select(m => new TeamMemberDto
        {
            UserId = m.UserId,
            Role = m.Role.ToString(),
            JoinedAt = m.JoinedAt
        }).ToList();
    }
}
