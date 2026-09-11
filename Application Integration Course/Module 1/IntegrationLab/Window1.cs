using System;
using System.Globalization;
using IntegrationLab.Widgets;
using Wisej.Web;

namespace IntegrationLab
{
    /// <summary>
    /// Sensor Monitor: the TemperatureGauge on the left, the live client/server
    /// message trace on the right.
    /// </summary>
    public partial class Window1 : Form
    {
        // Readings replayed by "Stream readings": climbs through warm, crosses the
        // threshold once, then cools down.
        private static readonly double[] StreamReadings =
            { 72, 76, 81, 85, 89, 94, 98, 102, 104, 101, 96, 90, 84, 79 };

        private int _streamIndex = -1;

        public Window1()
        {
            InitializeComponent();

            this.gaugeTemperature.ThresholdExceeded += gaugeTemperature_ThresholdExceeded;
            this.gaugeTemperature.RangeChanged += gaugeTemperature_RangeChanged;
            this.gaugeTemperature.WidgetError += gaugeTemperature_WidgetError;
            this.gaugeTemperature.Trace += gaugeTemperature_Trace;
        }

        private void Window1_Load(object sender, EventArgs e)
        {
            AddTrace(TraceDirection.ServerToClient, "init(options)", this.gaugeTemperature.ToJson());
            SetStatus("streaming", StatusKind.Normal);
        }

        #region Readings

        private void buttonSet72_Click(object sender, EventArgs e) => SetValue(72);
        private void buttonSet88_Click(object sender, EventArgs e) => SetValue(88);
        private void buttonSet104_Click(object sender, EventArgs e) => SetValue(104);
        private void buttonSet79_Click(object sender, EventArgs e) => SetValue(79);
        private void buttonInvalid_Click(object sender, EventArgs e) => SetValue(150);

        /// <summary>
        /// Sets the reading on the server. The Value setter validates the range; a
        /// rejected value is never rendered. Returns false when the value was rejected.
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

                return true;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                ShowAlarm($"✖ Rejected on the server: {ex.Message}", AlarmKind.Error);
                SetStatus("rejected", StatusKind.Error);
                AlertBox.Show(ex.Message, MessageBoxIcon.Error,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return false;
            }
        }

        #endregion

        #region Streaming

        private void buttonStream_Click(object sender, EventArgs e)
        {
            if (this.timerStream.Enabled)
            {
                StopStream();
                return;
            }

            _streamIndex = -1;
            this.buttonStream.Text = "■ Stop streaming";
            this.timerStream.Start();
            timerStream_Tick(sender, e);
        }

        private void timerStream_Tick(object sender, EventArgs e)
        {
            _streamIndex++;
            if (_streamIndex >= StreamReadings.Length)
            {
                StopStream();
                return;
            }

            SetStatus($"streaming {_streamIndex + 1}/{StreamReadings.Length}", StatusKind.Normal, keepColor: true);
            if (!SetValue(StreamReadings[_streamIndex]))
                StopStream();
        }

        private void StopStream()
        {
            this.timerStream.Stop();
            this.buttonStream.Text = "▶ Stream readings";
            SetStatus(this.gaugeTemperature.CurrentRange == "high" ? "alarm" : "idle",
                      this.gaugeTemperature.CurrentRange == "high" ? StatusKind.Error : StatusKind.Normal);
        }

        #endregion

        #region Corrupt payload and resync

        private void buttonCorrupt_Click(object sender, EventArgs e)
        {
            if (this.timerStream.Enabled)
                StopStream();

            this.gaugeTemperature.CorruptStateForTesting();
        }

        private void buttonResync_Click(object sender, EventArgs e)
        {
            this.gaugeTemperature.ResyncFromServer();
            bool high = this.gaugeTemperature.CurrentRange == "high";
            if (high)
                ShowAlarm("⚠ ThresholdExceeded raised on the server — operator notified", AlarmKind.Alarm);
            else
                HideAlarm();
            SetStatus(high ? "alarm" : "streaming", high ? StatusKind.Error : StatusKind.Normal);
        }

        #endregion

        #region Gauge events

        private void gaugeTemperature_ThresholdExceeded(object sender, GaugeEventArgs e)
        {
            ShowAlarm("⚠ ThresholdExceeded raised on the server — operator notified", AlarmKind.Alarm);
            SetStatus("alarm", StatusKind.Error);
            AlertBox.Show($"Boiler 3 reached {F(e.Value)}{this.gaugeTemperature.Units}. Operator notified.", MessageBoxIcon.Warning,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        private void gaugeTemperature_RangeChanged(object sender, GaugeEventArgs e)
        {
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
        }

        private void gaugeTemperature_WidgetError(object sender, GaugeErrorEventArgs e)
        {
            ShowAlarm($"✖ Gauge failure during {e.Phase}: {e.Message}", AlarmKind.Error);
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
            string prefix = direction == TraceDirection.ServerToClient ? "→ .NET→JS " : "← JS→.NET ";
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            this.listTrace.Items.Add($"{time}  {prefix} {name,-28} {payload}");
            this.listTrace.SelectedIndex = this.listTrace.Items.Count - 1;
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

        private static string F(double value) => value.ToString(CultureInfo.InvariantCulture);

        #endregion
    }
}
