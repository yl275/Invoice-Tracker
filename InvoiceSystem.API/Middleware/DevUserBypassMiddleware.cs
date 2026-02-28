using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace InvoiceSystem.API.Middleware;

/// <summary>
/// Development-only: when Clerk is not configured, sets a fake user from X-User-Id header (default: user_demo).
/// Allows local testing without setting up Clerk.
/// </summary>
public class DevUserBypassMiddleware
{
    private const string DefaultDevUserId = "user_demo";
    private readonly RequestDelegate _next;

    public DevUserBypassMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var userId = context.Request.Headers["X-User-Id"].FirstOrDefault()?.Trim()
            ?? DefaultDevUserId;

        if (!string.IsNullOrEmpty(userId))
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim("sub", userId)
            }, "DevBypass");
            context.User = new ClaimsPrincipal(identity);
        }

        await _next(context);
    }
}
