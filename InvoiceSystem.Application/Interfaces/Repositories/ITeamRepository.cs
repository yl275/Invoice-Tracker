using InvoiceSystem.Domain.Entities;

namespace InvoiceSystem.Application.Interfaces.Repositories;

public interface ITeamRepository
{
    Task<IReadOnlyList<Team>> GetByIdsAsync(IEnumerable<Guid> ids);
    Task<Team?> GetByIdAsync(Guid id);
    Task<TeamInvitation?> GetInvitationByTokenAsync(string token);
    Task AddInvitationAsync(TeamInvitation invitation);
    Task AddTeamAsync(Team team);
    Task SaveChangesAsync();
}
