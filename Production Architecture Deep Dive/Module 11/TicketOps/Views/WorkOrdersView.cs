using System;
using System.Collections.Generic;
using TicketOps.Controls;
using TicketOps.Data;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using TicketOps.Security;
using TicketOps.Services;
using Wisej.Web;

namespace TicketOps.Views
{
    /// <summary>
    /// TicketOps Console · Work Orders — the screen behind the login gate.
    ///
    /// Left card:   the work-order grid, the note editor with its two safe renderings, the action buttons
    ///              (Close / Delete hidden or shown by IPermissionService — a COURTESY, not a control) and the
    ///              in-app audit trail.
    /// Right card:  the activity trace: UI → SVC → PermissionService → DATA, plus [SESSION] and [AUDIT] lines.
    /// Bottom bar:  the progress path (20 audited notes), the bypass proof ("call DeleteAsync directly" and
    ///              "force-enable Delete" — the service refuses either way), the error path with recovery
    ///              (data outage) and Clear trace.
    ///
    /// Nothing in this file decides whether an action is allowed. The screen asks IPermissionService what to
    /// SHOW; TicketService asks it again what to ALLOW. A handler that receives UnauthorizedAccessException
    /// shows Strings.AccessDenied — never ex.Message, never a stack trace.
    /// </summary>
    public partial class WorkOrdersView : Form
    {
        private readonly ITicketService _tickets;
        private readonly IPermissionService _permissions;
        private readonly IAuditService _audit;
        private readonly IUserSession _session;
        private readonly IAuthenticationService _auth;
        private readonly InMemoryTicketRepository _repository;   // only for the lab's outage switch
        private readonly ILog _log;
        private readonly Func<Form> _openLogin;

        private IReadOnlyList<Ticket> _rows = new List<Ticket>();
        private int _bulkRemaining;

        // The Designer keeps the parameterless constructor; real wiring goes through the other one.
        public WorkOrdersView() : this(null, null, null, null, null, null, new ActivityLog(), null)
        {
        }

        public WorkOrdersView(ITicketService tickets, IPermissionService permissions, IAuditService audit, IUserSession session,
                              IAuthenticationService auth, InMemoryTicketRepository repository, ILog log, Func<Form> openLogin)
        {
            InitializeComponent();

            _tickets = tickets;
            _permissions = permissions;
            _audit = audit;
            _session = session;
            _auth = auth;
            _repository = repository;
            _log = log;
            _openLogin = openLogin;

            if (log is ActivityLog activityLog)
                this.tracePanel.Attach(activityLog);

            if (_audit != null)
            {
                foreach (var entry in _audit.Entries)
                    this.listAudit.Items.Add(AuditLog.Format(entry));
                _audit.EntryAdded += Audit_EntryAdded;
            }
        }

        #region Screen lifecycle

        private async void WorkOrdersView_Load(object sender, EventArgs e)
        {
            try
            {
                var user = _session?.User;
                _log.Info(LogLayer.UI, "WorkOrdersView.Load", $"screen shown for {(user == null ? "(nobody)" : user.ToString())} → IPermissionService.PermissionsOf decides what to SHOW");
                ShowSignedInUser();
                ApplyPermissionsToControls();
                await RefreshGridAsync();
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrdersView.Load", ex);
            }
        }

        private void WorkOrdersView_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.tracePanel.Attach(null);
            if (_audit != null)
                _audit.EntryAdded -= Audit_EntryAdded;
        }

        #endregion

        #region data → UI

        private void ShowSignedInUser()
        {
            var user = _session?.User;
            this.labelSignedIn.Text = user == null
                ? "Signed in: (nobody) — session identity missing"
                : $"Signed in: {user.DisplayName} · {string.Join(", ", user.Roles)} · session {WisejSessionBinding.ShortSessionId()}";
        }

        /// <summary>
        /// The UI courtesy: hide what the user cannot do. This is display logic, not a security control —
        /// the same IPermissionService answers again inside TicketService before anything executes.
        /// </summary>
        private void ApplyPermissionsToControls()
        {
            var user = _session?.User;
            bool canClose = _permissions.Can(user, Permission.CloseTicket);
            bool canDelete = _permissions.Can(user, Permission.DeleteTicket);
            bool canNote = _permissions.Can(user, Permission.AddNote);
            bool canAudit = _permissions.Can(user, Permission.ViewAuditTrail);

            this.buttonCloseTicket.Enabled = canClose;
            this.buttonDelete.Visible = canDelete;                   // hidden for a Technician — a UX hint only
            this.buttonRenderNote.Enabled = canNote;
            this.labelDeleteHint.Text = canDelete
                ? "Delete is shown because the role grants DeleteTicket — the service checks it again anyway."
                : "Delete is hidden for this role — a UX hint. Use the bottom bar to prove the service still refuses.";
            this.labelAuditCaption.Text = canAudit
                ? "AUDIT TRAIL · who / what / target / when (server, append-only) · this role may review it"
                : "AUDIT TRAIL · who / what / target / when (server, append-only) · shown in the lab; production requires ViewAuditTrail";

            _log.Info(LogLayer.UI, "WorkOrdersView.ApplyPermissions",
                $"buttonDelete.Visible = {canDelete} · buttonCloseTicket.Enabled = {canClose} — display only, TicketService re-checks");
        }

        private async System.Threading.Tasks.Task RefreshGridAsync()
        {
            try
            {
                this.statusBanner.SetStatus("loading", StatusKind.Busy);
                _rows = await _tickets.GetTicketsAsync();

                this.gridTickets.Rows.Clear();
                foreach (var t in _rows)
                    this.gridTickets.Rows.Add(t.Id, t.Title, t.Status.ToString(), string.IsNullOrEmpty(t.Note) ? "" : $"{t.Note.Length} chars", t.ClosedBy ?? "");

                this.labelCount.Text = $"{_rows.Count} work orders";
                this.statusBanner.SetStatus("ready", StatusKind.Success);
                _log.Info(LogLayer.UI, "WorkOrdersView.RefreshGrid", $"{_rows.Count} rows shown");
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrdersView.RefreshGrid", ex);
            }
        }

        private int? SelectedTicketId()
        {
            var row = this.gridTickets.CurrentRow;
            if (row == null || row.Index < 0 || row.Index >= _rows.Count)
                return null;
            return _rows[row.Index].Id;
        }

        private void ShowResult<T>(OperationResult<T> result)
        {
            if (result.Succeeded)
            {
                this.statusBanner.HideBanner();
                this.statusBanner.SetStatus(result.Message, StatusKind.Success);
                _log.Info(LogLayer.UI, "WorkOrdersView.ShowResult", $"OK · {result.Message}");
            }
            else
            {
                this.statusBanner.ShowBanner(result.Message, StatusKind.Warning);
                this.statusBanner.SetStatus("not done", StatusKind.Warning);
                _log.Warn(LogLayer.UI, "WorkOrdersView.ShowResult", $"FAIL · {result.Message}");
            }
        }

        /// <summary>
        /// The safe rendering of user text (Security/HtmlPolicy):
        /// labelNotePlain keeps AllowHtml = false, so the framework escapes the note and any markup shows as characters;
        /// labelNoteAllowList is the ONE reviewed AllowHtml = true surface — it receives HtmlPolicy.RenderWithAllowList(note),
        /// which encoded everything and re-enabled only &lt;b&gt;, &lt;i&gt; and &lt;br&gt;.
        /// </summary>
        private void RenderNote(Ticket ticket)
        {
            string note = ticket?.Note ?? string.Empty;

            this.labelNotePlain.AllowHtml = false;                      // the default — stated here so a reviewer sees it
            this.labelNotePlain.Text = note;

            this.labelNoteAllowList.Text = HtmlPolicy.RenderWithAllowList(note);   // AllowHtml = true is set in the Designer with a comment

            bool markup = HtmlPolicy.LooksLikeMarkup(note);
            _log.Info(LogLayer.UI, "WorkOrdersView.RenderNote",
                markup
                    ? "note contains markup → rendered as text (AllowHtml = false) and through the allow-list (<b>, <i>, <br> only) — nothing executes"
                    : "note rendered as text");
        }

        /// <summary>
        /// Unexpected failure OR an authorization denial. Both keep the details in the log; the user reads
        /// one safe sentence. A denial is a warning (expected in a hostile world), an outage is an error.
        /// </summary>
        private void ReportFailure(string source, Exception ex)
        {
            if (ex is UnauthorizedAccessException)
            {
                _log.Warn(LogLayer.UI, source, $"UnauthorizedAccessException from the service — denied where it executes; user sees Strings.AccessDenied");
                this.statusBanner.ShowBanner("⛔ " + Strings.AccessDenied, StatusKind.Error);
                this.statusBanner.SetStatus("access denied", StatusKind.Error);
                AlertBox.Show(Strings.AccessDenied, MessageBoxIcon.Stop,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return;
            }

            _log.Error(LogLayer.UI, source, ex, $"caught {ex.GetType().Name} — user sees the safe message");
            this.statusBanner.ShowBanner("✖ " + Strings.ActionFailed, StatusKind.Error);
            this.statusBanner.SetStatus("failed", StatusKind.Error);
            AlertBox.Show(Strings.ActionFailed, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        private void Audit_EntryAdded(object sender, AuditEntry entry)
        {
            if (this.IsDisposed)
                return;

            this.listAudit.Items.Add(AuditLog.Format(entry));
            this.listAudit.SelectedIndex = this.listAudit.Items.Count - 1;
        }

        #endregion

        #region Thin handlers

        private void gridTickets_SelectionChanged(object sender, EventArgs e)
        {
            int? id = SelectedTicketId();
            if (id == null)
            {
                this.labelSelected.Text = "No work order selected";
                return;
            }

            var t = _rows[this.gridTickets.CurrentRow.Index];
            this.labelSelected.Text = $"#{t.Id} · {t.Title} · {t.Status}";
            this.textNote.Text = t.Note ?? string.Empty;
            RenderNote(t);
        }

        /// <summary>Success path: store the note through the service, then render it safely.</summary>
        private async void buttonRenderNote_Click(object sender, EventArgs e)
        {
            try
            {
                int? id = SelectedTicketId();
                if (id == null)
                {
                    ShowResult(OperationResult<Ticket>.Fail("Select a work order first."));
                    return;
                }

                string note = this.textNote.Text;                                                  // UI → data (untrusted text)
                _log.Info(LogLayer.UI, "WorkOrdersView.buttonRenderNote_Click", $"→ ITicketService.AddNoteAsync(#{id}, {note.Length} chars)");
                var result = await _tickets.AddNoteAsync(id.Value, note);                          // authorization + validation live there
                ShowResult(result);
                if (result.Succeeded)
                {
                    RenderNote(result.Value);                                                     // data → UI, through HtmlPolicy
                    await RefreshGridAsync();
                }
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrdersView.buttonRenderNote_Click", ex);
            }
        }

        private async void buttonCloseTicket_Click(object sender, EventArgs e)
        {
            try
            {
                int? id = SelectedTicketId();
                if (id == null)
                {
                    ShowResult(OperationResult<Ticket>.Fail("Select a work order to close."));
                    return;
                }

                _log.Info(LogLayer.UI, "WorkOrdersView.buttonCloseTicket_Click", $"→ ITicketService.CloseAsync(#{id})");
                var result = await _tickets.CloseAsync(id.Value);                                 // Permission.CloseTicket is checked by the service (Role: Supervisor / Admin)
                ShowResult(result);
                if (result.Succeeded)
                    await RefreshGridAsync();
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrdersView.buttonCloseTicket_Click", ex);
            }
        }

        /// <summary>
        /// The Delete button. Hidden for a Technician by ApplyPermissionsToControls — but this handler does not
        /// check anything itself: if the button is force-enabled, the service still decides.
        /// </summary>
        private async void buttonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                int? id = SelectedTicketId();
                if (id == null)
                {
                    ShowResult(OperationResult<int>.Fail("Select a work order to delete."));
                    return;
                }

                _log.Info(LogLayer.UI, "WorkOrdersView.buttonDelete_Click", $"→ ITicketService.DeleteAsync(#{id}) — no permission check here; TicketService authorizes");
                var result = await _tickets.DeleteAsync(id.Value);                                // Permission.DeleteTicket is demanded by the service
                ShowResult(result);
                if (result.Succeeded)
                    await RefreshGridAsync();
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrdersView.buttonDelete_Click", ex);
            }
        }

        private void buttonSignOut_Click(object sender, EventArgs e)
        {
            try
            {
                _log.Info(LogLayer.UI, "WorkOrdersView.buttonSignOut_Click", "→ IAuthenticationService.SignOut → back to the login gate");
                _auth.SignOut();
                var login = _openLogin();
                login.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrdersView.buttonSignOut_Click", ex);
            }
        }

        #endregion

        #region Bottom bar: progress, bypass proofs, outage and recovery

        /// <summary>Progress path: a Timer adds five notes per tick through the same authorized, audited AddNoteAsync.</summary>
        private void buttonBulkNotes_Click(object sender, EventArgs e)
        {
            if (this.timerBulk.Enabled)
                return;

            _bulkRemaining = 20;
            this.progressBulk.Value = 0;
            this.progressBulk.Visible = true;
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus("adding 20 notes", StatusKind.Busy);
            _log.Info(LogLayer.UI, "WorkOrdersView.buttonBulkNotes_Click", "20 notes in batches of 5 per tick — each one authorized and audited by the service");
            this.timerBulk.Start();
        }

        private async void timerBulk_Tick(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < 5 && _bulkRemaining > 0; i++, _bulkRemaining--)
                {
                    int n = 20 - _bulkRemaining + 1;
                    int ticketId = _rows.Count == 0 ? 2001 : _rows[n % _rows.Count].Id;
                    var result = await _tickets.AddNoteAsync(ticketId, $"Shift note {n:00}: inspected, no change.");
                    if (!result.Succeeded)
                        throw new InvalidOperationException("Generated note was rejected: " + result.Message);
                }

                this.progressBulk.Value = 20 - _bulkRemaining;
                this.statusBanner.SetStatus($"adding {20 - _bulkRemaining}/20", StatusKind.Busy);

                if (_bulkRemaining == 0)
                {
                    this.timerBulk.Stop();
                    this.progressBulk.Visible = false;
                    _log.Info(LogLayer.UI, "WorkOrdersView.timerBulk_Tick", "bulk notes complete → refresh");
                    await RefreshGridAsync();
                }
            }
            catch (Exception ex)
            {
                this.timerBulk.Stop();
                this.progressBulk.Visible = false;
                ReportFailure("WorkOrdersView.timerBulk_Tick", ex);
            }
        }

        /// <summary>
        /// Failure path 1 (permission): call the service directly, exactly as a forged client event would reach it.
        /// The Delete button may be hidden — that must not matter. As a Technician the service refuses and audits;
        /// as a Supervisor it succeeds, which is the point: the decision is the ROLE's, not the button's.
        /// </summary>
        private async void buttonBypass_Click(object sender, EventArgs e)
        {
            try
            {
                const int targetTicket = 2002;
                _log.Info(LogLayer.UI, "WorkOrdersView.buttonBypass_Click",
                    $"calling ITicketService.DeleteAsync(#{targetTicket}) directly — buttonDelete.Visible = {this.buttonDelete.Visible}, irrelevant to the service");
                var result = await _tickets.DeleteAsync(targetTicket);                            // the Permission check (Role → DeleteTicket) runs inside the service
                ShowResult(result);
                if (result.Succeeded)
                    await RefreshGridAsync();
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrdersView.buttonBypass_Click", ex);
            }
        }

        /// <summary>
        /// Failure path 2 (permission): what "btnDelete.disabled = false" in the browser console achieves — the widget
        /// obeys, so we do the same on the server side of the widget to make the click possible. Then click Delete.
        /// </summary>
        private void buttonForceEnable_Click(object sender, EventArgs e)
        {
            this.buttonDelete.Visible = true;
            this.buttonDelete.Enabled = true;
            this.labelDeleteHint.Text = "Delete force-enabled (as DevTools would) — click it: the UI obeys, the server won't.";
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus("Delete force-enabled", StatusKind.Warning);
            _log.Warn(LogLayer.Client, "WorkOrdersView.buttonForceEnable_Click",
                "simulating a client that re-enables the hidden Delete button — the UI hint is gone, the service check is not");
        }

        /// <summary>Error path + recovery: toggle the repository outage, then refresh through the service.</summary>
        private async void buttonOutage_Click(object sender, EventArgs e)
        {
            if (_repository == null)
                return;

            _repository.SimulateOutage = !_repository.SimulateOutage;
            this.buttonOutage.Text = _repository.SimulateOutage ? "Recover the data store" : "Simulate data outage";
            _log.Info(LogLayer.UI, "WorkOrdersView.buttonOutage_Click",
                _repository.SimulateOutage ? "outage ON → refresh (expect ✖ in DATA, safe message in UI)" : "outage OFF → refresh (recovery)");
            await RefreshGridAsync();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.tracePanel.ClearTrace();
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus("ready", StatusKind.Normal);
        }

        #endregion
    }
}
