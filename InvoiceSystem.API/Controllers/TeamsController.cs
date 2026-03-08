using System.Security.Claims;
using InvoiceSystem.Application.DTOs.Team;
using InvoiceSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceSystem.API.Controllers;

[ApiController]
[Route("api/teams")]
[Authorize]
public class TeamsController : ControllerBase
{
    private readonly ITeamService _teamService;
    private readonly IConfiguration _configuration;

    public TeamsController(ITeamService teamService, IConfiguration configuration)
    {
        _teamService = teamService;
        _configuration = configuration;
    }

    /// <summary>List teams the current user is a member of.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TeamDto>>> List()
    {
        var teams = await _teamService.ListMyTeamsAsync();
        return Ok(teams);
    }

    /// <summary>Get a single team by ID (must be a member).</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TeamDto>> Get(Guid id)
    {
        var team = await _teamService.GetTeamAsync(id);
        if (team == null) return NotFound();
        return Ok(team);
    }

    /// <summary>Create a new team (current user becomes owner).</summary>
    [HttpPost]
    public async Task<ActionResult<TeamDto>> Create([FromBody] CreateTeamRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Name?.Trim())) return BadRequest("Team name is required.");
        var team = await _teamService.CreateTeamAsync(request.Name.Trim());
        if (team == null) return Unauthorized();
        return CreatedAtAction(nameof(Get), new { id = team.Id }, team);
    }

    /// <summary>Leave a team (remove current user from members). Cannot leave your last team.</summary>
    [HttpPost("{id:guid}/leave")]
    public async Task<ActionResult> Leave(Guid id)
    {
        var (ok, error) = await _teamService.LeaveTeamAsync(id);
        if (!string.IsNullOrEmpty(error)) return BadRequest(error);
        if (!ok) return NotFound();
        return Ok(new { success = true });
    }

    /// <summary>Invite a user by email. Returns invite link to share.</summary>
    [HttpPost("{id:guid}/invite")]
    public async Task<ActionResult<TeamInvitationDto>> Invite(Guid id, [FromBody] InviteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Email)) return BadRequest("Email is required.");
        var baseUrl = _configuration["PublicUrl"] ?? $"{Request.Scheme}://{Request.Host}";
        var dto = await _teamService.InviteAsync(id, request.Email, baseUrl);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    /// <summary>Accept an invitation by token. Current user's email must match the invitation.
    /// Email is read from JWT claims first; if not present (e.g. Clerk default JWT), pass email in the request body.</summary>
    [HttpPost("invitations/accept")]
    public async Task<ActionResult> AcceptInvitation([FromBody] AcceptInviteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Token)) return BadRequest("Token is required.");
        var email = User.FindFirstValue(ClaimTypes.Email)
            ?? User.FindFirstValue("email")
            ?? User.FindFirstValue("primary_email") // Clerk custom claim
            ?? request?.Email?.Trim();
        if (string.IsNullOrWhiteSpace(email)) return BadRequest("User email not found. Sign in with email or pass email in the request.");
        var ok = await _teamService.AcceptInvitationAsync(request.Token, email);
        if (!ok) return BadRequest("Invalid or expired invitation, or email mismatch.");
        return Ok(new { success = true });
    }

    /// <summary>List members of a team (must be a member).</summary>
    [HttpGet("{id:guid}/members")]
    public async Task<ActionResult<IReadOnlyList<TeamMemberDto>>> ListMembers(Guid id)
    {
        var members = await _teamService.ListMembersAsync(id);
        if (members == null) return NotFound();
        return Ok(members);
    }
}

public class CreateTeamRequest
{
    public string Name { get; set; } = "";
}

public class InviteRequest
{
    public string Email { get; set; } = "";
}

public class AcceptInviteRequest
{
    public string Token { get; set; } = "";
    /// <summary>Current user's email (used when JWT has no email claim, e.g. Clerk default).</summary>
    public string? Email { get; set; }
}
