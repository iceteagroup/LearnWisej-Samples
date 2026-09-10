using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using OperationsConsole.Models;
using OperationsConsole.Orders;
using OperationsConsole.Services;
using OperationsConsole.Shell;
using Wisej.Web;

namespace OperationsConsole.Sections
{
    /// <summary>
    /// The <b>Orders</b> section — Module 5 · DataGridView Mastery.
    /// <para>
    /// One grid, two paths. The <b>bound path</b> puts a filtered working set through <c>ordersSource</c>
    /// (a <see cref="BindingSource"/>) into <c>ordersGrid</c>, with <c>AutoGenerateColumns</c> off and six columns
    /// defined on purpose. The <b>virtual path</b> hands the grid only a row count and answers
    /// <c>CellValueNeeded</c> from <see cref="OrderCache"/>, which fetches whole pages through
    /// <see cref="OrderService"/> and prefetches in <c>DataRead</c>. The filter strip switches between them without
    /// changing what the user is looking at.
    /// </para>
    /// <para>
    /// Everything a user can be told happens through the shell (<see cref="ConsoleLog"/>) and the strips composed
    /// around the grid; every business decision happens in <see cref="OrderService"/>. The cell handlers below are
    /// deliberately three lines long each — that is the lesson.
    /// </para>
    /// </summary>
    public partial class DataGridViewPage : UserControl, ISection
    {
        private readonly OrderService _orderService = new OrderService();
        private readonly OrderCache _orderCache;
        private readonly OrderFilter _filter = new OrderFilter();

        /// <summary>How many rows the bound path is allowed to materialise before it says "narrow the filter".</summary>
        private const int BoundRowLimit = 250;

        private int _boundMatchCount;
        private DateTime? _lastRefresh;
        private bool _loading;
        private bool _writingCell;              // re-entrancy guard while a cell value is put back
        private string _editingOrderNumber;     // the order whose due-date cell is open for edit
        private DateTime _editingOriginalDueDate;

        public DataGridViewPage()
        {
            InitializeComponent();

            _orderCache = new OrderCache(_orderService, pageSize: 200);

            ConfigureGrid();
            PopulateStatusFilter();
            ComposeGrid();
            ApplyEditorChoice();

            ConsoleLog.Add("OrdersService generated " + _orderService.TotalOrders + " orders in memory (bound path shows the filtered set, virtual path shows all of them)");
            LoadOrders("section opened");
        }

        // ------------------------------------------------------------------------------------------------------------
        // ISection
        // ------------------------------------------------------------------------------------------------------------

        /// <inheritdoc/>
        public string Title => "DataGridView";

        /// <inheritdoc/>
        public void RefreshSection()
        {
            // the shell's Refresh re-runs exactly what the filter strip currently asks for — nothing is reset behind
            // the user's back, the cache is primed again and the "last refresh" stamp moves.
            LoadOrdersAsync("RefreshSection()");
        }

        // ------------------------------------------------------------------------------------------------------------
        // Set-up: grid, filter strip, composition
        // ------------------------------------------------------------------------------------------------------------

        /// <summary>Grid-wide settings that are behaviour rather than layout.</summary>
        private void ConfigureGrid()
        {
            // AutoGenerateColumns stays off for good: the six columns in the designer are the screen's contract with
            // the user. Add a property to OrderRow tomorrow and this grid does not move.
            ordersGrid.AutoGenerateColumns = false;

            // the friendly empty state — the grid says it, so no message label has to be shown and hidden
            ordersGrid.NoDataMessage =
                "<div style='padding:24px;text-align:center;color:#5a6b7d'>" +
                "No orders match this filter.<br/>Clear the search box or pick another status.</div>";
        }

        private void PopulateStatusFilter()
        {
            cboStatus.Items.Add(OrderFilter.AllStatuses);
            foreach (var status in OrderService.Statuses)
                cboStatus.Items.Add(status);

            cboStatus.SelectedIndex = 0;
        }

        /// <summary>
        /// The lab's composition step. The grid card holds three children; docking is applied from the last child to
        /// the first, so the two strips have to sit <i>behind</i> the grid for them to span the full width and for the
        /// Fill grid to take exactly what is left. <see cref="Control.SendToBack"/> and
        /// <see cref="Control.BringToFront"/> say that in one line each — and the grid keeps its full API, because it
        /// is a plain child of a Panel and not hidden inside a UserControl.
        /// </summary>
        private void ComposeGrid()
        {
            pnlFilterStrip.SendToBack();    // docked first  → Top strip spans the card
            pnlGridStatus.SendToBack();     // docked next   → Bottom strip spans the card
            ordersGrid.BringToFront();      // docked last   → Fill takes the space between them

            ConsoleLog.Add("compose pnlGridCard · dock order = " + DescribeDockOrder(pnlGridCard));
        }

        /// <summary>The children of a container in the order docking is applied to them (last child first).</summary>
        private static string DescribeDockOrder(Control parent)
        {
            var names = new List<string>();
            for (var i = parent.Controls.Count - 1; i >= 0; i--)
                names.Add(parent.Controls[i].Name + " (" + parent.Controls[i].Dock + ")");

            return string.Join(" → ", names);
        }

        // ------------------------------------------------------------------------------------------------------------
        // Loading — the success path, the progress path and the two failure paths
        // ------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// The loading state: the grid shows its loader, the commands are disabled, and the work happens after a short
        /// wait so the state is actually visible. A real query would be awaited here instead of <c>Task.Delay</c>.
        /// </summary>
        private async void LoadOrdersAsync(string reason)
        {
            if (_loading)
                return;

            _loading = true;
            SetBusy(true);
            ConsoleLog.Add("… loading orders (" + reason + ")");
            ConsoleLog.Status("Loading orders…", StatusLevel.Warning);
            Application.Update(this);            // push the loader before the wait

            try
            {
                await Task.Delay(400);
                LoadOrders(reason);
            }
            finally
            {
                SetBusy(false);
                _loading = false;
                Application.Update(this);
            }
        }

        /// <summary>Reads the filter strip and rebuilds the grid on whichever path is selected.</summary>
        private void LoadOrders(string reason)
        {
            ReadFilterFromStrip();

            ConsoleLog.Add("LoadOrders(" + reason + ") · " + (chkVirtualMode.Checked ? "virtual path" : "bound path") +
                           " · " + _filter.Describe());
            _orderService.ResetFetchCount();

            try
            {
                if (chkVirtualMode.Checked)
                    ShowVirtualOrders();
                else
                    ShowBoundOrders();

                _lastRefresh = DateTime.Now;
            }
            catch (OrderServiceException ex)
            {
                ReportServiceFailure(ex);
            }

            UpdateStatusStrip();
        }

        private void ReadFilterFromStrip()
        {
            _filter.Search = txtSearch.Text ?? "";
            _filter.Status = cboStatus.SelectedItem as string ?? OrderFilter.AllStatuses;
        }

        /// <summary>The bound path: a filtered working set, materialised once, handed to the BindingSource.</summary>
        private void ShowBoundOrders()
        {
            LeaveVirtualMode();

            var page = _orderService.GetOrders(_filter, BoundRowLimit);
            _boundMatchCount = page.TotalCount;
            BindOrders(page.Rows);

            if (page.Rows.Count == 0)
            {
                ConsoleLog.Add("empty result — ordersGrid.NoDataMessage is what the user sees, not an exception");
                ConsoleLog.Status("No orders match " + _filter.Describe() + " — clear the filter or search for something else.", StatusLevel.Warning);
            }
            else if (page.TotalCount > page.Rows.Count)
            {
                // the boundary the module is about: the bound path is for a working set, not for everything
                ConsoleLog.Add("bound path capped at " + BoundRowLimit + " rows (" + page.TotalCount + " match) — " +
                               "binding the whole result would create one row object per row");
                ConsoleLog.Status("Showing the first " + page.Rows.Count + " of " + page.TotalCount.ToString("N0") +
                                  " matching orders — narrow the filter, or tick Virtual mode to scroll them all.", StatusLevel.Warning);
            }
            else
            {
                ConsoleLog.Status(page.Rows.Count + " orders on the bound path · " + _filter.Describe(), StatusLevel.Ok);
            }
        }

        /// <summary>
        /// The lab's binding method: columns are designed, the BindingSource holds the list, the grid holds the
        /// BindingSource. Three lines, and nothing else in the screen knows how the rows were obtained.
        /// </summary>
        private void BindOrders(IEnumerable<OrderRow> rows)
        {
            ordersGrid.AutoGenerateColumns = false;
            ordersSource.DataSource = rows.ToList();
            ordersGrid.DataSource = ordersSource;
        }

        /// <summary>
        /// The virtual path: no rows are materialised at all. The grid is told how many rows exist and asks for the
        /// values of the ones it actually shows.
        /// </summary>
        private void ShowVirtualOrders()
        {
            ordersGrid.DataSource = null;
            ordersSource.DataSource = null;

            _orderCache.Reset(_filter);

            ordersGrid.VirtualMode = true;
            colDueDate.ReadOnly = true;     // the cache is a read model; edits stay on the bound path (see docs/GridDecisions.md)

            var count = _orderService.Count(_filter);
            ordersGrid.RowCount = count;

            if (count == 0)
            {
                ConsoleLog.Add("virtual path · RowCount = 0 — the same friendly empty state, no rows fetched");
                ConsoleLog.Status("No orders match " + _filter.Describe() + " — clear the filter or search for something else.", StatusLevel.Warning);
            }
            else
            {
                ConsoleLog.Add("virtual path · RowCount = " + count + ", rows created: 0 — values arrive through CellValueNeeded");
                ConsoleLog.Status(count + " orders on the virtual path · " + _filter.Describe() + " · scroll and watch the fetch count", StatusLevel.Ok);
            }
        }

        private void LeaveVirtualMode()
        {
            if (ordersGrid.VirtualMode)
            {
                ordersGrid.RowCount = 0;
                ordersGrid.VirtualMode = false;
            }

            colDueDate.ReadOnly = false;
        }

        private void SetBusy(bool busy)
        {
            ordersGrid.ShowLoader = busy;
            btnLoadOrders.Enabled = !busy;
            btnApply.Enabled = !busy;
            btnClearFilter.Enabled = !busy;
            btnOpenSelected.Enabled = !busy;
            btnPushDueDate.Enabled = !busy;
            btnPastDueDate.Enabled = !busy;
            btnInjectUnsafeStatus.Enabled = !busy;
        }

        // ------------------------------------------------------------------------------------------------------------
        // The composed status strip
        // ------------------------------------------------------------------------------------------------------------

        private void UpdateStatusStrip()
        {
            lblRowCount.Text = ordersGrid.VirtualMode
                ? "Rows: " + ordersGrid.RowCount.ToString("N0") + " (virtual)"
                : "Rows: " + ordersSource.Count.ToString("N0") + " of " + _boundMatchCount.ToString("N0") + " (bound)";

            lblCacheState.Text = ordersGrid.VirtualMode
                ? "Cache: " + _orderCache.Describe()
                : "Bound path — the whole filtered set is in memory";

            lblLastRefresh.Text = "Last refresh: " + (_lastRefresh.HasValue ? _lastRefresh.Value.ToString("HH:mm:ss") : "—");
        }

        private void ShowSelectedOrder(OrderRow order)
        {
            if (order == null)
            {
                lblSelectedOrder.Text = "Selected: —";
                ConsoleLog.Record(null);
                return;
            }

            lblSelectedOrder.Text = "Selected: " + order.Number;
            ConsoleLog.Record(order.Number);
        }

        // ------------------------------------------------------------------------------------------------------------
        // Grid events — each one hands the work to a named method
        // ------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Display only: the raw status becomes an encoded badge. The model still holds "On hold"; no rule is decided
        /// here, and <c>WebUtility.HtmlEncode</c> makes sure text that came from a human is shown, never executed.
        /// </summary>
        private void ordersGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.ColumnIndex >= ordersGrid.Columns.Count)
                return;
            if (ordersGrid.Columns[e.ColumnIndex].Name != colStatus.Name)
                return;

            var status = Convert.ToString(e.Value);
            if (string.IsNullOrEmpty(status))
                return;

            e.Value = StatusBadge(status);
            e.FormattingApplied = true;
        }

        /// <summary>The badge markup. Encoded text, inline colours, nothing that can execute.</summary>
        private static string StatusBadge(string status)
        {
            string fore, back;
            switch (status)
            {
                case "Open": fore = "#1565d8"; back = "#e7f0fd"; break;
                case "Confirmed": fore = "#1f9d57"; back = "#e6f6ec"; break;
                case "Packed": fore = "#7d5ae0"; back = "#efeafc"; break;
                case "Shipped": fore = "#0f8a8a"; back = "#e3f5f5"; break;
                case "On hold": fore = "#b8791f"; back = "#fdf1de"; break;
                case "Cancelled": fore = "#c0452c"; back = "#fbe9e5"; break;
                default: fore = "#5a6b7d"; back = "#eef2f7"; break;
            }

            return "<span style=\"display:inline-block;padding:1px 10px;border-radius:10px;font-weight:600;color:" +
                   fore + ";background-color:" + back + ";\">" + WebUtility.HtmlEncode(status) + "</span>";
        }

        /// <summary>The whole large-data read path: one cell, answered by the cache.</summary>
        private void ordersGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            e.Value = _orderCache.GetValue(e.RowIndex, e.ColumnIndex);
        }

        /// <summary>
        /// The documented hook for building the cache: the client says which block of rows it is about to read, so the
        /// pages are in memory before the first <c>CellValueNeeded</c> of that block arrives.
        /// </summary>
        private void ordersGrid_DataRead(object sender, DataGridViewDataReadEventArgs e)
        {
            var before = _orderCache.Fetches;
            var ok = _orderCache.Prefetch(e.FirstIndex, e.LastIndex);

            ConsoleLog.Add("DataRead rows " + e.FirstIndex + "–" + e.LastIndex + " → " +
                           (_orderCache.Fetches - before) + " page fetch(es) · " + _orderCache.Describe());

            if (!ok)
                ReportServiceFailure(_orderCache.LastError);

            UpdateStatusStrip();
        }

        private void ordersGrid_SelectionChanged(object sender, EventArgs e)
        {
            // the grid re-selects a row while it is being rebuilt; resolving an order then would only make the cache
            // fetch a page nobody asked for
            if (_loading || _writingCell)
                return;

            ShowSelectedOrder(CurrentOrder());
        }

        /// <summary>
        /// The command column. Wisej.NET 4.1 raises <c>CellClick</c> — "fired when any part of a cell is clicked" —
        /// where WinForms has a separate <c>CellContentClick</c>; the handler keeps the lab's name.
        /// It reads the row's stable ID and calls the service. No business data is read out of the cell text.
        /// </summary>
        private void ordersGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != colOpen.Index)
                return;

            ConsoleLog.Control(colOpen.Name);
            OpenOrder(OrderNumberAt(e.RowIndex));
        }

        /// <summary>
        /// The custom editor's first half: copy the stored value into the editor. A <see cref="MonthCalendar"/> has no
        /// change event the grid listens to, so the round trip is done by hand — this is the "editor that never
        /// commits" bug, solved.
        /// </summary>
        private void ordersGrid_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != colDueDate.Index)
                return;

            var order = OrderAt(e.RowIndex);
            if (order == null)
            {
                e.Cancel = true;
                return;
            }

            _editingOrderNumber = order.Number;
            _editingOriginalDueDate = order.DueDate;

            if (chkCalendarEditor.Checked)
            {
                dueDateCalendar.SelectionStart = order.DueDate;
                dueDateCalendar.SelectionEnd = order.DueDate;
            }

            ConsoleLog.Control(colDueDate.Name);
            ConsoleLog.Record(order.Number);
            ConsoleLog.Add("CellBeginEdit " + order.Number + " · stored due date " + order.DueDate.ToString("yyyy-MM-dd") +
                           " → " + (chkCalendarEditor.Checked ? "MonthCalendar (custom editor)" : "DateTimePicker column (built-in editor)"));
        }

        /// <summary>
        /// The custom editor's second half: take the edited value back out and let the service decide. Rejection is a
        /// normal answer — the old value goes back into the cell and the user is told why.
        /// </summary>
        private void ordersGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (_writingCell)
                return;
            if (e.RowIndex < 0 || e.ColumnIndex != colDueDate.Index)
                return;

            var number = _editingOrderNumber;
            var original = _editingOriginalDueDate;
            _editingOrderNumber = null;

            if (string.IsNullOrEmpty(number))
                return;

            var edited = EditedDueDate(e.RowIndex, original);
            ConsoleLog.Add("CellEndEdit " + number + " · editor returned " + edited.ToString("yyyy-MM-dd"));

            if (edited.Date == original.Date)
            {
                ConsoleLog.Add("… unchanged — no service call");
                return;
            }

            ApplyDueDate(number, edited, e.RowIndex, original);
        }

        /// <summary>Where the edited value comes from: the custom editor when it is switched on, the cell otherwise.</summary>
        private DateTime EditedDueDate(int rowIndex, DateTime fallback)
        {
            if (chkCalendarEditor.Checked)
                return dueDateCalendar.SelectionStart.Date;

            var value = ordersGrid.Rows[rowIndex].Cells[colDueDate.Index].Value;
            if (value is DateTime date)
                return date.Date;

            DateTime parsed;
            if (value != null && DateTime.TryParse(Convert.ToString(value), out parsed))
                return parsed.Date;

            return fallback;
        }

        // ------------------------------------------------------------------------------------------------------------
        // Commands — one path each, every one of them clickable
        // ------------------------------------------------------------------------------------------------------------

        private void btnLoadOrders_Click(object sender, EventArgs e)
        {
            ConsoleLog.Control(btnLoadOrders.Name);
            LoadOrdersAsync("btnLoadOrders");
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            ConsoleLog.Control(btnApply.Name);
            LoadOrdersAsync("btnApply · filter strip");
        }

        private void btnClearFilter_Click(object sender, EventArgs e)
        {
            ConsoleLog.Control(btnClearFilter.Name);
            txtSearch.Text = "";
            cboStatus.SelectedIndex = 0;
            LoadOrdersAsync("btnClearFilter");
        }

        private void chkVirtualMode_CheckedChanged(object sender, EventArgs e)
        {
            ConsoleLog.Control(chkVirtualMode.Name);
            ConsoleLog.Add(chkVirtualMode.Checked
                ? "switching to the virtual path — VirtualMode = true, RowCount from the service, values through OrderCache"
                : "switching to the bound path — the filtered set is materialised once into ordersSource");
            LoadOrdersAsync("chkVirtualMode");
        }

        private void chkCalendarEditor_CheckedChanged(object sender, EventArgs e)
        {
            ConsoleLog.Control(chkCalendarEditor.Name);
            ApplyEditorChoice();
        }

        /// <summary>
        /// Assigns (or clears) the custom editor. <c>DataGridViewColumn.Editor</c> takes any control; clearing it puts
        /// the typed <see cref="DataGridViewDateTimePickerColumn"/>'s own editor back, so both halves of the lab step
        /// stay demonstrable side by side.
        /// </summary>
        private void ApplyEditorChoice()
        {
            colDueDate.Editor = chkCalendarEditor.Checked ? dueDateCalendar : null;

            ConsoleLog.Add(chkCalendarEditor.Checked
                ? "colDueDate.Editor = dueDateCalendar (MonthCalendar) — CellBeginEdit / CellEndEdit move the value"
                : "colDueDate.Editor = null — the DataGridViewDateTimePickerColumn edits the cell itself");
            ConsoleLog.Status(chkCalendarEditor.Checked
                ? "Due-date cells are edited by a MonthCalendar; Tab, Enter or a click elsewhere commits."
                : "Due-date cells are edited by the built-in date picker.", StatusLevel.Ok);
        }

        private void btnOpenSelected_Click(object sender, EventArgs e)
        {
            ConsoleLog.Control(btnOpenSelected.Name);

            var order = CurrentOrder();
            if (order == null)
            {
                ConsoleLog.Status("Select an order first — click a row, then Open.", StatusLevel.Warning);
                return;
            }

            OpenOrder(order.Number);
        }

        private void btnPushDueDate_Click(object sender, EventArgs e)
        {
            ConsoleLog.Control(btnPushDueDate.Name);

            var order = CurrentOrder();
            if (order == null)
            {
                ConsoleLog.Status("Select an order first — click a row, then move its due date.", StatusLevel.Warning);
                return;
            }

            // never before today: the same rule the service enforces, so the success path really succeeds
            var start = order.DueDate < DateTime.Today ? DateTime.Today : order.DueDate;
            ApplyDueDate(order.Number, start.AddDays(7), CurrentRowIndex(), order.DueDate);
        }

        private void btnPastDueDate_Click(object sender, EventArgs e)
        {
            ConsoleLog.Control(btnPastDueDate.Name);

            var order = CurrentOrder();
            if (order == null)
            {
                ConsoleLog.Status("Select an order first — click a row, then try the rejected edit.", StatusLevel.Warning);
                return;
            }

            // deliberately invalid — the same call the in-cell editor makes, so the rejection is the real one
            ApplyDueDate(order.Number, DateTime.Today.AddDays(-7), CurrentRowIndex(), order.DueDate);
        }

        private void btnInjectUnsafeStatus_Click(object sender, EventArgs e)
        {
            ConsoleLog.Control(btnInjectUnsafeStatus.Name);

            var order = CurrentOrder();
            if (order == null)
            {
                ConsoleLog.Status("Select an order first — the demo rewrites the status of the selected row.", StatusLevel.Warning);
                return;
            }

            try
            {
                // what a careless import (or a hostile one) could put in a status field
                _orderService.SetStatus(order.Number, "On hold <script>alert('xss')</script>");
                RefreshCell(CurrentRowIndex(), colStatus.Index);

                ConsoleLog.Add("status of " + order.Number + " now contains markup — CellFormatting encodes it, the badge shows the text, nothing runs");
                ConsoleLog.Status("The unsafe status is displayed as text: WebUtility.HtmlEncode in CellFormatting is what makes AllowHtml safe.", StatusLevel.Warning);
                new Toast("The markup is shown as text, not executed.", "icon-warning")
                { AutoCloseDelay = 4000, Alignment = ContentAlignment.TopRight }.Show();
            }
            catch (OrderServiceException ex)
            {
                ReportServiceFailure(ex);
            }
        }

        private void chkSimulateFailure_CheckedChanged(object sender, EventArgs e)
        {
            ConsoleLog.Control(chkSimulateFailure.Name);

            _orderService.SimulateFailure = chkSimulateFailure.Checked;
            _orderCache.ClearError();

            ConsoleLog.Add(chkSimulateFailure.Checked
                ? "OrderService.SimulateFailure = true — the next call throws; nothing on screen is destroyed"
                : "OrderService.SimulateFailure = false — the service answers again (recovery: press Load orders)");
            ConsoleLog.Status(chkSimulateFailure.Checked
                ? "The orders service will fail on the next call."
                : "The orders service is available again — press Load orders.", chkSimulateFailure.Checked ? StatusLevel.Warning : StatusLevel.Ok);
        }

        // ------------------------------------------------------------------------------------------------------------
        // Business calls — the only three places this screen talks to the service
        // ------------------------------------------------------------------------------------------------------------

        private void OpenOrder(string number)
        {
            if (string.IsNullOrEmpty(number))
            {
                ConsoleLog.Status("That row is still loading — try again in a moment.", StatusLevel.Warning);
                return;
            }

            try
            {
                var order = _orderService.GetOrder(number);
                if (order == null)
                {
                    ConsoleLog.Status("Order " + number + " is no longer available. Refresh the list.", StatusLevel.Warning);
                    return;
                }

                ShowSelectedOrder(order);
                ConsoleLog.Status("Order " + order.Number + " · " + order.Customer + " · " + order.Status +
                                  " · due " + order.DueDate.ToString("d") + " · " + order.Total.ToString("C2"), StatusLevel.Ok);
                new Toast("Opened " + order.Number + " — " + order.Customer, "icon-info")
                { AutoCloseDelay = 3000, Alignment = ContentAlignment.TopRight }.Show();
            }
            catch (OrderServiceException ex)
            {
                ReportServiceFailure(ex);
            }
        }

        /// <summary>
        /// The one place a due date is written, whether the value came from the in-cell editor or from the command
        /// row. Accepted → the cell keeps the new value; rejected → the stored value goes back and the user is told
        /// why; the service unreachable → a friendly message and the grid is left exactly as it was.
        /// </summary>
        private void ApplyDueDate(string number, DateTime dueDate, int rowIndex, DateTime storedDueDate)
        {
            try
            {
                var result = _orderService.UpdateDueDate(number, dueDate);

                if (result.Accepted)
                {
                    WriteDueDateCell(rowIndex, dueDate);
                    ConsoleLog.Add("✓ " + result.Message);
                    ConsoleLog.Status(result.Message, StatusLevel.Ok);
                    new Toast(result.Message, "icon-check")
                    { AutoCloseDelay = 3000, Alignment = ContentAlignment.TopRight }.Show();
                }
                else
                {
                    WriteDueDateCell(rowIndex, storedDueDate);      // the edit is undone on screen, not just refused
                    ConsoleLog.Add("✗ rejected edit — " + result.Message);
                    ConsoleLog.Status(result.Message, StatusLevel.Error);
                    new Toast(result.Message, "icon-warning")
                    { AutoCloseDelay = 5000, Alignment = ContentAlignment.TopRight }.Show();
                }
            }
            catch (OrderServiceException ex)
            {
                WriteDueDateCell(rowIndex, storedDueDate);
                ReportServiceFailure(ex);
            }
        }

        /// <summary>
        /// Puts a value back on screen. On the bound path the cell is written and the binding carries it to the row.
        /// On the virtual path there is no cell object to write: the change lives in the service, so the cache is
        /// dropped and the visible block is read again — a page fetch you can watch in the Event log, which is exactly
        /// what a write does to a cached read model. The guard keeps <c>CellEndEdit</c> off our own write.
        /// </summary>
        private void WriteDueDateCell(int rowIndex, DateTime value)
        {
            if (rowIndex < 0)
                return;

            _writingCell = true;
            try
            {
                if (ordersGrid.VirtualMode)
                    ReloadVirtualRows("a due date changed");
                else if (rowIndex < ordersGrid.Rows.Count)
                    ordersGrid.Rows[rowIndex].Cells[colDueDate.Index].Value = value;
            }
            finally
            {
                _writingCell = false;
            }
        }

        /// <summary>Re-reads one cell after the model behind it changed.</summary>
        private void RefreshCell(int rowIndex, int columnIndex)
        {
            if (rowIndex < 0)
                return;

            if (ordersGrid.VirtualMode)
            {
                ReloadVirtualRows("a status changed");
            }
            else if (rowIndex < ordersGrid.Rows.Count)
            {
                var order = ordersGrid.Rows[rowIndex].DataBoundItem as OrderRow;
                if (order != null)
                    ordersGrid.Rows[rowIndex].Cells[columnIndex].Value = OrderColumns.ValueOf(order, columnIndex);
            }
        }

        /// <summary>
        /// Throws the cached pages away and makes the grid ask for the visible block again. Dropping the whole cache
        /// after a write is the safe default: a page is only a snapshot, and a stale row is worse than one fetch.
        /// </summary>
        private void ReloadVirtualRows(string reason)
        {
            var count = ordersGrid.RowCount;

            _orderCache.Reset(_filter);
            ordersGrid.RowCount = 0;
            ordersGrid.RowCount = count;

            ConsoleLog.Add("virtual path re-read (" + reason + ") — the cache was dropped, the visible block is fetched again");
        }

        /// <summary>One friendly sentence for the user; the type and the message go to the Event log only.</summary>
        private void ReportServiceFailure(Exception ex)
        {
            ConsoleLog.Add("✗ the orders service did not answer — " + ex.GetType().Name);
            ConsoleLog.Add("   " + ex.Message);
            ReportServiceFailure((string)null);
        }

        private void ReportServiceFailure(string logged)
        {
            if (!string.IsNullOrEmpty(logged))
                ConsoleLog.Add("✗ the orders service did not answer — " + logged);

            ConsoleLog.Status("The orders service is not answering. The list on screen is unchanged — clear the simulated failure and press Load orders.", StatusLevel.Error);
            new Toast("Orders could not be loaded. Please try again.", "icon-error")
            { AutoCloseDelay = 5000, Alignment = ContentAlignment.TopRight }.Show();

            lblCacheState.Text = "Service unavailable — showing the last good result";
        }

        // ------------------------------------------------------------------------------------------------------------
        // Row helpers — one place that turns a grid row index into an order, on either path
        // ------------------------------------------------------------------------------------------------------------

        private int CurrentRowIndex()
        {
            // CurrentRow works on both paths: in virtual mode the grid still hands out a row object for the current
            // cell (DataGridViewRow.IsVirtual), it simply has no bound item behind it.
            var row = ordersGrid.CurrentRow;
            if (row != null)
                return row.Index;

            return ordersGrid.SelectedRows.Count > 0 ? ordersGrid.SelectedRows[0].Index : -1;
        }

        private OrderRow CurrentOrder() => OrderAt(CurrentRowIndex());

        private OrderRow OrderAt(int rowIndex)
        {
            if (rowIndex < 0)
                return null;

            if (ordersGrid.VirtualMode)
                return _orderCache.RowAt(rowIndex);

            if (rowIndex >= ordersGrid.Rows.Count)
                return null;

            return ordersGrid.Rows[rowIndex].DataBoundItem as OrderRow;
        }

        private string OrderNumberAt(int rowIndex)
        {
            var order = OrderAt(rowIndex);
            return order == null ? null : order.Number;
        }
    }
}
