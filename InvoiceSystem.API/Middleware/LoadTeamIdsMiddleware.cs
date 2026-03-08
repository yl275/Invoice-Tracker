using System.Security.Claims;
using InvoiceSystem.Application.Interfaces;
using InvoiceSystem.Domain.Entities;
using InvoiceSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InvoiceSystem.API.Middleware;

/// <summary>
/// Loads the current user's team IDs into UserContext and ensures they have a personal team.
/// Must run after authentication.
/// </summary>
public class LoadTeamIdsMiddleware
{
    private const string PersonalTeamName = "My workspace";
    private readonly RequestDelegate _next;

    public LoadTeamIdsMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext, UserContext userContext)
    {
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await _next(context);
            return;
        }

        var scopeHeader = context.Request.Headers["X-Data-Scope"].FirstOrDefault()?.Trim();
        userContext.SetDataScope(scopeHeader);

        var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? context.User?.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            await _next(context);
            return;
        }

        var teamIds = await dbContext.TeamMembers
            .AsNoTracking()
            .Where(m => m.UserId == userId)
            .Select(m => m.TeamId)
            .Distinct()
            .ToListAsync();

        if (teamIds.Count == 0)
        {
            var team = Team.Create(PersonalTeamName, userId);
            dbContext.Teams.Add(team);
            await dbContext.SaveChangesAsync();
            teamIds.Add(team.Id);
        }

        userContext.SetTeamIds(teamIds);

        // Default current team: use header if valid; else prefer shared (multi-member) so owner and members create in same team
        var currentTeamHeader = context.Request.Headers["X-Current-Team-Id"].FirstOrDefault();
        if (Guid.TryParse(currentTeamHeader, out var headerTeamId) && teamIds.Contains(headerTeamId))
        {
            userContext.SetCurrentTeamId(headerTeamId);
        }
        else
        {
            var memberCounts = await dbContext.TeamMembers
                .AsNoTracking()
                .Where(m => teamIds.Contains(m.TeamId))
                .GroupBy(m => m.TeamId)
                .Select(g => new { TeamId = g.Key, Count = g.Count() })
                .ToListAsync();
            var sharedTeamId = memberCounts.FirstOrDefault(x => x.Count > 1)?.TeamId;
            var defaultTeamId = sharedTeamId ?? teamIds[0];
            userContext.SetCurrentTeamId(defaultTeamId);
        }

        await _next(context);
    }
}
