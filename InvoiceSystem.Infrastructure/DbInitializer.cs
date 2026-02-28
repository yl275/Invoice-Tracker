using Catamac.Domain.Entities;
using InvoiceSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InvoiceSystem.Infrastructure
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.Migrate();

            // Seed Clients
            if (!context.Clients.Any())
            {
                var clients = new Client[]
                {
                    new Client("ABN123456789", "Acme Corp", "0400111222"),
                    new Client("ABN987654321", "Globex Corporation", "0400333444"),
                    new Client("ABN112233445", "Soylent Corp", "0400555666"),
                    new Client("ABN998877665", "Initech", "0400777888")
                };
                context.Clients.AddRange(clients);
                context.SaveChanges();
            }

            // Seed Products
            if (!context.Products.Any())
            {
                var products = new Product[]
                {
                    new Product("Widget A", "WID-001", 10.50m),
                    new Product("Widget B", "WID-002", 25.00m),
                    new Product("Super Gadget", "GAD-999", 500.00m),
                    new Product("Flux Capacitor", "FLUX-001", 1000.00m),
                    new Product("Sonic Screwdriver", "SONIC-001", 120.00m)
                };
                context.Products.AddRange(products);
                context.SaveChanges();
            }

            // Seed Invoices
            if (!context.Invoices.Any())
            {
                var clients = context.Clients.ToList();
                var products = context.Products.ToList();

                if (clients.Any() && products.Any())
                {
                    var invoice1 = new Invoice("INV-1001", DateTime.UtcNow.AddDays(-10), clients[0]);
                    invoice1.AddItem(products[0], 5); // 5 * Widget A
                    invoice1.AddItem(products[1], 2); // 2 * Widget B

                    var invoice2 = new Invoice("INV-1002", DateTime.UtcNow.AddDays(-5), clients[1]);
                    invoice2.AddItem(products[2], 1); // 1 * Super Gadget

                    var invoice3 = new Invoice("INV-1003", DateTime.UtcNow.AddDays(-2), clients[2]);
                    invoice3.AddItem(products[3], 1); // 1 * Flux Capacitor
                    invoice3.AddItem(products[4], 3); // 3 * Sonic Screwdriver

                    context.Invoices.AddRange(invoice1, invoice2, invoice3);
                    context.SaveChanges();
                }
            }
        }
    }
}
