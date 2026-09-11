using Microsoft.EntityFrameworkCore;
using SupportDesk.Data.Diagnostics;

namespace SupportDesk.Data;

/// <summary>
/// The Support Desk unit of work. One instance per operation: created from the
/// <see cref="IDbContextFactory{TContext}"/>, used for one query or one save, disposed before the
/// handler returns. It is never a session object, never a static, never shared between users.
/// </summary>
/// <remarks>
/// The constructor and Dispose report to <see cref="QueryTrace"/> so the tests can count the
/// contexts created and disposed inside one operation.
/// </remarks>
public sealed class SupportDeskContext : DbContext
{
    private readonly int _number = QueryTrace.NextContextNumber();

    public SupportDeskContext(DbContextOptions<SupportDeskContext> options) : base(options)
    {
        QueryTrace.Report(TraceKind.Context, $"#{_number} created (SupportDeskContext from the factory)");
    }

    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Module 1: just enough schema for the count. Module 2 replaces this with the full Fluent configuration.
        modelBuilder.Entity<Ticket>(b =>
        {
            b.Property(x => x.Number).HasMaxLength(20).IsRequired();
            b.Property(x => x.Title).HasMaxLength(180).IsRequired();
            b.Property(x => x.Status).HasMaxLength(30).IsRequired();
        });
    }

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
