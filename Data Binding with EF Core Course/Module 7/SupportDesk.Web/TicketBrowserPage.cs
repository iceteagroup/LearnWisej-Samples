using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupportDesk.Data.Diagnostics;
using SupportDesk.Services;
using Wisej.Services;
using Wisej.Web;

namespace SupportDesk.Web
{
    /// <summary>
    /// Module 5 lab page — the Support Desk ticket browser's Add/Edit editor gains validation, on top of
    /// Module 4's Add/Edit, Module 3's search and paging, Module 2's model and Module 1's first query.
    ///
    /// Left card:  the lab controls (searchTextBox, statusComboBox, customerComboBox, the two due-date
    ///             pickers, searchButton, ticketsDataGridView bound through ticketBindingSource,
    ///             statusLabel, prevPageButton / nextPageButton, and CellDoubleClick opens the editor)
    ///             and the friendly error banner.
    /// Right card: everything EF Core does inside one click — context created, SQL, context disposed.
    ///             A search is exactly two statements: a COUNT and one paged SELECT; a save or a delete
    ///             inside <see cref="TicketEditorForm"/> reports at the same detail level, through the
    ///             <see cref="AddTraceRaw"/> callback the dialog is given when it is opened — including
    ///             the validator's own trace lines and the failed INSERT a duplicate ticket number produces.
    /// Under both: the Module 2 "Model &amp; migration" card and the Module 1 four-lifetimes table.
    /// Bottom bars: row 1 = the Module 5 validation paths (five one-click negative-case demos that open the
    ///             editor pre-filled, plus <c>buttonBreak</c>/<c>buttonRestore</c>/<c>buttonSlowSave</c>,
    ///             since every dialog this row opens still needs them — <c>buttonBreak</c>/<c>buttonRestore</c>
    ///             govern the editor's Save and Delete through the shared <see cref="Outage"/> switch);
    ///             row 2 = the Module 4 editor paths (Add, Edit, "simulate another operator deletes it");
    ///             row 3 = the Module 3 browser paths (slow search, the bound-IQueryable anti-pattern);
    ///             row 4 = the Module 2 model paths; row 5 = the Module 1 lifetimes paths.
    ///
    /// The page is session state: it lives on the server for as long as the browser tab does, and so do
    /// the BindingSource, the fifty <see cref="TicketListItem"/> records it holds and the selected row.
    /// The services are stateless; every DbContext is created and disposed inside one handler — including
    /// the ones <see cref="TicketCommandService"/> creates for <see cref="TicketEditorForm"/>'s Save and
    /// Delete, which never becomes state the page holds.
    /// </summary>
    public partial class TicketBrowserPage : Page
    {
        /// <summary>Rows per page. One number, used by the criteria, the status label and the paging maths.</summary>
        private const int PageSize = 50;

        /// <summary>The key the "All customers" lookup row carries — it becomes a null criterion.</summary>
        private const int AllCustomersId = 0;

        /// <summary>The key the "All statuses" lookup row carries — it becomes a null criterion.</summary>
        private const string AllStatusesKey = "";

        // Resolved through Microsoft DI: Startup.cs registered app.Services with Wisej.NET.
        [Inject]
        private TicketQueryService TicketQueries { get; set; }

        [Inject]
        private TicketCommandService Commands { get; set; }

        [Inject]
        private SchemaInfoService SchemaInfo { get; set; }

        [Inject]
        private DevelopmentSeeder Seeder { get; set; }

        [Inject]
        private ModelDemoService ModelDemos { get; set; }

        [Inject]
        private SharedContextAntiPattern AntiPattern { get; set; }

        [Inject]
        private BoundIQueryableAntiPattern BoundQueryAntiPattern { get; set; }

        [Inject]
        private DevelopmentOutageSwitch Outage { get; set; }

        [Inject]
        private TransactionFailureSwitch TransactionFailure { get; set; }

        [Inject]
        private StartupDiagnostics Diagnostics { get; set; }

        private readonly DateTime _createdAt = DateTime.Now;
        private readonly object _traceLock = new object();

        // The loading guard: one database operation per page at a time.
        private bool _loading;

        private int _handlerRuns;
        private int _contextsCreated;
        private int _contextsDisposed;

        // Browser state — session state, exactly like the grid and the BindingSource.
        private int _pageIndex;
        private int _totalCount;

        // Module 4: armed by buttonSlowSave, read once when the next editor dialog opens — a latency
        // inside TicketCommandService.SaveAsync's unit of work, like Module 3's slow search.
        private bool _slowSaveOn;

        public TicketBrowserPage()
        {
            InitializeComponent();
        }

        private async void TicketBrowserPage_Load(object sender, EventArgs e)
        {
            AddTrace(Glyph.Server, "session",
                $"page created for session {ShortSessionId()} · services through [Inject] → " +
                (TicketQueries != null && Seeder != null ? "resolved from Microsoft DI" : "NULL — the IServiceProvider bridge is missing"));
            UpdateLifetimes();
            SetState("● idle", StateKind.Ok);

            // Module 7: cboRole is filled once, here — Agent first (the default, safer choice), Supervisor
            // second. No database round trip; ConflictResolution.CanOverwrite reads whichever is selected
            // when the editor is opened.
            var roleRows = new List<NamedValue> { new NamedValue(nameof(UserRole.Agent), "Agent"), new NamedValue(nameof(UserRole.Supervisor), "Supervisor") };
            this.cboRole.DisplayMember = nameof(NamedValue.Name);
            this.cboRole.ValueMember = nameof(NamedValue.Value);
            this.cboRole.DataSource = roleRows;
            this.cboRole.SelectedIndex = 0;

            RefreshEnvironmentPanel();

            // The lookups come FIRST: a SelectedValue with no matching item resolves to nothing, so the
            // ComboBoxes must be filled before any criteria are read from them.
            await LoadLookupsAsync();
            await RefreshModelCardAsync();

            // …and only then the first search.
            await LoadTicketsAsync("page Load");

            Application.Update(this);
        }

        #region The pattern every handler follows

        /// <summary>
        /// The pattern of the whole course: guard → busy UI → one awaited service call → result,
        /// friendly message in catch, UI restored in finally. No DbContext is visible here at all.
        /// </summary>
        /// <param name="origin">Which handler runs (for the trace).</param>
        /// <param name="call">The service call, as shown in the trace.</param>
        /// <param name="busyStatus">Text for statusLabel while the operation runs.</param>
        /// <param name="operation">The awaited service call; returns the text for statusLabel.</param>
        /// <param name="dbUpdateMessage">Friendly message when the database rejects the change (constraint, foreign key).</param>
        /// <param name="genericMessage">Friendly message for any other failure.</param>
        private async Task RunAsync(string origin, string call, string busyStatus, Func<Task<string>> operation, string dbUpdateMessage, string genericMessage)
        {
            _handlerRuns++;

            if (_loading)
            {
                AddTrace(Glyph.Server, "guard", $"{origin}: an operation is already running — this click is ignored");
                UpdateLifetimes();
                return;
            }

            using var trace = QueryTrace.Begin(OnTrace);
            try
            {
                _loading = true;
                SetBusy(true);
                HideBanner();
                SetState("● working", StateKind.Normal);
                statusLabel.Text = busyStatus;
                AddTrace(Glyph.Server, origin, call);

                var result = await operation();

                statusLabel.Text = result;
                AddTrace(Glyph.Result, "result", $"{result} · {trace.Commands} statement(s) · {trace.Milliseconds:0.0} ms in the database · {trace.ContextsCreated} context created, {trace.ContextsDisposed} disposed");
                if (this.labelState.Text == "● working")
                    SetState("● ok", StateKind.Ok);          // an operation may have set its own state ("● nothing to seed")
            }
            catch (DatabaseUnavailableException ex)
            {
                Fail("The Support Desk database is not reachable right now. Nothing was changed — please try again in a moment.", ex);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Fail("Someone else changed this row in the meantime. Nothing was saved — reload and try again.", ex);
            }
            catch (DbUpdateException ex)
            {
                Fail(dbUpdateMessage, ex);
            }
            catch (Exception ex)
            {
                Fail(genericMessage, ex);
            }
            finally
            {
                _loading = false;
                SetBusy(false);
                await RefreshModelCardAsync();
                UpdateLifetimes();
                Application.Update(this);
            }
        }

        #endregion

        #region Module 3 · success path: the ticket browser

        private async void searchButton_Click(object sender, EventArgs e)
        {
            // A new search always starts at the first page: the old page 4 may not exist under new filters.
            _pageIndex = 0;
            await LoadTicketsAsync("searchButton_Click");
        }

        private async void nextPageButton_Click(object sender, EventArgs e)
        {
            _pageIndex++;
            AddTrace(Glyph.Server, "paging", $"nextPageButton_Click → page {_pageIndex + 1}: one new query with OFFSET {_pageIndex * PageSize}, not a cached copy of the result set");
            await LoadTicketsAsync("nextPageButton_Click");
        }

        private async void prevPageButton_Click(object sender, EventArgs e)
        {
            _pageIndex = Math.Max(0, _pageIndex - 1);
            AddTrace(Glyph.Server, "paging", $"prevPageButton_Click → page {_pageIndex + 1}: one new query with OFFSET {_pageIndex * PageSize}, not a cached copy of the result set");
            await LoadTicketsAsync("prevPageButton_Click");
        }

        private async void buttonSlowSearch_Click(object sender, EventArgs e)
        {
            await LoadTicketsAsync("buttonSlowSearch_Click", TimeSpan.FromSeconds(2.5));
        }

        /// <summary>
        /// The lab's method: build the criteria from the controls, run one search behind the loading guard,
        /// and hand the <b>materialised list</b> to the BindingSource. Everything else on the page follows
        /// from those three lines.
        /// </summary>
        private Task LoadTicketsAsync(string origin, TimeSpan latency = default)
        {
            var criteria = ReadCriteria();
            var slow = latency > TimeSpan.Zero;

            return RunAsync(
                origin,
                slow
                    ? $"TicketQueryService.SearchTicketsSlowlyAsync({criteria.Describe()}, {latency.TotalSeconds:0.#} s)"
                    : $"TicketQueryService.SearchTicketsAsync({criteria.Describe()})",
                slow ? $"Loading tickets… (simulated {latency.TotalSeconds:0.#} s latency)" : "Loading tickets…",
                async () =>
                {
                    var result = slow
                        ? await TicketQueries.SearchTicketsSlowlyAsync(criteria, latency)
                        : await TicketQueries.SearchTicketsAsync(criteria);

                    _totalCount = result.TotalCount;

                    // The one line the whole module is about: a real List<TicketListItem> goes in, never the
                    // query it came from. The BindingSource is session state; the context that produced the
                    // rows is already gone.
                    this.ticketBindingSource.DataSource = result.Items.ToList();
                    this.ticketBindingSource.ResetBindings(false);

                    if (result.TotalCount == 0)
                    {
                        _pageIndex = 0;
                        SetState("● no matches", StateKind.Warn);
                        return $"No tickets match these filters · page size {PageSize}";
                    }

                    return $"Showing {result.Items.Count} of {result.TotalCount} tickets · page {_pageIndex + 1} of {result.PageCount(PageSize)} · page size {PageSize}";
                },
                "The tickets could not be loaded because the database rejected the query.",
                "Tickets could not be loaded. Your filters are unchanged — please try again in a moment.");
        }

        /// <summary>
        /// Turns the controls into a <see cref="TicketSearchCriteria"/>. The ComboBoxes give up their
        /// <b>keys</b>, never their display text: <c>SelectedValue</c> is a boxed object, so it is matched
        /// with <c>is int</c> / <c>is string</c>, and the "All" rows map to <c>null</c> — which is how an
        /// unset filter stops existing instead of becoming a clause that matches everything.
        /// </summary>
        private TicketSearchCriteria ReadCriteria()
        {
            var text = this.searchTextBox.Text;

            var status = this.statusComboBox.SelectedValue is string s && s != AllStatusesKey ? s : null;

            var customerId = this.customerComboBox.SelectedValue is int id && id != AllCustomersId ? id : (int?)null;

            // ShowCheckBox = true: an unticked picker means "no bound", not "today".
            var dueFrom = this.dueFromDateTimePicker.Checked ? this.dueFromDateTimePicker.Value.Date : (DateTime?)null;
            var dueTo = this.dueToDateTimePicker.Checked ? this.dueToDateTimePicker.Value.Date : (DateTime?)null;

            return new TicketSearchCriteria
            {
                Text = text,
                Status = status,
                CustomerId = customerId,
                DueFrom = dueFrom,
                DueTo = dueTo,
                PageIndex = _pageIndex,
                PageSize = PageSize
            };
        }

        /// <summary>
        /// Loads the two lookups before the first search and gives each ComboBox its
        /// <c>DisplayMember</c> (what the operator reads) and <c>ValueMember</c> (what the query filters
        /// by). Both lists get an "All" row whose key becomes a null criterion. It runs in its own trace
        /// scope and reports one summary line.
        /// </summary>
        private async Task LoadLookupsAsync()
        {
            if (TicketQueries == null)
                return;

            using var scope = QueryTrace.Begin(entry => { if (entry.Kind == TraceKind.Note) AddTrace(Glyph.Server, "lookup", entry.Text); });
            try
            {
                var statuses = await TicketQueries.GetStatusesAsync();
                var customers = await TicketQueries.GetCustomersAsync();
                _contextsCreated += scope.ContextsCreated;
                _contextsDisposed += scope.ContextsDisposed;

                var statusRows = new List<LookupText> { new LookupText(AllStatusesKey, "All statuses") };
                statusRows.AddRange(statuses.Select(status => new LookupText(status, status)));
                this.statusComboBox.DisplayMember = nameof(LookupText.Name);
                this.statusComboBox.ValueMember = nameof(LookupText.Key);
                this.statusComboBox.DataSource = statusRows;
                this.statusComboBox.SelectedIndex = 0;

                var customerRows = new List<LookupItem> { new LookupItem(AllCustomersId, "All customers") };
                customerRows.AddRange(customers);
                this.customerComboBox.DisplayMember = nameof(LookupItem.Name);
                this.customerComboBox.ValueMember = nameof(LookupItem.Id);
                this.customerComboBox.DataSource = customerRows;
                this.customerComboBox.SelectedIndex = 0;

                this.dueFromDateTimePicker.Value = DateTime.Today;
                this.dueToDateTimePicker.Value = DateTime.Today.AddDays(14);

                AddTrace(Glyph.Context, "lookups",
                    $"statusComboBox {statusRows.Count} rows, customerComboBox {customerRows.Count} rows — filled BEFORE the first search · " +
                    $"{scope.Commands} statement(s), {scope.ContextsCreated} context created, {scope.ContextsDisposed} disposed");
            }
            catch (Exception ex)
            {
                _contextsCreated += scope.ContextsCreated;
                _contextsDisposed += scope.ContextsDisposed;
                AddTrace(Glyph.Context, "lookups", $"not loaded: {ex.GetType().Name} — {FirstSentence(ex.Message)}");
                ShowBanner("The filter lists could not be loaded. Search still works without them.");
                SetState("● fault", StateKind.Error);
            }
        }

        /// <summary>A status lookup row: a string key instead of an integer one.</summary>
        private sealed class LookupText
        {
            public LookupText(string key, string name)
            {
                Key = key;
                Name = name;
            }

            public string Key { get; }

            public string Name { get; }
        }

        private void UpdatePagingButtons()
        {
            var pageCount = _totalCount <= 0 ? 1 : (_totalCount + PageSize - 1) / PageSize;
            this.prevPageButton.Enabled = _pageIndex > 0;
            this.nextPageButton.Enabled = _pageIndex + 1 < pageCount;
        }

        #endregion

        #region Module 4 · success path: Add ticket, Edit ticket

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            await OpenEditorAsync(null, "Add ticket");
        }

        private async void btnEdit_Click(object sender, EventArgs e)
        {
            var selected = this.ticketsDataGridView.CurrentRow?.DataBoundItem as TicketListItem;
            if (selected == null)
            {
                AddTrace(Glyph.Server, "dialog", "Edit ticket: no row selected in the grid — pick a ticket first");
                return;
            }

            await OpenEditorAsync(selected.Id, $"Edit ticket {selected.Number}");
        }

        private async void ticketsDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (this.ticketsDataGridView.Rows[e.RowIndex].DataBoundItem is TicketListItem item)
                await OpenEditorAsync(item.Id, $"Edit ticket {item.Number}");
        }

        /// <summary>
        /// Opens <see cref="TicketEditorForm"/> as an awaitable modal and re-runs the search — resetting
        /// back to page 1, since a save always sorts the changed row to the top — only when the dialog
        /// closes with <see cref="DialogResult.OK"/>. Every other result leaves the grid exactly as it was;
        /// either way the outcome is traced, so "nothing happened" is as visible as "the grid refreshed".
        /// </summary>
        private Task OpenEditorAsync(int? ticketId, string label) => OpenEditorAsync(ticketId, label, EditorLabScenario.None);

        /// <param name="scenario">Module 5 lab prop: which negative case the dialog should pre-fill itself with — see EditorLabScenario.</param>
        private async Task OpenEditorAsync(int? ticketId, string label, EditorLabScenario scenario)
        {
            if (_loading)
            {
                AddTrace(Glyph.Server, "guard", $"{label}: an operation is already running — this click is ignored");
                return;
            }

            var latency = _slowSaveOn ? TimeSpan.FromSeconds(2.5) : TimeSpan.Zero;
            var role = ReadRole();
            AddTrace(Glyph.Server, "dialog", $"{label}: opening TicketEditorForm (role {role})" + (latency > TimeSpan.Zero ? " (slow save armed — the next Save waits 2.5 s inside its unit of work)" : ""));

            DialogResult result;
            using (var editor = new TicketEditorForm(ticketId, latency, AddTraceRaw, role, scenario))
            {
                result = await editor.ShowDialogAsync();
            }

            if (result == DialogResult.OK)
            {
                AddTrace(Glyph.Server, "dialog", $"{label} → OK → grid refreshed");
                _pageIndex = 0;
                await LoadTicketsAsync("dialog OK → refresh");
            }
            else
            {
                AddTrace(Glyph.Server, "dialog", $"{label} → Cancel → nothing persisted, grid untouched");
            }
        }

        private void buttonSlowSave_Click(object sender, EventArgs e)
        {
            _slowSaveOn = !_slowSaveOn;
            this.buttonSlowSave.Text = _slowSaveOn ? "Slow save (2.5 s): ON" : "Slow save (2.5 s): off";
            AddTrace(Glyph.Server, "toggle", $"slow save armed for the next Add/Edit dialog: {_slowSaveOn}");
        }

        /// <summary>
        /// Failure-path lab prop: deletes the selected row through <see cref="TicketCommandService"/>
        /// directly, <b>bypassing the editor</b> — exactly as if another operator (or another browser tab)
        /// had done it. The grid is deliberately left showing the now-deleted row: open Edit (or Delete
        /// inside the editor) on it afterwards to see the "already deleted" handling that
        /// <see cref="TicketEditorForm"/> demonstrates.
        /// </summary>
        private async void buttonSimulateDelete_Click(object sender, EventArgs e)
        {
            var selected = this.ticketsDataGridView.CurrentRow?.DataBoundItem as TicketListItem;
            if (selected == null)
            {
                AddTrace(Glyph.Server, "simulate", "no row selected in the grid — pick a ticket first");
                return;
            }

            await RunAsync(
                "buttonSimulateDelete_Click",
                $"TicketCommandService.DeleteAsync(#{selected.Id} / {selected.Number}) — bypassing the editor, as if another operator deleted it",
                "Simulating another operator deleting the selected ticket…",
                async () =>
                {
                    var result = await Commands.DeleteAsync(selected.Id);
                    switch (result.Outcome)
                    {
                        case DeleteOutcome.Deleted:
                            return $"{result.Number} deleted behind the scenes — the grid still shows it until the next search. Open Edit on it to see the already-deleted handling.";
                        case DeleteOutcome.Refused:
                            SetState("● nothing to do", StateKind.Warn);
                            return $"{result.Reason} (the simulate button honours the same business rule the editor does)";
                        default:
                            return "It was already gone.";
                    }
                },
                "The simulated delete could not run because the database rejected the change.",
                "The simulated delete could not run. Please try again in a moment.");
        }

        #endregion

        #region Module 7 · concurrency and transactions

        /// <summary>Reads <c>cboRole</c> — Agent by default if nothing is selected yet (should not happen once Load has run).</summary>
        private UserRole ReadRole()
            => this.cboRole.SelectedValue is string s && Enum.TryParse<UserRole>(s, out var role) ? role : UserRole.Agent;

        /// <summary>
        /// Reproduces the concurrency conflict without a second browser session: updates the selected ticket
        /// through a brand-new context, exactly as another operator's save would, while the editor may
        /// already be open on the same row with the old <c>RowVersion</c>. Open Edit first (leave the dialog
        /// open or close and reopen it), click this, then Save in the editor — the stale token makes
        /// <c>SaveChangesAsync</c> throw <c>DbUpdateConcurrencyException</c> and the conflict dialog opens.
        /// See <c>docs/ConcurrencyResolution.md</c> for the two-browser-session alternative.
        /// </summary>
        private async void buttonSimulateChange_Click(object sender, EventArgs e)
        {
            var selected = this.ticketsDataGridView.CurrentRow?.DataBoundItem as TicketListItem;
            if (selected == null)
            {
                AddTrace(Glyph.Server, "simulate", "no row selected in the grid — pick a ticket first");
                return;
            }

            await RunAsync(
                "buttonSimulateChange_Click",
                $"TicketCommandService.SimulateAnotherOperatorChangeAsync(#{selected.Id} / {selected.Number}) — Status = Closed, Priority = High, through a separate context",
                "Simulating another operator changing the selected ticket…",
                async () =>
                {
                    var result = await Commands.SimulateAnotherOperatorChangeAsync(selected.Id);
                    return $"{result.Number} changed behind the scenes (Status = Closed, Priority = High) — a fresh RowVersion is on the row now. Open Edit on it (or Save an editor already open on it) to see the conflict.";
                },
                "The simulated change could not run because the database rejected it.",
                "The simulated change could not run. Please try again in a moment.");
        }

        /// <summary>The transaction success path: both writes land, or neither does.</summary>
        private async void buttonCloseWithComment_Click(object sender, EventArgs e)
        {
            await CloseWithCommentAsync("buttonCloseWithComment_Click", armFailure: false);
        }

        /// <summary>The rollback path: arms <see cref="TransactionFailureSwitch.FailAfterFirstWrite"/> first, then runs the same method — the status write lands, the comment write never happens, and the whole unit is rolled back.</summary>
        private async void buttonCloseWithCommentFail_Click(object sender, EventArgs e)
        {
            await CloseWithCommentAsync("buttonCloseWithCommentFail_Click", armFailure: true);
        }

        private async Task CloseWithCommentAsync(string origin, bool armFailure)
        {
            var selected = this.ticketsDataGridView.CurrentRow?.DataBoundItem as TicketListItem;
            if (selected == null)
            {
                AddTrace(Glyph.Server, "transaction", "no row selected in the grid — pick a ticket first");
                return;
            }

            if (armFailure && TransactionFailure != null)
            {
                TransactionFailure.FailAfterFirstWrite = true;
                AddTrace(Glyph.Server, "transaction", "TransactionFailureSwitch.FailAfterFirstWrite = true — the next CloseTicketWithCommentAsync throws right after the status write");
            }

            await RunAsync(
                origin,
                $"TicketCommandService.CloseTicketWithCommentAsync(#{selected.Id} / {selected.Number}, \"Closed by the transaction demo\")",
                armFailure ? "Closing with a comment (armed to fail after the first write)…" : "Closing with a comment (transaction)…",
                async () =>
                {
                    var result = await Commands.CloseTicketWithCommentAsync(selected.Id, "Closed by the transaction demo — status and comment written together.");
                    return $"{result.Number} closed with comment #{result.CommentId} — both writes committed together";
                },
                "The ticket could not be closed because the database rejected the change.",
                "The ticket could not be closed. Please try again in a moment.");
        }

        /// <summary>Reads what <c>Startup.cs</c> decided once, at host start, and renders it — never guessed by the page.</summary>
        private void RefreshEnvironmentPanel()
        {
            if (Diagnostics == null)
            {
                this.labelEnvironment.Text = "StartupDiagnostics not injected — the IServiceProvider bridge is missing.";
                return;
            }

            string Row(string label, string value) =>
                "<tr><td style=\"padding:2px 14px 2px 0;color:#1f2d3d;font-weight:600;white-space:nowrap\">" + label + "</td><td style=\"padding:2px 0\">" + value + "</td></tr>";

            var migrations = Diagnostics.AppliedMigrations.Count == 0 ? "none" : string.Join(", ", Diagnostics.AppliedMigrations);

            this.labelEnvironment.Text =
                "<table style=\"border-collapse:collapse;font-family:monospace;font-size:12px;color:#3c4858\">" +
                Row("environment", Diagnostics.EnvironmentName) +
                Row("migrations at startup", Diagnostics.MigrationsAppliedAtStartup ? "yes — Development only, MigrateAsync ran from this process" : "no — applied by the release pipeline instead, see docs/DeploymentNotes.md") +
                Row("sensitive-data logging", Diagnostics.SensitiveDataLoggingOn ? "<span style=\"color:#b23b27\">ON (Development)</span>" : "off") +
                Row("applied migrations", migrations) +
                Row("script artifact", Diagnostics.MigrationScriptPath) +
                "</table>";
        }

        #endregion

        #region Module 5 · validation lab scenarios: one click, a pre-filled editor, Save shows the result

        private async void btnLabEmptyTitle_Click(object sender, EventArgs e)
        {
            await OpenEditorAsync(null, "Add: empty title", EditorLabScenario.EmptyTitle);
        }

        private async void btnLabTitleTooLong_Click(object sender, EventArgs e)
        {
            await OpenEditorAsync(null, "Add: title 200 chars", EditorLabScenario.TitleTooLong);
        }

        private async void btnLabNoCustomerCategory_Click(object sender, EventArgs e)
        {
            await OpenEditorAsync(null, "Add: no customer/category", EditorLabScenario.NoCustomerCategory);
        }

        /// <summary>
        /// Finds an existing Closed ticket (Module 2's seed always has some) and opens the editor on it with
        /// the ClosedFutureDueDate scenario — TicketEditorForm sets DueDate to next week in the model once
        /// it loads. Nothing is written by this search; it is the same TicketQueryService.SearchTicketsAsync
        /// the browser card uses, filtered to one row.
        /// </summary>
        private async void btnLabClosedFutureDue_Click(object sender, EventArgs e)
        {
            if (_loading)
            {
                AddTrace(Glyph.Server, "guard", "Edit: closed ticket, due date next week: an operation is already running — this click is ignored");
                return;
            }

            if (TicketQueries == null)
                return;

            var closed = await TicketQueries.SearchTicketsAsync(new TicketSearchCriteria { Status = TicketStatuses.Closed, PageIndex = 0, PageSize = 1 });
            if (closed.Items.Count == 0)
            {
                AddTrace(Glyph.Server, "lab scenario", "no Closed ticket found — Seed development data first");
                return;
            }

            var ticket = closed.Items[0];
            await OpenEditorAsync(ticket.Id, $"Edit: closed ticket, due date next week ({ticket.Number})", EditorLabScenario.ClosedFutureDueDate);
        }

        private async void btnLabDuplicateNumber_Click(object sender, EventArgs e)
        {
            await OpenEditorAsync(null, "Add: duplicate number (DbUpdateException)", EditorLabScenario.DuplicateNumber);
        }

        #endregion

        #region Module 3 · anti-pattern: the query bound to the grid

        private async void buttonBindQuery_Click(object sender, EventArgs e)
        {
            _handlerRuns++;

            if (_loading)
            {
                AddTrace(Glyph.Server, "guard", "buttonBindQuery_Click: an operation is already running — this click is ignored");
                UpdateLifetimes();
                return;
            }

            AddTrace(Glyph.Server, "anti-pattern", "ticketBindingSource.DataSource = db.Tickets.Select(...) — the query instead of the list; the context is then disposed and the grid enumerates");
            using var trace = QueryTrace.Begin(OnTrace);
            try
            {
                _loading = true;
                SetBusy(true);
                HideBanner();

                var text = await BoundQueryAntiPattern.BindTheQueryAndLetTheGridEnumerateAsync();

                statusLabel.Text = text;                                  // not expected
                SetState("● anti-pattern: no failure", StateKind.Warn);
            }
            catch (Exception ex) when (ex is ObjectDisposedException || ex is InvalidOperationException)
            {
                AddTrace(Glyph.Server, "caught", $"{ex.GetType().Name}: {FirstSentence(ex.Message)}");
                AddTrace(Glyph.Server, "anti-pattern", "in a real page this exception is thrown while the grid paints its first row — inside the UI layer, with no data code in the stack. The grid shows nothing and the session looks broken");
                AddTrace(Glyph.Server, "anti-pattern", "the fix is one call: await …ToListAsync() before the assignment, so the BindingSource holds rows and not a recipe");
                ShowBanner("The grid was given the query instead of the rows: by the time it enumerated, the DbContext was gone. Execute with ToListAsync and bind the list.");
                SetState("● anti-pattern caught", StateKind.Warn);
                statusLabel.Text = "Not completed — the bound query outlived its context";
            }
            catch (Exception ex)
            {
                Fail("The demo could not run.", ex);
            }
            finally
            {
                _loading = false;
                SetBusy(false);
                await RefreshModelCardAsync();
                UpdateLifetimes();
                Application.Update(this);
            }
        }

        #endregion

        #region Module 1 · success path: the lab handler

        private async void countButton_Click(object sender, EventArgs e)
        {
            await CountAsync(TimeSpan.Zero, "countButton_Click");
        }

        private Task CountAsync(TimeSpan latency, string origin)
        {
            var slow = latency > TimeSpan.Zero;
            return RunAsync(
                origin,
                slow ? $"TicketQueryService.CountTicketsSlowlyAsync({latency.TotalSeconds:0.#} s)" : "TicketQueryService.CountTicketsAsync()",
                slow ? $"Counting tickets… (simulated {latency.TotalSeconds:0.#} s latency)" : "Counting tickets…",
                async () =>
                {
                    var count = slow ? await TicketQueries.CountTicketsSlowlyAsync(latency) : await TicketQueries.CountTicketsAsync();
                    return $"{count} tickets in the Support Desk database";
                },
                "The ticket count is not available right now. Please try again in a moment.",
                "The ticket count is not available right now. Please try again in a moment.");
        }

        #endregion

        #region Module 2 · seed (the lab handler) and reset

        private async void btnSeed_Click(object sender, EventArgs e)
        {
            await RunAsync(
                "btnSeed_Click",
                "DevelopmentSeeder.SeedDevelopmentDataAsync() — one context: AnyAsync, then AddRange + SaveChangesAsync or nothing",
                "Seeding development data…",
                async () =>
                {
                    var seed = await Seeder.SeedDevelopmentDataAsync();
                    if (!seed.Seeded)
                        SetState("● nothing to seed", StateKind.Warn);
                    return seed.Describe();
                },
                "The development data could not be saved because the database rejected the change.",
                "The development data could not be seeded. Please try again in a moment.");
        }

        private async void buttonReset_Click(object sender, EventArgs e)
        {
            await RunAsync(
                "buttonReset_Click",
                "DevelopmentSeeder.ResetDevelopmentDataAsync() — ExecuteDelete on every table (children first), then the seeder (lab prop)",
                "Resetting development data…",
                async () => (await Seeder.ResetDevelopmentDataAsync()).Describe(),
                "The development data could not be reset because the database rejected the change.",
                "The development data could not be reset. Please try again in a moment.");
        }

        #endregion

        #region Module 2 · failure paths: what the schema refuses

        private async void buttonOverlongTitle_Click(object sender, EventArgs e)
        {
            await RunAsync(
                "buttonOverlongTitle_Click",
                "ModelDemoService.SaveOverlongTitleAsync() — a 200-character title against HasMaxLength(180) + CK_Tickets_Title_Length",
                "Saving a ticket with a 200-character title…",
                async () =>
                {
                    await ModelDemos.SaveOverlongTitleAsync();
                    return "The 200-character title was accepted — the CHECK constraint is missing";   // not expected
                },
                "The ticket could not be saved because the database rejected the change.",
                "The ticket could not be saved. Please try again in a moment.");
        }

        private async void buttonDeleteCustomer_Click(object sender, EventArgs e)
        {
            await RunAsync(
                "buttonDeleteCustomer_Click",
                "ModelDemoService.DeleteCustomerWithTicketsAsync() — Remove a customer that has tickets (DeleteBehavior.Restrict)",
                "Deleting a customer that still has tickets…",
                async () =>
                {
                    await ModelDemos.DeleteCustomerWithTicketsAsync();
                    return "The customer was deleted — the Restrict rule is missing";   // not expected
                },
                "This customer still has tickets and cannot be deleted.",
                "The customer could not be deleted. Please try again in a moment.");
        }

        #endregion

        #region Module 2 · delete behaviours the database carries out

        private async void buttonDeleteAgent_Click(object sender, EventArgs e)
        {
            await RunAsync(
                "buttonDeleteAgent_Click",
                "ModelDemoService.UnassignAgentByDeletingAsync() — delete an agent that owns tickets (DeleteBehavior.SetNull)",
                "Deleting an agent that owns tickets…",
                async () =>
                {
                    var r = await ModelDemos.UnassignAgentByDeletingAsync();
                    if (r.Before == r.After) SetState("● nothing to do", StateKind.Warn);
                    return r.Summary;
                },
                "The agent could not be deleted because the database rejected the change.",
                "The agent could not be deleted. Please try again in a moment.");
        }

        private async void buttonDeleteTicket_Click(object sender, EventArgs e)
        {
            await RunAsync(
                "buttonDeleteTicket_Click",
                "ModelDemoService.DeleteTicketWithCommentsAsync() — delete a ticket that has comments (DeleteBehavior.Cascade)",
                "Deleting a ticket that has comments…",
                async () =>
                {
                    var r = await ModelDemos.DeleteTicketWithCommentsAsync();
                    if (r.Before == r.After) SetState("● nothing to do", StateKind.Warn);
                    return r.Summary;
                },
                "The ticket could not be deleted because the database rejected the change.",
                "The ticket could not be deleted. Please try again in a moment.");
        }

        #endregion

        #region Module 1 · progress path: the loading guard

        private async void buttonSlowCount_Click(object sender, EventArgs e)
        {
            await CountAsync(TimeSpan.FromSeconds(2.5), "buttonSlowCount_Click");
        }

        private async void buttonRapid_Click(object sender, EventArgs e)
        {
            // Three counts requested in one go: the first one runs, the guard drops the other two.
            var first = CountAsync(TimeSpan.FromSeconds(2.5), "rapid click 1");
            await CountAsync(TimeSpan.Zero, "rapid click 2");
            await CountAsync(TimeSpan.Zero, "rapid click 3");
            await first;
        }

        #endregion

        #region Module 3 · failure path and recovery: the database goes away

        private async void buttonBreak_Click(object sender, EventArgs e)
        {
            Outage.IsDown = true;
            AddTrace(Glyph.Server, "outage", "DevelopmentOutageSwitch.IsDown = true — every connection open now fails (lab prop, development only)");
            await LoadTicketsAsync("buttonBreak_Click");
        }

        private async void buttonRestore_Click(object sender, EventArgs e)
        {
            Outage.IsDown = false;
            AddTrace(Glyph.Server, "outage", "DevelopmentOutageSwitch.IsDown = false — the next search gets a fresh context and a working connection");
            await LoadTicketsAsync("buttonRestore_Click");
        }

        #endregion

        #region Module 1 · anti-pattern: what a shared DbContext does

        private async void buttonAntiPattern_Click(object sender, EventArgs e)
        {
            AddTrace(Glyph.Server, "anti-pattern", "one context, two concurrent CountAsync calls — what a static/shared DbContext does when two sessions use it");
            using var trace = QueryTrace.Begin(OnTrace);
            try
            {
                var rounds = await AntiPattern.RunTwoOperationsOnOneContextAsync();
                AddTrace(Glyph.Server, "anti-pattern", $"no collision in {rounds} rounds: each pair happened to finish before the other started — the race is timing-dependent, which is exactly why it is unsafe");
                SetState("● anti-pattern: race not hit", StateKind.Warn);
            }
            catch (InvalidOperationException ex)
            {
                AddTrace(Glyph.Server, "caught", $"InvalidOperationException: {FirstSentence(ex.Message)} (round {AntiPattern.LastRound})");
                ShowBanner("EF Core refused a second operation on the same DbContext. A static or shared context runs every user into this.");
                SetState("● anti-pattern caught", StateKind.Warn);
            }
            catch (Exception ex)
            {
                Fail("The demo could not run.", ex);
            }
            finally
            {
                UpdateLifetimes();
                Application.Update(this);
            }
        }

        #endregion

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.listTrace.Items.Clear();
        }

        #region The Model & migration card

        /// <summary>
        /// Re-reads the row counts, the migration history and the model metadata after every operation.
        /// It is a database operation of its own (one context, nine statements), so it runs in its own trace
        /// scope and reports a single summary line instead of nine SQL lines — otherwise the card would bury
        /// the operation the learner just clicked.
        /// </summary>
        private async Task RefreshModelCardAsync()
        {
            if (SchemaInfo == null)
                return;

            using var scope = QueryTrace.Begin(_ => { });
            try
            {
                var info = await SchemaInfo.DescribeAsync();
                _contextsCreated += scope.ContextsCreated;
                _contextsDisposed += scope.ContextsDisposed;
                this.labelModel.Text = RenderModelCard(info);
                AddTrace(Glyph.Context, "card", $"Model & migration card refreshed · SchemaInfoService.DescribeAsync(): {scope.Commands} statements ({scope.Milliseconds:0.0} ms), {scope.ContextsCreated} context created, {scope.ContextsDisposed} disposed");
            }
            catch (Exception ex)
            {
                _contextsCreated += scope.ContextsCreated;
                _contextsDisposed += scope.ContextsDisposed;
                AddTrace(Glyph.Context, "card", $"Model & migration card not refreshed: {ex.GetType().Name} — {FirstSentence(ex.Message)}");
            }
        }

        private static string RenderModelCard(SchemaInfo info)
        {
            string Row(string label, string html) =>
                "<tr><td style=\"padding:3px 12px 3px 0;color:#1f2d3d;font-weight:600;vertical-align:top;white-space:nowrap\">" + label + "</td>" +
                "<td style=\"padding:3px 0;vertical-align:top\">" + html + "</td></tr>";

            var c = info.Counts;
            var rows = $"Customers <b>{c.Customers}</b> · Agents <b>{c.Agents}</b> · Categories <b>{c.Categories}</b> · Tickets <b>{c.Tickets}</b> · TicketComments <b>{c.TicketComments}</b>";

            var migrations = info.AppliedMigrations.Count == 0
                ? "<span style=\"color:#b23b27\">none applied</span>"
                : $"applied {info.AppliedMigrations.Count}: {string.Join(", ", info.AppliedMigrations)}";
            migrations += info.PendingMigrations.Count == 0
                ? " · pending 0"
                : $" · <span style=\"color:#b23b27\">pending {info.PendingMigrations.Count}: {string.Join(", ", info.PendingMigrations)}</span>";

            var indexes = new StringBuilder();
            foreach (var group in info.Indexes.GroupBy(i => i.Table).OrderBy(g => g.Key))
            {
                if (indexes.Length > 0) indexes.Append("<br>");
                indexes.Append(group.Key).Append(": ");
                indexes.Append(string.Join(" · ", group.Select(i => $"{i.Name} ({string.Join(", ", i.Columns)}){(i.IsUnique ? " <b>UNIQUE</b>" : "")}")));
            }

            var relationships = string.Join("<br>", info.Relationships
                .OrderBy(r => r.DependentTable).ThenBy(r => r.ForeignKeyColumn)
                .Select(r => $"{r.DependentTable}.{r.ForeignKeyColumn} → {r.PrincipalTable} <b>{DeleteBehaviourSql(r.DeleteBehavior)}</b>"));

            var checks = info.CheckConstraints.Count == 0
                ? "none"
                : string.Join("<br>", info.CheckConstraints.Select(k => $"{k.Table}: {k.Name} = <code>{System.Net.WebUtility.HtmlEncode(k.Sql)}</code>"));

            return "<table style=\"border-collapse:collapse;font-family:monospace;font-size:12px;color:#3c4858\">" +
                   Row("rows", rows) +
                   Row("migrations", migrations) +
                   Row("indexes", indexes.ToString()) +
                   Row("on delete", relationships) +
                   Row("checks", checks) +
                   "</table>";
        }

        private static string DeleteBehaviourSql(DeleteBehavior behavior)
        {
            switch (behavior)
            {
                case DeleteBehavior.Cascade:
                case DeleteBehavior.ClientCascade:
                    return "ON DELETE CASCADE";
                case DeleteBehavior.SetNull:
                case DeleteBehavior.ClientSetNull:
                    return "ON DELETE SET NULL";
                case DeleteBehavior.Restrict:
                    return "ON DELETE RESTRICT";
                case DeleteBehavior.NoAction:
                case DeleteBehavior.ClientNoAction:
                    return "ON DELETE NO ACTION";
                default:
                    return behavior.ToString();
            }
        }

        #endregion

        #region Helpers

        private void Fail(string friendlyMessage, Exception ex)
        {
            statusLabel.Text = "Not completed — nothing was changed";
            ShowBanner(friendlyMessage);
            AlertBox.Show(friendlyMessage, MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            var detail = ex.InnerException != null
                ? $"{ex.GetType().Name} → {ex.InnerException.GetType().Name}: {FirstSentence(ex.InnerException.Message)}"
                : $"{ex.GetType().Name}: {FirstSentence(ex.Message)}";
            AddTrace(Glyph.Server, "caught", $"{detail} → friendly message shown, full exception logged server-side");
            SetState("● fault", StateKind.Error);
        }

        private void SetBusy(bool busy)
        {
            this.searchButton.Enabled = !busy;
            this.prevPageButton.Enabled = !busy;
            this.nextPageButton.Enabled = !busy;
            this.btnAdd.Enabled = !busy;
            this.btnEdit.Enabled = !busy;
            this.buttonSimulateDelete.Enabled = !busy;
            this.btnLabEmptyTitle.Enabled = !busy;
            this.btnLabTitleTooLong.Enabled = !busy;
            this.btnLabNoCustomerCategory.Enabled = !busy;
            this.btnLabClosedFutureDue.Enabled = !busy;
            this.btnLabDuplicateNumber.Enabled = !busy;
            this.buttonSlowSearch.Enabled = !busy;
            this.buttonBindQuery.Enabled = !busy;
            this.countButton.Enabled = !busy;
            this.btnSeed.Enabled = !busy;
            this.buttonOverlongTitle.Enabled = !busy;
            this.buttonDeleteCustomer.Enabled = !busy;
            this.buttonDeleteAgent.Enabled = !busy;
            this.buttonDeleteTicket.Enabled = !busy;
            this.buttonReset.Enabled = !busy;
            this.buttonSlowCount.Enabled = !busy;
            this.buttonRapid.Enabled = !busy;
            this.buttonBreak.Enabled = !busy;
            this.buttonRestore.Enabled = !busy;
            this.buttonSimulateChange.Enabled = !busy;
            this.buttonCloseWithComment.Enabled = !busy;
            this.buttonCloseWithCommentFail.Enabled = !busy;

            // Previous on page 1 and Next on the last page stay off even when nothing is running.
            if (!busy)
                UpdatePagingButtons();
        }

        /// <summary>Receives the EF Core trace for the current operation (see QueryTrace).</summary>
        private void OnTrace(TraceEntry entry)
        {
            switch (entry.Kind)
            {
                case TraceKind.Context:
                    if (entry.Text.Contains("created")) _contextsCreated++;
                    if (entry.Text.Contains("disposed")) _contextsDisposed++;
                    AddTrace(Glyph.Context, "context", entry.Text);
                    break;
                case TraceKind.Command:
                    AddTrace(Glyph.Sql, "SQL", $"{entry.Text}   ({entry.Milliseconds:0.0} ms)");
                    break;
                case TraceKind.Failure:
                    AddTrace(Glyph.Sql, "SQL failed", entry.Text);
                    break;
                case TraceKind.Note:
                    AddTrace(Glyph.Server, "service", entry.Text);
                    break;
            }
        }

        private enum Glyph { Server, Context, Sql, Result }

        private void AddTrace(Glyph glyph, string label, string text)
        {
            var symbol = glyph switch
            {
                Glyph.Server => "•",
                Glyph.Context => "◦",
                Glyph.Sql => "→",
                _ => "←"
            };

            AddTraceRaw(symbol[0], label, text);
        }

        /// <summary>
        /// Module 4: the raw form of <see cref="AddTrace"/>, taking a literal glyph instead of the page's
        /// own <see cref="Glyph"/> enum, so <see cref="TicketEditorForm"/> can choose "◦ context …",
        /// "→ SQL …" and "• editor …" lines for itself — the same vocabulary the page uses, from a dialog
        /// that cannot see the page's private enum. Passed to the dialog's constructor as the trace callback.
        /// </summary>
        private void AddTraceRaw(char symbol, string label, string text)
        {
            lock (_traceLock)
            {
                if (this.listTrace.Items.Count >= 400)
                    this.listTrace.Items.RemoveAt(0);
                this.listTrace.Items.Add($"{DateTime.Now:HH:mm:ss.fff}  {symbol} {label,-12} {text}");
                this.listTrace.SelectedIndex = this.listTrace.Items.Count - 1;
            }
        }

        private void UpdateLifetimes()
        {
            string Row(string lifetime, string owner, string now) =>
                "<tr><td style=\"padding:2px 14px 2px 0;color:#1f2d3d;font-weight:600\">" + lifetime + "</td>" +
                "<td style=\"padding:2px 14px 2px 0\">" + owner + "</td><td style=\"padding:2px 0\">" + now + "</td></tr>";

            var bound = this.ticketBindingSource.DataSource is System.Collections.ICollection list ? list.Count : 0;

            this.labelLifetimes.Text =
                "<table style=\"border-collapse:collapse;font-family:monospace;font-size:12px;color:#3c4858\">" +
                Row("session state", "Wisej.NET", "id " + ShortSessionId() + " · started " + _createdAt.ToString("HH:mm:ss")) +
                Row("UI object", "this Page", "1 page, the grid, the trace list and a BindingSource holding " + bound + " TicketListItem row(s)") +
                Row("request / thread", "one click", _handlerRuns + " handler run(s) · continuations may resume on any thread") +
                Row("unit of work", "DbContext", _contextsCreated + " created · " + _contextsDisposed + " disposed · " + (_contextsCreated - _contextsDisposed) + " alive between clicks") +
                "</table>";
        }

        private enum StateKind { Normal, Ok, Warn, Error }

        private void SetState(string text, StateKind kind)
        {
            this.labelState.Text = text;
            this.labelState.ForeColor = kind switch
            {
                StateKind.Ok => System.Drawing.Color.FromArgb(31, 157, 87),
                StateKind.Warn => System.Drawing.Color.FromArgb(232, 161, 60),
                StateKind.Error => System.Drawing.Color.FromArgb(224, 86, 59),
                _ => System.Drawing.Color.FromArgb(90, 107, 125)
            };
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

        private static string ShortSessionId()
        {
            var id = Application.SessionId ?? "";
            return id.Length > 8 ? id.Substring(0, 8) : id;
        }

        private static string FirstSentence(string message)
        {
            var i = message.IndexOf(". ", StringComparison.Ordinal);
            return i > 0 ? message.Substring(0, i + 1) : message;
        }

        #endregion
    }
}
