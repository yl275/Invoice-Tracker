using System.Security.Claims;
using InvoiceSystem.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace InvoiceSystem.API;

/// <summary>
/// Resolves the current user ID from the authenticated user's claims (Clerk JWT sub).
/// </summary>
public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");

    public bool HasUser => !string.IsNullOrEmpty(UserId);
}
