using System;
using System.Collections.Generic;
using System.Globalization;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The per-session "Server · live activity trace". Services write their decisions here
    /// (<c>Service:</c>, <c>Security:</c>, <c>Data:</c>); the page writes <c>UI →</c> lines and the observer
    /// copies job history (<c>Job:</c>) into it. It lives in Application.Session, not in the page, so
    /// "Reopen page" replays it and the learner can see the page being recreated around a running job.
    /// </summary>
    public sealed class ActivityTrace
    {
        private readonly object _gate = new object();
        private readonly List<string> _lines = new List<string>();

        /// <summary>Raised on the thread that added the line (always a session thread in this sample).</summary>
        public event Action<string> LineAdded;

        public IReadOnlyList<string> Snapshot()
        {
            lock (_gate)
                return _lines.ToArray();
        }

        public void Add(string message)
        {
            string line = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture) + "  " + message;
            lock (_gate)
                _lines.Add(line);
            LineAdded?.Invoke(line);
        }

        public void Error(Exception ex) => Add($"ERROR  {ex.GetType().Name}: {ex.Message}");

        public void Clear()
        {
            lock (_gate)
                _lines.Clear();
        }
    }
}
