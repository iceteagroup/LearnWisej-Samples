using Microsoft.EntityFrameworkCore;
using SupportDesk.Data;

namespace SupportDesk.Services;

/// <summary>
/// Deliberately wrong code, kept out of the real services and named for what it is.
/// A static or shared DbContext ends up doing exactly this: two operations on one instance at the
/// same time, from two threads (two sessions, two clicks). EF Core refuses with
/// <see cref="InvalidOperationException"/> ("A second operation was started on this context instance
/// before a previous operation completed"). The context here is still created from the factory and
/// disposed — the anti-pattern is the sharing, not the lifetime.
/// </summary>
public sealed class SharedContextAntiPattern
{
    private const int MaxRounds = 25;
    private readonly IDbContextFactory<SupportDeskContext> _dbFactory;

    public SharedContextAntiPattern(IDbContextFactory<SupportDeskContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    /// <summary>The round in which the collision happened (or the number of rounds tried).</summary>
    public int LastRound { get; private set; }

    /// <summary>
    /// Runs pairs of concurrent counts on ONE context until EF Core detects the overlap.
    /// SQLite answers in a fraction of a millisecond, so a single pair can slip through; a few rounds
    /// make the collision reliable. Returns the number of rounds when no collision happened.
    /// </summary>
    public async Task<int> RunTwoOperationsOnOneContextAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        db.Tickets.Count();   // warm the context up so the collision is the classic "second operation" one

        for (LastRound = 1; LastRound <= MaxRounds; LastRound++)
        {
            var first = Task.Run(() => db.Tickets.Count());   // thread A: session 1 clicks Count
            var second = Task.Run(() => db.Tickets.Count());  // thread B: session 2 clicks Count — same instance
            await Task.WhenAll(first, second);                // throws InvalidOperationException when they overlap
        }

        return MaxRounds;
    }
}
