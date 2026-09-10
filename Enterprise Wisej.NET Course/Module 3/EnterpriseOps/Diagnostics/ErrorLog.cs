using System;
using System.Collections.Generic;

namespace EnterpriseOps.Diagnostics
{
    /// <summary>
    /// Where unexpected exceptions go. The user gets a generic sentence plus the correlation id; the details
    /// stay here. Per session, like the trace — a shared log would need synchronization for no benefit, since
    /// the correlation id already ties a line to the request that produced it.
    /// </summary>
    public sealed class ErrorLog
    {
        private readonly ActivityTrace _trace;
        private readonly List<string> _entries = new List<string>();

        public ErrorLog(ActivityTrace trace)
        {
            _trace = trace;
        }

        public IReadOnlyList<string> Entries => _entries;

        /// <summary>Logs the exception with the correlation id of the command that failed.</summary>
        public void Error(Exception ex, string correlationId = null)
        {
            if (ex == null) return;

            string line = correlationId == null
                ? $"{ex.GetType().Name}: {ex.Message}"
                : $"[{correlationId}] {ex.GetType().Name}: {ex.Message}";

            _entries.Add(line);
            _trace?.Write("Diagnostics: ErrorLog ← " + line + " (details stay server-side; the user sees a generic message)");
        }
    }
}
