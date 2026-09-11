using System;
using System.ComponentModel;
using Wisej.Web;

namespace IntegrationLab.Controls
{
    /// <summary>
    /// Server-side Component (a Control, because it has a visual surface) that
    /// owns the gauge state and hosts the third-party VendorGauge inside a
    /// Wisej.NET widget container.
    ///
    /// Vocabulary, applied:
    ///   Component/Control  = this class (server, owns State)
    ///   Widget             = wwwroot/temperature-gauge.js (browser, owns the DOM + vendor instance)
    ///   Rendering          = Options → compact JSON → init(options) / update(options, old)
    ///   Event              = fireWidgetEvent(...) → OnWidgetEvent → ThresholdExceeded / RangeChanged
    /// </summary>
    [ToolboxItem(true)]
    [DefaultEvent("ThresholdExceeded")]
    [DefaultProperty("Value")]
    [Description("Hosts the VendorGauge JavaScript library inside a Wisej.NET widget container.")]
    public class TemperatureGauge : Widget
    {
        // ---- server-owned state -------------------------------------------
        private double _value = 72;
        private double _minimum = 40;
        private double _maximum = 120;
        private double _warnAt = 85;
        private double _threshold = 100;
        private string _label = "";
        private string _units = "°F";

        public TemperatureGauge()
        {
            // The vendor library is a package: loaded once per page, cached by name.
            this.Packages.Add(new Package
            {
                Name = "vendor-gauge",
                Source = "wwwroot/vendor-gauge.js"
            });

            // The client adapter is embedded in this assembly and handed to the wrapper.
            this.InitScript = GetResourceString("IntegrationLab.wwwroot.temperature-gauge.js");

            // The events this wrapper is allowed to raise (the documented contract).
            this.WiredEvents = new[] { "thresholdExceeded", "rangeChanged", "error" };

            this.Size = new System.Drawing.Size(360, 230);
            PushState();
        }

        #region Properties (server state)

        /// <summary>Current reading. Must be within Minimum..Maximum.</summary>
        [DefaultValue(72.0)]
        [Description("Current reading. Must be within Minimum..Maximum.")]
        public double Value
        {
            get => _value;
            set
            {
                if (double.IsNaN(value) || double.IsInfinity(value))
                    throw new ArgumentOutOfRangeException(nameof(Value), value, "Value must be a finite number.");
                if (value < _minimum || value > _maximum)
                    throw new ArgumentOutOfRangeException(nameof(Value), value,
                        $"Value must be between {_minimum} and {_maximum}.");

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

        [DefaultValue("")]
        public string Label
        {
            get => _label;
            set
            {
                _label = value ?? "";
                dynamic options = this.Options;
                options.label = _label;
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

        #endregion

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
                        ThresholdExceeded?.Invoke(this, new GaugeEventArgs(_value, "high", reported));
                        break;
                    }
                case "rangeChanged":
                    {
                        string range = (string)(data?.range ?? "normal");
                        double reported = ToDouble(data?.value);
                        this.CurrentRange = range;
                        RangeChanged?.Invoke(this, new GaugeEventArgs(_value, range, reported));
                        break;
                    }
                case "error":
                    {
                        string phase = (string)(data?.phase ?? "unknown");
                        string message = (string)(data?.message ?? "");
                        WidgetError?.Invoke(this, new GaugeErrorEventArgs(phase, message));
                        break;
                    }
                default:
                    base.OnWidgetEvent(e);
                    break;
            }
        }

        private void PushState()
        {
            dynamic options = this.Options;
            options.value = _value;
            options.min = _minimum;
            options.max = _maximum;
            options.warnAt = _warnAt;
            options.threshold = _threshold;
            options.label = _label;
            options.units = _units;
        }


        private static string F(double value)
            => value.ToString(System.Globalization.CultureInfo.InvariantCulture);

        private static double ToDouble(object value)
        {
            try { return value == null ? double.NaN : Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture); }
            catch { return double.NaN; }
        }
    }
}
