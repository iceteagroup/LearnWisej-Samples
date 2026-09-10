namespace SupportDesk.Data.Diagnostics;

public enum TraceKind
{
    /// <summary>A DbContext was created or disposed.</summary>
    Context,
    /// <summary>A command reached the database (text and duration).</summary>
    Command,
    /// <summary>A command failed.</summary>
    Failure,
    /// <summary>A note from a service about what it decided (which row it picked, what it expects the database to do).</summary>
    Note
}

public sealed record TraceEntry(TraceKind Kind, string Text, double? Milliseconds = null);

/// <summary>
/// Lab instrument: an <see cref="AsyncLocal{T}"/> sink that receives what EF Core does inside one UI
/// operation — every context created and disposed, every SQL statement with its duration — so the
/// running app can show the "one context per operation" rule instead of describing it.
/// </summary>
/// <remarks>
/// The scope flows with the async call chain (the page starts it, the interceptor inside the service
/// reports into it), which is exactly why nothing here is static per user: a static sink would mix
/// the sessions up. Production code uses ILogger / <c>LogTo</c> instead (Module 6 turns that on).
/// </remarks>
public static class QueryTrace
{
    private static readonly AsyncLocal<TraceScope?> Current = new();
    private static int _contextCounter;

    /// <summary>Starts a scope for the current async flow. Dispose it when the operation ends.</summary>
    public static TraceScope Begin(Action<TraceEntry> sink)
    {
        var scope = new TraceScope(sink, Current.Value);
        Current.Value = scope;
        return scope;
    }

    public static void Report(TraceKind kind, string text, double? milliseconds = null)
        => Current.Value?.Handle(new TraceEntry(kind, text, milliseconds));

    /// <summary>A service-side note for the trace ("customer #3 has 12 tickets — the DELETE goes to the database").</summary>
    public static void Note(string text) => Report(TraceKind.Note, text);

    public static int NextContextNumber() => Interlocked.Increment(ref _contextCounter);

    internal static void Restore(TraceScope? previous) => Current.Value = previous;
}

/// <summary>Counters for one operation: how many statements ran and how long the database spent on them.</summary>
public sealed class TraceScope : IDisposable
{
    private readonly Action<TraceEntry> _sink;
    private readonly TraceScope? _previous;

    internal TraceScope(Action<TraceEntry> sink, TraceScope? previous)
    {
        _sink = sink;
        _previous = previous;
    }

    public int Commands { get; private set; }
    public double Milliseconds { get; private set; }
    public int ContextsCreated { get; private set; }
    public int ContextsDisposed { get; private set; }

    internal void Handle(TraceEntry entry)
    {
        lock (this)
        {
            switch (entry.Kind)
            {
                case TraceKind.Command:
                    Commands++;
                    Milliseconds += entry.Milliseconds ?? 0;
                    break;
                case TraceKind.Context when entry.Text.Contains("created"):
                    ContextsCreated++;
                    break;
                case TraceKind.Context when entry.Text.Contains("disposed"):
                    ContextsDisposed++;
                    break;
            }
        }

        _sink(entry);
    }

    public void Dispose() => QueryTrace.Restore(_previous);
}
