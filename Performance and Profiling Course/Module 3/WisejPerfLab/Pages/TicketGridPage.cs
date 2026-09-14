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
    /// Written the obvious way, so it carries four separate costs the later modules take apart:
    /// the search returns full entities including two long text columns (Module 5), the customer name is
    /// fetched one statement per row (Module 6), the display rows are rebuilt on every redraw (Module 4),
    /// and the export blocks the request thread on <c>.Result</c> while writing the file a line at a time
    /// (Module 6).
    /// </remarks>
    public partial class TicketGridPage : UserControl, IScenarioPage
    {
        private readonly IPerfLabShell _shell;
        private readonly ScenarioProbe _probe;
        private readonly TicketSearchService _search;
        private readonly ExportService _export;

        private TicketSearchResult _result;

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
        public TicketDisplayRow FirstRowOrPlaceholder()
        {
            if (bindingSource1.DataSource is List<TicketDisplayRow> rows && rows.Count > 0)
                return rows[0];

            return new TicketDisplayRow
            {
                Id = 0,
                Number = "T-00000",
                Customer = "(no search yet)",
                Status = "Open",
                Priority = "Normal",
                AgeText = "0 min",
                UpdatedText = DateTime.Now.ToString("g")
            };
        }

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
            { Name = "colNumber", DataPropertyName = nameof(TicketDisplayRow.Number), HeaderText = "Ticket", Width = 110 });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colCustomer", DataPropertyName = nameof(TicketDisplayRow.Customer), HeaderText = "Customer", Width = 260 });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colStatus", DataPropertyName = nameof(TicketDisplayRow.Status), HeaderText = "Status", Width = 150 });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colPriority", DataPropertyName = nameof(TicketDisplayRow.Priority), HeaderText = "Priority", Width = 100 });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colAge", DataPropertyName = nameof(TicketDisplayRow.AgeText), HeaderText = "Age", Width = 90 });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colUpdated", DataPropertyName = nameof(TicketDisplayRow.UpdatedText), HeaderText = "Updated", Width = 140 });
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

                using (scope.Stage("query + per-row customer lookup"))
                    _result = _search.Search(filter);

                scope.Rows = _result.Tickets.Count;

                List<TicketDisplayRow> rows;
                using (scope.Stage("project display rows"))
                    rows = ProjectRows();

                using (scope.Stage("bind to the grid"))
                    BindRows(rows);

                var elapsed = scope.ElapsedMs;
                lblGridStatus.Text =
                    $"{_result.Tickets.Count:N0} rows   {_result.QueryCount:N0} SQL statements   " +
                    PerfBudget.Describe(Scenario, UserAction, elapsed);
                _shell.SetStatus(PerfBudget.StateFor(Scenario, UserAction, elapsed),
                    $"Tickets/Search {PerfBudget.Describe(Scenario, UserAction, elapsed)} — {_result.QueryCount:N0} statements for {_result.Tickets.Count:N0} rows");
            }
            catch (DatabaseUnavailableException ex)
            {
                scope.Fail(ex);
                BindRows(new List<TicketDisplayRow>());
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
        /// Builds the display rows from the entities that are already in memory — again, every time the
        /// screen redraws. Module 4 measures what this costs in allocations and replaces it with one
        /// projection that is kept.
        /// </summary>
        private List<TicketDisplayRow> ProjectRows()
        {
            var rows = new List<TicketDisplayRow>();
            if (_result == null)
                return rows;

            var now = DateTime.Now;
            foreach (var ticket in _result.Tickets)
            {
                var customerName = _result.CustomerNames.TryGetValue(ticket.CustomerId, out var name) ? name : "(unknown)";
                rows.Add(TicketFormatter.FormatRow(ticket, customerName, now));
            }

            return rows;
        }

        private void BindRows(List<TicketDisplayRow> rows)
        {
            bindingSource1.DataSource = rows;
            dataGridView1.DataSource = bindingSource1;
        }

        /// <summary>
        /// The redraw the user triggers by sorting, filtering or coming back to the tab: no query, and
        /// still a full rebuild of every row object on the server.
        /// </summary>
        private void btnRedraw_Click(object sender, EventArgs e)
        {
            if (_result == null)
            {
                _shell.SetStatus(ShellState.Idle, "search first — there is nothing to redraw");
                return;
            }

            using var scope = _probe.Measure(Scenario, "Redraw");
            var rows = ProjectRows();
            scope.Rows = rows.Count;
            BindRows(rows);

            lblGridStatus.Text = $"{rows.Count:N0} rows re-projected in {scope.ElapsedMs:N0} ms (no query)";
            _shell.SetStatus(ShellState.Warn, $"Tickets/Redraw {scope.ElapsedMs:N0} ms — no database work, all of it allocation");
        }

        #endregion

        #region Tickets/Export

        /// <summary>
        /// The export as it is first written: a blocking wait on an asynchronous method, inside the click
        /// handler, on the request thread. The progress callback updates the progress bar the user cannot
        /// see, because nothing reaches the browser until the handler returns.
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
            if (e.RowIndex < 0 || bindingSource1.DataSource is not List<TicketDisplayRow> rows)
                return;

            if (e.RowIndex >= rows.Count)
                return;

            var row = rows[e.RowIndex];
            var form = new TicketDetailForm(row);
            form.Show();
        }

        #endregion
    }
}
