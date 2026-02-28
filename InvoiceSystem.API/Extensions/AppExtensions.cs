using InvoiceSystem.Infrastructure;
using InvoiceSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InvoiceSystem.API.Extensions
{
    public static class AppExtensions
    {
        /// <summary>Applies pending EF migrations. Run in background so health check can pass quickly.</summary>
        public static void EnsureMigrationsApplied(this IApplicationBuilder app)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(500);
                    using var scope = app.ApplicationServices.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    await context.Database.MigrateAsync();
                }
                catch (Exception ex)
                {
                    using var logScope = app.ApplicationServices.CreateScope();
                    var logger = logScope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Migration failed");
                }
            });
        }

        public static void SeedDatabase(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }
        }
    }
}
