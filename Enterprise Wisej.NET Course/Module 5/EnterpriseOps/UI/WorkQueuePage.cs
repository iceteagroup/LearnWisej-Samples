using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Data;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;
using EnterpriseOps.Services;
using EnterpriseOps.Services.WorkQueues;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// EnterpriseOps — Enterprise Work Queue (Advanced Module 5).
    ///
    /// Left card:   the filter bar (search · status · assigned · sort), the saved views, <c>dgvQueue</c> bound to
    ///              <b>one page</b> of <see cref="WorkQueueRow"/>, the pager, the batch bar, the progress line and
    ///              the dark footer ("50 of 4,331 matching · sorted by Priority ↓ · 38 ms server · 12.4 KB page").
    /// Right card:  the live activity trace — every layer's decision, tagged UI → / Session: / Security: /
    ///              Service: / Data: / Job:.
    /// Bottom bar:  the anti-pattern the video measures (load everything), the failure paths (approval lock,
    ///              concurrent edit, run as a Technician), the recovery (retry the failed rows only) and Clear trace.
    ///
    /// The boundary: this file owns UI state only — what is on screen and what colour it is. Every decision is a
    /// service's: <see cref="WorkQueueQueryService"/> filters, sorts, pages and projects;
    /// <see cref="BatchReassignWorkflow"/> validates and commits each row and returns the per-row report;
    /// <see cref="PermissionService"/> decides who may reassign. The grid state (filters, sort, page, selection)
    /// lives in <see cref="SessionContext"/>, not in this page, so "Simulate refresh" can throw the page away.
    /// </summary>
    public partial class WorkQueuePage : Page
    {
        // Per-session services, wired in the constructor. Instance fields — never statics: two users must never
        // share a session, a selection or a trace. The store and the audit log stand in for shared tables.
        private readonly SessionContext _session;
        private readonly ActivityTrace _trace;
        private readonly WorkOrderStore _store;
        private readonly PermissionService _permissions;
        private readonly WorkQueueQueryService _queryService;
        private readonly BatchReassignWorkflow _batchWorkflow;
        private readonly LoadEverythingAntiPattern _antiPattern;

        private CommandContext _current;

        /// <summary>Kept in an instance field so the batch button can cancel the run it started.</summary>
        private CancellationTokenSource _batchCancellation;

        private string _batchTarget = "";
        private BatchResult _lastResult;
        private List<WorkQueueRow> _pageRows = new List<WorkQueueRow>();
        private bool _syncingSelection;
        private int _lockedOrderId;
        private double _lastPageKb;

        /// <summary>Grid column order → the sort key the service understands (null = not sortable).</summary>
        private static readonly string[] SortKeys =
        {
            "Number", "Title", "Status", "Priority", "AssignedTo", "DueAt", "AgeDays", null,
        };

        public WorkQueuePage()
        {
            InitializeComponent();

            _session = SessionContext.Current;
            _trace = _session.Trace;
            _store = WorkOrderStore.Instance;
            _permissions = new PermissionService();
            _queryService = new WorkQueueQueryService(_store, _permissions, _session);
            _batchWorkflow = new BatchReassignWorkflow(_store, _permissions, AuditTrail.Instance, _trace);
            _antiPattern = new LoadEverythingAntiPattern(_store, _queryService, _trace);

            FillFilterChoices();
            _trace.LineAdded += trace_LineAdded;
        }

        /// <summary>The grid state the session owns — not a field of this page, on purpose.</summary>
        private GridState Grid => _session.WorkQueueGrid;

        /// <summary>The command running right now: same tenant and user, one correlation id per action.</summary>
        private CommandContext CurrentContext => _current ?? NewCommand();

        #region Event handlers — thin, one service call each (the shape the lab code check expects)

        private async void WorkQueuePage_Load(object sender, EventArgs e)
        {
            ReplayTrace();
            ShowSignedIn();
            WriteQueryToControls(Grid.Query);
            _trace.Write($"UI → WorkQueuePage_Load: restoring grid state from the session — {Grid.Describe()}");
            try
            {
                await RunQueryAsync(Grid.Query, "Loading the first page…");
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
        }

        /// <summary>The walkthrough's Search: the screen fills a WorkQueueQuery, the service returns one page.</summary>
        private async void btnSearch_Click(object sender, EventArgs e)
        {
            _trace.Write("UI → btnSearch_Click: filters read into the grid state, page reset to 1");
            try
            {
                await RunQueryAsync(ReadFiltersIntoGridState(1), "Searching…");
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnSearch_Click(sender, e);
        }

        private async void btnFirst_Click(object sender, EventArgs e) => await GoToPageAsync(1);

        private async void btnPrev_Click(object sender, EventArgs e) => await GoToPageAsync(Grid.Query.Page - 1);

        private async void btnNext_Click(object sender, EventArgs e) => await GoToPageAsync(Grid.Query.Page + 1);

        private async void btnLast_Click(object sender, EventArgs e) => await GoToPageAsync(Grid.LastPageCount);

        /// <summary>Sort persistence: the header does not sort the grid, it changes the query the server runs.</summary>
        private async void dgvQueue_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string key = SortKeyFor(e.ColumnIndex);
            if (key == null)
                return;

            bool descending = Grid.Query.SortBy == key ? !Grid.Query.Descending : DefaultDescending(key);
            _trace.Write($"UI → header '{key}' clicked → sort {(descending ? "desc" : "asc")} stored in GridState; page reset to 1");
            try
            {
                SelectByKey(cboSort, key);
                await RunQueryAsync(Grid.Query with { SortBy = key, Descending = descending, Page = 1 }, "Sorting…");
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
        }

        private void dgvQueue_SelectionChanged(object sender, EventArgs e)
        {
            if (_syncingSelection)
                return;
            SyncSelectionFromGrid();
        }

        private void btnSelectPage_Click(object sender, EventArgs e)
        {
            _syncingSelection = true;
            foreach (DataGridViewRow row in dgvQueue.Rows)
                row.Selected = true;
            _syncingSelection = false;
            SyncSelectionFromGrid();
            _trace.Write($"UI → btnSelectPage_Click: page added to the server-side selection ({Grid.Selected.Count} total)");
        }

        private void btnClearSelection_Click(object sender, EventArgs e)
        {
            Grid.Selected.Clear();
            _syncingSelection = true;
            dgvQueue.ClearSelection();
            _syncingSelection = false;
            UpdateActionUi();
            _trace.Write("UI → btnClearSelection_Click: selection cleared in GridState");
        }

        /// <summary>Applying a saved view runs the stored query again — the rows are always current.</summary>
        private async void btnApplyView_Click(object sender, EventArgs e)
        {
            var view = cboSavedView.SelectedItem as SavedView;
            if (view == null)
                return;

            _trace.Write($"Service: saved view \"{view.Name}\" = {SavedViewStore.Serialize(view.Definition)} — a stored query, not a cached result");
            try
            {
                Grid.SavedViewName = view.Name;
                WriteQueryToControls(view.Definition);
                await RunQueryAsync(view.Definition, $"Applying \"{view.Name}\"…");
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
        }

        private void btnSaveView_Click(object sender, EventArgs e)
        {
            var definition = ReadFiltersIntoGridState(1);
            var view = _session.SavedViews.Add(SavedViewStore.SuggestName(definition), _session.TenantId, _session.UserName, definition);
            Grid.SavedViewName = view.Name;

            _trace.Write($"Service: saved view #{view.Id} \"{view.Name}\" stored for {view.Owner}@{view.TenantId} → {SavedViewStore.Serialize(view.Definition)}");
            FillSavedViews(view);
            UpdateViewBadge();
            AlertBox.Show($"Saved view “{view.Name}”.", MessageBoxIcon.Information,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        /// <summary>
        /// Review question 3: can the user repeat the search after a refresh? This throws the page away and builds
        /// a new one. Nothing about the query lives here, so the new page finds the same filters, sort, page and
        /// selection in <see cref="SessionContext"/>.
        /// </summary>
        private void btnSimulateRefresh_Click(object sender, EventArgs e)
        {
            _trace.Write($"UI → btnSimulateRefresh_Click: disposing the page; the session keeps {Grid.Describe()}");
            DetachTrace();
            Application.MainPage = new WorkQueuePage();
        }

        /// <summary>The batch command. While it runs the same button cancels it.</summary>
        private async void btnBatchReassign_Click(object sender, EventArgs e)
        {
            if (_batchCancellation != null)
            {
                _trace.Write("UI → cancel requested; the rows already committed stay committed");
                _batchCancellation.Cancel();
                return;
            }

            try
            {
                var items = Grid.Selected.Values
                    .OrderBy(r => r.Id)
                    .Select(r => new BatchItem(r.Id, r.Number, r.Version))
                    .ToList();
                string target = KeyOf(cboTechnician);

                var answer = await MessageBox.ShowAsync(
                    $"Reassign {items.Count} work order(s) to {target}?\r\n\r\nEach row is validated and committed on its own; you will get a per-row report.",
                    "Batch reassignment", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (answer != DialogResult.Yes)
                {
                    _trace.Write("UI → batch abandoned at the confirmation");
                    return;
                }

                await RunBatchLoopAsync(items, target, isRetry: false);
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
        }

        /// <summary>The recovery: retry the failed rows only, with freshly read versions, never the successes.</summary>
        private async void btnRetryFailed_Click(object sender, EventArgs e)
        {
            try
            {
                if (_lastResult == null || _lastResult.Failed == 0)
                {
                    AlertBox.Show("There are no failed rows to retry.", MessageBoxIcon.Information,
                        alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                    return;
                }

                await RunBatchLoopAsync(BuildRetryItems(_lastResult), _lastResult.TargetTechnician, isRetry: true);
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
        }

        /// <summary>The anti-pattern the video shows, measured: every tenant row materialized, projected and bound.</summary>
        private async void btnLoadEverything_Click(object sender, EventArgs e)
        {
            _trace.Write("UI → btnLoadEverything_Click: grid.DataSource = db.WorkOrders.ToList() — measured, not guessed");
            BeginBusy("Loading everything…");
            try
            {
                var measurement = await _antiPattern.LoadEverythingAsync(_session.TenantId);
                ShowAntiPattern(measurement);
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

        /// <summary>Failure path 1 / recovery 1: another user opens an approval on a selected row, then completes it.</summary>
        private async void btnApprovalLock_Click(object sender, EventArgs e)
        {
            try
            {
                if (_lockedOrderId != 0)
                {
                    _store.CompleteApproval(_lockedOrderId);
                    _trace.Write($"Data: approval on WO-{_lockedOrderId} completed by another user — the row can be reassigned again");
                    _lockedOrderId = 0;
                    btnApprovalLock.Text = "Fail: approval lock on a row";
                    await RunQueryAsync(Grid.Query, "Reloading the page…");
                    return;
                }

                var row = LastSelectedRow();
                if (row == null)
                {
                    AlertBox.Show("Select at least one row first.", MessageBoxIcon.Warning,
                        alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                    return;
                }

                _store.OpenApproval(row.Id, "APR-1042");
                _lockedOrderId = row.Id;
                btnApprovalLock.Text = "Recover: complete the approval";
                _trace.Write($"Data: another user opened approval APR-1042 on {row.Number} — the next batch will fail that row only");
                ShowBanner($"{row.Number} is now locked by approval APR-1042. Run the batch: it fails that row and changes the others.", BannerKind.Warning);
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
        }

        /// <summary>
        /// Failure path 2: another session writes to a selected row. The page is deliberately NOT reloaded, so the
        /// selection snapshot keeps the version the user saw and the batch fails that row with stale-version.
        /// </summary>
        private void btnConcurrentEdit_Click(object sender, EventArgs e)
        {
            var row = LastSelectedRow();
            if (row == null)
            {
                AlertBox.Show("Select at least one row first.", MessageBoxIcon.Warning,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return;
            }

            _store.TouchByAnotherUser(row.Id);
            var order = _store.Find(row.Id);
            _trace.Write($"Data: another session wrote {row.Number} — Version {row.Version} → {order.Version}; the grid still shows v{row.Version}");
            ShowBanner($"{row.Number} was changed by another user (v{row.Version} → v{order.Version}). The batch will refuse it instead of overwriting.", BannerKind.Warning);
        }

        /// <summary>Failure path 3 / recovery 3: the permission flags are recomputed by the server, per row.</summary>
        private async void btnSwitchUser_Click(object sender, EventArgs e)
        {
            string next = _session.UserName == "ana.ops" ? "ben.tech" : "ana.ops";
            try
            {
                _session.SwitchUser(next);
                ShowSignedIn();
                await RunQueryAsync(Grid.Query, $"Re-running as {next}…");
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
        }

        private void btnClearTrace_Click(object sender, EventArgs e)
        {
            _trace.Clear();
            lstTrace.Items.Clear();
        }

        #endregion

        #region Running the services (await here, decisions there)

        /// <summary>One page in, one page out. The only place that calls the query service.</summary>
        private async Task RunQueryAsync(WorkQueueQuery query, string busyText)
        {
            BeginBusy(busyText);
            try
            {
                var result = await _queryService.SearchAsync(query);
                Grid.Query = query;
                ShowPage(result);
            }
            finally
            {
                EndBusy();
            }
        }

        private async Task GoToPageAsync(int page)
        {
            int target = Math.Clamp(page, 1, Math.Max(1, Grid.LastPageCount));
            if (target == Grid.Query.Page)
                return;

            _trace.Write($"UI → pager: page {Grid.Query.Page} → {target}; one more SearchAsync, {Grid.Query.PageSize} rows");
            try
            {
                await RunQueryAsync(Grid.Query with { Page = target }, $"Loading page {target}…");
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
        }

        /// <summary>
        /// Runs the batch, shows the per-row report, and loops while the user asks to retry the failures. No
        /// recursion: a retry is the next turn of this loop, with freshly read versions and a new correlation id.
        /// </summary>
        private async Task RunBatchLoopAsync(IReadOnlyList<BatchItem> items, string technician, bool isRetry)
        {
            while (items != null && items.Count > 0)
            {
                var result = await RunBatchAsync(items, technician, isRetry);
                if (result == null)
                    return;                                     // cancelled, or the command was rejected

                _lastResult = result;
                ShowBatchOutcome(result);
                await RunQueryAsync(Grid.Query, "Reloading the page…");

                DialogResult answer;
                using (var dialog = new BatchResultDialog(result))
                    answer = await dialog.ShowDialogAsync();

                if (answer != DialogResult.Retry)
                    return;

                items = BuildRetryItems(result);
                isRetry = true;
                _trace.Write($"UI → retry: {items.Count} failed row(s), versions re-read from the store");
            }
        }

        /// <summary>One run of the workflow with progress. Returns null when the user cancelled or the command was invalid.</summary>
        private async Task<BatchResult> RunBatchAsync(IReadOnlyList<BatchItem> items, string technician, bool isRetry)
        {
            BeginBusy(isRetry ? "Retrying…" : "Reassigning…");     // one fresh correlation id for this batch
            var context = CurrentContext;

            _batchTarget = technician;
            _batchCancellation = new CancellationTokenSource();

            ShowProgress(0, items.Count);
            btnBatchReassign.Enabled = true;
            btnBatchReassign.Text = "■ Cancel batch";

            try
            {
                var command = new ReassignBatchCommand(items, technician, context, isRetry);
                return await _batchWorkflow.RunAsync(command, OnBatchProgress, _batchCancellation.Token);
            }
            catch (OperationCanceledException)
            {
                _trace.Write($"Job: {context.CorrelationId} cancelled by the user — committed rows stay committed and audited");
                ShowBanner("Batch cancelled. The rows already committed were not rolled back — the audit log lists them.", BannerKind.Warning);
                SetStatus("batch cancelled", StatusKind.Warn);
                return null;
            }
            catch (ArgumentException ex)
            {
                // Command-level validation: nothing was touched.
                _trace.Write($"Service: command rejected — {ex.Message}; no row was changed");
                ShowBanner(ex.Message, BannerKind.Warning);
                SetStatus("command rejected", StatusKind.Warn);
                return null;
            }
            finally
            {
                _batchCancellation.Dispose();
                _batchCancellation = null;
                btnBatchReassign.Text = "Reassign selected…";
                HideProgress();
                EndBusy();
            }
        }

        /// <summary>Progress after every row — the workflow reports, the screen shows. Pushed over the socket.</summary>
        private void OnBatchProgress(BatchProgress progress)
        {
            if (IsDisposed)
                return;

            ShowProgress(progress.Done, progress.Total);
            Application.Update(this);
        }

        /// <summary>The failed rows only, with the version they have <b>now</b> — that is what "refresh and retry" means.</summary>
        private List<BatchItem> BuildRetryItems(BatchResult result)
        {
            var items = new List<BatchItem>();
            foreach (var row in result.FailedRows)
            {
                var order = _store.Find(row.WorkOrderId);
                if (order != null)
                    items.Add(new BatchItem(order.Id, order.Number, order.Version));
            }
            return items;
        }

        #endregion

        #region Showing results — UI state only, no decisions

        /// <summary>One page of the projection → the grid, the pager, the footer. Nothing is computed per row here.</summary>
        private void ShowPage(PagedResult<WorkQueueRow> result)
        {
            _pageRows = new List<WorkQueueRow>(result.Items);

            _syncingSelection = true;
            dgvQueue.DataSource = _pageRows;
            RestoreSelection();
            _syncingSelection = false;
            PaintOverdueRows();

            Grid.LastTotalCount = result.TotalCount;
            Grid.LastPageCount = result.PageCount;
            Grid.LastLoadedUtc = DateTime.UtcNow;
            _lastPageKb = _queryService.LastPayloadBytes / 1024.0;

            lblPage.Text = string.Format(CultureInfo.InvariantCulture, "Page {0:N0} of {1:N0}", result.Page, result.PageCount);
            lblStatusBar.Text = string.Format(CultureInfo.InvariantCulture,
                "{0} of {1:N0} matching · sorted by {2} {3} · {4} ms server · {5:0.0} KB page",
                result.Items.Count, result.TotalCount, Grid.Query.SortBy, Grid.Query.Descending ? "↓" : "↑",
                _queryService.LastElapsedMs, _lastPageKb);

            UpdateSortHeaders();
            UpdateViewBadge();
            UpdatePagerButtons(result);
            SetStatus($"{result.Items.Count} rows loaded", StatusKind.Ok);
            _trace.Write($"UI ← ShowPage: {result.Items.Count} rows bound to dgvQueue; {Grid.Selected.Count} selected across pages");
        }

        /// <summary>The measured anti-pattern: bind everything, then say what it cost next to one page.</summary>
        private void ShowAntiPattern(AntiPatternMeasurement measurement)
        {
            _syncingSelection = true;
            dgvQueue.DataSource = new List<WorkQueueRow>(measurement.Rows);
            _syncingSelection = false;

            double kb = measurement.PayloadBytes / 1024.0;
            double factor = _lastPageKb > 0 ? kb / _lastPageKb : 0;

            lblPage.Text = "no paging";
            lblStatusBar.Text = string.Format(CultureInfo.InvariantCulture,
                "{0:N0} rows bound · {1:N0} KB · materialize {2} ms + project {3} ms — one page was {4:0.0} KB",
                measurement.EntitiesMaterialized, kb, measurement.MaterializeMs, measurement.ProjectMs, _lastPageKb);
            ShowBanner(string.Format(CultureInfo.InvariantCulture,
                "Anti-pattern: {0:N0} rows loaded into the browser, the session and this grid — ≈{1:0}× the payload of one page. Click Search to go back to paging.",
                measurement.EntitiesMaterialized, factor), BannerKind.Warning);
            SetStatus("everything loaded — do not ship this", StatusKind.Warn);
        }

        /// <summary>Partial failure is a normal result: a banner that names the numbers, then the per-row report.</summary>
        private void ShowBatchOutcome(BatchResult result)
        {
            if (result.Failed == 0)
            {
                ShowBanner($"Batch {result.CorrelationId} — {result.Summary} in {result.ElapsedMs} ms · {result.Rows.Count} audit entries.", BannerKind.Success);
                SetStatus(result.Summary, StatusKind.Ok);
            }
            else
            {
                ShowBanner($"Batch {result.CorrelationId} — {result.Summary}. {result.Failed} row(s) were not changed; the report says why.", BannerKind.Warning);
                SetStatus(result.Summary, result.Succeeded == 0 ? StatusKind.Error : StatusKind.Warn);
            }

            // A row that succeeded is no longer interesting to the selection; the failures stay selected for the retry.
            foreach (var row in result.Rows.Where(r => r.Outcome == BatchRowOutcome.Succeeded))
                Grid.Selected.Remove(row.WorkOrderId);

            btnRetryFailed.Enabled = result.Failed > 0;
        }

        private void ShowProgress(int done, int total)
        {
            int percent = total == 0 ? 0 : (int)Math.Round(done * 100.0 / total);
            lblProgress.Visible = true;
            progressBatch.Visible = true;
            progressBatch.Value = Math.Clamp(percent, 0, 100);
            lblProgress.Text = string.Format(CultureInfo.InvariantCulture,
                "Reassigning to {0} — {1} of {2}…   {3}%", _batchTarget, done, total, percent);
        }

        private void HideProgress()
        {
            lblProgress.Visible = false;
            progressBatch.Visible = false;
            progressBatch.Value = 0;
        }

        /// <summary>Unexpected failure: log it with the correlation id, tell the user something generic, keep the screen usable.</summary>
        private void ReportFailure(Exception ex)
        {
            _trace.Write($"Service: unhandled {ex.GetType().Name} — {ex.Message} (ref {CurrentContext.CorrelationId})");
            ShowBanner($"The action could not be completed. Check the log for details.  (ref {CurrentContext.CorrelationId})", BannerKind.Error);
            SetStatus("failed — see the trace", StatusKind.Error);
            AlertBox.Show("The action could not be completed. Check the log for details.", MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion

        #region Selection (kept on the server, so it survives paging)

        /// <summary>The page's selected rows → the session's selection. Rows on other pages are left alone.</summary>
        private void SyncSelectionFromGrid()
        {
            for (int i = 0; i < dgvQueue.Rows.Count && i < _pageRows.Count; i++)
            {
                var row = _pageRows[i];
                if (dgvQueue.Rows[i].Selected)
                    Grid.Selected[row.Id] = row;
                else
                    Grid.Selected.Remove(row.Id);
            }
            UpdateActionUi();
        }

        /// <summary>The session's selection → this page's rows, after a page change or a refresh.</summary>
        private void RestoreSelection()
        {
            dgvQueue.ClearSelection();
            for (int i = 0; i < dgvQueue.Rows.Count && i < _pageRows.Count; i++)
            {
                if (Grid.Selected.ContainsKey(_pageRows[i].Id))
                    dgvQueue.Rows[i].Selected = true;
            }
        }

        /// <summary>The row the failure-path buttons act on: the last one the user selected.</summary>
        private WorkQueueRow LastSelectedRow() =>
            Grid.Selected.Count == 0 ? null : Grid.Selected.Values.OrderBy(r => r.Id).Last();

        private void UpdateActionUi()
        {
            int selected = Grid.Selected.Count;
            int allowed = Grid.Selected.Values.Count(r => r.CanReassign);

            lblSelection.Text = selected == 0
                ? "0 selected"
                : $"{selected} selected across pages · {allowed} the server would allow";
            btnBatchReassign.Text = selected == 0 ? "Reassign selected…" : $"Reassign {selected} selected…";
            btnBatchReassign.Enabled = selected > 0;
            btnRetryFailed.Enabled = _lastResult != null && _lastResult.Failed > 0;
        }

        #endregion

        #region Filters, saved views and the grid state

        /// <summary>Reads the filter controls into the session's grid state and returns the query to run.</summary>
        private WorkQueueQuery ReadFiltersIntoGridState(int page)
        {
            int pageSize = int.Parse(KeyOf(cboPageSize) ?? "50", CultureInfo.InvariantCulture);
            string sortBy = KeyOf(cboSort) ?? "Priority";
            bool descending = Grid.Query.SortBy == sortBy ? Grid.Query.Descending : DefaultDescending(sortBy);

            var query = Grid.Query with
            {
                TenantId = _session.TenantId,
                SearchText = txtSearch.Text,
                Status = KeyOf(cboStatus),
                AssignedTo = KeyOf(cboAssigned),
                SortBy = sortBy,
                Descending = descending,
                Page = page,
                PageSize = pageSize,
            };

            // The badge stays only while the query still is the saved view (records compare by value).
            var view = Grid.SavedViewName == null ? null : _session.SavedViews.Find(Grid.SavedViewName);
            Grid.SavedViewName = view != null && view.Definition == (query with { Page = 1 }) ? view.Name : null;

            Grid.Query = query;
            return query;
        }

        /// <summary>The grid state → the controls, after a refresh or when a saved view is applied.</summary>
        private void WriteQueryToControls(WorkQueueQuery query)
        {
            txtSearch.Text = query.SearchText ?? "";
            SelectByKey(cboStatus, query.Status);
            SelectByKey(cboAssigned, query.AssignedTo);
            SelectByKey(cboSort, query.SortBy);
            SelectByKey(cboPageSize, query.PageSize.ToString(CultureInfo.InvariantCulture));
            UpdateViewBadge();
        }

        private void FillFilterChoices()
        {
            cboStatus.Items.Add(new Choice(null, "Status: any"));
            cboStatus.Items.Add(new Choice("Open", "Status: Open"));
            foreach (WorkOrderStatus status in Enum.GetValues(typeof(WorkOrderStatus)))
                cboStatus.Items.Add(new Choice(status.ToString(), status.ToString()));
            cboStatus.SelectedIndex = 1;

            cboAssigned.Items.Add(new Choice(null, "Assigned: any"));
            foreach (var technician in _store.Technicians)
                cboAssigned.Items.Add(new Choice(technician.UserName, technician.UserName));
            cboAssigned.SelectedIndex = 0;

            foreach (var sort in new[]
            {
                new Choice("Priority", "Sort: Priority"), new Choice("DueAt", "Sort: Due date"),
                new Choice("AgeDays", "Sort: Age"), new Choice("Number", "Sort: Number"),
                new Choice("Title", "Sort: Title"), new Choice("Status", "Sort: Status"),
                new Choice("AssignedTo", "Sort: Assigned"),
            })
                cboSort.Items.Add(sort);
            cboSort.SelectedIndex = 0;

            foreach (int size in new[] { 25, 50, 100, 200 })
                cboPageSize.Items.Add(new Choice(size.ToString(CultureInfo.InvariantCulture), size + " / page"));
            cboPageSize.SelectedIndex = 1;

            foreach (var technician in _store.Technicians)
                cboTechnician.Items.Add(new Choice(technician.UserName, technician.UserName));
            SelectByKey(cboTechnician, "s.patel");

            FillSavedViews(null);
        }

        private void FillSavedViews(SavedView select)
        {
            cboSavedView.Items.Clear();
            foreach (var view in _session.SavedViews.All)
                cboSavedView.Items.Add(view);

            int index = select == null ? 0 : _session.SavedViews.All.ToList().FindIndex(v => v.Id == select.Id);
            if (cboSavedView.Items.Count > 0)
                cboSavedView.SelectedIndex = Math.Max(0, index);
        }

        private void UpdateViewBadge()
        {
            bool saved = Grid.SavedViewName != null;
            lblViewBadge.Text = saved ? "★ " + Grid.SavedViewName : "custom filters";
            lblViewBadge.ForeColor = saved
                ? System.Drawing.Color.FromArgb(11, 106, 230)
                : System.Drawing.Color.FromArgb(90, 107, 125);
        }

        private void UpdatePagerButtons(PagedResult<WorkQueueRow> result)
        {
            btnFirst.Enabled = btnPrev.Enabled = result.Page > 1;
            btnNext.Enabled = btnLast.Enabled = result.Page < result.PageCount;
        }

        /// <summary>The active sort column carries the arrow — the grid never sorted anything itself.</summary>
        private void UpdateSortHeaders()
        {
            string[] captions = { "Number", "Title", "Status", "Priority", "Assigned", "Due", "Age (d)", "v" };
            for (int i = 0; i < dgvQueue.Columns.Count && i < captions.Length; i++)
            {
                bool active = SortKeys[i] != null && SortKeys[i] == Grid.Query.SortBy;
                dgvQueue.Columns[i].HeaderText = active
                    ? captions[i] + (Grid.Query.Descending ? " ▼" : " ▲")
                    : captions[i];
            }
        }

        private static string SortKeyFor(int columnIndex) =>
            columnIndex >= 0 && columnIndex < SortKeys.Length ? SortKeys[columnIndex] : null;

        /// <summary>Priority and age read best newest-first; everything else reads best ascending.</summary>
        private static bool DefaultDescending(string sortBy) => sortBy == "Priority" || sortBy == "AgeDays";

        #endregion

        #region Small UI helpers

        private enum StatusKind { Ok, Warn, Error }
        private enum BannerKind { Success, Warning, Error }

        /// <summary>A filter / sort / page-size option: the key the service understands plus the text the user reads.</summary>
        private sealed class Choice
        {
            public string Key { get; }
            public string Label { get; }

            public Choice(string key, string label)
            {
                Key = key;
                Label = label;
            }

            public override string ToString() => Label;
        }

        private static string KeyOf(ComboBox combo) => (combo.SelectedItem as Choice)?.Key;

        private static void SelectByKey(ComboBox combo, string key)
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (combo.Items[i] is Choice choice && choice.Key == key)
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
        }

        private CommandContext NewCommand()
        {
            _current = _session.NewCommandContext();
            lblCorrelation.Text = "corr " + _current.CorrelationId;
            return _current;
        }

        private void BeginBusy(string text)
        {
            NewCommand();
            SetControlsEnabled(false);
            SetStatus(text, StatusKind.Warn);
            HideBanner();
        }

        /// <summary>After the awaits: controls back on, then push the pending changes over the socket.</summary>
        private void EndBusy()
        {
            SetControlsEnabled(true);
            UpdateActionUi();
            Application.Update(this);
        }

        private void SetControlsEnabled(bool enabled)
        {
            btnSearch.Enabled = enabled;
            btnApplyView.Enabled = enabled;
            btnSaveView.Enabled = enabled;
            btnSimulateRefresh.Enabled = enabled;
            btnFirst.Enabled = btnPrev.Enabled = btnNext.Enabled = btnLast.Enabled = enabled;
            btnSelectPage.Enabled = btnClearSelection.Enabled = enabled;
            btnBatchReassign.Enabled = enabled && Grid.Selected.Count > 0;
            btnLoadEverything.Enabled = enabled;
            btnApprovalLock.Enabled = enabled;
            btnConcurrentEdit.Enabled = enabled;
            btnSwitchUser.Enabled = enabled;
            btnRetryFailed.Enabled = enabled && _lastResult != null && _lastResult.Failed > 0;
            dgvQueue.Enabled = enabled;
        }

        private void ShowSignedIn()
        {
            lblTenant.Text = "tenant: " + _session.TenantId;
            lblUser.Text = $"Signed in: {_session.UserName} · {_session.Role}";
            btnSwitchUser.Text = _session.Role == UserRole.Technician
                ? "Back to ana.ops (Manager)"
                : "Run as ben.tech (Technician)";
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

        private void HideBanner() => lblBanner.Visible = false;

        /// <summary>Overdue rows read red; everything else reads normal. Display only — IsOverdue was computed on the server.</summary>
        private void PaintOverdueRows()
        {
            for (int i = 0; i < dgvQueue.Rows.Count && i < _pageRows.Count; i++)
            {
                dgvQueue.Rows[i].DefaultCellStyle.ForeColor = _pageRows[i].IsOverdue
                    ? System.Drawing.Color.FromArgb(178, 59, 39)
                    : System.Drawing.Color.FromArgb(31, 45, 58);
            }
        }

        /// <summary>The trace sink: one line per layer decision. The only place that knows about lstTrace.</summary>
        private void trace_LineAdded(string line)
        {
            if (IsDisposed)
                return;

            lstTrace.Items.Add(line);
            lstTrace.SelectedIndex = lstTrace.Items.Count - 1;
        }

        /// <summary>The trace belongs to the session: a rebuilt page replays what the previous one wrote.</summary>
        private void ReplayTrace()
        {
            lstTrace.Items.Clear();
            foreach (string line in _trace.Lines)
                lstTrace.Items.Add(line);
            if (lstTrace.Items.Count > 0)
                lstTrace.SelectedIndex = lstTrace.Items.Count - 1;
        }

        /// <summary>Called from Dispose and before "Simulate refresh" so a discarded page stops receiving trace lines.</summary>
        private void DetachTrace()
        {
            if (_trace != null)
                _trace.LineAdded -= trace_LineAdded;
        }

        #endregion
    }
}
