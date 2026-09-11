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
    /// The <b>Orders</b> grid. The bound path puts a filtered working set through <c>ordersSource</c> into
    /// <c>ordersGrid</c> with explicit columns; the virtual path hands the grid a row count and answers
    /// <c>CellValueNeeded</c> from <see cref="OrderCache"/>. The filter strip switches between them. Every business
    /// decision happens in <see cref="OrderService"/>.
    /// </summary>
    public partial class DataGridViewPage : UserControl, ISection
    {
        private readonly OrderService _orderService = new OrderService();
        private readonly OrderCache _orderCache;
        private readonly OrderFilter _filter = new OrderFilter();

        /// <summary>How many rows the bound path materialises before it asks the user to narrow the filter.</summary>
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

            LoadOrders();
        }

        /// <inheritdoc/>
        public string Title => "DataGridView";

        /// <inheritdoc/>
        public void RefreshSection()
        {
            LoadOrdersAsync();
        }

        // ------------------------------------------------------------------------------------------------------------
        // Set-up
        // ------------------------------------------------------------------------------------------------------------

        private void ConfigureGrid()
        {
            ordersGrid.AutoGenerateColumns = false;

            // the custom editor: a MonthCalendar edits the due-date cells
            colDueDate.Editor = dueDateCalendar;

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
        /// Docking is applied from the last child to the first, so the two strips go behind the grid (they span the
        /// card) and the Fill grid comes to the front (it takes what is left).
        /// </summary>
        private void ComposeGrid()
        {
            pnlFilterStrip.SendToBack();
            pnlGridStatus.SendToBack();
            ordersGrid.BringToFront();
        }

        // ------------------------------------------------------------------------------------------------------------
        // Loading
        // ------------------------------------------------------------------------------------------------------------

        /// <summary>The loading state: the grid shows its loader and Apply is disabled while the orders load.</summary>
        private async void LoadOrdersAsync()
        {
            if (_loading)
                return;

            _loading = true;
            SetBusy(true);
            ShellStatus.Show("Loading orders…", StatusLevel.Warning);
            Application.Update(this);

            try
            {
                await Task.Delay(400);
                LoadOrders();
            }
            finally
            {
                SetBusy(false);
                _loading = false;
                Application.Update(this);
            }
        }

        /// <summary>Reads the filter strip and rebuilds the grid on whichever path is selected.</summary>
        private void LoadOrders()
        {
            ReadFilterFromStrip();

            try
            {
                if (chkVirtualMode.Checked)
                    ShowVirtualOrders();
                else
                    ShowBoundOrders();

                _lastRefresh = DateTime.Now;
            }
            catch (OrderServiceException)
            {
                ReportServiceFailure();
            }

            UpdateStatusStrip();
        }

        private void ReadFilterFromStrip()
        {
            _filter.Search = txtSearch.Text ?? "";
            _filter.Status = cboStatus.SelectedItem as string ?? OrderFilter.AllStatuses;
        }

        /// <summary>The bound path: a filtered working set, handed to the BindingSource.</summary>
        private void ShowBoundOrders()
        {
            LeaveVirtualMode();

            var page = _orderService.GetOrders(_filter, BoundRowLimit);
            _boundMatchCount = page.TotalCount;
            BindOrders(page.Rows);

            if (page.Rows.Count == 0)
            {
                ShellStatus.Show("No orders match " + _filter.Describe() + ".", StatusLevel.Warning);
            }
            else if (page.TotalCount > page.Rows.Count)
            {
                ShellStatus.Show("Showing the first " + page.Rows.Count + " of " + page.TotalCount.ToString("N0") +
                                 " matching orders — narrow the filter, or tick Virtual mode to scroll them all.", StatusLevel.Warning);
            }
            else
            {
                ShellStatus.Show(page.Rows.Count + " orders · " + _filter.Describe(), StatusLevel.Ok);
            }
        }

        private void BindOrders(IEnumerable<OrderRow> rows)
        {
            ordersGrid.AutoGenerateColumns = false;
            ordersSource.DataSource = rows.ToList();
            ordersGrid.DataSource = ordersSource;
        }

        /// <summary>The virtual path: the grid is told how many rows exist and asks for the values it shows.</summary>
        private void ShowVirtualOrders()
        {
            ordersGrid.DataSource = null;
            ordersSource.DataSource = null;

            _orderCache.Reset(_filter);

            ordersGrid.VirtualMode = true;
            colDueDate.ReadOnly = true;     // the cache is a read model; edits stay on the bound path

            var count = _orderService.Count(_filter);
            ordersGrid.RowCount = count;

            if (count == 0)
                ShellStatus.Show("No orders match " + _filter.Describe() + ".", StatusLevel.Warning);
            else
                ShellStatus.Show(count.ToString("N0") + " orders (virtual mode) · " + _filter.Describe(), StatusLevel.Ok);
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
            btnApply.Enabled = !busy;
        }

        // ------------------------------------------------------------------------------------------------------------
        // The status strip
        // ------------------------------------------------------------------------------------------------------------

        private void UpdateStatusStrip()
        {
            lblRowCount.Text = ordersGrid.VirtualMode
                ? "Rows: " + ordersGrid.RowCount.ToString("N0")
                : "Rows: " + ordersSource.Count.ToString("N0") + " of " + _boundMatchCount.ToString("N0");

            lblCacheState.Text = ordersGrid.VirtualMode ? _orderCache.Describe() : "";

            lblLastRefresh.Text = "Last refresh: " + (_lastRefresh.HasValue ? _lastRefresh.Value.ToString("HH:mm:ss") : "—");
        }

        private void ShowSelectedOrder(OrderRow order)
        {
            if (order == null)
            {
                lblSelectedOrder.Text = "Selected: —";
                ShellStatus.Record(null);
                return;
            }

            lblSelectedOrder.Text = "Selected: " + order.Number;
            ShellStatus.Record(order.Number);
        }

        // ------------------------------------------------------------------------------------------------------------
        // Grid events
        // ------------------------------------------------------------------------------------------------------------

        /// <summary>Display only: the raw status becomes an encoded badge; the model value is untouched.</summary>
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

        /// <summary>The large-data read path: one cell, answered by the cache.</summary>
        private void ordersGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            e.Value = _orderCache.GetValue(e.RowIndex, e.ColumnIndex);
        }

        /// <summary>The client names the block of rows it is about to read, so the pages are fetched ahead of it.</summary>
        private void ordersGrid_DataRead(object sender, DataGridViewDataReadEventArgs e)
        {
            if (!_orderCache.Prefetch(e.FirstIndex, e.LastIndex))
                ReportServiceFailure();

            UpdateStatusStrip();
        }

        private void ordersGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (_loading || _writingCell)
                return;

            ShowSelectedOrder(CurrentOrder());
        }

        /// <summary>
        /// The command column. Wisej.NET 4.1 raises <c>CellClick</c> where WinForms has <c>CellContentClick</c>; the
        /// handler keeps the lab's name and calls the service by the row's order ID.
        /// </summary>
        private void ordersGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != colOpen.Index)
                return;

            ShellStatus.Control(colOpen.Name);
            OpenOrder(OrderNumberAt(e.RowIndex));
        }

        /// <summary>Copies the stored due date into the MonthCalendar editor.</summary>
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

            dueDateCalendar.SelectionStart = order.DueDate;
            dueDateCalendar.SelectionEnd = order.DueDate;

            ShellStatus.Control(colDueDate.Name);
            ShellStatus.Record(order.Number);
        }

        /// <summary>Takes the edited date out of the MonthCalendar and lets the service decide.</summary>
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

            var edited = dueDateCalendar.SelectionStart.Date;
            if (edited == original.Date)
                return;

            ApplyDueDate(number, edited, e.RowIndex, original);
        }

        // ------------------------------------------------------------------------------------------------------------
        // Filter strip
        // ------------------------------------------------------------------------------------------------------------

        private void btnApply_Click(object sender, EventArgs e)
        {
            ShellStatus.Control(btnApply.Name);
            LoadOrdersAsync();
        }

        private void chkVirtualMode_CheckedChanged(object sender, EventArgs e)
        {
            ShellStatus.Control(chkVirtualMode.Name);
            LoadOrdersAsync();
        }

        private void chkSimulateFailure_CheckedChanged(object sender, EventArgs e)
        {
            _orderService.SimulateFailure = chkSimulateFailure.Checked;
            _orderCache.ClearError();
        }

        // ------------------------------------------------------------------------------------------------------------
        // Business calls
        // ------------------------------------------------------------------------------------------------------------

        private void OpenOrder(string number)
        {
            if (string.IsNullOrEmpty(number))
            {
                ShellStatus.Show("That row is still loading — try again in a moment.", StatusLevel.Warning);
                return;
            }

            try
            {
                var order = _orderService.GetOrder(number);
                if (order == null)
                {
                    ShellStatus.Show("Order " + number + " is no longer available. Refresh the list.", StatusLevel.Warning);
                    return;
                }

                ShowSelectedOrder(order);
                ShellStatus.Show("Order " + order.Number + " · " + order.Customer + " · " + order.Status +
                                 " · due " + order.DueDate.ToString("d") + " · " + order.Total.ToString("C2"), StatusLevel.Ok);
                new Toast("Opened " + order.Number + " — " + order.Customer, "icon-info")
                { AutoCloseDelay = 3000, Alignment = ContentAlignment.TopRight }.Show();
            }
            catch (OrderServiceException)
            {
                ReportServiceFailure();
            }
        }

        /// <summary>
        /// Writes a due date through the service. Accepted → the cell keeps the new value; rejected → the stored value
        /// goes back and the user is told why; the service unreachable → the grid is left as it was.
        /// </summary>
        private void ApplyDueDate(string number, DateTime dueDate, int rowIndex, DateTime storedDueDate)
        {
            try
            {
                var result = _orderService.UpdateDueDate(number, dueDate);

                if (result.Accepted)
                {
                    WriteDueDateCell(rowIndex, dueDate);
                    ShellStatus.Show(result.Message, StatusLevel.Ok);
                    new Toast(result.Message, "icon-check")
                    { AutoCloseDelay = 3000, Alignment = ContentAlignment.TopRight }.Show();
                }
                else
                {
                    WriteDueDateCell(rowIndex, storedDueDate);
                    ShellStatus.Show(result.Message, StatusLevel.Error);
                    new Toast(result.Message, "icon-warning")
                    { AutoCloseDelay = 5000, Alignment = ContentAlignment.TopRight }.Show();
                }
            }
            catch (OrderServiceException)
            {
                WriteDueDateCell(rowIndex, storedDueDate);
                ReportServiceFailure();
            }
        }

        /// <summary>Puts a value back on screen; the guard keeps <c>CellEndEdit</c> off our own write.</summary>
        private void WriteDueDateCell(int rowIndex, DateTime value)
        {
            if (rowIndex < 0)
                return;

            _writingCell = true;
            try
            {
                if (ordersGrid.VirtualMode)
                    ReloadVirtualRows();
                else if (rowIndex < ordersGrid.Rows.Count)
                    ordersGrid.Rows[rowIndex].Cells[colDueDate.Index].Value = value;
            }
            finally
            {
                _writingCell = false;
            }
        }

        /// <summary>Throws the cached pages away and makes the grid ask for the visible block again.</summary>
        private void ReloadVirtualRows()
        {
            var count = ordersGrid.RowCount;

            _orderCache.Reset(_filter);
            ordersGrid.RowCount = 0;
            ordersGrid.RowCount = count;
        }

        /// <summary>One friendly sentence for the user; no exception text.</summary>
        private void ReportServiceFailure()
        {
            ShellStatus.Show("The orders service is not answering. The list on screen is unchanged — try again in a moment.", StatusLevel.Error);
            new Toast("Orders could not be loaded. Please try again.", "icon-error")
            { AutoCloseDelay = 5000, Alignment = ContentAlignment.TopRight }.Show();

            lblCacheState.Text = "Service unavailable — showing the last good result";
        }

        // ------------------------------------------------------------------------------------------------------------
        // Row helpers
        // ------------------------------------------------------------------------------------------------------------

        private int CurrentRowIndex()
        {
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
