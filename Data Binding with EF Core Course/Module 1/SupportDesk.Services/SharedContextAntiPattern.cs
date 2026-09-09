using Microsoft.EntityFrameworkCore;
using SupportDesk.Data;

namespace SupportDesk.Services;

/// <summary>
/// Deliberately wrong code, kept out of the real services and named for what it is.
/// A static or shared DbContext ends up doing exactly this: two operations on one instance at the
/// same time. EF Core refuses with <see cref="InvalidOperationException"/> ("A second operation was
/// started on this context instance before a previous operation completed"). The context here is
/// still created from the factory and disposed — the anti-pattern is the sharing, not the lifetime.
/// </summary>
public sealed class SharedContextAntiPattern
{
    private readonly IDbContextFactory<SupportDeskContext> _dbFactory;

    public SharedContextAntiPattern(IDbContextFactory<SupportDeskContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task RunTwoOperationsOnOneContextAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        Task<int>? first = null;
        try
        {
            first = db.Tickets.CountAsync();      // operation 1 starts and awaits the database
            var second = db.Tickets.CountAsync(); // operation 2 on the same instance while 1 is in flight
            await Task.WhenAll(first, second);
        }
        finally
        {
            // Let the in-flight operation finish before the context is disposed.
            if (first is not null)
            {
                try { await first; } catch { /* reported by the caller */ }
            }
        }
    }
}
