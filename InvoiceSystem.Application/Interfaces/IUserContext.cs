namespace InvoiceSystem.Application.Interfaces;

/// <summary>
/// Provides the current authenticated user's ID (from Clerk JWT sub claim).
/// </summary>
public interface IUserContext
{
    string? UserId { get; }
    bool HasUser { get; }
    IReadOnlyList<Guid> TeamIds { get; }
    /// <summary>Team to use for creating new entities. From X-Current-Team-Id when valid, else first in TeamIds.</summary>
    Guid? CurrentTeamId { get; }
    /// <summary>When "mine", list only entities created by the current user (same team). When "team", list all in team.</summary>
    string DataScope { get; }
}
