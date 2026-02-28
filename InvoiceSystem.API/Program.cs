using InvoiceSystem.API;
using InvoiceSystem.API.Extensions;
using InvoiceSystem.API.Middleware;
using InvoiceSystem.Application;
using InvoiceSystem.Application.Interfaces;
using InvoiceSystem.Infrastructure;
using InvoiceSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

// Allow UTC DateTimes to be saved to PostgreSQL timestamp columns
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Register Application and Infrastructure layers
var rawConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var connectionString = ConnectionStringHelper.ConvertPostgresUriToNpgsqlFormat(rawConnectionString);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddApplication();
if (builder.Environment.EnvironmentName != "Testing")
{
    builder.Services.AddInfrastructure(connectionString);
}

// Clerk JWT authentication (when configured)
var clerkIssuer = builder.Configuration["Clerk:Issuer"];
if (!string.IsNullOrEmpty(clerkIssuer))
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = clerkIssuer.TrimEnd('/') + "/";
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
}

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Run migrations in background (Production) so health check passes quickly
if (app.Environment.EnvironmentName != "Testing")
{
    app.EnsureMigrationsApplied();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Testing")
{
    app.UseDeveloperExceptionPage();
    if (app.Environment.IsDevelopment())
    {
        app.SeedDatabase();
    }
}

// Dev/Test bypass: when Clerk is not configured, accept X-User-Id header (default: user_demo)
if (string.IsNullOrEmpty(builder.Configuration["Clerk:Issuer"]) &&
    (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Testing"))
{
    app.UseMiddleware<DevUserBypassMiddleware>();
}
else if (!string.IsNullOrEmpty(clerkIssuer))
{
    app.UseAuthentication();
}

app.UseAuthorization();
app.UseCors();
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.WithTitle("Invoice System API")
           .WithTheme(ScalarTheme.DeepSpace)
           .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }
