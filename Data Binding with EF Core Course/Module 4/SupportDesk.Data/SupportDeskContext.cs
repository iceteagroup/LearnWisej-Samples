using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SupportDesk.Data.Diagnostics;

namespace SupportDesk.Data;

/// <summary>
/// The Support Desk unit of work. One instance per operation: created from the
/// <see cref="IDbContextFactory{TContext}"/>, used for one query or one save, disposed before the
/// handler returns. It is never a session object, never a static, never shared between users.
/// </summary>
/// <remarks>
/// <para>
/// Module 2: the full model. Every rule that touches the schema — required strings and lengths, the
/// concurrency token, the planned indexes, the delete behaviour of each relationship and one CHECK
/// constraint — is configured here with the Fluent API, so the entity classes stay free of mapping
/// attributes and UI validation (Module 5) never mixes with persistence mapping.
/// </para>
/// <para>
/// The constructor and Dispose report to <see cref="QueryTrace"/> so the lab UI can show every
/// context being created and disposed inside one click — a teaching instrument, not a
/// production pattern.
/// </para>
/// </remarks>
public sealed class SupportDeskContext : DbContext
{
    /// <summary>The longest title the database accepts; the CHECK constraint and Module 5's validator share it.</summary>
    public const int TitleMaxLength = 180;

    private readonly int _number = QueryTrace.NextContextNumber();

    public SupportDeskContext(DbContextOptions<SupportDeskContext> options) : base(options)
    {
        QueryTrace.Report(TraceKind.Context, $"#{_number} created (SupportDeskContext from the factory)");
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Agent> Agents => Set<Agent>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketComment> TicketComments => Set<TicketComment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(b =>
        {
            b.Property(x => x.Name).HasMaxLength(160).IsRequired();
            b.Property(x => x.Email).HasMaxLength(240);
            b.HasIndex(x => x.Name);                                   // the customer ComboBox and the customer filter
        });

        modelBuilder.Entity<Agent>(b =>
        {
            b.Property(x => x.DisplayName).HasMaxLength(160).IsRequired();
            b.Property(x => x.Email).HasMaxLength(240);
        });

        modelBuilder.Entity<Category>(b =>
        {
            b.Property(x => x.Name).HasMaxLength(160).IsRequired();
        });

        modelBuilder.Entity<Ticket>(b =>
        {
            b.Property(x => x.Number).HasMaxLength(20).IsRequired();
            b.Property(x => x.Title).HasMaxLength(TitleMaxLength).IsRequired();
            b.Property(x => x.Description).HasMaxLength(4000);
            b.Property(x => x.Status).HasMaxLength(30).IsRequired();
            b.Property(x => x.Priority).HasMaxLength(30).IsRequired();

            // The concurrency token. IsRowVersion() = concurrency token + ValueGeneratedOnAddOrUpdate, which
            // on SQL Server maps to a server-generated rowversion column. SQLite has no such type, so this
            // context writes the token itself in SaveChanges (see StampTickets). For EF Core to send a value
            // we write, the save behaviours must be Save — by default a generated property is ignored on
            // both INSERT and UPDATE and read back from the database instead (which on SQLite would stay NULL).
            b.Property(x => x.RowVersion).IsRowVersion();
            b.Property(x => x.RowVersion).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Save);
            b.Property(x => x.RowVersion).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);

            // The planned indexes: each one serves a filter or a sort the ticket browser (Module 3) issues.
            b.HasIndex(x => new { x.Status, x.DueDate });               // the queue: WHERE Status = … ORDER BY DueDate
            b.HasIndex(x => x.CustomerId);                              // the customer filter
            b.HasIndex(x => x.UpdatedAt);                               // "recently changed" sort
            b.HasIndex(x => x.Number).IsUnique();                       // ticket numbers are unique (Module 5's duplicate path)

            // Relationships and what a delete does to them.
            b.HasOne(x => x.Customer).WithMany(c => c.Tickets)
                .HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);   // a customer with tickets cannot go
            b.HasOne(x => x.Agent).WithMany(a => a.Tickets)
                .HasForeignKey(x => x.AgentId).OnDelete(DeleteBehavior.SetNull);       // a departing agent unassigns their tickets
            b.HasOne(x => x.Category).WithMany(c => c.Tickets)
                .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);   // a category in use cannot go

            // SQLite does not enforce HasMaxLength (TEXT has no length). This CHECK makes the 180-character
            // rule real at the database layer, so a too-long title fails the same way it would on SQL Server.
            // The database constraint is the LAST line of defence; the friendly UI validation arrives in Module 5.
            b.ToTable(t => t.HasCheckConstraint("CK_Tickets_Title_Length", $"length(\"Title\") <= {TitleMaxLength}"));
        });

        modelBuilder.Entity<TicketComment>(b =>
        {
            b.Property(x => x.Body).HasMaxLength(4000).IsRequired();
            b.Property(x => x.Author).HasMaxLength(120).IsRequired();
            b.HasOne(x => x.Ticket).WithMany(t => t.Comments)
                .HasForeignKey(x => x.TicketId).OnDelete(DeleteBehavior.Cascade);      // comments die with their ticket
        });
    }

    #region RowVersion and UpdatedAt stamping (SQLite has no rowversion type)

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        StampTickets();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        StampTickets();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    /// <summary>
    /// Gives every inserted or updated ticket a fresh 16-byte token and stamps UpdatedAt on updates.
    /// The ORIGINAL value of RowVersion is untouched, so EF Core still adds
    /// <c>WHERE "Id" = @p AND "RowVersion" = @original</c> to the UPDATE/DELETE: a stale original
    /// matches zero rows and SaveChanges throws <see cref="DbUpdateConcurrencyException"/>.
    /// On SQL Server the rowversion column does this by itself and this method would be unnecessary.
    /// </summary>
    private void StampTickets()
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<Ticket>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(t => t.RowVersion).CurrentValue = NewToken();
                    break;
                case EntityState.Modified:
                    entry.Property(t => t.RowVersion).CurrentValue = NewToken();
                    entry.Property(t => t.UpdatedAt).CurrentValue = now;
                    break;
            }
        }
    }

    private static byte[] NewToken() => Guid.NewGuid().ToByteArray();

    #endregion

    public override void Dispose()
    {
        ReportDisposed();
        base.Dispose();
    }

    public override ValueTask DisposeAsync()
    {
        ReportDisposed();
        return base.DisposeAsync();
    }

    private void ReportDisposed()
    {
        var tracked = ChangeTracker.Entries().Count();
        QueryTrace.Report(TraceKind.Context, $"#{_number} disposed ({tracked} tracked entit{(tracked == 1 ? "y" : "ies")} released)");
    }
}
