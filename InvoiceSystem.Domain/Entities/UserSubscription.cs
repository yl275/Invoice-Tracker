namespace InvoiceSystem.Domain.Entities;

public class UserSubscription
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; } = null!;
    public string Plan { get; private set; } = null!;
    public DateTime PurchasedAt { get; private set; }

    protected UserSubscription()
    {
    }

    public UserSubscription(string userId, string plan, DateTime? purchasedAt = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        if (string.IsNullOrWhiteSpace(plan))
            throw new ArgumentException("Plan cannot be empty", nameof(plan));

        Id = Guid.NewGuid();
        UserId = userId;
        Plan = plan.Trim();
        PurchasedAt = purchasedAt?.ToUniversalTime() ?? DateTime.UtcNow;
    }

    public void ChangePlan(string plan, DateTime? changedAt = null)
    {
        if (string.IsNullOrWhiteSpace(plan))
            throw new ArgumentException("Plan cannot be empty", nameof(plan));

        Plan = plan.Trim();
        PurchasedAt = changedAt?.ToUniversalTime() ?? DateTime.UtcNow;
    }
}

