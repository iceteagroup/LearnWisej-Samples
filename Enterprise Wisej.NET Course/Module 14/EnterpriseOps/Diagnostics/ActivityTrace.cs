using System;
using System.Globalization;

namespace EnterpriseOps.Diagnostics
{
    /// <summary>
    /// The per-session server log. Each layer writes one line through it (<c>Service:</c>, <c>Security:</c>,
    /// <c>Data:</c>, <c>Job:</c>, <c>Review:</c>, <c>Docs:</c>) with a timestamp, and every line goes to
    /// <see cref="System.Diagnostics.Trace"/> — the debugger output locally, the configured trace listeners
    /// in production.
    /// </summary>
    public sealed class ActivityTrace
    {
        public void Add(string layer, string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            System.Diagnostics.Trace.WriteLine($"{time}  {layer,-9} {message}");
        }

        public void Service(string message) => Add("Service:", message);
        public void Security(string message) => Add("Security:", message);
        public void Data(string message) => Add("Data:", message);
        public void Job(string message) => Add("Job:", message);
        public void Review(string message) => Add("Review:", message);
        public void Docs(string message) => Add("Docs:", message);
    }
}
