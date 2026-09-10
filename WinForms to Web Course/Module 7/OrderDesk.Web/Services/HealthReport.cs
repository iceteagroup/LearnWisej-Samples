using System;
using System.Reflection;
using OrderDesk.Domain;

namespace OrderDesk.Services
{
    /// <summary>
    /// The JSON returned by GET /health (Startup.cs). Built without a Wisej.NET session: a load balancer
    /// probes it, so it must not touch Application.* members that need a request context. Sessions come
    /// from <see cref="SessionRegistry"/> (Program.Main / ApplicationExit), orders from the shared store.
    /// </summary>
    public sealed class HealthReport
    {
        public string status { get; set; }
        public int sessions { get; set; }
        public int orders { get; set; }
        public string version { get; set; }
        public string framework { get; set; }
        public string machine { get; set; }
        public string os { get; set; }
        public string uptime { get; set; }
        public string storageRoot { get; set; }
        public string timestamp { get; set; }

        public static HealthReport Build()
        {
            var app = typeof(HealthReport).Assembly.GetName();
            var wisej = typeof(Wisej.Web.Application).Assembly.GetName();
            return new HealthReport
            {
                status = "ok",
                sessions = SessionRegistry.Count,
                orders = OrderStore.Shared().Count,
                version = "OrderDesk 7.0 (" + app.Name + " " + (app.Version?.ToString() ?? "1.0") + ")",
                framework = wisej.Name + " " + wisej.Version + " · " + System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
                machine = Environment.MachineName,
                os = System.Runtime.InteropServices.RuntimeInformation.OSDescription,
                uptime = SessionRegistry.Uptime.ToString(@"hh\:mm\:ss"),
                storageRoot = AppConfig.Effective("OrderDesk.StorageRoot", "App_Data") + (AppConfig.Override("OrderDesk.StorageRoot") != null ? " (ORDERDESK_STORAGEROOT)" : " (Web.config)"),
                timestamp = DateTime.UtcNow.ToString("o"),
            };
        }

        public string ToJson() => System.Text.Json.JsonSerializer.Serialize(this);
    }
}
