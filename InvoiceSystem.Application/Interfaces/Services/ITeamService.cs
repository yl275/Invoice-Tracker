using InvoiceSystem.Application.DTOs.Team;

namespace InvoiceSystem.Application.Interfaces.Services;

public interface ITeamService
{
    Task<IReadOnlyList<TeamDto>> ListMyTeamsAsync();
    Task<TeamDto?> GetTeamAsync(Guid teamId);
    Task<TeamDto?> CreateTeamAsync(string name);
    Task<(bool Ok, string? Error)> LeaveTeamAsync(Guid teamId);
    Task<TeamInvitationDto?> InviteAsync(Guid teamId, string email, string baseUrl);
    Task<bool> AcceptInvitationAsync(string token, string currentUserEmail);
    Task<IReadOnlyList<TeamMemberDto>> ListMembersAsync(Guid teamId);
}
