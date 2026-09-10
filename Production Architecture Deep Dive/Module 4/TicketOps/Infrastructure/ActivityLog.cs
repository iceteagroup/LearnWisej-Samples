using System;
using System.Collections.Generic;

namespace TicketOps.Infrastructure
{
    /// <summary>One line of the activity trace.</summary>
    public sealed class ActivityEntry
    {
        public DateTime Time { get; init; }
        public LogLevel Level { get; init; }
        public LogLayer Layer { get; init; }
        public string Source { get; init; }
        public string Message { get; init; }
        public Exception Exception { get; init; }
    }

    /// <summary>
    /// Session-scoped, in-memory implementation of <see cref="ILog"/>.
    /// Plain C#: no Wisej.NET dependency, so services can be exercised without a control.
    /// The Diagnostics trace panel subscribes to <see cref="EntryAdded"/> to render entries live.
    /// Thread-safe, because background tasks (Module 7) log from worker threads.
    /// </summary>
    public sealed class ActivityLog : ILog
    {
        private readonly object _gate = new object();
        private readonly List<ActivityEntry> _entries = new List<ActivityEntry>();

        public event EventHandler<ActivityEntry> EntryAdded;

        public IReadOnlyList<ActivityEntry> Entries
        {
            get { lock (_gate) return _entries.ToArray(); }
        }

        public void Info(LogLayer layer, string source, string message) => Add(LogLevel.Info, layer, source, message, null);
        public void Warn(LogLayer layer, string source, string message) => Add(LogLevel.Warn, layer, source, message, null);

        public void Error(LogLayer layer, string source, Exception exception, string message = null)
            => Add(LogLevel.Error, layer, source, message ?? exception?.Message, exception);

        public void Clear()
        {
            lock (_gate) _entries.Clear();
        }

        private void Add(LogLevel level, LogLayer layer, string source, string message, Exception exception)
        {
            var entry = new ActivityEntry
            {
                Time = DateTime.Now,
                Level = level,
                Layer = layer,
                Source = source,
                Message = message,
                Exception = exception
            };
            lock (_gate) _entries.Add(entry);
            EntryAdded?.Invoke(this, entry);
        }
    }
}
