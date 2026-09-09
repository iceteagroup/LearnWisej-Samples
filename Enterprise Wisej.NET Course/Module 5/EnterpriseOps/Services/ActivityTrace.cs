using System;
using System.Collections.Generic;
using System.Globalization;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The live activity trace every layer writes to: <c>UI →</c>, <c>Service:</c>, <c>Data:</c>,
    /// <c>Security:</c>, <c>Job:</c>, <c>Session:</c>. It belongs to the session (not the page), so a page
    /// rebuild replays the same lines and the reviewer can see which layer took each decision.
    /// </summary>
    public sealed class ActivityTrace
    {
        private readonly object _gate = new object();
        private readonly List<string> _lines = new List<string>();

        /// <summary>Raised after a line is added; the page appends it to <c>lstTrace</c>.</summary>
        public event Action<string> LineAdded;

        public IReadOnlyList<string> Lines
        {
            get { lock (_gate) return _lines.ToArray(); }
        }

        public void Write(string message)
        {
            string line = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture) + "  " + message;
            lock (_gate)
                _lines.Add(line);
            LineAdded?.Invoke(line);
        }

        public void Clear()
        {
            lock (_gate)
                _lines.Clear();
        }
    }
}
