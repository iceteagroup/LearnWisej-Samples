using System;
using System.Globalization;
using IntegrationLab.Contracts;
using IntegrationLab.Controls;
using Wisej.Web;

namespace IntegrationLab
{
    /// <summary>
    /// Gauge Commands — Module 6 lab window.
    ///
    /// Left card:  the SimpleGauge Control ("Boiler 3") with the four lesson commands under it.
    /// Right card: every command and result that crosses the wire, with timestamps, so the
    ///             difference between a queued Call and an awaited CallAsync/EvalAsync is visible.
    /// Bottom bar: the alarm value, the EvalAsync variant, two failure paths (camel-case pitfall,
    ///             leaked domain object), the progress path (stream) and Clear.
    ///
    /// Handlers that await are "async void" event handlers: the framework does not await them,
    /// so every one of them owns its try/catch and reports failures to the trace and the banner.
    /// </summary>
    public partial class Window1 : Form
    {
        // Values the "Stream values" button replays with one-way Call commands: climbs through
        // warm, crosses the threshold once, then cools down.
        private static readonly double[] StreamReadings =
            { 72, 78, 86, 93, 99, 104, 101, 95, 88, 80, 74 };

        private int _streamIndex = -1;

        public Window1()
        {
            InitializeComponent();

            // Widget events → .NET events (the contract in action).
            this.gauge.ThresholdExceeded += gauge_ThresholdExceeded;
            this.gauge.RangeChanged += gauge_RangeChanged;
            this.gauge.WidgetError += gauge_WidgetError;
            this.gauge.LeakDetected += gauge_LeakDetected;
            this.gauge.Trace += gauge_Trace;
        }

        private void Window1_Load(object sender, EventArgs e)
        {
            // First render ships the whole Options object to init(options); afterwards only changed
            // fields and queued commands travel.
            AddTrace(TraceDirection.ServerToClient, "render → init(options)", this.gauge.ToJson());
            SetStatus("idle", StatusKind.Normal);
            UpdateStateLabel();
        }

        #region One-way commands: Call (server does not wait)

        private void buttonSetValue72_Click(object sender, EventArgs e) => DoSetValue(72);
        private void buttonSet104_Click(object sender, EventArgs e) => DoSetValue(104);

        /// <summary>
        /// The only way the UI changes the reading. Validation happens on the server before anything
        /// is queued. Returns false when the value was rejected.
        /// </summary>
        private bool DoSetValue(double value)
        {
            try
            {
                this.gauge.SetValue(value);                    // Options update + Call("setValue", v), both queued
                AddTrace(TraceDirection.Server, "next statement", "runs immediately — nothing was awaited");

                if (value < this.gauge.Threshold)
                    HideAlarm();

                UpdateStateLabel();
                return true;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // Server-side rejection: nothing crossed the wire, the browser keeps the last good value.
                AddTrace(TraceDirection.Server, "rejected", ex.Message);
                ShowAlarm($"✖ Rejected on the server: {ex.Message}", AlarmKind.Error);
                SetStatus("rejected", StatusKind.Error);
                AlertBox.Show(ex.Message, MessageBoxIcon.Error,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return false;
            }
        }

        private void buttonResetAnimation_Click(object sender, EventArgs e)
        {
            this.gauge.ResetAnimation();                       // Call("resetAnimation"), queued
            AddTrace(TraceDirection.Server, "next statement", "runs immediately — nothing was awaited");
        }

        #endregion

        #region Awaited commands: CallAsync / EvalAsync (the next statement needs the value)

        private async void buttonReadSize_Click(object sender, EventArgs e)
        {
            BeginAwait();
            try
            {
                RenderedSize size = await this.gauge.GetRenderedSizeAsync();

                // The next statement cannot run without the answer: a layout decision.
                string layout = size.Width >= 400 ? "wide" : "compact";
                this.gauge.Caption = $"Boiler 3 · {layout}";
                this.gauge.TraceStateOut("update(options)", $"{{\"label\":\"Boiler 3 · {layout}\"}}");
                AddTrace(TraceDirection.Server, "next statement", $"RenderedSize {size} → \"{layout}\" layout chosen, Caption updated");

                ShowResult($"RenderedSize ← await CallAsync(\"{SimpleGauge.JsGetRenderedSize}\")", SimpleGauge.ToWireJson(size));
                EndAwait();
            }
            catch (Exception ex)
            {
                FailAwait(SimpleGauge.JsGetRenderedSize, ex);
            }
        }

        private async void buttonEvalWidth_Click(object sender, EventArgs e)
        {
            BeginAwait();
            try
            {
                double width = await this.gauge.GetWidthViaEvalAsync();

                AddTrace(TraceDirection.Server, "next statement", $"width {F(width)} px → {(width >= 400 ? "wide" : "compact")} (same decision, EvalAsync variant)");
                ShowResult($"double ← await EvalAsync(\"{SimpleGauge.JsGetWidthExpression}\")", F(width));
                EndAwait();
            }
            catch (Exception ex)
            {
                FailAwait("getWidth", ex);
            }
        }

        private async void buttonGetState_Click(object sender, EventArgs e)
        {
            BeginAwait();
            try
            {
                GaugeStateDto state = await this.gauge.GetSelectedStateAsync();

                AddTrace(TraceDirection.Server, "next statement",
                    $"GaugeStateDto mapped: Value={F(state.Value)} IsAboveThreshold={state.IsAboveThreshold} " +
                    $"{state.Width}×{state.Height} IsAnimating={state.IsAnimating}");
                ShowResult($"GaugeStateDto ← await CallAsync(\"{SimpleGauge.JsGetSelectedState}\")", SimpleGauge.ToWireJson(state));
                EndAwait(state.IsAnimating ? "animating (client-only state)" : null);
            }
            catch (Exception ex)
            {
                FailAwait(SimpleGauge.JsGetSelectedState, ex);
            }
        }

        #endregion

        #region Failure paths

        private async void buttonPitfall_Click(object sender, EventArgs e)
        {
            // Same round trip twice: first read with Pascal-case names (wrong), then camelCase (right).
            BeginAwait();
            try
            {
                GaugeStateDto wrong = await this.gauge.GetSelectedStateWrongCaseAsync();
                GaugeStateDto right = await this.gauge.GetSelectedStateAsync();

                ShowResult("Pascal-case read (result.Width / result.Value) vs camelCase read",
                    $"wrong: {SimpleGauge.ToWireJson(wrong)}\nright: {SimpleGauge.ToWireJson(right)}");
                ShowAlarm("✖ Camel-case pitfall: result.Width is undefined on the wire — no error, just zeros. Read result.width, or map once into GaugeStateDto.", AlarmKind.Error);
                EndAwait("pitfall shown");
            }
            catch (Exception ex)
            {
                FailAwait(SimpleGauge.JsGetSelectedState, ex);
            }
        }

        private void buttonLeak_Click(object sender, EventArgs e)
        {
            if (this.timerStream.Enabled)
                StopStream("stopped before the leak demo");

            // A domain object goes into Options on purpose. The client adapter measures what it
            // received and reports it back; the LeakDetected handler below cleans up.
            int chars = this.gauge.LeakDomainObjectForTesting();
            ShowAlarm($"⚠ A DomainWorkOrder ({chars} chars, Customer.TaxId and CreditLimit included) is being shipped to the browser — waiting for the client to confirm…", AlarmKind.Alarm);
            SetStatus("leak in flight", StatusKind.Error);
        }

        #endregion

        #region Progress path: a Timer streams one-way commands

        private void buttonStream_Click(object sender, EventArgs e)
        {
            if (this.timerStream.Enabled)
            {
                StopStream("stopped by operator");
                return;
            }

            _streamIndex = -1;
            this.buttonStream.Text = "■ Stop streaming";
            AddTrace(TraceDirection.Server, "stream", $"replaying {StreamReadings.Length} values every {this.timerStream.Interval} ms with Call(\"setValue\")");
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

            SetStatus($"streaming {_streamIndex + 1}/{StreamReadings.Length} — click “Get selected state” to catch isAnimating: true", StatusKind.Normal, keepColor: true);
            if (!DoSetValue(StreamReadings[_streamIndex]))
                StopStream("aborted after a rejected value");
        }

        private void StopStream(string reason)
        {
            this.timerStream.Stop();
            this.buttonStream.Text = "▶ Stream values";
            AddTrace(TraceDirection.Server, "stream", reason);
            SetStatus(this.gauge.CurrentRange == "high" ? "alarm" : "idle",
                      this.gauge.CurrentRange == "high" ? StatusKind.Error : StatusKind.Normal);
        }

        #endregion

        #region Widget → .NET events

        private void gauge_ThresholdExceeded(object sender, GaugeEventArgs e)
        {
            AddTrace(TraceDirection.Server, "ThresholdExceeded fired in C#",
                $"Value={F(e.Value)} (client reported {F(e.ReportedValue)})");
            ShowAlarm("⚠ ThresholdExceeded raised on the server — operator notified", AlarmKind.Alarm);
            SetStatus("alarm", StatusKind.Error);
        }

        private void gauge_RangeChanged(object sender, GaugeEventArgs e)
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
                    SetStatus("idle", StatusKind.Normal, keepText: this.timerStream.Enabled);
                    HideAlarm();
                    break;
            }
            UpdateStateLabel();
        }

        private void gauge_WidgetError(object sender, GaugeErrorEventArgs e)
        {
            AddTrace(TraceDirection.Server, "WidgetError fired in C#", $"phase={e.Phase}");
            ShowAlarm($"✖ Vendor failure during {e.Phase}: {e.Message}", AlarmKind.Error);
            SetStatus("fault", StatusKind.Error);
        }

        private void gauge_LeakDetected(object sender, LeakDetectedEventArgs e)
        {
            // The browser confirmed what it received. Recovery: take the object back out of Options.
            AddTrace(TraceDirection.Server, "LeakDetected fired in C#", $"browser holds {e.Bytes} bytes, {e.Keys} keys incl. {e.Sample}");
            this.gauge.RemoveLeakedObject();
            ShowAlarm($"✖ Leak confirmed by the browser: {e.Bytes} bytes incl. {e.Sample}. Removed from Options — ship a DTO built for the crossing instead.", AlarmKind.Error);
            SetStatus("recovered", StatusKind.Warn);
        }

        private void gauge_Trace(object sender, TraceEventArgs e)
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
            this.listTrace.Items.Add($"{time}  {prefix} {name,-34} {payload}");
            this.listTrace.SelectedIndex = this.listTrace.Items.Count - 1;
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.listTrace.Items.Clear();
        }

        private void BeginAwait()
        {
            SetStatus("awaiting the browser… (handler suspended)", StatusKind.Warn);
        }

        private void EndAwait(string status = null)
        {
            bool high = this.gauge.CurrentRange == "high";
            SetStatus(status ?? (high ? "alarm" : "idle"), status != null ? StatusKind.Normal : (high ? StatusKind.Error : StatusKind.Normal));
            UpdateStateLabel();
        }

        private void FailAwait(string what, Exception ex)
        {
            AddTrace(TraceDirection.Server, "await failed", $"{what}: {ex.GetType().Name}: {ex.Message}");
            ShowAlarm($"✖ {what} failed: {ex.Message}", AlarmKind.Error);
            SetStatus("fault", StatusKind.Error);
        }

        private void ShowResult(string title, string json)
        {
            this.labelDto.Text = $"{title}\n{json}";
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
            var g = this.gauge;
            this.labelState.Text =
                $"SERVER STATE (authoritative)   Value={F(g.Value)}  Min={F(g.Minimum)}  Max={F(g.Maximum)}  Threshold={F(g.Threshold)}\n" +
                $"range={g.CurrentRange}   IsAboveThreshold={g.IsAboveThreshold}   widget loaded={g.IsLoaded}   (isAnimating is client-only: ask for it)";
        }

        private static string F(double value) => value.ToString(CultureInfo.InvariantCulture);

        #endregion
    }
}
