using InvoiceSystem.Domain.Entities;
using InvoiceSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InvoiceSystem.Infrastructure
{
    public static class DbInitializer
    {
        /// <summary>Demo user ID for development seed data. Real users get their Clerk sub (e.g. user_xxx).</summary>
        public const string DemoUserId = "user_demo";

        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.Migrate();

            // Seed demo data for development (bypass user filter)
            if (!context.Clients.IgnoreQueryFilters().Any())
            {
                var clients = new Client[]
                {
                    new Client(DemoUserId, "ABN123456789", "Acme Corp", "0400111222"),
                    new Client(DemoUserId, "ABN987654321", "Globex Corporation", "0400333444"),
                    new Client(DemoUserId, "ABN112233445", "Soylent Corp", "0400555666"),
                    new Client(DemoUserId, "ABN998877665", "Initech", "0400777888")
                };
                context.Clients.AddRange(clients);
                context.SaveChanges();
            }

            if (!context.Products.IgnoreQueryFilters().Any())
            {
                var products = new Product[]
                {
                    new Product(DemoUserId, "Widget A", "WID-001", 10.50m),
                    new Product(DemoUserId, "Widget B", "WID-002", 25.00m),
                    new Product(DemoUserId, "Super Gadget", "GAD-999", 500.00m),
                    new Product(DemoUserId, "Flux Capacitor", "FLUX-001", 1000.00m),
                    new Product(DemoUserId, "Sonic Screwdriver", "SONIC-001", 120.00m)
                };
                context.Products.AddRange(products);
                context.SaveChanges();
            }

            if (!context.BusinessProfiles.IgnoreQueryFilters().Any())
            {
                var demoProfile = new BusinessProfile(
                    DemoUserId,
                    "InvoiceSys Demo",
                    "form-InvoiceSys@yl1.org",
                    "0400000000",
                    "123 Demo Street, Sydney NSW 2000",
                    "https://invoicesys.local",
                    "ABN123456789",
                    "BankTransfer",
                    "123-456",
                    "987654321",
                    null
                );

                context.BusinessProfiles.Add(demoProfile);
                context.SaveChanges();
            }

            if (!context.Invoices.IgnoreQueryFilters().Any())
            {
                var clients = context.Clients.IgnoreQueryFilters().Where(c => c.UserId == DemoUserId).ToList();
                var products = context.Products.IgnoreQueryFilters().Where(p => p.UserId == DemoUserId).ToList();

                if (clients.Count > 0 && products.Count > 0)
                {
                    var invoice1 = new Invoice(DemoUserId, "INV-1001", DateTime.UtcNow.AddDays(-10), clients[0]);
                    invoice1.AddItem(products[0], 5);
                    invoice1.AddItem(products[1], 2);

                    var invoice2 = new Invoice(DemoUserId, "INV-1002", DateTime.UtcNow.AddDays(-5), clients[1]);
                    invoice2.AddItem(products[2], 1);

                    var invoice3 = new Invoice(DemoUserId, "INV-1003", DateTime.UtcNow.AddDays(-2), clients[2]);
                    invoice3.AddItem(products[3], 1);
                    invoice3.AddItem(products[4], 3);

                    context.Invoices.AddRange(invoice1, invoice2, invoice3);
                    context.SaveChanges();
                }
            }
        }
    }
}
