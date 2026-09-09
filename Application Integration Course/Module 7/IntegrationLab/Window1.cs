using System;
using System.Globalization;
using IntegrationLab.Contracts;
using IntegrationLab.Widgets;
using Wisej.Web;

namespace IntegrationLab
{
    /// <summary>
    /// IntegrationLab — Event Demo (Module 7 lab window).
    ///
    /// Left:  three cards — Gauge, Knob, Chart — each a Widget hosting a vendor library.
    /// Right: "Server WidgetEvent log" — every message in both directions, every .NET event, every rejection.
    /// Bottom: buttons that exercise the success paths, the progress path (stream), the failure path
    ///         (bad payload → server validation rejects it), recreate-and-rewire, and the noise counter.
    ///
    /// The three server-side handlers, on purpose in three different styles:
    ///   1. GaugeWidget.OnWidgetEvent  → typed ThresholdCrossed(GaugeThresholdEventArgs)   (handled here: gauge_ThresholdCrossed)
    ///   2. knob.WidgetEvent           → page-level switch on e.Type                          (handled here: knob_WidgetEvent)
    ///   3. ChartWidget.OnWebEvent     → typed PointClicked(ChartPointEventArgs)             (handled here: chart_PointClicked)
    /// </summary>
    public partial class Window1 : Form
    {
        // Readings the "Stream" button replays: climbs into warm, crosses the threshold once, cools down.
        private static readonly double[] StreamReadings =
            { 72, 76, 81, 85, 89, 94, 98, 102, 104, 101, 96, 90, 84, 79, 72 };

        // Alternative data sets for "Chart: new data".
        private static readonly double[][] DataSets =
        {
            new double[] { 30, 52, 41, 68, 47, 80 },
            new double[] { 44, 39, 61, 73, 58, 66 },
            new double[] { 22, 35, 58, 49, 77, 91 },
        };

        private int _streamIndex = -1;
        private int _dataSetIndex = 0;
        private int _eventsRaised = 0;

        public Window1()
        {
            InitializeComponent();

            // ---- the three server-side handlers ------------------------------------------
            this.gauge.ThresholdCrossed += gauge_ThresholdCrossed;        // #1 typed event (raised from OnWidgetEvent)
            this.knob.WidgetEvent += knob_WidgetEvent;                    // #2 raw catch-all, page-level switch on e.Type
            this.chart.PointClicked += chart_PointClicked;                // #3 typed event (raised from OnWebEvent)

            // log lines emitted by the widgets themselves
            this.gauge.Trace += widget_Trace;
            this.knob.Trace += widget_Trace;
            this.chart.Trace += widget_Trace;
        }

        private void Window1_Load(object sender, EventArgs e)
        {
            AddTrace(TraceDirection.ServerToClient, "render → init(options)", "gauge " + this.gauge.ToJson());
            AddTrace(TraceDirection.ServerToClient, "render → init(options)", "knob " + this.knob.ToJson());
            AddTrace(TraceDirection.ServerToClient, "render → init(options)", "chart {labels:[Jan..Jun], series:[Orders], theme:light}");
            SetStatus("idle", StatusKind.Normal);
            UpdateStateLabels();
        }

        #region Server event handler #1 — GaugeWidget.ThresholdCrossed (typed, from OnWidgetEvent)

        private void gauge_ThresholdCrossed(object sender, GaugeThresholdEventArgs e)
        {
            // Application code sees a DTO, never the string name or the dynamic data.
            RaisedBanner($"ThresholdCrossed  {e}", e.Level == "high" ? BannerKind.Alarm : BannerKind.Warn);
            SetStatus(e.Level == "high" ? "alarm" : "warm", e.Level == "high" ? StatusKind.Error : StatusKind.Warn, keepText: this.timerStream.Enabled);
            if (e.Level == "high")
                AlertBox.Show($"Boiler reached {F(e.Value)}{this.gauge.Units} — operator notified.", MessageBoxIcon.Warning,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            UpdateStateLabels();
        }

        #endregion

        #region Server event handler #2 — knob.WidgetEvent (raw catch-all, switch on e.Type)

        /// <summary>
        /// Wisej.Web.Widget → WidgetEvent. Every fireWidgetEvent from knob-init.js lands here with
        /// the name in e.Type and the deserialized payload in e.Data. The handler validates the
        /// small DTO through the widget and only then touches server state.
        /// </summary>
        private void knob_WidgetEvent(object sender, WidgetEventArgs e)
        {
            switch (e.Type)
            {
                case "valueChanged":
                    {
                        AddTrace(TraceDirection.ClientToServer, "valueChanged", "e.Data = " + PayloadReader.ToJson(e.Data));

                        if (!this.knob.TryReadValueChanged(e.Data, out KnobValueEventArgs d, out string reason))
                        {
                            AddTrace(TraceDirection.Rejected, "valueChanged", "rejected: " + reason);
                            RaisedBanner("valueChanged rejected: " + reason, BannerKind.Error);
                            return;
                        }

                        OnValueChanged(d);
                        break;
                    }

                case "error":
                    {
                        dynamic data = e.Data;
                        AddTrace(TraceDirection.ClientToServer, "error", PayloadReader.ToJson(e.Data));
                        RaisedBanner($"knob vendor failure during {PayloadReader.Get(() => data.phase)}: {PayloadReader.Get(() => data.message)}", BannerKind.Error);
                        break;
                    }

                default:
                    AddTrace(TraceDirection.Rejected, e.Type ?? "(null)", "rejected: not in the contract");
                    break;
            }
        }

        /// <summary>The knob's server-owned state changed (the video's OnValueChanged).</summary>
        private void OnValueChanged(KnobValueEventArgs e)
        {
            this.knob.AcceptClientValue(e);
            AddTrace(TraceDirection.Server, "OnValueChanged", $"page-level WidgetEvent switch → KnobValueEventArgs {e}");
            RaisedBanner($"valueChanged  {e}", BannerKind.Info);
            if (e.IsUserChange)
                this.knob.Pulse();                                   // Call("pulse"): a visual acknowledgement, no state
            UpdateStateLabels();
        }

        #endregion

        #region Server event handler #3 — ChartWidget.PointClicked (typed, from OnWebEvent)

        private void chart_PointClicked(object sender, ChartPointEventArgs e)
        {
            RaisedBanner($"PointClicked  {e}", BannerKind.Info);
            this.labelChartState.Text = $"last drill-down: #{e.Index} {e.Label} = {F(e.Value)}   (server data, index used as lookup key)";
            AlertBox.Show($"Drill-down: {e.Label} = {F(e.Value)} {this.chart.SeriesLabel}", MessageBoxIcon.Information,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 2500);
        }

        #endregion

        #region Buttons — success paths

        private void buttonGauge72_Click(object sender, EventArgs e) => SetGauge(72);
        private void buttonGauge104_Click(object sender, EventArgs e) => SetGauge(104);

        private bool SetGauge(double value)
        {
            try
            {
                this.gauge.Value = value;                            // server state → {"value":…} → update() → vendor → maybe ONE thresholdCrossed
                if (value < this.gauge.WarnAt) SetStatus("idle", StatusKind.Normal, keepText: this.timerStream.Enabled);
                UpdateStateLabels();
                return true;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                AddTrace(TraceDirection.Rejected, "gauge.Value", ex.Message);
                RaisedBanner("gauge.Value rejected on the server: " + ex.Message, BannerKind.Error);
                return false;
            }
        }

        private void buttonKnobPlus10_Click(object sender, EventArgs e)
        {
            // Server-driven change: the vendor fires knobchange inside update(); the adapter reports
            // valueChanged { source: "server" } through the deferred handler.
            double next = Math.Min(this.knob.Maximum, this.knob.Value + 10);
            if (next == this.knob.Value) next = this.knob.Minimum;   // wrap around at the top
            try
            {
                this.knob.Value = next;
                UpdateStateLabels();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                AddTrace(TraceDirection.Rejected, "knob.Value", ex.Message);
            }
        }

        private void buttonChartData_Click(object sender, EventArgs e)
        {
            _dataSetIndex = (_dataSetIndex + 1) % DataSets.Length;
            this.chart.SetData(this.chart.Labels, DataSets[_dataSetIndex]);
            this.labelChartState.Text = $"data set {_dataSetIndex + 1}/{DataSets.Length} rendered — hover/zoom/render stay in the browser; click a point";
        }

        private void buttonRecreate_Click(object sender, EventArgs e)
        {
            // Libraries that recreate their instance on some option changes need the handlers
            // re-attached: chart-init.js runs the same wire() from init and from this path.
            this.chart.RecreateVendorInstance();
            this.labelChartState.Text = $"vendor instance recreated (theme={this.chart.Theme}) and RE-WIRED — click a point: it still arrives";
        }

        private async void buttonNoise_Click(object sender, EventArgs e)
        {
            // CallAsync: ask the adapter how much it kept in the browser (proof of filtering).
            try
            {
                AddTrace(TraceDirection.ServerToClient, "CallAsync(\"getNoiseCount\")", "");
                dynamic r = await this.chart.GetNoiseCountAsync();
                int hover = ToInt(PayloadReader.Get(() => r.hover)), zoom = ToInt(PayloadReader.Get(() => r.zoom)),
                    render = ToInt(PayloadReader.Get(() => r.render)), layout = ToInt(PayloadReader.Get(() => r.layout)),
                    legend = ToInt(PayloadReader.Get(() => r.legendclick)), total = ToInt(PayloadReader.Get(() => r.total)),
                    forwarded = ToInt(PayloadReader.Get(() => r.forwarded)), generation = ToInt(PayloadReader.Get(() => r.generation));

                AddTrace(TraceDirection.ClientToServer, "getNoiseCount →",
                    $"{{hover:{hover}, zoom:{zoom}, render:{render}, layout:{layout}, legendclick:{legend}}} kept · pointClicked forwarded: {forwarded} · vendor instance #{generation}");
                this.labelNoise.Text = $"events kept in the browser: {total}   (hover {hover} · zoom {zoom} · render {render} · layout {layout} · legend {legend})   ·   forwarded to .NET: {forwarded}";
                RaisedBanner($"noise counter: {total} vendor events stayed in the browser, {forwarded} reached .NET", BannerKind.Info);
            }
            catch (Exception ex)
            {
                AddTrace(TraceDirection.Rejected, "CallAsync", ex.Message);
                RaisedBanner("CallAsync failed: " + ex.Message, BannerKind.Error);
            }
        }

        #endregion

        #region Buttons — failure path, progress path, housekeeping

        private void buttonBadPayload_Click(object sender, EventArgs e)
        {
            // Failure path: the adapter fires a pointClicked that violates the contract.
            // The server validates every field and rejects it ("rejected: index out of range").
            this.chart.FireBadPayloadForTesting();
        }

        private void buttonStream_Click(object sender, EventArgs e)
        {
            if (this.timerStream.Enabled)
            {
                StopStream("stopped by operator");
                return;
            }
            _streamIndex = -1;
            this.buttonStream.Text = "■ Stop";
            AddTrace(TraceDirection.Server, "stream", $"replaying {StreamReadings.Length} gauge readings every {this.timerStream.Interval} ms — expect ONE warn and ONE high crossing");
            this.timerStream.Start();
            timerStream_Tick(sender, e);
        }

        private void timerStream_Tick(object sender, EventArgs e)
        {
            _streamIndex++;
            if (_streamIndex >= StreamReadings.Length)
            {
                StopStream("complete");
                return;
            }
            SetStatus($"streaming {_streamIndex + 1}/{StreamReadings.Length}", StatusKind.Normal, keepColor: true);
            if (!SetGauge(StreamReadings[_streamIndex]))
                StopStream("aborted after a rejected reading");
        }

        private void StopStream(string reason)
        {
            this.timerStream.Stop();
            this.buttonStream.Text = "▶ Stream";
            AddTrace(TraceDirection.Server, "stream", reason);
            SetStatus("idle", StatusKind.Normal);
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.listTrace.Items.Clear();
            this.labelBanner.Visible = false;
        }

        #endregion

        #region UI helpers

        private enum StatusKind { Normal, Warn, Error }
        private enum BannerKind { Info, Warn, Alarm, Error }

        private void widget_Trace(object sender, TraceEventArgs e) => AddTrace(e.Direction, e.Name, e.Payload);

        private void AddTrace(TraceDirection direction, string name, string payload)
        {
            string prefix = direction switch
            {
                TraceDirection.ServerToClient => "→ .NET→JS ",
                TraceDirection.ClientToServer => "← JS→.NET ",
                TraceDirection.Rejected => "✖ rejected ",
                _ => "• .NET     ",
            };
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            this.listTrace.Items.Add($"{time}  {prefix} {name,-22} {payload}");
            this.listTrace.SelectedIndex = this.listTrace.Items.Count - 1;
        }

        /// <summary>The banner shows the last .NET event raised (or the last rejection).</summary>
        private void RaisedBanner(string text, BannerKind kind)
        {
            if (kind != BannerKind.Error) _eventsRaised++;
            this.labelBanner.Text = (kind == BannerKind.Error ? "✖ " : $"#{_eventsRaised}  last .NET event raised:  ") + text;
            this.labelBanner.BackColor = kind switch
            {
                BannerKind.Alarm => System.Drawing.Color.FromArgb(253, 236, 234),
                BannerKind.Warn => System.Drawing.Color.FromArgb(255, 244, 229),
                BannerKind.Error => System.Drawing.Color.FromArgb(253, 236, 234),
                _ => System.Drawing.Color.FromArgb(232, 243, 255),
            };
            this.labelBanner.ForeColor = kind switch
            {
                BannerKind.Alarm => System.Drawing.Color.FromArgb(178, 59, 39),
                BannerKind.Warn => System.Drawing.Color.FromArgb(146, 64, 14),
                BannerKind.Error => System.Drawing.Color.FromArgb(178, 59, 39),
                _ => System.Drawing.Color.FromArgb(13, 71, 140),
            };
            this.labelBanner.Visible = true;
        }

        private void SetStatus(string text, StatusKind kind, bool keepColor = false, bool keepText = false)
        {
            if (!keepText) this.labelStatus.Text = "● " + text;
            if (keepColor) return;
            this.labelStatus.ForeColor = kind switch
            {
                StatusKind.Error => System.Drawing.Color.FromArgb(224, 86, 59),
                StatusKind.Warn => System.Drawing.Color.FromArgb(232, 161, 60),
                _ => System.Drawing.Color.FromArgb(31, 157, 87),
            };
        }

        private void UpdateStateLabels()
        {
            this.labelGaugeState.Text = $"server: Value={F(this.gauge.Value)}  WarnAt={F(this.gauge.WarnAt)}  Threshold={F(this.gauge.Threshold)}  lastLevel={(this.gauge.LastLevel == "" ? "—" : this.gauge.LastLevel)}";
            this.labelKnobState.Text = $"server: Value={F(this.knob.Value)}  lastSource={(this.knob.LastSource == "" ? "—" : this.knob.LastSource)}";
        }

        private static string F(double value) => value.ToString(CultureInfo.InvariantCulture);

        private static int ToInt(object raw) => PayloadReader.TryInt(raw, out int i) ? i : 0;

        #endregion
    }
}
