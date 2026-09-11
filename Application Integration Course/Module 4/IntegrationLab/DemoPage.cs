using System;
using System.Globalization;
using IntegrationLab.Controls;
using Wisej.Web;

namespace IntegrationLab
{
    /// <summary>
    /// Demo page: one SimpleGauge dropped from the Toolbox (see DemoPage.Designer.cs), configured
    /// and updated through typed properties only. This page contains no JavaScript, no InitScript,
    /// no Packages and no vendor name: it sets properties and handles .NET events.
    /// </summary>
    public partial class DemoPage : Page
    {
        public DemoPage()
        {
            InitializeComponent();
        }

        #region Buttons: normal .NET properties

        private void buttonValue72_Click(object sender, EventArgs e)
            => Run(() => this.simpleGauge1.Value = 72, "simpleGauge1.Value = 72;");

        private void buttonValue90_Click(object sender, EventArgs e)
            => Run(() => this.simpleGauge1.Value = 90, "simpleGauge1.Value = 90;");

        private void buttonMax120_Click(object sender, EventArgs e)
            => Run(() =>
            {
                this.simpleGauge1.Maximum = 120;
                this.simpleGauge1.Value = 45;
            }, "simpleGauge1.Maximum = 120;", "simpleGauge1.Value = 45;");

        private void buttonValue78_Click(object sender, EventArgs e)
            => Run(() => this.simpleGauge1.Value = 78, "simpleGauge1.Value = 78;");

        /// <summary>
        /// Runs typed-property statements. Validation happens in the wrapper, on the server,
        /// before anything is rendered: an invalid value is rejected and the gauge keeps its state.
        /// </summary>
        private void Run(Action action, params string[] code)
        {
            foreach (string line in code)
                AddCode(line);

            try
            {
                action();
                if (this.labelAlarm.Visible && this.labelAlarm.Tag as string == "error")
                    HideAlarm();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                AddCode("✖ " + ex.Message);
                ShowAlarm($"✖ {ex.Message}", AlarmKind.Error);
            }
        }

        #endregion

        #region SimpleGauge events

        private void simpleGauge1_ValueChanged(object sender, GaugeValueChangedEventArgs e)
        {
            AddCode($"✓ gauge updated · Value = {F(e.Value)}");

            if (e.Value < this.simpleGauge1.Threshold && this.labelAlarm.Visible && this.labelAlarm.Tag as string == "alarm")
                HideAlarm();
        }

        private void simpleGauge1_ThresholdExceeded(object sender, GaugeEventArgs e)
        {
            ShowAlarm($"⚠ {this.simpleGauge1.Caption} reached {F(e.Value)} (threshold {F(this.simpleGauge1.Threshold)}).", AlarmKind.Alarm);
        }

        private void simpleGauge1_WidgetError(object sender, GaugeErrorEventArgs e)
        {
            ShowAlarm($"✖ Gauge failure during {e.Phase}: {e.Message}", AlarmKind.Error);
        }

        #endregion

        #region UI helpers

        private enum AlarmKind { Alarm, Error }

        private void AddCode(string line)
        {
            this.listCodeBehind.Items.Add(line);
            this.listCodeBehind.SelectedIndex = this.listCodeBehind.Items.Count - 1;
        }

        private void ShowAlarm(string text, AlarmKind kind)
        {
            this.labelAlarm.Text = text;
            this.labelAlarm.Tag = kind == AlarmKind.Alarm ? "alarm" : "error";
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
            this.labelAlarm.Tag = null;
        }

        private static string F(double value) => value.ToString(CultureInfo.InvariantCulture);

        #endregion
    }
}
