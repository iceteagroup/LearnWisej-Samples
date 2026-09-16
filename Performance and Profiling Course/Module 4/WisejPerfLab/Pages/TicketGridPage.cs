using System;
using System.Collections.Generic;
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
    /// Module 4 changed one thing here: the search result is projected into <see cref="TicketRow"/>
    /// <b>once</b> and kept, so a redraw rebinds the rows it already has instead of building 5,000 new
    /// objects and 25,000 new strings. The query still returns full entities and still issues one
    /// statement per row — Module 5 narrows the projection at the source, Module 6 removes the N+1.
    /// </remarks>
    public partial class TicketGridPage : UserControl, IScenarioPage
    {
        private readonly IPerfLabShell _shell;
        private readonly ScenarioProbe _probe;
        private readonly TicketSearchService _search;
        private readonly ExportService _export;

        /// <summary>The projected rows of the last search. Built once, rebound as often as needed.</summary>
        private List<TicketRow> _rows = new List<TicketRow>();

        private int _lastQueryCount;

        public TicketGridPage(IPerfLabShell shell)
        {
            InitializeComponent();

            _shell = shell;
            _probe = PerfLabServices.Get<ScenarioProbe>();
            _search = PerfLabServices.Get<TicketSearchService>();
            _export = PerfLabServices.Get<ExportService>();

            BuildColumns();
            BuildFilterLookups();
        }

        public string Scenario => "Tickets";

        public string UserAction => "Search";

        public void RunScenario() => RunSearch(fromShell: true);

        /// <summary>The first row of the current result, or a stand-in — used by the lab controls.</summary>
        public TicketRow FirstRowOrPlaceholder()
            => _rows.Count > 0
                ? _rows[0]
                : new TicketRow
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
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colNumber", DataPropertyName = nameof(TicketRow.Number), HeaderText = "Ticket", Width = 110 });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colCustomer", DataPropertyName = nameof(TicketRow.Customer), HeaderText = "Customer", Width = 260 });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colStatus", DataPropertyName = nameof(TicketRow.Status), HeaderText = "Status", Width = 150 });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colPriority", DataPropertyName = nameof(TicketRow.Priority), HeaderText = "Priority", Width = 100 });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colAge", DataPropertyName = nameof(TicketRow.AgeText), HeaderText = "Age", Width = 90 });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colUpdated", DataPropertyName = nameof(TicketRow.UpdatedText), HeaderText = "Updated", Width = 140 });
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

                TicketSearchResult result;
                using (scope.Stage("query + per-row customer lookup"))
                    result = _search.Search(filter);

                scope.Rows = result.Tickets.Count;
                _lastQueryCount = result.QueryCount;

                // Projected once. From here on the grid, the redraw and the detail form all read these
                // rows; the entities are dropped when this method returns.
                using (scope.Stage("project display rows (once)"))
                    _rows = ProjectRows(result);

                using (scope.Stage("bind to the grid"))
                    BindRows();

                var elapsed = scope.ElapsedMs;
                lblGridStatus.Text =
                    $"{_rows.Count:N0} rows   {_lastQueryCount:N0} SQL statements   " +
                    PerfBudget.Describe(Scenario, UserAction, elapsed);
                _shell.SetStatus(PerfBudget.StateFor(Scenario, UserAction, elapsed),
                    $"Tickets/Search {PerfBudget.Describe(Scenario, UserAction, elapsed)} — {_lastQueryCount:N0} statements for {_rows.Count:N0} rows");
            }
            catch (DatabaseUnavailableException ex)
            {
                scope.Fail(ex);
                _rows = new List<TicketRow>();
                BindRows();
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
        /// One pass over the entities, one <see cref="TicketRow"/> each, display strings built here and
        /// not again. Everything the grid needs is a string by the time this returns.
        /// </summary>
        private static List<TicketRow> ProjectRows(TicketSearchResult result)
        {
            var now = DateTime.Now;
            var rows = new List<TicketRow>(result.Tickets.Count);

            foreach (var ticket in result.Tickets)
            {
                var customerName = result.CustomerNames.TryGetValue(ticket.CustomerId, out var name) ? name : "(unknown)";
                var display = TicketFormatter.FormatRow(ticket, customerName, now);

                rows.Add(new TicketRow
                {
                    Id = display.Id,
                    Number = display.Number,
                    Customer = display.Customer,
                    Status = display.Status,
                    Priority = display.Priority,
                    AgeText = display.AgeText,
                    UpdatedText = display.UpdatedText
                });
            }

            return rows;
        }

        private void BindRows()
        {
            bindingSource1.DataSource = _rows;
            dataGridView1.DataSource = bindingSource1;
        }

        /// <summary>
        /// The redraw the user triggers by sorting, filtering or coming back to the tab. Since Module 4
        /// it rebinds the rows that already exist: no query, and no new objects.
        /// </summary>
        private void btnRedraw_Click(object sender, EventArgs e)
        {
            if (_rows.Count == 0)
            {
                _shell.SetStatus(ShellState.Idle, "search first — there is nothing to redraw");
                return;
            }

            using var scope = _probe.Measure(Scenario, "Redraw");
            scope.Rows = _rows.Count;
            BindRows();

            lblGridStatus.Text = $"{_rows.Count:N0} rows rebound in {scope.ElapsedMs:N0} ms (no query, no new rows)";
            _shell.SetStatus(ShellState.Ok, $"Tickets/Redraw {scope.ElapsedMs:N0} ms — the rows were projected once, at search time");
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
            if (e.RowIndex < 0 || e.RowIndex >= _rows.Count)
                return;

            var form = new TicketDetailForm(_rows[e.RowIndex]);
            form.Show();
        }

        #endregion
    }
}
