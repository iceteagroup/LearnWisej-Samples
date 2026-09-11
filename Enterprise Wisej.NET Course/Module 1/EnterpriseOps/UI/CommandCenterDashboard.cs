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
    /// EnterpriseOps — Command Center, the reference screen (ADR-001, docs/ReferenceScreen.md).
    ///
    /// This file owns UI state only (tiles, grid, banner, status bar). Every decision — who may look, what changed,
    /// what counts as "SLA at risk" — is made in EnterpriseOps.Services.Workflow.DashboardWorkflow.
    /// </summary>
    public partial class CommandCenterDashboard : Page, IWorkflowScreen
    {
        // Per-session services, wired in the constructor. Instance fields — never statics.
        private readonly SessionContext _session;
        private readonly ErrorLog _log;
        private readonly DashboardWorkflow _workflow;

        private CommandContext _current;

        /// <summary>Designer / default constructor: the default tenant and manager.</summary>
        public CommandCenterDashboard() : this(SessionContext.CreateDefault())
        {
        }

        public CommandCenterDashboard(SessionContext session)
        {
            InitializeComponent();

            _session = session;
            var trace = new ActivityTrace();
            _log = new ErrorLog(trace);
            _workflow = new DashboardWorkflow(
                new InMemoryWorkOrderRepository(trace),
                new OperationsFeed(trace),
                new ReleaseCalendar(trace),
                new DashboardPolicy(trace),
                trace);
        }

        /// <summary>The command running right now: same tenant and user, one correlation id per click.</summary>
        private CommandContext CurrentContext => _current ?? NewCommand();

        #region IWorkflowScreen

        public string ScreenName => "CommandCenterDashboard";

        /// <summary>First load: the workflow reads the store and the screen shows the result.</summary>
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

        public bool CanClose() => btnRefresh.Enabled;   // never close mid-refresh

        #endregion

        #region Event handlers

        private void CommandCenterDashboard_Load(object sender, EventArgs e)
        {
            lblUser.Text = $"Signed in: {_session.User.UserName} · {_session.User.Role}";
            LoadScreen();
        }

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
                ReportFailure(ex);
            }
            finally
            {
                EndBusy();
            }
        }

        #endregion

        #region Showing results

        /// <summary>The DashboardResult → tiles, grid, status bar. A denied result → amber banner, data untouched.</summary>
        private void ShowResult(DashboardResult result)
        {
            lblStatusBar.Text = $"{result.Summary} — EnterpriseOps.Services.Workflow · {result.ElapsedMs} ms";

            if (!result.Succeeded)
            {
                ShowBanner(string.Join(" ", result.Errors), warning: true);
                return;
            }

            kpiOpenIncidents.Value = result.Kpis.OpenIncidents.ToString();
            kpiSlaAtRisk.Value = result.Kpis.SlaAtRisk.ToString();
            kpiDeploymentsToday.Value = result.Kpis.DeploymentsToday.ToString();

            dgvIncidents.DataSource = result.Incidents;
            HighlightChangedRows(result);

            lblBanner.Visible = false;
        }

        /// <summary>Rows the feed changed in this refresh are painted green.</summary>
        private void HighlightChangedRows(DashboardResult result)
        {
            for (int i = 0; i < dgvIncidents.Rows.Count && i < result.Incidents.Count; i++)
            {
                bool changed = result.Incidents[i].ChangedByThisRefresh;
                dgvIncidents.Rows[i].DefaultCellStyle.BackColor = changed ? System.Drawing.Color.FromArgb(240, 249, 243) : System.Drawing.Color.White;
                dgvIncidents.Rows[i].DefaultCellStyle.ForeColor = changed ? System.Drawing.Color.FromArgb(15, 122, 58) : System.Drawing.Color.FromArgb(31, 45, 58);
            }
        }

        /// <summary>Unexpected failure: log with the correlation id, tell the user something generic, keep the screen usable.</summary>
        private void ReportFailure(Exception ex)
        {
            _log.Error(ex, CurrentContext.CorrelationId);
            ShowBanner(UiText.ActionFailed + $"  (ref {CurrentContext.CorrelationId})", warning: false);
            lblStatusBar.Text = $"Refresh failed — ref {CurrentContext.CorrelationId}";
            AlertBox.Show(UiText.ActionFailed, MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion

        #region Helpers

        private CommandContext NewCommand()
        {
            _current = _session.BeginCommand();
            return _current;
        }

        /// <summary>A new command starts: fresh correlation id, Refresh disabled until it completes.</summary>
        private void BeginBusy(string text)
        {
            NewCommand();
            btnRefresh.Enabled = false;
            lblStatusBar.Text = text;
        }

        /// <summary>After the awaits: Refresh back on, push the pending changes to the browser.</summary>
        private void EndBusy()
        {
            btnRefresh.Enabled = true;
            Application.Update(this);
        }

        private void ShowBanner(string text, bool warning)
        {
            lblBanner.Text = text;
            lblBanner.BackColor = warning ? System.Drawing.Color.FromArgb(255, 244, 229) : System.Drawing.Color.FromArgb(253, 236, 234);
            lblBanner.ForeColor = warning ? System.Drawing.Color.FromArgb(146, 64, 14) : System.Drawing.Color.FromArgb(178, 59, 39);
            lblBanner.Visible = true;
        }

        #endregion
    }
}
