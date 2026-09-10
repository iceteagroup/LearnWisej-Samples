using System;
using EnterpriseOps.Architecture;
using EnterpriseOps.Data;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Integrations;
using EnterpriseOps.Resources;
using EnterpriseOps.Security;
using EnterpriseOps.Services;
using EnterpriseOps.Services.Workflow;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// EnterpriseOps — Command Center. The reference screen of the course (ADR-001, docs/ReferenceScreen.md).
    ///
    /// Left card:   Refresh · three KpiTiles (Open incidents, SLA at risk, Deployments today) · dgvIncidents ·
    ///              the failure banner · the dark footer ("Loaded 12 incidents — EnterpriseOps.Services.Workflow · 41 ms").
    /// Right card:  the live activity trace — every layer's decision, tagged UI → / Security: / Integration: / Data: / Service:.
    /// Bottom bar:  the failure paths (ops feed down, technician denied, legacy handler through the review gate),
    ///              the recoveries (feed restored, back to the manager, this screen through the gate) and Clear trace.
    ///
    /// The boundary: this file owns UI state (labels, tiles, grid, banner). Every decision — who may look,
    /// what changed, what counts as "SLA at risk" — is made in EnterpriseOps.Services.Workflow.DashboardWorkflow.
    /// Every handler is a few lines and calls a service; the review gate below proves it on the real file.
    /// </summary>
    public partial class CommandCenterDashboard : Page, IWorkflowScreen
    {
        // Per-session services, wired in the constructor (the composition root until Module 3 adds a registry).
        // Instance fields — never statics: two users must never share a session, a feed or a trace.
        private readonly SessionContext _session;
        private readonly ActivityTrace _trace;
        private readonly ErrorLog _log;
        private readonly OperationsFeed _feed;
        private readonly DashboardWorkflow _workflow;
        private readonly ReviewGateService _gate;

        private CommandContext _current;

        /// <summary>Designer / default constructor: the default tenant and manager.</summary>
        public CommandCenterDashboard() : this(SessionContext.CreateDefault())
        {
        }

        public CommandCenterDashboard(SessionContext session)
        {
            InitializeComponent();

            _session = session;
            _trace = new ActivityTrace();
            _trace.EntryAdded += trace_EntryAdded;

            _log = new ErrorLog(_trace);
            _feed = new OperationsFeed(_trace);
            _workflow = new DashboardWorkflow(
                new InMemoryWorkOrderRepository(_trace),
                _feed,
                new ReleaseCalendar(_trace),
                new DashboardPolicy(_trace),
                _trace);
            _gate = new ReviewGateService(_trace);
        }

        /// <summary>The command running right now: same tenant and user, one correlation id per click.</summary>
        private CommandContext CurrentContext => _current ?? NewCommand();

        #region IWorkflowScreen

        public string ScreenName => "CommandCenterDashboard";

        /// <summary>First load: the workflow reads the store (no feed pull) and the screen shows the result.</summary>
        public async void LoadScreen()
        {
            BeginBusy("Loading…");
            try
            {
                var result = await _workflow.LoadAsync(CurrentContext);
                ShowResult(result);
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
            finally
            {
                EndBusy();
            }
        }

        public bool CanClose() => !btnRefresh.Enabled ? false : true;   // never close mid-refresh

        #endregion

        #region Event handlers — thin, one service call each (the shape the lab code check expects)

        private void CommandCenterDashboard_Load(object sender, EventArgs e)
        {
            var adr = DecisionLog.Adr001SolutionStructure;
            _trace.Architecture($"{adr.Id} {adr.Title} — accepted {adr.Date:yyyy-MM-dd} · owner {adr.Owner} · review {adr.ReviewDate:yyyy-MM-dd}");
            _trace.Ui($"{ScreenName}_Load → LoadScreen()  session={_session.Tenant.Id}/{_session.User.UserName}");
            lblTenant.Text = "tenant: " + _session.Tenant.Id;
            ShowSignedIn();
            LoadScreen();
        }

        /// <summary>The walkthrough's one-line handler: the workflow decides, the screen shows.</summary>
        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            BeginBusy(UiText.Refreshing);
            try
            {
                var result = await _workflow.RefreshAsync(CurrentContext);
                ShowResult(result);
            }
            catch (Exception ex)
            {
                _log.Error(ex, CurrentContext.CorrelationId);
                ShowFailure(UiText.ActionFailed);
                AlertBox.Show(UiText.ActionFailed, MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            }
            finally
            {
                EndBusy();
            }
        }

        /// <summary>Failure path 1: the integration goes dark; the next refresh throws and the catch above handles it.</summary>
        private void btnFeedDown_Click(object sender, EventArgs e)
        {
            _trace.Ui("btnFeedDown_Click → OperationsFeed.SetAvailable(false), then Refresh");
            _feed.SetAvailable(false);
            btnRefresh_Click(sender, e);
        }

        /// <summary>Recovery 1: the feed is back; the same Refresh handler succeeds again.</summary>
        private void btnFeedRestore_Click(object sender, EventArgs e)
        {
            _trace.Ui("btnFeedRestore_Click → OperationsFeed.SetAvailable(true), then Refresh");
            _feed.SetAvailable(true);
            btnRefresh_Click(sender, e);
        }

        /// <summary>Failure path 2 / recovery 2: refresh as the technician (policy denies), then back as the manager.</summary>
        private void btnSwitchUser_Click(object sender, EventArgs e)
        {
            UserIdentity next = _session.User.Role == UserRole.Technician ? KnownUsers.AnaOps : KnownUsers.BenTech;
            _trace.Ui($"btnSwitchUser_Click → SessionContext.SignInAs({next.UserName} · {next.Role}), then Refresh");
            _session.SignInAs(next);
            ShowSignedIn();
            btnRefresh_Click(sender, e);
        }

        /// <summary>Failure path 3: the 74-line btnSubmitOrder_Click with no service call — the gate blocks it.</summary>
        private async void btnGateLegacy_Click(object sender, EventArgs e)
        {
            BeginBusy(UiText.Reviewing);
            try
            {
                var report = await _gate.InspectAsync(SourceFiles.LegacyOrderEntry, CurrentContext);
                ShowGateReport(report);
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
            finally
            {
                EndBusy();
            }
        }

        /// <summary>Recovery 3: the same gate on this file — every handler small, every handler calls a service.</summary>
        private async void btnGateScreen_Click(object sender, EventArgs e)
        {
            BeginBusy(UiText.Reviewing);
            try
            {
                var report = await _gate.InspectAsync(SourceFiles.CommandCenterDashboard, CurrentContext);
                ShowGateReport(report);
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
            finally
            {
                EndBusy();
            }
        }

        private void btnClearTrace_Click(object sender, EventArgs e)
        {
            _trace.Clear();
        }

        #endregion

        #region Showing results — UI state only, no decisions

        /// <summary>The DashboardResult → tiles, grid, footer, status. A denied result → amber banner, data untouched.</summary>
        private void ShowResult(DashboardResult result)
        {
            lblCorrelation.Text = "corr " + result.CorrelationId;

            if (!result.Succeeded)
            {
                ShowBanner(string.Join(" ", result.Errors), BannerKind.Warning);
                SetStatus("not permitted", StatusKind.Warn);
                lblStatusBar.Text = $"{result.Summary} — EnterpriseOps.Services.Workflow · {result.ElapsedMs} ms";
                _trace.UiResult($"ShowResult: Succeeded=false → banner \"{result.Errors[0]}\"; tiles and grid unchanged");
                return;
            }

            kpiOpenIncidents.Value = result.Kpis.OpenIncidents.ToString();
            kpiSlaAtRisk.Value = result.Kpis.SlaAtRisk.ToString();
            kpiDeploymentsToday.Value = result.Kpis.DeploymentsToday.ToString();

            dgvIncidents.DataSource = result.Incidents;
            HighlightChangedRows(result);

            HideBanner();
            lblStatusBar.Text = $"{result.Summary} — EnterpriseOps.Services.Workflow · {result.ElapsedMs} ms";
            SetStatus(result.Summary.ToLowerInvariant(), StatusKind.Ok);
            _trace.UiResult($"ShowResult: tiles {result.Kpis.OpenIncidents}/{result.Kpis.SlaAtRisk}/{result.Kpis.DeploymentsToday}, grid {result.Incidents.Count} rows, footer \"{result.Summary}\"");
        }

        /// <summary>Rows the feed changed in this refresh are painted green (the walkthrough's "mitigated" row).</summary>
        private void HighlightChangedRows(DashboardResult result)
        {
            for (int i = 0; i < dgvIncidents.Rows.Count && i < result.Incidents.Count; i++)
            {
                bool changed = result.Incidents[i].ChangedByThisRefresh;
                dgvIncidents.Rows[i].DefaultCellStyle.BackColor = changed ? System.Drawing.Color.FromArgb(240, 249, 243) : System.Drawing.Color.White;
                dgvIncidents.Rows[i].DefaultCellStyle.ForeColor = changed ? System.Drawing.Color.FromArgb(15, 122, 58) : System.Drawing.Color.FromArgb(31, 45, 58);
            }
        }

        /// <summary>The review-gate report → red banner with the issues, or green banner "may merge".</summary>
        private void ShowGateReport(ReviewGateReport report)
        {
            lblCorrelation.Text = "corr " + report.CorrelationId;

            if (!report.FileFound)
            {
                ShowBanner(report.Errors[0], BannerKind.Warning);
                SetStatus("review gate: source not found", StatusKind.Warn);
                lblStatusBar.Text = "Review gate — source file not found";
                return;
            }

            if (report.Succeeded)
            {
                ShowBanner($"Review gate — {report.FileName} · {report.Handlers.Count} handlers · 0 issues · may merge", BannerKind.Success);
                SetStatus("review gate passed", StatusKind.Ok);
                lblStatusBar.Text = $"Review gate — {report.FileName} · {report.Handlers.Count} handlers · 0 issues · may merge";
                return;
            }

            var failing = report.Handlers.Find(h => h.Issues.Count > 0);
            ShowBanner($"Review gate — {failing.Name} · {report.TotalIssues} issues · {failing.Lines} lines · callsService: {failing.CallsService.ToString().ToLowerInvariant()} — caught before merge, not in production", BannerKind.Error);
            SetStatus($"review gate: {report.TotalIssues} issues on {failing.Name}", StatusKind.Error);
            lblStatusBar.Text = $"Review gate — {report.FileName} · {report.TotalIssues} issues · blocked before merge";
        }

        /// <summary>Unexpected failure: log with the correlation id, tell the user something generic, keep the screen usable.</summary>
        private void ReportFailure(Exception ex)
        {
            _log.Error(ex, CurrentContext.CorrelationId);
            ShowFailure(UiText.ActionFailed);
            AlertBox.Show(UiText.ActionFailed, MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        private void ShowFailure(string userMessage)
        {
            ShowBanner(userMessage + $"  (ref {CurrentContext.CorrelationId})", BannerKind.Error);
            SetStatus("failed — see the log", StatusKind.Error);
            lblStatusBar.Text = $"Refresh failed — EnterpriseOps.Services.Workflow · ref {CurrentContext.CorrelationId}";
            _trace.UiResult("ShowFailure: generic message + correlation id shown; exception details stayed in the log");
        }

        #endregion

        #region Helpers (small, reusable)

        private enum StatusKind { Ok, Warn, Error }
        private enum BannerKind { Success, Warning, Error }

        private CommandContext NewCommand()
        {
            _current = _session.BeginCommand();
            lblCorrelation.Text = "corr " + _current.CorrelationId;
            return _current;
        }

        /// <summary>A new command starts: fresh correlation id, buttons off, amber "busy" status. Sent before the first await.</summary>
        private void BeginBusy(string text)
        {
            NewCommand();
            SetButtons(false);
            SetStatus(text, StatusKind.Warn);
            lblStatusBar.Text = text;
        }

        /// <summary>After the awaits: buttons back on, push the pending changes (the request that started the handler is long gone).</summary>
        private void EndBusy()
        {
            SetButtons(true);
            Application.Update(this);
        }

        private void SetButtons(bool enabled)
        {
            btnRefresh.Enabled = enabled;
            btnFeedDown.Enabled = enabled;
            btnFeedRestore.Enabled = enabled;
            btnSwitchUser.Enabled = enabled;
            btnGateLegacy.Enabled = enabled;
            btnGateScreen.Enabled = enabled;
        }

        private void ShowSignedIn()
        {
            lblUser.Text = $"Signed in: {_session.User.UserName} · {_session.User.Role}";
            btnSwitchUser.Text = _session.User.Role == UserRole.Technician
                ? "Back to ana.ops (Manager)"
                : "Refresh as ben.tech (Technician)";
        }

        private void SetStatus(string text, StatusKind kind)
        {
            lblStatus.Text = "● " + text;
            lblStatus.ForeColor = kind switch
            {
                StatusKind.Error => System.Drawing.Color.FromArgb(224, 86, 59),
                StatusKind.Warn => System.Drawing.Color.FromArgb(232, 161, 60),
                _ => System.Drawing.Color.FromArgb(31, 157, 87),
            };
        }

        private void ShowBanner(string text, BannerKind kind)
        {
            lblBanner.Text = text;
            switch (kind)
            {
                case BannerKind.Success:
                    lblBanner.BackColor = System.Drawing.Color.FromArgb(233, 247, 238);
                    lblBanner.ForeColor = System.Drawing.Color.FromArgb(15, 122, 58);
                    break;
                case BannerKind.Warning:
                    lblBanner.BackColor = System.Drawing.Color.FromArgb(255, 244, 229);
                    lblBanner.ForeColor = System.Drawing.Color.FromArgb(146, 64, 14);
                    break;
                default:
                    lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
                    lblBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
                    break;
            }
            lblBanner.Visible = true;
        }

        private void HideBanner()
        {
            lblBanner.Visible = false;
        }

        /// <summary>The trace sink: one line per layer decision; null clears. The only place that knows about lstTrace.</summary>
        private void trace_EntryAdded(string line)
        {
            if (line == null)
            {
                lstTrace.Items.Clear();
                return;
            }
            lstTrace.Items.Add(line);
            lstTrace.SelectedIndex = lstTrace.Items.Count - 1;
        }

        #endregion
    }
}
