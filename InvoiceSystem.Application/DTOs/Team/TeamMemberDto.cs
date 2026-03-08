namespace InvoiceSystem.Application.DTOs.Team;

public class TeamMemberDto
{
    public string UserId { get; set; } = null!;
    public string Role { get; set; } = null!;
    public DateTime JoinedAt { get; set; }
}
