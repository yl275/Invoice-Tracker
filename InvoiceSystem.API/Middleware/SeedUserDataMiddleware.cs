using System.Security.Claims;
using InvoiceSystem.Application.Interfaces;
using InvoiceSystem.Domain.Entities;
using InvoiceSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InvoiceSystem.API.Middleware;

/// <summary>
/// Seeds starter data for a signed-in user on first API visit.
/// If the user has no clients, products, and business profile, create a default set.
/// </summary>
public class SeedUserDataMiddleware
{
    private readonly RequestDelegate _next;

    public SeedUserDataMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext, IUserContext userContext)
    {
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await _next(context);
            return;
        }

        var userId = context.User?.FindFirst("sub")?.Value ?? context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
        {
            await _next(context);
            return;
        }

        var teamId = userContext.CurrentTeamId;
        if (!teamId.HasValue)
        {
            await _next(context);
            return;
        }

        var hasClients = await dbContext.Clients.IgnoreQueryFilters().AnyAsync(c => c.TeamId == teamId.Value);
        var hasProducts = await dbContext.Products.IgnoreQueryFilters().AnyAsync(p => p.TeamId == teamId.Value);
        var hasProfile = await dbContext.BusinessProfiles.IgnoreQueryFilters().AnyAsync(p => p.UserId == userId);

        if (!hasClients && !hasProducts && !hasProfile)
        {
            var clients = new[]
            {
                new Client(teamId.Value, userId, "ABN123456789", "Acme Corp", "0400111222", "billing@acme.test", "Default seeded client"),
                new Client(teamId.Value, userId, "ABN987654321", "Globex Corporation", "0400333444", "finance@globex.test", "Default seeded client"),
                new Client(teamId.Value, userId, "ABN112233445", "Soylent Corp", "0400555666", "accounts@soylent.test", "Default seeded client"),
                new Client(teamId.Value, userId, "ABN998877665", "Initech", "0400777888", "payables@initech.test", "Default seeded client")
            };

            var products = new[]
            {
                new Product(teamId.Value, userId, "Widget A", "WID-001", 10.50m),
                new Product(teamId.Value, userId, "Widget B", "WID-002", 25.00m),
                new Product(teamId.Value, userId, "Super Gadget", "GAD-999", 500.00m),
                new Product(teamId.Value, userId, "Flux Capacitor", "FLUX-001", 1000.00m),
                new Product(teamId.Value, userId, "Sonic Screwdriver", "SONIC-001", 120.00m)
            };

            var emailFromClaim = context.User.FindFirst(ClaimTypes.Email)?.Value
                ?? context.User.FindFirst("email")?.Value
                ?? "profile@invoicesys.local";

            var profile = new BusinessProfile(
                userId,
                "My Business",
                emailFromClaim,
                "0400000000",
                "123 Main Street, Sydney NSW 2000",
                null,
                "ABN123456789",
                "BankTransfer",
                "123-456",
                "987654321",
                null
            );

            await dbContext.Clients.AddRangeAsync(clients);
            await dbContext.Products.AddRangeAsync(products);
            await dbContext.BusinessProfiles.AddAsync(profile);
            await dbContext.SaveChangesAsync();
        }

        await _next(context);
    }
}
