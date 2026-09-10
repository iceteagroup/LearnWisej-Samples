using System;
using System.Diagnostics;
using System.Globalization;
using OrderDesk.Domain;
using OrderDesk.Services;
using OrderDesk.Shared;
using Wisej.Web;

namespace OrderDesk.Pages
{
    /// <summary>
    /// The same orders grid, fixed for the web: a filter toolbar (Status defaults to Open — how the desk
    /// really finds orders), a search box, a sort combo, <c>VirtualMode = true</c> with
    /// <c>RowCount = service.Count(query)</c>, cells filled from an <see cref="OrderPageCache"/> that calls
    /// <c>OrderService.Search(query)</c> one block at a time, grid tool buttons, and a summary footer computed
    /// on the server over every matching row.
    /// </summary>
    public sealed class OptimizedOrdersPanel : Panel
    {
        public const int BlockSize = 50;

        private readonly Panel toolbar;
        private readonly TextBox searchBox;
        private readonly ComboBox statusCombo;
        private readonly ComboBox sortCombo;
        private readonly Button applyButton;
        private readonly DataGridView grid;
        private readonly Label footer;

        private OrderService _service;
        private OrderPageCache _cache;

        /// <summary>Trace sink (name, payload) — MainPage plugs the TracePanel in.</summary>
        public Action<string, string> Log { get; set; } = (n, p) => { };
        /// <summary>The toolbar or the refresh tool asked for a reload with the current query.</summary>
        public event EventHandler ApplyRequested;
        /// <summary>Row double-click or the edit tool.</summary>
        public event Action<Order> EditRequested;

        public OptimizedOrdersPanel()
        {
            this.BackColor = Palette.CardBackground;

            // ── filter toolbar (the enhancement the video adds after the basic bind) ──
            toolbar = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = Palette.PanelBackground };
            searchBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 8),
                Size = new System.Drawing.Size(190, 28),
                Watermark = "Search customer, owner, PO, order…",
                ToolTipText = "Server-side search: OrderQuery.Search (Enter to apply)",
            };
            searchBox.KeyDown += SearchBox_KeyDown;
            statusCombo = new ComboBox
            {
                Location = new System.Drawing.Point(208, 8),
                Size = new System.Drawing.Size(110, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                ToolTipText = "Default filter: Open — ask how the user really finds orders",
            };
            statusCombo.Items.AddRange(new object[] { "Any status", "Open", "InProgress", "Shipped", "Invoiced", "Hold" });
            statusCombo.SelectedIndex = 1; // Open — the default filter of the desk
            sortCombo = new ComboBox
            {
                Location = new System.Drawing.Point(326, 8),
                Size = new System.Drawing.Size(118, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                ToolTipText = "Server-side sort: OrderQuery.SortBy / Descending",
            };
            sortCombo.Items.AddRange(new object[] { "Order ↓", "Order ↑", "Customer ↑", "Total ↓", "Total ↑", "Status ↑", "Date ↓", "Owner ↑" });
            sortCombo.SelectedIndex = 0;
            applyButton = new Button
            {
                Location = new System.Drawing.Point(452, 8),
                Size = new System.Drawing.Size(62, 28),
                Text = "Apply",
                ToolTipText = "Count + first block for this query",
            };
            applyButton.Click += (s, e) => ApplyRequested?.Invoke(this, EventArgs.Empty);
            toolbar.Controls.Add(searchBox);
            toolbar.Controls.Add(statusCombo);
            toolbar.Controls.Add(sortCombo);
            toolbar.Controls.Add(applyButton);

            // ── the grid: VirtualMode, values pulled per block ──
            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BorderStyle = BorderStyle.None,
                VirtualMode = true,
                BlockSize = BlockSize,
                NoDataMessage = "Not loaded — click  Optimized load ✓  (filter Open, virtual rows)",
            };
            var colId = new DataGridViewTextBoxColumn { Name = "colId", HeaderText = "Order", Width = 80 };
            var colCustomer = new DataGridViewTextBoxColumn { Name = "colCustomer", HeaderText = "Customer", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
            var colOwner = new DataGridViewTextBoxColumn { Name = "colOwner", HeaderText = "Owner", Width = 80 };
            var colTotal = new DataGridViewTextBoxColumn { Name = "colTotal", HeaderText = "Total", Width = 110 };
            colTotal.DefaultCellStyle.Format = "C2";
            colTotal.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            var colStatus = new DataGridViewTextBoxColumn { Name = "colStatus", HeaderText = "Status", Width = 90 };
            grid.Columns.AddRange(new DataGridViewColumn[] { colId, colCustomer, colOwner, colTotal, colStatus });
            grid.CellValueNeeded += Grid_CellValueNeeded;
            grid.DataRead += Grid_DataRead;
            grid.CellDoubleClick += Grid_CellDoubleClick;

            // grid tool buttons (＋ ⤓ ⟳ in the video) — theme icon names, compile-checked
            grid.Tools.Add("refresh", "icon-refresh").ToolTipText = "Reload this query";
            grid.Tools.Add("top", "icon-up").ToolTipText = "Scroll to the first row";
            grid.Tools.Add("edit", "icon-edit").ToolTipText = "Edit the selected order (validation dialog)";
            grid.ToolClick += Grid_ToolClick;

            footer = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Bottom,
                Height = 26,
                Padding = new Padding(10, 0, 10, 0),
                ForeColor = Palette.Ink,
                Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold),
                Text = "Σ Total — computed on the server over the whole filtered set (not loaded yet)",
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
            };

            this.Controls.Add(grid);
            this.Controls.Add(toolbar);
            this.Controls.Add(footer);
        }

        public void Attach(OrderService service) => _service = service;

        public int RowsInGrid => grid.RowCount;
        public int BlocksFetched => _cache?.BlocksFetched ?? 0;

        /// <summary>The order under the current row (from the page cache), else the first row.</summary>
        public Order SelectedOrder
        {
            get
            {
                if (_cache == null) return null;
                int index = grid.CurrentRow != null ? grid.CurrentRow.Index : 0;
                return _cache.Get(index) ?? _cache.Get(0);
            }
        }

        /// <summary>The toolbar as a server-side query.</summary>
        public OrderQuery BuildQuery()
        {
            var q = new OrderQuery { Search = searchBox.Text?.Trim() };
            var status = statusCombo.SelectedItem as string;
            if (!string.IsNullOrEmpty(status) && status != "Any status")
                q.Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), status);
            switch (sortCombo.SelectedIndex)
            {
                case 1: q.SortBy = "Id"; q.Descending = false; break;
                case 2: q.SortBy = "Customer"; q.Descending = false; break;
                case 3: q.SortBy = "Total"; q.Descending = true; break;
                case 4: q.SortBy = "Total"; q.Descending = false; break;
                case 5: q.SortBy = "Status"; q.Descending = false; break;
                case 6: q.SortBy = "Date"; q.Descending = true; break;
                case 7: q.SortBy = "Owner"; q.Descending = false; break;
                default: q.SortBy = "Id"; q.Descending = true; break;
            }
            return q;
        }

        /// <summary>Count + RowCount + first block + server-side summary, all timed.</summary>
        public LoadMetrics Load()
        {
            if (_service == null) throw new InvalidOperationException("The 200,000-order store is still seeding.");

            var query = BuildQuery();
            long before = GC.GetTotalMemory(false);
            var sw = Stopwatch.StartNew();

            _cache = new OrderPageCache(_service, query, BlockSize, Log);
            int count = _cache.Count();

            grid.BeginUpdate();
            try
            {
                grid.RowCount = 0;
                grid.RowCount = count;                 // row shells only — no values live in the grid
                Log("grid.RowCount = " + count.ToString("N0"), "VirtualMode — the grid holds row shells, values come from CellValueNeeded");
                grid.ClearSelection();
            }
            finally
            {
                grid.EndUpdate();
            }

            // the first paint: the block(s) the client will ask for first
            if (count > 0) _cache.Prefetch(0, Math.Min(count, BlockSize) - 1);
            sw.Stop();
            long after = GC.GetTotalMemory(false);

            double sumMs = 0;
            var sum = count > 0 ? _cache.SumTotal(out sumMs) : 0m;
            string filter = query.Status.HasValue ? query.Status.ToString() : "any status";
            footer.Text = "Σ Total (" + filter + ")  " + sum.ToString("C2") + "   ·   " + count.ToString("N0") + " rows · virtual · block " + BlockSize
                        + "   ·   summary " + sumMs.ToString("0", CultureInfo.InvariantCulture) + " ms on the server";
            Log("Σ Total (" + filter + ")", sum.ToString("C2") + " over " + count.ToString("N0") + " rows · " + sumMs.ToString("0.0", CultureInfo.InvariantCulture)
                + " ms — server-side; a client sum over 50 visible rows would be wrong");

            return new LoadMetrics
            {
                Mode = "optimized",
                Query = query.ToString(),
                RowsFetched = _cache.RowsFetched,
                RowsInGrid = count,
                RowsShipped = _cache.RowsFetched,
                ServerMs = sw.Elapsed.TotalMilliseconds,
                MemoryDeltaBytes = after - before,
            };
        }

        public void ScrollToTop()
        {
            if (grid.RowCount > 0) grid.ScrollRowIntoView(0);
        }

        private void Grid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            var o = _cache?.Get(e.RowIndex);
            if (o == null) return;
            switch (e.ColumnIndex)
            {
                case 0: e.Value = o.Id; break;
                case 1: e.Value = o.CustomerName; break;
                case 2: e.Value = o.Owner ?? "—"; break;
                case 3: e.Value = o.Total; break;
                case 4: e.Value = o.Status.ToString(); break;
            }
        }

        private void Grid_DataRead(object sender, DataGridViewDataReadEventArgs e)
        {
            Log("← DataRead", "client needs rows " + e.FirstIndex.ToString("N0") + "–" + e.LastIndex.ToString("N0") + " → page cache");
            _cache?.Prefetch(e.FirstIndex, e.LastIndex);
        }

        private void Grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || _cache == null) return;
            var o = _cache.Get(e.RowIndex);
            if (o != null) EditRequested?.Invoke(o);
        }

        private void Grid_ToolClick(object sender, ToolClickEventArgs e)
        {
            Log("← ToolClick", "grid tool \"" + e.Tool.Name + "\"");
            switch (e.Tool.Name)
            {
                case "refresh": ApplyRequested?.Invoke(this, EventArgs.Empty); break;
                case "top": ScrollToTop(); break;
                case "edit": if (SelectedOrder != null) EditRequested?.Invoke(SelectedOrder); break;
            }
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) ApplyRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
