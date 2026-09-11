using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Data;
using EnterpriseOps.Services;
using EnterpriseOps.Services.Commands;
using EnterpriseOps.Services.Queries;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// EnterpriseOps — Approvals: the work queue (<see cref="WorkQueueRow"/> projections from
    /// <see cref="IWorkOrderQueryService"/>) next to the approve panel. The Approve handler builds an
    /// <see cref="ApproveWorkOrderCommand"/>, hands it to <see cref="IWorkOrderCommandService"/> and shows the
    /// <see cref="CommandResult"/>. No DbContext, no LINQ over entities, no transaction in this file.
    /// </summary>
    public partial class ApprovalsPage : Page
    {
        // Per-session instances, never statics: two users must never share a database or a session context.
        private readonly SessionContext _session;
        private readonly SessionDatabase _database;
        private readonly IWorkOrderCommandService _commands;
        private readonly IWorkOrderQueryService _queries;

        private readonly BindingSource _queue = new BindingSource();

        public ApprovalsPage()
        {
            InitializeComponent();

            _session = new SessionContext(Application.SessionId);

            // The session's in-memory SQLite database; every DbContext over it is per-operation
            // (docs/DbContextLifetimeDecision.md).
            var log = new ActivityTrace();
            _database = new SessionDatabase(log);

            _commands = new WorkOrderCommandService(_database, log, () => _session.CommandTimeout);
            _queries = new WorkOrderQueryService(_database, log);

            dgvWorkQueue.DataSource = _queue;
        }

        private async void ApprovalsPage_Load(object sender, EventArgs e)
        {
            try
            {
                await RefreshQueueAsync(2002);
                SetStatusBar($"Ready — {Describe(SelectedRow)}");
            }
            catch (Exception ex)
            {
                ShowUnexpected(ex);
            }
            finally
            {
                // The awaits end after the request returned: push the final UI state to the browser.
                Application.Update(this);
            }
        }

        #region Approve

        private async void btnApprove_Click(object sender, EventArgs e)
        {
            if (!RequireSelection())
                return;

            HideResult();
            SetStatusBar("ApproveAsync — short-lived DbContext · transaction open…");
            SetButtonsEnabled(false);
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
                SetButtonsEnabled(true);
                await SafeRefreshQueueAsync();

                // The awaits end after the request returned: push the final UI state to the browser.
                Application.Update(this);
            }
        }

        /// <summary>Intent as data: id, tenant, user, comment, and the version the user was looking at.</summary>
        private ApproveWorkOrderCommand BuildApproveCommand()
        {
            var row = SelectedRow;
            return new ApproveWorkOrderCommand(
                row.Id,
                _session.TenantId,
                _session.UserId,
                txtComment.Text,
                row.Version);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtComment.Text = "";
            HideResult();
            SetStatusBar($"Ready — {Describe(SelectedRow)}");
        }

        #endregion

        #region Work queue: search and selection

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                await RefreshQueueAsync();
            }
            catch (Exception ex)
            {
                ShowUnexpected(ex);
            }
            finally
            {
                // The awaits end after the request returned: push the final UI state to the browser.
                Application.Update(this);
            }
        }

        private void dgvWorkQueue_SelectionChanged(object sender, EventArgs e)
        {
            var row = SelectedRow;
            if (row == null)
            {
                lblWorkOrder.Text = "Approve work order";
                lblVersion.Text = "select a work order";
                return;
            }

            lblWorkOrder.Text = $"Approve work order {row.Number} — {row.Title}";
            lblVersion.Text = $"v{row.Version} · tenant {_session.TenantId}";
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
            lblQueueTitle.Text = $"Work queue ({page.Rows.Count} of {page.Total})";

            SelectById(keep);
        }

        /// <summary>
        /// The refresh after a command. The command's result is already on screen, and an
        /// <c>async void</c> handler must not end with an exception escaping from its finally block.
        /// </summary>
        private async Task SafeRefreshQueueAsync()
        {
            try
            {
                await RefreshQueueAsync();
            }
            catch (Exception exception)
            {
                System.Diagnostics.Trace.TraceError(exception.ToString());
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

        #endregion

        #region Audit log

        private async void btnAudit_Click(object sender, EventArgs e)
        {
            try
            {
                var audit = await _queries.GetAuditAsync(NewCommand(), null, 100, CancellationToken.None);

                if (!audit.Allowed)
                {
                    Warn(audit.DeniedReason);
                    return;
                }

                var dialog = new AuditLogDialog(_session.TenantId, audit.Rows);
                await dialog.ShowDialogAsync();
            }
            catch (Exception ex)
            {
                ShowUnexpected(ex);
            }
            finally
            {
                // The awaits end after the request returned: push the final UI state to the browser.
                Application.Update(this);
            }
        }

        #endregion

        #region Result banner and status bar

        /// <summary>A new correlation id per command — it reaches the result and the audit row.</summary>
        private CommandContext NewCommand() => _session.NewCommandContext();

        private void SetButtonsEnabled(bool enabled)
        {
            btnApprove.Enabled = enabled && SelectedRow != null;
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
            lblResultDetail.Visible = false;
            PaintResult(false, warning: true);
            lblResult.Text = message;
            AlertBox.Show(message, MessageBoxIcon.Warning,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        /// <summary>
        /// The last line of defence. Every expected failure already came back as a CommandResult, so
        /// reaching here means a bug in the screen — the user still gets a safe message and an id.
        /// </summary>
        private void ShowUnexpected(Exception exception)
        {
            System.Diagnostics.Trace.TraceError(exception.ToString());
            lblResult.Visible = true;
            lblResultDetail.Visible = true;
            PaintResult(false);
            lblResult.Text = "The action could not be completed. Check the log for details.";
            lblResultDetail.Text = "correlation " + _session.LastCorrelationId;
            SetStatusBar("UNEXPECTED · correlation " + _session.LastCorrelationId);
            AlertBox.Show("The action could not be completed. Check the log for details.", MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        /// <summary>"work order WO-2002 · on hold".</summary>
        private static string Describe(WorkQueueRow row)
            => row == null
                ? "no work order selected"
                : $"work order {row.Number} · {Regex.Replace(row.Status, "(?<=[a-z])(?=[A-Z])", " ").ToLowerInvariant()}";

        #endregion

        /// <summary>The session's database dies with the screen: closing the SQLite connection deletes it.</summary>
        private void DisposeSessionResources()
        {
            _database?.Dispose();
        }
    }
}
