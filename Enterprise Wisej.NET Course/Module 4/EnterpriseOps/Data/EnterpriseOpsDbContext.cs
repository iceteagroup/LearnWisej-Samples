using EnterpriseOps.Domain;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// The EF Core unit of work. Short-lived: one instance per command or per query, created by
    /// <see cref="SessionDatabase.CreateContext"/> and disposed when the operation ends. Never a field
    /// on a page, never static — see docs/DbContextLifetimeDecision.md and the "Wrong lifetime" button.
    /// </summary>
    public sealed class EnterpriseOpsDbContext : DbContext
    {
        public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
        public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();
        public DbSet<Tenant> Tenants => Set<Tenant>();

        /// <summary>Per-session sequence number, so the trace can say "DbContext #12 created / disposed".</summary>
        public int InstanceNumber { get; }

        public EnterpriseOpsDbContext(DbContextOptions<EnterpriseOpsDbContext> options, int instanceNumber)
            : base(options)
        {
            InstanceNumber = instanceNumber;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tenant>(e =>
            {
                e.ToTable("Tenants");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasMaxLength(32);
                e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<WorkOrder>(e =>
            {
                e.ToTable("WorkOrders");
                e.HasKey(x => x.Id);
                e.Property(x => x.TenantId).HasMaxLength(32).IsRequired();
                e.Property(x => x.Number).HasMaxLength(16).IsRequired();
                e.Property(x => x.Title).HasMaxLength(120).IsRequired();
                e.Property(x => x.Customer).HasMaxLength(80);
                e.Property(x => x.Site).HasMaxLength(80);
                e.Property(x => x.AssignedTo).HasMaxLength(40);
                e.Property(x => x.ApprovedBy).HasMaxLength(40);
                e.Property(x => x.ApprovalComment).HasMaxLength(400);
                e.Property(x => x.Status).HasConversion<string>().HasMaxLength(16);
                e.Property(x => x.Priority).HasConversion<string>().HasMaxLength(16);

                // The optimistic-concurrency token: UPDATE … WHERE Id = @id AND Version = @original.
                // Zero rows affected → DbUpdateConcurrencyException → ErrorMap → WO_CONCURRENCY.
                e.Property(x => x.Version).IsConcurrencyToken();

                // Business number unique per tenant: the UNIQUE-constraint failure path (WO_NUMBER_IN_USE).
                e.HasIndex(x => new { x.TenantId, x.Number }).IsUnique().HasDatabaseName("UX_WorkOrders_Tenant_Number");
                e.HasIndex(x => new { x.TenantId, x.Status });

                e.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AuditEntry>(e =>
            {
                e.ToTable("AuditEntries");
                e.HasKey(x => x.Id);
                e.Property(x => x.TenantId).HasMaxLength(32).IsRequired();
                e.Property(x => x.Action).HasMaxLength(16).IsRequired();
                e.Property(x => x.Outcome).HasMaxLength(16).IsRequired();
                e.Property(x => x.ErrorCode).HasMaxLength(32);
                e.Property(x => x.UserId).HasMaxLength(40);
                e.Property(x => x.CorrelationId).HasMaxLength(16);
                e.Property(x => x.Detail).HasMaxLength(400);
                e.HasIndex(x => new { x.TenantId, x.TimestampUtc });
            });
        }
    }
}
