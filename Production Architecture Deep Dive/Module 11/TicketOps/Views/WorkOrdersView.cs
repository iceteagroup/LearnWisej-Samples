using System;
using System.Collections.Generic;
using TicketOps.Controls;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using TicketOps.Security;
using TicketOps.Services;
using Wisej.Web;

namespace TicketOps.Views
{
    /// <summary>
    /// TicketOps Console · Work Orders — the screen behind the login gate: the work-order grid, the note editor
    /// with its two safe renderings, Close / Delete (enabled by IPermissionService — a courtesy, not a control)
    /// and the audit trail.
    ///
    /// Nothing in this file decides whether an action is allowed. The screen asks IPermissionService what to
    /// ENABLE; TicketService asks it again what to ALLOW. A handler that receives UnauthorizedAccessException
    /// shows Strings.AccessDenied — never ex.Message, never a stack trace.
    /// </summary>
    public partial class WorkOrdersView : Form
    {
        private readonly ITicketService _tickets;
        private readonly IPermissionService _permissions;
        private readonly IAuditService _audit;
        private readonly IUserSession _session;
        private readonly IAuthenticationService _auth;
        private readonly ILog _log;
        private readonly Func<Form> _openLogin;

        private IReadOnlyList<Ticket> _rows = new List<Ticket>();

        // The Designer keeps the parameterless constructor; real wiring goes through the other one.
        public WorkOrdersView() : this(null, null, null, null, null, new ActivityLog(), null)
        {
        }

        public WorkOrdersView(ITicketService tickets, IPermissionService permissions, IAuditService audit, IUserSession session,
                              IAuthenticationService auth, ILog log, Func<Form> openLogin)
        {
            InitializeComponent();

            _tickets = tickets;
            _permissions = permissions;
            _audit = audit;
            _session = session;
            _auth = auth;
            _log = log;
            _openLogin = openLogin;

            if (_audit != null)
            {
                foreach (var entry in _audit.Entries)
                    this.listAudit.Items.Add(AuditLog.Format(entry));
                _audit.EntryAdded += Audit_EntryAdded;
            }
        }

        private async void WorkOrdersView_Load(object sender, EventArgs e)
        {
            try
            {
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
            if (_audit != null)
                _audit.EntryAdded -= Audit_EntryAdded;
        }

        #region data → UI

        private void ShowSignedInUser()
        {
            var user = _session?.User;
            this.labelSignedIn.Text = user == null
                ? "Signed in: (nobody)"
                : $"Signed in: {user.DisplayName} · {string.Join(", ", user.Roles)}";
        }

        /// <summary>
        /// The UI courtesy: disable what the user cannot do. This is display logic, not a security control —
        /// the same IPermissionService answers again inside TicketService before anything executes.
        /// </summary>
        private void ApplyPermissionsToControls()
        {
            var user = _session?.User;
            this.buttonCloseTicket.Enabled = _permissions.Can(user, Permission.CloseTicket);
            this.buttonDelete.Enabled = _permissions.Can(user, Permission.DeleteTicket);
            this.buttonRenderNote.Enabled = _permissions.Can(user, Permission.AddNote);
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
            }
            else
            {
                this.statusBanner.ShowBanner(result.Message, StatusKind.Warning);
                this.statusBanner.SetStatus("not done", StatusKind.Warning);
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
        }

        /// <summary>
        /// Unexpected failure OR an authorization denial. Both keep the details in the log; the user reads
        /// one safe sentence. The service already audited the denial.
        /// </summary>
        private void ReportFailure(string source, Exception ex)
        {
            if (ex is UnauthorizedAccessException)
            {
                this.statusBanner.ShowBanner("⛔ " + Strings.AccessDenied, StatusKind.Error);
                this.statusBanner.SetStatus("access denied", StatusKind.Error);
                AlertBox.Show(Strings.AccessDenied, MessageBoxIcon.Stop,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return;
            }

            _log.Error(LogLayer.UI, source, ex);
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

        #region Handlers

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

        /// <summary>Store the note through the service, then render it safely.</summary>
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

                var result = await _tickets.AddNoteAsync(id.Value, this.textNote.Text);          // authorization + validation live there
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

                var result = await _tickets.CloseAsync(id.Value);                                 // Permission.CloseTicket is checked by the service
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
        /// The Delete button. Disabled for a Technician by ApplyPermissionsToControls — but this handler does not
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

        /// <summary>
        /// The lab's demonstration: what "btnDelete.disabled = false" in the browser console achieves — the widget
        /// obeys. Click Delete afterwards: the UI lets the click through, TicketService still refuses and audits it.
        /// </summary>
        private void buttonForceEnable_Click(object sender, EventArgs e)
        {
            this.buttonDelete.Enabled = true;
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus("Delete force-enabled", StatusKind.Warning);
        }

        private void buttonSignOut_Click(object sender, EventArgs e)
        {
            try
            {
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
    }
}
