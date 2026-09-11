using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;
using EnterpriseOps.Services;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// EnterpriseOps — Work queue + audit.
    ///
    /// The work queue with its three sensitive commands (Approve, Approve pending export, Export data), the
    /// customer note of the selected site, and the audit log: every Demand, granted or denied, filtered by
    /// user · permission · result, with the status bar underneath.
    ///
    /// The screen owns UI state only (labels, grids, banner, which buttons are visible). Which identity signed
    /// in, who may do what, what is written to the audit log and how untrusted text is encoded are decided in
    /// the services.
    /// </summary>
    public partial class AuditLogPage : Page
    {
        private readonly ServiceRegistry _services;

        private CommandContext _current;
        private List<WorkQueueRow> _queueRows = new List<WorkQueueRow>();
        private WorkOrderNote _note;
        private bool _suppressFilterEvents;

        /// <summary>Designer / default constructor: its own session graph, so the file opens in the Designer.</summary>
        public AuditLogPage() : this(new ServiceRegistry(new ActivityTrace()))
        {
        }

        public AuditLogPage(ServiceRegistry services)
        {
            InitializeComponent();

            _services = services;
        }

        /// <summary>The command running right now: one correlation id per click, minted from the session.</summary>
        private CommandContext CurrentContext => _current ?? NewCommand();

        #region Event handlers

        /// <summary>Nothing on this screen has data, and no service can be called, until a verified identity exists.</summary>
        private async void AuditLogPage_Load(object sender, EventArgs e)
        {
            LockScreen("Not signed in");
            await RequireSignInAsync();
        }

        /// <summary>Approve the selected work order. The service demands the permission on the record's tenant.</summary>
        private async void btnApprove_Click(object sender, EventArgs e)
        {
            WorkQueueRow row = SelectedRow;
            if (row == null)
            {
                ShowBanner("Select a work order first.", BannerKind.Warning);
                return;
            }

            BeginBusy($"approving {row.Reference}…");
            try
            {
                CommandResult result = await _services.WorkOrders.ApproveAsync(CurrentContext, row.Id);
                await ShowResultAsync(result, $"{row.Reference} approved");
                await ReloadQueueAsync();
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

        /// <summary>
        /// The handler does not ask whether the user may export — it calls the service, and the service demands
        /// the permission where the action executes.
        /// </summary>
        private async void btnExport_Click(object sender, EventArgs e)
        {
            BeginBusy("ExportAsync — PermissionService.Demand(ExportData)…");
            try
            {
                ExportResult result = await _services.Exports.RequestExportAsync(CurrentContext);
                await ShowExportResultAsync(result, "You don't have permission to export data.");
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

        /// <summary>Release a pending export — a different person from the one who requested it.</summary>
        private async void btnApproveExport_Click(object sender, EventArgs e)
        {
            ExportApproval pending = _services.Exports.PendingFor(_services.Session.TenantId).FirstOrDefault();
            if (pending == null)
            {
                ShowBanner("No export is waiting for approval.", BannerKind.Warning);
                return;
            }

            BeginBusy($"approving export #{pending.Id}…");
            try
            {
                ExportResult result = await _services.Exports.ApproveExportAsync(CurrentContext, pending.Id);
                await ShowExportResultAsync(result, null);
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

        /// <summary>The three filter combos: the service filters, the grid only shows.</summary>
        private async void auditFilter_Changed(object sender, EventArgs e)
        {
            if (_suppressFilterEvents || !_services.Session.IsAuthenticated) return;

            try
            {
                await RefreshAuditAsync();
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
            finally
            {
                Application.Update(this);
            }
        }

        #endregion

        #region Sign-in

        /// <summary>Opens the sign-in gate and, when it returns a verified identity, unlocks the screen.</summary>
        private async Task RequireSignInAsync()
        {
            var gate = new SignInGate(_services.SignIn);
            try
            {
                DialogResult result = await gate.ShowDialogAsync();
                if (result != DialogResult.OK || !_services.Session.IsAuthenticated)
                {
                    LockScreen("Signed out");
                    return;
                }

                await OnSignedInAsync(gate.Identity);
            }
            finally
            {
                gate.Dispose();
                Application.Update(this);
            }
        }

        private async Task OnSignedInAsync(MappedIdentity identity)
        {
            _current = null;

            ApplyPermissionsToUi();

            lblStatusBar.Text = $"Work queue — signed in via SSO · claims mapped · {identity.UserId} ({identity.RoleList})";

            if (identity.Roles.Count == 0)
                ShowBanner($"Signed in, but no group of this account maps to a role ({string.Join(", ", identity.UnmappedGroups)}). "
                         + "Every action will be refused.", BannerKind.Warning);
            else
                HideBanner();

            await ReloadQueueAsync();
            LoadNote();
            await RefreshAuditAsync();
        }

        /// <summary>
        /// Screens adapt, services enforce: <c>Has</c> only hides what the user cannot do, and the services demand
        /// the permission again. Export data is offered to every signed-in user —
        /// <see cref="ExportService.RequestExportAsync"/> demands ExportData and refuses (and audits) anyone without it.
        /// </summary>
        private void ApplyPermissionsToUi()
        {
            CommandContext context = CurrentContext;

            btnApprove.Visible = _services.Permissions.Has(context, Permission.ApproveWorkOrders);
            btnApproveExport.Visible = _services.Permissions.Has(context, Permission.ApproveExport);
            btnExport.Visible = true;

            SetButtons(true);
        }

        private void LockScreen(string reason)
        {
            btnApprove.Visible = btnExport.Visible = btnApproveExport.Visible = false;

            _queueRows = new List<WorkQueueRow>();
            dgvWorkOrders.DataSource = null;
            dgvAudit.DataSource = null;
            lblNote.Text = "";
            lblNoteSource.Text = "";

            lblStatusBar.Text = reason;
            HideBanner();
        }

        #endregion

        #region Loading and showing

        private async Task ReloadQueueAsync()
        {
            WorkQueueResult result = await _services.WorkOrders.LoadQueueAsync(CurrentContext);

            if (!result.Succeeded)
            {
                _queueRows = new List<WorkQueueRow>();
                dgvWorkOrders.DataSource = null;
                ShowBanner(FriendlyDenial(result.FirstError), BannerKind.Error);
                return;
            }

            _queueRows = result.Rows.ToList();
            dgvWorkOrders.DataSource = _queueRows;
        }

        private void LoadNote()
        {
            _note = _services.NoteStore.ForWorkOrder(_services.Session.TenantId, 1002).LastOrDefault();
            if (_note == null) return;

            // The screen sets the two properties the service chose; it never picks the encoding itself.
            NoteRenderResult result = _services.Notes.Prepare(CurrentContext, _note);
            lblNote.AllowHtml = result.AllowHtml;
            lblNote.Text = result.Html;
            lblNoteSource.Text = $"WO-1002 · note #{_note.Id} · from {_note.Source}";
        }

        private async Task RefreshAuditAsync()
        {
            if (!_services.Session.IsAuthenticated) return;

            AuditQueryResult result = await _services.AuditQuery.QueryAsync(CurrentContext, ReadFilter());

            RebuildFilterCombos();

            dgvAudit.DataSource = result.Rows.ToList();
            PaintDenials(result);
        }

        private void PaintDenials(AuditQueryResult result)
        {
            for (int i = 0; i < dgvAudit.Rows.Count && i < result.Rows.Count; i++)
            {
                bool denied = result.Rows[i].IsDenied;
                dgvAudit.Rows[i].DefaultCellStyle.BackColor = denied
                    ? System.Drawing.Color.FromArgb(253, 243, 243)
                    : System.Drawing.Color.White;
                dgvAudit.Rows[i].DefaultCellStyle.ForeColor = denied
                    ? System.Drawing.Color.FromArgb(192, 57, 43)
                    : System.Drawing.Color.FromArgb(31, 45, 58);
            }
        }

        private void RebuildFilterCombos()
        {
            var values = _services.AuditQuery.FilterValues(CurrentContext);

            _suppressFilterEvents = true;
            try
            {
                Repopulate(cboUser, "User: any", values.Users, _filterUser);
                Repopulate(cboPermission, "Permission: any", values.Actions, _filterAction);
                Repopulate(cboResult, "Result: any", ResultChoices, _filterResult);
            }
            finally
            {
                _suppressFilterEvents = false;
            }
        }

        private static readonly List<string> ResultChoices = new List<string> { "OK", "DENIED", "PENDING", "FAILED" };

        private static void Repopulate(ComboBox combo, string anyLabel, List<string> values, string selected)
        {
            combo.Items.Clear();
            combo.Items.Add(anyLabel);
            foreach (string value in values) combo.Items.Add(value);

            int index = selected == AuditFilter.Any ? 0 : values.IndexOf(selected) + 1;
            combo.SelectedIndex = index > 0 && index < combo.Items.Count ? index : 0;
        }

        private string _filterUser = AuditFilter.Any;
        private string _filterAction = AuditFilter.Any;
        private string _filterResult = AuditFilter.Any;

        /// <summary>Reads the three combos into a filter object — the service does the filtering, not the grid.</summary>
        private AuditFilter ReadFilter()
        {
            _filterUser = ValueOf(cboUser);
            _filterAction = ValueOf(cboPermission);
            _filterResult = ValueOf(cboResult);
            return new AuditFilter { UserId = _filterUser, Action = _filterAction, Result = _filterResult };
        }

        private static string ValueOf(ComboBox combo)
            => combo.SelectedIndex <= 0 ? AuditFilter.Any : Convert.ToString(combo.Items[combo.SelectedIndex]);

        /// <summary>A command result → banner and status bar. Denied and failed look different on purpose.</summary>
        private async Task ShowResultAsync(CommandResult result, string successText)
        {
            if (result.Denied)
            {
                ShowBanner(FriendlyDenial(result.FirstError), BannerKind.Error);
                lblStatusBar.Text = $"{DenialPrefix(result.FirstError)}{result.FirstError} · audited · nothing changed";
            }
            else if (!result.Succeeded)
            {
                ShowBanner(result.FirstError + $"  (ref {result.CorrelationId})", BannerKind.Warning);
                lblStatusBar.Text = $"{result.Summary} — ref {result.CorrelationId}";
            }
            else
            {
                ShowBanner(successText, BannerKind.Success);
                lblStatusBar.Text = $"{successText} · corr {result.CorrelationId}";
            }

            await RefreshAuditAsync();
        }

        private async Task ShowExportResultAsync(ExportResult result, string deniedText)
        {
            if (result.Denied)
            {
                ShowBanner(deniedText ?? FriendlyDenial(result.FirstError), BannerKind.Error);
                lblStatusBar.Text = $"{DenialPrefix(result.FirstError)}{result.FirstError} · audited · nothing exported";
            }
            else if (result.Pending != null)
            {
                ShowBanner(result.FirstError, BannerKind.Warning);
                lblStatusBar.Text = $"export #{result.Pending.Id} · {result.Pending.RowCount} rows · requested by {result.Pending.RequestedBy} · awaiting a second approver";
            }
            else if (!result.Succeeded)
            {
                ShowBanner(result.FirstError + $"  (ref {result.CorrelationId})", BannerKind.Warning);
                lblStatusBar.Text = $"{result.FirstError} · ref {result.CorrelationId}";
            }
            else
            {
                ShowBanner($"{result.RowsExported} rows exported.", BannerKind.Success);
                lblStatusBar.Text = $"ExportData ok — {result.RowsExported} rows · corr {result.CorrelationId}";
            }

            await RefreshAuditAsync();
        }

        /// <summary>Unexpected failure: generic message plus the correlation id; the details go to the server log.</summary>
        private void ReportFailure(Exception ex)
        {
            string correlationId = CurrentContext.CorrelationId;
            System.Diagnostics.Trace.TraceError($"AuditLogPage: {ex} — corr {correlationId}");

            ShowBanner($"The action could not be completed.  (ref {correlationId})", BannerKind.Error);
            lblStatusBar.Text = $"failed — ref {correlationId}";
            AlertBox.Show("The action could not be completed.", MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion

        #region Helpers

        private enum BannerKind { Success, Warning, Error }

        /// <summary>The selected queue row, read from the bound projection rather than from cell text.</summary>
        private WorkQueueRow SelectedRow
            => dgvWorkOrders.CurrentRow?.DataBoundItem as WorkQueueRow ?? _queueRows.FirstOrDefault();

        private CommandContext NewCommand()
        {
            _current = _services.Session.BeginCommand();
            return _current;
        }

        /// <summary>A new command starts: fresh correlation id, buttons off. Sent before the first await.</summary>
        private void BeginBusy(string text)
        {
            _current = null;
            NewCommand();
            SetButtons(false);
            lblStatusBar.Text = text;
        }

        /// <summary>After the awaits: buttons back on, push the pending changes.</summary>
        private void EndBusy()
        {
            SetButtons(true);
            Application.Update(this);
        }

        private void SetButtons(bool enabled)
        {
            btnApprove.Enabled = enabled;
            btnExport.Enabled = enabled;
            btnApproveExport.Enabled = enabled;
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

        private static bool IsMissingPermission(string serviceMessage)
            => serviceMessage.StartsWith("Missing permission", StringComparison.Ordinal);

        private static string DenialPrefix(string serviceMessage)
            => IsMissingPermission(serviceMessage) ? "UnauthorizedAccessException — " : "";

        /// <summary>
        /// A denial may name the permission: it helps the user ask for it and the help desk grant it. What it must
        /// never do is leak the rule behind it.
        /// </summary>
        private static string FriendlyDenial(string serviceMessage)
            => IsMissingPermission(serviceMessage)
                ? $"You don't have permission for that action. ({serviceMessage})"
                : serviceMessage;

        #endregion
    }
}
