using System;
using System.Globalization;
using IntegrationLab.Contracts;
using IntegrationLab.Widgets;
using Wisej.Web;

namespace IntegrationLab
{
    /// <summary>
    /// IntegrationLab — Event Demo. Three widgets (gauge, knob, chart), each forwarding one
    /// meaningful event, and the server WidgetEvent log.
    ///
    /// The three server-side handlers, in three different styles:
    ///   1. GaugeWidget.OnWidgetEvent  → typed ThresholdCrossed(GaugeThresholdEventArgs)   (consumed here: gauge_ThresholdCrossed)
    ///   2. knob.WidgetEvent           → page-level switch on e.Type                          (knob_WidgetEvent)
    ///   3. ChartWidget.OnWebEvent     → typed PointClicked(ChartPointEventArgs)             (consumed here: chart_PointClicked)
    /// </summary>
    public partial class Window1 : Form
    {
        // Live boiler readings for the gauge: climbs into warm, crosses the threshold, cools down, repeats.
        private static readonly double[] Readings =
            { 72, 76, 81, 85, 89, 94, 98, 102, 104, 101, 96, 90, 84, 79, 72 };

        private int _readingIndex = -1;

        public Window1()
        {
            InitializeComponent();

            this.gauge.ThresholdCrossed += gauge_ThresholdCrossed;        // #1 typed event (raised from OnWidgetEvent)
            this.knob.WidgetEvent += knob_WidgetEvent;                    // #2 raw catch-all, page-level switch on e.Type
            this.chart.PointClicked += chart_PointClicked;                // #3 typed event (raised from OnWebEvent)

            this.gauge.Trace += widget_Trace;
            this.chart.Trace += widget_Trace;
        }

        private void Window1_Load(object sender, EventArgs e)
        {
            this.timerStream.Start();
        }

        private void timerStream_Tick(object sender, EventArgs e)
        {
            _readingIndex = (_readingIndex + 1) % Readings.Length;
            try
            {
                this.gauge.Value = Readings[_readingIndex];     // server state → update() → vendor → at most ONE thresholdCrossed
            }
            catch (ArgumentOutOfRangeException ex)
            {
                this.timerStream.Stop();
                AddTrace(TraceDirection.Rejected, "gauge.Value", ex.Message);
            }
        }

        #region Server event handler #1 — GaugeWidget.ThresholdCrossed (typed, from OnWidgetEvent)

        private void gauge_ThresholdCrossed(object sender, GaugeThresholdEventArgs e)
        {
            // Application code sees a DTO, never the string name or the dynamic data.
            if (e.Level == "high")
                AlertBox.Show($"Boiler reached {F(e.Value)}{this.gauge.Units} — operator notified.", MessageBoxIcon.Warning,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
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
                            return;
                        }

                        OnValueChanged(d);
                        break;
                    }

                case "error":
                    AddTrace(TraceDirection.ClientToServer, "error", PayloadReader.ToJson(e.Data));
                    break;

                default:
                    AddTrace(TraceDirection.Rejected, e.Type ?? "(null)", "rejected: not in the contract");
                    break;
            }
        }

        /// <summary>The knob's server-owned state changed.</summary>
        private void OnValueChanged(KnobValueEventArgs e)
        {
            this.knob.AcceptClientValue(e);
            if (e.IsUserChange)
                this.knob.Pulse();                                   // Call("pulse"): a visual acknowledgement, no state
        }

        #endregion

        #region Server event handler #3 — ChartWidget.PointClicked (typed, from OnWebEvent)

        private void chart_PointClicked(object sender, ChartPointEventArgs e)
        {
            AlertBox.Show($"Drill-down: {e.Label} = {F(e.Value)} {this.chart.SeriesLabel}", MessageBoxIcon.Information,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 2500);
        }

        #endregion

        #region Log

        private void widget_Trace(object sender, TraceEventArgs e) => AddTrace(e.Direction, e.Name, e.Payload);

        private void AddTrace(TraceDirection direction, string name, string payload)
        {
            string prefix = direction == TraceDirection.Rejected ? "✖ rejected " : "← JS→.NET  ";
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            this.listTrace.Items.Add($"{time}  {prefix} {name,-18} {payload}");
            this.listTrace.SelectedIndex = this.listTrace.Items.Count - 1;
        }

        private static string F(double value) => value.ToString(CultureInfo.InvariantCulture);

        #endregion
    }
}
