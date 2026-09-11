using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using Wisej.Web;

namespace IntegrationLab.Controls
{
    /// <summary>
    /// <b>SimpleGauge</b> — the reusable, designer-friendly gauge control of the IntegrationLab
    /// application framework.
    /// <para>
    /// <b>What it is for:</b> showing one live reading (a temperature, a pressure, a load) against a
    /// fixed range with a warning threshold, on any screen of the application, by dropping it from the
    /// Toolbox and setting typed properties. Screens never see JavaScript, package paths or option names.
    /// </para>
    /// <para>
    /// <b>Which vendor library it wraps:</b> <c>VendorGauge</c> 1.0 (<c>wwwroot/vendor-gauge.js</c>, global
    /// <c>VendorGauge</c>), a third-party SVG gauge that knows nothing about Wisej.NET. The adapter that
    /// bridges the two lives in the embedded resource <c>wwwroot/gauge-init.js</c> and is versioned with
    /// this class. When the vendor releases a new version, this file (and the adapter) change; the screens
    /// that use the gauge do not.
    /// </para>
    /// <para>
    /// This is the Module 1 decision record (ADR-001 "host the gauge inside a Widget container, server owns
    /// the state") turned into code: the class name, its namespace and this summary are what a developer two
    /// years from now needs to know first.
    /// </para>
    /// <para>
    /// <b>Public surface:</b> <see cref="Value"/>, <see cref="Minimum"/>, <see cref="Maximum"/>,
    /// <see cref="Caption"/>, <see cref="AnimationEnabled"/>, <see cref="Threshold"/> and the events
    /// <see cref="ValueChanged"/>, <see cref="ThresholdExceeded"/>, <see cref="WidgetError"/>.
    /// <b>Hidden:</b> <c>Packages</c>, <c>InitScript</c>, <c>WiredEvents</c>, the raw <c>Options</c> object and
    /// the low-level <c>Call</c>/<c>CallAsync</c> members (see docs/HiddenMembers.md). The escape hatch for
    /// derived classes is <see cref="OnConfigureOptions"/>.
    /// </para>
    /// </summary>
    [ToolboxItem(true)]
    [DefaultProperty("Value")]
    [DefaultEvent("ValueChanged")]
    [Description("Reusable gauge control wrapping the VendorGauge JavaScript library. Set the typed properties; packages, scripts and options are registered by the class.")]
    public class SimpleGauge : Widget
    {
        #region Vendor resources — constants, one place

        /// <summary>Package name under which the vendor library is registered (loaded once per page).</summary>
        public const string VendorPackageName = "vendor-gauge";

        /// <summary>Path of the vendor library relative to the application root (served as /wwwroot/vendor-gauge.js).</summary>
        public const string VendorPackageSource = "wwwroot/vendor-gauge.js";

        /// <summary>Version of the vendor library this wrapper was written against.</summary>
        public const string VendorVersion = "1.0";

        /// <summary>Logical name of the embedded client adapter (see IntegrationLab.csproj).</summary>
        private const string InitScriptResourceName = "IntegrationLab.wwwroot.gauge-init.js";

        #endregion

        #region Defaults — chosen to match the vendor where sensible

        public const double DefaultMinimum = 0;
        public const double DefaultMaximum = 100;
        public const double DefaultReading = 0;
        public const double DefaultThreshold = 85;
        public const string DefaultCaption = "";
        public const bool DefaultAnimationEnabled = true;

        #endregion

        // ---- server-owned state ---------------------------------------------
        private double _value = DefaultReading;
        private double _minimum = DefaultMinimum;
        private double _maximum = DefaultMaximum;
        private double _threshold = DefaultThreshold;
        private string _caption = DefaultCaption;
        private bool _animationEnabled = DefaultAnimationEnabled;

        /// <summary>
        /// Registers everything the prototype used to type on every page: the vendor package,
        /// the embedded InitScript, the wired events and the default Options.
        /// </summary>
        public SimpleGauge()
        {
            // 1. Resources — once, here, never on a page.
            base.Packages.Add(new Package
            {
                Name = VendorPackageName,
                Source = VendorPackageSource
            });

            // 2. The client adapter ships inside this assembly, versioned with this class.
            base.InitScript = GetResourceString(InitScriptResourceName);

            // 3. The events the adapter is allowed to raise (the client/server contract).
            base.WiredEvents = new[] { "valueChanged", "thresholdExceeded", "error" };

            this.Size = new System.Drawing.Size(360, 230);

            // 4. Default Options, so a fresh instance renders sensibly before any property is set.
            dynamic options = base.Options;
            options.value = _value;
            options.min = _minimum;
            options.max = _maximum;
            options.threshold = _threshold;
            options.warnAt = WarnBandStart();
            options.label = _caption;
            options.units = "";
            options.animationEnabled = _animationEnabled;

            // 5. Escape hatch for derived classes (units, colors, anything vendor-specific).
            OnConfigureOptions(options);
        }

        #region Typed properties (the public API)

        /// <summary>The current gauge reading. Setting it queues a client update — no raw Options editing.</summary>
        [Category("Gauge")]
        [Description("The current gauge reading. Setting it queues a client update — no raw Options editing.")]
        [DefaultValue(DefaultReading)]
        public double Value
        {
            get => _value;
            set
            {
                if (double.IsNaN(value) || double.IsInfinity(value))
                    throw new ArgumentOutOfRangeException(nameof(Value), value, "Value must be a finite number.");
                if (value < _minimum || value > _maximum)
                    throw new ArgumentOutOfRangeException(nameof(Value), value,
                        $"Value must be between Minimum ({F(_minimum)}) and Maximum ({F(_maximum)}).");

                if (_value == value)
                    return;          // nothing changed: nothing is rendered

                _value = value;
                dynamic options = base.Options;
                options.value = value;     // first-level field: Wisej.NET renders {"value":…} and calls update()
            }
        }

        /// <summary>Lower bound of the scale. Must be less than Maximum. Changing it re-lays out the scale on the client.</summary>
        [Category("Gauge")]
        [Description("Lower bound of the scale. Must be less than Maximum. Changing it re-lays out the scale on the client; a Value below the new Minimum is clamped.")]
        [DefaultValue(DefaultMinimum)]
        public double Minimum
        {
            get => _minimum;
            set
            {
                if (double.IsNaN(value) || double.IsInfinity(value))
                    throw new ArgumentOutOfRangeException(nameof(Minimum), value, "Minimum must be a finite number.");
                if (value >= _maximum)
                    throw new ArgumentOutOfRangeException(nameof(Minimum), value,
                        $"Minimum must be less than Maximum ({F(_maximum)}).");

                if (_minimum == value)
                    return;

                _minimum = value;
                dynamic options = base.Options;
                options.min = value;
                options.warnAt = WarnBandStart();
                ClampValueIntoRange();
            }
        }

        /// <summary>Upper bound of the scale. Must be greater than Minimum. Changing it re-lays out the scale on the client.</summary>
        [Category("Gauge")]
        [Description("Upper bound of the scale. Must be greater than Minimum. Changing it re-lays out the scale on the client; a Value above the new Maximum is clamped.")]
        [DefaultValue(DefaultMaximum)]
        public double Maximum
        {
            get => _maximum;
            set
            {
                if (double.IsNaN(value) || double.IsInfinity(value))
                    throw new ArgumentOutOfRangeException(nameof(Maximum), value, "Maximum must be a finite number.");
                if (value <= _minimum)
                    throw new ArgumentOutOfRangeException(nameof(Maximum), value,
                        $"Maximum must be greater than Minimum ({F(_minimum)}).");

                if (_maximum == value)
                    return;

                _maximum = value;
                dynamic options = base.Options;
                options.max = value;
                options.warnAt = WarnBandStart();
                ClampValueIntoRange();
            }
        }

        /// <summary>Text shown in the corner of the gauge (for example the name of the sensor).</summary>
        [Category("Gauge")]
        [Description("Text shown in the corner of the gauge (for example the name of the sensor).")]
        [DefaultValue(DefaultCaption)]
        public string Caption
        {
            get => _caption;
            set
            {
                value = value ?? "";
                if (_caption == value)
                    return;

                _caption = value;
                dynamic options = base.Options;
                options.label = value;
            }
        }

        /// <summary>When true the needle sweeps to a new Value; when false it jumps. Purely visual: it never changes the Value.</summary>
        [Category("Gauge")]
        [Description("When true the needle sweeps to a new Value; when false it jumps. Purely visual: it never changes the Value.")]
        [DefaultValue(DefaultAnimationEnabled)]
        public bool AnimationEnabled
        {
            get => _animationEnabled;
            set
            {
                if (_animationEnabled == value)
                    return;

                _animationEnabled = value;
                dynamic options = base.Options;
                options.animationEnabled = value;
            }
        }

        /// <summary>Crossing this reading upward raises ThresholdExceeded once (rising edge).</summary>
        [Category("Gauge")]
        [Description("Crossing this reading upward raises ThresholdExceeded once (rising edge). The warning band starts shortly before it.")]
        [DefaultValue(DefaultThreshold)]
        public double Threshold
        {
            get => _threshold;
            set
            {
                if (double.IsNaN(value) || double.IsInfinity(value))
                    throw new ArgumentOutOfRangeException(nameof(Threshold), value, "Threshold must be a finite number.");

                if (_threshold == value)
                    return;

                _threshold = value;
                dynamic options = base.Options;
                options.threshold = value;
                options.warnAt = WarnBandStart();
            }
        }

        #endregion

        #region Events (.NET events raised from widget events)

        /// <summary>
        /// Raised after the browser rendered a new <see cref="Value"/>: the client acknowledges the update
        /// (after the sweep when <see cref="AnimationEnabled"/> is true). Use it for "the operator can see it now" logic.
        /// </summary>
        [Category("Gauge")]
        [Description("Raised after the browser rendered a new Value (the client acknowledges the update).")]
        public event EventHandler<GaugeValueChangedEventArgs> ValueChanged;

        /// <summary>Raised once when the reading crosses <see cref="Threshold"/> upward.</summary>
        [Category("Gauge")]
        [Description("Raised once when the reading crosses Threshold upward.")]
        public event EventHandler<GaugeEventArgs> ThresholdExceeded;

        /// <summary>Raised when the client adapter caught a vendor failure instead of crashing the page.</summary>
        [Category("Gauge")]
        [Description("Raised when the client adapter caught a vendor failure instead of crashing the page.")]
        public event EventHandler<GaugeErrorEventArgs> WidgetError;

        protected virtual void OnValueChanged(GaugeValueChangedEventArgs e) => ValueChanged?.Invoke(this, e);
        protected virtual void OnThresholdExceeded(GaugeEventArgs e) => ThresholdExceeded?.Invoke(this, e);
        protected virtual void OnWidgetError(GaugeErrorEventArgs e) => WidgetError?.Invoke(this, e);

        /// <summary>
        /// Every event fired by the client adapter lands here. The server decides what to raise;
        /// it never trusts the browser to hold the authoritative value.
        /// </summary>
        protected override void OnWidgetEvent(WidgetEventArgs e)
        {
            dynamic data = e.Data;

            switch (e.Type)
            {
                case "valueChanged":
                    OnValueChanged(new GaugeValueChangedEventArgs(_value, ToDouble(data?.value), ToDouble(data?.previous)));
                    break;

                case "thresholdExceeded":
                    OnThresholdExceeded(new GaugeEventArgs(_value, ToDouble(data?.value)));
                    break;

                case "error":
                    OnWidgetError(new GaugeErrorEventArgs((string)(data?.phase ?? "unknown"), (string)(data?.message ?? "")));
                    break;

                default:
                    base.OnWidgetEvent(e);     // never swallow unknown events
                    break;
            }
        }

        #endregion

        #region Hidden escape hatches (see docs/HiddenMembers.md)

        // The members below are what the prototype edited on every page. Once the class owns the
        // setup they become dangerous: editing Packages on one instance breaks that instance only.
        // They are shadowed read-only, hidden from the Properties window, from IntelliSense and
        // from designer serialization. The class itself always goes through "base.".

        /// <summary>Hidden. Vendor packages are registered by the class; see <see cref="VendorPackageSource"/>.</summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new List<Package> Packages => base.Packages;

        /// <summary>Hidden. The client adapter is an embedded resource versioned with this class.</summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new string InitScript => base.InitScript;

        /// <summary>Hidden. Use the typed properties; derived classes use <see cref="OnConfigureOptions"/>.</summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new object Options => base.Options;

        /// <summary>Hidden. The event contract is fixed by the class and its adapter.</summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new string[] WiredEvents => base.WiredEvents;

        /// <summary>Hidden from IntelliSense. Low-level client calls belong inside the wrapper.</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Control Call(string function, params object[] args) => base.Call(function, args);

        /// <summary>Hidden from IntelliSense. Low-level client calls belong inside the wrapper.</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Control Call(string function, Action<object> callback, params object[] args) => base.Call(function, callback, args);

        /// <summary>Hidden from IntelliSense. Low-level client calls belong inside the wrapper.</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Task<dynamic> CallAsync(string function, params object[] args) => base.CallAsync(function, args);

        /// <summary>
        /// The one sanctioned escape hatch. Called once from the constructor after the default options
        /// have been written; a derived class can add or override vendor options here (for example
        /// <c>options.units = "°F"</c>) without ever touching <c>Options</c> from application code.
        /// </summary>
        /// <param name="options">The dynamic Options object about to be rendered with the first request.</param>
        protected virtual void OnConfigureOptions(dynamic options)
        {
        }

        #endregion

        #region Internals

        /// <summary>
        /// Vendor detail the application never sees: the vendor draws a "warm" band that starts at
        /// warnAt. The wrapper places it 15% of the span before Threshold.
        /// </summary>
        private double WarnBandStart() => _threshold - 0.15 * (_maximum - _minimum);

        /// <summary>A shrinking range must not leave Value outside it: clamp it.</summary>
        private void ClampValueIntoRange()
        {
            double clamped = Math.Max(_minimum, Math.Min(_maximum, _value));
            if (clamped == _value)
                return;

            _value = clamped;
            dynamic options = base.Options;
            options.value = clamped;
        }

        private static string F(double value)
            => value.ToString(CultureInfo.InvariantCulture);

        private static double ToDouble(object value)
        {
            try { return value == null ? double.NaN : Convert.ToDouble(value, CultureInfo.InvariantCulture); }
            catch { return double.NaN; }
        }

        #endregion
    }
}
