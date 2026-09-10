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
    /// EnterpriseOps — Work Order Editor. The Module 3 screen: tenant selection, a tenant-aware session,
    /// a correlation id on every command, optimistic concurrency on the save and the conflict dialog.
    ///
    /// Header:      the screen name, <c>cboTenant</c>, the signed-in user and the correlation id of the
    ///              command running right now.
    /// Left card:   the work queue for the current tenant, the editor for one work order (title, status and
    ///              the version token it was loaded at), the banner and the dark footer.
    /// Right card:  the live activity trace — every layer's decision, tagged UI → / Session: / Security: /
    ///              Service: / Data: / Audit:.
    /// Bottom bar:  four failure paths (another session saves first · a cross-tenant read · a spoofed tenant
    ///              value · the static-state leak), the static-state audit, the recovery and Clear trace.
    ///
    /// Where the state lives — the point of the module:
    ///  • session state — <see cref="SessionContext"/>, created once at sign-in and put in Application.Session:
    ///    identity, roles, entitlements, culture and the current tenant;
    ///  • tab state — <see cref="_open"/>, the work order this window is editing and the version token it
    ///    loaded. It is a field of the screen, never of the session, because a second tab is a second editor;
    ///  • request state — <see cref="_current"/>, a <see cref="CommandContext"/> with a fresh correlation id
    ///    per click, passed explicitly into every service call;
    ///  • application state — the store and the audit trail, shared by every session, documented and locked.
    ///
    /// Nothing here is static, and nothing here decides: the handlers are a few lines each and every decision
    /// belongs to a service in <c>EnterpriseOps.Services</c>.
    /// </summary>
    public partial class WorkOrderEditorPage : Page
    {
        // Per-session, per-screen. Instance fields — two users must never share a session, a trace or an editor.
        private readonly ActivityTrace _trace;
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

            _trace = new ActivityTrace();
            _trace.EntryAdded += trace_EntryAdded;

            _services = ServiceRegistry.CreateForSession(userId, CurrentSessionId(), _trace);
            _session = _services.Session;
            _log = _services.Log;

            StoreSessionContext();
        }

        public string ScreenName => "WorkOrderEditorPage";

        /// <summary>The command running right now: same session, same tenant, one correlation id per click.</summary>
        private CommandContext CurrentContext => _current ?? NewCommand("ui.action");

        #region Event handlers — thin, one service call each (the shape the lab code check expects)

        private void WorkOrderEditorPage_Load(object sender, EventArgs e)
        {
            _trace.Ui($"{ScreenName}_Load → bind tenants, bind statuses, load the queue for '{_session.TenantId}'");
            BindTenants();
            BindStatuses();
            ShowSignedIn();
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
            BeginBusy($"Switching to {tenant?.Id}…", "session.switch-tenant");
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

            BeginBusy($"Opening #{id}…", "work-order.open");
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
        /// The success path — and, once another session has saved first, the failure path. Identical code:
        /// the handler does not know which it will be, because the concurrency check lives in the service.
        /// </summary>
        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (_open == null)
            {
                AlertBox.Show(UiText.OpenBeforeSaving, MessageBoxIcon.Information,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return;
            }

            BeginBusy($"Saving #{_open.Id}…", "work-order.save");
            try
            {
                var command = new SaveWorkOrderCommand(_open.Id, _open.TenantId, txtTitle.Text, SelectedStatus(), _open.Token);
                SaveWorkOrderResult result = await _services.WorkOrders.SaveAsync(CurrentContext, command);
                await ShowSaveResultAsync(result);
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

        /// <summary>Failure setup: a second session opens the same record and saves it, moving the version on.</summary>
        private async void btnOtherSession_Click(object sender, EventArgs e)
        {
            int id = _open?.Id ?? SelectedWorkOrderId();
            if (id == 0)
            {
                AlertBox.Show(UiText.OpenBeforeConflict, MessageBoxIcon.Information,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return;
            }

            // Another user in the same tenant: two dispatchers, one work order — the walkthrough's failure path.
            string otherUser = StringComparer.Ordinal.Equals(_session.TenantId, "contoso") ? "ben.tech" : "cara.admin";

            BeginBusy("Another session is saving…", "demo.other-session");
            try
            {
                OtherSessionSaveResult result = await _services.OtherSession.SaveAsync(
                    otherUser, _session.TenantId, id, model => model.Title, WorkOrderStatus.Completed);
                await ShowOtherSessionAsync(result);
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

        /// <summary>Failure path: a record owned by another customer. The guard rejects it before it is read.</summary>
        private async void btnCrossTenant_Click(object sender, EventArgs e)
        {
            int foreignId = StringComparer.Ordinal.Equals(_session.TenantId, "fabrikam") ? 4001 : 3001;

            BeginBusy($"Reading #{foreignId}…", "demo.cross-tenant");
            try
            {
                _trace.Ui($"btnCrossTenant_Click → OpenAsync(#{foreignId}) — a record this session's tenant does not own");
                WorkOrderEditModel model = await _services.WorkOrders.OpenAsync(CurrentContext, foreignId);
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
        /// Failure path: the browser sends a tenant id the dropdown never offered. The answer to the review
        /// question — the tenant comes from verified claims, so the session refuses and the context is untouched.
        /// </summary>
        private async void btnSpoofTenant_Click(object sender, EventArgs e)
        {
            const string Spoofed = "northwind";

            BeginBusy("Tenant switch requested…", "session.switch-tenant");
            try
            {
                _trace.Ui($"btnSpoofTenant_Click → a client value of '{Spoofed}' arrives, although cboTenant only offers " +
                          $"{string.Join(", ", _session.EntitledTenants)}");
                TenantSwitchResult result = _session.SwitchTenant(Spoofed);
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

        /// <summary>Failure path: the static that holds "the current user", overwritten by another sign-in.</summary>
        private void btnStaticLeak_Click(object sender, EventArgs e)
        {
            try
            {
                _trace.Ui("btnStaticLeak_Click → StateAuditService.DemonstrateLeak(CurrentContext, session)");
                StaticLeakResult result = _services.StateAudit.DemonstrateLeak(CurrentContext, _session);
                ShowLeakResult(result);
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
        }

        /// <summary>The deliverable, run live: every static field in the assembly, classified.</summary>
        private void btnAudit_Click(object sender, EventArgs e)
        {
            try
            {
                _trace.Ui("btnAudit_Click → StateAuditService.RunAudit(CurrentContext)");
                StaticStateReport report = _services.StateAudit.RunAudit(CurrentContext);
                ShowAuditReport(report);
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
        }

        /// <summary>The recovery: re-open the record at its current version — the conflict dialog's Reload path.</summary>
        private async void btnReloadLatest_Click(object sender, EventArgs e)
        {
            if (_open == null)
            {
                AlertBox.Show(UiText.NothingToReload, MessageBoxIcon.Information,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return;
            }

            BeginBusy($"Reloading #{_open.Id}…", "work-order.reload");
            try
            {
                await ReloadLatestAsync();
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

        /// <summary>Loads the work queue for the tenant on the context. Called on load and after every change.</summary>
        private async void LoadQueue()
        {
            BeginBusy("Loading the work queue…", "work-queue.load");
            try
            {
                await ReloadQueueAsync();
                SetStatus($"{_rows.Count} work orders for {_session.TenantId}", StatusKind.Ok);
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
            lblStatusBar.Text = $"{CurrentContext.CommandName} · tenant {_session.TenantId} · {page.Total} rows · corr {CurrentContext.CorrelationId}";
            _trace.UiResult($"grid bound to {_rows.Count} rows — every row belongs to '{_session.TenantId}'");
        }

        /// <summary>The tenant switch outcome. Rejected → amber banner and nothing changes, not even the dropdown.</summary>
        private async Task ApplyTenantSwitchAsync(TenantSwitchResult result)
        {
            if (!result.Succeeded)
            {
                _trace.Security($"SessionContext.SwitchTenant REJECTED — {result.Reason}");
                ShowBanner($"Tenant not switched — {result.Reason}. The session still owns '{result.TenantId}', " +
                           "and every command context built from it still carries that tenant.", BannerKind.Warning);
                SetStatus("tenant switch rejected", StatusKind.Warn);
                SelectCurrentTenant();
                await ReloadQueueAsync();
                return;
            }

            if (!result.Changed)
            {
                _trace.Session($"SwitchTenant('{result.TenantId}') → unchanged");
                return;
            }

            _trace.Session($"SwitchTenant: '{result.PreviousTenantId}' → '{result.TenantId}' (entitled) — the open editor is dropped, " +
                           "because its record belongs to the previous tenant");
            ClearEditor();
            HideBanner();
            await ReloadQueueAsync();
            SetStatus($"tenant {result.TenantId} · {_rows.Count} work orders", StatusKind.Ok);
        }

        /// <summary>A work order was opened: the editor now owns that record and its version token.</summary>
        private void ShowOpened(WorkOrderEditModel model)
        {
            if (model == null)
            {
                ShowBanner("That work order no longer exists.", BannerKind.Warning);
                SetStatus("not found", StatusKind.Warn);
                return;
            }

            _open = model;
            _binding = true;
            txtTitle.Text = model.Title;
            cboStatus.SelectedItem = model.Status.ToString();
            _binding = false;

            txtVersion.Text = model.Token.ToDisplayText();
            lblEditCaption.Text = $"Editing #{model.Id} — {model.Customer} · {model.Site} · loaded at {model.Token.ToDisplayText()} " +
                                  $"(last saved by {model.ModifiedBy})";
            HideBanner();
            SetStatus($"#{model.Id} open at {model.Token.ToDisplayText()}", StatusKind.Ok);
            lblStatusBar.Text = $"work-order.open · #{model.Id} · {model.Token.ToDisplayText()} · corr {CurrentContext.CorrelationId}";
            _trace.UiResult($"editor bound to #{model.Id} — this tab holds {model.Token.ToDisplayText()}; the session holds no work order at all");
        }

        /// <summary>Saved, rejected, or stale — the three answers a save can give.</summary>
        private async Task ShowSaveResultAsync(SaveWorkOrderResult result)
        {
            lblCorrelation.Text = "corr " + result.CorrelationId;

            if (result.Succeeded)
            {
                _open = result.Saved;
                txtVersion.Text = result.NewVersion.ToDisplayText();
                lblEditCaption.Text = $"Editing #{_open.Id} — saved · this tab now holds {result.NewVersion.ToDisplayText()}";
                ShowBanner($"Saved — the record moved to {result.NewVersion.ToDisplayText()}. Another session editing the old version " +
                           "will now be told, not overwritten.", BannerKind.Success);
                SetStatus($"saved · {result.NewVersion.ToDisplayText()}", StatusKind.Ok);
                await ReloadQueueAsync();
                return;
            }

            if (result.IsConflict)
            {
                await ResolveConflictAsync(result.Conflict);
                return;
            }

            ShowBanner(string.Join(" ", result.Errors), BannerKind.Warning);
            SetStatus("rejected — nothing written", StatusKind.Warn);
            _trace.UiResult("ShowSaveResultAsync: validation rejected → banner; the record is untouched");
        }

        /// <summary>
        /// The stale save. The screen explains nothing itself: it hands the ConflictInfo to the dialog and
        /// then does what the user chose. Reload is the only path that changes anything on screen.
        /// </summary>
        private async Task ResolveConflictAsync(ConflictInfo conflict)
        {
            ShowBanner($"This work order changed while you were editing — {conflict.Footnote}. Nothing was overwritten.",
                BannerKind.Warning);
            SetStatus("stale version — resolve the conflict", StatusKind.Warn);

            var dialog = new ConflictDialog(conflict, CurrentContext, _services.Conflicts, _trace);
            DialogResult answer = await dialog.ShowDialogAsync();

            _trace.UiResult($"ConflictDialog → {dialog.Resolution}" +
                            (dialog.ComparisonViewed ? " (after comparing field by field)" : "") + $" · DialogResult={answer}");

            if (dialog.Resolution == ConflictResolution.Reload)
            {
                await ReloadLatestAsync();
                return;
            }

            ShowBanner($"Conflict left open — your edit is still on screen at {conflict.Expected.ToDisplayText()}, " +
                       $"the record is at {conflict.Found.ToDisplayText()}. Nothing was saved and nothing was lost.",
                BannerKind.Warning);
            SetStatus("conflict cancelled — your edit kept", StatusKind.Warn);
        }

        /// <summary>The recovery both the dialog and the bottom bar run: re-open at the current version.</summary>
        private async Task ReloadLatestAsync()
        {
            WorkOrderEditModel latest = await _services.WorkOrders.OpenAsync(CurrentContext, _open.Id);
            ShowOpened(latest);

            ShowBanner($"Reloaded {latest.Token.ToDisplayText()} — re-apply your edit on the latest version. " +
                       "Both sessions now agree, and the correlation ids tie every step to the audit log.", BannerKind.Success);
            SetStatus($"reloaded · {latest.Token.ToDisplayText()}", StatusKind.Ok);
            await ReloadQueueAsync();
        }

        /// <summary>What the other session did — and what it did to this tab's token.</summary>
        private async Task ShowOtherSessionAsync(OtherSessionSaveResult result)
        {
            if (!result.Succeeded)
            {
                ShowBanner(result.Message, BannerKind.Warning);
                SetStatus("the other session could not save", StatusKind.Warn);
                return;
            }

            await ReloadQueueAsync();

            string mine = _open == null ? "—" : _open.Token.ToDisplayText();
            ShowBanner($"{result.Message}. This tab still holds {mine} — the next Save will be rejected, not merged.",
                BannerKind.Warning);
            SetStatus($"another session moved the record to {result.NewVersion.ToDisplayText()}", StatusKind.Warn);
            _trace.UiResult($"two live SessionContext objects, one store: this tab {mine}, the record {result.NewVersion.ToDisplayText()}");
        }

        /// <summary>A cross-tenant read: rejected by the guard, logged with the correlation id, explained plainly.</summary>
        private void ShowCrossTenantRejection(CrossTenantAccessException ex)
        {
            _log.Error(ex, ex.CorrelationId);
            ShowBanner($"Cross-tenant access denied — this session owns '{ex.SessionTenantId}' and the record belongs to " +
                       $"'{ex.RequestedTenantId}'. Rejected in the service, before the record was read (ref {ex.CorrelationId}).",
                BannerKind.Error);
            SetStatus("blocked by TenantGuard", StatusKind.Error);
            lblStatusBar.Text = $"cross-tenant read denied · corr {ex.CorrelationId}";
            AlertBox.Show(UiText.NotYourTenant, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        private void ShowLeakResult(StaticLeakResult result)
        {
            ShowBanner(result.Headline + " — nothing in this sample authorizes on that static; in the code it is modelled on, " +
                       "something did.", result.Leaked ? BannerKind.Error : BannerKind.Warning);
            SetStatus(result.Leaked ? "static-state leak reproduced" : "no divergence this run", StatusKind.Error);
            lblStatusBar.Text = $"state.leak-demo · static says {result.StaticUserId}@{result.StaticTenantId} · " +
                                $"session says {result.SessionUserId}@{result.SessionTenantId} · corr {result.CorrelationId}";
        }

        private void ShowAuditReport(StaticStateReport report)
        {
            ShowBanner(report.Headline, report.Findings.Count == 0 ? BannerKind.Success : BannerKind.Error);
            SetStatus($"{report.Findings.Count} findings in {report.Entries.Count} statics",
                report.Findings.Count == 0 ? StatusKind.Ok : StatusKind.Error);
            lblStatusBar.Text = $"state.audit · {report.Assembly} · {report.Entries.Count} statics · " +
                                $"{report.Findings.Count} findings · corr {report.CorrelationId}";
        }

        /// <summary>Unexpected failure: log it with the correlation id, say something generic, keep the screen usable.</summary>
        private void ReportFailure(Exception ex)
        {
            _log.Error(ex, CurrentContext.CorrelationId);
            ShowBanner($"{UiText.ActionFailed}  (ref {CurrentContext.CorrelationId})", BannerKind.Error);
            SetStatus("failed — see the log", StatusKind.Error);
            AlertBox.Show(UiText.ActionFailed, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion

        #region Helpers (small, reusable)

        private enum StatusKind { Ok, Warn, Error }
        private enum BannerKind { Success, Warning, Error }

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
                _trace.Session($"Application.Session.Context = SessionContext ({_session}) — one object per browser session, " +
                               "created from verified claims, never static");
            }
            catch (Exception ex)
            {
                _trace.Session("Application.Session is not available outside a live session: " + ex.GetType().Name);
            }
        }

        private CommandContext NewCommand(string commandName)
        {
            _current = _session.BeginCommand(commandName);
            lblCorrelation.Text = "corr " + _current.CorrelationId;
            _trace.Ui($"{commandName} → new CommandContext: {_current}");
            return _current;
        }

        /// <summary>A new command starts: fresh correlation id, buttons off, amber "busy" status. Sent before the first await.</summary>
        private void BeginBusy(string text, string commandName)
        {
            NewCommand(commandName);
            SetButtons(false);
            SetStatus(text, StatusKind.Warn);
            lblStatusBar.Text = $"{commandName} · tenant {_session.TenantId} · corr {_current.CorrelationId}";
        }

        /// <summary>After the awaits: buttons back on, push the pending changes (the original request is long gone).</summary>
        private void EndBusy()
        {
            SetButtons(true);
            Application.Update(this);
        }

        private void SetButtons(bool enabled)
        {
            btnOpen.Enabled = enabled;
            btnSave.Enabled = enabled;
            btnOtherSession.Enabled = enabled;
            btnCrossTenant.Enabled = enabled;
            btnSpoofTenant.Enabled = enabled;
            btnStaticLeak.Enabled = enabled;
            btnAudit.Enabled = enabled;
            btnReloadLatest.Enabled = enabled;
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

            _trace.Session($"cboTenant offers {cboTenant.Items.Count} of {TenantDirectory.Tenants.Count} tenants — " +
                           "the entitlement is a claim, the dropdown is only a convenience");
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
            lblEditCaption.Text = "Editing — nothing open";
        }

        private void ShowSignedIn()
        {
            lblUser.Text = $"Signed in: {_session.UserId} · {string.Join("/", _session.Roles)}";
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
