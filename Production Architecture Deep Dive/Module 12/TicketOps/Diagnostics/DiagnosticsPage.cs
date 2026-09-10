using System;
using TicketOps.Controls;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Services;
using Wisej.Web;

namespace TicketOps.Diagnostics
{
    /// <summary>
    /// TicketOps — Diagnostics: the role-protected page a Supervisor reads at 2 a.m.
    /// Runtime card (server, port, runtime mode, product version, framework, sessions, WebSocket, uptime),
    /// health-check card (each dependency's live status, the aggregate and the HTTP code a probe would get)
    /// and the health JSON exactly as a /health endpoint would return it.
    ///
    /// Display only — like StatusBanner, it never decides anything: the Form asks IDiagnosticsService and
    /// hands the result here. Refresh raises <see cref="RefreshRequested"/>; the Form owns the handler.
    /// Nothing rendered here is a secret: no connection string, key, token or another user's data.
    /// </summary>
    public partial class DiagnosticsPage : UserControl
    {
        public event EventHandler RefreshRequested;

        public DiagnosticsPage()
        {
            InitializeComponent();
        }

        /// <summary>data → UI: the whole snapshot (Supervisor path).</summary>
        public void ShowSnapshot(DiagnosticsSnapshot snapshot)
        {
            if (snapshot == null) throw new ArgumentNullException(nameof(snapshot));

            this.labelAccess.Text = $"Role required: Supervisor · ✓ {snapshot.OperatorName} ({snapshot.OperatorRole})";
            this.labelAccess.ForeColor = StatusBanner.ColorFor(StatusKind.Success);

            this.valueServer.Text = snapshot.ServerName;
            this.valuePort.Text = snapshot.ServerPort.ToString();
            this.valueMode.Text = snapshot.RuntimeMode ? "release (RuntimeMode = true)" : "debug / design (RuntimeMode = false)";
            this.valueVersion.Text = snapshot.ProductVersion;
            this.valueFramework.Text = snapshot.Framework;
            this.valueSessions.Text = $"{snapshot.SessionCount} live on this node · this one {snapshot.ShortSessionId}";
            this.valueWebSocket.Text = snapshot.IsWebSocket ? "connected (server push on)" : "long-polling fallback";
            this.valueUptime.Text = $"{DiagnosticsService.FormatUptime(snapshot.Uptime)} · {snapshot.ServerTimeUtc:HH:mm:ss} UTC · {snapshot.TimeZoneId}";

            ShowHealth(snapshot.Health);
        }

        /// <summary>data → UI: the health part alone (the "Run health check" path).</summary>
        public void ShowHealth(HealthReport report)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));

            this.gridChecks.Rows.Clear();
            foreach (var check in report.Checks)
                this.gridChecks.Rows.Add(check.Name, check.Status.ToString(), check.Detail ?? check.Probe ?? "");
            this.gridChecks.ClearSelection();
            this.gridChecks.CurrentCell = null;

            this.labelOverall.Text =
                $"Status: {report.Status} → HTTP {report.HttpStatusCode} ({(report.IsServing() ? "in rotation" : "out of rotation")})\n" +
                $"{report.App} v{report.Version} · build {report.Build} · {report.Environment}";
            this.labelOverall.ForeColor = StatusBanner.ColorFor(KindFor(report.Status));

            this.textJson.Text = HealthCheckJson.Serialize(report);
            this.labelRefreshed.Text = $"refreshed {DateTime.Now:HH:mm:ss}";
        }

        /// <summary>The Technician path: the page says why, and shows nothing else.</summary>
        public void ShowAccessDenied(string userName, OperatorRole role, string message)
        {
            this.labelAccess.Text = $"Role required: Supervisor · ✖ {userName} ({role}) — access denied";
            this.labelAccess.ForeColor = StatusBanner.ColorFor(StatusKind.Error);

            foreach (var value in new[] { this.valueServer, this.valuePort, this.valueMode, this.valueVersion, this.valueFramework, this.valueSessions, this.valueWebSocket, this.valueUptime })
                value.Text = "—";

            this.gridChecks.Rows.Clear();
            this.labelOverall.Text = "Status: not shown to this role";
            this.labelOverall.ForeColor = StatusBanner.ColorFor(StatusKind.Error);
            this.textJson.Text = message;
            this.labelRefreshed.Text = $"refreshed {DateTime.Now:HH:mm:ss}";
        }

        /// <summary>Progress path: the simulated load test's count climbing.</summary>
        public void ShowLoad(int open, int target)
        {
            this.progressLoad.Maximum = Math.Max(1, target);
            this.progressLoad.Value = Math.Min(open, target);
            this.progressLoad.Visible = open < target;
            this.labelLoad.Text = open >= target
                ? $"load test: {open} work units opened on this node"
                : $"load test: {open}/{target} work units open…";
        }

        public void ResetLoad()
        {
            this.progressLoad.Visible = false;
            this.labelLoad.Text = "load test: idle";
        }

        public static StatusKind KindFor(HealthStatus status) => status switch
        {
            HealthStatus.Healthy => StatusKind.Success,
            HealthStatus.Degraded => StatusKind.Warning,
            _ => StatusKind.Error
        };

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            RefreshRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
