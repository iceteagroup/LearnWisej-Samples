using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupportDesk.Data.Diagnostics;
using SupportDesk.Services;
using Wisej.Services;
using Wisej.Web;

namespace SupportDesk.Web
{
    /// <summary>
    /// Module 4 lab: the Add/Edit Ticket modal. Opened by <c>TicketBrowserPage</c> with
    /// <c>await editor.ShowDialogAsync()</c> and closed with <see cref="DialogResult.OK"/> (something was
    /// written, or the row was found already gone) or <see cref="DialogResult.Cancel"/> (nothing was).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Two lifetimes, kept apart.</b> This <see cref="Form"/>, its <c>editBindingSource</c> and the
    /// current <see cref="TicketEditModel"/> live for as long as the dialog is open — session state, the
    /// same rule the browser page follows for its grid. Every <c>DbContext</c> the dialog ever touches is
    /// created by <see cref="TicketCommandService"/>, used for exactly one read or one write, and disposed
    /// before that service method returns; the form never sees one.
    /// </para>
    /// <para>
    /// <b>The DateTimePicker is bound by hand, not through <c>DataBindings.Add</c>.</b> The course cookbook
    /// flags binding a nullable <c>DateTime?</c> model property to <c>DateTimePicker.Value</c> (which is
    /// not nullable) as unverified, and names the safer alternative: copy <c>Checked</c>/<c>Value</c> by
    /// hand. <see cref="LoadEditorAsync"/> sets them from the model after the model is loaded;
    /// <see cref="SaveAsync"/> reads them back into the model right after <c>EndEdit</c>, before the model
    /// is mapped onto the entity. Every other control — <c>TextBox</c>, <c>ComboBox</c>, <c>CheckBox</c> —
    /// is bound the ordinary way.
    /// </para>
    /// <para>
    /// <b>The trace.</b> The dialog has no trace list of its own — it logs into the parent page's, through
    /// the <c>trace</c> callback passed to the constructor. <see cref="OnQueryTrace"/> translates the same
    /// <see cref="TraceEntry"/> stream <c>TicketBrowserPage</c> reads (context created/disposed, SQL sent,
    /// service notes) into that callback for <see cref="SaveAsync"/> and <see cref="DeleteAsync"/>, so a
    /// save or a delete shows the same level of detail as a search. <see cref="LoadEditorAsync"/> instead
    /// reports a single summary line — it is a background read, the same restraint
    /// <c>TicketBrowserPage.LoadLookupsAsync</c> uses for its own lookups.
    /// </para>
    /// </remarks>
    public partial class TicketEditorForm : Form
    {
        [Inject]
        private TicketCommandService Commands { get; set; }

        private readonly int? _ticketId;
        private readonly TimeSpan _saveLatency;
        private readonly Action<char, string, string> _trace;

        // The guard: Save and Delete share it, so a double click on either while the other is running is
        // dropped the same way TicketBrowserPage._loading drops a second search.
        private bool _saving;

        private string _ticketNumber;

        /// <param name="ticketId">Null for "Add ticket"; an existing ticket's key for "Edit ticket".</param>
        /// <param name="saveLatency">Zero in the ordinary path; armed from the page's "Slow save (2.5 s)" toggle so the guard and the busy Save button can be watched.</param>
        /// <param name="trace">Receives (symbol, label, text) for one line in the parent page's trace list — the same shape <c>TicketBrowserPage.AddTraceRaw</c> renders.</param>
        public TicketEditorForm(int? ticketId, TimeSpan saveLatency, Action<char, string, string> trace)
        {
            _ticketId = ticketId;
            _saveLatency = saveLatency;
            _trace = trace ?? ((symbol, label, text) => { });

            InitializeComponent();
        }

        private async void TicketEditorForm_Load(object sender, EventArgs e)
        {
            // Verified in the browser: an exception that escapes an async Load handler becomes a Wisej.NET
            // "Application Error" dialog. The ticket may have been deleted between the grid search and this
            // click (another session, or the Simulate button on the page), so the load has its own catch.
            try
            {
                await LoadEditorAsync();
            }
            catch (TicketNotFoundException)
            {
                _trace('•', "editor", $"LoadEditorAsync(#{_ticketId}): the ticket no longer exists — deleted by another session after the grid was loaded; closing with OK so the grid refreshes");
                AlertBox.Show("This ticket was already deleted by someone else. The list will refresh.", MessageBoxIcon.Warning, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                this.DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                _trace('•', "editor", $"LoadEditorAsync failed: {ex.GetType().Name}: {ex.Message} — closing with Cancel, nothing changed");
                AlertBox.Show("The ticket could not be loaded right now. Please try again in a moment.", MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                this.DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        #region Load — lookups, then the model; the model becomes current only once both are ready

        private async Task LoadEditorAsync()
        {
            if (Commands == null)
            {
                _trace('•', "editor", "TicketCommandService not injected — the IServiceProvider bridge is missing");
                return;
            }

            // A summary, not the full SQL detail — the same restraint TicketBrowserPage.LoadLookupsAsync
            // uses: this is a background read, not the action the operator asked for.
            using var scope = QueryTrace.Begin(_ => { });

            var lookups = await Commands.GetLookupsAsync();

            // The lookups' DataSource is set BEFORE the model becomes editBindingSource.DataSource —
            // otherwise a bound SelectedValue with nothing to select resolves to nothing.
            this.cboCustomer.DisplayMember = nameof(LookupItem.Name);
            this.cboCustomer.ValueMember = nameof(LookupItem.Id);
            this.cboCustomer.DataSource = lookups.Customers.ToList();

            var agentRows = new List<LookupItem> { new LookupItem(TicketCommandService.UnassignedAgentId, "— unassigned —") };
            agentRows.AddRange(lookups.Agents);
            this.cboAgent.DisplayMember = nameof(LookupItem.Name);
            this.cboAgent.ValueMember = nameof(LookupItem.Id);
            this.cboAgent.DataSource = agentRows;

            this.cboCategory.DisplayMember = nameof(LookupItem.Name);
            this.cboCategory.ValueMember = nameof(LookupItem.Id);
            this.cboCategory.DataSource = lookups.Categories.ToList();

            // Status and Priority are strings, but SelectedValue still needs a ValueMember (see NamedValue).
            this.cboStatus.DisplayMember = nameof(NamedValue.Name);
            this.cboStatus.ValueMember = nameof(NamedValue.Value);
            this.cboStatus.DataSource = lookups.Statuses.Select(s => new NamedValue(s, s)).ToList();
            this.cboPriority.DisplayMember = nameof(NamedValue.Name);
            this.cboPriority.ValueMember = nameof(NamedValue.Value);
            this.cboPriority.DataSource = lookups.Priorities.Select(p => new NamedValue(p, p)).ToList();

            TicketEditModel model;
            if (_ticketId is int existingId)
            {
                var data = await Commands.LoadEditModelAsync(existingId);
                model = data.Model;
                _ticketNumber = data.Number;
                this.Text = $"Edit ticket {_ticketNumber}";
                this.lblNumber.Text = _ticketNumber;
                this.btnDelete.Visible = true;
            }
            else
            {
                model = TicketEditModel.NewTicket();
                _ticketNumber = null;
                this.Text = "Add ticket";
                this.lblNumber.Text = "assigned on save";
                this.btnDelete.Visible = false;
            }

            this.editBindingSource.DataSource = model;
            BindLookupControls();

            // dtpDueDate is not bound through DataBindings.Add — see the class remarks. Copy by hand, now
            // that the model exists.
            this.dtpDueDate.Checked = model.DueDate.HasValue;
            this.dtpDueDate.Value = model.DueDate ?? DateTime.Today;

            _trace('•', "editor", $"LoadEditorAsync({(_ticketId is int i ? "#" + i : "new")}): {(_ticketId is null ? 0 : 1)} no-tracking read + lookups ({scope.Commands} statement(s), {scope.ContextsCreated} context created, {scope.ContextsDisposed} disposed)");
            Application.Update(this);
        }

        /// <summary>
        /// Maps the "— unassigned —" sentinel (<see cref="TicketCommandService.UnassignedAgentId"/>) onto a
        /// null <c>AgentId</c> and back. Everything else in this form binds a plain nullable value straight
        /// through — nothing selected already means null — this is the one ComboBox with a real "no value"
        /// row sitting in the list next to the real choices.
        /// </summary>
        private bool _lookupsBound;

        /// <summary>
        /// The lookup ComboBoxes are bound here, not in the Designer. Verified in the browser: a
        /// SelectedValue binding added before the ComboBox has its DataSource (the lookup list) and before
        /// the BindingSource has its current item pushes nothing into the control (Priority showed the first
        /// row instead of the model value) and never writes the selection back (CustomerId stayed null after
        /// EndEdit). Bind once, after both exist; the TextBox and CheckBox bindings can stay in the Designer.
        /// </summary>
        private void BindLookupControls()
        {
            if (_lookupsBound)
                return;
            _lookupsBound = true;

            this.cboCustomer.DataBindings.Add("SelectedValue", this.editBindingSource, nameof(TicketEditModel.CustomerId), true, DataSourceUpdateMode.OnPropertyChanged);

            var agentBinding = this.cboAgent.DataBindings.Add("SelectedValue", this.editBindingSource, nameof(TicketEditModel.AgentId), true, DataSourceUpdateMode.OnPropertyChanged);
            agentBinding.Format += new ConvertEventHandler(this.AgentBinding_Format);
            agentBinding.Parse += new ConvertEventHandler(this.AgentBinding_Parse);

            this.cboCategory.DataBindings.Add("SelectedValue", this.editBindingSource, nameof(TicketEditModel.CategoryId), true, DataSourceUpdateMode.OnPropertyChanged);
            this.cboStatus.DataBindings.Add("SelectedValue", this.editBindingSource, nameof(TicketEditModel.Status), true, DataSourceUpdateMode.OnPropertyChanged);
            this.cboPriority.DataBindings.Add("SelectedValue", this.editBindingSource, nameof(TicketEditModel.Priority), true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void AgentBinding_Format(object sender, ConvertEventArgs e)
        {
            e.Value = (e.Value as int?) ?? TicketCommandService.UnassignedAgentId;
        }

        private void AgentBinding_Parse(object sender, ConvertEventArgs e)
        {
            if (e.Value is int selected && selected == TicketCommandService.UnassignedAgentId)
                e.Value = null;
        }

        #endregion

        #region Save — guard, EndEdit, the validation placeholder, a fresh context, map, SaveChangesAsync

        private async void btnSave_Click(object sender, EventArgs e)
        {
            await SaveAsync();
        }

        private async Task SaveAsync()
        {
            if (_saving)
            {
                _trace('•', "editor", "guard: save already running — this click is ignored");
                return;
            }

            using var scope = QueryTrace.Begin(OnQueryTrace);
            try
            {
                _saving = true;
                this.btnSave.Enabled = false;
                this.btnDelete.Enabled = false;
                HideBanner();

                this.editBindingSource.EndEdit();
                var model = (TicketEditModel)this.editBindingSource.Current;

                // dtpDueDate is copied by hand — see the class remarks — right after EndEdit, so the model
                // reflects the picker no matter which control the operator touched last.
                model.DueDate = this.dtpDueDate.Checked ? this.dtpDueDate.Value.Date : (DateTime?)null;

                _trace('•', "editor", $"EndEdit → model {{Title='{Truncate(model.Title, 40)}', CustomerId={Fmt(model.CustomerId)}, AgentId={Fmt(model.AgentId)}, CategoryId={Fmt(model.CategoryId)}, Status={model.Status}, Priority={model.Priority}, DueDate={(model.DueDate?.ToString("yyyy-MM-dd") ?? "—")}, IsUrgent={model.IsUrgent}}}");

                // The Module 4 validation placeholder: Title required. Module 5 replaces this with a real
                // validator shared by the client and the server.
                this.errorProvider.Clear();
                if (string.IsNullOrWhiteSpace(model.Title))
                {
                    this.errorProvider.SetError(this.txtTitle, "Title is required.");
                    _trace('•', "editor", "validation placeholder: Title is required — Save stopped, no context created");
                    return;
                }

                var result = await Commands.SaveAsync(model, _saveLatency);
                _ticketNumber = result.Number;
                _trace('←', "result", $"saved {result.Number} · {scope.Commands} statement(s), {scope.ContextsCreated} context created, {scope.ContextsDisposed} disposed");

                this.DialogResult = DialogResult.OK;
                Close();
            }
            catch (TicketNotFoundException)
            {
                Fail("This ticket was already deleted by someone else. Nothing was saved.", null);
                // The row the editor opened on is gone; closing with OK tells the browser to refresh so the
                // stale row disappears from the grid instead of sitting there forever.
                this.DialogResult = DialogResult.OK;
                Close();
            }
            catch (DatabaseUnavailableException ex)
            {
                Fail("The Support Desk database is not reachable right now. Nothing was saved — please try again in a moment.", ex);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Fail("Someone else changed this ticket in the meantime. Nothing was saved — reload and try again.", ex);
            }
            catch (DbUpdateException ex)
            {
                Fail("The ticket could not be saved because the database rejected the change.", ex);
            }
            catch (Exception ex)
            {
                Fail("The ticket could not be saved. Please try again in a moment.", ex);
            }
            finally
            {
                _saving = false;
                this.btnSave.Enabled = true;
                this.btnDelete.Enabled = true;
                Application.Update(this);
            }
        }

        #endregion

        #region Delete — confirm, fresh load by key, a business rule, then Remove + SaveChangesAsync

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            await DeleteAsync();
        }

        private async Task DeleteAsync()
        {
            if (_saving)
            {
                _trace('•', "editor", "guard: an operation is already running — this click is ignored");
                return;
            }

            if (_ticketId is not int id)
                return; // btnDelete is hidden for a new ticket; nothing to delete yet.

            var confirm = await MessageBox.ShowAsync(
                $"Delete ticket {_ticketNumber}? This cannot be undone.",
                "Confirm delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
            {
                _trace('•', "editor", "delete: not confirmed — nothing sent to the database");
                return;
            }

            using var scope = QueryTrace.Begin(OnQueryTrace);
            try
            {
                _saving = true;
                this.btnSave.Enabled = false;
                this.btnDelete.Enabled = false;
                HideBanner();

                var result = await Commands.DeleteAsync(id);
                switch (result.Outcome)
                {
                    case DeleteOutcome.Deleted:
                        _trace('←', "result", $"deleted {result.Number} · {scope.Commands} statement(s), {scope.ContextsCreated} context created, {scope.ContextsDisposed} disposed");
                        this.DialogResult = DialogResult.OK;
                        Close();
                        break;

                    case DeleteOutcome.NotFound:
                        _trace('•', "editor", "delete: already gone — another session (or the page's Simulate button) deleted it first");
                        AlertBox.Show(result.Reason, MessageBoxIcon.Warning, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                        // Close with OK anyway: the grid is stale on this row either way, and a refresh fixes it.
                        this.DialogResult = DialogResult.OK;
                        Close();
                        break;

                    case DeleteOutcome.Refused:
                        _trace('•', "editor", $"delete refused: {result.Reason}");
                        ShowBanner(result.Reason);
                        break;
                }
            }
            catch (DatabaseUnavailableException ex)
            {
                Fail("The Support Desk database is not reachable right now. Nothing was deleted — please try again in a moment.", ex);
            }
            catch (DbUpdateException ex)
            {
                Fail("The ticket could not be deleted because the database rejected the change.", ex);
            }
            catch (Exception ex)
            {
                Fail("The ticket could not be deleted. Please try again in a moment.", ex);
            }
            finally
            {
                _saving = false;
                this.btnSave.Enabled = true;
                this.btnDelete.Enabled = true;
                Application.Update(this);
            }
        }

        #endregion

        #region Cancel — no context created, nothing written

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _trace('•', "editor", "Cancel: DialogResult.Cancel — no context created, nothing written");
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion

        #region Helpers

        /// <summary>Translates the QueryTrace stream from Commands.SaveAsync/DeleteAsync into the parent page's trace, at the same detail level a search gets.</summary>
        private void OnQueryTrace(TraceEntry entry)
        {
            switch (entry.Kind)
            {
                case TraceKind.Context:
                    _trace('◦', "context", entry.Text);
                    break;
                case TraceKind.Command:
                    _trace('→', "SQL", $"{entry.Text}   ({entry.Milliseconds:0.0} ms)");
                    break;
                case TraceKind.Failure:
                    _trace('→', "SQL failed", entry.Text);
                    break;
                case TraceKind.Note:
                    _trace('•', "editor", entry.Text);
                    break;
            }
        }

        private void Fail(string friendlyMessage, Exception ex)
        {
            ShowBanner(friendlyMessage);
            AlertBox.Show(friendlyMessage, MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);

            if (ex == null)
            {
                _trace('•', "editor", $"{friendlyMessage} → dialog stays open and usable");
                return;
            }

            var detail = ex.InnerException != null
                ? $"{ex.GetType().Name} → {ex.InnerException.GetType().Name}: {FirstSentence(ex.InnerException.Message)}"
                : $"{ex.GetType().Name}: {FirstSentence(ex.Message)}";
            _trace('•', "editor", $"caught {detail} → friendly message shown, dialog stays open and usable");
        }

        private void ShowBanner(string text)
        {
            this.labelBanner.Text = text;
            this.labelBanner.Visible = true;
        }

        private void HideBanner()
        {
            this.labelBanner.Visible = false;
        }

        private static string Fmt(int? value) => value?.ToString() ?? "—";

        private static string Truncate(string text, int max) => string.IsNullOrEmpty(text) || text.Length <= max ? text : text.Substring(0, max) + "…";

        private static string FirstSentence(string message)
        {
            var i = message.IndexOf(". ", StringComparison.Ordinal);
            return i > 0 ? message.Substring(0, i + 1) : message;
        }

        #endregion
    }
}
