using System;
using System.Globalization;
using IntegrationLab.Controls;
using IntegrationLab.Data;
using IntegrationLab.Services;
using Wisej.Web;

namespace IntegrationLab
{
    /// <summary>
    /// Operations Dashboard: the HeatmapWidget (postback data source) and the TemperatureGauge,
    /// fed by one bounded background task, with the dashboard counters on top.
    /// </summary>
    public partial class OperationsPage : Page
    {
        private const int LeakCycles = 25;
        private static readonly string[] DayNames = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };

        private readonly LiveUpdateService _live;
        private HeatmapWidget _leakWidget;        // the widget currently alive in the create/dispose test
        private int _leakCycle;
        private int _errorCount;

        public OperationsPage()
        {
            InitializeComponent();

            this.heatmap.CellSelected += heatmap_CellSelected;
            this.heatmap.DataLoaded += heatmap_DataLoaded;
            this.heatmap.LoadFailed += heatmap_LoadFailed;

            this.gauge.ThresholdExceeded += gauge_ThresholdExceeded;
            this.gauge.WidgetError += gauge_WidgetError;

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
            RefreshStats();
        }

        #region Live updates (Application.StartTask → Call → Application.Update)

        private void buttonLive_Click(object sender, EventArgs e)
        {
            if (_live.IsRunning)
            {
                _live.Stop();
                this.buttonLive.Text = "■ stopping…";
                return;
            }

            _live.Start();
            this.buttonLive.Text = "■ Stop live updates";
        }

        /// <summary>Runs inside Application.Update(this): change the widgets like a button handler would, then the push happens.</summary>
        private void live_Updated(object sender, LiveUpdateEventArgs e)
        {
            var reading = e.Reading;
            this.heatmap.SetCells(reading.Cells);       // Call("setCells", cells) on the client
            SetGauge(reading.Temperature);              // Options.value → update(options)
            RefreshStats();
        }

        private void live_Stopped(object sender, LiveUpdateStoppedEventArgs e)
        {
            this.buttonLive.Text = "▶ Start live updates";
        }

        #endregion

        #region Server calls

        private void buttonPeak_Click(object sender, EventArgs e)
        {
            try
            {
                // The server decides from its copy of the data, then tells the client what to show.
                var peak = this.heatmap.FindPeak();
                this.heatmap.Highlight(peak.Day, peak.Hour);
                ShowBanner($"▲ Peak load: {DayName(peak.Day)} {peak.Hour:00}:00 = {F(peak.Value)}", BannerKind.Info);
            }
            catch (InvalidOperationException ex)
            {
                ShowBanner("✖ " + ex.Message, BannerKind.Error);
            }
        }

        private void buttonReload_Click(object sender, EventArgs e)
        {
            HideBanner();
            this.heatmap.Reload();
        }

        private async void buttonCellCount_Click(object sender, EventArgs e)
        {
            this.buttonCellCount.Enabled = false;
            try
            {
                int count = await this.heatmap.GetCellCountAsync();   // CallAsync round trip
                AlertBox.Show($"The client widget holds {count} cells.", MessageBoxIcon.Information,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 3000);
            }
            finally
            {
                this.buttonCellCount.Enabled = true;
                // The code after await can finish after the click's request has returned: push the re-enable now.
                Application.Update(this);
            }
        }

        #endregion

        #region Create/dispose test

        private void buttonLeak_Click(object sender, EventArgs e)
        {
            if (this.timerLeak.Enabled) return;

            // Reset the client counters the adapter maintains (init → __integrationLabCreated, dispose → __integrationLabDisposed).
            this.Eval("window.__integrationLabCreated = 0; window.__integrationLabDisposed = 0;");
            _leakCycle = 0;
            this.labelDisposedValue.Text = "running…";
            this.labelDisposedValue.ForeColor = System.Drawing.Color.FromArgb(232, 161, 60);
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
                _leakWidget = new HeatmapWidget { Days = 3, Hours = 12, Title = $"#{_leakCycle}", Dock = DockStyle.Fill };
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
            int disposed = ToInt(await this.EvalAsync("window.__integrationLabDisposed"));
            int created = ToInt(await this.EvalAsync("window.__integrationLabCreated"));
            int vendorAlive = ToInt(await this.EvalAsync("(typeof VendorHeatmap !== 'undefined') ? VendorHeatmap.liveInstances() : -1"));

            bool clean = disposed == LeakCycles && created == LeakCycles && vendorAlive == 1;   // 1 = the dashboard heatmap
            this.labelDisposedValue.Text = $"{disposed}/{created}";
            this.labelDisposedValue.ForeColor = clean
                ? System.Drawing.Color.FromArgb(31, 157, 87)
                : System.Drawing.Color.FromArgb(224, 86, 59);
            ShowBanner(clean
                ? $"✔ Disposed cleanly {disposed}/{created}"
                : $"✖ Leak suspected: created {created}, disposed {disposed}, vendor instances alive {vendorAlive}",
                clean ? BannerKind.Info : BannerKind.Error);
            RefreshStats();

            // The EvalAsync results arrive after the timer tick's request has returned: push the tile and banner now.
            Application.Update(this);
        }

        #endregion

        #region Widget events

        private void heatmap_CellSelected(object sender, HeatmapCellEventArgs e)
        {
            ShowBanner($"● {DayName(e.Day)} {e.Hour:00}:00 → load {F(e.Value)}", BannerKind.Info);
        }

        private void heatmap_DataLoaded(object sender, HeatmapLoadedEventArgs e)
        {
            if (this.labelBanner.Visible && this.labelBanner.Tag is BannerKind kind && kind != BannerKind.Info)
                HideBanner();
            RefreshStats();
        }

        private void heatmap_LoadFailed(object sender, HeatmapErrorEventArgs e)
        {
            _errorCount++;
            string http = e.Status > 0 ? $" (HTTP {e.Status})" : "";
            ShowBanner($"✖ Heatmap failed during {e.Phase}{http}: {e.Message}", BannerKind.Error);
            RefreshStats();
        }

        private void gauge_ThresholdExceeded(object sender, GaugeEventArgs e)
        {
            ShowBanner($"⚠ Boiler 3 reached {F(e.Value)}{this.gauge.Units}", BannerKind.Alarm);
        }

        private void gauge_WidgetError(object sender, GaugeErrorEventArgs e)
        {
            _errorCount++;
            ShowBanner($"✖ Gauge failed during {e.Phase}: {e.Message}", BannerKind.Error);
            RefreshStats();
        }

        #endregion

        #region UI helpers

        private enum BannerKind { Info, Alarm, Error }

        private void SetGauge(double value)
        {
            try
            {
                this.gauge.Value = value;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                ShowBanner("✖ " + ex.Message, BannerKind.Error);
            }
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

        /// <summary>Dashboard counters: live Widget instances, postback requests in the last minute, errors.</summary>
        private void RefreshStats()
        {
            this.labelWidgetsValue.Text = CountLiveWidgets(this).ToString(CultureInfo.InvariantCulture);
            this.labelRequestsValue.Text = CountRequestsPerMinute(this).ToString(CultureInfo.InvariantCulture);
            this.labelErrorsValue.Text = _errorCount.ToString(CultureInfo.InvariantCulture);
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
