using System;
using System.Globalization;
using System.IO;
using Wisej.Web;

namespace IntegrationLab
{
    public enum TraceDirection { ServerToClient, ClientToServer, Server }

    /// <summary>
    /// Knob Demo — Module 2 lab window (JavaScript Essentials).
    ///
    /// Left card:  two plain Wisej.Web.Widget instances hosting the same jQuery-style VendorKnob plugin.
    ///             gaugeKnob        runs the context-safe InitScript  (wwwroot/gauge-init.js:        var me = this)
    ///             gaugeKnobBroken  runs the broken InitScript        (wwwroot/gauge-init.broken.js: this.fireWidgetEvent
    ///                              inside the vendor callback — the event never arrives, a contextError does)
    ///             plus an empty slot where "Wrong package order" creates a third widget at runtime.
    /// Right card: every message that crosses the wire, in both directions.
    /// Bottom bar: success path (Set 25/60/85), progress path (Stream), failure paths (turn the broken
    ///             knob, wrong package order), the plain-JS proof page, clear.
    /// </summary>
    public partial class Window1 : Form
    {
        // Values the "Stream" button replays through the knob (a Timer drives them).
        private static readonly double[] StreamReadings =
            { 40, 48, 57, 66, 74, 81, 88, 93, 96, 90, 80, 68, 55, 45 };

        private int _streamIndex = -1;

        // The server copy of the pressure value: the knob is an input control, so the server
        // records what the user turned it to (valueChanged) and pushes what the buttons set.
        private double _value = 40;

        private Widget _wrongOrderKnob;
        private bool _wrongOrderReportedError;

        public Window1()
        {
            InitializeComponent();

            // The Designer declares Packages, WiredEvents, size and events (Window1.Designer.cs).
            // The InitScript text and the Options object are set here: the Designer would keep the
            // same script in a .resx; reading the embedded resource keeps the .js file editable.
            this.gaugeKnob.InitScript = LoadScript("IntegrationLab.wwwroot.gauge-init.js");
            SetKnobOptions(this.gaugeKnob, "gaugeKnob", 40);

            this.gaugeKnobBroken.InitScript = LoadScript("IntegrationLab.wwwroot.gauge-init.broken.js");
            SetKnobOptions(this.gaugeKnobBroken, "gaugeKnobBroken", 40);
        }

        private void Window1_Load(object sender, EventArgs e)
        {
            // Rendering: the first response ships the whole Options object as compact JSON and the
            // client calls init(options) once the Packages have loaded, in order.
            AddTrace(TraceDirection.ServerToClient, "render gaugeKnob → init(options)", OptionsJson("gaugeKnob", _value));
            AddTrace(TraceDirection.Server, "packages (gaugeKnob)", "jquery-lite.js → vendor-knob.css → vendor-knob.js → InitScript gauge-init.js");
            AddTrace(TraceDirection.ServerToClient, "render gaugeKnobBroken → init(options)", OptionsJson("gaugeKnobBroken", _value));
            AddTrace(TraceDirection.Server, "packages (gaugeKnobBroken)", "same packages, already cached by name → loaded once; InitScript gauge-init.broken.js");
            SetStatus("ready · turn a knob", StatusKind.Normal);
            UpdateStateLabel();
        }

        #region Success path: the server sets Options.value, the client runs update(options, old)

        private void buttonSet25_Click(object sender, EventArgs e) => SetValue(25);
        private void buttonSet60_Click(object sender, EventArgs e) => SetValue(60);
        private void buttonSet85_Click(object sender, EventArgs e) => SetValue(85);

        /// <summary>
        /// The only way the UI moves the knobs from the server. Both widgets receive the same
        /// first-level Options change, so both re-render — update() is a widget method and is
        /// unaffected by the callback bug, which only bites on the way back (events).
        /// </summary>
        private void SetValue(double value)
        {
            if (value < 0 || value > 100)
            {
                AddTrace(TraceDirection.Server, "rejected", $"{F(value)} is outside 0..100 — nothing rendered");
                return;
            }

            _value = value;
            dynamic good = this.gaugeKnob.Options;
            good.value = value;
            dynamic broken = this.gaugeKnobBroken.Options;
            broken.value = value;

            AddTrace(TraceDirection.ServerToClient, "update(options) ×2", $"{{\"value\":{F(value)}}}  (gaugeKnob, gaugeKnobBroken)");
            HideAlarm();
            SetStatus($"set to {F(value)} psi from the server", StatusKind.Normal, keepText: this.timerStream.Enabled);
            UpdateStateLabel();
        }

        #endregion

        #region Progress path: a Timer streams readings through the knob

        private void buttonStream_Click(object sender, EventArgs e)
        {
            if (this.timerStream.Enabled)
            {
                StopStream("stopped by operator");
                return;
            }

            _streamIndex = -1;
            this.buttonStream.Text = "■ Stop";
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

            SetStatus($"streaming {_streamIndex + 1}/{StreamReadings.Length}", StatusKind.Normal);
            SetValue(StreamReadings[_streamIndex]);
        }

        private void StopStream(string reason)
        {
            this.timerStream.Stop();
            this.buttonStream.Text = "▶ Stream";
            AddTrace(TraceDirection.Server, "stream", reason);
            SetStatus("idle", StatusKind.Normal);
        }

        #endregion

        #region Failure path 2: a widget whose Packages list the plugin before jQuery

        private void buttonWrongOrder_Click(object sender, EventArgs e)
        {
            if (!this.gaugeKnob.IsLoaded || !this.gaugeKnobBroken.IsLoaded)
            {
                AddTrace(TraceDirection.Server, "wrong order", "wait until both knobs report loaded — the demo must not interfere with their package chain");
                SetStatus("knobs still loading — try again in a moment", StatusKind.Warn);
                return;
            }

            if (_wrongOrderKnob != null)
            {
                AddTrace(TraceDirection.Server, "wrongOrder", "already created — reload the page to run it again");
                return;
            }

            if (this.timerStream.Enabled)
                StopStream("stopped before the wrong-order test");

            // A third plain Widget, created at runtime, with the plugin listed BEFORE jQuery.
            // Package names differ from gaugeKnob's on purpose: Wisej.NET caches packages by name
            // and would otherwise not load them again. hide-jquery.js parks the jQuery
            // global first, because this page already has it (see the file header).
            var knob = new Widget();
            knob.Name = "gaugeKnobWrongOrder";
            knob.Dock = DockStyle.Fill;
            knob.Packages.Add(new Widget.Package { Name = "hide-jquery", Source = "wwwroot/hide-jquery.js" });   // lab prop: parks the jQuery globals so the plugin really runs without them
            knob.Packages.Add(new Widget.Package { Name = "vendor-knob-before-jquery", Source = "wwwroot/vendor-knob.js?order=wrong" });
            knob.Packages.Add(new Widget.Package { Name = "vendor-knob-css-wrong", Source = "wwwroot/vendor-knob.css?order=wrong" });
            knob.Packages.Add(new Widget.Package { Name = "jquery-lite-after-plugin", Source = "wwwroot/jquery-lite.js?order=wrong" });
            knob.WiredEvents = new[] { "valueChanged", "error" };
            knob.InitScript = LoadScript("IntegrationLab.wwwroot.gauge-init.js");     // the GOOD script: the order is the bug
            SetKnobOptions(knob, "gaugeKnobWrongOrder", 50, label: "Wrong order");
            knob.WidgetEvent += new WidgetEventHandler(this.knob_WidgetEvent);

            _wrongOrderKnob = knob;
            _wrongOrderReportedError = false;
            this.labelWrongOrder.Text = "gaugeKnobWrongOrder · Packages: hide-jquery.js → vendor-knob.js → vendor-knob.css → jquery-lite.js";
            this.panelWrongOrder.Controls.Add(knob);

            AddTrace(TraceDirection.ServerToClient, "render gaugeKnobWrongOrder → init?", OptionsJson("gaugeKnobWrongOrder", 50, "Wrong order"));
            AddTrace(TraceDirection.Server, "packages (wrong order)", "vendor-knob.js BEFORE jquery-lite.js → console: ReferenceError: jQuery is not defined");
            SetStatus("loading wrong-order widget…", StatusKind.Warn);
            this.buttonWrongOrder.Enabled = false;
            this.timerWrongOrder.Start();      // check IsLoaded after 2 s
        }

        private void timerWrongOrder_Tick(object sender, EventArgs e)
        {
            this.timerWrongOrder.Stop();

            // Put jQuery back for everyone else (the prop parked it); the failed widget stays failed.
            this.Eval("if (window.__jqParked) { window.jQuery = window.__jqParked.jQuery; window.$ = window.__jqParked.$; delete window.__jqParked; }");
            AddTrace(TraceDirection.ServerToClient, "Eval", "restore window.jQuery / window.$ parked by hide-jquery.js");

            bool loaded = _wrongOrderKnob != null && _wrongOrderKnob.IsLoaded;
            AddTrace(TraceDirection.Server, "IsLoaded check (2 s)", $"gaugeKnobWrongOrder.IsLoaded = {loaded.ToString().ToLowerInvariant()}");

            if (!loaded)
            {
                ShowAlarm("✖ gaugeKnobWrongOrder: widget never initialized — check Packages order (console: jQuery is not defined)", AlarmKind.Error);
                SetStatus("fault: package order", StatusKind.Error);
            }
            else if (!_wrongOrderReportedError)
            {
                // Should not happen: init ran and the plugin applied. Say so instead of hiding it.
                ShowAlarm("⚠ gaugeKnobWrongOrder initialized anyway — the plugin found a jQuery global; check the Network panel for the load order", AlarmKind.Alarm);
                SetStatus("unexpected", StatusKind.Warn);
            }
            // else: init ran, caught the vendor failure and the "error" event already raised the banner.

            UpdateStateLabel();
        }

        #endregion

        #region Plain-JS proof and trace

        private void buttonProof_Click(object sender, EventArgs e)
        {
            // The proof page has no Wisej.NET in it: the same three files, loaded with plain tags.
            // It is served by the app's static file server from the project folder.
            AddTrace(TraceDirection.Server, "navigate", "wwwroot/proof/knob-proof.html (new tab)");
            Application.Navigate("wwwroot/proof/knob-proof.html", "_blank");
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.listTrace.Items.Clear();
        }

        #endregion

        #region Widget → .NET: every fireWidgetEvent from any of the three knobs lands here

        private void knob_WidgetEvent(object sender, WidgetEventArgs e)
        {
            var widget = (Widget)sender;
            string name = widget.Name;
            dynamic data = e.Data;

            switch (e.Type)
            {
                case "valueChanged":
                    {
                        // Success path on the way back: the user turned the knob, the context-safe
                        // callback fired the event from the captured widget reference.
                        double reported = ToDouble(data?.value);
                        AddTrace(TraceDirection.ClientToServer, $"{name}.valueChanged", $"{{\"value\":{F(reported)}}}");

                        if (widget == this.gaugeKnob && !double.IsNaN(reported))
                        {
                            _value = reported;
                            dynamic options = widget.Options;
                            options.value = reported;     // server state follows the input control; update() re-applies it silently
                            AddTrace(TraceDirection.ServerToClient, "update(options)", $"{{\"value\":{F(reported)}}}  (server state follows the knob)");
                            HideAlarm();
                            SetStatus($"{F(reported)} psi from the user · valueChanged reached .NET ✓", StatusKind.Normal, keepText: this.timerStream.Enabled);
                        }
                        break;
                    }

                case "contextError":
                    {
                        // Failure path 1: the broken InitScript called this.fireWidgetEvent inside the
                        // vendor callback. "this" was the <input>; the TypeError was caught and reported
                        // (deferred, from the captured reference) so the bug is visible here.
                        string message = ToStr(data?.message);
                        string thisWas = ToStr(data?.thisWas);
                        double value = ToDouble(data?.value);
                        AddTrace(TraceDirection.ClientToServer, $"{name}.contextError",
                            $"{{\"message\":\"{message}\",\"thisWas\":\"{thisWas}\",\"value\":{F(value)}}}");
                        AddTrace(TraceDirection.Server, "valueChanged NOT received", $"{name}: the event was lost in the vendor callback");
                        ShowAlarm($"✖ {name}: {message} — inside the vendor callback ‘this’ was {thisWas}, not the widget. Fix: var me = this; me.fireWidgetEvent(...)  (gauge-init.js)", AlarmKind.Error);
                        SetStatus("context lost", StatusKind.Error);
                        break;
                    }

                case "error":
                    {
                        // The adapter caught a vendor failure during init or update (wrong package order
                        // ends up here when the loader still calls init: $.fn.vendorKnob is missing).
                        string phase = ToStr(data?.phase);
                        string message = ToStr(data?.message);
                        AddTrace(TraceDirection.ClientToServer, $"{name}.error", $"{{\"phase\":\"{phase}\",\"message\":\"{message}\"}}");

                        if (widget == _wrongOrderKnob)
                        {
                            _wrongOrderReportedError = true;
                            ShowAlarm($"✖ {name}: {phase} failed — {message} → vendor-knob.js ran before jQuery, check Packages order (console: jQuery is not defined)", AlarmKind.Error);
                            SetStatus("fault: package order", StatusKind.Error);
                        }
                        else
                        {
                            ShowAlarm($"✖ {name}: vendor failure during {phase}: {message}", AlarmKind.Error);
                            SetStatus("fault", StatusKind.Error);
                        }
                        break;
                    }

                default:
                    AddTrace(TraceDirection.ClientToServer, $"{name}.{e.Type}", "(unhandled widget event)");
                    break;
            }

            UpdateStateLabel();
        }

        #endregion

        #region Helpers

        /// <summary>Reads an InitScript embedded in this assembly (see the csproj EmbeddedResource entries).</summary>
        private static string LoadScript(string resourceName)
        {
            using (var stream = typeof(Window1).Assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new FileNotFoundException("Embedded InitScript not found: " + resourceName);
                using (var reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// The Options object every knob receives in init(options). First-level fields only, so a
        /// later change to any of them makes Wisej.NET call update(options, old) on the client.
        /// "name" is debugging metadata: the InitScript registers the widget under it for DevTools.
        /// </summary>
        private static void SetKnobOptions(Widget widget, string name, double value, string label = "Pressure")
        {
            dynamic options = widget.Options;
            options.name = name;
            options.value = value;
            options.min = 0D;
            options.max = 100D;
            options.step = 1D;
            options.label = label;
            options.units = " psi";
            options.color = "#1a86ff";
        }

        private static string OptionsJson(string name, double value, string label = "Pressure")
            => $"{{\"name\":\"{name}\",\"value\":{F(value)},\"min\":0,\"max\":100,\"step\":1,\"label\":\"{label}\",\"units\":\" psi\",\"color\":\"#1a86ff\"}}";

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

        private void SetStatus(string text, StatusKind kind, bool keepText = false)
        {
            if (!keepText)
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
                : System.Drawing.Color.FromArgb(255, 244, 229);
            this.labelAlarm.ForeColor = kind == AlarmKind.Error
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
            string wrong = _wrongOrderKnob == null ? "not created" : (_wrongOrderKnob.IsLoaded ? "true" : "false");
            this.labelState.Text =
                $"SERVER STATE (authoritative)   Value={F(_value)} psi   range 0..100\n" +
                $"IsLoaded: gaugeKnob={Low(this.gaugeKnob.IsLoaded)}  gaugeKnobBroken={Low(this.gaugeKnobBroken.IsLoaded)}  gaugeKnobWrongOrder={wrong}\n" +
                $"DevTools: app.getWidget(\"gaugeKnob\").instance   ·   Sources: gauge-init.js (//# sourceURL)";
        }

        private static string Low(bool value) => value ? "true" : "false";

        private static string F(double value) => value.ToString(CultureInfo.InvariantCulture);

        private static string ToStr(object value) => value == null ? "" : Convert.ToString(value, CultureInfo.InvariantCulture);

        private static double ToDouble(object value)
        {
            try { return value == null ? double.NaN : Convert.ToDouble(value, CultureInfo.InvariantCulture); }
            catch { return double.NaN; }
        }

        #endregion
    }
}
