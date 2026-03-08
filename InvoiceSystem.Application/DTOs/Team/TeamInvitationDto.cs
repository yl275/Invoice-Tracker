namespace InvoiceSystem.Application.DTOs.Team;

public class TeamInvitationDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public string InviteLink { get; set; } = null!;
}
