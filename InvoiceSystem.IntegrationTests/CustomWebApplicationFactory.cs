using System.Net.Http.Headers;
using InvoiceSystem.Application.Interfaces;
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

                var userContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IUserContext));
                if (userContextDescriptor != null) services.Remove(userContextDescriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                services.AddScoped<IClientRepository, ClientRepository>();
                services.AddScoped<IProductRepository, ProductRepository>();
                services.AddScoped<IInvoiceRepository, InvoiceRepository>();

                services.AddScoped<IUserContext, TestUserContext>();

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();
            });
        }

        protected override void ConfigureClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Add("X-User-Id", TestUserId);
            base.ConfigureClient(client);
        }
    }

    public class TestUserContext : IUserContext
    {
        public string? UserId => CustomWebApplicationFactory.TestUserId;
        public bool HasUser => true;
    }
}
