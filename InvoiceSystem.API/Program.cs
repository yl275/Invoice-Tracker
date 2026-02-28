using InvoiceSystem.API.Extensions;
using InvoiceSystem.Application;
using InvoiceSystem.Infrastructure;
using InvoiceSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

// Allow UTC DateTimes to be saved to PostgreSQL timestamp columns
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Register Application and Infrastructure layers
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddApplication();
if (builder.Environment.EnvironmentName != "Testing")
{
    builder.Services.AddInfrastructure(connectionString);
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

app.UseCors();
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.WithTitle("Invoice System API")
           .WithTheme(ScalarTheme.DeepSpace)
           .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

app.UseAuthorization();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }
