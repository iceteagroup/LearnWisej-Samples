using System;
using WisejTrainingApp.Models;

namespace WisejTrainingApp.Services
{
    /// <summary>
    /// C# owns the data and the rules. The page asks this service for a <see cref="StatusInfo"/>
    /// and forwards three display values to the widget; the rule that turns a load figure into
    /// "ok" / "warn" / "error" lives here, in one place, testable without a browser.
    ///
    /// One instance per StatusPage = per user session (an instance field on the page, never static).
    /// </summary>
    public class StatusService
    {
        // ------------------------------------------------------------------------------------------
        // Server-only values — the whole point of lab step 9.
        //
        // A real status service would call a monitoring API with a credential and read a database.
        // Both stay private fields of this C# class: they are never copied into StatusInfo, so they
        // can never end up in widStatus.Options. Anything assigned to Options reaches the browser
        // (view-source / DevTools show it), so the boundary is "only what the widget needs to draw".
        // The values below are fakes for the lab.
        // ------------------------------------------------------------------------------------------
        private readonly string ApiKey = "mon-live-4f9c-DEMO-NEVER-SENT";                                 // never sent
        private readonly string ConnectionString = "Server=db01;Database=OpsTickets;User Id=svc_ops;Password=demo-only"; // never sent

        /// <summary>The business rule: below this the status is "ok".</summary>
        public const int WarnFrom = 60;

        /// <summary>The business rule: above this the status is "error"; between is "warn".</summary>
        public const int ErrorAbove = 85;

        /// <summary>Deterministic load steps applied by <see cref="Refresh"/>, so every run reads the same.</summary>
        private static readonly int[] LoadSteps = { +7, +6, +9, +11, +12, -3, -31, +5 };

        private int _open = 12;
        private int _closed = 47;
        private int _systemLoad = 42;
        private int _refreshIndex;

        /// <summary>
        /// Lab prop: when true the next <see cref="GetStatus"/> throws (the monitoring API is "down"),
        /// then resets itself so the following call succeeds — the recovery path.
        /// </summary>
        public bool FailNextCall { get; set; }

        /// <summary>
        /// Reads the current status. Throws when the (simulated) monitoring API is unavailable —
        /// the page catches it, shows a safe message and keeps the widget's last values.
        /// </summary>
        public StatusInfo GetStatus()
        {
            // The credential and the connection string are used HERE, on the server, and nowhere else.
            CallMonitoringApi(ApiKey);
            ReadTicketCounters(ConnectionString);

            return Build();
        }

        /// <summary>
        /// Stands in for the HTTP call a real service would make. The credential never leaves this
        /// method; the only thing that comes out is a load figure (kept in memory for the lab).
        /// </summary>
        private void CallMonitoringApi(string credential)
        {
            if (string.IsNullOrEmpty(credential))
                throw new InvalidOperationException("Monitoring credential is not configured");

            if (FailNextCall)
            {
                FailNextCall = false;   // one failure, then the next call works again (the recovery)
                throw new InvalidOperationException(
                    "Monitoring API returned 503 Service Unavailable for https://monitor.internal/api/status");
            }
        }

        /// <summary>
        /// Stands in for the database query a real service would run for the ticket counters.
        /// </summary>
        private void ReadTicketCounters(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Ticket database connection is not configured");
        }

        /// <summary>
        /// Takes a new load sample (deterministic sequence) and moves the ticket counters a little,
        /// so "Refresh Server Data" visibly changes the C# data before the widget is refreshed.
        /// </summary>
        public StatusInfo Refresh()
        {
            // While the monitoring API is "down" no sample can be taken: the data stays as it was,
            // and the GetStatus() that follows reports the failure. Nothing is half-updated.
            if (FailNextCall)
                return Build();

            int step = LoadSteps[_refreshIndex % LoadSteps.Length];
            _refreshIndex++;
            _systemLoad = Clamp(_systemLoad + step);

            if (step > 0)
            {
                _open++;
            }
            else
            {
                _open = Math.Max(0, _open - 1);
                _closed++;
            }

            return Build();
        }

        /// <summary>Forces a load that the rule classifies as "ok" (35 %).</summary>
        public StatusInfo SetHealthy() => SetLoad(35);

        /// <summary>Forces a load that the rule classifies as "warn" (72 %).</summary>
        public StatusInfo SetWarning() => SetLoad(72);

        /// <summary>Forces a load that the rule classifies as "error" (93 %).</summary>
        public StatusInfo SetCritical() => SetLoad(93);

        /// <summary>
        /// THE rule. JavaScript never sees it: the widget only receives the resulting word
        /// and picks a colour class for it (lw-gauge-ok / -warn / -error).
        /// </summary>
        public static string StatusFor(int systemLoad)
        {
            if (systemLoad > ErrorAbove) return "error";
            if (systemLoad >= WarnFrom) return "warn";
            return "ok";
        }

        /// <summary>
        /// Names (never values) of what stays on the server — shown on the page's data card so the
        /// boundary is visible to the learner.
        /// </summary>
        public string[] DescribeServerOnly()
        {
            return new[]
            {
                "ApiKey            monitoring credential        → stays in C# (private field)",
                "ConnectionString  database connection string   → stays in C# (private field)",
                "raw exception     GetStatus() failure detail   → stays in C# (log); user sees a safe message",
            };
        }

        private StatusInfo SetLoad(int systemLoad)
        {
            _systemLoad = Clamp(systemLoad);
            return Build();
        }

        /// <summary>Builds the display object. Note what is NOT copied: ApiKey, ConnectionString.</summary>
        private StatusInfo Build()
        {
            return new StatusInfo
            {
                Percent = _systemLoad,
                Label = "System load",
                Status = StatusFor(_systemLoad),
                Open = _open,
                Closed = _closed,
                SystemLoad = _systemLoad,
            };
        }

        private static int Clamp(int value) => Math.Max(0, Math.Min(100, value));
    }
}
