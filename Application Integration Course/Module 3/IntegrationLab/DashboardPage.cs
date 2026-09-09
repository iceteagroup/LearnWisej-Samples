using System;
using System.Globalization;
using Wisej.Core;
using Wisej.Web;

namespace IntegrationLab
{
    /// <summary>
    /// Dashboard — Module 3 lab page (Rapid Widget Integration with Wisej.Web.Widget).
    ///
    /// Two plain Wisej.Web.Widget instances (no wrapper class): a VendorGauge and a jQuery-style
    /// VendorKnob. Each is configured in the designer file with Packages (JS + CSS), Options
    /// (.NET values serialized to the client) and an InitScript (init / update / a callable method).
    ///
    /// Left card:  the two widgets, banner, and the server-owned state.
    /// Right card: every message that crosses the wire, in both directions.
    /// Bottom bar: server buttons that change Options and queue a client Call (success path),
    ///             a Timer stream (progress), a nested change without notify (failure) and
    ///             Update() (recovery), and the destroy-and-recreate case.
    /// </summary>
    public partial class DashboardPage : Page
    {
        // Values the "Stream" button replays through both widgets: 20 → 95 → 20.
        private static readonly double[] StreamValues =
            { 20, 30, 40, 50, 60, 70, 80, 88, 95, 88, 80, 70, 60, 50, 40, 30, 20 };

        // ---- server-owned state (authoritative) -------------------------------------------
        private double _gaugeValue = 72;
        private double _knobLevel = 60;
        private string _style = "card";
        private string _bandsName = "default";
        private bool _nestedChangePending;
        private int _streamIndex = -1;

        public DashboardPage()
        {
            InitializeComponent();

            // Widget events → page logic. A plain Widget raises WidgetEvent for every
            // fireWidgetEvent(...) on the client; e.Type is the name, e.Data the payload.
            this.gauge.WidgetEvent += gauge_WidgetEvent;
            this.knob.WidgetEvent += knob_WidgetEvent;
        }

        #region Options helpers (the only way the page writes widget state)

        /// <summary>
        /// The colored bands the gauge shows. Built as Wisej.Core.DynamicObject instances rather
        /// than anonymous objects because anonymous-type properties are read-only, and the lab's
        /// failure path mutates a band in place (bands[0].color = …) to show that nested changes
        /// are not detected. Replacing the whole array is a first-level change and travels.
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
        private void ApplySetPoint(string name, double gaugeValue, double knobLevel, DynamicObject[] bands, string bandsName)
        {
            _gaugeValue = gaugeValue;
            _knobLevel = knobLevel;

            AddTrace(TraceDirection.Server, $"Set \"{name}\"",
                $"Options = {{ value: {F(gaugeValue)}, level: {F(knobLevel)} }}  →  update(options)");

            dynamic gaugeOptions = this.gauge.Options;
            gaugeOptions.value = gaugeValue;
            AddTrace(TraceDirection.ServerToClient, "gauge.Options", LabJson.Of(new { value = gaugeValue }));

            if (bands != null)
            {
                _bandsName = bandsName;
                gaugeOptions.bands = bands;               // whole array replaced = first-level change
                AddTrace(TraceDirection.ServerToClient, "gauge.Options", LabJson.Of(new { bands }));
            }

            dynamic knobOptions = this.knob.Options;
            knobOptions.level = knobLevel;
            AddTrace(TraceDirection.ServerToClient, "knob.Options", LabJson.Of(new { level = knobLevel }));

            _nestedChangePending = false;
            HideAlarm();
            UpdateStateLabel();
        }

        /// <summary>
        /// Call(...) names a function defined by the InitScript ("this.flash = function…") and
        /// runs it in the wrapper's context. It is queued and flushed with the next response,
        /// together with the Options changes made in the same request — in order. That is why
        /// every button sets the Options FIRST and issues the Call SECOND: the batch is applied
        /// in order, so the client method sees the updated options.
        /// </summary>
        private void QueueCall(Widget widget, string function)
        {
            widget.Call(function);
            AddTrace(TraceDirection.ServerToClient, $"{widget.Name}.Call(\"{function}\")", "— queued to client");
        }

        #endregion

        #region Success path: a server button changes Options and calls the client

        private void buttonHigh_Click(object sender, EventArgs e)
        {
            StopStreamIfRunning();
            ApplySetPoint("High", 88, 78, null, _bandsName);   // 1. state
            QueueCall(this.gauge, "flash");                     // 2. imperative method — same batch, applied after
            SetStatus("high", StatusKind.Warn);
        }

        private void buttonPeak_Click(object sender, EventArgs e)
        {
            StopStreamIfRunning();
            ApplySetPoint("Peak", 104, 92, PeakBands(), "peak");   // also replaces the bands array
            QueueCall(this.knob, "pulse");
            SetStatus("peak", StatusKind.Error);
        }

        private void buttonIdle_Click(object sender, EventArgs e)
        {
            StopStreamIfRunning();
            ApplySetPoint("Idle", 64, 40, DefaultBands(), "default");
            QueueCall(this.gauge, "flash");
            SetStatus("idle", StatusKind.Normal);
        }

        #endregion

        #region Progress path: a Timer streams values through Options

        private void buttonStream_Click(object sender, EventArgs e)
        {
            if (this.timerStream.Enabled)
            {
                StopStream("stopped by operator");
                return;
            }

            _streamIndex = -1;
            this.buttonStream.Text = "■ Stop";
            AddTrace(TraceDirection.Server, "stream", $"replaying {StreamValues.Length} values every {this.timerStream.Interval} ms");
            this.timerStream.Start();
            timerStream_Tick(sender, e);
        }

        private void timerStream_Tick(object sender, EventArgs e)
        {
            _streamIndex++;
            if (_streamIndex >= StreamValues.Length)
            {
                StopStream("complete");
                return;
            }

            double v = StreamValues[_streamIndex];
            _gaugeValue = v;
            _knobLevel = Math.Min(100, v);

            // Only the top-level fields change: the client's update() touches value/level and
            // nothing else (no bands re-sync, no recreation) — the "old" comparison at work.
            dynamic gaugeOptions = this.gauge.Options;
            gaugeOptions.value = _gaugeValue;
            dynamic knobOptions = this.knob.Options;
            knobOptions.level = _knobLevel;
            AddTrace(TraceDirection.ServerToClient, "stream → update(options)",
                $"gauge {LabJson.Of(new { value = _gaugeValue })}   knob {LabJson.Of(new { level = _knobLevel })}");

            SetStatus($"streaming {_streamIndex + 1}/{StreamValues.Length}", StatusKind.Normal);
            UpdateStateLabel();
        }

        private void StopStream(string reason)
        {
            this.timerStream.Stop();
            this.buttonStream.Text = "▶ Stream";
            AddTrace(TraceDirection.Server, "stream", reason);
            SetStatus("idle", StatusKind.Normal);
        }

        private void StopStreamIfRunning()
        {
            if (this.timerStream.Enabled)
                StopStream("stopped by a set-point button");
        }

        #endregion

        #region Failure path: nested change without notify — and recovery with Update()

        private void buttonNested_Click(object sender, EventArgs e)
        {
            // The Widget detects changes to FIRST-LEVEL fields only. Mutating a nested object in
            // place changes the server copy, but nothing is rendered: the client keeps the old
            // color until Update() (or Options.Notify("bands")) is called.
            try
            {
                dynamic gaugeOptions = this.gauge.Options;
                string current = (string)gaugeOptions.bands[0].color;
                string next = current == "#1a86ff" ? "#1f9d57" : "#1a86ff";
                gaugeOptions.bands[0].color = next;                 // nested: NOT detected

                _nestedChangePending = true;
                AddTrace(TraceDirection.Server, "nested change",
                    $"gauge.Options.bands[0].color = \"{next}\"  (nested field: nothing sent, no update() on the client)");
                ShowAlarm($"✖ bands[0].color is now \"{next}\" on the server, but the legend under the gauge did not change: nested changes are not detected. Click “Notify / Update()”.", AlarmKind.Error);
                SetStatus("out of sync", StatusKind.Error);
                UpdateStateLabel();
            }
            catch (Exception ex)
            {
                AddTrace(TraceDirection.Server, "nested change failed", ex.Message);
                ShowAlarm($"✖ Could not mutate bands[0] in place: {ex.Message}", AlarmKind.Error);
            }
        }

        private void buttonNotify_Click(object sender, EventArgs e)
        {
            // Recovery: Update() re-renders the widget's Options as a whole, so the client gets
            // update(options, old) and its "old" comparison finds the changed band.
            // Alternative with the same effect: ((dynamic)this.gauge.Options).Notify("bands").
            this.gauge.Update();
            AddTrace(TraceDirection.ServerToClient, "gauge.Update()",
                $"re-render → update(options, old)   bands = {LabJson.Of(((dynamic)this.gauge.Options).bands)}");

            if (_nestedChangePending)
            {
                _nestedChangePending = false;
                ShowAlarm("✔ Update() pushed the nested change: the legend now shows the new color.", AlarmKind.Info);
            }
            else
            {
                ShowAlarm("ℹ Nothing was pending: Update() re-sent the current Options (harmless, the client re-syncs).", AlarmKind.Info);
            }
            SetStatus("in sync", StatusKind.Normal);
            UpdateStateLabel();
        }

        #endregion

        #region Destroy & recreate: the option the vendor cannot take after construction

        private void buttonRecreate_Click(object sender, EventArgs e)
        {
            // "style" selects which host element the vendor is bound to. VendorGauge binds to its
            // element in the constructor and has no re-parent API, so the InitScript treats a
            // style change as "destroy and recreate" — only for this field, only when it changed.
            _style = _style == "card" ? "compact" : "card";
            dynamic gaugeOptions = this.gauge.Options;
            gaugeOptions.style = _style;
            AddTrace(TraceDirection.ServerToClient, "gauge.Options", LabJson.Of(new { style = _style }) + "   (InitScript: destroy + new VendorGauge)");
            SetStatus("recreating…", StatusKind.Warn);
            UpdateStateLabel();
        }

        #endregion

        #region Widget → page events

        private void gauge_WidgetEvent(object sender, WidgetEventArgs e)
        {
            dynamic data = e.Data;
            switch (e.Type)
            {
                case "initialized":
                    // Camel-casing evidence straight from the browser: the keys as they arrived.
                    AddTrace(TraceDirection.ClientToServer, "initialized", LabJson.Of(e.Data));
                    AddTrace(TraceDirection.Server, "camel-casing",
                        "C# new { MinValue, MaxValue } arrived as range.minValue / range.maxValue");
                    UpdateStateLabel();
                    break;

                case "recreated":
                    AddTrace(TraceDirection.ClientToServer, "recreated", LabJson.Of(e.Data));
                    ShowAlarm($"↻ Vendor instance destroyed and recreated for style = \"{(string)data?.style}\" — all other options were re-applied from the full Options object.", AlarmKind.Info);
                    SetStatus("recreated", StatusKind.Normal);
                    UpdateStateLabel();
                    break;

                case "error":
                    AddTrace(TraceDirection.ClientToServer, "error", LabJson.Of(e.Data));
                    ShowAlarm($"✖ Gauge adapter caught a vendor failure during {(string)data?.phase}: {(string)data?.message}", AlarmKind.Error);
                    SetStatus("fault", StatusKind.Error);
                    break;

                default:
                    AddTrace(TraceDirection.ClientToServer, e.Type, LabJson.Of(e.Data));
                    break;
            }
        }

        private void knob_WidgetEvent(object sender, WidgetEventArgs e)
        {
            dynamic data = e.Data;
            switch (e.Type)
            {
                case "valueChanged":
                    {
                        // User drag / wheel. The server validates and takes ownership of the value:
                        // it becomes the server's level and is written back to Options so the next
                        // full render (Update()) cannot snap the knob back to a stale value.
                        double reported = ToDouble(data?.value);
                        AddTrace(TraceDirection.ClientToServer, "valueChanged", LabJson.Of(e.Data));
                        if (double.IsNaN(reported))
                        {
                            AddTrace(TraceDirection.Server, "rejected", "valueChanged payload is not numeric: ignored");
                            break;
                        }

                        _knobLevel = Math.Max(0, Math.Min(100, reported));
                        dynamic knobOptions = this.knob.Options;
                        knobOptions.level = _knobLevel;
                        SetStatus($"knob {F(_knobLevel)}% (user)", StatusKind.Normal);
                        UpdateStateLabel();
                        break;
                    }

                case "error":
                    AddTrace(TraceDirection.ClientToServer, "error", LabJson.Of(e.Data));
                    ShowAlarm($"✖ Knob adapter caught a vendor failure during {(string)data?.phase}: {(string)data?.message}", AlarmKind.Error);
                    SetStatus("fault", StatusKind.Error);
                    break;

                default:
                    AddTrace(TraceDirection.ClientToServer, e.Type, LabJson.Of(e.Data));
                    break;
            }
        }

        #endregion

        #region UI helpers

        private enum TraceDirection { ServerToClient, ClientToServer, Server }
        private enum StatusKind { Normal, Warn, Error }
        private enum AlarmKind { Error, Info }

        private void DashboardPage_Load(object sender, EventArgs e)
        {
            // First render: each widget ships its Packages list, its whole Options object and its
            // InitScript; the client loads the packages in order, then calls init(options).
            AddTrace(TraceDirection.ServerToClient, "gauge render → init(options)", LabJson.Of(this.gauge.Options));
            AddTrace(TraceDirection.ServerToClient, "knob render → init(options)", LabJson.Of(this.knob.Options));
            SetStatus("idle", StatusKind.Normal);
            UpdateStateLabel();
        }

        private void AddTrace(TraceDirection direction, string name, string payload)
        {
            string prefix = direction switch
            {
                TraceDirection.ServerToClient => "→ .NET→JS ",
                TraceDirection.ClientToServer => "← JS→.NET ",
                _ => "• server  ",
            };
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            this.listTrace.Items.Add($"{time}  {prefix} {name,-30} {payload}");
            this.listTrace.SelectedIndex = this.listTrace.Items.Count - 1;
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.listTrace.Items.Clear();
        }

        private void SetStatus(string text, StatusKind kind)
        {
            this.labelStatus.Text = "● " + text;
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
            this.labelAlarm.BackColor = kind == AlarmKind.Error
                ? System.Drawing.Color.FromArgb(253, 236, 234)
                : System.Drawing.Color.FromArgb(234, 243, 255);
            this.labelAlarm.ForeColor = kind == AlarmKind.Error
                ? System.Drawing.Color.FromArgb(178, 59, 39)
                : System.Drawing.Color.FromArgb(21, 101, 216);
            this.labelAlarm.Visible = true;
        }

        private void HideAlarm()
        {
            this.labelAlarm.Visible = false;
        }

        private void UpdateStateLabel()
        {
            dynamic gaugeOptions = this.gauge.Options;
            string bands;
            try { bands = LabJson.Of(gaugeOptions.bands); }
            catch { bands = "?"; }

            this.labelState.Text =
                "SERVER STATE (authoritative)\n" +
                $"gauge: value={F(_gaugeValue)}  range=0..120  style={_style}  bands={_bandsName}  loaded={this.gauge.IsLoaded}" +
                (_nestedChangePending ? "  [nested change NOT sent]" : "") + "\n" +
                $"       bands={bands}\n" +
                $"knob:  level={F(_knobLevel)}  range=0..100 step=1  loaded={this.knob.IsLoaded}";
        }

        private static string F(double value) => value.ToString(CultureInfo.InvariantCulture);

        private static double ToDouble(object value)
        {
            try { return value == null ? double.NaN : Convert.ToDouble(value, CultureInfo.InvariantCulture); }
            catch { return double.NaN; }
        }

        #endregion
    }
}
