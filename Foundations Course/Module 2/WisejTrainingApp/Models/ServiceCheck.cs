namespace WisejTrainingApp.Models
{
    /// <summary>The three states a dashboard indicator can show. Each one has its own colour on screen.</summary>
    public enum ServiceState
    {
        Offline,    // red   — the service is not running
        Online,     // green — the service answered
        Degraded    // amber — the service was asked but the check failed (timeout)
    }

    /// <summary>
    /// The result of one service check — what <c>ServiceMonitor</c> returns and what the
    /// dashboard labels display. Plain data, no UI in here.
    /// </summary>
    public class ServiceCheck
    {
        public string Name { get; set; }          // "Server", "Database", "API Service"
        public ServiceState State { get; set; }
        public int LatencyMs { get; set; }        // only meaningful when Online
        public string Error { get; set; }         // only set when Degraded

        /// <summary>The text the indicator label shows: "Server: Online".</summary>
        public string DisplayText => $"{Name}: {State}";

        /// <summary>The fragment used in the "Status refreshed" log line: "Server 12 ms".</summary>
        public string Summary => State switch
        {
            ServiceState.Online => $"{Name} {LatencyMs} ms",
            ServiceState.Degraded => $"{Name} degraded",
            _ => $"{Name} offline",
        };
    }
}
