using InvoiceSystem.Application.Interfaces.Services;
using InvoiceSystem.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceSystem.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IInvoiceService, InvoiceService>();

            return services;
        }
    }
}
