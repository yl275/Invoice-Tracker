namespace InvoiceSystem.Domain.Entities;

public class TeamMember
{
    public Guid TeamId { get; private set; }
    public string UserId { get; private set; } = null!;
    public TeamRole Role { get; private set; }
    public DateTime JoinedAt { get; private set; }

    public Team Team { get; private set; } = null!;

    internal TeamMember(Guid teamId, string userId, TeamRole role)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User ID is required", nameof(userId));
        TeamId = teamId;
        UserId = userId;
        Role = role;
        JoinedAt = DateTime.UtcNow;
    }

    private TeamMember() { }
}
