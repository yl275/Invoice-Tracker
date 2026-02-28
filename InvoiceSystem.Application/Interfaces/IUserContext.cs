namespace InvoiceSystem.Application.Interfaces;

/// <summary>
/// Provides the current authenticated user's ID (from Clerk JWT sub claim).
/// </summary>
public interface IUserContext
{
    string? UserId { get; }
    bool HasUser { get; }
}
