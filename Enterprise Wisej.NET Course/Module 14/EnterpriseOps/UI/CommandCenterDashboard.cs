using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;
using EnterpriseOps.Services;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// EnterpriseOps — Command Center. The KPI cards a service computes, the tenant-scoped work queue, the
    /// approve command with its permission check and audit line, and the diagnostics card.
    ///
    /// This file owns UI state only. Which statuses count as "open", who may approve, what an audit line says
    /// and whether a write is stale are decided in <c>EnterpriseOps.Services</c>.
    /// </summary>
    public partial class CommandCenterDashboard : Page
    {
        // Per-session state and services — instance fields, never statics.
        private readonly SessionContext _session;
        private readonly ActivityTrace _trace;

        private CommandContext _current;
        private CancellationTokenSource _healthCancellation;
        private System.Collections.Generic.List<HealthProbe> _probes;
        private CapstoneReviewPage _capstoneReview;
        private bool _loading;

        /// <summary>Designer / default constructor: contoso, signed in as ana.ops.</summary>
        public CommandCenterDashboard() : this(SessionContext.CreateDefault())
        {
        }

        public CommandCenterDashboard(SessionContext session)
        {
            InitializeComponent();

            _session = session;
            _trace = session.Services.Trace;

            _loading = true;                    // setting the filter must not fire a refresh before Load
            cboStatus.SelectedIndex = 0;
            _loading = false;

            _probes = session.Services.Health.CreateProbes();
        }

        /// <summary>The command running right now: same tenant and user, one correlation id per click.</summary>
        private CommandContext CurrentContext => _current ?? NewCommand();

        #region Event handlers

        private async void CommandCenterDashboard_Load(object sender, EventArgs e)
        {
            lblUser.Text = $"Signed in: {_session.User.Name} · {_session.User.Role}";
            BindHealth();
            await RefreshAsync();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await RefreshAsync();
        }

        private async void cboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            await RefreshAsync();
        }

        private async void btnApprove_Click(object sender, EventArgs e)
        {
            WorkQueueRow row = SelectedRow();
            if (row == null)
            {
                Warn("Select a work order first.");
                return;
            }

            btnApprove.Enabled = false;
            ClearBanner();
            NewCommand();

            try
            {
                ApproveResult result = await _session.Services.WorkOrders.ApproveAsync(
                    new ApproveWorkOrderCommand { WorkOrderId = row.Id, ExpectedVersion = row.Version }, CurrentContext);

                ShowApproveResult(result);
                if (result.Succeeded)
                    await RefreshAsync();
            }
            catch (Exception ex)
            {
                ReportUnexpected(ex, "The approval could not be completed.");
            }
            finally
            {
                btnApprove.Enabled = CanApprove(SelectedRow());
                Application.Update(this);
            }
        }

        private async void btnRunHealth_Click(object sender, EventArgs e)
        {
            _probes = _session.Services.Health.CreateProbes();
            BindHealth();

            _healthCancellation = new CancellationTokenSource();
            btnRunHealth.Enabled = false;
            btnCancelHealth.Enabled = true;
            SetHealthStatus("● running…", System.Drawing.Color.FromArgb(232, 161, 60));

            try
            {
                CommandResult result = await _session.Services.Health.RunAsync(_probes, CurrentContext, OnProbeFinished, _healthCancellation.Token);
                ShowHealthResult(result);
            }
            catch (OperationCanceledException)
            {
                SetHealthStatus("● cancelled", System.Drawing.Color.FromArgb(232, 161, 60));
            }
            catch (Exception ex)
            {
                ReportUnexpected(ex, "The health check could not be completed.");
            }
            finally
            {
                btnRunHealth.Enabled = true;
                btnCancelHealth.Enabled = false;
                _healthCancellation?.Dispose();
                _healthCancellation = null;
                NewCommand();
                Application.Update(this);
            }
        }

        private void btnCancelHealth_Click(object sender, EventArgs e)
        {
            _healthCancellation?.Cancel();
        }

        private void dgvWorkQueue_SelectionChanged(object sender, EventArgs e)
        {
            WorkQueueRow row = SelectedRow();
            btnApprove.Enabled = CanApprove(row);
            if (row != null)
                lblStatusBar.Text = $"selected #{row.Id} · {row.Status} · v{row.Version}";
        }

        private void btnCapstoneReview_Click(object sender, EventArgs e)
        {
            if (_capstoneReview == null || _capstoneReview.IsDisposed)
                _capstoneReview = new CapstoneReviewPage(_session, this);
            Application.MainPage = _capstoneReview;
        }

        #endregion

        #region Showing results

        /// <summary>Reads the KPIs and one page of the queue: two service calls, one banner, one footer line.</summary>
        private async Task RefreshAsync()
        {
            _loading = true;
            btnRefresh.Enabled = false;
            ClearBanner();
            NewCommand();

            try
            {
                string denial = null;
                DashboardKpis kpis = await _session.Services.Dashboard.GetKpisAsync(CurrentContext, reason => denial = reason);
                PagedResult<WorkQueueRow> page = await _session.Services.WorkOrders.GetQueueAsync(
                    new WorkQueueQuery { StatusFilter = cboStatus.Text }, CurrentContext);

                ShowKpis(kpis);
                ShowQueue(page);

                if (denial != null)
                    ShowBanner($"Not allowed — {denial}");
            }
            catch (Exception ex)
            {
                ReportUnexpected(ex, "The queue could not be loaded.");
            }
            finally
            {
                btnRefresh.Enabled = true;
                _loading = false;
                // The work above can finish after the request that started it has returned;
                // push the final state or the browser keeps showing a disabled Refresh.
                Application.Update(this);
            }
        }

        private void ShowApproveResult(ApproveResult result)
        {
            if (result.Succeeded)
            {
                lblStatusBar.Text = $"audit ← {result.AuditLine}";
                AlertBox.Show($"Work order #{result.WorkOrderId} approved.", MessageBoxIcon.Information,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return;
            }

            ShowBanner(result.ErrorText);
            if (!string.IsNullOrEmpty(result.AuditLine))
                lblStatusBar.Text = $"audit ← {result.AuditLine}";
        }

        /// <summary>Called by the diagnostics service after every probe — the browser sees the card fill in.</summary>
        private Task OnProbeFinished(HealthProbe probe)
        {
            BindHealth();
            SetHealthStatus($"● {_probes.Count(p => p.State != ProbeState.Pending)}/{_probes.Count} probes", System.Drawing.Color.FromArgb(232, 161, 60));
            Application.Update(this);
            return Task.CompletedTask;
        }

        private void ShowHealthResult(CommandResult result)
        {
            BindHealth();
            if (result.Succeeded)
            {
                SetHealthStatus($"● healthy · {_probes.Count}/{_probes.Count} probes", System.Drawing.Color.FromArgb(31, 157, 87));
                return;
            }
            SetHealthStatus($"● {result.ErrorText}", System.Drawing.Color.FromArgb(232, 161, 60));
            ShowBanner(result.ErrorText);
        }

        private void ShowKpis(DashboardKpis kpis)
        {
            kpiOpen.Value = kpis == null ? "—" : kpis.Open.ToString();
            kpiEscalated.Value = kpis == null ? "—" : kpis.Escalated.ToString();
            kpiDueToday.Value = kpis == null ? "—" : kpis.DueToday.ToString();
            kpiOverdue.Value = kpis == null ? "—" : kpis.Overdue.ToString();
            kpiCompleted.Value = kpis == null ? "—" : kpis.CompletedLast7Days.ToString();
        }

        private void ShowQueue(PagedResult<WorkQueueRow> page)
        {
            dgvWorkQueue.DataSource = new BindingSource { DataSource = page.Rows };
            lblStatusBar.Text = $"{page.Rows.Count} of {page.Total} rows · EnterpriseOps.Services.WorkOrderService · {page.ElapsedMs} ms · corr {CurrentContext.CorrelationId}";
            btnApprove.Enabled = false;
        }

        private void BindHealth()
        {
            dgvHealth.DataSource = null;
            dgvHealth.DataSource = new BindingSource { DataSource = _probes };
        }

        #endregion

        #region Helpers

        private WorkQueueRow SelectedRow() => dgvWorkQueue.CurrentRow?.DataBoundItem as WorkQueueRow;

        private bool CanApprove(WorkQueueRow row) => row != null && _session.User.Role != Role.Technician;

        private CommandContext NewCommand()
        {
            _current = _session.BeginCommand();
            return _current;
        }

        private void SetHealthStatus(string text, System.Drawing.Color colour)
        {
            lblHealthStatus.Text = text;
            lblHealthStatus.ForeColor = colour;
        }

        private void ShowBanner(string text)
        {
            lblBanner.Text = "⚠ " + text;
            lblBanner.Visible = true;
        }

        private void ClearBanner()
        {
            lblBanner.Text = "";
            lblBanner.Visible = false;
        }

        private void Warn(string text)
            => AlertBox.Show(text, MessageBoxIcon.Warning, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);

        /// <summary>An unexpected exception is logged with its correlation id and hidden behind one sentence.</summary>
        private void ReportUnexpected(Exception ex, string message)
        {
            _trace.Service($"unhandled {ex.GetType().Name}: {ex.Message} [corr {CurrentContext.CorrelationId}]");
            ShowBanner($"{message} Quote correlation id {CurrentContext.CorrelationId} when you report it.");
            AlertBox.Show(message, MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion
    }
}
