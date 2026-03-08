using InvoiceSystem.Domain.Entities;
using InvoiceSystem.Application.Interfaces;
using InvoiceSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvoiceSystem.API.Controllers;

[ApiController]
[Route("api/dev")]
[Authorize]
public class DevController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IUserContext _userContext;

    public DevController(ApplicationDbContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    /// <summary>
    /// Debug: returns the current request's user context and per-team entity counts (no query filter).
    /// Use this to see why list APIs might return fewer rows than expected (e.g. TeamIds or DataScope).
    /// </summary>
    [HttpGet("context")]
    public async Task<IActionResult> GetContextAsync()
    {
        if (!_userContext.HasUser)
            return Ok(new { userId = (string?)null, teamIds = Array.Empty<Guid>(), dataScope = _userContext.DataScope, currentTeamId = (Guid?)null, perTeamCounts = (object?)null });

        var teamIds = _userContext.TeamIds;
        var perTeamCounts = new List<object>();
        foreach (var tid in teamIds)
        {
            var clientCount = await _dbContext.Clients.IgnoreQueryFilters().CountAsync(c => c.TeamId == tid);
            var productCount = await _dbContext.Products.IgnoreQueryFilters().CountAsync(p => p.TeamId == tid);
            var invoiceCount = await _dbContext.Invoices.IgnoreQueryFilters().CountAsync(i => i.TeamId == tid);
            perTeamCounts.Add(new { teamId = tid, clients = clientCount, products = productCount, invoices = invoiceCount });
        }

        return Ok(new
        {
            userId = _userContext.UserId,
            teamIds = teamIds.ToArray(),
            dataScope = _userContext.DataScope,
            currentTeamId = _userContext.CurrentTeamId,
            perTeamCounts
        });
    }

    /// <summary>
    /// Development-only helper: seed some dummy data for the current user.
    /// Creates a few clients, products and invoices. Safe to call multiple times.
    /// </summary>
    [HttpPost("seed")]
    public async Task<IActionResult> SeedAsync()
    {
        if (!_userContext.HasUser)
        {
            return Unauthorized("No current user.");
        }
        if (!_userContext.CurrentTeamId.HasValue)
        {
            return BadRequest("No team context.");
        }

        var userId = _userContext.UserId!;
        var teamId = _userContext.CurrentTeamId.Value;

        var random = new Random();
        var suffix = DateTime.UtcNow.Ticks % 100000;

        var clients = new List<Client>
        {
            new(teamId, userId, $"ABN{random.Next(100000000, 999999999)}", $"Dev Client {suffix}-A", "0400" + random.Next(100000, 999999)),
            new(teamId, userId, $"ABN{random.Next(100000000, 999999999)}", $"Dev Client {suffix}-B", "0400" + random.Next(100000, 999999))
        };

        var products = new List<Product>
        {
            new(teamId, userId, $"Dev Service {suffix}-A", $"DEV-{suffix}-A", Math.Round((decimal)random.NextDouble() * 500m + 50m, 2)),
            new(teamId, userId, $"Dev Service {suffix}-B", $"DEV-{suffix}-B", Math.Round((decimal)random.NextDouble() * 500m + 50m, 2))
        };

        await _dbContext.Clients.AddRangeAsync(clients);
        await _dbContext.Products.AddRangeAsync(products);
        await _dbContext.SaveChangesAsync();

        // Reload with IDs
        var client1 = clients[0];
        var client2 = clients[1];
        var product1 = products[0];
        var product2 = products[1];

        // Use a synthetic business profile snapshot for demo invoices,
        // so this does not depend on any real saved BusinessProfile.
        var demoProfile = new BusinessProfile(
            userId,
            $"Dev Business {suffix}",
            $"dev-{suffix}@invoicesys.local",
            "0400" + random.Next(100000, 999999),
            $"Level {random.Next(1, 20)} Demo Street, Sydney NSW 2000",
            null,
            $"ABN{random.Next(100000000, 999999999)}",
            "BankTransfer",
            $"{random.Next(100, 999)}-{random.Next(100, 999)}",
            random.NextInt64(100000000, 999999999).ToString(),
            null);

        var invoice1 = new Invoice(teamId, userId, $"DEV-{suffix}-1", DateTime.UtcNow.Date, client1, demoProfile);
        invoice1.AddItem(product1, random.Next(1, 5));
        invoice1.AddItem(product2, random.Next(1, 3));

        var invoice2 = new Invoice(teamId, userId, $"DEV-{suffix}-2", DateTime.UtcNow.Date.AddDays(-1), client2, demoProfile);
        invoice2.AddItem(product2, random.Next(1, 4));

        await _dbContext.Invoices.AddRangeAsync(invoice1, invoice2);
        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            clientsCreated = clients.Count,
            productsCreated = products.Count,
            invoicesCreated = 2
        });
    }

    /// <summary>
    /// Permanently delete all data (clients, products, invoices, business profile)
    /// for the current user. Intended for developer cleanup and testing.
    /// </summary>
    [HttpDelete("everything")]
    public async Task<IActionResult> DeleteEverythingAsync()
    {
        if (!_userContext.HasUser)
        {
            return Unauthorized("No current user.");
        }

        var userId = _userContext.UserId!;
        var teamIds = _userContext.TeamIds;

        var invoices = await _dbContext.Invoices.IgnoreQueryFilters().Where(i => teamIds.Contains(i.TeamId)).ToListAsync();
        var clients = await _dbContext.Clients.IgnoreQueryFilters().Where(c => teamIds.Contains(c.TeamId)).ToListAsync();
        var products = await _dbContext.Products.IgnoreQueryFilters().Where(p => teamIds.Contains(p.TeamId)).ToListAsync();
        var profiles = await _dbContext.BusinessProfiles.IgnoreQueryFilters().Where(p => p.UserId == userId).ToListAsync();

        var invoiceCount = invoices.Count;
        var clientCount = clients.Count;
        var productCount = products.Count;
        var profileCount = profiles.Count;

        _dbContext.Invoices.RemoveRange(invoices);
        _dbContext.Clients.RemoveRange(clients);
        _dbContext.Products.RemoveRange(products);
        _dbContext.BusinessProfiles.RemoveRange(profiles);

        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            invoicesDeleted = invoiceCount,
            clientsDeleted = clientCount,
            productsDeleted = productCount,
            profilesDeleted = profileCount
        });
    }
}

