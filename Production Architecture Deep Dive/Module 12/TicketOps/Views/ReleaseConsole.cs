using System;
using System.Threading.Tasks;
using TicketOps.Controls;
using TicketOps.Data;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using TicketOps.Services;
using Wisej.Web;

namespace TicketOps.Views
{
    /// <summary>
    /// TicketOps Console · Module 12 — the capstone's release console.
    ///
    /// Left card:   Diagnostics/DiagnosticsPage — the role-protected diagnostics page (runtime facts, live health
    ///              check, the health JSON a probe would read) with its Refresh.
    /// Right card:  the activity trace: every click travels UI → SVC → INFRA / DATA / SESSION and back.
    /// Bottom bar:  success path (run the health check), progress path (simulated load: 40 work units), two
    ///              failure paths (Technician → access denied; a Degraded dependency → HTTP 200 but reported),
    ///              the error path (repository outage → Unhealthy, HTTP 503) with its recovery, and Clear trace.
    ///
    /// Handlers read like sentences: ask the service, show the result. The role check, the probes and the
    /// Healthy/Degraded/Unhealthy rule live in Services and Domain — the words "sql01" and "Supervisor" appear
    /// in this file only as text the services already decided. "async void" handlers own their try/catch: an
    /// exception is logged with its details and the user sees the safe message from Resources.Strings.
    /// </summary>
    public partial class ReleaseConsole : Form
    {
        private const int LoadTestUnits = 40;
        private const int LoadUnitsPerTick = 4;
        private const string DegradedDependency = "storage";

        private readonly IDiagnosticsService _diagnostics;
        private readonly IHealthCheckService _health;
        private readonly ILoadTestService _loadTest;
        private readonly ITicketService _tickets;
        private readonly InMemoryTicketRepository _repository;   // only for the lab's outage switch
        private readonly SessionUserContext _user;               // only for the lab's sign-in switch
        private readonly ILog _log;

        // The Designer keeps the parameterless constructor; real wiring goes through the other one.
        public ReleaseConsole() : this(null, null, null, null, null, null, new ActivityLog())
        {
        }

        public ReleaseConsole(IDiagnosticsService diagnostics, IHealthCheckService health, ILoadTestService loadTest,
                              ITicketService tickets, InMemoryTicketRepository repository, SessionUserContext user, ILog log)
        {
            InitializeComponent();

            _diagnostics = diagnostics;
            _health = health;
            _loadTest = loadTest;
            _tickets = tickets;
            _repository = repository;
            _user = user;
            _log = log;

            if (log is ActivityLog activityLog)
                this.tracePanel.Attach(activityLog);
        }

        #region Screen lifecycle

        private void ReleaseConsole_Load(object sender, EventArgs e)
        {
            // Application.IsWebSocket is still false while Load runs, so the first health check would report the
            // websocket dependency Degraded. A one-shot Timer defers it to the next client round-trip.
            _log.Info(LogLayer.UI, "ReleaseConsole.Load", "screen shown — first refresh deferred 900 ms (Application.IsWebSocket is false during Load)");
            this.statusBanner.SetStatus("starting", StatusKind.Busy);
            this.timerFirstRefresh.Start();
        }

        private async void timerFirstRefresh_Tick(object sender, EventArgs e)
        {
            this.timerFirstRefresh.Stop();
            await RefreshDiagnosticsAsync("ReleaseConsole.timerFirstRefresh_Tick");
        }

        /// <summary>UI → SVC → INFRA / DATA → UI: the only place that asks for a diagnostics snapshot.</summary>
        private async Task<bool> RefreshDiagnosticsAsync(string source)
        {
            try
            {
                this.statusBanner.SetStatus("checking", StatusKind.Busy);
                _log.Info(LogLayer.UI, source, "→ IDiagnosticsService.GetSnapshotAsync()");
                var result = await _diagnostics.GetSnapshotAsync();          // role check + runtime facts + health check
                ShowSnapshot(result);                                          // data → UI
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                ReportFailure(source, ex);
                return false;
            }
        }

        #endregion

        #region data → UI (show the result)

        private void ShowSnapshot(OperationResult<DiagnosticsSnapshot> result)
        {
            if (!result.Succeeded)
            {
                // Expected outcome: the service said no in words the user may read; nothing else is revealed.
                this.diagnosticsPage.ShowAccessDenied(_user.UserName, _user.Role, result.Message);
                this.statusBanner.ShowBanner(result.Message, StatusKind.Warning);
                this.statusBanner.SetStatus("access denied", StatusKind.Warning);
                _log.Warn(LogLayer.UI, "ReleaseConsole.ShowSnapshot", $"FAIL · {result.Message}");
                return;
            }

            var snapshot = result.Value;
            this.diagnosticsPage.ShowSnapshot(snapshot);
            ShowHealthStatus(snapshot.Health);
            _log.Info(LogLayer.UI, "ReleaseConsole.ShowSnapshot",
                $"{snapshot.ServerName}:{snapshot.ServerPort} · {snapshot.SessionCount} session(s) · WebSocket {(snapshot.IsWebSocket ? "yes" : "no")} · {snapshot.Health.Status} · HTTP {snapshot.Health.HttpStatusCode}");
        }

        private void ShowHealth(HealthReport report)
        {
            this.diagnosticsPage.ShowHealth(report);
            ShowHealthStatus(report);
            _log.Info(LogLayer.UI, "ReleaseConsole.ShowHealth", $"{report.Status} · HTTP {report.HttpStatusCode} shown; the JSON below is the report serialized");
        }

        /// <summary>The status strip mirrors what the load balancer would do with this report.</summary>
        private void ShowHealthStatus(HealthReport report)
        {
            switch (report.Status)
            {
                case HealthStatus.Healthy:
                    this.statusBanner.HideBanner();
                    this.statusBanner.SetStatus("Healthy · HTTP 200 · in rotation", StatusKind.Success);
                    break;

                case HealthStatus.Degraded:
                    this.statusBanner.ShowBanner(Strings.HealthDegraded, StatusKind.Warning);
                    this.statusBanner.SetStatus("Degraded · HTTP 200 · still serving", StatusKind.Warning);
                    break;

                default:
                    this.statusBanner.ShowBanner("✖ " + Strings.HealthUnhealthy, StatusKind.Error);
                    this.statusBanner.SetStatus("Unhealthy · HTTP 503 · out of rotation", StatusKind.Error);
                    break;
            }
        }

        /// <summary>
        /// Unexpected failure: details go to the log (with the exception type and message),
        /// the user sees one generic sentence. Nothing internal leaks through the banner.
        /// </summary>
        private void ReportFailure(string source, Exception ex)
        {
            _log.Error(LogLayer.UI, source, ex, $"caught {ex.GetType().Name} — user sees the safe message");
            this.statusBanner.ShowBanner("✖ " + Strings.ActionFailed, StatusKind.Error);
            this.statusBanner.SetStatus("failed", StatusKind.Error);
            AlertBox.Show(Strings.ActionFailed, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion

        #region Thin handlers

        /// <summary>The page's Refresh: the full diagnostics snapshot (role check included).</summary>
        private async void diagnosticsPage_RefreshRequested(object sender, EventArgs e)
        {
            await RefreshDiagnosticsAsync("ReleaseConsole.diagnosticsPage_RefreshRequested");
        }

        /// <summary>Success path: the health check alone — what a probe of this node learns.</summary>
        private async void buttonHealth_Click(object sender, EventArgs e)
        {
            try
            {
                _log.Info(LogLayer.UI, "ReleaseConsole.buttonHealth_Click", "→ IHealthCheckService.CheckAsync() (HealthCheck.json + live probes)");
                var report = await _health.CheckAsync();                     // the probes and the rule live in the service
                ShowHealth(report);                                            // data → UI
            }
            catch (Exception ex)
            {
                ReportFailure("ReleaseConsole.buttonHealth_Click", ex);
            }
        }

        #endregion

        #region Bottom bar: progress, failures, outage and recovery

        /// <summary>Progress path: a Timer opens four work units per tick through ILoadTestService; the count climbs.</summary>
        private void buttonLoad_Click(object sender, EventArgs e)
        {
            if (this.timerLoad.Enabled)
                return;

            try
            {
                _loadTest.Start(LoadTestUnits);
                this.diagnosticsPage.ShowLoad(0, LoadTestUnits);
                this.statusBanner.HideBanner();
                this.statusBanner.SetStatus($"load test 0/{LoadTestUnits}", StatusKind.Busy);
                _log.Info(LogLayer.UI, "ReleaseConsole.buttonLoad_Click", $"{LoadUnitsPerTick} work units per tick — the handler stays thin, the Timer paces it");
                this.timerLoad.Start();
            }
            catch (Exception ex)
            {
                ReportFailure("ReleaseConsole.buttonLoad_Click", ex);
            }
        }

        private async void timerLoad_Tick(object sender, EventArgs e)
        {
            try
            {
                int open = _loadTest.OpenUnits;
                for (int i = 0; i < LoadUnitsPerTick && !_loadTest.IsComplete; i++)
                    open = await _loadTest.OpenWorkUnitAsync();

                this.diagnosticsPage.ShowLoad(open, _loadTest.Target);
                this.statusBanner.SetStatus($"load test {open}/{_loadTest.Target}", StatusKind.Busy);

                if (_loadTest.IsComplete)
                {
                    this.timerLoad.Stop();
                    _log.Info(LogLayer.UI, "ReleaseConsole.timerLoad_Tick", $"{open} work units open → refresh diagnostics (expect database OK with the new rows, node still Healthy)");
                    await RefreshDiagnosticsAsync("ReleaseConsole.timerLoad_Tick");
                }
            }
            catch (Exception ex)
            {
                this.timerLoad.Stop();
                this.diagnosticsPage.ResetLoad();
                ReportFailure("ReleaseConsole.timerLoad_Tick", ex);
            }
        }

        /// <summary>Failure path 1 (permission): a Technician asks for diagnostics; the service says no. Click again to sign back in.</summary>
        private async void buttonTechnician_Click(object sender, EventArgs e)
        {
            try
            {
                bool asTechnician = _user.Role == OperatorRole.Supervisor;
                if (asTechnician)
                    _user.SignInAs("l.romero", OperatorRole.Technician);
                else
                    _user.SignInAs("m.weber", OperatorRole.Supervisor);

                this.buttonTechnician.Text = asTechnician ? "Sign back in as Supervisor" : "Sign in as Technician";
                _log.Info(LogLayer.UI, "ReleaseConsole.buttonTechnician_Click", asTechnician
                    ? "Technician → refresh (expect ⚠ access denied from IDiagnosticsService, no runtime facts shown)"
                    : "Supervisor → refresh (recovery: the diagnostics page is visible again)");
                await RefreshDiagnosticsAsync("ReleaseConsole.buttonTechnician_Click");
            }
            catch (Exception ex)
            {
                ReportFailure("ReleaseConsole.buttonTechnician_Click", ex);
            }
        }

        /// <summary>Failure path 2 (dependency): "storage" reports Degraded — the node keeps serving and the app keeps working. Click again to restore.</summary>
        private async void buttonDegrade_Click(object sender, EventArgs e)
        {
            try
            {
                bool degrade = !_health.IsOverridden(DegradedDependency);
                if (degrade)
                    _health.OverrideDependency(DegradedDependency, DependencyStatus.Degraded, "disk free 9% (threshold 15%)");
                else
                    _health.ClearOverride(DegradedDependency);

                this.buttonDegrade.Text = degrade ? "Restore 'storage'" : "Degrade 'storage'";
                _log.Info(LogLayer.UI, "ReleaseConsole.buttonDegrade_Click", degrade
                    ? "storage Degraded → refresh (expect Degraded · HTTP 200; the app keeps working)"
                    : "storage restored → refresh (expect Healthy · HTTP 200)");

                bool shown = await RefreshDiagnosticsAsync("ReleaseConsole.buttonDegrade_Click");
                if (shown && degrade)
                {
                    // Proof the node still serves users while Degraded: an ordinary ticket workflow goes through.
                    var open = await _tickets.GetOpenTicketsAsync();
                    _log.Info(LogLayer.UI, "ReleaseConsole.buttonDegrade_Click", $"app keeps working while Degraded: {open.Count} open tickets loaded through ITicketService");
                    this.statusBanner.SetStatus($"Degraded · HTTP 200 · still serving ({open.Count} open tickets loaded)", StatusKind.Warning);
                }
            }
            catch (Exception ex)
            {
                ReportFailure("ReleaseConsole.buttonDegrade_Click", ex);
            }
        }

        /// <summary>Error path + recovery: the repository throws like a real driver → database Unhealthy, HTTP 503. Click again to recover.</summary>
        private async void buttonOutage_Click(object sender, EventArgs e)
        {
            try
            {
                _repository.SimulateOutage = !_repository.SimulateOutage;
                this.buttonOutage.Text = _repository.SimulateOutage ? "Recover the repository" : "Simulate repository outage";
                _log.Info(LogLayer.UI, "ReleaseConsole.buttonOutage_Click", _repository.SimulateOutage
                    ? "outage ON → refresh (expect ✖ in DATA, database Unhealthy, HTTP 503, safe message in UI)"
                    : "outage OFF → refresh (recovery: Healthy · HTTP 200)");
                await RefreshDiagnosticsAsync("ReleaseConsole.buttonOutage_Click");
            }
            catch (Exception ex)
            {
                ReportFailure("ReleaseConsole.buttonOutage_Click", ex);
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.tracePanel.ClearTrace();
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus("ready", StatusKind.Normal);
        }

        #endregion
    }
}
