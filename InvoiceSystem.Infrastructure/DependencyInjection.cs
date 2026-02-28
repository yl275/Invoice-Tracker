using InvoiceSystem.Application.Interfaces.Repositories;
using InvoiceSystem.Infrastructure.Data;
using InvoiceSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();

            return services;
        }
    }
}
