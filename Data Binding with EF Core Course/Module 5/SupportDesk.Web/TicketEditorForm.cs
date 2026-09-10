using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupportDesk.Data;
using SupportDesk.Data.Diagnostics;
using SupportDesk.Services;
using Wisej.Services;
using Wisej.Web;

namespace SupportDesk.Web
{
    /// <summary>
    /// Module 5 lab props: which negative case <see cref="TicketEditorForm"/> should pre-fill itself with
    /// once its lookups and model are loaded, so the reviewer only has to click Save to see the result.
    /// None of these paint <c>errorProvider</c> icons on their own — only Save (or a live field change)
    /// does that; see <see cref="TicketEditorForm.ApplyLabScenario"/>.
    /// </summary>
    public enum EditorLabScenario
    {
        /// <summary>The ordinary path — Add or Edit with no preset.</summary>
        None,
        /// <summary>Customer and Category pre-selected, Title left blank.</summary>
        EmptyTitle,
        /// <summary>Customer and Category pre-selected, Title set to 200 characters (over the 180 limit).</summary>
        TitleTooLong,
        /// <summary>Title filled in, Customer and Category left unselected (null).</summary>
        NoCustomerCategory,
        /// <summary>An existing Closed ticket, DueDate pushed to next week in the model.</summary>
        ClosedFutureDueDate,
        /// <summary>A new, otherwise-valid ticket with <c>chkDuplicateNumber</c> pre-ticked.</summary>
        DuplicateNumber
    }

    /// <summary>
    /// Module 4's Add/Edit Ticket modal, now with Module 5's validation: DataAnnotations on
    /// <see cref="TicketEditModel"/>, <see cref="TicketValidator"/> for the domain layer, <c>ErrorProvider</c>
    /// and <c>validationSummaryLabel</c> for feedback, and <c>DbUpdateException</c> as the database's final
    /// word. Opened by <c>TicketBrowserPage</c> with <c>await editor.ShowDialogAsync()</c> and closed with
    /// <see cref="DialogResult.OK"/> (something was written, or the row was found already gone) or
    /// <see cref="DialogResult.Cancel"/> (nothing was).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Two lifetimes, kept apart.</b> This <see cref="Form"/>, its <c>editBindingSource</c> and the
    /// current <see cref="TicketEditModel"/> live for as long as the dialog is open — session state, the
    /// same rule the browser page follows for its grid. Every <c>DbContext</c> the dialog ever touches is
    /// created by <see cref="TicketCommandService"/>, used for exactly one read or one write, and disposed
    /// before that service method returns; the form never sees one. <see cref="TicketValidator"/> never
    /// creates one at all — it has no reference to <c>SupportDesk.Data</c> or the database.
    /// </para>
    /// <para>
    /// <b>Three validation layers, in order.</b> <see cref="RunValidation"/> is the UI layer: it calls
    /// <see cref="TicketValidator.Validate"/> (the domain layer) and, when told to paint, routes the result
    /// onto <c>errorProvider</c> and <c>validationSummaryLabel</c> through <see cref="ShowValidation"/>, and
    /// always keeps <c>btnSave.Enabled</c> equal to "the model is valid right now". <see cref="SaveAsync"/>
    /// calls it once more, synchronously, immediately after <c>EndEdit</c> — if that still reports messages,
    /// the method returns <b>before any <see cref="DbContext"/> is created</b>: EF Core and the database
    /// layer are never reached by an invalid model. The database is the third and final layer: a duplicate
    /// <see cref="Ticket.Number"/> is a failure the annotations and the cross-field rule cannot see coming
    /// (uniqueness can only be guaranteed by the database), so it surfaces as <see cref="DbUpdateException"/>
    /// from <c>SaveChangesAsync</c> and is handled separately, below the concurrency catch.
    /// </para>
    /// <para>
    /// <b>Painting is deliberately delayed.</b> <see cref="LoadEditorAsync"/> computes the model's initial
    /// validity (to set <c>btnSave.Enabled</c> correctly from the first frame) but never calls
    /// <see cref="ShowValidation"/> — no icon, no red summary, appears before the operator has touched
    /// anything or clicked Save. <c>_wireLiveValidation</c> stays false for the whole of
    /// <see cref="LoadEditorAsync"/> (including <see cref="ApplyLabScenario"/>'s presets, which are
    /// themselves programmatic field changes) and only becomes true at the very end, so the live-validation
    /// event handlers (<see cref="Field_Changed"/>, <see cref="DueDate_Changed"/>) — wired in the Designer to
    /// <c>Validated</c>/<c>SelectedValueChanged</c>/<c>ValueChanged</c> — do nothing until the operator (or
    /// the reviewer, driving a preset dialog) actually changes a field or presses Save. This is a deliberate
    /// choice: a blank "Add ticket" screen showing five red icons before a single keystroke would be
    /// unhelpful noise, not feedback.
    /// </para>
    /// <para>
    /// <b>The DateTimePicker is bound by hand, not through <c>DataBindings.Add</c>.</b> The course cookbook
    /// flags binding a nullable <c>DateTime?</c> model property to <c>DateTimePicker.Value</c> (which is
    /// not nullable) as unverified, and names the safer alternative: copy <c>Checked</c>/<c>Value</c> by
    /// hand. <see cref="LoadEditorAsync"/> sets them from the model after the model is loaded;
    /// <see cref="SaveAsync"/> and <see cref="DueDate_Changed"/> read them back into the model, before the
    /// model is validated or mapped onto the entity. Every other control — <c>TextBox</c>, <c>ComboBox</c>,
    /// <c>CheckBox</c> — is bound the ordinary way.
    /// </para>
    /// <para>
    /// <b>The trace.</b> The dialog has no trace list of its own — it logs into the parent page's, through
    /// the <c>trace</c> callback passed to the constructor. <see cref="OnQueryTrace"/> translates the same
    /// <see cref="TraceEntry"/> stream <c>TicketBrowserPage</c> reads (context created/disposed, SQL sent,
    /// service notes — including the failed <c>INSERT</c> a duplicate number produces) into that callback
    /// for <see cref="SaveAsync"/> and <see cref="DeleteAsync"/>, so a save or a delete shows the same level
    /// of detail as a search. <see cref="LoadEditorAsync"/> instead reports a single summary line — it is a
    /// background read, the same restraint <c>TicketBrowserPage.LoadLookupsAsync</c> uses for its own
    /// lookups.
    /// </para>
    /// </remarks>
    public partial class TicketEditorForm : Form
    {
        [Inject]
        private TicketCommandService Commands { get; set; }

        [Inject]
        private TicketValidator Validator { get; set; }

        private readonly int? _ticketId;
        private readonly TimeSpan _saveLatency;
        private readonly Action<char, string, string> _trace;
        private readonly EditorLabScenario _scenario;

        // The guard: Save and Delete share it, so a double click on either while the other is running is
        // dropped the same way TicketBrowserPage._loading drops a second search.
        private bool _saving;

        // Module 5: live-validation event handlers (Field_Changed, DueDate_Changed) are wired in the
        // Designer, but they no-op until this is true. It flips true only after LoadEditorAsync (lookups,
        // the model, any lab-scenario preset) has fully finished — see the class remarks.
        private bool _wireLiveValidation;

        private string _ticketNumber;

        /// <param name="ticketId">Null for "Add ticket"; an existing ticket's key for "Edit ticket".</param>
        /// <param name="saveLatency">Zero in the ordinary path; armed from the page's "Slow save (2.5 s)" toggle so the guard and the busy Save button can be watched.</param>
        /// <param name="trace">Receives (symbol, label, text) for one line in the parent page's trace list — the same shape <c>TicketBrowserPage.AddTraceRaw</c> renders.</param>
        /// <param name="scenario">Module 5 lab prop: which negative case to pre-fill once the model is loaded. None in the ordinary path.</param>
        public TicketEditorForm(int? ticketId, TimeSpan saveLatency, Action<char, string, string> trace, EditorLabScenario scenario = EditorLabScenario.None)
        {
            _ticketId = ticketId;
            _saveLatency = saveLatency;
            _trace = trace ?? ((symbol, label, text) => { });
            _scenario = scenario;

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
                return;
            }
            catch (Exception ex)
            {
                _trace('•', "editor", $"LoadEditorAsync failed: {ex.GetType().Name}: {ex.Message} — closing with Cancel, nothing changed");
                AlertBox.Show("The ticket could not be loaded right now. Please try again in a moment.", MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                this.DialogResult = DialogResult.Cancel;
                Close();
                return;
            }
        }

        #region Load — lookups, then the model; the model becomes current only once both are ready

        private async Task LoadEditorAsync()
        {
            if (Commands == null || Validator == null)
            {
                _trace('•', "editor", "TicketCommandService/TicketValidator not injected — the IServiceProvider bridge is missing");
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

            // Module 5 lab scenario: pre-fill the fields a demo button asked for. Nothing here paints —
            // _wireLiveValidation is still false — the reviewer clicks Save to see the icons/summary.
            ApplyLabScenario(model, lookups);

            // dtpDueDate is not bound through DataBindings.Add — see the class remarks. Copy by hand, now
            // that the model (and any lab-scenario DueDate) exists.
            this.dtpDueDate.Checked = model.DueDate.HasValue;
            this.dtpDueDate.Value = model.DueDate ?? DateTime.Today;

            // chkDuplicateNumber only means anything for a new ticket — SaveAsync only reads it when
            // model.Id == 0. Hide it for Edit so it cannot mislead the operator into thinking it does
            // something on an existing ticket.
            this.chkDuplicateNumber.Visible = _ticketId is null;

            // Module 5: compute initial validity WITHOUT painting icons (see the class remarks). Save
            // starts disabled for a blank new ticket, with a neutral nudge instead of the raw validator
            // text; an existing ticket (or a scenario preset) keeps whatever validity it actually has.
            this.errorProvider.Clear();
            var initialMessages = Validator.Validate(model);
            this.btnSave.Enabled = initialMessages.Count == 0;
            if (initialMessages.Count > 0 && _ticketId is null)
            {
                this.validationSummaryLabel.Text = "Fill in Title, Customer and Category.";
                this.validationSummaryLabel.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
                this.validationSummaryLabel.BackColor = this.BackColor;
                this.validationSummaryLabel.Visible = true;
            }
            else
            {
                this.validationSummaryLabel.Visible = false;
            }

            _trace('•', "editor", $"LoadEditorAsync({(_ticketId is int i ? "#" + i : "new")}): {(_ticketId is null ? 0 : 1)} no-tracking read + lookups ({scope.Commands} statement(s), {scope.ContextsCreated} context created, {scope.ContextsDisposed} disposed)" + (_scenario == EditorLabScenario.None ? "" : $" · lab scenario {_scenario}"));

            // Only now do field changes start repainting errorProvider/validationSummaryLabel live.
            _wireLiveValidation = true;

            // Verified in the browser: Save is disabled while the model is invalid, so a disabled Save can
            // never be the thing that paints the first icons. A lab scenario pre-fills the fields for the
            // reviewer, which counts as "touched": run the validator once now so the icons and the summary
            // are visible the moment the dialog opens. A hand-typed ticket still gets its first icons from
            // the live re-check when a field changes (Field_Changed / DueDate_Changed).
            if (_scenario is EditorLabScenario.EmptyTitle or EditorLabScenario.TitleTooLong
                or EditorLabScenario.NoCustomerCategory or EditorLabScenario.ClosedFutureDueDate)
            {
                _trace('•', "editor", $"lab scenario {_scenario}: fields pre-filled, validator runs once as if you had touched them");
                RunValidation(paint: true);
            }
            Application.Update(this);
        }

        /// <summary>
        /// Pre-fills <paramref name="model"/> (and, for <see cref="EditorLabScenario.DuplicateNumber"/>,
        /// ticks <c>chkDuplicateNumber</c>) for the negative case a Module 5 demo button asked for. Called
        /// from <see cref="LoadEditorAsync"/> while <c>_wireLiveValidation</c> is still false, so none of
        /// this paints — the reviewer clicks Save to see the result, the same as typing it in by hand would.
        /// </summary>
        private void ApplyLabScenario(TicketEditModel model, EditorLookups lookups)
        {
            int? firstCustomerId = lookups.Customers.Count > 0 ? lookups.Customers[0].Id : (int?)null;
            int? firstCategoryId = lookups.Categories.Count > 0 ? lookups.Categories[0].Id : (int?)null;

            switch (_scenario)
            {
                case EditorLabScenario.None:
                    return;

                case EditorLabScenario.EmptyTitle:
                    model.Title = "";
                    model.CustomerId = firstCustomerId;
                    model.CategoryId = firstCategoryId;
                    break;

                case EditorLabScenario.TitleTooLong:
                    model.Title = new string('A', 200);
                    model.CustomerId = firstCustomerId;
                    model.CategoryId = firstCategoryId;
                    break;

                case EditorLabScenario.NoCustomerCategory:
                    model.Title = "Demo ticket: no customer or category";
                    model.CustomerId = null;
                    model.CategoryId = null;
                    break;

                case EditorLabScenario.ClosedFutureDueDate:
                    // _ticketId names an existing Closed ticket the page picked — push its DueDate a week
                    // into the future so the cross-field rule TicketValidator appends by hand has something
                    // to report.
                    model.DueDate = DateTime.Today.AddDays(7);
                    break;

                case EditorLabScenario.DuplicateNumber:
                    model.Title = "Demo ticket: duplicate number";
                    model.CustomerId = firstCustomerId;
                    model.CategoryId = firstCategoryId;
                    this.chkDuplicateNumber.Checked = true;
                    break;
            }
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

        #region Live validation — re-run on every field change, but never before the operator touched one

        /// <summary>
        /// Wired (in the Designer) to <c>Validated</c> on <c>txtTitle</c>/<c>txtDescription</c> and
        /// <c>SelectedValueChanged</c> on <c>cboCustomer</c>/<c>cboCategory</c>/<c>cboStatus</c> — every
        /// control whose <c>DataBindings.Add</c> entry already pushed the new value into the model by the
        /// time this fires. No-ops until <see cref="LoadEditorAsync"/> has finished (<c>_wireLiveValidation</c>).
        /// </summary>
        private void Field_Changed(object sender, EventArgs e)
        {
            if (!_wireLiveValidation)
                return;

            RunValidation(paint: true);
        }

        /// <summary>
        /// <c>dtpDueDate</c> is not bound through <c>DataBindings.Add</c> (see the class remarks), so unlike
        /// <see cref="Field_Changed"/> this copies <c>Checked</c>/<c>Value</c> into the model by hand before
        /// validating. Wired to both <c>ValueChanged</c> and <c>Validated</c> so a plain date edit and a
        /// tick/untick of the picker's checkbox both re-run the validator.
        /// </summary>
        private void DueDate_Changed(object sender, EventArgs e)
        {
            if (!_wireLiveValidation)
                return;

            if (this.editBindingSource.Current is TicketEditModel model)
                model.DueDate = this.dtpDueDate.Checked ? this.dtpDueDate.Value.Date : (DateTime?)null;

            RunValidation(paint: true);
        }

        /// <summary>
        /// Runs <see cref="TicketValidator.Validate"/> against the BindingSource's current model, always
        /// keeps <c>btnSave.Enabled</c> equal to "no messages", and — only when <paramref name="paint"/> —
        /// routes the result through <see cref="ShowValidation"/> and logs the detailed trace lines the
        /// README quotes (<c>• validation Title: … · CustomerId: …</c> and
        /// <c>• editor Save enabled/disabled: model valid/invalid</c>).
        /// </summary>
        /// <returns>The messages <see cref="TicketValidator.Validate"/> returned.</returns>
        private IReadOnlyList<ValidationMessage> RunValidation(bool paint)
        {
            if (Validator == null || this.editBindingSource.Current is not TicketEditModel model)
                return Array.Empty<ValidationMessage>();

            var messages = Validator.Validate(model);
            var valid = messages.Count == 0;

            if (paint)
            {
                ShowValidation(messages);

                var detail = valid
                    ? "no messages — model valid"
                    : string.Join(" · ", messages.Select(m => $"{m.FieldName ?? "(summary)"}: {m.Message}"));
                _trace('•', "validation", detail);
                _trace('•', "editor", valid ? "Save enabled: model valid" : "Save disabled: model invalid");
            }

            this.btnSave.Enabled = valid && !_saving;
            return messages;
        }

        /// <summary>
        /// Clears every icon, then paints one <c>errorProvider.SetError</c> per field <see cref="ValidationMessage"/>
        /// this form has a control for (<c>Title</c>, <c>Description</c>, <c>Status</c>, <c>Priority</c>,
        /// <c>CustomerId</c>, <c>CategoryId</c>, <c>DueDate</c>) and joins every message — field-level and
        /// the <see langword="null"/>-field cross-field rule alike — into <c>validationSummaryLabel</c>.
        /// </summary>
        private void ShowValidation(IReadOnlyList<ValidationMessage> messages)
        {
            this.errorProvider.Clear();

            foreach (var group in messages.GroupBy(m => m.FieldName))
            {
                var text = string.Join(" ", group.Select(m => m.Message));
                switch (group.Key)
                {
                    case nameof(TicketEditModel.Title):
                        this.errorProvider.SetError(this.txtTitle, text);
                        break;
                    case nameof(TicketEditModel.Description):
                        this.errorProvider.SetError(this.txtDescription, text);
                        break;
                    case nameof(TicketEditModel.Status):
                        this.errorProvider.SetError(this.cboStatus, text);
                        break;
                    case nameof(TicketEditModel.Priority):
                        this.errorProvider.SetError(this.cboPriority, text);
                        break;
                    case nameof(TicketEditModel.CustomerId):
                        this.errorProvider.SetError(this.cboCustomer, text);
                        break;
                    case nameof(TicketEditModel.CategoryId):
                        this.errorProvider.SetError(this.cboCategory, text);
                        break;
                    case nameof(TicketEditModel.DueDate):
                        this.errorProvider.SetError(this.dtpDueDate, text);
                        break;
                        // A null FieldName (or one this form has no control for) reaches only the summary below.
                }
            }

            if (messages.Count == 0)
            {
                this.validationSummaryLabel.Visible = false;
                return;
            }

            this.validationSummaryLabel.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.validationSummaryLabel.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.validationSummaryLabel.Text = string.Join("   ·   ", messages.Select(m => m.Message));
            this.validationSummaryLabel.Visible = true;
        }

        #endregion

        #region Save — guard, Clear, EndEdit, TicketValidator, a fresh context, map, SaveChangesAsync

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

                this.errorProvider.Clear();
                this.editBindingSource.EndEdit();
                var model = (TicketEditModel)this.editBindingSource.Current;

                // dtpDueDate is copied by hand — see the class remarks — right after EndEdit, so the model
                // reflects the picker no matter which control the operator touched last.
                model.DueDate = this.dtpDueDate.Checked ? this.dtpDueDate.Value.Date : (DateTime?)null;

                _trace('•', "editor", $"EndEdit → model {{Title='{Truncate(model.Title, 40)}', CustomerId={Fmt(model.CustomerId)}, AgentId={Fmt(model.AgentId)}, CategoryId={Fmt(model.CategoryId)}, Status={model.Status}, Priority={model.Priority}, DueDate={(model.DueDate?.ToString("yyyy-MM-dd") ?? "—")}, IsUrgent={model.IsUrgent}}}");

                // The Module 5 validator: DataAnnotations on TicketEditModel plus the closed/future-due-date
                // rule. RunValidation(paint: true) both shows the icons/summary AND logs the detailed trace
                // lines (• validation Title: … · editor Save enabled/disabled: …).
                var messages = RunValidation(paint: true);
                if (messages.Count > 0)
                {
                    _trace('•', "editor", $"validation {messages.Count} message(s) → shown, no context created");
                    return;
                }

                var result = await Commands.SaveAsync(model, _saveLatency, this.chkDuplicateNumber.Checked && model.Id == 0);
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
                await FailDbUpdateAsync(ex);
            }
            catch (Exception ex)
            {
                Fail("The ticket could not be saved. Please try again in a moment.", ex);
            }
            finally
            {
                _saving = false;
                // btnSave.Enabled reflects the model's validity again (not just "true") — RunValidation is
                // UI-safe to call even after Close() already ran on the success path.
                RunValidation(paint: false);
                this.btnDelete.Enabled = true;
                Application.Update(this);
            }
        }

        /// <summary>
        /// Module 5's final safety layer: the database refused the change (the lab's example is a duplicate
        /// <see cref="Ticket.Number"/> — <see cref="TicketCommandService.SaveAsync(TicketEditModel, TimeSpan, bool, System.Threading.CancellationToken)"/>'s
        /// <c>forceDuplicateNumber</c> lab prop reproduces it on demand). The full exception — including the
        /// inner <c>SqliteException</c> "UNIQUE constraint failed: Tickets.Number" — is written to the server
        /// log and reported by the interceptor into the trace as its own <c>→ SQL failed</c> line
        /// (see <see cref="OnQueryTrace"/>); the operator sees exactly one plain sentence, never the SQL, the
        /// constraint name or anything about the connection. A stale foreign key is one plausible cause too
        /// (a customer or category deleted while the dialog was open), so the lookups are reloaded here.
        /// </summary>
        private async Task FailDbUpdateAsync(DbUpdateException ex)
        {
            Console.Error.WriteLine($"[SupportDesk] server log: DbUpdateException saving ticket — {ex}");

            var friendly = FriendlyDatabaseErrors.TicketSaveRejected;
            ShowBanner(friendly);
            AlertBox.Show(friendly, MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            _trace('•', "editor", "caught DbUpdateException → friendly message shown, full exception logged server-side");

            await ReloadLookupsAsync();
        }

        /// <summary>Re-fetches the three lookup ComboBoxes' DataSources — a foreign key may have gone stale while the dialog was open.</summary>
        private async Task ReloadLookupsAsync()
        {
            if (Commands == null)
                return;

            try
            {
                var lookups = await Commands.GetLookupsAsync();

                this.cboCustomer.DataSource = lookups.Customers.ToList();

                var agentRows = new List<LookupItem> { new LookupItem(TicketCommandService.UnassignedAgentId, "— unassigned —") };
                agentRows.AddRange(lookups.Agents);
                this.cboAgent.DataSource = agentRows;

                this.cboCategory.DataSource = lookups.Categories.ToList();

                _trace('•', "editor", "lookups reloaded — a foreign key may have gone stale while the dialog was open");
            }
            catch (Exception ex)
            {
                _trace('•', "editor", $"lookups not reloaded: {ex.GetType().Name} — {FirstSentence(ex.Message)}");
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
                Fail(FriendlyDatabaseErrors.TicketDeleteRejected, ex);
            }
            catch (Exception ex)
            {
                Fail("The ticket could not be deleted. Please try again in a moment.", ex);
            }
            finally
            {
                _saving = false;
                RunValidation(paint: false);
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
