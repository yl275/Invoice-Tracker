using InvoiceSystem.Application.Interfaces.Repositories;
using InvoiceSystem.Domain.Entities;
using InvoiceSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InvoiceSystem.Infrastructure.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly ApplicationDbContext _context;

    public TeamRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Team>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        var list = ids.ToList();
        if (list.Count == 0) return Array.Empty<Team>();
        return await _context.Teams
            .AsNoTracking()
            .Where(t => list.Contains(t.Id))
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<Team?> GetByIdAsync(Guid id)
    {
        return await _context.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<TeamInvitation?> GetInvitationByTokenAsync(string token)
    {
        return await _context.TeamInvitations
            .Include(i => i.Team)
            .FirstOrDefaultAsync(i => i.Token == token);
    }

    public async Task AddInvitationAsync(TeamInvitation invitation)
    {
        await _context.TeamInvitations.AddAsync(invitation);
        await _context.SaveChangesAsync();
    }

    public async Task AddTeamAsync(Team team)
    {
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
