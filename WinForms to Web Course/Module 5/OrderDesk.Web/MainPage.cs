using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using OrderDesk.Dialogs;
using OrderDesk.Domain;
using OrderDesk.Pages;
using OrderDesk.Services;
using OrderDesk.Shared;
using Wisej.Web;

namespace OrderDesk
{
    /// <summary>
    /// Module 5 — DataGridView, Validation &amp; Performance. The Orders card holds the same grid twice:
    /// the naive port (✕ <c>DataSource = service.GetAll()</c>, the WinForms habit) and the optimized one
    /// (✓ default filter Open + <c>VirtualMode</c> + server-side <see cref="OrderQuery"/>, one block at a time).
    /// A 200,000-order private store is seeded in the background so the page opens instantly; every load is
    /// timed with a Stopwatch and written to the trace. Validation runs in <see cref="OrderValidator"/> on
    /// the server and shows up as <c>ErrorProvider</c> field messages in <see cref="EditOrderDialog"/>.
    /// </summary>
    public partial class MainPage : Page
    {
        private const int NaiveCap = 20_000;

        private readonly BigOrderData _data = new BigOrderData();
        private readonly PerformanceLog _perf = new PerformanceLog();
        private OrderService _service;
        private int _measureStep = -1;
        private int _lastNaiveCap;
        private bool _optimizedLoaded;

        public MainPage()
        {
            InitializeComponent();
            optimizedPanel.Log = (name, payload) => trace.Server(name, payload);
        }

        // ───────────────────────────── startup: seed 200k orders in the background ─────────────────────────────

        private void MainPage_Load(object sender, EventArgs e)
        {
            trace.Server("Program.Main", "Application.MainPage = new MainPage()  · session " + Application.SessionId.Substring(0, 8));
            trace.Server("Application.StartTask", "seeding OrderStore.Create(200000, 42) in the background — the page opened without waiting");
            trace.Finding("data volume", "OrdersForm bound every row of a LAN database; on the web that habit is what Module 5 measures and replaces");
            SetStatus(Palette.Warn, "● working — seeding the 200,000-order store");

            Application.StartTask(() =>
            {
                Exception error = null;
                try { _data.Build(); }
                catch (Exception ex) { error = ex; }

                if (this.IsDisposed) return;
                OnStoreReady(error);
                Application.Update(this);
            });
        }

        private void OnStoreReady(Exception error)
        {
            if (error != null)
            {
                storeLabel.ForeColor = Palette.Bad;
                storeLabel.Text = "● store failed: " + error.Message;
                trace.Fail("OrderStore.Create", error.GetType().Name + ": " + error.Message);
                SetStatus(Palette.Bad, "● alarm — the store could not be seeded");
                return;
            }

            _service = _data.Service;
            optimizedPanel.Attach(_service);
            storeLabel.ForeColor = Palette.Good;
            storeLabel.Text = "● " + BigOrderData.OrderCount.ToString("N0") + " orders ready · seeded in " + _data.SeedMs + " ms";
            trace.Ok("OrderStore.Create(200000, 42)", BigOrderData.OrderCount.ToString("N0") + " orders ready in " + _data.SeedMs + " ms · private store, not OrderStore.Shared() · heap "
                + PayloadEstimate.Human(_data.HeapAfterSeedBytes));
            trace.Out("Application.Update(this)", "store ready — buttons enabled, pushed over the WebSocket from the task thread");
            SetButtons(true);
            SetStatus(Palette.Good, "● idle — store ready; pick a path below");
        }

        // ───────────────────────────── naive port (✕ failure / measure) ─────────────────────────────

        private void naiveButton_Click(object sender, EventArgs e)
        {
            trace.In("click", "Naive load (20k) ✕");
            HideBanner();
            var m = RunNaive(NaiveCap, "naive 20k");
            trace.Fail("naive port", "capped at " + NaiveCap.ToString("N0") + " rows for the demo — Bind all 200k ✕ binds the real table");
            trace.Finding("full-table load", "desktop habit: SELECT * then bind. Fine on a LAN with one user; on the web every session pays it on open");
            ShowBanner(Palette.WarnSoft, "Naive port: " + m.RowsInGrid.ToString("N0") + " of " + m.RowsFetched.ToString("N0") + " rows bound in "
                + Ms(m.ServerMs) + " ms · Δ heap " + PayloadEstimate.Signed(m.MemoryDeltaBytes) + " · ~" + PayloadEstimate.Human(m.EstimatedPayloadBytes)
                + " if every row ships — and that is only a tenth of the table.");
            SetStatus(Palette.Warn, "● idle — naive grid bound (" + m.RowsInGrid.ToString("N0") + " rows)");
        }

        private void bindAllButton_Click(object sender, EventArgs e)
        {
            trace.In("click", "Bind all 200k ✕");
            HideBanner();
            trace.Fail("warning", "binding all " + BigOrderData.OrderCount.ToString("N0") + " rows for real — expect seconds and a large heap; the server pays for every session that does this");
            SetStatus(Palette.Warn, "● working — binding 200,000 rows…");
            var m = RunNaive(int.MaxValue, "naive 200k");
            trace.Finding("multiply by every session", "the video's 8.4 s / 96 MB screen: the same list per user, per tab, per refresh");
            ShowBanner(Palette.BadSoft, "✖ " + m.RowsInGrid.ToString("N0") + " rows bound in " + Ms(m.ServerMs) + " ms · Δ heap " + PayloadEstimate.Signed(m.MemoryDeltaBytes)
                + " · ~" + PayloadEstimate.Human(m.EstimatedPayloadBytes) + " if every row ships — multiply by every session.");
            SetStatus(Palette.Bad, "● alarm — the whole table is bound in one session's grid");
        }

        /// <summary>GetAll() (the whole table) → cap → DataSource = list, timed. Shared by the buttons and Measure.</summary>
        private LoadMetrics RunNaive(int cap, string mode)
        {
            tabs.SelectedIndex = 0;

            var sw = Stopwatch.StartNew();
            var all = _service.GetAll();
            sw.Stop();
            trace.Server("OrderService.GetAll()", all.Count.ToString("N0") + " rows in " + Ms(sw.Elapsed.TotalMilliseconds) + " ms — the whole table, exactly what OrdersForm.ReloadGrid did");

            var rows = cap < all.Count ? all.Take(cap).ToList() : all;
            var m = naivePanel.Bind(rows, mode, all.Count, sw.Elapsed.TotalMilliseconds);
            _lastNaiveCap = cap;

            trace.Server("grid.DataSource = list", rows.Count.ToString("N0") + " rows bound in " + Ms(m.ServerMs) + " ms total · Δ heap " + PayloadEstimate.Signed(m.MemoryDeltaBytes)
                + " · " + rows.Count.ToString("N0") + " DataGridViewRow objects on the server");
            trace.Out("DataGridView rows", rows.Count.ToString("N0") + " rows · ~" + PayloadEstimate.Human(m.EstimatedPayloadBytes) + " if every row is shipped (" + PayloadEstimate.Formula + "); Wisej streams blocks, so check DevTools for the real bytes");
            ShowMetrics(m, good: false);
            _perf.Add(m);
            return m;
        }

        // ───────────────────────────── optimized (✓ success) ─────────────────────────────

        private void optimizedButton_Click(object sender, EventArgs e)
        {
            trace.In("click", "Optimized load ✓");
            HideBanner();
            var m = RunOptimized();
            trace.Finding("virtual rows + default filter", "VirtualMode + RowCount = Count(query) + Search(Skip/Take) per block; Status defaults to Open because that is how the desk finds orders");
            ShowBanner(Palette.GoodSoft, "✓ Optimized: " + m.Query + " → " + m.RowsInGrid.ToString("N0") + " virtual rows, " + m.RowsFetched + " fetched for the first paint in "
                + Ms(m.ServerMs) + " ms · ~" + PayloadEstimate.Human(m.EstimatedPayloadBytes) + ". Scroll the grid: every new block is one Search call in the trace.");
            SetStatus(Palette.Good, "● idle — optimized grid ready (" + m.RowsInGrid.ToString("N0") + " rows, " + optimizedPanel.BlocksFetched + " block(s) fetched)");
        }

        private void optimizedPanel_ApplyRequested(object sender, EventArgs e)
        {
            if (_service == null) return;
            trace.In("toolbar", "Apply / refresh → " + optimizedPanel.BuildQuery());
            HideBanner();
            var m = RunOptimized();
            SetStatus(Palette.Good, "● idle — query applied (" + m.RowsInGrid.ToString("N0") + " rows)");
        }

        private LoadMetrics RunOptimized()
        {
            tabs.SelectedIndex = 1;
            var m = optimizedPanel.Load();
            _optimizedLoaded = true;
            trace.Out("DataGridView rows", m.RowsShipped + " rows · ~" + PayloadEstimate.Human(m.EstimatedPayloadBytes) + " — one block of " + OptimizedOrdersPanel.BlockSize + "; the rest stays on the server until scrolled into view");
            trace.Ok("optimized load", "first paint " + Ms(m.ServerMs) + " ms · " + m.RowsFetched + " of " + m.RowsInGrid.ToString("N0") + " rows fetched · Δ heap " + PayloadEstimate.Signed(m.MemoryDeltaBytes));
            ShowMetrics(m, good: true);
            _perf.Add(m);
            return m;
        }

        // ───────────────────────────── Measure (progress path, Timer) ─────────────────────────────

        private void measureButton_Click(object sender, EventArgs e)
        {
            trace.In("click", "Measure");
            HideBanner();
            _perf.Clear();
            _measureStep = 0;
            SetButtons(false);
            SetStatus(Palette.Warn, "● working — measuring: naive 20k vs optimized, 5 runs each");
            trace.Server("Measure", "Timer(700 ms) alternates naive 20k and optimized, 5 runs each; the summary becomes the performance notes");
            measureTimer.Start();
        }

        private void measureTimer_Tick(object sender, EventArgs e)
        {
            if (_service == null || _measureStep < 0) { measureTimer.Stop(); return; }

            int run = _measureStep / 2 + 1;
            if (_measureStep % 2 == 0)
            {
                var m = RunNaive(NaiveCap, "naive 20k");
                trace.Server("measure " + run + "/5", "naive 20k → " + Ms(m.ServerMs) + " ms");
                SetStatus(Palette.Warn, "● working — run " + run + "/5 naive 20k: " + Ms(m.ServerMs) + " ms");
            }
            else
            {
                var m = RunOptimized();
                trace.Server("measure " + run + "/5", "optimized → " + Ms(m.ServerMs) + " ms");
                SetStatus(Palette.Warn, "● working — run " + run + "/5 optimized: " + Ms(m.ServerMs) + " ms");
            }

            _measureStep++;
            if (_measureStep < 10) return;

            measureTimer.Stop();
            _measureStep = -1;
            trace.Finding("performance notes", "server-side Stopwatch, " + BigOrderData.OrderCount.ToString("N0") + "-order store, this machine:");
            foreach (var line in _perf.Lines())
                trace.Finding("  " + line.Substring(0, 10).Trim(), line.Substring(10).Trim());
            WritePerformanceNotes();
            var naive = _perf.Line("naive 20k");
            var opt = _perf.Line("optimized");
            ShowBanner(Palette.GoodSoft, "Measured — " + (naive ?? "") + "  |  " + (opt ?? "") + ". Copy these into docs/performance-notes.md.");
            SetButtons(true);
            SetStatus(Palette.Good, "● idle — measurement finished (see ★ performance notes in the trace)");
        }

        private void WritePerformanceNotes()
        {
            try
            {
                var dir = Path.Combine(Application.StartupPath, "App_Data");
                Directory.CreateDirectory(dir);
                var path = Path.Combine(dir, "performance-notes.md");
                File.WriteAllText(path, _perf.Markdown("OrderDesk · Module 5 · measured grid loads"));
                trace.Server("File.WriteAllText", "App_Data/performance-notes.md (under the project folder, never C:\\Orders)");
            }
            catch (Exception ex)
            {
                trace.Fail("performance-notes.md", ex.GetType().Name + ": " + ex.Message);
            }
        }

        // ───────────────────────────── validation: dialog + ErrorProvider ─────────────────────────────

        private void editButton_Click(object sender, EventArgs e)
        {
            trace.In("click", "Edit selected → validate");
            HideBanner();
            var order = tabs.SelectedIndex == 1 ? optimizedPanel.SelectedOrder : naivePanel.SelectedOrder;
            if (order == null)
            {
                order = _service.Find(1042);
                trace.Server("selection", "no grid loaded yet — using order 1042 Northwind Traders");
            }
            EditOrder(order, "button");
        }

        private void naivePanel_EditRequested(Order order) { trace.In("CellDoubleClick", "naive grid · order " + order.Id); EditOrder(order, "naive grid"); }
        private void optimizedPanel_EditRequested(Order order) { trace.In("CellDoubleClick / tool", "optimized grid · order " + order.Id); EditOrder(order, "optimized grid"); }

        private void EditOrder(Order order, string source)
        {
            if (_service == null || order == null) return;
            trace.Server("EditOrderDialog", "new EditOrderDialog(order " + order.Id + " · " + order.CustomerName + " · " + order.Total.ToString("C2") + ") from " + source + " — inside using, ShowDialog() blocks this handler");

            var dlg = new EditOrderDialog(order, _service);
            using (dlg)
            {
                dlg.Trace += (name, payload) => trace.Server(name, payload);
                dlg.ValidationChecked += result =>
                {
                    if (!result.HasErrors) { trace.Ok("OrderValidator.Validate", "valid — same rule OrderService.Save runs again"); return; }
                    foreach (var err in result.Errors)
                        trace.Fail("ErrorProvider → " + err.Key, err.Value);
                };

                var result = dlg.ShowDialog();
                trace.Server("ShowDialog()", "returned DialogResult." + result + " — the handler resumed here");

                if (result == DialogResult.OK)
                {
                    try
                    {
                        _service.Save(dlg.Order);              // ✓ validates again — the service is the boundary
                        trace.Ok("OrderService.Save", "order " + dlg.Order.Id + " saved · " + dlg.Order.CustomerName + " · owner " + (dlg.Order.Owner ?? "—") + " · " + dlg.Order.Total.ToString("C2") + " · " + dlg.Order.Status);
                        Notify.Saved("Order " + dlg.Order.Id + " saved.");
                        trace.Out("Toast", "\"Order " + dlg.Order.Id + " saved.\" — non-blocking (Module 3 policy)");
                        ReloadActiveGrid();
                        ShowBanner(Palette.GoodSoft, "✓ Order " + dlg.Order.Id + " saved through OrderService.Save — the grid reloaded from the store.");
                        SetStatus(Palette.Good, "● idle — order " + dlg.Order.Id + " saved");
                    }
                    catch (ValidationException ex)
                    {
                        foreach (var err in ex.Result.Errors) trace.Fail("ValidationException → " + err.Key, err.Value);
                        ShowBanner(Palette.BadSoft, "✖ OrderService.Save rejected the order: " + ex.Result);
                        SetStatus(Palette.Bad, "● alarm — save rejected");
                    }
                }
                else
                {
                    SetStatus(Palette.Good, "● idle — edit cancelled, nothing saved");
                }
            }
            trace.Ok("dialog disposed", "dlg.IsDisposed = " + dlg.IsDisposed + " after the using block (closed dialogs are not disposed by themselves)");
        }

        private void ReloadActiveGrid()
        {
            if (tabs.SelectedIndex == 1 && _optimizedLoaded) { trace.Server("reload", "optimized grid — new page cache for the same query"); RunOptimized(); }
            else if (_lastNaiveCap > 0) { trace.Server("reload", "naive grid — GetAll() again, like OrdersForm.ReloadGrid"); RunNaive(_lastNaiveCap, _lastNaiveCap == NaiveCap ? "naive 20k" : "naive 200k"); }
        }

        private void saveInvalidButton_Click(object sender, EventArgs e)
        {
            trace.In("click", "Save invalid ✕");
            HideBanner();

            var original = _service.Find(1039);
            var bad = original.Clone();
            bad.Owner = null;
            bad.Lines[0].Quantity = 99_999;
            trace.Server("order 1039 clone", "Owner = null · Lines[0].Quantity = 99,999 → Total " + bad.Total.ToString("C2") + " vs credit limit " + bad.Customer.CreditLimit.ToString("C2") + " (Adventure Works)");

            try
            {
                _service.Save(bad);
                trace.Fail("OrderService.Save", "unexpected: the invalid order was saved");
            }
            catch (ValidationException ex)
            {
                trace.Fail("OrderService.Save", "ValidationException — " + ex.Result.Errors.Count + " field(s)");
                foreach (var err in ex.Result.Errors)
                    trace.Fail("  " + err.Key, err.Value);
                ShowBanner(Palette.BadSoft, "✖ ValidationException from OrderService.Save — " + string.Join("  ·  ", ex.Result.Errors.Select(p => p.Key + ": " + p.Value)));
            }

            var check = _service.Find(1039);
            trace.Ok("store unchanged", "order 1039 still " + check.Total.ToString("C2") + " · owner " + check.Owner + " · " + check.Status);
            trace.Finding("validation boundary", "the rule lives in OrderValidator and the service enforces it; the dialog shows the same messages inline through ErrorProvider — client checks are a courtesy, never the boundary");
            SetStatus(Palette.Bad, "● alarm — invalid save rejected on the server; store unchanged");
        }

        // ───────────────────────────── helpers ─────────────────────────────

        private void tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            trace.In("tab", tabs.SelectedIndex == 0 ? "Naive port ✕" : "Optimized ✓");
        }

        private void ShowMetrics(LoadMetrics m, bool good)
        {
            var color = good ? Palette.Good : Palette.Bad;
            perfMode.Text = m.Mode;
            perfMode.ForeColor = color;
            perfFetched.Text = m.RowsFetched.ToString("N0");
            perfFetched.ForeColor = color;
            perfInGrid.Text = m.RowsInGrid.ToString("N0") + (good ? " virtual" : "");
            perfInGrid.ForeColor = color;
            perfMs.Text = Ms(m.ServerMs) + " ms";
            perfMs.ForeColor = color;
            perfMemory.Text = PayloadEstimate.Signed(m.MemoryDeltaBytes);
            perfMemory.ForeColor = color;
            perfPayload.Text = "~" + PayloadEstimate.Human(m.EstimatedPayloadBytes);
            perfPayload.ForeColor = color;
        }

        private void SetButtons(bool enabled)
        {
            foreach (var b in new[] { naiveButton, bindAllButton, optimizedButton, measureButton, editButton, saveInvalidButton })
                b.Enabled = enabled;
        }

        private void SetStatus(System.Drawing.Color color, string text)
        {
            statusLabel.ForeColor = color;
            statusLabel.Text = text;
        }

        private void ShowBanner(System.Drawing.Color back, string text)
        {
            bannerLabel.BackColor = back;
            bannerLabel.Text = text;
            bannerLabel.Visible = true;
        }

        private void HideBanner() => bannerLabel.Visible = false;

        private static string Ms(double ms) => ms.ToString(ms < 10 ? "0.0" : "0", CultureInfo.InvariantCulture);
    }
}
