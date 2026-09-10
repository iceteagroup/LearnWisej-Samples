using System;
using System.Globalization;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The live activity trace every layer writes to (per session — one instance per page, never static).
    /// The page subscribes to EntryAdded and appends the line to lstTrace; services never touch a control.
    /// The layer prefix is how the reviewer proves the handler was thin and the service decided.
    /// </summary>
    public sealed class ActivityTrace
    {
        public event Action<string> EntryAdded;

        public void Ui(string message) => Add("UI →       ", message);
        public void UiResult(string message) => Add("UI ←       ", message);
        public void Service(string message) => Add("Service:   ", message);
        public void Data(string message) => Add("Data:      ", message);
        public void Security(string message) => Add("Security:  ", message);
        public void Host(string message) => Add("Host:      ", message);          // Startup.cs / builder.Configuration facts
        public void Health(string message) => Add("Health:    ", message);        // HealthCheck.json / GET /healthz
        public void Balancer(string message) => Add("Balancer:  ", message);      // the (simulated) load balancer's decision
        public void Runbook(string message) => Add("Runbook:   ", message);       // one line per runbook step

        /// <summary>A null entry tells the page to clear the list.</summary>
        public void Clear() => EntryAdded?.Invoke(null);

        private void Add(string layer, string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            EntryAdded?.Invoke($"{time}  {layer} {message}");
        }
    }
}
