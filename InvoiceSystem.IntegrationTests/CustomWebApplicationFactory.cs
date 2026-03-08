using System.Net.Http.Headers;
using InvoiceSystem.Application.Interfaces;
using InvoiceSystem.Domain.Entities;
using InvoiceSystem.Application.Interfaces.Repositories;
using InvoiceSystem.Infrastructure.Data;
using InvoiceSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceSystem.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public const string TestUserId = "user_test";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                var dbDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (dbDescriptor != null) services.Remove(dbDescriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                services.AddScoped<IClientRepository, ClientRepository>();
                services.AddScoped<IProductRepository, ProductRepository>();
                services.AddScoped<IInvoiceRepository, InvoiceRepository>();
                services.AddScoped<ITeamRepository, TeamRepository>();

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    db.Database.EnsureCreated();
                    var team = Team.Create("Test", TestUserId);
                    db.Teams.Add(team);
                    db.SaveChanges();

                    var profile = new BusinessProfile(
                        TestUserId,
                        "Test Biz",
                        "test@test.local",
                        "0400000000",
                        "123 Test St",
                        null,
                        "ABN123456789",
                        "BankTransfer",
                        "123-456",
                        "987654321",
                        null);
                    db.BusinessProfiles.Add(profile);
                    db.SaveChanges();
                }
            });
        }

        protected override void ConfigureClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Add("X-User-Id", TestUserId);
            base.ConfigureClient(client);
        }
    }

}
