using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OrderDesk.Dialogs;
using OrderDesk.Domain;
using OrderDesk.Views;
using Wisej.Web;

namespace OrderDesk
{
    /// <summary>
    /// The ported orders grid over the production-sized store (200,000 orders): a default filter,
    /// server-side sort and VirtualMode with a block cache, so only the rows the viewport asks for
    /// ever leave the server. Editing goes through EditOrderDialog, validated on the server.
    /// </summary>
    public partial class MainPage : Page
    {
        /// <summary>Rows the grid asks for per block in VirtualMode (≈ one viewport plus a margin).</summary>
        private const int BlockSize = 50;

        /// <summary>Bytes a rendered grid row costs on the wire, estimated from the cell text plus framing.</summary>
        private const int RowFramingBytes = 96;

        private readonly OrderQueryService _queryService = new OrderQueryService();
        private readonly OrderService _orderService = new OrderService(OrderStore.Large);
        private readonly CustomerService _customerService = new CustomerService();
        private readonly Dictionary<int, IReadOnlyList<Order>> _blocks = new Dictionary<int, IReadOnlyList<Order>>();

        private OrderQuery _query = new OrderQuery { Status = OrderStatus.Open, SortBy = OrderSort.Date, Descending = true, Take = BlockSize };
        private int _rowCount;
        private double _loadMs;
        private int _blockFetches;
        private double _blockMs;
        private long _blockBytes;

        public MainPage()
        {
            InitializeComponent();
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            comboStatus.SelectedIndex = 1;           // Open — the default filter the users actually work in
            comboSort.SelectedIndex = 0;             // Date, newest first
            LoadOrders();
        }

        #region The orders grid: server-side filter + sort, virtual rows

        /// <summary>Reads the toolbar into the query (filter + sort; the page is decided by the grid).</summary>
        private void ReadToolbar()
        {
            _query = new OrderQuery
            {
                Text = textSearch.Text,
                Status = StatusFromCombo(),
                SortBy = SortFromCombo(out bool descending),
                Descending = descending,
                Take = BlockSize
            };
        }

        private OrderStatus? StatusFromCombo()
        {
            var text = comboStatus.SelectedItem as string;
            if (string.IsNullOrEmpty(text) || text == "All") return null;
            return (OrderStatus)Enum.Parse(typeof(OrderStatus), text);
        }

        private OrderSort SortFromCombo(out bool descending)
        {
            switch (comboSort.SelectedIndex)
            {
                case 1: descending = true; return OrderSort.Total;
                case 2: descending = false; return OrderSort.Customer;
                case 3: descending = false; return OrderSort.Status;
                case 4: descending = true; return OrderSort.Id;
                default: descending = true; return OrderSort.Date;
            }
        }

        private void buttonApply_Click(object sender, EventArgs e) => LoadOrders();

        /// <summary>Filter and sort on the server, RowCount from a count, rows per block on demand.</summary>
        private void LoadOrders()
        {
            ReadToolbar();
            _blocks.Clear();
            _blockFetches = 0; _blockMs = 0; _blockBytes = 0;

            var watch = Stopwatch.StartNew();
            _rowCount = _queryService.Count(_query);
            var summary = _queryService.Summarize(_query);
            watch.Stop();
            _loadMs = watch.Elapsed.TotalMilliseconds;

            gridOrders.BeginUpdate();
            gridOrders.Rows.Clear();
            gridOrders.RowCount = _rowCount;
            gridOrders.EndUpdate();

            labelFooter.Text = $"Σ {summary.Count:N0} orders · total {summary.Total:N2}";
            RefreshPerformance();
        }

        /// <summary>The grid asks for one cell; the block that holds it is fetched once and memoized.</summary>
        private void gridOrders_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _rowCount)
                return;

            var order = OrderAt(e.RowIndex);
            if (order == null)
                return;

            switch (e.ColumnIndex)
            {
                case 0: e.Value = order.Id; break;
                case 1: e.Value = order.CustomerName; break;
                case 2: e.Value = order.Total; break;
                case 3: e.Value = order.Status.ToString(); break;
                case 4: e.Value = order.CreatedOn.ToString("yyyy-MM-dd"); break;
            }
        }

        private Order OrderAt(int rowIndex)
        {
            int start = rowIndex - rowIndex % BlockSize;
            if (!_blocks.TryGetValue(start, out var block))
            {
                var page = _queryService.Page(_query.WithPage(start, BlockSize));
                block = page.Items;
                _blocks[start] = block;

                _blockFetches++;
                _blockMs += page.Elapsed.TotalMilliseconds;
                _blockBytes += EstimateBytes(block);
                RefreshPerformance();
            }
            int offset = rowIndex - start;
            return offset < block.Count ? block[offset] : null;
        }

        private void RefreshPerformance()
        {
            int fetched = _blocks.Values.Sum(b => b.Count);
            labelPerfCounts.Text =
                $"Rows matching  {_rowCount:N0}\n" +
                $"Rows fetched   {fetched:N0} (virtual)\n" +
                $"Load (server)  {_loadMs:N0} ms\n" +
                $"Block fetches  {_blockFetches} · {_blockMs:N0} ms\n" +
                $"≈ Payload      {FormatBytes(_blockBytes)}";
        }

        private Order SelectedOrder
        {
            get
            {
                var row = gridOrders.CurrentRow;
                return row == null ? null : OrderAt(row.Index);
            }
        }

        #endregion

        #region Edit workflow: the dialog collects, the validator decides, the service saves

        private void gridOrders_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                EditOrder(SelectedOrder);
        }

        private void buttonEdit_Click(object sender, EventArgs e) => EditOrder(SelectedOrder);

        private void buttonNewOrder_Click(object sender, EventArgs e)
        {
            EditOrder(new Order { Status = OrderStatus.Open, CreatedOn = DateTime.Today });
        }

        private void EditOrder(Order order)
        {
            if (order == null)
            {
                Ui.Toast("Select an order first.", MessageBoxIcon.Warning);
                return;
            }

            var dialog = new EditOrderDialog(order, _customerService.GetCustomers());
            dialog.ShowDialog((form, result) =>
            {
                try
                {
                    if (result == DialogResult.OK)
                    {
                        var saved = _orderService.Save(dialog.Order);
                        Ui.Toast($"Order {saved.Id} saved.");
                        LoadOrders();
                    }
                }
                finally
                {
                    form.Dispose();
                }
            });
        }

        #endregion

        private static long EstimateBytes(IEnumerable<Order> rows) => rows.Sum(EstimateBytes);

        private static long EstimateBytes(Order order) =>
            RowFramingBytes + order.Id.ToString().Length + (order.CustomerName ?? "").Length + order.Total.ToString("N2").Length + order.Status.ToString().Length + 10;

        private static string FormatBytes(long bytes) =>
            bytes >= 1024 * 1024 ? $"{bytes / 1024.0 / 1024.0:N1} MB" : bytes >= 1024 ? $"{bytes / 1024.0:N0} KB" : $"{bytes} B";
    }
}
