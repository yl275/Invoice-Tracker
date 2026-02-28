using InvoiceSystem.Application.Interfaces;
using InvoiceSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvoiceSystem.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IUserContext? _userContext;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IUserContext userContext) : base(options)
        {
            _userContext = userContext;
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Client
            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Abn).IsRequired();
                entity.Property(e => e.Name).IsRequired();
                entity.HasMany(e => e.Invoices)
                      .WithOne(e => e.Client)
                      .HasForeignKey(e => e.ClientId);

                if (_userContext != null)
                    entity.HasQueryFilter(e => e.UserId == _userContext.UserId);
            });

            // Configure Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");

                if (_userContext != null)
                    entity.HasQueryFilter(e => e.UserId == _userContext.UserId);
            });

            // Configure Invoice
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.HasMany(e => e.Items)
                      .WithOne()
                      .HasForeignKey(e => e.InvoiceId);

                entity.Property(e => e.InvoiceDate).HasColumnType("timestamp without time zone");

                if (_userContext != null)
                    entity.HasQueryFilter(e => e.UserId == _userContext.UserId);
            });

            // Configure InvoiceItem
            modelBuilder.Entity<InvoiceItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Total).HasColumnType("decimal(18,2)");
            });
        }
    }
}
