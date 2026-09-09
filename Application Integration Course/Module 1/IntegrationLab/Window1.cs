using System;
using System.Globalization;
using IntegrationLab.Widgets;
using Wisej.Web;

namespace IntegrationLab
{
    /// <summary>
    /// Sensor Monitor — Module 1 lab window.
    ///
    /// Left card:  the TemperatureGauge Control (server state) hosting the vendor gauge (client widget).
    /// Right card: every message that crosses the wire, in both directions.
    /// Bottom bar: success path (set values), progress path (stream), failure paths (invalid / corrupt).
    /// </summary>
    public partial class Window1 : Form
    {
        // Readings the "Stream readings" button replays: climbs through warm, crosses the
        // threshold once (one meaningful event), then cools down.
        private static readonly double[] StreamReadings =
            { 72, 76, 81, 85, 89, 94, 98, 102, 104, 101, 96, 90, 84, 79 };

        private int _streamIndex = -1;

        public Window1()
        {
            InitializeComponent();

            // Widget events → .NET events (the contract in action).
            this.gaugeTemperature.ThresholdExceeded += gaugeTemperature_ThresholdExceeded;
            this.gaugeTemperature.RangeChanged += gaugeTemperature_RangeChanged;
            this.gaugeTemperature.WidgetError += gaugeTemperature_WidgetError;
            this.gaugeTemperature.Trace += gaugeTemperature_Trace;
        }

        private void Window1_Load(object sender, EventArgs e)
        {
            // Rendering: the first time the component is sent to the browser it ships its
            // whole Options object as compact JSON; afterwards only changed fields travel.
            AddTrace(TraceDirection.ServerToClient, "render → init(options)", this.gaugeTemperature.ToJson());
            SetStatus("streaming", StatusKind.Normal);
            UpdateStateLabel();
        }

        #region Success path: server sets Value, compact JSON renders the gauge

        private void buttonSet72_Click(object sender, EventArgs e) => SetValue(72);
        private void buttonSet88_Click(object sender, EventArgs e) => SetValue(88);
        private void buttonSet104_Click(object sender, EventArgs e) => SetValue(104);
        private void buttonSet79_Click(object sender, EventArgs e) => SetValue(79);

        /// <summary>
        /// The only way the UI changes the reading. Validation happens here, on the server,
        /// before anything is rendered. Returns false when the value was rejected.
        /// </summary>
        private bool SetValue(double value)
        {
            try
            {
                double previous = this.gaugeTemperature.Value;
                this.gaugeTemperature.Value = value;

                if (previous != value)
                    this.gaugeTemperature.TraceStateOut(
                        $"setValue({F(value)})",
                        $"{{\"value\":{F(value)}}}");

                if (value < this.gaugeTemperature.Threshold)
                    HideAlarm();

                UpdateStateLabel();
                return true;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // Failure path 1: the server refuses invalid state. Nothing is rendered,
                // the browser keeps showing the last good value, the UI says why.
                AddTrace(TraceDirection.Server, "rejected", ex.Message);
                ShowAlarm($"✖ Rejected on the server: {ex.Message}", AlarmKind.Error);
                SetStatus("rejected", StatusKind.Error);
                AlertBox.Show(ex.Message, MessageBoxIcon.Error,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return false;
            }
        }

        #endregion

        #region Progress path: a Timer (a Component with no visual surface) streams readings

        private void buttonStream_Click(object sender, EventArgs e)
        {
            if (this.timerStream.Enabled)
            {
                StopStream("stopped by operator");
                return;
            }

            _streamIndex = -1;
            this.buttonStream.Text = "■ Stop streaming";
            AddTrace(TraceDirection.Server, "stream", $"replaying {StreamReadings.Length} readings every {this.timerStream.Interval} ms");
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
            if (!SetValue(StreamReadings[_streamIndex]))
                StopStream("aborted after a rejected reading");
        }

        private void StopStream(string reason)
        {
            this.timerStream.Stop();
            this.buttonStream.Text = "▶ Stream readings";
            AddTrace(TraceDirection.Server, "stream", reason);
            SetStatus(this.gaugeTemperature.CurrentRange == "high" ? "alarm" : "idle",
                      this.gaugeTemperature.CurrentRange == "high" ? StatusKind.Error : StatusKind.Normal);
        }

        #endregion

        #region Failure paths

        private void buttonInvalid_Click(object sender, EventArgs e)
        {
            // 150 is above Maximum (120): the typed property throws before any JSON is rendered.
            SetValue(150);
        }

        private void buttonCorrupt_Click(object sender, EventArgs e)
        {
            // Bypasses the typed property and ships a malformed payload. The vendor throws,
            // the adapter catches it and reports one "error" event; the page stays alive.
            if (this.timerStream.Enabled)
                StopStream("stopped before corrupting the payload");

            SetStatus("fault expected…", StatusKind.Warn);
            this.gaugeTemperature.CorruptStateForTesting();
        }

        private void buttonResync_Click(object sender, EventArgs e)
        {
            // Recovery: the server is the source of truth, so recovery is a re-render.
            this.gaugeTemperature.ResyncFromServer();
            bool high = this.gaugeTemperature.CurrentRange == "high";
            if (high)
                ShowAlarm("⚠ ThresholdExceeded raised on the server — operator notified", AlarmKind.Alarm);
            else
                HideAlarm();
            SetStatus(high ? "alarm" : "streaming", high ? StatusKind.Error : StatusKind.Normal);
            UpdateStateLabel();
        }

        #endregion

        #region Widget → .NET events

        private void gaugeTemperature_ThresholdExceeded(object sender, GaugeEventArgs e)
        {
            AddTrace(TraceDirection.Server, "ThresholdExceeded fired in C#",
                $"Value={F(e.Value)} (client reported {F(e.ReportedValue)})");
            ShowAlarm("⚠ ThresholdExceeded raised on the server — operator notified", AlarmKind.Alarm);
            SetStatus("alarm", StatusKind.Error);
            AlertBox.Show($"Boiler 3 reached {F(e.Value)}{this.gaugeTemperature.Units}. Operator notified.", MessageBoxIcon.Warning,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        private void gaugeTemperature_RangeChanged(object sender, GaugeEventArgs e)
        {
            AddTrace(TraceDirection.Server, "RangeChanged fired in C#", $"range={e.Range}");
            switch (e.Range)
            {
                case "high":
                    SetStatus("alarm", StatusKind.Error, keepText: this.timerStream.Enabled);
                    break;
                case "warm":
                    SetStatus("warm", StatusKind.Warn, keepText: this.timerStream.Enabled);
                    HideAlarm();
                    break;
                default:
                    SetStatus("streaming", StatusKind.Normal, keepText: this.timerStream.Enabled);
                    HideAlarm();
                    break;
            }
            UpdateStateLabel();
        }

        private void gaugeTemperature_WidgetError(object sender, GaugeErrorEventArgs e)
        {
            // Failure path 2 surfaced in the UI: the vendor failed inside the client adapter.
            AddTrace(TraceDirection.Server, "WidgetError fired in C#", $"phase={e.Phase}");
            ShowAlarm($"✖ Vendor failure during {e.Phase}: {e.Message}  → click “Resync from server”.", AlarmKind.Error);
            SetStatus("fault", StatusKind.Error);
        }

        private void gaugeTemperature_Trace(object sender, TraceEventArgs e)
            => AddTrace(e.Direction, e.Name, e.Payload);

        #endregion

        #region UI helpers

        private enum StatusKind { Normal, Warn, Error }
        private enum AlarmKind { Alarm, Error }

        private void AddTrace(TraceDirection direction, string name, string payload)
        {
            string prefix = direction switch
            {
                TraceDirection.ServerToClient => "→ .NET→JS ",
                TraceDirection.ClientToServer => "← JS→.NET ",
                _ => "• server  ",
            };
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            this.listTrace.Items.Add($"{time}  {prefix} {name,-28} {payload}");
            this.listTrace.SelectedIndex = this.listTrace.Items.Count - 1;
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.listTrace.Items.Clear();
        }

        private void SetStatus(string text, StatusKind kind, bool keepColor = false, bool keepText = false)
        {
            if (!keepText)
                this.labelStatus.Text = "● " + text;

            if (keepColor)
                return;

            this.labelStatus.ForeColor = kind switch
            {
                StatusKind.Error => System.Drawing.Color.FromArgb(224, 86, 59),
                StatusKind.Warn => System.Drawing.Color.FromArgb(232, 161, 60),
                _ => System.Drawing.Color.FromArgb(31, 157, 87),
            };
        }

        private void ShowAlarm(string text, AlarmKind kind)
        {
            this.labelAlarm.Text = text;
            this.labelAlarm.BackColor = kind == AlarmKind.Alarm
                ? System.Drawing.Color.FromArgb(253, 236, 234)
                : System.Drawing.Color.FromArgb(255, 244, 229);
            this.labelAlarm.ForeColor = kind == AlarmKind.Alarm
                ? System.Drawing.Color.FromArgb(178, 59, 39)
                : System.Drawing.Color.FromArgb(146, 64, 14);
            this.labelAlarm.Visible = true;
        }

        private void HideAlarm()
        {
            this.labelAlarm.Visible = false;
        }

        private void UpdateStateLabel()
        {
            var g = this.gaugeTemperature;
            this.labelState.Text =
                $"SERVER STATE (authoritative)\n" +
                $"Value={F(g.Value)}  Min={F(g.Minimum)}  Max={F(g.Maximum)}  WarnAt={F(g.WarnAt)}  Threshold={F(g.Threshold)}\n" +
                $"range={g.CurrentRange}   widget loaded={g.IsLoaded}";
        }

        private static string F(double value) => value.ToString(CultureInfo.InvariantCulture);

        #endregion
    }
}
