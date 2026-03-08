namespace InvoiceSystem.Domain.Entities;

public class Team
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private readonly List<TeamMember> _members = new();
    public IReadOnlyCollection<TeamMember> Members => _members.AsReadOnly();

    protected Team()
    {
    }

    public static Team Create(string name, string ownerUserId)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Team name is required", nameof(name));
        if (string.IsNullOrWhiteSpace(ownerUserId)) throw new ArgumentException("Owner user ID is required", nameof(ownerUserId));

        var team = new Team
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            CreatedAt = DateTime.UtcNow
        };
        team.AddMember(ownerUserId, TeamRole.Owner);
        return team;
    }

    public void AddMember(string userId, TeamRole role = TeamRole.Member)
    {
        if (_members.Any(m => m.UserId == userId))
            return;
        _members.Add(new TeamMember(Id, userId, role));
    }

    public void RemoveMember(string userId)
    {
        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member != null)
            _members.Remove(member);
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Team name is required", nameof(name));
        Name = name.Trim();
    }
}

public enum TeamRole
{
    Owner = 0,
    Member = 1
}
