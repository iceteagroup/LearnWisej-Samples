using System;
using System.Globalization;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// Server-side activity log every layer writes to (per session — one instance per page, never static).
    /// Each line carries a timestamp and the layer prefix and is written to <see cref="System.Diagnostics.Trace"/>.
    /// </summary>
    public sealed class ActivityTrace
    {
        public void Service(string message) => Add("Service:   ", message);
        public void Security(string message) => Add("Security:  ", message);
        public void Host(string message) => Add("Host:      ", message);          // Startup.cs / builder.Configuration facts
        public void Health(string message) => Add("Health:    ", message);        // HealthCheck.json / GET /healthz
        public void Balancer(string message) => Add("Balancer:  ", message);      // the (simulated) load balancer's decision
        public void Runbook(string message) => Add("Runbook:   ", message);       // one line per runbook step

        private static void Add(string layer, string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            System.Diagnostics.Trace.WriteLine($"{time}  {layer} {message}");
        }
    }
}
