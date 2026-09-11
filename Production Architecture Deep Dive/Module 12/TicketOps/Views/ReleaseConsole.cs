using System;
using System.Threading.Tasks;
using TicketOps.Controls;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using TicketOps.Services;
using Wisej.Web;

namespace TicketOps.Views
{
    /// <summary>
    /// TicketOps Console · the capstone's release console: the role-protected diagnostics page
    /// (Diagnostics/DiagnosticsPage — runtime facts, live health check, the health JSON a probe would read).
    ///
    /// Handlers read like sentences: ask the service, show the result. The role check, the probes and the
    /// Healthy/Degraded/Unhealthy rule live in Services and Domain. "async void" handlers own their try/catch:
    /// an exception is logged with its details and the user sees the safe message from Resources.Strings.
    /// </summary>
    public partial class ReleaseConsole : Form
    {
        private readonly IDiagnosticsService _diagnostics;
        private readonly IUserContext _user;
        private readonly ILog _log;

        // The Designer keeps the parameterless constructor; real wiring goes through the other one.
        public ReleaseConsole() : this(null, null, new ActivityLog())
        {
        }

        public ReleaseConsole(IDiagnosticsService diagnostics, IUserContext user, ILog log)
        {
            InitializeComponent();

            _diagnostics = diagnostics;
            _user = user;
            _log = log;
        }

        private void ReleaseConsole_Load(object sender, EventArgs e)
        {
            // Application.IsWebSocket is still false while Load runs, so the first health check would report the
            // websocket dependency Degraded. A one-shot Timer defers it to the next client round-trip.
            this.statusBanner.SetStatus("starting", StatusKind.Busy);
            this.timerFirstRefresh.Start();
        }

        private async void timerFirstRefresh_Tick(object sender, EventArgs e)
        {
            this.timerFirstRefresh.Stop();
            await RefreshDiagnosticsAsync("ReleaseConsole.timerFirstRefresh_Tick");
        }

        /// <summary>The page's Refresh: the full diagnostics snapshot (role check included).</summary>
        private async void diagnosticsPage_RefreshRequested(object sender, EventArgs e)
        {
            await RefreshDiagnosticsAsync("ReleaseConsole.diagnosticsPage_RefreshRequested");
        }

        /// <summary>The only place that asks for a diagnostics snapshot.</summary>
        private async Task RefreshDiagnosticsAsync(string source)
        {
            try
            {
                this.statusBanner.SetStatus("checking", StatusKind.Busy);
                var result = await _diagnostics.GetSnapshotAsync();          // role check + runtime facts + health check
                ShowSnapshot(result);                                          // data → UI
            }
            catch (Exception ex)
            {
                ReportFailure(source, ex);
            }
        }

        private void ShowSnapshot(OperationResult<DiagnosticsSnapshot> result)
        {
            if (!result.Succeeded)
            {
                // Expected outcome: the service said no in words the user may read; nothing else is revealed.
                this.diagnosticsPage.ShowAccessDenied(_user.UserName, _user.Role, result.Message);
                this.statusBanner.ShowBanner(result.Message, StatusKind.Warning);
                this.statusBanner.SetStatus("access denied", StatusKind.Warning);
                return;
            }

            this.diagnosticsPage.ShowSnapshot(result.Value);
            ShowHealthStatus(result.Value.Health);
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

        /// <summary>Unexpected failure: the details go to the log, the user sees one generic sentence.</summary>
        private void ReportFailure(string source, Exception ex)
        {
            _log.Error(LogLayer.UI, source, ex);
            this.statusBanner.ShowBanner("✖ " + Strings.ActionFailed, StatusKind.Error);
            this.statusBanner.SetStatus("failed", StatusKind.Error);
            AlertBox.Show(Strings.ActionFailed, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }
    }
}
