using System;
using System.Collections.Generic;
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
    /// EnterpriseOps — Command Center. The capstone application in one screen: the KPI cards a service
    /// computes, the tenant-scoped work queue, the approve command with its permission check and audit line,
    /// and the diagnostics card that answers the operations questions of a production review.
    ///
    /// Left card:   Refresh · the queue filter · five KpiTiles · dgvWorkQueue · the failure banner · the footer.
    /// Health card: six probes run one at a time, cancellable, each with the evidence it found.
    /// Right card:  the live activity trace — every layer's decision, tagged UI → / Security: / Service: /
    ///              Data: / Job: / Review: / Docs:. The same buffer as the Capstone Review screen.
    /// Bottom bar:  the success path (approve), the progress path (health check), two failure paths (stale
    ///              version, permission denied), the recovery (reload), the second screen and Clear trace.
    ///
    /// The boundary this screen defends: this file owns UI state only. Which statuses count as "open", who
    /// may approve, what an audit line says and whether a write is stale are decided in
    /// <c>EnterpriseOps.Services</c>. Every handler below is a few lines and calls one service — the shape
    /// the module's own review checklist (rule R10) demands of generated code as well.
    /// </summary>
    public partial class CommandCenterDashboard : Page
    {
        // Per-session state and services — instance fields, never statics (checklist Q2).
        private readonly SessionContext _session;
        private readonly ActivityTrace _trace;

        private CommandContext _current;
        private CancellationTokenSource _healthCancellation;
        private List<HealthProbe> _probes;
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
            _trace.LineAdded += trace_LineAdded;

            _loading = true;                    // setting the filter must not fire a refresh before Load
            cboStatus.SelectedIndex = 0;
            _loading = false;

            _probes = session.Services.Health.CreateProbes();
        }

        /// <summary>The command running right now: same tenant and user, one correlation id per click.</summary>
        private CommandContext CurrentContext => _current ?? NewCommand();

        #region Event handlers — thin, one service call each

        private async void CommandCenterDashboard_Load(object sender, EventArgs e)
        {
            ShowTraceHistory();
            ShowSignedIn();
            BindHealth();
            _trace.Ui($"CommandCenterDashboard_Load  session={_session.Tenant.Id}/{_session.User.Name}  docs={(_session.Services.Docs.Found ? "found" : "MISSING")}");
            await RefreshAsync("first load");
        }

        /// <summary>The walkthrough's Refresh: two service calls, no decision taken here.</summary>
        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await RefreshAsync("refresh");
        }

        private async void cboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            await RefreshAsync($"filter {cboStatus.Text}");
        }

        /// <summary>The success path: one command, decided by the service, audited whatever the answer is.</summary>
        private async void btnApprove_Click(object sender, EventArgs e)
        {
            await ApproveSelectedAsync(null);
        }

        /// <summary>Failure path 1: another session saved the row first, so the version we hold is stale.</summary>
        private async void btnStaleVersion_Click(object sender, EventArgs e)
        {
            WorkQueueRow row = SelectedRow();
            if (row == null)
            {
                Warn("Select a work order first.");
                return;
            }

            if (!row.CanBeApproved)
            {
                // The stale-version path needs a row the service would otherwise approve; a New/OnHold row is
                // refused earlier ("only InProgress or Escalated") and never reaches the version check.
                WorkQueueRow candidate = PageRows().FirstOrDefault(r => r.CanBeApproved);
                if (candidate == null)
                {
                    Warn("No approvable work order on this page — change the status filter to InProgress or Escalated.");
                    return;
                }
                _trace.Ui($"btnStaleVersion_Click → #{row.Id} is {row.Status} and cannot be approved; using #{candidate.Id} ({candidate.Status}) instead");
                row = candidate;
            }

            _trace.Ui($"btnStaleVersion_Click → simulate a concurrent edit on #{row.Id}, then approve with v{row.Version}");
            _session.Services.WorkOrders.SimulateConcurrentEdit(row.Id);
            await ApproveRowAsync(row, "after a concurrent edit");
        }

        /// <summary>Failure path 2 and its recovery: a Technician may view the queue and nothing else.</summary>
        private async void btnSwitchUser_Click(object sender, EventArgs e)
        {
            bool isTechnician = _session.User.Role == Role.Technician;
            _session.SignInAs(isTechnician ? KnownUsers.AnaOps : KnownUsers.BenTech);
            btnSwitchUser.Text = isTechnician ? "Switch to ben.tech" : "Switch back to ana.ops";
            _trace.Ui($"btnSwitchUser_Click → signed in as {_session.User}");
            ShowSignedIn();
            await RefreshAsync("after switching user");
        }

        /// <summary>The recovery: the same read path; afterwards the row carries the version the store has.</summary>
        private async void btnReload_Click(object sender, EventArgs e)
        {
            await RefreshAsync("recovery reload");
        }

        /// <summary>The progress path: six probes, one at a time, each pushed to the browser as it finishes.</summary>
        private async void btnRunHealth_Click(object sender, EventArgs e)
        {
            _probes = _session.Services.Health.CreateProbes();
            BindHealth();

            _healthCancellation = new CancellationTokenSource();
            btnRunHealth.Enabled = false;
            btnCancelHealth.Enabled = true;
            SetHealthStatus("● running…", System.Drawing.Color.FromArgb(232, 161, 60));
            _trace.Ui("btnRunHealth_Click → DiagnosticsService.RunAsync(6 probes)");

            try
            {
                CommandResult result = await _session.Services.Health.RunAsync(_probes, CurrentContext, OnProbeFinished, _healthCancellation.Token);
                ShowHealthResult(result);
            }
            catch (OperationCanceledException)
            {
                SetHealthStatus("● cancelled", System.Drawing.Color.FromArgb(232, 161, 60));
                _trace.Job("health check cancelled by the operator");
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
            }
        }

        private void btnCancelHealth_Click(object sender, EventArgs e)
        {
            _trace.Ui("btnCancelHealth_Click → CancellationTokenSource.Cancel()");
            _healthCancellation?.Cancel();
        }

        private void dgvWorkQueue_SelectionChanged(object sender, EventArgs e)
        {
            WorkQueueRow row = SelectedRow();
            btnApprove.Enabled = row != null && _session.User.Role != Role.Technician;
            if (row != null)
                lblStatusBar.Text = $"selected #{row.Id} · {row.Status} · v{row.Version} — the approve command will carry v{row.Version}";
        }

        private void btnCapstoneReview_Click(object sender, EventArgs e)
        {
            _trace.Ui("btnCapstoneReview_Click → Application.MainPage = CapstoneReviewPage");
            if (_capstoneReview == null || _capstoneReview.IsDisposed)
                _capstoneReview = new CapstoneReviewPage(_session, this);
            Application.MainPage = _capstoneReview;
        }

        private void btnClearTrace_Click(object sender, EventArgs e)
        {
            _trace.Clear();
            lstTrace.Items.Clear();
        }

        #endregion

        #region Screen work — everything below only shows what a service decided

        /// <summary>Reads the KPIs and one page of the queue. Two service calls, one banner, one footer line.</summary>
        private async Task RefreshAsync(string reason)
        {
            _loading = true;
            btnRefresh.Enabled = false;
            SetStatus("● loading…", System.Drawing.Color.FromArgb(232, 161, 60));
            ClearBanner();
            NewCommand();
            _trace.Ui($"refresh ({reason}) corr={CurrentContext.CorrelationId}");

            try
            {
                string denial = null;
                DashboardKpis kpis = await _session.Services.Dashboard.GetKpisAsync(CurrentContext, reason2 => denial = reason2);
                PagedResult<WorkQueueRow> page = await _session.Services.WorkOrders.GetQueueAsync(
                    new WorkQueueQuery { StatusFilter = cboStatus.Text }, CurrentContext);

                ShowKpis(kpis);
                ShowQueue(page);

                if (denial != null)
                {
                    ShowBanner($"Not allowed — {denial}");
                    SetStatus("● view refused", System.Drawing.Color.FromArgb(224, 86, 59));
                }
                else
                {
                    SetStatus($"● {page.Total} work orders · {_session.Tenant.Name}", System.Drawing.Color.FromArgb(31, 157, 87));
                }
            }
            catch (Exception ex)
            {
                ReportUnexpected(ex, "The queue could not be loaded.");
            }
            finally
            {
                btnRefresh.Enabled = true;
                _loading = false;
            }
        }

        /// <summary>The approve path, shared by the success button and the stale-version failure button.</summary>
        private Task ApproveSelectedAsync(string note)
        {
            WorkQueueRow row = SelectedRow();
            if (row == null)
            {
                Warn("Select a work order first.");
                return Task.CompletedTask;
            }
            return ApproveRowAsync(row, note);
        }

        /// <summary>The rows currently bound to the grid (the page the service returned).</summary>
        private IEnumerable<WorkQueueRow> PageRows()
            => ((dgvWorkQueue.DataSource as BindingSource)?.DataSource as IEnumerable<WorkQueueRow>) ?? Enumerable.Empty<WorkQueueRow>();

        private async Task ApproveRowAsync(WorkQueueRow row, string note)
        {
            btnApprove.Enabled = false;
            ClearBanner();
            NewCommand();
            _trace.Ui($"btnApprove_Click → ApproveWorkOrderCommand #{row.Id} v{row.Version}");

            try
            {
                ApproveResult result = await _session.Services.WorkOrders.ApproveAsync(
                    new ApproveWorkOrderCommand { WorkOrderId = row.Id, ExpectedVersion = row.Version, Note = note }, CurrentContext);

                ShowApproveResult(result);
                if (result.Succeeded)
                    await RefreshAsync("after approve");
            }
            catch (Exception ex)
            {
                ReportUnexpected(ex, "The approval could not be completed.");
            }
            finally
            {
                btnApprove.Enabled = SelectedRow() != null && _session.User.Role != Role.Technician;
            }
        }

        private void ShowApproveResult(ApproveResult result)
        {
            if (result.Succeeded)
            {
                SetStatus($"● #{result.WorkOrderId} approved · v{result.NewVersion}", System.Drawing.Color.FromArgb(31, 157, 87));
                lblStatusBar.Text = $"audit ← {result.AuditLine}";
                AlertBox.Show($"Work order #{result.WorkOrderId} approved.", MessageBoxIcon.Information,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return;
            }

            ShowBanner(result.ErrorText);
            SetStatus("● approval refused", System.Drawing.Color.FromArgb(224, 86, 59));
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

        private WorkQueueRow SelectedRow() => dgvWorkQueue.CurrentRow?.DataBoundItem as WorkQueueRow;

        private CommandContext NewCommand()
        {
            _current = _session.BeginCommand();
            lblCorrelation.Text = "corr " + _current.CorrelationId;
            return _current;
        }

        private void ShowSignedIn()
        {
            lblTenant.Text = "tenant: " + _session.Tenant.Id;
            lblUser.Text = $"Signed in: {_session.User.Name} · {_session.User.Role}";
            btnSwitchUser.Text = _session.User.Role == Role.Technician ? "Switch back to ana.ops" : "Switch to ben.tech";
        }

        private void SetStatus(string text, System.Drawing.Color colour)
        {
            lblStatus.Text = text;
            lblStatus.ForeColor = colour;
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
            SetStatus("● error", System.Drawing.Color.FromArgb(224, 86, 59));
            AlertBox.Show(message, MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        private void ShowTraceHistory()
        {
            lstTrace.Items.Clear();
            foreach (string line in _trace.Lines)
                lstTrace.Items.Add(line);
            SelectLastTraceLine();
        }

        private void trace_LineAdded(object sender, string line)
        {
            if (IsDisposed)
                return;
            lstTrace.Items.Add(line);
            SelectLastTraceLine();
        }

        private void SelectLastTraceLine()
        {
            if (lstTrace.Items.Count > 0)
                lstTrace.SelectedIndex = lstTrace.Items.Count - 1;
        }

        /// <summary>
        /// Called from Dispose: the activity trace belongs to the session and outlives this screen, so the
        /// handler must not. A subscription to a longer-lived object is the leak the checklist's disposal
        /// rule (R8) is about.
        /// </summary>
        private void DetachTrace()
        {
            if (_trace != null)
                _trace.LineAdded -= trace_LineAdded;
        }

        #endregion
    }
}
