using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Domain;
using EnterpriseOps.Resources;
using EnterpriseOps.Security;
using EnterpriseOps.Services;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// EnterpriseOps — Work Order Editor: tenant selection, a tenant-aware session, a correlation id on every
    /// command, optimistic concurrency on the save and the conflict dialog.
    ///
    /// Where the state lives:
    ///  • session state — <see cref="SessionContext"/>, created once at sign-in and put in Application.Session:
    ///    identity, roles, entitlements, culture and the current tenant;
    ///  • tab state — <see cref="_open"/>, the work order this window is editing and the version token it
    ///    loaded. It is a field of the screen, never of the session, because a second tab is a second editor;
    ///  • request state — <see cref="_current"/>, a <see cref="CommandContext"/> with a fresh correlation id
    ///    per user action, passed explicitly into every service call;
    ///  • application state — the store and the audit trail, shared by every session, documented and locked.
    /// </summary>
    public partial class WorkOrderEditorPage : Page
    {
        private readonly ServiceRegistry _services;
        private readonly SessionContext _session;
        private readonly ErrorLog _log;

        /// <summary>Request state: the command running right now — one correlation id per user action.</summary>
        private CommandContext _current;

        /// <summary>Tab state: the work order this window has open, and the version it loaded.</summary>
        private WorkOrderEditModel _open;

        private List<WorkQueueRow> _rows = new List<WorkQueueRow>();
        private bool _binding;

        /// <summary>Session entry point: the course's manager, entitled to contoso and fabrikam.</summary>
        public WorkOrderEditorPage() : this("ana.ops")
        {
        }

        public WorkOrderEditorPage(string userId)
        {
            InitializeComponent();

            _services = ServiceRegistry.CreateForSession(userId, CurrentSessionId());
            _session = _services.Session;
            _log = _services.Log;

            StoreSessionContext();
        }

        /// <summary>The command running right now: same session, same tenant, one correlation id per action.</summary>
        private CommandContext CurrentContext => _current ?? NewCommand("ui.action");

        #region Event handlers

        private void WorkOrderEditorPage_Load(object sender, EventArgs e)
        {
            BindTenants();
            BindStatuses();
            LoadQueue();
        }

        /// <summary>
        /// The user picked a tenant. The dropdown is a request, not a decision — the session checks the
        /// entitlement, and the open editor is dropped because its record belongs to the previous tenant.
        /// </summary>
        private async void cboTenant_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_binding)
                return;

            var tenant = cboTenant.SelectedItem as Tenant;
            BeginBusy("session.switch-tenant");
            try
            {
                TenantSwitchResult result = _session.SwitchTenant(tenant?.Id);
                await ApplyTenantSwitchAsync(result);
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

        /// <summary>Opens the selected work order. The guard runs before the record is read.</summary>
        private async void btnOpen_Click(object sender, EventArgs e)
        {
            int id = SelectedWorkOrderId();
            if (id == 0)
            {
                AlertBox.Show(UiText.SelectAWorkOrder, MessageBoxIcon.Information,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return;
            }

            BeginBusy("work-order.open");
            try
            {
                WorkOrderEditModel model = await _services.WorkOrders.OpenAsync(CurrentContext, id);
                ShowOpened(model);
            }
            catch (CrossTenantAccessException ex)
            {
                ShowCrossTenantRejection(ex);
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
        /// Saves the edit. The concurrency check lives in the service: when another session saved first, the
        /// result comes back stale and the conflict dialog opens.
        /// </summary>
        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (_open == null)
            {
                AlertBox.Show(UiText.OpenBeforeSaving, MessageBoxIcon.Information,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return;
            }

            BeginBusy("work-order.save");
            try
            {
                var command = new SaveWorkOrderCommand(_open.Id, _open.TenantId, txtTitle.Text, SelectedStatus(), _open.Token);
                SaveWorkOrderResult result = await _services.WorkOrders.SaveAsync(CurrentContext, command);
                await ShowSaveResultAsync(result);
            }
            catch (CrossTenantAccessException ex)
            {
                ShowCrossTenantRejection(ex);
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

        /// <summary>Loads the work queue for the current tenant.</summary>
        private async void LoadQueue()
        {
            BeginBusy("work-queue.load");
            try
            {
                await ReloadQueueAsync();
                lblStatusBar.Text = $"{_rows.Count} work orders · {TenantDirectory.NameOf(_session.TenantId)}";
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

        private async Task ReloadQueueAsync()
        {
            PagedResult<WorkQueueRow> page = await _services.WorkOrders.QueryAsync(CurrentContext, new WorkQueueQuery());

            _rows = new List<WorkQueueRow>(page.Rows);
            dgvWorkQueue.DataSource = _rows;

            lblQueueTitle.Text = $"Work queue · {TenantDirectory.NameOf(_session.TenantId)}";
        }

        /// <summary>The tenant switch outcome. Rejected → amber banner and nothing changes, not even the dropdown.</summary>
        private async Task ApplyTenantSwitchAsync(TenantSwitchResult result)
        {
            if (!result.Succeeded)
            {
                ShowBanner($"Tenant not switched — {result.Reason}. You are still working in " +
                           $"{TenantDirectory.NameOf(result.TenantId)}.", BannerKind.Warning);
                SelectCurrentTenant();
                return;
            }

            if (!result.Changed)
                return;

            ClearEditor();
            HideBanner();
            await ReloadQueueAsync();
            lblStatusBar.Text = $"{_rows.Count} work orders · {TenantDirectory.NameOf(result.TenantId)}";
        }

        /// <summary>A work order was opened: the editor now owns that record and its version token.</summary>
        private void ShowOpened(WorkOrderEditModel model)
        {
            if (model == null)
            {
                ShowBanner("That work order no longer exists.", BannerKind.Warning);
                return;
            }

            _open = model;
            _binding = true;
            txtTitle.Text = model.Title;
            cboStatus.SelectedItem = model.Status.ToString();
            _binding = false;

            txtVersion.Text = model.Token.ToDisplayText();
            lblEditCaption.Text = $"Work order {model.Id} — Edit";
            HideBanner();
            lblStatusBar.Text = $"Editing — expected version {model.Token.ToDisplayText()}";
        }

        /// <summary>Saved, rejected, or stale — the three answers a save can give.</summary>
        private async Task ShowSaveResultAsync(SaveWorkOrderResult result)
        {
            if (result.Succeeded)
            {
                ConcurrencyToken previous = _open.Token;
                _open = result.Saved;
                txtVersion.Text = result.NewVersion.ToDisplayText();
                HideBanner();
                lblStatusBar.Text = $"Saved — {previous.ToDisplayText()} → {result.NewVersion.ToDisplayText()} · correlation {result.CorrelationId}";
                await ReloadQueueAsync();
                return;
            }

            if (result.IsConflict)
            {
                await ResolveConflictAsync(result.Conflict);
                return;
            }

            ShowBanner(string.Join(" ", result.Errors), BannerKind.Warning);
            lblStatusBar.Text = $"Save rejected — nothing written · correlation {result.CorrelationId}";
        }

        /// <summary>
        /// The stale save. The screen hands the ConflictInfo to the dialog and then does what the user chose.
        /// Reload is the only path that changes anything on screen.
        /// </summary>
        private async Task ResolveConflictAsync(ConflictInfo conflict)
        {
            lblStatusBar.Text = $"Save rejected — expected {conflict.Expected.ToDisplayText()}, found {conflict.Found.ToDisplayText()} · " +
                                $"correlation {conflict.CorrelationId}";

            var dialog = new ConflictDialog(conflict, CurrentContext, _services.Conflicts);
            await dialog.ShowDialogAsync();

            if (dialog.Resolution == ConflictResolution.Reload)
            {
                await ReloadLatestAsync();
                return;
            }

            ShowBanner($"Conflict left open — your edit is still on screen at {conflict.Expected.ToDisplayText()}, " +
                       $"the record is at {conflict.Found.ToDisplayText()}. Nothing was saved and nothing was lost.",
                BannerKind.Warning);
        }

        /// <summary>The conflict dialog's Reload path: re-open the record at its current version.</summary>
        private async Task ReloadLatestAsync()
        {
            WorkOrderEditModel latest = await _services.WorkOrders.OpenAsync(CurrentContext, _open.Id);
            ShowOpened(latest);
            if (latest == null)
                return;

            lblStatusBar.Text = $"Reloaded {latest.Token.ToDisplayText()} — re-apply your edit on the latest version";
            await ReloadQueueAsync();
        }

        /// <summary>A cross-tenant read: rejected by the guard, logged with the correlation id, explained plainly.</summary>
        private void ShowCrossTenantRejection(CrossTenantAccessException ex)
        {
            _log.Error(ex, ex.CorrelationId);
            ShowBanner($"{UiText.NotYourTenant}  (ref {ex.CorrelationId})", BannerKind.Error);
            lblStatusBar.Text = $"Access denied · correlation {ex.CorrelationId}";
        }

        /// <summary>Unexpected failure: log it with the correlation id, say something generic, keep the screen usable.</summary>
        private void ReportFailure(Exception ex)
        {
            _log.Error(ex, CurrentContext.CorrelationId);
            ShowBanner($"{UiText.ActionFailed}  (ref {CurrentContext.CorrelationId})", BannerKind.Error);
            AlertBox.Show(UiText.ActionFailed, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion

        #region Helpers

        private enum BannerKind { Warning, Error }

        /// <summary>The session's own id, from Wisej. Falls back to a new id when there is no live session (designer).</summary>
        private static string CurrentSessionId()
        {
            try
            {
                return Application.SessionId ?? Guid.NewGuid().ToString("N");
            }
            catch (Exception)
            {
                return Guid.NewGuid().ToString("N");
            }
        }

        /// <summary>
        /// The session context goes into <c>Application.Session</c> — the per-user bag Wisej keeps alive for
        /// this browser session. That is its home: not a static, not a control's Tag, not a cookie.
        /// </summary>
        private void StoreSessionContext()
        {
            try
            {
                Application.Session.Context = _session;
            }
            catch (Exception)
            {
                // No live session (designer).
            }
        }

        private CommandContext NewCommand(string commandName)
        {
            _current = _session.BeginCommand(commandName);
            return _current;
        }

        /// <summary>A new command starts: fresh correlation id, controls off until it completes.</summary>
        private void BeginBusy(string commandName)
        {
            NewCommand(commandName);
            SetControlsEnabled(false);
        }

        /// <summary>After the awaits: controls back on, push the pending changes.</summary>
        private void EndBusy()
        {
            SetControlsEnabled(true);
            Application.Update(this);
        }

        private void SetControlsEnabled(bool enabled)
        {
            btnOpen.Enabled = enabled;
            btnSave.Enabled = enabled;
            cboTenant.Enabled = enabled;
        }

        /// <summary>Only the tenants the verified claims allow ever reach the dropdown.</summary>
        private void BindTenants()
        {
            _binding = true;
            cboTenant.Items.Clear();
            foreach (Tenant tenant in TenantDirectory.EntitledFor(_session))
                cboTenant.Items.Add(tenant);
            SelectCurrentTenant();
            _binding = false;
        }

        private void SelectCurrentTenant()
        {
            bool wasBinding = _binding;
            _binding = true;
            for (int i = 0; i < cboTenant.Items.Count; i++)
            {
                var tenant = cboTenant.Items[i] as Tenant;
                if (tenant != null && StringComparer.Ordinal.Equals(tenant.Id, _session.TenantId))
                {
                    cboTenant.SelectedIndex = i;
                    break;
                }
            }
            _binding = wasBinding;
        }

        private void BindStatuses()
        {
            _binding = true;
            cboStatus.Items.Clear();
            foreach (string name in Enum.GetNames(typeof(WorkOrderStatus)))
                cboStatus.Items.Add(name);
            _binding = false;
        }

        private WorkOrderStatus SelectedStatus()
        {
            string name = cboStatus.SelectedItem as string;
            return name != null && Enum.TryParse(name, out WorkOrderStatus status) ? status : _open.Status;
        }

        private int SelectedWorkOrderId()
        {
            DataGridViewRow row = dgvWorkQueue.CurrentRow;
            if (row == null || row.Index < 0 || row.Index >= _rows.Count)
                return 0;

            return _rows[row.Index].Id;
        }

        private void ClearEditor()
        {
            _open = null;
            _binding = true;
            txtTitle.Text = "";
            cboStatus.SelectedIndex = -1;
            _binding = false;
            txtVersion.Text = "—";
            lblEditCaption.Text = "No work order open";
        }

        private void ShowBanner(string text, BannerKind kind)
        {
            lblBanner.Text = text;
            if (kind == BannerKind.Warning)
            {
                lblBanner.BackColor = System.Drawing.Color.FromArgb(255, 244, 229);
                lblBanner.ForeColor = System.Drawing.Color.FromArgb(146, 64, 14);
            }
            else
            {
                lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
                lblBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            }
            lblBanner.Visible = true;
        }

        private void HideBanner()
        {
            lblBanner.Visible = false;
        }

        #endregion
    }
}
