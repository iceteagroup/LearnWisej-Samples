using System;
using System.Globalization;
using IntegrationLab.Contracts;
using IntegrationLab.Controls;
using Wisej.Web;

namespace IntegrationLab
{
    /// <summary>
    /// Gauge Commands: the SimpleGauge ("Boiler 3") with its four commands, and the command trace.
    ///
    /// Handlers that await are "async void" event handlers: the framework does not await them,
    /// so each one owns its try/catch and reports failures on the banner.
    /// </summary>
    public partial class Window1 : Form
    {
        public Window1()
        {
            InitializeComponent();

            this.gauge.ThresholdExceeded += gauge_ThresholdExceeded;
            this.gauge.RangeChanged += gauge_RangeChanged;
            this.gauge.WidgetError += gauge_WidgetError;
            this.gauge.Trace += gauge_Trace;
        }

        #region One-way commands: Call

        private void buttonSetValue72_Click(object sender, EventArgs e)
        {
            try
            {
                this.gauge.SetValue(72);          // validated on the server, then Call("setValue", 72)
                if (this.gauge.Value < this.gauge.Threshold)
                    HideAlarm();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                ShowAlarm("✖ " + ex.Message, AlarmKind.Error);
            }
        }

        private void buttonResetAnimation_Click(object sender, EventArgs e)
        {
            this.gauge.ResetAnimation();          // Call("resetAnimation")
        }

        #endregion

        #region Awaited commands: CallAsync

        private async void buttonReadSize_Click(object sender, EventArgs e)
        {
            try
            {
                RenderedSize size = await this.gauge.GetRenderedSizeAsync();

                // The next statement needs the value: choose the caption layout from the rendered width.
                this.gauge.Caption = size.Width >= 400 ? "Boiler 3 · wide" : "Boiler 3 · compact";
            }
            catch (Exception ex)
            {
                ShowAlarm($"✖ {SimpleGauge.JsGetRenderedSize} failed: {ex.Message}", AlarmKind.Error);
            }
            finally
            {
                // The code after await can finish after the click's request has returned: push the changes now.
                Application.Update(this);
            }
        }

        private async void buttonGetState_Click(object sender, EventArgs e)
        {
            try
            {
                GaugeStateDto state = await this.gauge.GetSelectedStateAsync();
                if (!state.IsAboveThreshold)
                    HideAlarm();
            }
            catch (Exception ex)
            {
                ShowAlarm($"✖ {SimpleGauge.JsGetSelectedState} failed: {ex.Message}", AlarmKind.Error);
            }
            finally
            {
                // The code after await can finish after the click's request has returned: push the changes now.
                Application.Update(this);
            }
        }

        #endregion

        #region Widget events

        private void gauge_ThresholdExceeded(object sender, GaugeEventArgs e)
        {
            ShowAlarm($"⚠ Boiler 3 above threshold: {F(e.Value)}{this.gauge.Units}", AlarmKind.Alarm);
        }

        private void gauge_RangeChanged(object sender, GaugeEventArgs e)
        {
            if (e.Range != "high")
                HideAlarm();
        }

        private void gauge_WidgetError(object sender, GaugeErrorEventArgs e)
        {
            ShowAlarm($"✖ Vendor failure during {e.Phase}: {e.Message}", AlarmKind.Error);
        }

        private void gauge_Trace(object sender, TraceEventArgs e)
        {
            string prefix = e.Direction == TraceDirection.ServerToClient ? "→ .NET→JS " : "← JS→.NET ";
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            this.listTrace.Items.Add($"{time}  {prefix} {e.Name} {e.Payload}".TrimEnd());
            this.listTrace.SelectedIndex = this.listTrace.Items.Count - 1;
        }

        #endregion

        #region Banner

        private enum AlarmKind { Alarm, Error }

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
