using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OrderDesk.Dialogs;
using OrderDesk.Domain;
using OrderDesk.Legacy;
using OrderDesk.Views;
using Wisej.Web;

namespace OrderDesk
{
    /// <summary>
    /// Module 5 · DataGridView, Validation and Performance.
    ///
    /// Left, card A:  the ported orders grid over the production-sized store (200,000 orders):
    ///                the naive port ("load every row", the desktop habit) next to the optimized
    ///                screen — a default filter, server-side sort and VirtualMode with a block cache,
    ///                so only the rows the viewport asks for ever leave the server.
    /// Left, card B:  validation taken out of the form — OrderValidator runs on the server for the
    ///                edit dialog (ErrorProvider, field by field), for a batch and for a rule call
    ///                with no form at all; the desktop's single blocking MessageBox rule for comparison.
    /// Right:         the migration log and the performance card — every fetch that crossed the wire
    ///                with rows, server time and an estimate of the bytes it cost the browser.
    /// </summary>
    public partial class MainPage : Page
    {
        /// <summary>Rows the grid asks for per block in VirtualMode (≈ one viewport plus a margin).</summary>
        private const int BlockSize = 50;

        /// <summary>Bytes a rendered grid row costs on the wire, measured from the cell text plus framing.</summary>
        private const int RowFramingBytes = 96;

        private readonly OrderQueryService _queryService = new OrderQueryService();               // OrderStore.Large
        private readonly OrderService _orderService = new OrderService(OrderStore.Large);          // ✓ the same business logic, on the big store
        private readonly CustomerService _customerService = new CustomerService();
        private readonly Dictionary<int, IReadOnlyList<Order>> _blocks = new Dictionary<int, IReadOnlyList<Order>>();
        private readonly Random _random = new Random(5);

        private OrderQuery _query = new OrderQuery { Status = OrderStatus.Open, SortBy = OrderSort.Date, Descending = true, Take = BlockSize };
        private bool _virtual;
        private int _rowCount;
        private int _blockFetches;
        private double _blockMs;
        private long _blockBytes;

        private int _batchDone;
        private int _batchInvalid;
        private double _batchMs;
        private int _measureDone;
        private double _measureMs;
        private long _measureBytes;

        public MainPage()
        {
            InitializeComponent();
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            trace.Add(TraceKind.Server, "startup", "Default.json → OrderDesk.Program.Main → Application.MainPage = new MainPage()");
            trace.Add(TraceKind.Server, "session", $"new browser session {Short(Application.SessionId)} · {Application.SessionCount} session(s) in this process");

            // The first session of the process pays the one-time seed; every later session finds it ready.
            bool seeded = OrderStore.IsSeeded;
            var watch = Stopwatch.StartNew();
            int count = OrderStore.Large.Count;
            watch.Stop();
            trace.Add(TraceKind.Server, "OrderStore.Large",
                seeded
                    ? $"{count:N0} orders already seeded by an earlier session (shared, like a database)"
                    : $"{count:N0} orders generated in {OrderStore.SeedElapsed.TotalMilliseconds:N0} ms — once per process, deterministic");

            comboStatus.SelectedIndex = 1;           // Open — the default filter the users actually work in
            comboSort.SelectedIndex = 0;             // Date, newest first
            numNaiveRows.Value = 20000;

            LoadOptimized("page load");
            ShowValidationDetail("OrderValidator: Customer required · PO number required (≤ 20 chars) · at least one line · quantity ≥ 1 · unit price ≥ 0 · total ≥ 0.\nThe rule is a class in Domain/, not code inside a form: the dialog, a batch and an API all call the same method.");
        }

        #region Card A · the orders grid: naive port vs. filtered + virtual rows

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

        private void buttonApply_Click(object sender, EventArgs e)
        {
            trace.Add(TraceKind.FromClient, "toolbar", $"search='{textSearch.Text.Trim()}' status={comboStatus.SelectedItem} sort={comboSort.SelectedItem}");
            LoadOptimized("filter / sort changed");
        }

        private void buttonOptimized_Click(object sender, EventArgs e) => LoadOptimized("Optimized clicked");

        /// <summary>✓ The web-safe screen: filter and sort on the server, RowCount from a count, rows per block on demand.</summary>
        private void LoadOptimized(string reason)
        {
            ReadToolbar();
            _blocks.Clear();
            _blockFetches = 0; _blockMs = 0; _blockBytes = 0;

            var watch = Stopwatch.StartNew();
            _rowCount = _queryService.Count(_query);
            var summary = _queryService.Summarize(_query);
            watch.Stop();

            // Rebind as a virtual grid: RowCount is the only thing that crosses now; cells come from CellValueNeeded.
            gridOrders.BeginUpdate();
            gridOrders.Rows.Clear();
            gridOrders.VirtualMode = true;
            _virtual = true;
            gridOrders.RowCount = _rowCount;
            gridOrders.EndUpdate();

            trace.Add(TraceKind.Server, "OrderQueryService.Count", $"{_query.Describe()} → {_rowCount:N0} rows in {watch.Elapsed.TotalMilliseconds:N1} ms ({reason})");
            trace.Add(TraceKind.ToClient, "grid.RowCount", $"{_rowCount:N0} (VirtualMode: rows are fetched {BlockSize} at a time as the viewport moves)");
            labelFooter.Text = $"Σ {summary.Count:N0} rows match · total {summary.Total:N2} · summarized on the server in {summary.Elapsed.TotalMilliseconds:N1} ms";

            AddMeasurement("Optimized · filter + virtual rows", Math.Min(BlockSize, _rowCount), watch.Elapsed.TotalMilliseconds, EstimateBytes(Math.Min(BlockSize, _rowCount)), "count + Σ; first block on demand");
            Ui.HideBanner(labelBanner);
            Ui.SetStatus(labelStatus, $"virtual · {_rowCount:N0} rows · {BlockSize}/block", Ui.Ok);
            if (_rowCount > 0)
                ShowSelectedOrder();
        }

        /// <summary>The grid asks for one cell; the block that holds it is fetched once and memoized.</summary>
        private void gridOrders_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (!_virtual || e.RowIndex < 0 || e.RowIndex >= _rowCount)
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
                long bytes = EstimateBytes(block);
                _blockBytes += bytes;
                trace.Add(TraceKind.ToClient, "block fetched", $"rows {start:N0}–{start + block.Count - 1:N0} of {page.Total:N0} · {page.Elapsed.TotalMilliseconds:N1} ms{(page.FromCache ? " (ordered set memoized)" : " (filter + sort ran)")} · ≈{bytes:N0} bytes");
                labelPerfCounts.Text = $"{_blockFetches} blocks · {_blockMs:N0} ms · ≈{_blockBytes / 1024.0:N0} KB so far";
            }
            int offset = rowIndex - start;
            return offset < block.Count ? block[offset] : null;
        }

        /// <summary>✕ The desktop habit, executed on purpose: clone + sort the whole table and hand rows to the grid.</summary>
        private void buttonNaive_Click(object sender, EventArgs e)
        {
            int limit = (int)numNaiveRows.Value;
            trace.Add(TraceKind.FromClient, "Naive port clicked", $"grid limit {limit:N0} rows (the desktop had no limit)");

            var watch = Stopwatch.StartNew();
            var all = DesktopGridHabits.LoadWholeTable(_orderService);      // ✕ OrdersForm.ReloadGrid: GetOrders() → 200,000 clones, sorted
            var serverMs = watch.Elapsed.TotalMilliseconds;

            gridOrders.BeginUpdate();
            gridOrders.Rows.Clear();
            gridOrders.VirtualMode = false;
            _virtual = false;
            int added = 0;
            long bytes = 0;
            foreach (var order in all)
            {
                if (added >= limit) break;
                int index = gridOrders.Rows.Add(order.Id, order.CustomerName, order.Total, order.Status.ToString(), order.CreatedOn.ToString("yyyy-MM-dd"));
                gridOrders.Rows[index].Tag = order;
                bytes += EstimateBytes(order);
                added++;
            }
            gridOrders.EndUpdate();
            watch.Stop();

            labelFooter.Text = $"✕ {all.Count:N0} rows materialized on the server · {added:N0} handed to the grid · Σ computed in the browser? no — nobody asked, the desktop never did";
            trace.Add(TraceKind.Boundary, "load-everything", $"OrderService.GetOrders() cloned + sorted {all.Count:N0} rows in {serverMs:N0} ms; {added:N0} rows built in {watch.Elapsed.TotalMilliseconds - serverMs:N0} ms ⇒ ≈{bytes / 1024.0 / 1024.0:N1} MB for this one session");
            AddMeasurement($"Naive · load all ({added:N0} rows)", added, watch.Elapsed.TotalMilliseconds, bytes, all.Count == added ? "the whole table" : $"of {all.Count:N0} cloned");
            Ui.ShowBanner(labelBanner,
                $"✕ Naive port: every session clones and sorts {all.Count:N0} orders on the server and ships ≈{bytes / 1024.0 / 1024.0:N1} MB to a browser that can show 12 rows. " +
                "Multiply by every open session. Same screen, same data — click Optimized to fix it.", Ui.BannerKind.Error);
            Ui.SetStatus(labelStatus, $"naive · {added:N0} rows in the browser", Ui.Error);
        }

        private void gridOrders_SelectionChanged(object sender, EventArgs e)
        {
            if (gridOrders.CurrentRow != null)
                ShowSelectedOrder();
        }

        private Order SelectedOrder
        {
            get
            {
                var row = gridOrders.CurrentRow;
                if (row == null) return null;
                return _virtual ? OrderAt(row.Index) : row.Tag as Order;
            }
        }

        private void ShowSelectedOrder()
        {
            var order = SelectedOrder;
            labelSelected.Text = order == null ? "" : $"{order.Id} · {order.CustomerName} · {order.PoNumber} · owner {(string.IsNullOrEmpty(order.Owner) ? "—" : order.Owner)} · {order.Lines.Count} line(s) · {order.Total:N2}";
        }

        private void gridOrders_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                EditOrder(SelectedOrder);
        }

        private void buttonEdit_Click(object sender, EventArgs e) => EditOrder(SelectedOrder);

        #endregion

        #region Card B · validation on the server (deliverable 3)

        /// <summary>The ported edit workflow: the dialog collects, the validator decides, the service saves.</summary>
        private void EditOrder(Order order)
        {
            if (order == null)
            {
                Ui.Toast("Select an order first.", MessageBoxIcon.Warning);
                return;
            }

            trace.Add(TraceKind.FromClient, "edit", $"order {order.Id} → EditOrderDialog (ShowDialog, disposed in the callback)");
            var dialog = new EditOrderDialog(order, _customerService.GetCustomers());
            dialog.Validated += (s, args) =>
                trace.Add(args.Result.HasErrors ? TraceKind.Boundary : TraceKind.Server, "OrderValidator.Validate", $"order {dialog.Order.Id}: {args.Result.Describe()}");

            dialog.ShowDialog((form, result) =>
            {
                try
                {
                    if (result == DialogResult.OK)
                    {
                        var saved = _orderService.Save(dialog.Order);          // ✓ business logic reused: CalculateOrderTotal + repository
                        trace.Add(TraceKind.Server, "OrderService.Save", $"order {saved.Id} · total {saved.Total:N2} · ordered result set invalidated");
                        Ui.Toast($"Order {saved.Id} saved.");
                        ShowValidationDetail($"Saved order {saved.Id}: {saved.CustomerName} · {saved.PoNumber} · {saved.Lines.Count} line(s) · total {saved.Total:N2}.\nThe validator passed on the server; the grid re-fetches its blocks so the row shows the new values.");
                        if (_virtual) LoadOptimized("after save"); else buttonNaive_Click(this, EventArgs.Empty);
                    }
                    else
                    {
                        trace.Add(TraceKind.FromClient, "edit", $"order {order.Id} cancelled — nothing saved");
                    }
                }
                finally
                {
                    form.Dispose();
                }
            });
        }

        private void buttonNewOrder_Click(object sender, EventArgs e)
        {
            // A new order with nothing filled in: press Save in the dialog and watch the ErrorProvider.
            var order = new Order { Status = OrderStatus.Open, CreatedOn = DateTime.Today };
            trace.Add(TraceKind.FromClient, "new order", "empty order → dialog; press Save to see the field-level messages");
            EditOrder(order);
        }

        /// <summary>✓ The rule with no form at all — what a batch import or a web API calls.</summary>
        private void buttonBadOrder_Click(object sender, EventArgs e)
        {
            var bad = new Order
            {
                Id = 0,
                PoNumber = "PO-NUMBER-THAT-IS-FAR-TOO-LONG-FOR-THE-FIELD",
                Status = OrderStatus.Open,
                CreatedOn = DateTime.Today
            };
            bad.Lines.Add(new OrderLine { Sku = "WJ-DEV-SEAT", Description = "Developer seat", Quantity = 0, UnitPrice = -1m });

            var watch = Stopwatch.StartNew();
            var result = OrderValidator.Validate(bad);
            watch.Stop();

            trace.Add(TraceKind.Server, "OrderValidator.Validate", $"no form, no dialog: {result.Describe()} ({watch.Elapsed.TotalMilliseconds:N2} ms)");
            ShowValidationDetail("Rule called directly (no form):\n" + string.Join("\n", result.Errors.Select(x => $"  {x.Key,-10} {x.Value}").Concat(result.General.Select(g => $"  order      {g}"))));
            Ui.ShowBanner(labelBanner, $"✓ {result.Errors.Count + result.General.Count} problems found by the same class the dialog uses — field by field, on the server, without a MessageBox. A client-side check would be a nice touch, never the security boundary.", Ui.BannerKind.Ok);
            Ui.SetStatus(labelStatus, "validator: reusable, server-side", Ui.Ok);
        }

        /// <summary>✕ The desktop rule: one message, inside the form, blocking.</summary>
        private void buttonDesktopRule_Click(object sender, EventArgs e)
        {
            var bad = new Order { PoNumber = "", Status = OrderStatus.Open };
            string message = DesktopGridHabits.ValidateInsideTheForm(bad);      // ✕ EditOrderDialog.saveButton_Click: "Select a customer."
            trace.Add(TraceKind.Boundary, "desktop rule", $"ValidateInsideTheForm → \"{message}\" — one rule, one blocking MessageBox, unreachable from a batch or an API");
            Ui.ShowBanner(labelBanner, "✕ The desktop rule lived inside saveButton_Click: it found one problem (\"Select a customer.\"), said nothing about the missing PO number or the empty line, and blocked the user with a MessageBox. Compare with the field-level result on the left.", Ui.BannerKind.Warn);
            MessageBox.Show(message, "LegacyOrderDesk", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>Progress path: the same rule over 2,000 stored orders, 200 per tick.</summary>
        private void buttonBatch_Click(object sender, EventArgs e)
        {
            if (timerBatch.Enabled)
            {
                timerBatch.Stop();
                buttonBatch.Text = "Batch 2,000";
                trace.Add(TraceKind.Server, "batch", $"stopped by the user after {_batchDone:N0} rows");
                return;
            }
            _batchDone = 0; _batchInvalid = 0; _batchMs = 0;
            buttonBatch.Text = "■ Stop";
            trace.Add(TraceKind.Server, "batch", "validating 2,000 orders (all statuses) with OrderValidator, 200 per Timer tick");
            timerBatch.Start();
        }

        private void timerBatch_Tick(object sender, EventArgs e)
        {
            var page = _queryService.Page(new OrderQuery { SortBy = OrderSort.Id, Descending = false, Skip = _batchDone, Take = 200 });
            var watch = Stopwatch.StartNew();
            foreach (var order in page.Items)
            {
                if (OrderValidator.Validate(order).HasErrors)
                    _batchInvalid++;
            }
            watch.Stop();
            _batchMs += watch.Elapsed.TotalMilliseconds;
            _batchDone += page.Items.Count;

            ShowValidationDetail($"Batch: {_batchDone:N0} / 2,000 orders validated · {_batchInvalid} invalid · {_batchMs:N1} ms of rule time so far.\nThe same OrderValidator the dialog uses, with no form on the screen — that is the reuse the lesson asks for.");
            Ui.SetStatus(labelStatus, $"batch {_batchDone:N0}/2,000", Ui.Warn);

            if (_batchDone >= 2000 || page.Items.Count == 0)
            {
                timerBatch.Stop();
                buttonBatch.Text = "Batch 2,000";
                trace.Add(TraceKind.Server, "batch", $"done: {_batchDone:N0} rows, {_batchInvalid} invalid, {_batchMs:N1} ms rule time");
                Ui.SetStatus(labelStatus, $"batch done · {_batchInvalid} invalid of {_batchDone:N0}", Ui.Ok);
            }
        }

        private void ShowValidationDetail(string text) => labelValidationDetail.Text = text;

        #endregion

        #region Right · performance card

        private void AddMeasurement(string scenario, int rows, double serverMs, long bytes, string note)
        {
            int index = gridPerf.Rows.Add(scenario, rows.ToString("N0"), serverMs.ToString("N0"), FormatBytes(bytes), note);
            gridPerf.Rows[index].Cells[3].Style.ForeColor = bytes > 1024 * 1024 ? Ui.Error : Ui.Ok;
            gridPerf.Rows[index].Cells[3].Style.Font = Ui.SmallBold;

        }

        /// <summary>Progress path: ten random viewport jumps, timed — what scrolling a big grid costs the server.</summary>
        private void buttonMeasure_Click(object sender, EventArgs e)
        {
            if (!_virtual)
            {
                Ui.Toast("Switch to the optimized grid first — the naive grid has nothing left to fetch.", MessageBoxIcon.Warning);
                return;
            }
            if (timerMeasure.Enabled) return;
            _measureDone = 0; _measureMs = 0; _measureBytes = 0;
            trace.Add(TraceKind.Server, "measure", "10 random block fetches against the current filter + sort");
            timerMeasure.Start();
        }

        private void timerMeasure_Tick(object sender, EventArgs e)
        {
            int start = _rowCount <= BlockSize ? 0 : _random.Next(_rowCount / BlockSize) * BlockSize;
            var page = _queryService.Page(_query.WithPage(start, BlockSize));
            long bytes = EstimateBytes(page.Items);
            _measureDone++;
            _measureMs += page.Elapsed.TotalMilliseconds;
            _measureBytes += bytes;
            AddMeasurement($"Interaction {_measureDone} · rows {start:N0}+", page.Items.Count, page.Elapsed.TotalMilliseconds, bytes, page.FromCache ? "memoized order" : "filter + sort");
            Ui.SetStatus(labelStatus, $"measuring {_measureDone}/10", Ui.Warn);

            if (_measureDone >= 10)
            {
                timerMeasure.Stop();
                string summary = $"10 interactions · avg {_measureMs / 10:N1} ms · ≈{_measureBytes / 10 / 1024.0:N0} KB each";
                AddMeasurement("Average of 10", BlockSize, _measureMs / 10, _measureBytes / 10, "ready for the migration log");
                trace.Add(TraceKind.Server, "measure", summary + " → docs/GridPerformanceNotes.md");
                Ui.SetStatus(labelStatus, summary, Ui.Ok);
            }
        }

        private void buttonSecondSession_Click(object sender, EventArgs e)
        {
            trace.Add(TraceKind.FromClient, "second session", "Application.Navigate(url, _blank) — the same store, its own grid, its own blocks");
            Application.Navigate(Application.Url, "_blank");
        }

        private void buttonClear_Click(object sender, EventArgs e) => trace.Clear();

        #endregion

        #region helpers

        private static long EstimateBytes(IEnumerable<Order> rows) => rows.Sum(EstimateBytes);

        private static long EstimateBytes(int rowCount) => rowCount * (RowFramingBytes + 48);

        private static long EstimateBytes(Order order) =>
            RowFramingBytes + order.Id.ToString().Length + (order.CustomerName ?? "").Length + order.Total.ToString("N2").Length + order.Status.ToString().Length + 10;

        private static string FormatBytes(long bytes) =>
            bytes >= 1024 * 1024 ? $"{bytes / 1024.0 / 1024.0:N1} MB" : bytes >= 1024 ? $"{bytes / 1024.0:N0} KB" : $"{bytes} B";

        private static string Short(string sessionId) => string.IsNullOrEmpty(sessionId) ? "?" : sessionId.Substring(0, Math.Min(8, sessionId.Length));

        #endregion
    }
}
