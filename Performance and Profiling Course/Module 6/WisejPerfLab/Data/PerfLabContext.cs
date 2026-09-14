using Microsoft.EntityFrameworkCore;
using WisejPerfLab.Data.Entities;

namespace WisejPerfLab.Data
{
    /// <summary>
    /// The lab database. SQLite keeps the sample self-contained; everything the course teaches about
    /// query counts, projections and indexes is provider-independent (the SQL text differs, the
    /// statement count does not).
    /// </summary>
    /// <remarks>
    /// Module 1–5 deliberately shipped <b>without</b> an index on the columns the ticket search filters
    /// and orders by. Module 6 adds it — after the Database trace showed the scan, and not before.
    /// </remarks>
    public class PerfLabContext : DbContext
    {
        public PerfLabContext(DbContextOptions<PerfLabContext> options) : base(options)
        {
        }

        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(b =>
            {
                b.Property(c => c.Name).HasMaxLength(120).IsRequired();
                b.Property(c => c.Region).HasMaxLength(40);
                b.HasOne(c => c.Parent)
                    .WithMany(c => c.Children)
                    .HasForeignKey(c => c.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(c => c.ParentId);
            });

            modelBuilder.Entity<Ticket>(b =>
            {
                b.Property(t => t.Number).HasMaxLength(16).IsRequired();
                b.Property(t => t.Status).HasMaxLength(16).IsRequired();
                b.Property(t => t.Priority).HasMaxLength(16).IsRequired();
                b.Property(t => t.Subject).HasMaxLength(160);
                b.HasIndex(t => t.Number).IsUnique();

                // Module 6: the search filters on Status and orders by UpdatedAt, and the export and the
                // grid both page through that order. Without this index the statement is a table scan of
                // 50,000 rows for every page. Verify the filter and sort columns are indexed before
                // concluding that the remaining query time is inherent.
                b.HasIndex(t => new { t.Status, t.UpdatedAt }).HasDatabaseName("IX_Tickets_Status_UpdatedAt");
                b.HasOne(t => t.Customer)
                    .WithMany(c => c.Tickets)
                    .HasForeignKey(t => t.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
