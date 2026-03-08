using System.Security.Claims;
using InvoiceSystem.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace InvoiceSystem.API;

/// <summary>
/// Resolves the current user ID from the authenticated user's claims (Clerk JWT sub).
/// TeamIds are set by middleware after loading from DB.
/// </summary>
public class UserContext : IUserContext
{
    private static readonly IReadOnlyList<Guid> EmptyTeamIds = Array.Empty<Guid>();

    private readonly IHttpContextAccessor _httpContextAccessor;
    private IReadOnlyList<Guid> _teamIds = EmptyTeamIds;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");

    public bool HasUser => !string.IsNullOrEmpty(UserId);

    public IReadOnlyList<Guid> TeamIds => _teamIds;
    private Guid? _currentTeamIdOverride;
    public Guid? CurrentTeamId => _currentTeamIdOverride ?? (_teamIds.Count > 0 ? _teamIds[0] : null);

    /// <summary>Set from middleware when X-Current-Team-Id is present and user is in that team.</summary>
    public void SetCurrentTeamId(Guid? teamId)
    {
        _currentTeamIdOverride = teamId != null && _teamIds.Contains(teamId.Value) ? teamId : null;
    }

    private string _dataScope = "team";
    public string DataScope => _dataScope;

    /// <summary>Called by middleware to set team membership for the current request.</summary>
    public void SetTeamIds(IReadOnlyList<Guid> teamIds)
    {
        _teamIds = teamIds ?? EmptyTeamIds;
    }

    /// <summary>Called by middleware to set data scope from X-Data-Scope header (team | mine). Default is team so owner/members see all.</summary>
    public void SetDataScope(string? scope)
    {
        _dataScope = string.Equals(scope, "mine", StringComparison.OrdinalIgnoreCase) ? "mine" : "team";
    }
}
