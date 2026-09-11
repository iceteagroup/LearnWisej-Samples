using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using IntegrationLab.Contracts;
using Wisej.Core;
using Wisej.Web;

namespace IntegrationLab.Controls
{
    /// <summary>
    /// Reusable gauge: hosts the third-party VendorGauge inside a Wisej.NET widget container and
    /// exposes a small command API on top of the typed state.
    /// <para>
    /// Two ways the server talks to the browser, and when to use each:
    /// </para>
    /// <list type="bullet">
    ///   <item><b>State</b> — typed properties (<see cref="Value"/>, <see cref="Threshold"/>…) write into
    ///   <c>Options</c>; Wisej.NET renders the changed first-level fields as compact JSON and the
    ///   client adapter applies them in <c>update(options, old)</c>. State is durable: it is
    ///   re-rendered on every <c>init</c> (page refresh, late-joining client).</item>
    ///   <item><b>Commands</b> — <see cref="SetValue"/>, <see cref="ResetAnimation"/> use <c>Call</c>
    ///   (one-way, queued, no answer). <see cref="GetRenderedSizeAsync"/> and <see cref="GetSelectedStateAsync"/>
    ///   use <c>CallAsync</c>: the handler awaits the browser because the next statement needs the value.</item>
    /// </list>
    /// <para>
    /// Every command targets a function defined on the client <b>wrapper</b> (gauge-init.js), never a
    /// path such as <c>App.MainPage.panelGauge.gauge</c>. The wrapper is <c>this</c> on the client and
    /// owns its own container; nothing here depends on where the gauge sits in the page.
    /// </para>
    /// </summary>
    [ToolboxItem(true)]
    [DefaultEvent("ThresholdExceeded")]
    [DefaultProperty("Value")]
    [Description("Hosts the VendorGauge JavaScript library inside a Wisej.NET widget container and exposes server-to-client commands.")]
    public class SimpleGauge : Widget
    {
        // ---- client function names (keep in sync with wwwroot/gauge-init.js) --------------
        // Call("setValue") targets the WRAPPER function this.setValue in gauge-init.js.
        // Instance.setValue(v) would target this.widget.setValue(v) — the VENDOR method — and skip the
        // wrapper's sweep and bookkeeping. Both are legal; the wrapper functions are the contract.
        public const string JsSetValue = "setValue";
        public const string JsResetAnimation = "resetAnimation";
        public const string JsGetRenderedSize = "getRenderedSize";
        public const string JsGetSelectedState = "getSelectedState";

        // ---- server-owned state -------------------------------------------
        private double _value = 72;
        private double _minimum = 40;
        private double _maximum = 120;
        private double _warnAt = 85;
        private double _threshold = 100;
        private string _caption = "";
        private string _units = "°F";

        public SimpleGauge()
        {
            // The vendor library is a package: loaded once per page, cached by name.
            this.Packages.Add(new Package
            {
                Name = "vendor-gauge",
                Source = "wwwroot/vendor-gauge.js"
            });

            // The client adapter is embedded in this assembly and handed to the wrapper.
            this.InitScript = GetResourceString("IntegrationLab.wwwroot.gauge-init.js");

            // The events this wrapper is allowed to raise (the documented contract).
            this.WiredEvents = new[] { "thresholdExceeded", "rangeChanged", "error" };

            this.Size = new System.Drawing.Size(480, 244);
            PushState();
        }

        #region Properties (server state → Options → update(options))

        /// <summary>Current reading. Must be within Minimum..Maximum. Setting it renders {"value":…}.</summary>
        [DefaultValue(72.0)]
        [Description("Current reading. Must be within Minimum..Maximum.")]
        public double Value
        {
            get => _value;
            set
            {
                ValidateValue(value);
                if (_value == value)
                    return;

                _value = value;
                dynamic options = this.Options;
                options.value = value;     // first-level field: Wisej.NET renders {"value":…} automatically
            }
        }

        [DefaultValue(40.0)]
        public double Minimum
        {
            get => _minimum;
            set
            {
                if (value >= _maximum)
                    throw new ArgumentOutOfRangeException(nameof(Minimum), value, "Minimum must be less than Maximum.");
                _minimum = value;
                dynamic options = this.Options;
                options.min = value;
            }
        }

        [DefaultValue(120.0)]
        public double Maximum
        {
            get => _maximum;
            set
            {
                if (value <= _minimum)
                    throw new ArgumentOutOfRangeException(nameof(Maximum), value, "Maximum must be greater than Minimum.");
                _maximum = value;
                dynamic options = this.Options;
                options.max = value;
            }
        }

        /// <summary>Start of the "warm" band.</summary>
        [DefaultValue(85.0)]
        public double WarnAt
        {
            get => _warnAt;
            set
            {
                _warnAt = value;
                dynamic options = this.Options;
                options.warnAt = value;
            }
        }

        /// <summary>Crossing this value (rising edge) raises <see cref="ThresholdExceeded"/>.</summary>
        [DefaultValue(100.0)]
        public double Threshold
        {
            get => _threshold;
            set
            {
                _threshold = value;
                dynamic options = this.Options;
                options.threshold = value;
            }
        }

        /// <summary>Small caption drawn by the vendor (sent as the vendor's "label" option).</summary>
        [DefaultValue("")]
        public string Caption
        {
            get => _caption;
            set
            {
                _caption = value ?? "";
                dynamic options = this.Options;
                options.label = _caption;
            }
        }

        [DefaultValue("°F")]
        public string Units
        {
            get => _units;
            set
            {
                _units = value ?? "";
                dynamic options = this.Options;
                options.units = _units;
            }
        }

        /// <summary>Last range name reported by the widget ("normal", "warm", "high").</summary>
        [Browsable(false)]
        public string CurrentRange { get; private set; } = "normal";

        /// <summary>
        /// How long an awaited call may wait for the browser. A closed tab never replies;
        /// without a bound the handler would stay suspended.
        /// </summary>
        [Browsable(false)]
        public TimeSpan ClientReplyTimeout { get; set; } = TimeSpan.FromSeconds(5);

        #endregion

        #region Events (.NET events raised from widget events)

        /// <summary>Raised once when the reading crosses <see cref="Threshold"/> upward.</summary>
        [Description("Raised once when the reading crosses Threshold upward.")]
        public event EventHandler<GaugeEventArgs> ThresholdExceeded;

        /// <summary>Raised when the reading moves into a different band (normal / warm / high).</summary>
        [Description("Raised when the reading moves into a different band.")]
        public event EventHandler<GaugeEventArgs> RangeChanged;

        /// <summary>Raised when the client adapter caught a vendor failure.</summary>
        [Description("Raised when the client adapter caught a vendor failure.")]
        public event EventHandler<GaugeErrorEventArgs> WidgetError;

        /// <summary>Raised for every command, result and event that crosses the wire.</summary>
        [Browsable(false)]
        public event EventHandler<TraceEventArgs> Trace;

        #endregion

        #region Commands: server → client, one-way (Call)

        /// <summary>
        /// Sets the reading and tells the live widget to sweep its needle to it.
        /// <para>
        /// <c>Value</c> writes the durable state into <c>Options</c>: it survives a page refresh and is
        /// what <c>init(options)</c> renders for a late-joining client. <c>Call("setValue", v)</c> is a
        /// transient command to the widget that exists right now: queued, flushed with this response,
        /// executed once and never replayed. The client wrapper dedupes the two by target value.
        /// </para>
        /// <para>Server does not wait: the return is immediate, no result comes back.</para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The value is outside Minimum..Maximum; nothing is sent.</exception>
        public void SetValue(double value)
        {
            ValidateValue(value);                    // validate on the server BEFORE anything crosses the wire

            if (_value != value)
            {
                _value = value;
                dynamic options = this.Options;
                options.value = value;               // state: {"value":72} rendered with this response
            }

            this.Call(JsSetValue, value);            // command: queued, runs this.setValue(72) on the wrapper
            RaiseTrace(TraceDirection.ServerToClient, $"Call(\"{JsSetValue}\", {F(value)})", "");
        }

        /// <summary>
        /// Cancels the needle sweep and restarts the client-side settle animation.
        /// Nothing about the animation is server state, so this is a pure command: <c>Call</c>, no result.
        /// </summary>
        public void ResetAnimation()
        {
            this.Call(JsResetAnimation);
            RaiseTrace(TraceDirection.ServerToClient, $"Call(\"{JsResetAnimation}\")", "");
        }

        #endregion

        #region Commands: server → client with a result (CallAsync)

        /// <summary>
        /// Asks the browser how big the gauge really is. The handler that calls this needs the
        /// answer for its next statement (a layout decision), so awaiting is justified.
        /// <para>
        /// The JavaScript function returns <c>{ width, height }</c>; the result arrives as a dynamic
        /// object and is read by its JavaScript names, validated, then mapped into <see cref="RenderedSize"/>.
        /// </para>
        /// </summary>
        /// <exception cref="TimeoutException">The browser did not reply within <see cref="ClientReplyTimeout"/>.</exception>
        /// <exception cref="InvalidOperationException">The browser replied with nothing usable.</exception>
        public async Task<RenderedSize> GetRenderedSizeAsync()
        {
            RaiseTrace(TraceDirection.ServerToClient, $"await CallAsync(\"{JsGetRenderedSize}\")", "");

            dynamic result = await AwaitClient(this.CallAsync(JsGetRenderedSize), JsGetRenderedSize);

            RaiseTrace(TraceDirection.ClientToServer, "result", ToWireJson((object)result));

            if (result == null)
                throw new InvalidOperationException($"{JsGetRenderedSize} returned null.");

            // Read by JavaScript names. result.Width would be null: the wire is camelCase.
            var size = new RenderedSize
            {
                Width = ToInt(result.width),
                Height = ToInt(result.height)
            };

            // Validate before trusting: a hidden or not-yet-laid-out element reports 0×0.
            if (size.Width <= 0 || size.Height <= 0)
                throw new InvalidOperationException($"{JsGetRenderedSize} reported {size}: the gauge is not laid out.");

            return size;
        }

        /// <summary>
        /// Returns the selected client-side state as a small DTO. The browser answers with
        /// <c>{ value, isAboveThreshold, width, height, isAnimating }</c>; the server maps it by the
        /// JavaScript names and validates the value against its own range before trusting it.
        /// </summary>
        public async Task<GaugeStateDto> GetSelectedStateAsync()
        {
            RaiseTrace(TraceDirection.ServerToClient, $"await CallAsync(\"{JsGetSelectedState}\")", "");

            dynamic result = await AwaitClient(this.CallAsync(JsGetSelectedState), JsGetSelectedState);

            RaiseTrace(TraceDirection.ClientToServer, "result", ToWireJson((object)result));

            if (result == null)
                throw new InvalidOperationException($"{JsGetSelectedState} returned null.");

            var state = new GaugeStateDto
            {
                Value = ToDouble(result.value),
                IsAboveThreshold = ToBool(result.isAboveThreshold),
                Width = ToInt(result.width),
                Height = ToInt(result.height),
                IsAnimating = ToBool(result.isAnimating)
            };

            // Validate the awaited value before it is trusted.
            if (double.IsNaN(state.Value) || state.Value < _minimum || state.Value > _maximum)
                throw new InvalidOperationException(
                    $"client reported value {F(state.Value)} outside {F(_minimum)}..{F(_maximum)}; state rejected.");

            return state;
        }

        #endregion

        /// <summary>Serializes a value the way Wisej.NET puts it on the wire: camelCase property names.</summary>
        public static string ToWireJson(object value)
        {
            if (value == null)
                return "null";
            try { return WisejSerializer.Serialize(value, WisejSerializerOptions.CamelCase); }
            catch (Exception ex) { return $"<unserializable: {ex.Message}>"; }
        }

        /// <summary>
        /// Every event fired by the client wrapper lands here. The server decides what
        /// to raise; it never trusts the browser to hold the authoritative value.
        /// </summary>
        protected override void OnWidgetEvent(WidgetEventArgs e)
        {
            dynamic data = e.Data;

            switch (e.Type)
            {
                case "thresholdExceeded":
                    {
                        double reported = ToDouble(data?.value);
                        RaiseTrace(TraceDirection.ClientToServer, "thresholdExceeded", $"{{\"value\":{F(reported)}}}");
                        ThresholdExceeded?.Invoke(this, new GaugeEventArgs(_value, "high"));
                        break;
                    }
                case "rangeChanged":
                    {
                        string range = (string)(data?.range ?? "normal");
                        double reported = ToDouble(data?.value);
                        this.CurrentRange = range;
                        RaiseTrace(TraceDirection.ClientToServer, "rangeChanged", $"{{\"range\":\"{range}\",\"value\":{F(reported)}}}");
                        RangeChanged?.Invoke(this, new GaugeEventArgs(_value, range));
                        break;
                    }
                case "error":
                    {
                        string phase = (string)(data?.phase ?? "unknown");
                        string message = (string)(data?.message ?? "");
                        RaiseTrace(TraceDirection.ClientToServer, "error", $"{{\"phase\":\"{phase}\",\"message\":\"{message}\"}}");
                        WidgetError?.Invoke(this, new GaugeErrorEventArgs(phase, message));
                        break;
                    }
                default:
                    base.OnWidgetEvent(e);
                    break;
            }
        }

        #region internals

        /// <summary>
        /// Bounds an awaited client call. If the tab is gone the browser never replies and the
        /// task never completes; a bound turns "stuck forever" into a reported failure.
        /// </summary>
        private async Task<object> AwaitClient(Task<object> call, string what)
        {
            var completed = await Task.WhenAny(call, Task.Delay(this.ClientReplyTimeout));
            if (completed != call)
                throw new TimeoutException($"The browser did not answer {what} within {ClientReplyTimeout.TotalSeconds:0.#} s.");
            return await call;      // re-await to surface any exception from the call itself
        }

        private void ValidateValue(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentOutOfRangeException(nameof(Value), value, "Value must be a finite number.");
            if (value < _minimum || value > _maximum)
                throw new ArgumentOutOfRangeException(nameof(Value), value,
                    $"Value must be between {F(_minimum)} and {F(_maximum)}.");
        }

        private void PushState()
        {
            dynamic options = this.Options;
            options.value = _value;
            options.min = _minimum;
            options.max = _maximum;
            options.warnAt = _warnAt;
            options.threshold = _threshold;
            options.label = _caption;
            options.units = _units;
        }

        private void RaiseTrace(TraceDirection direction, string name, string payload)
            => Trace?.Invoke(this, new TraceEventArgs(direction, name, payload));

        private static string F(double value)
            => value.ToString(CultureInfo.InvariantCulture);

        private static double ToDouble(object value)
        {
            try { return value == null ? double.NaN : Convert.ToDouble(value, CultureInfo.InvariantCulture); }
            catch { return double.NaN; }
        }

        private static int ToInt(object value)
        {
            double d = ToDouble(value);
            return double.IsNaN(d) ? 0 : (int)Math.Round(d);
        }

        private static bool ToBool(object value)
        {
            try { return value != null && Convert.ToBoolean(value, CultureInfo.InvariantCulture); }
            catch { return false; }
        }

        #endregion
    }
}
