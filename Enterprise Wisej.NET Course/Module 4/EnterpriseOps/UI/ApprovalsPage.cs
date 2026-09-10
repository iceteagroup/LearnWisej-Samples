using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Data;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;
using EnterpriseOps.Services;
using EnterpriseOps.Services.Commands;
using EnterpriseOps.Services.Queries;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// EnterpriseOps — Approvals. The Module 4 screen: the same ApprovePanel the walkthrough builds, in
    /// front of a real EF Core / SQLite database.
    ///
    /// Left card:   the work queue — <see cref="WorkQueueRow"/> projections from
    ///              <see cref="IWorkOrderQueryService"/>. Read-only by construction; nothing here can be saved.
    /// Middle card: the ApprovePanel — a comment, Approve / Cancel, and the <see cref="CommandResult"/>
    ///              banner. The Approve handler builds a command and shows a result. That is all it does.
    /// Right card:  the live activity trace, tagged by layer (UI → · Security: · Service: · Data: · Audit:)
    ///              so a reviewer can watch the DbContext being created and disposed, the transaction open
    ///              and close, and every failure being mapped.
    /// Bottom bar:  the success path (Create), four failure paths (duplicate number, stale version, slow
    ///              query, technician denied), the progress path (batch approve, one transaction each),
    ///              the wrong-lifetime demonstration and its recovery, the audit log and Clear trace.
    ///
    /// The boundary this file lives behind: no DbContext, no LINQ over entities, no transaction, no SQL,
    /// no <c>EnterpriseOps.Domain.WorkOrder</c>. Commands go out, results and projections come back.
    /// This screen owns UI state only — the selected row, the comment text, the banner colours.
    /// </summary>
    public partial class ApprovalsPage : Page, IActivityTrace
    {
        // Per-session, wired here (the composition root of this sample). Instance fields, never statics:
        // two users must never share a database, a session context or a trace.
        private readonly SessionContext _session;
        private readonly SessionDatabase _database;
        private readonly FaultInjector _faults;
        private readonly IWorkOrderCommandService _commands;
        private readonly IWorkOrderQueryService _queries;
        private readonly SessionLongContextAntiPattern _lifetimeDemo;

        private readonly BindingSource _queue = new BindingSource();
        private const int MaxTraceLines = 500;

        private bool _ready;
        private bool _shuttingDown;
        private int _createdCounter;

        public ApprovalsPage()
        {
            InitializeComponent();

            _session = new SessionContext(Application.SessionId);
            _faults = new FaultInjector();

            // Opens this session's in-memory SQLite connection, creates the schema and seeds it.
            // The connection is session-long because an in-memory database dies with its connection;
            // every DbContext over it is per-operation (docs/DbContextLifetimeDecision.md).
            _database = new SessionDatabase(this);

            _commands = new WorkOrderCommandService(_database, this, _faults, () => _session.CommandTimeout);
            _queries = new WorkOrderQueryService(_database, this);
            _lifetimeDemo = new SessionLongContextAntiPattern(_database, this);

            dgvWorkQueue.DataSource = _queue;
        }

        #region Load

        private async void ApprovalsPage_Load(object sender, EventArgs e)
        {
            try
            {
                cboTenant.Items.AddRange(new object[] { "contoso", "fabrikam", "northwind" });
                cboTenant.SelectedItem = _session.TenantId;

                Trace(TraceLayer.UI, "ApprovalsPage_Load → IWorkOrderQueryService.SearchAsync (the read side)");
                await RefreshQueueAsync(2002);          // the walkthrough's work order
                _ready = true;
            }
            catch (Exception ex)
            {
                ShowUnexpected(ex);
            }
        }

        #endregion

        #region The Approve handler — the shape the lesson asks for

        /// <summary>
        /// The whole handler: build the command, hand it to the service, show the result. No DbContext,
        /// no LINQ, no transaction, no idea that a database exists. Everything it needs is the selected
        /// row's id and version and the comment the user typed.
        /// </summary>
        private async void btnApprove_Click(object sender, EventArgs e)
        {
            if (!RequireSelection())
                return;

            BeginBusy("ApproveAsync — short-lived DbContext · transaction open…",
                      $"btnApprove_Click → ApproveWorkOrderCommand({SelectedRow.Number}, v{SelectedRow.Version}) → IWorkOrderCommandService.ApproveAsync");
            try
            {
                var result = await _commands.ApproveAsync(BuildApproveCommand(), NewCommand(), CancellationToken.None);
                ShowResult(result);
            }
            catch (Exception ex)
            {
                ShowUnexpected(ex);
            }
            finally
            {
                EndBusy();
                await SafeRefreshQueueAsync();
            }
        }

        /// <summary>Intent as data: id, tenant, user, comment, and the version the user was looking at.</summary>
        private ApproveWorkOrderCommand BuildApproveCommand(WorkQueueRow row = null, int? expectedVersion = null)
        {
            row = row ?? SelectedRow;
            return new ApproveWorkOrderCommand(
                row.Id,
                _session.TenantId,
                _session.UserId,
                txtComment.Text,
                expectedVersion ?? row.Version);
        }

        /// <summary>Pure UI state: the comment box and the banner. No service, no database.</summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtComment.Text = "";
            HideResult();
            Trace(TraceLayer.UI, "btnCancel_Click → cleared the comment and the banner (no command was built)");
            SetStatusBar($"Ready — {Describe(SelectedRow)}");
        }

        #endregion

        #region The read side: search, tenant, selection

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Trace(TraceLayer.UI, $"btnSearch_Click → WorkQueueQuery(tenant '{_session.TenantId}', search '{txtSearch.Text}')");
                await RefreshQueueAsync();
            }
            catch (Exception ex)
            {
                ShowUnexpected(ex);
            }
        }

        private async void cboTenant_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ready)
                return;

            try
            {
                _session.TenantId = (string)cboTenant.SelectedItem;
                Trace(TraceLayer.UI, $"cboTenant → tenant '{_session.TenantId}': every query and command is filtered by it");
                HideResult();
                await RefreshQueueAsync();
            }
            catch (Exception ex)
            {
                ShowUnexpected(ex);
            }
        }

        private void dgvWorkQueue_SelectionChanged(object sender, EventArgs e)
        {
            var row = SelectedRow;
            if (row == null)
            {
                lblWorkOrder.Text = "—";
                lblVersion.Text = "select a work order";
                return;
            }

            lblWorkOrder.Text = $"{row.Number} — {row.Title}";
            lblVersion.Text = $"v{row.Version} · tenant {_session.TenantId} · {row.Status}";
            btnApprove.Enabled = true;
        }

        /// <summary>One query, one short-lived DbContext, one page of projections into the grid.</summary>
        private async Task RefreshQueueAsync(int? select = null)
        {
            int keep = select ?? (SelectedRow != null ? SelectedRow.Id : 0);

            var page = await _queries.SearchAsync(
                new WorkQueueQuery
                {
                    TenantId = _session.TenantId,
                    Search = txtSearch.Text,
                    Page = 1,
                    PageSize = 200,
                },
                CancellationToken.None);

            _queue.DataSource = page.Rows;
            _queue.ResetBindings(false);
            lblQueueTitle.Text = $"Work queue · the read side ({page.Rows.Count} of {page.Total})";

            SelectById(keep);
            UpdateHeader();
        }

        /// <summary>
        /// The refresh every handler runs in its <c>finally</c>. It swallows its own failures on purpose:
        /// the command's result is already on screen, and an <c>async void</c> handler must not end with
        /// an exception escaping from a finally block.
        /// </summary>
        private async Task SafeRefreshQueueAsync()
        {
            try
            {
                await RefreshQueueAsync();
            }
            catch (Exception exception)
            {
                Trace(TraceLayer.UI, $"the queue could not be refreshed ({exception.GetType().Name}) — the result above still stands");
            }
        }

        private void SelectById(int workOrderId)
        {
            if (dgvWorkQueue.Rows.Count == 0)
            {
                btnApprove.Enabled = false;
                dgvWorkQueue_SelectionChanged(this, EventArgs.Empty);
                return;
            }

            for (int i = 0; i < dgvWorkQueue.Rows.Count; i++)
            {
                if (dgvWorkQueue.Rows[i].DataBoundItem is WorkQueueRow row && row.Id == workOrderId)
                {
                    dgvWorkQueue.Rows[i].Selected = true;
                    dgvWorkQueue_SelectionChanged(this, EventArgs.Empty);
                    return;
                }
            }

            dgvWorkQueue.Rows[0].Selected = true;
            dgvWorkQueue_SelectionChanged(this, EventArgs.Empty);
        }

        private WorkQueueRow SelectedRow
        {
            get
            {
                if (dgvWorkQueue.SelectedRows.Count > 0)
                    return dgvWorkQueue.SelectedRows[0].DataBoundItem as WorkQueueRow;
                return dgvWorkQueue.CurrentRow != null ? dgvWorkQueue.CurrentRow.DataBoundItem as WorkQueueRow : null;
            }
        }

        private IEnumerable<WorkQueueRow> QueueRows
            => _queue.DataSource as List<WorkQueueRow> ?? new List<WorkQueueRow>();

        #endregion

        #region Success path — Create (INSERT + audit in one transaction)

        private async void btnCreate_Click(object sender, EventArgs e)
        {
            _createdCounter++;
            var command = new CreateWorkOrderCommand(
                _session.TenantId,
                _session.UserId,
                $"WO-90{_createdCounter:00}",
                "Replace dock seal (created from the lab)",
                "Fabrikam Logistics",
                "Dock 4",
                Priority.Normal,
                DateTime.UtcNow.Date.AddDays(7));

            BeginBusy("CreateAsync — short-lived DbContext · transaction open…",
                      $"btnCreate_Click → CreateWorkOrderCommand({command.Number}) → IWorkOrderCommandService.CreateAsync");
            try
            {
                var result = await _commands.CreateAsync(command, NewCommand(), CancellationToken.None);
                ShowResult(result);
            }
            catch (Exception ex)
            {
                ShowUnexpected(ex);
            }
            finally
            {
                EndBusy();
                await SafeRefreshQueueAsync();
            }
        }

        #endregion

        #region Failure path 1 — duplicate number (the UNIQUE index decides)

        private async void btnDuplicate_Click(object sender, EventArgs e)
        {
            if (!RequireSelection())
                return;

            var existing = SelectedRow;
            var command = new CreateWorkOrderCommand(
                _session.TenantId,
                _session.UserId,
                existing.Number,                                  // already taken in this tenant
                "Duplicate of " + existing.Title,
                existing.Customer,
                existing.Site,
                Priority.Normal,
                null);

            BeginBusy("CreateAsync — the UNIQUE index will decide…",
                      $"btnDuplicate_Click → CreateWorkOrderCommand({command.Number}) — that number already exists in tenant '{_session.TenantId}'");
            try
            {
                var result = await _commands.CreateAsync(command, NewCommand(), CancellationToken.None);
                ShowResult(result);
            }
            catch (Exception ex)
            {
                ShowUnexpected(ex);
            }
            finally
            {
                EndBusy();
                await SafeRefreshQueueAsync();
            }
        }

        #endregion

        #region Failure path 2 — stale version (optimistic concurrency)

        private async void btnStale_Click(object sender, EventArgs e)
        {
            var target = QueueRows.FirstOrDefault(r => r.Status == WorkOrderStatus.InProgress.ToString()) ?? SelectedRow;
            if (target == null)
            {
                Warn("No work order in this tenant is InProgress, so there is nothing to approve. Switch to fabrikam.");
                return;
            }

            SelectById(target.Id);
            int stale = target.Version - 1;

            BeginBusy("ApproveAsync — the database will check the version…",
                      $"btnStale_Click → ApproveWorkOrderCommand({target.Number}, ExpectedVersion=v{stale}) — the row is at v{target.Version}");
            try
            {
                // The transition is valid, so the command reaches SaveChanges: EF Core emits
                // UPDATE … WHERE Id = @id AND Version = @stale, which affects 0 rows.
                var result = await _commands.ApproveAsync(BuildApproveCommand(target, stale), NewCommand(), CancellationToken.None);
                ShowResult(result);
            }
            catch (Exception ex)
            {
                ShowUnexpected(ex);
            }
            finally
            {
                EndBusy();
                await SafeRefreshQueueAsync();
            }
        }

        #endregion

        #region Failure path 3 — the command timeout

        private async void btnSlowQuery_Click(object sender, EventArgs e)
        {
            if (!RequireSelection())
                return;

            _faults.SlowNextCommand(1500, 400);       // 1.5 s of work, 400 ms of budget

            BeginBusy("ApproveAsync — simulated slow query inside the transaction…",
                      "btnSlowQuery_Click → the next command sleeps 1500 ms with a 400 ms timeout budget");
            try
            {
                var result = await _commands.ApproveAsync(BuildApproveCommand(), NewCommand(), CancellationToken.None);
                ShowResult(result);
            }
            catch (Exception ex)
            {
                ShowUnexpected(ex);
            }
            finally
            {
                EndBusy();
                await SafeRefreshQueueAsync();
            }
        }

        #endregion

        #region Failure path 4 — a technician may not approve (and the recovery)

        private async void btnSwitchUser_Click(object sender, EventArgs e)
        {
            bool toTechnician = _session.UserId != KnownUsers.Technician;
            _session.UserId = toTechnician ? KnownUsers.Technician : KnownUsers.Manager;
            btnSwitchUser.Text = toTechnician ? "Sign in as ana.ops" : "Sign in as ben.tech";
            UpdateHeader();
            Trace(TraceLayer.UI, $"btnSwitchUser_Click → signed in as {_session.UserId} ({_session.Role})");

            if (!toTechnician || !RequireSelection())
            {
                SetStatusBar($"Ready — signed in as {_session.UserId} · {_session.Role}");
                return;
            }

            // Same command, same service, different caller: the rule is checked on the server, before
            // any transaction is opened.
            BeginBusy("ApproveAsync — authorizing…",
                      $"btnSwitchUser_Click → the same ApproveWorkOrderCommand, now as {_session.UserId} ({_session.Role})");
            try
            {
                var result = await _commands.ApproveAsync(BuildApproveCommand(), NewCommand(), CancellationToken.None);
                ShowResult(result);
            }
            catch (Exception ex)
            {
                ShowUnexpected(ex);
            }
            finally
            {
                EndBusy();
                await SafeRefreshQueueAsync();
            }
        }

        #endregion

        #region Progress path — batch approve, one transaction per work order

        private async void btnBatchApprove_Click(object sender, EventArgs e)
        {
            BeginBusy("Batch approve — one command, one transaction per work order…",
                      "btnBatchApprove_Click → GetApprovalCandidatesAsync(6) → one ApproveWorkOrderCommand each");
            try
            {
                var ids = await _queries.GetApprovalCandidatesAsync(_session.TenantId, 6, CancellationToken.None);
                int committed = 0, rejected = 0;

                foreach (int id in ids)
                {
                    var header = await _queries.GetHeaderAsync(_session.TenantId, id, CancellationToken.None);
                    if (header == null)
                        continue;

                    var command = new ApproveWorkOrderCommand(
                        header.Id, _session.TenantId, _session.UserId,
                        "Batch approval — weekly close.", header.Version);

                    var result = await _commands.ApproveAsync(command, NewCommand(), CancellationToken.None);
                    if (result.Success)
                        committed++;
                    else
                        rejected++;

                    // Push the progress out between transactions instead of after the loop.
                    SetStatusBar($"Batch approve — {committed} committed · {rejected} rejected · {ids.Count - committed - rejected} to go");
                    UpdateHeader();
                    Application.Update(this);
                    await Task.Delay(250);
                }

                Trace(TraceLayer.Service, $"batch finished: {committed} committed, {rejected} rejected — each in its own transaction, nothing half-saved");
                lblResult.Visible = true;
                lblResultDetail.Visible = true;
                PaintResult(rejected == 0);
                lblResult.Text = $"Batch approve: {committed} approved, {rejected} rejected.";
                lblResultDetail.Text = $"{ids.Count} commands · {ids.Count} transactions · {rejected} rolled back";
                SetStatusBar($"Batch approve — {committed} committed · {rejected} rejected · done");
            }
            catch (Exception ex)
            {
                ShowUnexpected(ex);
            }
            finally
            {
                EndBusy(keepStatus: true);
                await SafeRefreshQueueAsync();
            }
        }

        #endregion

        #region The wrong lifetime, and the recovery

        private async void btnWrongLifetime_Click(object sender, EventArgs e)
        {
            if (!RequireSelection())
                return;

            BeginBusy("Reading through a DbContext held in a field…",
                      "btnWrongLifetime_Click → SessionLongContextAntiPattern.ReadThroughLongLivedContextAsync (deliberately wrong)");
            try
            {
                string verdict = await _lifetimeDemo.ReadThroughLongLivedContextAsync(
                    _session.TenantId, SelectedRow.Id, CancellationToken.None);

                bool stale = verdict.StartsWith("STALE", StringComparison.Ordinal);
                lblResult.Visible = true;
                lblResultDetail.Visible = true;
                PaintResult(!stale, warning: !stale);
                lblResult.Text = stale
                    ? "The session-long DbContext is serving stale data."
                    : "The session-long DbContext agrees with the database — for now.";
                lblResultDetail.Text = verdict;
                SetStatusBar(stale
                    ? "Wrong lifetime — the field-held context is stale · tracked entities are still alive"
                    : "Wrong lifetime — approve or update this work order, then click again");
            }
            catch (Exception ex)
            {
                ShowUnexpected(ex);
            }
            finally
            {
                EndBusy(keepStatus: true);
                UpdateHeader();
            }
        }

        private void btnFixLifetime_Click(object sender, EventArgs e)
        {
            if (!_lifetimeDemo.IsAlive)
            {
                Warn("There is no session-long DbContext to dispose — click \"Wrong lifetime\" first.");
                return;
            }

            Trace(TraceLayer.UI, "btnFixLifetime_Click → dispose the field-held DbContext; from now on, one context per command");
            _lifetimeDemo.Dispose();
            UpdateHeader();

            lblResult.Visible = true;
            lblResultDetail.Visible = true;
            PaintResult(true);
            lblResult.Text = "The field-held DbContext is gone.";
            lblResultDetail.Text = $"live contexts: {_database.LiveContexts} · every read is short-lived again";
            SetStatusBar("Recovered — one DbContext per operation · live contexts 0");
            AlertBox.Show("The session-long DbContext was disposed.", MessageBoxIcon.Information,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion

        #region Audit log

        private async void btnAudit_Click(object sender, EventArgs e)
        {
            try
            {
                Trace(TraceLayer.UI, "btnAudit_Click → IWorkOrderQueryService.GetAuditAsync (the service checks the permission)");
                var audit = await _queries.GetAuditAsync(NewCommand(), null, 100, CancellationToken.None);

                if (!audit.Allowed)
                {
                    Warn(audit.DeniedReason);
                    SetStatusBar("Audit log — PERMISSION_DENIED · no query was run");
                    return;
                }

                var dialog = new AuditLogDialog(_session.TenantId, audit.Rows);
                await dialog.ShowDialogAsync();
                SetStatusBar($"Audit log — {audit.Rows.Count} rows · committed and rejected commands, with their codes");
            }
            catch (Exception ex)
            {
                ShowUnexpected(ex);
            }
            finally
            {
                UpdateHeader();
            }
        }

        #endregion

        #region Trace, status, banner (UI state only)

        /// <summary>
        /// The trace sink — the only place that knows lstTrace exists. Every layer writes here:
        /// UI → · Security: · Service: · Data: · Audit:.
        /// </summary>
        public void Trace(string layer, string message)
        {
            if (_shuttingDown || IsDisposed)
                return;

            lstTrace.Items.Add($"{DateTime.Now:HH:mm:ss.fff}  {layer} {message}");
            while (lstTrace.Items.Count > MaxTraceLines)
                lstTrace.Items.RemoveAt(0);
            lstTrace.SelectedIndex = lstTrace.Items.Count - 1;
        }

        private void btnClearTrace_Click(object sender, EventArgs e)
        {
            lstTrace.Items.Clear();
            Trace(TraceLayer.UI, "btnClearTrace_Click → trace cleared (the database and the audit log are untouched)");
        }

        /// <summary>A new correlation id per command — it reaches the result, the audit row and the trace.</summary>
        private CommandContext NewCommand()
        {
            var context = _session.NewCommandContext();
            lblCorrelation.Text = "corr " + context.CorrelationId;
            return context;
        }

        private void BeginBusy(string statusBar, string uiTraceLine)
        {
            HideResult();
            Trace(TraceLayer.UI, uiTraceLine);
            SetStatusBar(statusBar);
            SetButtonsEnabled(false);
        }

        private void EndBusy(bool keepStatus = false)
        {
            SetButtonsEnabled(true);
            UpdateHeader();
            if (!keepStatus && _database.LiveContexts == 0)
                Trace(TraceLayer.Data, "live contexts: 0 — the operation owns no state after it ends");
        }

        private void SetButtonsEnabled(bool enabled)
        {
            btnApprove.Enabled = enabled && SelectedRow != null;
            btnCreate.Enabled = enabled;
            btnDuplicate.Enabled = enabled;
            btnStale.Enabled = enabled;
            btnSlowQuery.Enabled = enabled;
            btnSwitchUser.Enabled = enabled;
            btnBatchApprove.Enabled = enabled;
            btnWrongLifetime.Enabled = enabled;
            btnSearch.Enabled = enabled;
        }

        /// <summary>The user-facing outcome: a safe message, a code and a correlation id. Never an exception.</summary>
        private void ShowResult(CommandResult result)
        {
            lblResult.Visible = true;
            lblResultDetail.Visible = true;
            PaintResult(result.Success);

            lblResult.Text = result.Errors != null && result.Errors.Count > 0
                ? result.UserMessage + " " + string.Join(" ", result.Errors)
                : result.UserMessage;
            lblResultDetail.Text = result.Detail;

            if (result.Success)
            {
                SetStatusBar(result.NewVersion.HasValue
                    ? $"CommandResult.Ok — committed · v{result.NewVersion.Value - 1} → v{result.NewVersion.Value} · audited"
                    : "CommandResult.Ok — committed · audited");
                AlertBox.Show(result.UserMessage, MessageBoxIcon.Information,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            }
            else
            {
                SetStatusBar($"CommandResult.Fail — {result.ErrorCode} · rolled back · audited");
                AlertBox.Show(result.UserMessage, MessageBoxIcon.Warning,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            }
        }

        private void PaintResult(bool ok, bool warning = false)
        {
            if (ok && !warning)
            {
                lblResult.BackColor = System.Drawing.Color.FromArgb(240, 249, 243);
                lblResult.ForeColor = System.Drawing.Color.FromArgb(21, 95, 51);
            }
            else if (warning)
            {
                lblResult.BackColor = System.Drawing.Color.FromArgb(253, 245, 230);
                lblResult.ForeColor = System.Drawing.Color.FromArgb(150, 96, 12);
            }
            else
            {
                lblResult.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
                lblResult.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            }
        }

        private void HideResult()
        {
            lblResult.Visible = false;
            lblResultDetail.Visible = false;
        }

        private void SetStatusBar(string text)
        {
            lblStatusBar.Text = text;
        }

        private void UpdateHeader()
        {
            lblTenant.Text = "tenant: " + _session.TenantId;
            lblUser.Text = $"Signed in: {_session.UserId} · {_session.Role}";
            lblContexts.Text = $"ctx {_database.ContextsCreated} · live {_database.LiveContexts}";
            lblCorrelation.Text = "corr " + _session.LastCorrelationId;
        }

        private bool RequireSelection()
        {
            if (SelectedRow != null)
                return true;

            Warn("Select a work order in the queue first.");
            return false;
        }

        private void Warn(string message)
        {
            lblResult.Visible = true;
            lblResultDetail.Visible = true;
            PaintResult(false, warning: true);
            lblResult.Text = message;
            lblResultDetail.Text = "nothing was sent to the service";
            AlertBox.Show(message, MessageBoxIcon.Warning,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        /// <summary>
        /// The last line of defence. Every mapped failure already came back as a CommandResult, so
        /// reaching here means a bug in the screen — the user still gets a safe message and an id.
        /// </summary>
        private void ShowUnexpected(Exception exception)
        {
            Trace(TraceLayer.UI, $"unhandled {exception.GetType().Name} in the screen — the user never sees the exception");
            lblResult.Visible = true;
            lblResultDetail.Visible = true;
            PaintResult(false);
            lblResult.Text = "The action could not be completed. Check the log for details.";
            lblResultDetail.Text = "correlation " + _session.LastCorrelationId;
            SetStatusBar("UNEXPECTED · correlation " + _session.LastCorrelationId);
            AlertBox.Show("The action could not be completed. Check the log for details.", MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        private static string Describe(WorkQueueRow row)
            => row == null ? "no work order selected" : $"work order {row.Number} · {row.Status.ToLowerInvariant()}";

        #endregion

        #region Session cleanup (called from the Designer's Dispose)

        /// <summary>
        /// The session's database and the anti-pattern's field-held context die with the screen. Closing
        /// the SQLite connection deletes the in-memory database — nothing survives the session.
        /// </summary>
        private void DisposeSessionResources()
        {
            _shuttingDown = true;      // the trace card is on its way out; nothing more goes into it

            if (_lifetimeDemo != null)
                _lifetimeDemo.Dispose();

            if (_database != null)
                _database.Dispose();
        }

        #endregion
    }
}
