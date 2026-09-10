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
    /// EnterpriseOps — Work queue + audit. The screen the walkthrough builds.
    ///
    /// Top-left card:  the work queue and the three sensitive commands (Approve, Approve pending export,
    ///                 Export data), the status line and the denial banner.
    /// Bottom-left:    the lab deliverable — the audit log screen: every Demand, granted or denied, with the
    ///                 three filters (user · permission · result) and the dark status footer.
    /// Right, top:     the live activity trace, tagged by layer (UI → · Identity: · Security: · Service: · Data: · Audit:).
    /// Right, bottom:  the safe-HTML review: one untrusted customer note, rendered escaped, sanitized, or raw.
    /// Bottom bar:     switch identity · the three failure paths · the recoveries · the AllowHtml review · clear trace.
    ///
    /// The boundary this file respects: it owns UI state (labels, grids, banner, which buttons are visible) and
    /// nothing else. Which identity signed in, who may do what, what is written to the audit log and how
    /// untrusted text is encoded are all decided in services. The buttons below can be re-enabled by hand —
    /// there is a button that does exactly that — and the services still refuse.
    /// </summary>
    public partial class AuditLogPage : Page
    {
        private readonly ServiceRegistry _services;
        private readonly ActivityTrace _trace;

        private CommandContext _current;
        private List<WorkQueueRow> _queueRows = new List<WorkQueueRow>();
        private WorkOrderNote _note;
        private NoteRenderMode _noteMode = NoteRenderMode.Escaped;
        private bool _suppressFilterEvents;

        /// <summary>Designer / default constructor: its own session graph, so the file opens in the Designer.</summary>
        public AuditLogPage() : this(new ServiceRegistry(new ActivityTrace()))
        {
        }

        public AuditLogPage(ServiceRegistry services)
        {
            InitializeComponent();

            _services = services;
            _trace = services.Trace;
            _trace.EntryAdded += trace_EntryAdded;
        }

        /// <summary>The command running right now: one correlation id per click, minted from the session.</summary>
        private CommandContext CurrentContext => _current ?? NewCommand();

        #region Event handlers — thin, one service call each (the shape the lab code check expects)

        /// <summary>
        /// The gate runs before anything else. Not "the main page opens and then asks who you are" — nothing on
        /// this screen has data, and no service can be called, until a verified identity exists.
        /// </summary>
        private async void AuditLogPage_Load(object sender, EventArgs e)
        {
            _trace.Ui("AuditLogPage_Load → the authentication gate runs before any data is loaded");
            LockScreen("not signed in");
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
        /// The walkthrough's button. The handler does not ask whether the user may export — it calls the service,
        /// and the service demands the permission where the action executes.
        /// </summary>
        private async void btnExport_Click(object sender, EventArgs e)
        {
            BeginBusy("ExportAsync — PermissionService.Demand(ExportData)…");
            try
            {
                ExportResult result = await _services.Exports.RequestExportAsync(CurrentContext);
                await ShowExportResultAsync(result);
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
                ShowBanner("No export is waiting for approval. Request one with Export data first.", BannerKind.Warning);
                return;
            }

            BeginBusy($"approving export #{pending.Id}…");
            try
            {
                ExportResult result = await _services.Exports.ApproveExportAsync(CurrentContext, pending.Id);
                await ShowExportResultAsync(result);
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

        /// <summary>Failure path: reach for a record of another tenant. The tenant guard runs before the roles.</summary>
        private async void btnCrossTenant_Click(object sender, EventArgs e)
        {
            WorkOrder foreign = _services.WorkOrders.FirstOfOtherTenant(CurrentContext);
            if (foreign == null) return;

            _trace.Ui($"btnCrossTenant_Click → ApproveAsync(WO-{foreign.Id:0000}) — a '{foreign.TenantId}' record from a '{_services.Session.TenantId}' session");
            BeginBusy($"approving WO-{foreign.Id:0000} (tenant {foreign.TenantId})…");
            try
            {
                CommandResult result = await _services.WorkOrders.ApproveAsync(CurrentContext, foreign.Id);
                await ShowResultAsync(result, "approved");
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
        /// Failure path, first half: the UI bug. Buttons that <see cref="ApplyPermissionsToUi"/> had hidden are
        /// switched back on by hand — exactly what a merge mistake, a stale cache or a tampered client does.
        /// Nothing else changes: the next click goes to the same service.
        /// </summary>
        private void btnBreakUi_Click(object sender, EventArgs e)
        {
            btnExport.Visible = true;
            btnExport.Enabled = true;
            btnApprove.Visible = true;
            btnApprove.Enabled = true;

            _trace.Ui("btnBreakUi_Click → btnExport and btnApprove forced visible+enabled, bypassing ApplyPermissionsToUi");
            ShowBanner("The UI now offers Export data and Approve to a caller who may not hold those permissions. Click one.", BannerKind.Warning);
            SetStatus("UI deliberately broken — the services have not changed", StatusKind.Warn);
        }

        /// <summary>Failure path: the untrusted note rendered as raw HTML — the AllowHtml mistake, made on purpose.</summary>
        private async void btnUnsafeHtml_Click(object sender, EventArgs e)
        {
            _trace.Ui("btnUnsafeHtml_Click → NoteRenderService.Prepare(note, RawHtml)");
            await RenderNoteAsync(NoteRenderMode.RawHtml);
            Application.Update(this);
        }

        /// <summary>Recovery: back to the safe renderings — escaped, then the allow-list sanitizer, and back.</summary>
        private async void btnSafeHtml_Click(object sender, EventArgs e)
        {
            NoteRenderMode next = _noteMode == NoteRenderMode.Escaped ? NoteRenderMode.Sanitized : NoteRenderMode.Escaped;
            _trace.Ui($"btnSafeHtml_Click → NoteRenderService.Prepare(note, {next})");
            await RenderNoteAsync(next);
            Application.Update(this);
        }

        /// <summary>Runs the AllowHtml review over the live control tree and reports what it found.</summary>
        private void btnHtmlReview_Click(object sender, EventArgs e)
        {
            if (!Authenticated()) return;

            _trace.Ui("btnHtmlReview_Click → SecurityReviewService.Review(this) — reflection over the running screen");
            try
            {
                var sanitized = new HashSet<string>(StringComparer.Ordinal);
                if (_noteMode == NoteRenderMode.Sanitized) sanitized.Add("lblNote");

                HtmlReviewResult review = _services.Review.Review(CurrentContext, this, sanitized);
                ShowReview(review);
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
        }

        /// <summary>Sign out and reopen the gate. The audit log keeps every identity's entries.</summary>
        private async void btnSwitchIdentity_Click(object sender, EventArgs e)
        {
            _trace.Ui("btnSwitchIdentity_Click → SignInService.SignOut(), then the gate again");
            _services.SignIn.SignOut();
            _current = null;
            LockScreen("signed out");
            await RequireSignInAsync();
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

        private void btnClearTrace_Click(object sender, EventArgs e)
        {
            _trace.Clear();
        }

        #endregion

        #region The gate

        /// <summary>
        /// Opens the sign-in gate and, when it returns a verified identity, unlocks the screen. Every entry point
        /// of the application goes through a method like this — a second screen, a background job or a WebMethod
        /// would call the same services and be refused without a context.
        /// </summary>
        private async Task RequireSignInAsync()
        {
            var gate = new SignInGate(_services.SignIn);
            try
            {
                DialogResult result = await gate.ShowDialogAsync();
                if (result != DialogResult.OK || !_services.Session.IsAuthenticated)
                {
                    LockScreen("signed out — no verified identity in this session");
                    _trace.Identity("The gate closed without an identity; every service call would throw. The screen stays locked.");
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

        /// <summary>Everything a fresh identity implies, in order: header, permissions, data, note, audit.</summary>
        private async Task OnSignedInAsync(MappedIdentity identity)
        {
            _current = null;

            lblUser.Text = $"Signed in: {identity.DisplayName} ({identity.UserId}) · {identity.RoleList}";
            lblTenant.Text = $"tenant: {identity.TenantId}";
            lblWorkTitle.Text = $"Work queue — {identity.TenantId}";

            ApplyPermissionsToUi();

            if (identity.Roles.Count == 0)
            {
                ShowBanner($"Authenticated, but no group of this account maps to a role ({string.Join(", ", identity.UnmappedGroups)}). "
                         + "Every permission check will refuse — authentication is not authorization.", BannerKind.Warning);
                SetStatus("signed in with no role", StatusKind.Warn);
            }
            else
            {
                HideBanner();
                SetStatus($"signed in as {identity.UserId} · {identity.RoleList}", StatusKind.Ok);
            }

            await ReloadQueueAsync();
            await LoadNoteAsync();
            await RefreshAuditAsync();
        }

        /// <summary>
        /// Screens adapt, services enforce. Every line here calls <c>Has</c>, which answers without auditing,
        /// and every one of them is a **convenience**: hiding a button spares the user a refusal, it does not
        /// protect anything. "Break the UI: enable Export" undoes all of it in one click, and the services
        /// behave identically.
        /// </summary>
        private void ApplyPermissionsToUi()
        {
            CommandContext context = CurrentContext;

            bool mayApprove = _services.Permissions.Has(context, Permission.ApproveWorkOrders);
            bool mayExport = _services.Permissions.Has(context, Permission.ExportData);
            bool mayApproveExport = _services.Permissions.Has(context, Permission.ApproveExport);

            btnApprove.Visible = mayApprove;
            btnExport.Visible = mayExport;
            btnApproveExport.Visible = mayApproveExport;

            btnApprove.Enabled = btnExport.Enabled = btnApproveExport.Enabled = true;
            btnCrossTenant.Enabled = btnBreakUi.Enabled = btnHtmlReview.Enabled = true;
            btnUnsafeHtml.Enabled = btnSafeHtml.Enabled = true;

            _trace.Ui($"ApplyPermissionsToUi (Has, never Demand) → Approve={mayApprove} Export={mayExport} ApproveExport={mayApproveExport}"
                    + " — convenience only; the services check again");
        }

        private void LockScreen(string reason)
        {
            btnApprove.Visible = btnExport.Visible = btnApproveExport.Visible = false;
            btnCrossTenant.Enabled = btnBreakUi.Enabled = btnHtmlReview.Enabled = false;
            btnUnsafeHtml.Enabled = btnSafeHtml.Enabled = false;

            _queueRows = new List<WorkQueueRow>();
            dgvWorkOrders.DataSource = null;
            dgvAudit.DataSource = null;

            lblUser.Text = "not signed in";
            lblTenant.Text = "tenant: —";
            lblCorrelation.Text = "corr —";
            lblStatusBar.Text = $"{reason} — every service call would throw before it read a row";
            lblAuditScope.Text = "";
            lblAuditCount.Text = "";
            HideBanner();
            SetStatus(reason, StatusKind.Warn);
        }

        #endregion

        #region Loading and showing — UI state only, no decisions

        private async Task ReloadQueueAsync()
        {
            WorkQueueResult result = await _services.WorkOrders.LoadQueueAsync(CurrentContext);

            if (!result.Succeeded)
            {
                _queueRows = new List<WorkQueueRow>();
                dgvWorkOrders.DataSource = null;
                ShowBanner(FriendlyDenial(result.FirstError), BannerKind.Error);
                SetStatus("the queue itself is not readable by this caller", StatusKind.Error);
                _trace.UiResult("ShowQueue: refused — the grid stays empty, no row was read");
                return;
            }

            _queueRows = result.Rows.ToList();
            dgvWorkOrders.DataSource = _queueRows;
            _trace.UiResult($"ShowQueue: {_queueRows.Count} rows bound (projection, never the entity)");
        }

        private async Task LoadNoteAsync()
        {
            var notes = _services.NoteStore.ForWorkOrder(_services.Session.TenantId, 1002);
            _note = notes.LastOrDefault();
            await RenderNoteAsync(NoteRenderMode.Escaped);
        }

        /// <summary>
        /// The whole of the screen's involvement in safe HTML: ask the service, then set two properties.
        /// The decision — escape, sanitize or (deliberately) neither — was taken in <see cref="NoteRenderService"/>.
        /// </summary>
        private async Task RenderNoteAsync(NoteRenderMode mode)
        {
            if (_note == null || !_services.Session.IsAuthenticated) return;

            NoteRenderResult result = _services.Notes.Prepare(CurrentContext, _note, mode);
            _noteMode = result.Mode;

            lblNote.AllowHtml = result.AllowHtml;
            lblNote.Text = result.Html;

            lblNoteSource.Text = $"note #{_note.Id} · from {_note.Source}";
            lblNoteMode.Text = result.Mode == NoteRenderMode.Escaped ? "AllowHtml = false" : $"AllowHtml = true · {result.Mode}";
            lblNoteMode.ForeColor = result.Mode == NoteRenderMode.RawHtml
                ? System.Drawing.Color.FromArgb(224, 86, 59)
                : System.Drawing.Color.FromArgb(31, 157, 87);

            lblNoteWarning.Text = result.Warning ?? $"Stored raw, encoded at render time. Payload seen: {result.Markup}";
            lblNoteWarning.ForeColor = result.Mode == NoteRenderMode.RawHtml
                ? System.Drawing.Color.FromArgb(178, 59, 39)
                : System.Drawing.Color.FromArgb(90, 107, 125);

            if (result.Mode == NoteRenderMode.RawHtml)
            {
                ShowBanner("The note is live markup now: this is the finding an AllowHtml review exists to catch. It is in the audit log as UnsafeHtmlRender.", BannerKind.Error);
                SetStatus("raw HTML rendered — finding", StatusKind.Error);
                await RefreshAuditAsync();
            }
            else
            {
                SetStatus($"note rendered · {result.Mode}", StatusKind.Ok);
            }

            _trace.UiResult($"RenderNote: lblNote.AllowHtml = {result.AllowHtml}; Text set from NoteRenderService ({result.Mode})");
        }

        private async Task RefreshAuditAsync()
        {
            if (!_services.Session.IsAuthenticated) return;

            AuditQueryResult result = await _services.AuditQuery.QueryAsync(CurrentContext, ReadFilter());

            RebuildFilterCombos();

            dgvAudit.DataSource = result.Rows.ToList();
            PaintDenials(result);

            lblAuditScope.Text = "scope: " + result.ScopeLabel;
            lblAuditCount.Text = result.Summary;
            _trace.UiResult($"ShowAudit: {result.Rows.Count} rows bound · {result.ScopeLabel}");
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

        /// <summary>A command result → banner, status and footer. Denied and failed look different on purpose.</summary>
        private async Task ShowResultAsync(CommandResult result, string successText)
        {
            lblCorrelation.Text = "corr " + result.CorrelationId;

            if (result.Denied)
            {
                ShowBanner(FriendlyDenial(result.FirstError), BannerKind.Error);
                SetStatus("denied by the service", StatusKind.Error);
                lblStatusBar.Text = $"UnauthorizedAccessException — {result.FirstError} · audited · nothing changed";
                _trace.UiResult("ShowResult: Denied=true → banner, and the denial is a row in the audit log");
            }
            else if (!result.Succeeded)
            {
                ShowBanner(result.FirstError + $"  (ref {result.CorrelationId})", BannerKind.Warning);
                SetStatus("not completed", StatusKind.Warn);
                lblStatusBar.Text = $"{result.Summary} — ref {result.CorrelationId}";
            }
            else
            {
                ShowBanner(successText, BannerKind.Success);
                SetStatus(successText.ToLowerInvariant(), StatusKind.Ok);
                lblStatusBar.Text = $"{successText} — EnterpriseOps.Services · corr {result.CorrelationId}";
            }

            await RefreshAuditAsync();
        }

        private async Task ShowExportResultAsync(ExportResult result)
        {
            lblCorrelation.Text = "corr " + result.CorrelationId;

            if (result.Denied)
            {
                ShowBanner("You don't have permission to export data.", BannerKind.Error);
                SetStatus("denied by the service", StatusKind.Error);
                lblStatusBar.Text = $"UnauthorizedAccessException — {result.FirstError} · thrown by PermissionService.Demand · nothing left the server";
                _trace.UiResult("ShowExportResult: the button was clickable, the service was not fooled; the denial is audited");
            }
            else if (result.Pending != null)
            {
                ShowBanner(result.FirstError, BannerKind.Warning);
                SetStatus($"export #{result.Pending.Id} awaiting a second approver", StatusKind.Warn);
                lblStatusBar.Text = $"export #{result.Pending.Id} · {result.Pending.RowCount} rows · requested by {result.Pending.RequestedBy} · nothing exported yet";
            }
            else if (!result.Succeeded)
            {
                ShowBanner(result.FirstError + $"  (ref {result.CorrelationId})", BannerKind.Warning);
                SetStatus("export not completed", StatusKind.Warn);
                lblStatusBar.Text = $"{result.FirstError} · ref {result.CorrelationId}";
            }
            else
            {
                ShowBanner($"{result.RowsExported} rows exported.", BannerKind.Success);
                SetStatus($"{result.RowsExported} rows exported", StatusKind.Ok);
                lblStatusBar.Text = $"ExportData ok — {result.RowsExported} rows · corr {result.CorrelationId}";
            }

            await RefreshAuditAsync();
        }

        private void ShowReview(HtmlReviewResult review)
        {
            lblCorrelation.Text = "corr " + review.CorrelationId;
            lblStatusBar.Text = review.Summary + " · " + _services.Review.ChecklistSummary(CurrentContext);

            if (review.Findings.Count == 0)
            {
                ShowBanner($"AllowHtml review: {review.HtmlEnabled.Count} of {review.Surfaces.Count} HTML-capable surfaces have AllowHtml = true, and none of them carries unsanitized untrusted text.", BannerKind.Success);
                SetStatus("AllowHtml review clean", StatusKind.Ok);
                return;
            }

            HtmlSurface first = review.Findings[0];
            ShowBanner($"AllowHtml review — {review.Findings.Count} finding(s). {first.Name} ({first.Type}): {first.TextSource}, protected by {first.Handling}.", BannerKind.Error);
            SetStatus($"AllowHtml review: {review.Findings.Count} finding(s)", StatusKind.Error);
        }

        /// <summary>Unexpected failure: generic message plus the correlation id; the details stay in the trace.</summary>
        private void ReportFailure(Exception ex)
        {
            _trace.Service($"unhandled {ex.GetType().Name} — corr {CurrentContext.CorrelationId}");
            ShowBanner($"The action could not be completed. Check the log for details.  (ref {CurrentContext.CorrelationId})", BannerKind.Error);
            SetStatus("failed — see the log", StatusKind.Error);
            AlertBox.Show("The action could not be completed. Check the log for details.", MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion

        #region Helpers (small, reusable)

        private enum StatusKind { Ok, Warn, Error }
        private enum BannerKind { Success, Warning, Error }

        /// <summary>The selected queue row, read from the bound projection rather than from cell text.</summary>
        private WorkQueueRow SelectedRow
            => dgvWorkOrders.CurrentRow?.DataBoundItem as WorkQueueRow ?? _queueRows.FirstOrDefault();

        private CommandContext NewCommand()
        {
            _current = _services.Session.BeginCommand();
            lblCorrelation.Text = "corr " + _current.CorrelationId;
            return _current;
        }

        private bool Authenticated()
        {
            if (_services.Session.IsAuthenticated) return true;

            AlertBox.Show("Sign in first.", MessageBoxIcon.Warning,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 3000);
            return false;
        }

        /// <summary>A new command starts: fresh correlation id, buttons off, amber status. Sent before the first await.</summary>
        private void BeginBusy(string text)
        {
            _current = null;
            NewCommand();
            SetButtons(false);
            SetStatus(text, StatusKind.Warn);
            lblStatusBar.Text = text;
        }

        /// <summary>After the awaits: buttons back on, push the pending changes (the original request is long gone).</summary>
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
            btnCrossTenant.Enabled = enabled;
            btnBreakUi.Enabled = enabled;
            btnSwitchIdentity.Enabled = enabled;
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

        /// <summary>
        /// A denial may name the permission: it helps the user ask for it and the help desk grant it, and the
        /// person who tried already knows what they tried. What it must never do is leak the rule behind it.
        /// </summary>
        private static string FriendlyDenial(string serviceMessage)
            => serviceMessage.StartsWith("Missing permission", StringComparison.Ordinal)
                ? $"You don't have permission for that action. ({serviceMessage})"
                : serviceMessage;

        /// <summary>The trace sink: one line per layer decision; null clears. The only place that knows lstTrace.</summary>
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
