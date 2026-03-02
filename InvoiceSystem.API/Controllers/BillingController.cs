using InvoiceSystem.Application.Interfaces;
using InvoiceSystem.Domain.Entities;
using InvoiceSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace InvoiceSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BillingController : ControllerBase
{
    private readonly IUserContext _userContext;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _dbContext;

    public BillingController(IUserContext userContext, IConfiguration configuration, ApplicationDbContext dbContext)
    {
        _userContext = userContext;
        _configuration = configuration;
        _dbContext = dbContext;
    }

    [HttpPost("create-checkout-session")]
    public ActionResult CreateCheckoutSession()
    {
        if (!_userContext.HasUser)
        {
            return Unauthorized();
        }

        var secretKey = _configuration["Stripe:SecretKey"];
        var priceId = _configuration["Stripe:ProPriceId"];
        var publicUrl = _configuration["PublicUrl"] ?? "http://localhost:5173";

        if (string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(priceId))
        {
            return StatusCode(500, "Stripe is not configured.");
        }

        StripeConfiguration.ApiKey = secretKey;

        var options = new SessionCreateOptions
        {
            Mode = "payment",
            SuccessUrl = $"{publicUrl}/?upgrade=success",
            CancelUrl = $"{publicUrl}/?upgrade=cancel",
            LineItems =
            [
                new SessionLineItemOptions
                {
                    Price = priceId,
                    Quantity = 1
                }
            ],
            Metadata = new Dictionary<string, string>
            {
                ["userId"] = _userContext.UserId!
            }
        };

        var service = new SessionService();
        var session = service.Create(options);

        return Ok(new { url = session.Url });
    }

    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        Event stripeEvent;
        var skipVerification = string.Equals(
            _configuration["Stripe:SkipWebhookVerification"],
            "true",
            StringComparison.OrdinalIgnoreCase);

        if (skipVerification)
        {
            stripeEvent = EventUtility.ParseEvent(json, throwOnApiVersionMismatch: false);
        }
        else
        {
            var endpointSecret = _configuration["Stripe:WebhookSecret"];
            if (string.IsNullOrWhiteSpace(endpointSecret))
            {
                return StatusCode(500, "Stripe webhook secret not configured.");
            }

            try
            {
                var signatureHeader = Request.Headers["Stripe-Signature"];
                stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, endpointSecret);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        if (stripeEvent.Type == "checkout.session.completed")
        {
            var session = stripeEvent.Data.Object as Session;
            var userId = session?.Metadata?["userId"];
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var existing = await _dbContext.UserSubscriptions
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (existing is null)
                {
                    var sub = new UserSubscription(userId, "pro");
                    await _dbContext.UserSubscriptions.AddAsync(sub);
                }
                else
                {
                    existing.ChangePlan("pro");
                }

                await _dbContext.SaveChangesAsync();
            }
        }

        return Ok();
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        if (!_userContext.HasUser)
        {
            return Unauthorized();
        }

        var userId = _userContext.UserId!;
        var subscription = await _dbContext.UserSubscriptions
            .FirstOrDefaultAsync(x => x.UserId == userId);

        var plan = subscription?.Plan ?? "free";
        var isPro = string.Equals(plan, "pro", StringComparison.OrdinalIgnoreCase);

        return Ok(new { plan, isPro });
    }
}

