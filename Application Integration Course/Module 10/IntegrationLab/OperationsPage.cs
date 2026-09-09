using System;
using System.Globalization;
using IntegrationLab.Controls;
using IntegrationLab.Data;
using IntegrationLab.Services;
using Wisej.Web;

namespace IntegrationLab
{
    /// <summary>
    /// Operations Dashboard — Module 10 capstone page.
    ///
    /// Stats strip: Widgets live · Requests/min · Disposed cleanly · Live updates (● running / ● stopped).
    /// Left cards:  the HeatmapWidget (postback data source) and the TemperatureGauge fed by the same background task.
    /// Right card:  every message that crosses the wire, in both directions.
    /// Bottom bar:  success paths (highlight, reload, cell count), the progress path (background task),
    ///              the leak test, two failure paths (missing vendor script, malformed data) and recovery.
    /// </summary>
    public partial class OperationsPage : Page
    {
        private const int LeakCycles = 25;
        private static readonly string[] DayNames = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };

        private readonly LiveUpdateService _live;
        private HeatmapWidget _brokenWidget;      // the "missing vendor script" simulation
        private HeatmapWidget _leakWidget;        // the widget currently alive in the create/dispose test
        private int _leakCycle;
        private int _leakLoaded;

        public OperationsPage()
        {
            InitializeComponent();

            // Widget events → .NET events (the contract in action).
            this.heatmap.CellSelected += heatmap_CellSelected;
            this.heatmap.DataLoaded += heatmap_DataLoaded;
            this.heatmap.LoadFailed += heatmap_LoadFailed;
            this.heatmap.Trace += widget_Trace;

            this.gauge.ThresholdExceeded += gauge_ThresholdExceeded;
            this.gauge.RangeChanged += gauge_RangeChanged;
            this.gauge.WidgetError += gauge_WidgetError;
            this.gauge.Trace += widget_Trace;

            // Background updates: bounded (every 1500 ms, at most 40 pushes) and stopped when the page closes.
            _live = new LiveUpdateService(this,
                iteration => LoadSampleService.NextLiveBatch(iteration, this.heatmap.Days, this.heatmap.Hours),
                intervalMs: 1500, maxIterations: 40);
            _live.Updated += live_Updated;
            _live.Stopped += live_Stopped;
            this.Disposed += (s, e) => _live.Dispose();
        }

        private void OperationsPage_Load(object sender, EventArgs e)
        {
            AddTrace(TraceDirection.Server, "packages",
                $"{HeatmapWidget.VendorStylePath}, {HeatmapWidget.VendorScriptPath}  (vendor {HeatmapWidget.VendorVersion}, wrapper {HeatmapWidget.WrapperVersion})");
            AddTrace(TraceDirection.ServerToClient, "render → init(options)", this.heatmap.ToJson());
            string url = this.heatmap.PostbackUrl;
            AddTrace(TraceDirection.Server, "postback endpoint",
                (url ?? "this.getPostbackUrl()") + "&action=load  (client fetches it from init)");
            AddTrace(TraceDirection.ServerToClient, "render → init(options)", this.gauge.ToJson());

            SetHeatStatus("loading…", StatusKind.Warn);
            SetLiveStatus(false, "stopped");
            this.labelDisposedValue.Text = "—";
            RefreshStats();
            UpdateStateLabel();
        }

        #region Progress path: background task (Application.StartTask → Call → Application.Update)

        private void buttonLive_Click(object sender, EventArgs e)
        {
            if (_live.IsRunning)
            {
                _live.Stop();
                this.buttonLive.Text = "■ stopping…";
                AddTrace(TraceDirection.Server, "task", "stop requested; the loop ends within one interval");
                return;
            }

            _live.Start();
            this.buttonLive.Text = "■ Stop live updates";
            SetLiveStatus(true, "running 0/" + _live.MaxIterations);
            AddTrace(TraceDirection.Server, "Application.StartTask",
                $"bounded: one push every {_live.IntervalMs} ms, at most {_live.MaxIterations} pushes, stops on page dispose");
        }

        /// <summary>Runs inside Application.Update(this): change the widgets like a button handler would, then the push happens.</summary>
        private void live_Updated(object sender, LiveUpdateEventArgs e)
        {
            var reading = e.Reading;
            this.heatmap.SetCells(reading.Cells);                       // Call("setCells", cells) on the client
            SetGauge(reading.Temperature);                              // Options.value → update(options)
            SetLiveStatus(true, $"running {e.Iteration}/{e.MaxIterations}");
            AddTrace(TraceDirection.Server, "Application.Update(page)",
                $"push {e.Iteration}/{e.MaxIterations}: setCells + gauge {F(reading.Temperature)}°F in one flush");
            RefreshStats();
            UpdateStateLabel();
        }

        private void live_Stopped(object sender, LiveUpdateStoppedEventArgs e)
        {
            this.buttonLive.Text = "▶ Start live updates";
            SetLiveStatus(false, "stopped · " + e.Reason);
            AddTrace(TraceDirection.Server, "task stopped", $"{e.Reason} after {e.Iterations} pushes");
        }

        #endregion

        #region Success paths: server calls with and without a return value

        private void buttonPeak_Click(object sender, EventArgs e)
        {
            try
            {
                // The server decides from ITS copy of the data, then tells the client what to show.
                var peak = this.heatmap.FindPeak();
                this.heatmap.Highlight(peak.Day, peak.Hour);
                ShowBanner($"▲ Peak load: {DayName(peak.Day)} {peak.Hour:00}:00 = {F(peak.Value)} (computed on the server, highlighted on the client)", BannerKind.Info);
                AddTrace(TraceDirection.Server, "peak", $"{DayName(peak.Day)} {peak.Hour:00}:00 = {F(peak.Value)}");
            }
            catch (InvalidOperationException ex)
            {
                AddTrace(TraceDirection.Server, "rejected", ex.Message);
                ShowBanner("✖ " + ex.Message, BannerKind.Error);
            }
        }

        private void buttonReload_Click(object sender, EventArgs e)
        {
            // Recovery: fetch the good endpoint again. The server is the source of truth, the browser re-draws it.
            DisposeBrokenWidget();
            SetHeatStatus("reloading…", StatusKind.Warn);
            this.heatmap.Reload();
        }

        private async void buttonCellCount_Click(object sender, EventArgs e)
        {
            this.buttonCellCount.Enabled = false;
            try
            {
                int count = await this.heatmap.GetCellCountAsync();   // CallAsync round trip
                AddTrace(TraceDirection.Server, "CallAsync result", $"client holds {count} cells; server holds {this.heatmap.Cells.Count}");
                ShowBanner($"ⓘ getCellCount() → {count} cells on the client · {this.heatmap.Cells.Count} on the server", BannerKind.Info);
                AlertBox.Show($"The client widget holds {count} cells.", MessageBoxIcon.Information,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 3000);
            }
            finally
            {
                this.buttonCellCount.Enabled = true;
            }
        }

        #endregion

        #region Leak test: create + dispose ×25

        private void buttonLeak_Click(object sender, EventArgs e)
        {
            if (this.timerLeak.Enabled) return;
            DisposeBrokenWidget();

            // Reset the client counters the adapter maintains (init → __integrationLabCreated, dispose → __integrationLabDisposed).
            this.Eval("window.__integrationLabCreated = 0; window.__integrationLabDisposed = 0;");
            _leakCycle = 0;
            _leakLoaded = 0;
            this.labelDisposedValue.Text = "running…";
            this.labelDisposedValue.ForeColor = System.Drawing.Color.FromArgb(232, 161, 60);
            AddTrace(TraceDirection.Server, "leak test", $"create + dispose HeatmapWidget ×{LeakCycles} in the scratch panel (Timer, {this.timerLeak.Interval} ms steps)");
            this.timerLeak.Start();
        }

        private void timerLeak_Tick(object sender, EventArgs e)
        {
            if (_leakWidget == null)
            {
                if (_leakCycle >= LeakCycles)
                {
                    this.timerLeak.Stop();
                    FinishLeakTest();
                    return;
                }

                _leakCycle++;
                _leakWidget = new HeatmapWidget { Days = 3, Hours = 12, Title = $"scratch #{_leakCycle}", Dock = DockStyle.Fill };
                _leakWidget.DataLoaded += (s, a) => _leakLoaded++;   // each scratch widget loads through the postback too
                this.panelScratch.Controls.Add(_leakWidget);
                RefreshStats();
                return;
            }

            _leakWidget.Dispose();      // server dispose → client dispose(): vendor destroy + counters
            _leakWidget = null;
            RefreshStats();
        }

        private async void FinishLeakTest()
        {
            // (unverified beyond the docs) Control.EvalAsync returns the value of the evaluated script.
            int disposed = ToInt(await this.EvalAsync("window.__integrationLabDisposed"));
            int created = ToInt(await this.EvalAsync("window.__integrationLabCreated"));
            int vendorAlive = ToInt(await this.EvalAsync("(typeof VendorHeatmap !== 'undefined') ? VendorHeatmap.liveInstances() : -1"));

            bool clean = disposed == LeakCycles && created == LeakCycles && vendorAlive == 1;   // 1 = the dashboard heatmap
            this.labelDisposedValue.Text = $"{disposed}/{created}";
            this.labelDisposedValue.ForeColor = clean
                ? System.Drawing.Color.FromArgb(31, 157, 87)
                : System.Drawing.Color.FromArgb(224, 86, 59);
            AddTrace(TraceDirection.ClientToServer, "EvalAsync __integrationLabDisposed", disposed.ToString(CultureInfo.InvariantCulture));
            AddTrace(TraceDirection.Server, "leak test result",
                $"created {created}, disposed {disposed}, vendor instances alive {vendorAlive} (expected 1), scratch loads answered {_leakLoaded}");
            ShowBanner(clean
                ? $"✔ Disposed cleanly {disposed}/{created}: every vendor instance destroyed, no handlers or nodes left behind"
                : $"✖ Leak suspected: created {created}, disposed {disposed}, vendor instances alive {vendorAlive}",
                clean ? BannerKind.Info : BannerKind.Error);
            RefreshStats();
        }

        #endregion

        #region Failure paths

        private void buttonMissingVendor_Click(object sender, EventArgs e)
        {
#if DEBUG
            DisposeBrokenWidget();
            SetHeatStatus("fault expected…", StatusKind.Warn);

            // A second wrapper whose Packages omit vendor-heatmap.js. The guard clause in heatmap-init.js must throw
            // "VendorHeatmap not loaded — check Packages order." and the adapter reports it as error {phase:"init"}.
            _brokenWidget = HeatmapWidget.CreateWithMissingVendorScript();
            _brokenWidget.Title = "broken";
            _brokenWidget.Dock = DockStyle.Fill;
            _brokenWidget.Trace += widget_Trace;
            _brokenWidget.LoadFailed += broken_LoadFailed;
            this.panelScratch.Controls.Add(_brokenWidget);
            AddTrace(TraceDirection.Server, "simulate", "second HeatmapWidget created WITHOUT the vendor-heatmap package → guard clause must throw");
            RefreshStats();
#else
            ShowBanner("The failure simulations are compiled in DEBUG only.", BannerKind.Error);
#endif
        }

        private void broken_LoadFailed(object sender, HeatmapErrorEventArgs e)
        {
            AddTrace(TraceDirection.Server, "LoadFailed fired in C#", $"phase={e.Phase} (broken widget)");
            ShowBanner($"✖ init failed: {e.Message}  → fix: add Packages {{ vendor-heatmap }} before the InitScript runs; DevTools shows it under integrationlab.controls.HeatmapWidget.js",
                BannerKind.Alarm);
            SetHeatStatus("fault (second widget)", StatusKind.Error);

            // Undo the simulation (the global is hidden only while the broken widget initialises), then drop the widget.
            this.Eval("if (window.__restoreVendorHeatmap) window.__restoreVendorHeatmap();");
            DisposeBrokenWidget();
            RefreshStats();
        }

        private void buttonMalformed_Click(object sender, EventArgs e)
        {
#if DEBUG
            // The endpoint answers action=corrupt with invalid JSON (DEBUG only): the vendor throws inside load(),
            // the adapter reports ONE "error" event, the page stays alive and "Reload data" recovers.
            SetHeatStatus("fault expected…", StatusKind.Warn);
            this.heatmap.LoadWithActionForTesting("corrupt");
#else
            ShowBanner("The failure simulations are compiled in DEBUG only.", BannerKind.Error);
#endif
        }

        private void DisposeBrokenWidget()
        {
            if (_brokenWidget == null) return;
            _brokenWidget.Dispose();
            _brokenWidget = null;
        }

        #endregion

        #region Widget → .NET events

        private void heatmap_CellSelected(object sender, HeatmapCellEventArgs e)
        {
            AddTrace(TraceDirection.Server, "CellSelected fired in C#", $"{DayName(e.Day)} {e.Hour:00}:00 value={F(e.Value)} (client reported {F(e.ReportedValue)})");
            ShowBanner($"● {DayName(e.Day)} {e.Hour:00}:00 → load {F(e.Value)} (server value)", BannerKind.Info);
        }

        private void heatmap_DataLoaded(object sender, HeatmapLoadedEventArgs e)
        {
            AddTrace(TraceDirection.Server, "DataLoaded fired in C#", $"count={e.Count}");
            SetHeatStatus("loaded", StatusKind.Normal);
            if (this.labelBanner.Visible && this.labelBanner.Tag is BannerKind kind && kind != BannerKind.Info)
                HideBanner();
            RefreshStats();
            UpdateStateLabel();
        }

        private void heatmap_LoadFailed(object sender, HeatmapErrorEventArgs e)
        {
            AddTrace(TraceDirection.Server, "LoadFailed fired in C#", $"phase={e.Phase} status={e.Status}");
            string http = e.Status > 0 ? $" (HTTP {e.Status})" : "";
            ShowBanner($"✖ Vendor failure during {e.Phase}{http}: {e.Message}  → click “Reload data”.", BannerKind.Error);
            SetHeatStatus("fault", StatusKind.Error);
        }

        private void gauge_ThresholdExceeded(object sender, GaugeEventArgs e)
        {
            AddTrace(TraceDirection.Server, "ThresholdExceeded fired in C#", $"Value={F(e.Value)} (client reported {F(e.ReportedValue)})");
            ShowBanner($"⚠ Boiler 3 reached {F(e.Value)}{this.gauge.Units} — ThresholdExceeded raised on the server", BannerKind.Alarm);
        }

        private void gauge_RangeChanged(object sender, GaugeEventArgs e)
        {
            AddTrace(TraceDirection.Server, "RangeChanged fired in C#", $"range={e.Range}");
            UpdateStateLabel();
        }

        private void gauge_WidgetError(object sender, GaugeErrorEventArgs e)
        {
            AddTrace(TraceDirection.Server, "WidgetError fired in C#", $"phase={e.Phase}");
            ShowBanner($"✖ Gauge vendor failure during {e.Phase}: {e.Message}", BannerKind.Error);
        }

        private void widget_Trace(object sender, TraceEventArgs e)
            => AddTrace(e.Direction, e.Name, e.Payload);

        #endregion

        #region UI helpers

        private enum StatusKind { Normal, Warn, Error }
        private enum BannerKind { Info, Alarm, Error }

        private void AddTrace(TraceDirection direction, string name, string payload)
        {
            string prefix = direction switch
            {
                TraceDirection.ServerToClient => "→ .NET→JS ",
                TraceDirection.ClientToServer => "← JS→.NET ",
                _ => "• server  ",
            };
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            this.listTrace.Items.Add($"{time}  {prefix} {name,-30} {payload}");
            this.listTrace.SelectedIndex = this.listTrace.Items.Count - 1;
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.listTrace.Items.Clear();
        }

        private void SetGauge(double value)
        {
            try
            {
                double previous = this.gauge.Value;
                this.gauge.Value = value;
                if (previous != value)
                    this.gauge.TraceStateOut($"setValue({F(value)})", $"{{\"value\":{F(value)}}}");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                AddTrace(TraceDirection.Server, "rejected", ex.Message);
            }
        }

        private void SetHeatStatus(string text, StatusKind kind)
        {
            this.labelHeatStatus.Text = "● " + text;
            this.labelHeatStatus.ForeColor = kind switch
            {
                StatusKind.Error => System.Drawing.Color.FromArgb(224, 86, 59),
                StatusKind.Warn => System.Drawing.Color.FromArgb(232, 161, 60),
                _ => System.Drawing.Color.FromArgb(31, 157, 87),
            };
        }

        private void SetLiveStatus(bool running, string text)
        {
            this.labelLiveValue.Text = "● " + text;
            this.labelLiveValue.ForeColor = running
                ? System.Drawing.Color.FromArgb(31, 157, 87)
                : System.Drawing.Color.FromArgb(90, 107, 125);
        }

        private void ShowBanner(string text, BannerKind kind)
        {
            this.labelBanner.Text = text;
            this.labelBanner.Tag = kind;
            switch (kind)
            {
                case BannerKind.Alarm:
                    this.labelBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
                    this.labelBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
                    break;
                case BannerKind.Error:
                    this.labelBanner.BackColor = System.Drawing.Color.FromArgb(255, 244, 229);
                    this.labelBanner.ForeColor = System.Drawing.Color.FromArgb(146, 64, 14);
                    break;
                default:
                    this.labelBanner.BackColor = System.Drawing.Color.FromArgb(230, 240, 251);
                    this.labelBanner.ForeColor = System.Drawing.Color.FromArgb(21, 79, 143);
                    break;
            }
            this.labelBanner.Visible = true;
        }

        private void HideBanner()
        {
            this.labelBanner.Visible = false;
        }

        /// <summary>Stats strip: live Widget instances on the page, postback requests in the last minute.</summary>
        private void RefreshStats()
        {
            this.labelWidgetsValue.Text = CountLiveWidgets(this).ToString(CultureInfo.InvariantCulture);
            this.labelRequestsValue.Text = CountRequestsPerMinute(this).ToString(CultureInfo.InvariantCulture);
        }

        private static int CountLiveWidgets(Control root)
        {
            int count = 0;
            foreach (Control child in root.Controls)
            {
                if (child.IsDisposed) continue;
                if (child is Widget) count++;
                count += CountLiveWidgets(child);
            }
            return count;
        }

        private static int CountRequestsPerMinute(Control root)
        {
            int count = 0;
            foreach (Control child in root.Controls)
            {
                if (child.IsDisposed) continue;
                if (child is HeatmapWidget h) count += h.RequestsPerMinute;
                count += CountRequestsPerMinute(child);
            }
            return count;
        }

        private void UpdateStateLabel()
        {
            var h = this.heatmap;
            var g = this.gauge;
            string peak = "—";
            if (h.Cells.Count > 0)
            {
                var p = h.FindPeak();
                peak = $"{DayName(p.Day)} {p.Hour:00}:00={F(p.Value)}";
            }
            this.labelState.Text =
                $"SERVER STATE (authoritative)\n" +
                $"heatmap {h.Days}×{h.Hours} warn={F(h.WarnAt)} high={F(h.HighAt)}\n" +
                $"cells={h.Cells.Count} peak={peak} loaded={h.IsDataLoaded}\n" +
                $"gauge value={F(g.Value)} range={g.CurrentRange}\n" +
                $"task running={_live.IsRunning} pushes={_live.Iteration}/{_live.MaxIterations}";
        }

        private static string DayName(int day) => day >= 0 && day < DayNames.Length ? DayNames[day] : "D" + day;

        private static string F(double value) => value.ToString(CultureInfo.InvariantCulture);

        private static int ToInt(object value)
        {
            try { return value == null ? -1 : Convert.ToInt32(value, CultureInfo.InvariantCulture); }
            catch { return -1; }
        }

        #endregion
    }
}
