using System;
using System.Globalization;
using Wisej.Core;
using Wisej.Web;

namespace IntegrationLab
{
    /// <summary>
    /// Dashboard: two plain Wisej.Web.Widget instances (no wrapper class), a VendorGauge and a
    /// jQuery-style VendorKnob. Each is configured in the designer file with Packages (JS + CSS),
    /// Options (.NET values serialized to the client) and an InitScript (init / update / a
    /// callable method). The server buttons change both widgets' Options and queue a client Call.
    /// </summary>
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();

            // A plain Widget raises WidgetEvent for every fireWidgetEvent(...) on the client;
            // e.Type is the name, e.Data the payload.
            this.gauge.WidgetEvent += gauge_WidgetEvent;
            this.knob.WidgetEvent += knob_WidgetEvent;
        }

        private void DashboardPage_Load(object sender, EventArgs e)
        {
            // The first response ships each widget's whole Options object; the client loads the
            // packages in order, then calls init(options).
            AddTrace(TraceDirection.ServerToClient, "gauge init(options)", LabJson.Of(this.gauge.Options));
            AddTrace(TraceDirection.ServerToClient, "knob init(options)", LabJson.Of(this.knob.Options));
        }

        #region Options

        /// <summary>
        /// The colored bands the gauge shows, built as Wisej.Core.DynamicObject instances.
        /// Replacing the whole array is a first-level change and travels to update(options, old).
        /// </summary>
        internal static DynamicObject[] DefaultBands()
            => new[] { Band(0, 85, "#1f9d57"), Band(85, 100, "#e8a13c"), Band(100, 120, "#e0563b") };

        internal static DynamicObject[] PeakBands()
            => new[] { Band(0, 80, "#1f9d57"), Band(80, 95, "#e8a13c"), Band(95, 120, "#e0563b") };

        private static DynamicObject Band(double from, double to, string color)
        {
            dynamic band = new DynamicObject();
            band.from = from;
            band.to = to;
            band.color = color;
            return band;
        }

        /// <summary>
        /// One server set-point: changes both widgets' Options. Each assignment to a first-level
        /// field is detected by the Widget and rendered as compact JSON; the client receives
        /// update(options, old) once per widget with the next response.
        /// </summary>
        private void ApplySetPoint(double gaugeValue, double knobLevel, DynamicObject[] bands)
        {
            dynamic gaugeOptions = this.gauge.Options;
            gaugeOptions.value = gaugeValue;
            AddTrace(TraceDirection.ServerToClient, "gauge.Options", LabJson.Of(new { value = gaugeValue }));

            if (bands != null)
            {
                gaugeOptions.bands = bands;               // whole array replaced = first-level change
                AddTrace(TraceDirection.ServerToClient, "gauge.Options", LabJson.Of(new { bands }));
            }

            dynamic knobOptions = this.knob.Options;
            knobOptions.level = knobLevel;
            AddTrace(TraceDirection.ServerToClient, "knob.Options", LabJson.Of(new { level = knobLevel }));

            HideAlarm();
        }

        /// <summary>
        /// Call(...) names a function defined by the InitScript ("this.flash = function…") and
        /// runs it in the wrapper's context. It is queued and flushed with the next response,
        /// together with the Options changes made in the same request, in order: set the Options
        /// first and issue the Call second, so the client method sees the updated options.
        /// </summary>
        private void QueueCall(Widget widget, string function)
        {
            widget.Call(function);
            AddTrace(TraceDirection.ServerToClient, $"{widget.Name}.Call(\"{function}\")", "— queued to client");
        }

        #endregion

        #region Server buttons: change Options, then call the client

        private void buttonHigh_Click(object sender, EventArgs e)
        {
            ApplySetPoint(88, 78, null);
            QueueCall(this.gauge, "flash");
        }

        private void buttonPeak_Click(object sender, EventArgs e)
        {
            ApplySetPoint(104, 92, PeakBands());
            QueueCall(this.knob, "pulse");
        }

        private void buttonIdle_Click(object sender, EventArgs e)
        {
            ApplySetPoint(64, 40, DefaultBands());
            QueueCall(this.gauge, "flash");
        }

        #endregion

        #region Widget events

        private void gauge_WidgetEvent(object sender, WidgetEventArgs e)
        {
            dynamic data = e.Data;
            if (e.Type == "error")
            {
                AddTrace(TraceDirection.ClientToServer, "gauge.error", LabJson.Of(e.Data));
                ShowAlarm($"✖ Gauge adapter caught a vendor failure during {(string)data?.phase}: {(string)data?.message}");
            }
        }

        private void knob_WidgetEvent(object sender, WidgetEventArgs e)
        {
            dynamic data = e.Data;
            switch (e.Type)
            {
                case "valueChanged":
                    {
                        // User drag / wheel. The server validates the value and takes ownership of it:
                        // it is written back to Options so the next render cannot snap the knob back.
                        AddTrace(TraceDirection.ClientToServer, "knob.valueChanged", LabJson.Of(e.Data));
                        double reported = ToDouble(data?.value);
                        if (double.IsNaN(reported))
                            break;

                        dynamic knobOptions = this.knob.Options;
                        knobOptions.level = Math.Max(0, Math.Min(100, reported));
                        break;
                    }

                case "error":
                    AddTrace(TraceDirection.ClientToServer, "knob.error", LabJson.Of(e.Data));
                    ShowAlarm($"✖ Knob adapter caught a vendor failure during {(string)data?.phase}: {(string)data?.message}");
                    break;
            }
        }

        #endregion

        #region UI helpers

        private enum TraceDirection { ServerToClient, ClientToServer }

        private void AddTrace(TraceDirection direction, string name, string payload)
        {
            string prefix = direction == TraceDirection.ServerToClient ? "→ .NET→JS " : "← JS→.NET ";
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            this.listTrace.Items.Add($"{time}  {prefix} {name,-22} {payload}");
            this.listTrace.SelectedIndex = this.listTrace.Items.Count - 1;
        }

        private void ShowAlarm(string text)
        {
            this.labelAlarm.Text = text;
            this.labelAlarm.Visible = true;
        }

        private void HideAlarm()
        {
            this.labelAlarm.Visible = false;
        }

        private static double ToDouble(object value)
        {
            try { return value == null ? double.NaN : Convert.ToDouble(value, CultureInfo.InvariantCulture); }
            catch { return double.NaN; }
        }

        #endregion
    }
}
