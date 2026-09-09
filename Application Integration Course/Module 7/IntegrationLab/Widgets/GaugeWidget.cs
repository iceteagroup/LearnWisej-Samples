using System;
using System.ComponentModel;
using IntegrationLab.Contracts;
using Wisej.Web;

namespace IntegrationLab.Widgets
{
    /// <summary>
    /// Server-side wrapper around VendorGauge (client adapter: wwwroot/gauge-init.js).
    ///
    /// Server event handler #1 — OnWidgetEvent → typed .NET event.
    /// Every fireWidgetEvent from the wrapper lands in <see cref="OnWidgetEvent"/> (the
    /// Widget's single catch-all). This class handles the names it owns, validates every
    /// field of the payload, and re-raises a typed event, so application code never sees
    /// the string names or the dynamic data.
    /// </summary>
    [ToolboxItem(true)]
    [DefaultEvent("ThresholdCrossed")]
    [DefaultProperty("Value")]
    [Description("Hosts the VendorGauge library and raises ThresholdCrossed once per crossing.")]
    public class GaugeWidget : Widget
    {
        private double _value = 72;
        private double _minimum = 40;
        private double _maximum = 120;
        private double _warnAt = 85;
        private double _threshold = 100;
        private string _label = "";
        private string _units = "°F";

        public GaugeWidget()
        {
            this.Packages.Add(new Package { Name = "vendor-gauge", Source = "wwwroot/vendor-gauge.js" });
            this.InitScript = GetResourceString("IntegrationLab.wwwroot.gauge-init.js");
            this.WiredEvents = new[] { "thresholdCrossed", "error" };      // the contract: nothing else may arrive
            this.Size = new System.Drawing.Size(300, 200);
            PushState();
        }

        #region Server-owned state

        [DefaultValue(72.0)]
        public double Value
        {
            get => _value;
            set
            {
                if (double.IsNaN(value) || double.IsInfinity(value))
                    throw new ArgumentOutOfRangeException(nameof(Value), value, "Value must be a finite number.");
                if (value < _minimum || value > _maximum)
                    throw new ArgumentOutOfRangeException(nameof(Value), value, $"Value must be between {_minimum} and {_maximum}.");
                if (_value == value) return;
                _value = value;
                dynamic options = this.Options;
                options.value = value;                  // first-level field: Wisej renders {"value":…} → update(options)
                RaiseTrace(TraceDirection.ServerToClient, "update(options)", $"{{\"value\":{PayloadReader.F(value)}}}");
            }
        }

        [DefaultValue(40.0)]
        public double Minimum { get => _minimum; set { if (value >= _maximum) throw new ArgumentOutOfRangeException(nameof(Minimum)); _minimum = value; ((dynamic)this.Options).min = value; } }

        [DefaultValue(120.0)]
        public double Maximum { get => _maximum; set { if (value <= _minimum) throw new ArgumentOutOfRangeException(nameof(Maximum)); _maximum = value; ((dynamic)this.Options).max = value; } }

        /// <summary>Crossing this line upward raises ThresholdCrossed with Level = "warn".</summary>
        [DefaultValue(85.0)]
        public double WarnAt { get => _warnAt; set { _warnAt = value; ((dynamic)this.Options).warnAt = value; } }

        /// <summary>Crossing this line upward raises ThresholdCrossed with Level = "high".</summary>
        [DefaultValue(100.0)]
        public double Threshold { get => _threshold; set { _threshold = value; ((dynamic)this.Options).threshold = value; } }

        [DefaultValue("")]
        public string Label { get => _label; set { _label = value ?? ""; ((dynamic)this.Options).label = _label; } }

        [DefaultValue("°F")]
        public string Units { get => _units; set { _units = value ?? ""; ((dynamic)this.Options).units = _units; } }

        /// <summary>Last level reported by a validated thresholdCrossed payload ("warn" / "high"), or "" .</summary>
        [Browsable(false)]
        public string LastLevel { get; private set; } = "";

        #endregion

        #region .NET events

        /// <summary>Raised once when the reading crosses WarnAt or Threshold upward (a business decision).</summary>
        [Description("Raised once when the reading crosses WarnAt or Threshold upward.")]
        public event EventHandler<GaugeThresholdEventArgs> ThresholdCrossed;

        /// <summary>Every message in either direction, for the lab log.</summary>
        [Browsable(false)]
        public event EventHandler<TraceEventArgs> Trace;

        #endregion

        /// <summary>
        /// Server event handler #1. The Widget catch-all: switch on the contract name, validate
        /// every field, raise the typed event with a DTO, and pass everything else to base.
        /// </summary>
        protected override void OnWidgetEvent(WidgetEventArgs e)
        {
            switch (e.Type)
            {
                case "thresholdCrossed":
                    HandleThresholdCrossed(e.Data);
                    break;

                case "error":
                    {
                        dynamic data = e.Data;
                        RaiseTrace(TraceDirection.ClientToServer, "error", PayloadReader.ToJson(e.Data));
                        RaiseTrace(TraceDirection.Rejected, "vendor failure",
                            $"{PayloadReader.Get(() => data.phase)}: {PayloadReader.Get(() => data.message)}");
                        break;
                    }

                default:
                    base.OnWidgetEvent(e);              // anything not ours still reaches WidgetEvent subscribers
                    break;
            }
        }

        private void HandleThresholdCrossed(object payload)
        {
            dynamic data = payload;
            RaiseTrace(TraceDirection.ClientToServer, "thresholdCrossed", "e.Data = " + PayloadReader.ToJson(payload));

            // --- validate every field before it is used ------------------------------------
            if (!PayloadReader.TryDouble(PayloadReader.Get(() => data.value), out double value))
            { Reject("thresholdCrossed", "value is missing or not a finite number"); return; }

            if (value < _minimum || value > _maximum)
            { Reject("thresholdCrossed", $"value {PayloadReader.F(value)} outside {_minimum}..{_maximum}"); return; }

            if (!PayloadReader.TryString(PayloadReader.Get(() => data.level), out string level) || (level != "warn" && level != "high"))
            { Reject("thresholdCrossed", "level must be \"warn\" or \"high\""); return; }

            // The level must agree with the server's own lines — the client is not the authority.
            double line = level == "high" ? _threshold : _warnAt;
            if (value < line)
            { Reject("thresholdCrossed", $"level \"{level}\" claimed but value {PayloadReader.F(value)} is below {PayloadReader.F(line)}"); return; }

            if (Math.Abs(value - _value) > 0.001)
                RaiseTrace(TraceDirection.Server, "contract check", $"client reported {PayloadReader.F(value)}, server Value is {PayloadReader.F(_value)}: server wins");

            this.LastLevel = level;
            var args = new GaugeThresholdEventArgs(_value, level);
            RaiseTrace(TraceDirection.Server, "ThresholdCrossed", $"raised via OnWidgetEvent → GaugeThresholdEventArgs {args}");
            ThresholdCrossed?.Invoke(this, args);
        }

        private void Reject(string eventName, string reason)
            => RaiseTrace(TraceDirection.Rejected, eventName, "rejected: " + reason);

        public string ToJson()
            => $"{{\"value\":{PayloadReader.F(_value)},\"min\":{PayloadReader.F(_minimum)},\"max\":{PayloadReader.F(_maximum)},\"warnAt\":{PayloadReader.F(_warnAt)},\"threshold\":{PayloadReader.F(_threshold)},\"label\":\"{_label}\",\"units\":\"{_units}\"}}";

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

        private void RaiseTrace(TraceDirection direction, string name, string payload)
            => Trace?.Invoke(this, new TraceEventArgs(direction, name, payload));
    }
}
