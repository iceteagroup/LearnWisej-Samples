using System;
using System.Globalization;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The server-side log every layer writes through. Services, stores and security rules say which layer
    /// decided and what; the log stamps the time and the layer and writes the line to
    /// <see cref="System.Diagnostics.Trace"/>, where any configured listener (console, file, APM) picks it up.
    /// One instance per session, created by the <see cref="ServiceRegistry"/>.
    /// </summary>
    public sealed class ActivityTrace
    {
        public void Service(string message) => Write("Service:", message);
        public void Data(string message) => Write("Data:", message);
        public void Security(string message) => Write("Security:", message);
        public void Job(string message) => Write("Job:", message);

        private static void Write(string layer, string message)
        {
            string line = $"{DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)}  {layer,-9} {message}";
            System.Diagnostics.Trace.WriteLine(line);
        }
    }
}
