using System;
using System.Collections.Generic;
using System.Globalization;

namespace EnterpriseOps.Diagnostics
{
    /// <summary>
    /// The per-session "Server · live activity trace". Every layer writes one line through it
    /// (<c>UI →</c>, <c>Service:</c>, <c>Security:</c>, <c>Data:</c>, <c>Job:</c>, <c>Review:</c>, <c>Docs:</c>)
    /// so the reviewer can prove the handler was thin and the service decided. Both screens render the
    /// same buffer, so navigating between them keeps the story.
    /// </summary>
    public sealed class ActivityTrace
    {
        private const int MaxLines = 400;
        private readonly List<string> _lines = new List<string>();

        public IReadOnlyList<string> Lines => _lines;

        /// <summary>Raised on the thread that wrote the line; screens append it to their ListBox.</summary>
        public event EventHandler<string> LineAdded;

        public void Add(string layer, string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            string line = $"{time}  {layer,-9} {message}";
            _lines.Add(line);
            if (_lines.Count > MaxLines)
                _lines.RemoveAt(0);
            LineAdded?.Invoke(this, line);
        }

        public void Ui(string message) => Add("UI →", message);
        public void Service(string message) => Add("Service:", message);
        public void Security(string message) => Add("Security:", message);
        public void Data(string message) => Add("Data:", message);
        public void Job(string message) => Add("Job:", message);
        public void Review(string message) => Add("Review:", message);
        public void Docs(string message) => Add("Docs:", message);

        public void Clear() => _lines.Clear();
    }
}
