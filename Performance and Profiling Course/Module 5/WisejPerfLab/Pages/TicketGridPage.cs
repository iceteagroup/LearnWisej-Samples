using System;
using System.IO;
using WisejPerfLab.Data;
using WisejPerfLab.Diagnostics;
using WisejPerfLab.Forms;
using WisejPerfLab.Models;
using WisejPerfLab.Services;
using WisejPerfLab.Shell;
using Wisej.Web;

namespace WisejPerfLab.Pages
{
    /// <summary>
    /// The ticket list: a status filter, a row count, a grid, and a CSV export.
    /// </summary>
    /// <remarks>
    /// Module 5 put the grid into <b>virtual mode</b>. The search now asks how many rows match and sets
    /// <c>RowCount</c>; the grid asks for the cells it is showing, and those are answered from a page of
    /// <see cref="TicketGridRow"/> that <see cref="TicketPageCache"/> holds. Fifty thousand rows cost one
    /// count query and one page — not fifty thousand row objects on the server and fifty thousand rows in
    /// the update payload.
    /// </remarks>
    public partial class TicketGridPage : UserControl, IScenarioPage
    {
        private readonly IPerfLabShell _shell;
        private readonly ScenarioProbe _probe;
        private readonly TicketQueryService _tickets;
        private readonly TicketPageCache _cache;
        private readonly ExportService _export;

        private int _rowCount;

        public TicketGridPage(IPerfLabShell shell)
        {
            InitializeComponent();

            _shell = shell;
            _probe = PerfLabServices.Get<ScenarioProbe>();
            _tickets = PerfLabServices.Get<TicketQueryService>();
            _export = PerfLabServices.Get<ExportService>();

            // One cache per screen, per session. It holds a few pages of rows, never the whole result.
            _cache = new TicketPageCache(_tickets);

            BuildColumns();
            BuildFilterLookups();

            dataGridView1.VirtualMode = true;
        }

        public string Scenario => "Tickets";

        public string UserAction => "Search";

        public void RunScenario() => RunSearch(fromShell: true);

        /// <summary>The first row of the current result, or a stand-in — used by the lab controls.</summary>
        public TicketGridRow FirstRowOrPlaceholder()
            => (_rowCount > 0 ? _cache.RowAt(0) : null) ?? new TicketGridRow
            {
                Id = 0,
                Number = "T-00000",
                Customer = "(no search yet)",
                Status = "Open",
                Priority = "Normal",
                AgeText = "0 min",
                UpdatedText = DateTime.Now.ToString("g")
            };

        #region Filter and columns

        private void BuildFilterLookups()
        {
            cboStatus.Items.AddRange(new object[] { "Open", "Waiting", "Escalated", "Closed" });
            cboStatus.SelectedIndex = 0;

            cboPageSize.Items.AddRange(new object[] { "1000", "5000", "50000" });
            cboPageSize.SelectedIndex = 1;
        }

        private void BuildColumns()
        {
            // The column order is the switch in TicketPageCache.GetValue. Keep them together.
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNumber", HeaderText = "Ticket", Width = 110 });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCustomer", HeaderText = "Customer", Width = 260 });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStatus", HeaderText = "Status", Width = 150 });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPriority", HeaderText = "Priority", Width = 100 });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "colAge", HeaderText = "Age", Width = 90 });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUpdated", HeaderText = "Updated", Width = 140 });
        }

        private TicketFilter CurrentFilter => new TicketFilter
        {
            Status = cboStatus.SelectedItem as string ?? "Open",
            PageSize = int.Parse(cboPageSize.SelectedItem as string ?? "5000")
        };

        #endregion

        #region Tickets/Search

        private void btnSearch_Click(object sender, EventArgs e) => RunSearch(fromShell: false);

        private void RunSearch(bool fromShell)
        {
            using var scope = _probe.Measure(Scenario, UserAction);

            btnSearch.Enabled = false;
            if (!fromShell)
                _shell.ClearBanner();

            try
            {
                var filter = CurrentFilter;

                // The filter changed, so every held page is wrong. Throwing them away is the only way a
                // filter that matches nothing cannot show a stale page.
                _cache.Reset(filter);

                using (scope.Stage("count query"))
                    _rowCount = Math.Min(_tickets.Count(filter), filter.PageSize);

                using (scope.Stage("row count + first page"))
                {
                    dataGridView1.RowCount = 0;
                    dataGridView1.RowCount = _rowCount;
                }

                scope.Rows = _rowCount;

                var elapsed = scope.ElapsedMs;
                lblGridStatus.Text =
                    $"{_rowCount:N0} rows   {_cache.Describe()}   " + PerfBudget.Describe(Scenario, UserAction, elapsed);
                _shell.SetStatus(PerfBudget.StateFor(Scenario, UserAction, elapsed),
                    $"Tickets/Search {PerfBudget.Describe(Scenario, UserAction, elapsed)} — {_rowCount:N0} rows");
            }
            catch (DatabaseUnavailableException ex)
            {
                scope.Fail(ex);
                _rowCount = 0;
                dataGridView1.RowCount = 0;
                lblGridStatus.Text = "search failed after " + scope.ElapsedMs + " ms";
                _shell.SetStatus(ShellState.Fault, "Tickets/Search failed — the ticket database is unreachable");
                _shell.ShowBanner("Ticket search failed: the ticket database is unreachable. The grid was emptied so no stale page is mistaken for a result.");
            }
            finally
            {
                btnSearch.Enabled = true;
            }
        }

        /// <summary>
        /// One cell, answered from a page that is already in memory. No query here, no formatting here,
        /// and no lock: the browser can ask for several blocks of rows at once while scrolling.
        /// </summary>
        private void dataGridView1_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            e.Value = _cache.GetValue(e.RowIndex, e.ColumnIndex);
        }

        /// <summary>
        /// The client names the block of rows it is about to read, so one fetch serves the whole visible
        /// window instead of one per row.
        /// </summary>
        private void dataGridView1_DataRead(object sender, DataGridViewDataReadEventArgs e)
        {
            if (!_cache.Prefetch(e.FirstIndex, e.LastIndex))
            {
                _shell.SetStatus(ShellState.Fault, "a page of rows could not be loaded: " + _cache.LastError);
                _shell.ShowBanner("Some rows could not be loaded. The cells are empty rather than showing values from the wrong rows.");
                return;
            }

            lblGridStatus.Text = $"{_rowCount:N0} rows   {_cache.Describe()}";
        }

        /// <summary>
        /// The redraw the user triggers by sorting, filtering or coming back to the tab. In virtual mode
        /// it is a repaint of the rows on screen: no query, no row objects, nothing to rebuild.
        /// </summary>
        private void btnRedraw_Click(object sender, EventArgs e)
        {
            if (_rowCount == 0)
            {
                _shell.SetStatus(ShellState.Idle, "search first — there is nothing to redraw");
                return;
            }

            using var scope = _probe.Measure(Scenario, "Redraw");
            scope.Rows = _rowCount;
            dataGridView1.Refresh();

            lblGridStatus.Text = $"{_rowCount:N0} rows repainted in {scope.ElapsedMs:N0} ms — {_cache.Describe()}";
            _shell.SetStatus(ShellState.Ok, $"Tickets/Redraw {scope.ElapsedMs:N0} ms — only the visible rows exist");
        }

        #endregion

        #region Tickets/Export

        /// <summary>
        /// The export as it is first written: a blocking wait on an asynchronous method, inside the click
        /// handler, on the request thread. The progress callback updates a progress bar the user cannot
        /// see, because nothing reaches the browser until the handler returns. Module 6 fixes it.
        /// </summary>
        private void btnExport_Click(object sender, EventArgs e)
        {
            using var scope = _probe.Measure(Scenario, "Export");

            btnExport.Enabled = false;
            _shell.ClearBanner();

            try
            {
                var filter = CurrentFilter;
                var path = _export.BuildAsync(filter, (done, total) =>
                {
                    progressExport.Maximum = total;
                    progressExport.Value = done;
                }).Result;

                scope.Rows = filter.PageSize;
                lblGridStatus.Text = $"exported {Path.GetFileName(path)} in {scope.ElapsedMs:N0} ms";
                _shell.SetStatus(PerfBudget.StateFor(Scenario, "Export", scope.ElapsedMs),
                    "Tickets/Export " + PerfBudget.Describe(Scenario, "Export", scope.ElapsedMs));
            }
            catch (AggregateException ex)
            {
                scope.Fail(ex);
                _shell.SetStatus(ShellState.Fault, "Tickets/Export failed");
                _shell.ShowBanner("Export failed: " + (ex.InnerException?.Message ?? ex.Message));
            }
            finally
            {
                btnExport.Enabled = true;
                progressExport.Value = 0;
            }
        }

        #endregion

        #region The ticket detail form

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var row = e.RowIndex < 0 ? null : _cache.RowAt(e.RowIndex);
            if (row == null)
                return;

            var form = new TicketDetailForm(row);
            form.Show();
        }

        #endregion
    }
}
