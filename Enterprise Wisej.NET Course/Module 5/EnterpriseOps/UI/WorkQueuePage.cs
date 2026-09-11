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
    /// EnterpriseOps — Enterprise Work Queue.
    ///
    /// The filter bar (search · status · assigned · sort), the saved views, <c>dgvQueue</c> bound to <b>one page</b>
    /// of <see cref="WorkQueueRow"/>, the pager, the batch bar, the progress line and the status bar.
    ///
    /// This file owns UI state only. <see cref="WorkQueueQueryService"/> filters, sorts, pages and projects;
    /// <see cref="BatchReassignWorkflow"/> validates and commits each row and returns the per-row report;
    /// <see cref="PermissionService"/> decides who may reassign. The grid state (filters, sort, page, selection)
    /// lives in <see cref="SessionContext"/>, not in this page, so a rebuilt page restores the same view.
    /// </summary>
    public partial class WorkQueuePage : Page
    {
        // Per-session services, wired in the constructor. Instance fields — never statics.
        private readonly SessionContext _session;
        private readonly ActivityTrace _log;
        private readonly WorkOrderStore _store;
        private readonly PermissionService _permissions;
        private readonly WorkQueueQueryService _queryService;
        private readonly BatchReassignWorkflow _batchWorkflow;

        private CommandContext _current;

        /// <summary>Kept in an instance field so the batch button can cancel the run it started.</summary>
        private CancellationTokenSource _batchCancellation;

        private string _batchTarget = "";
        private List<WorkQueueRow> _pageRows = new List<WorkQueueRow>();
        private bool _syncingSelection;

        /// <summary>Grid column order → the sort key the service understands.</summary>
        private static readonly string[] SortKeys =
        {
            "Number", "Title", "Status", "Priority", "AssignedTo", "DueAt", "AgeDays",
        };

        public WorkQueuePage()
        {
            InitializeComponent();

            _session = SessionContext.Current;
            _log = _session.Trace;
            _store = WorkOrderStore.Instance;
            _permissions = new PermissionService();
            _queryService = new WorkQueueQueryService(_store, _permissions, _session);
            _batchWorkflow = new BatchReassignWorkflow(_store, _permissions, AuditTrail.Instance, _log);

            FillFilterChoices();
        }

        /// <summary>The grid state the session owns.</summary>
        private GridState Grid => _session.WorkQueueGrid;

        /// <summary>The command running right now: same tenant and user, one correlation id per action.</summary>
        private CommandContext CurrentContext => _current ?? NewCommand();

        #region Event handlers

        private async void WorkQueuePage_Load(object sender, EventArgs e)
        {
            WriteQueryToControls(Grid.Query);
            try
            {
                await RunQueryAsync(Grid.Query, "Loading the first page…");
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
        }

        /// <summary>Search: the screen fills a WorkQueueQuery, the service returns one page.</summary>
        private async void btnSearch_Click(object sender, EventArgs e)
        {
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
        }

        private void btnClearSelection_Click(object sender, EventArgs e)
        {
            Grid.Selected.Clear();
            _syncingSelection = true;
            dgvQueue.ClearSelection();
            _syncingSelection = false;
            UpdateActionUi();
        }

        /// <summary>Applying a saved view runs the stored query again — the rows are always current.</summary>
        private async void btnApplyView_Click(object sender, EventArgs e)
        {
            var view = cboSavedView.SelectedItem as SavedView;
            if (view == null)
                return;

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

            FillSavedViews(view);
            UpdateViewBadge();
            AlertBox.Show($"Saved view “{view.Name}”.", MessageBoxIcon.Information,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        /// <summary>The batch command. While it runs the same button cancels it.</summary>
        private async void btnBatchReassign_Click(object sender, EventArgs e)
        {
            if (_batchCancellation != null)
            {
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
                    return;

                await RunBatchLoopAsync(items, target);
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
        }

        #endregion

        #region Running the services

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
        /// Runs the batch, shows the per-row report, and loops while the user asks to retry the failures.
        /// A retry is the next turn of this loop, with freshly read versions and a new correlation id.
        /// </summary>
        private async Task RunBatchLoopAsync(IReadOnlyList<BatchItem> items, string technician)
        {
            bool isRetry = false;
            while (items != null && items.Count > 0)
            {
                var result = await RunBatchAsync(items, technician, isRetry);
                if (result == null)
                    return;                                     // cancelled, or the command was rejected

                await RunQueryAsync(Grid.Query, "Reloading the page…");
                ShowBatchOutcome(result);
                Application.Update(this);

                DialogResult answer;
                using (var dialog = new BatchResultDialog(result))
                    answer = await dialog.ShowDialogAsync();

                if (answer != DialogResult.Retry)
                    return;

                items = BuildRetryItems(result);
                isRetry = true;
            }
        }

        /// <summary>One run of the workflow with progress. Returns null when the user cancelled or the command was invalid.</summary>
        private async Task<BatchResult> RunBatchAsync(IReadOnlyList<BatchItem> items, string technician, bool isRetry)
        {
            BeginBusy($"Reassigning {items.Count} work order(s) to {technician}…");
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
                ShowBanner("Batch cancelled. The rows already committed were not rolled back — the audit log lists them.", BannerKind.Warning);
                lblStatusBar.Text = "Batch cancelled";
                return null;
            }
            catch (ArgumentException ex)
            {
                // Command-level validation: nothing was touched.
                ShowBanner(ex.Message, BannerKind.Warning);
                lblStatusBar.Text = "Batch rejected — no work order was changed";
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

        /// <summary>The failed rows only, with the version they have now.</summary>
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

        #region Showing results

        /// <summary>One page of the projection → the grid, the pager, the status bar.</summary>
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

            lblPage.Text = string.Format(CultureInfo.InvariantCulture, "Page {0:N0} of {1:N0}", result.Page, result.PageCount);
            lblStatusBar.Text = string.Format(CultureInfo.InvariantCulture,
                "Page {0:N0} of {1:N0} · {2} of {3:N0} matching · sorted by {4} {5} · {6} ms",
                result.Page, result.PageCount, result.Items.Count, result.TotalCount,
                Grid.Query.SortBy, Grid.Query.Descending ? "↓" : "↑", _queryService.LastElapsedMs);

            UpdateSortHeaders();
            UpdateViewBadge();
            UpdatePagerButtons(result);
        }

        /// <summary>Partial failure is a normal result: the status bar names the numbers, the report says why.</summary>
        private void ShowBatchOutcome(BatchResult result)
        {
            lblStatusBar.Text = string.Format(CultureInfo.InvariantCulture,
                "Batch complete — {0} succeeded · {1} failed{2} · per-row report · audited",
                result.Succeeded, result.Failed, result.Skipped > 0 ? $" · {result.Skipped} skipped" : "");

            // A row that succeeded leaves the selection; the failures stay selected for the retry.
            foreach (var row in result.Rows.Where(r => r.Outcome == BatchRowOutcome.Succeeded))
                Grid.Selected.Remove(row.WorkOrderId);
            UpdateActionUi();
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
            string reference = CurrentContext.CorrelationId;
            _log.Write($"Error: {ex.GetType().Name} — {ex.Message} (ref {reference})");
            ShowBanner($"The action could not be completed. Check the log for details.  (ref {reference})", BannerKind.Error);
            lblStatusBar.Text = "The action could not be completed";
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

        private void UpdateActionUi()
        {
            int selected = Grid.Selected.Count;
            int allowed = Grid.Selected.Values.Count(r => r.CanReassign);

            lblSelection.Text = selected == 0
                ? "0 selected"
                : $"{selected} selected · {allowed} can be reassigned";
            btnBatchReassign.Text = selected == 0 ? "Reassign selected…" : $"Reassign {selected} selected…";
            btnBatchReassign.Enabled = selected > 0;
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
            lblViewBadge.Text = saved ? $"★ Saved view: “{Grid.SavedViewName}”" : "custom filters";
            lblViewBadge.ForeColor = saved
                ? System.Drawing.Color.FromArgb(11, 106, 230)
                : System.Drawing.Color.FromArgb(90, 107, 125);
        }

        private void UpdatePagerButtons(PagedResult<WorkQueueRow> result)
        {
            btnFirst.Enabled = btnPrev.Enabled = result.Page > 1;
            btnNext.Enabled = btnLast.Enabled = result.Page < result.PageCount;
        }

        /// <summary>The active sort column carries the arrow.</summary>
        private void UpdateSortHeaders()
        {
            string[] captions = { "Number", "Title", "Status", "Priority", "Assigned", "Due", "Age (d)" };
            for (int i = 0; i < dgvQueue.Columns.Count && i < captions.Length; i++)
            {
                bool active = SortKeys[i] == Grid.Query.SortBy;
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

        private enum BannerKind { Warning, Error }

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
            return _current;
        }

        private void BeginBusy(string text)
        {
            NewCommand();
            SetControlsEnabled(false);
            lblStatusBar.Text = text;
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
            btnFirst.Enabled = btnPrev.Enabled = btnNext.Enabled = btnLast.Enabled = enabled;
            btnSelectPage.Enabled = btnClearSelection.Enabled = enabled;
            btnBatchReassign.Enabled = enabled && Grid.Selected.Count > 0;
            dgvQueue.Enabled = enabled;
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

        private void HideBanner() => lblBanner.Visible = false;

        /// <summary>Overdue rows read red; everything else reads normal. IsOverdue was computed on the server.</summary>
        private void PaintOverdueRows()
        {
            for (int i = 0; i < dgvQueue.Rows.Count && i < _pageRows.Count; i++)
            {
                dgvQueue.Rows[i].DefaultCellStyle.ForeColor = _pageRows[i].IsOverdue
                    ? System.Drawing.Color.FromArgb(178, 59, 39)
                    : System.Drawing.Color.FromArgb(31, 45, 58);
            }
        }

        #endregion
    }
}
