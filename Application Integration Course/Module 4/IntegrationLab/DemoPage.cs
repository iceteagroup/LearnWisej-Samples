using System;
using System.Globalization;
using IntegrationLab.Controls;
using Wisej.Web;

namespace IntegrationLab
{
    /// <summary>
    /// Demo — Module 4 lab page.
    ///
    /// Two SimpleGauge instances dropped from the Toolbox (see DemoPage.Designer.cs), configured with
    /// typed properties only. This file contains no JavaScript, no InitScript, no Packages and no
    /// vendor name: it sets properties and handles .NET events. If the vendor library were replaced
    /// tomorrow, this page would not change.
    ///
    /// Left card:  simpleGauge1 ("Boiler 3") and simpleGauge2 ("Chiller 1").
    /// Right card: every message that crosses the wire, in both directions (the wrapper reports it).
    /// Bottom bar: the exact statements from the walkthrough, a failure path, animation toggle, stream.
    /// </summary>
    public partial class DemoPage : Page
    {
        // Readings the "Stream both gauges" button replays. Boiler 3 crosses its threshold (85) once
        // on the way up; Chiller 1 crosses its own (50) once. Both stay inside their ranges.
        private static readonly double[] BoilerReadings =
            { 72, 76, 81, 85, 89, 94, 98, 96, 90, 84, 79, 74 };
        private static readonly double[] ChillerReadings =
            { 18, 22, 27, 33, 39, 44, 48, 52, 55, 51, 46, 40 };

        private int _streamIndex = -1;

        public DemoPage()
        {
            InitializeComponent();
        }

        private void DemoPage_Load(object sender, EventArgs e)
        {
            // Rendering: the first time a control is sent to the browser it ships its whole
            // Options object as compact JSON; afterwards only changed fields travel.
            AddTrace(TraceDirection.ServerToClient, "simpleGauge1.render → init", this.simpleGauge1.ToJson());
            AddTrace(TraceDirection.ServerToClient, "simpleGauge2.render → init", this.simpleGauge2.ToJson());
            AddTrace(TraceDirection.Server, "page", "no InitScript, no Packages, no vendor name in DemoPage.cs");
            SetStatus("ready", StatusKind.Normal);
            UpdateStateLabel();
        }

        #region Success path: the statements from the walkthrough, verbatim

        private void buttonValue72_Click(object sender, EventArgs e)
            => Run("simpleGauge1.Value = 72;", () => this.simpleGauge1.Value = 72);

        private void buttonValue90_Click(object sender, EventArgs e)
            => Run("simpleGauge1.Value = 90;", () => this.simpleGauge1.Value = 90);

        private void buttonMax120_Click(object sender, EventArgs e)
            => Run("simpleGauge1.Maximum = 120; simpleGauge1.Value = 45;", () =>
            {
                this.simpleGauge1.Maximum = 120;
                this.simpleGauge1.Value = 45;
            });

        private void buttonValue78_Click(object sender, EventArgs e)
            => Run("simpleGauge1.Value = 78;", () => this.simpleGauge1.Value = 78);

        /// <summary>
        /// The only way this page changes a gauge: a typed property. Validation happens in the
        /// wrapper, on the server, before anything is rendered. Returns false when it was rejected.
        /// </summary>
        private bool Run(string code, Action action)
        {
            AddTrace(TraceDirection.Server, "code-behind", code);
            try
            {
                action();
                if (this.labelAlarm.Visible && this.labelAlarm.Tag as string == "error")
                    HideAlarm();
                UpdateStateLabel();
                return true;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // Failure path: the typed property refuses invalid state. Nothing is rendered,
                // the browser keeps showing the last good value, the UI says why.
                AddTrace(TraceDirection.Server, "rejected", ex.Message);
                ShowAlarm($"✖ Rejected by the wrapper: {ex.Message}", AlarmKind.Error);
                SetStatus("rejected", StatusKind.Error);
                AlertBox.Show(ex.Message, MessageBoxIcon.Error,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return false;
            }
        }

        #endregion

        #region Failure path: the setter throws, the page stays alive

        private void buttonInvalid_Click(object sender, EventArgs e)
        {
            // 200 is above Maximum (100, or 120 after the third button): ArgumentOutOfRangeException
            // before any JSON is rendered.
            Run("simpleGauge1.Value = 200;", () => this.simpleGauge1.Value = 200);
        }

        #endregion

        #region Visual property: AnimationEnabled

        private void buttonToggleAnimation_Click(object sender, EventArgs e)
        {
            bool enabled = !this.simpleGauge1.AnimationEnabled;
            Run($"simpleGauge1.AnimationEnabled = {(enabled ? "true" : "false")}; simpleGauge2.AnimationEnabled = {(enabled ? "true" : "false")};", () =>
            {
                this.simpleGauge1.AnimationEnabled = enabled;
                this.simpleGauge2.AnimationEnabled = enabled;
            });
            this.buttonToggleAnimation.Text = enabled ? "AnimationEnabled = false" : "AnimationEnabled = true";
        }

        #endregion

        #region Progress path: a Timer streams readings into both gauges

        private void buttonStream_Click(object sender, EventArgs e)
        {
            if (this.timerStream.Enabled)
            {
                StopStream("stopped by operator");
                return;
            }

            _streamIndex = -1;
            this.buttonStream.Text = "■ Stop streaming";
            AddTrace(TraceDirection.Server, "stream",
                $"replaying {BoilerReadings.Length} readings into both gauges every {this.timerStream.Interval} ms");
            this.timerStream.Start();
            timerStream_Tick(sender, e);
        }

        private void timerStream_Tick(object sender, EventArgs e)
        {
            _streamIndex++;
            if (_streamIndex >= BoilerReadings.Length)
            {
                StopStream("complete");
                return;
            }

            double boiler = BoilerReadings[_streamIndex];
            double chiller = ChillerReadings[_streamIndex];
            SetStatus($"streaming {_streamIndex + 1}/{BoilerReadings.Length}", StatusKind.Normal, keepColor: true);

            bool ok = Run($"simpleGauge1.Value = {F(boiler)}; simpleGauge2.Value = {F(chiller)};", () =>
            {
                this.simpleGauge1.Value = boiler;
                this.simpleGauge2.Value = chiller;
            });
            if (!ok)
                StopStream("aborted after a rejected reading");
        }

        private void StopStream(string reason)
        {
            this.timerStream.Stop();
            this.buttonStream.Text = "▶ Stream both gauges";
            AddTrace(TraceDirection.Server, "stream", reason);
            SetStatus(AnyGaugeAboveThreshold() ? "alarm" : "ready",
                      AnyGaugeAboveThreshold() ? StatusKind.Error : StatusKind.Normal);
        }

        #endregion

        #region Widget → .NET events (wired in the designer, like any other control event)

        private void simpleGauge_ValueChanged(object sender, GaugeValueChangedEventArgs e)
        {
            var gauge = (SimpleGauge)sender;
            AddTrace(TraceDirection.Server, $"{gauge.Name}.ValueChanged fired in C#",
                $"Value={F(e.Value)}  (browser showed {F(e.Previous)}, now {F(e.ReportedValue)})");

            if (!this.timerStream.Enabled)
                SetStatus("gauge updated — wrapper handled the client call", StatusKind.Normal);

            // Alarm banner clears once every gauge is back under its threshold.
            if (!AnyGaugeAboveThreshold() && this.labelAlarm.Visible && this.labelAlarm.Tag as string == "alarm")
                HideAlarm();

            UpdateStateLabel();
        }

        private void simpleGauge_ThresholdExceeded(object sender, GaugeEventArgs e)
        {
            var gauge = (SimpleGauge)sender;
            AddTrace(TraceDirection.Server, $"{gauge.Name}.ThresholdExceeded fired in C#",
                $"Value={F(e.Value)} Threshold={F(gauge.Threshold)} (client reported {F(e.ReportedValue)})");
            ShowAlarm($"⚠ ThresholdExceeded raised on the server — {gauge.Caption} crossed {F(gauge.Threshold)}. Operator notified.", AlarmKind.Alarm);
            SetStatus("alarm", StatusKind.Error, keepText: this.timerStream.Enabled);
            AlertBox.Show($"{gauge.Caption} reached {F(e.Value)} (threshold {F(gauge.Threshold)}).", MessageBoxIcon.Warning,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        private void simpleGauge_WidgetError(object sender, GaugeErrorEventArgs e)
        {
            var gauge = (SimpleGauge)sender;
            AddTrace(TraceDirection.Server, $"{gauge.Name}.WidgetError fired in C#", $"phase={e.Phase}");
            ShowAlarm($"✖ Vendor failure inside the wrapper during {e.Phase}: {e.Message}", AlarmKind.Error);
            SetStatus("fault", StatusKind.Error);
        }

        private void simpleGauge_Trace(object sender, TraceEventArgs e)
            => AddTrace(e.Direction, $"{((Control)sender).Name}.{e.Name}", e.Payload);

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
            this.listTrace.Items.Add($"{time}  {prefix} {name,-34} {payload}");
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

        private bool AnyGaugeAboveThreshold()
            => this.simpleGauge1.Value >= this.simpleGauge1.Threshold
            || this.simpleGauge2.Value >= this.simpleGauge2.Threshold;

        private void UpdateStateLabel()
        {
            this.labelState.Text =
                "SERVER STATE (authoritative)\n" +
                Describe(this.simpleGauge1) + "\n" +
                Describe(this.simpleGauge2);
        }

        private static string Describe(SimpleGauge g)
            => $"{g.Name}: Value={F(g.Value)} Min={F(g.Minimum)} Max={F(g.Maximum)} Threshold={F(g.Threshold)} " +
               $"Caption=\"{g.Caption}\" Animation={g.AnimationEnabled} loaded={g.IsLoaded}";

        private static string F(double value) => value.ToString(CultureInfo.InvariantCulture);

        #endregion
    }
}
