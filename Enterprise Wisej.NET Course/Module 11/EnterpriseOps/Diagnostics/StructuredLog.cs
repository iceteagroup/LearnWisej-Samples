using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace EnterpriseOps.Diagnostics
{
    public enum LogLevel { Information, Warning, Error }

    /// <summary>
    /// One structured entry: named fields, not a sentence. ToJsonLine() renders it the way a log shipper
    /// would send it; SafeSummary is the redacted one-liner the diagnostics page may show.
    /// </summary>
    public sealed class LogEntry
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        public DateTime TimestampUtc { get; init; }
        public LogLevel Level { get; init; }
        public string Operation { get; init; }
        public string CorrelationId { get; init; }
        public IReadOnlyDictionary<string, object> Fields { get; init; }

        public string ToJsonLine()
        {
            var ordered = new Dictionary<string, object>
            {
                ["ts"] = TimestampUtc.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture),
                ["level"] = Level.ToString().ToLowerInvariant(),
                ["op"] = Operation,
            };
            foreach (KeyValuePair<string, object> field in Fields)
                ordered[field.Key] = field.Value;
            ordered["correlation"] = CorrelationId;

            return JsonSerializer.Serialize(ordered, JsonOptions);
        }

        /// <summary>
        /// What the diagnostics page is allowed to show: time, operation, elapsed, level, correlation.
        /// Never an exception message, never a field value that could be data.
        /// </summary>
        public string SafeSummary
        {
            get
            {
                string elapsed = Fields.TryGetValue("elapsedMs", out object ms) ? $" {ms} ms" : "";
                return $"{TimestampUtc.ToString("HH:mm:ss", CultureInfo.InvariantCulture)} {Operation}{elapsed} · {Level.ToString().ToLowerInvariant()} · {CorrelationId}";
            }
        }
    }

    /// <summary>
    /// The in-memory structured log of this session. Bounded (200 entries) on purpose: the log must not become
    /// the leak it exists to catch. In production the same entries go to a sink; the shape is what matters.
    /// </summary>
    public sealed class StructuredLog
    {
        public const int Capacity = 200;

        /// <summary>Rough per-entry footprint for the session-memory audit.</summary>
        public const int ApproxBytesPerEntry = 480;

        private readonly List<LogEntry> _entries = new List<LogEntry>();

        public event Action<LogEntry> EntryWritten;

        public IReadOnlyList<LogEntry> Entries => _entries;

        public int ErrorCount => _entries.Count(e => e.Level == LogLevel.Error);

        public long EstimatedBytes => (long)_entries.Count * ApproxBytesPerEntry;

        public LogEntry Write(LogLevel level, string operation, string correlationId, object fields = null)
        {
            var entry = new LogEntry
            {
                TimestampUtc = DateTime.UtcNow,
                Level = level,
                Operation = operation,
                CorrelationId = correlationId,
                Fields = ToFields(fields),
            };

            _entries.Add(entry);
            if (_entries.Count > Capacity)
                _entries.RemoveAt(0);

            EntryWritten?.Invoke(entry);
            return entry;
        }

        /// <summary>
        /// Full detail on the server — type, message, and what the user was told — never on the screen.
        /// </summary>
        public LogEntry Error(string operation, string correlationId, Exception ex, object fields = null)
        {
            if (ex == null) throw new ArgumentNullException(nameof(ex));

            Dictionary<string, object> all = ToFields(fields);
            all["exceptionType"] = ex.GetType().Name;
            all["message"] = ex.Message;
            all["userMessage"] = SafeErrorMessage.For(correlationId);
            return Write(LogLevel.Error, operation, correlationId, all);
        }

        /// <summary>The last N entries, redacted, for DiagnosticSnapshot.SafeRecentEvents.</summary>
        public IReadOnlyList<string> SafeRecentEvents(int count)
        {
            return _entries.Skip(Math.Max(0, _entries.Count - count)).Select(e => e.SafeSummary).ToList();
        }

        public IEnumerable<LogEntry> Where(Func<LogEntry, bool> predicate) => _entries.Where(predicate);

        private static Dictionary<string, object> ToFields(object fields)
        {
            var result = new Dictionary<string, object>();
            if (fields == null)
                return result;

            if (fields is IEnumerable<KeyValuePair<string, object>> pairs)
            {
                foreach (KeyValuePair<string, object> pair in pairs)
                    result[pair.Key] = pair.Value;
                return result;
            }

            // Anonymous objects: one property = one field.
            foreach (PropertyInfo property in fields.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                result[property.Name] = property.GetValue(fields);

            return result;
        }
    }

    /// <summary>
    /// The message the user sees is a different thing from the log entry: what happened in plain words, the
    /// reference to quote, what to do next. Never the exception text, the SQL or a server path.
    /// </summary>
    public static class SafeErrorMessage
    {
        public static string For(string correlationId) =>
            $"The operation could not be completed. Reference {correlationId} — try again, or contact support and quote the reference.";
    }
}
