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
        public DbSet<BusinessProfile> BusinessProfiles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<TeamInvitation> TeamInvitations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Client
            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TeamId).IsRequired();
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Abn).IsRequired();
                entity.Property(e => e.Name).IsRequired();
                entity.HasMany(e => e.Invoices)
                      .WithOne(e => e.Client)
                      .HasForeignKey(e => e.ClientId);

                if (_userContext != null)
                    entity.HasQueryFilter(e => _userContext.TeamIds.Contains(e.TeamId)
                        && (_userContext.DataScope != "mine" || e.UserId == _userContext.UserId));
            });

            // Configure Product
            modelBuilder.Entity<BusinessProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.Phone).IsRequired();
                entity.Property(e => e.PostalLocation).IsRequired();
                entity.Property(e => e.Abn).IsRequired();
                entity.Property(e => e.PaymentMethod).IsRequired();
                entity.HasIndex(e => e.UserId).IsUnique();

                if (_userContext != null)
                    entity.HasQueryFilter(e => e.UserId == _userContext.UserId);
            });

            // Configure Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TeamId).IsRequired();
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");

                if (_userContext != null)
                    entity.HasQueryFilter(e => _userContext.TeamIds.Contains(e.TeamId)
                        && (_userContext.DataScope != "mine" || e.UserId == _userContext.UserId));
            });

            // Configure Invoice
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TeamId).IsRequired();
                entity.Property(e => e.UserId).IsRequired();
                entity.HasMany(e => e.Items)
                      .WithOne()
                      .HasForeignKey(e => e.InvoiceId);

                entity.Property(e => e.InvoiceDate).HasColumnType("timestamp without time zone");
                entity.Property(e => e.DueDate).HasColumnType("timestamp without time zone");

                if (_userContext != null)
                    entity.HasQueryFilter(e => _userContext.TeamIds.Contains(e.TeamId)
                        && (_userContext.DataScope != "mine" || e.UserId == _userContext.UserId));
            });

            // Configure InvoiceItem
            modelBuilder.Entity<InvoiceItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Total).HasColumnType("decimal(18,2)");
            });

            // Configure UserSubscription
            modelBuilder.Entity<UserSubscription>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Plan).IsRequired();
                entity.Property(e => e.PurchasedAt)
                      .HasColumnType("timestamp without time zone");

                entity.HasIndex(e => e.UserId).IsUnique();

                if (_userContext != null)
                    entity.HasQueryFilter(e => e.UserId == _userContext.UserId);
            });

            // Configure Team (use backing field so Add(team) persists Members, e.g. owner)
            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                entity.HasMany(e => e.Members)
                      .WithOne(e => e.Team)
                      .HasForeignKey(e => e.TeamId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Navigation(e => e.Members).HasField("_members");
            });

            // Configure TeamMember (no query filter - used to load user's teams)
            modelBuilder.Entity<TeamMember>(entity =>
            {
                entity.HasKey(e => new { e.TeamId, e.UserId });
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.JoinedAt).HasColumnType("timestamp without time zone");
            });

            // Configure TeamInvitation
            modelBuilder.Entity<TeamInvitation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TeamId).IsRequired();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.InvitedByUserId).IsRequired();
                entity.Property(e => e.Token).IsRequired();
                entity.Property(e => e.ExpiresAt).HasColumnType("timestamp without time zone");
                entity.HasOne(e => e.Team)
                      .WithMany()
                      .HasForeignKey(e => e.TeamId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
