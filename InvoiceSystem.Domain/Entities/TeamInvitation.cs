namespace InvoiceSystem.Domain.Entities;

public class TeamInvitation
{
    public Guid Id { get; private set; }
    public Guid TeamId { get; private set; }
    public string Email { get; private set; } = null!;
    public string InvitedByUserId { get; private set; } = null!;
    public string Token { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public InvitationStatus Status { get; private set; }

    public Team Team { get; private set; } = null!;

    private TeamInvitation() { }

    public static TeamInvitation Create(Guid teamId, string email, string invitedByUserId, TimeSpan? validFor = null)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required", nameof(email));
        if (string.IsNullOrWhiteSpace(invitedByUserId)) throw new ArgumentException("InvitedBy user ID is required", nameof(invitedByUserId));
        var expiresAt = DateTime.UtcNow.Add(validFor ?? TimeSpan.FromDays(7));
        return new TeamInvitation
        {
            Id = Guid.NewGuid(),
            TeamId = teamId,
            Email = email.Trim().ToLowerInvariant(),
            InvitedByUserId = invitedByUserId,
            Token = Guid.NewGuid().ToString("N"),
            ExpiresAt = expiresAt,
            Status = InvitationStatus.Pending
        };
    }

    public void MarkAccepted()
    {
        if (Status != InvitationStatus.Pending)
            throw new InvalidOperationException("Invitation is no longer pending.");
        Status = InvitationStatus.Accepted;
    }

    public void MarkExpired()
    {
        if (Status == InvitationStatus.Pending)
            Status = InvitationStatus.Expired;
    }

    public bool IsValid => Status == InvitationStatus.Pending && DateTime.UtcNow < ExpiresAt;
}

public enum InvitationStatus
{
    Pending = 0,
    Accepted = 1,
    Expired = 2
}
