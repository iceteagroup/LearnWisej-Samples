using System;
using System.ComponentModel;
using System.Globalization;
using Wisej.Base;
using Wisej.Core;
using Wisej.Web;

namespace IntegrationLab.Controls
{
    /// <summary>
    /// Server half of the custom gauge control (Module 5 pattern).
    ///
    /// The pair:
    ///   server  IntegrationLab.Controls.SimpleGaugeControl   (this file)   owns the state, validates, raises .NET events
    ///   client  integrationlab.controls.SimpleGaugeControl   (Platform/SimpleGaugeControl.js)   qx class, owns the vendor object
    ///
    /// Wire protocol, in full:
    ///   OnWebRender  → config.className / appearance / value / minimum / maximum / threshold / caption / units + wiredEvents
    ///   OnWebEvent   ← "thresholdExceeded" { value, threshold }
    /// Nothing else crosses the wire. Everything the client class declares as a property is written
    /// here (camel-cased), and nothing that exists only for the Designer or for business logic is.
    /// </summary>
    [ToolboxItem(true)]
    [DefaultEvent("ThresholdExceeded")]
    [DefaultProperty("Value")]
    [Description("Gauge control rendered by the integrationlab.controls.SimpleGaugeControl client class and styled by the \"simplegauge\" appearance key.")]
    public class SimpleGaugeControl : Control
    {
        /// <summary>The qx class (Platform/SimpleGaugeControl.js) that renders this control in the browser.</summary>
        public const string ClientClassName = "integrationlab.controls.SimpleGaugeControl";

        /// <summary>The theme appearance key (Themes/simplegauge.mixin.theme).</summary>
        public const string DefaultAppearanceKey = "simplegauge";

        /// <summary>Custom theme state added while the reading is at or above <see cref="Threshold"/>.</summary>
        public const string AlarmState = "alarm";

        // ---- server-owned state (the only truth) -----------------------------
        private double _value = 0;
        private double _minimum = 0;
        private double _maximum = 100;
        private double _threshold = 90;
        private string _caption = "";
        private string _units = "";

        public SimpleGaugeControl()
        {
            // The appearance key joins the control to the theme system: the theme (or the
            // simplegauge.mixin.theme in /Themes) supplies background, border, radius, padding
            // and textColor for the "simplegauge" key, on every theme.
            this.AppearanceKey = DefaultAppearanceKey;
            this.Size = new System.Drawing.Size(320, 220);
        }

        #region Properties (typed, validated on the server)

        /// <summary>Current reading. Must be a finite number within Minimum..Maximum.</summary>
        [DefaultValue(0.0)]
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
                        $"Value must be between {F(_minimum)} and {F(_maximum)}.");

                if (_value == value)
                    return;

                _value = value;
                SyncAlarmState();
                Update();   // schedules OnWebRender; Wisej.NET sends only the fields that changed
            }
        }

        /// <summary>Lower end of the scale. Must be less than Maximum. A Value outside the new range is clamped.</summary>
        [DefaultValue(0.0)]
        [Description("Lower end of the scale. Must be less than Maximum.")]
        public double Minimum
        {
            get => _minimum;
            set
            {
                if (double.IsNaN(value) || double.IsInfinity(value))
                    throw new ArgumentOutOfRangeException(nameof(Minimum), value, "Minimum must be a finite number.");
                if (value >= _maximum)
                    throw new ArgumentOutOfRangeException(nameof(Minimum), value, "Minimum must be less than Maximum.");

                if (_minimum == value)
                    return;

                _minimum = value;
                ClampValueIntoRange();
                Update();
            }
        }

        /// <summary>Upper end of the scale. Must be greater than Minimum. A Value outside the new range is clamped.</summary>
        [DefaultValue(100.0)]
        [Description("Upper end of the scale. Must be greater than Minimum.")]
        public double Maximum
        {
            get => _maximum;
            set
            {
                if (double.IsNaN(value) || double.IsInfinity(value))
                    throw new ArgumentOutOfRangeException(nameof(Maximum), value, "Maximum must be a finite number.");
                if (value <= _minimum)
                    throw new ArgumentOutOfRangeException(nameof(Maximum), value, "Maximum must be greater than Minimum.");

                if (_maximum == value)
                    return;

                _maximum = value;
                ClampValueIntoRange();
                Update();
            }
        }

        /// <summary>Crossing this value upward (rising edge) raises <see cref="ThresholdExceeded"/>.</summary>
        [DefaultValue(90.0)]
        [Description("Crossing this value upward raises ThresholdExceeded.")]
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
                SyncAlarmState();
                Update();
            }
        }

        /// <summary>Short label drawn by the gauge (e.g. "Boiler 1 — pressure").</summary>
        [DefaultValue("")]
        [Description("Short label drawn inside the gauge.")]
        public string Caption
        {
            get => _caption;
            set
            {
                value = value ?? "";
                if (_caption == value)
                    return;

                _caption = value;
                Update();
            }
        }

        /// <summary>Unit suffix appended to the readout (e.g. "psi", "%", "°F").</summary>
        [DefaultValue("")]
        [Description("Unit suffix appended to the readout.")]
        public string Units
        {
            get => _units;
            set
            {
                value = value ?? "";
                if (_units == value)
                    return;

                _units = value;
                Update();
            }
        }

        /// <summary>True while Value is at or above Threshold.</summary>
        [Browsable(false)]
        public bool IsAlarm => _value >= _threshold;

        #endregion

        #region Events

        /// <summary>Raised once each time the reading crosses <see cref="Threshold"/> upward (reported by the client class).</summary>
        [Description("Raised once each time the reading crosses Threshold upward.")]
        public event EventHandler<GaugeThresholdEventArgs> ThresholdExceeded;

        #endregion

        #region Rendering: OnWebRender writes intentional state only

        /// <summary>
        /// The contract between the two halves of the control. Only the properties the client
        /// class declares are written onto config, camel-cased; Wisej.NET diffs the result
        /// against the previous render and sends the changed fields.
        /// </summary>
        protected override void OnWebRender(dynamic config)
        {
            base.OnWebRender((object)config);

            bool designMode = IsDesignMode();

            config.className = ClientClassName;
            config.appearance = this.AppearanceKey;          // "simplegauge" → theme styles + textColor

            // Design time: the Designer instantiates the control with default property values.
            // Render a sample reading so the design surface shows a gauge, not a grey box.
            config.value = designMode ? DesignTimeSampleValue() : _value;
            config.minimum = _minimum;
            config.maximum = _maximum;
            config.threshold = _threshold;
            config.caption = designMode && _caption.Length == 0 ? "SimpleGauge (design)" : _caption;
            config.units = _units;

            // Events the client class may raise; "(Data)" carries e.getData() as e.Parameters.Data.
            AddWiredEvent(config, "thresholdExceeded(Data)");
        }

        /// <summary>
        /// Every wired event fired by the client class lands here. The server decides what to
        /// raise; it never trusts the browser to hold the authoritative value.
        /// </summary>
        protected override void OnWebEvent(WisejEventArgs e)
        {
            switch (e.Type)
            {
                case "thresholdExceeded":
                    {
                        // the server raises the event with its own authoritative value
                        ThresholdExceeded?.Invoke(this, new GaugeThresholdEventArgs(_value, _threshold));
                        break;
                    }

                default:
                    base.OnWebEvent(e);   // never swallow the events the base control handles (resize, focus, pointer…)
                    break;
            }
        }

        #endregion

        #region Design-time support

        /// <summary>
        /// True when the control is being rendered by the Wisej Designer. Both checks are used:
        /// the .NET component site (Site.DesignMode) and the Wisej.NET component flag.
        /// </summary>
        private bool IsDesignMode()
        {
            if (this.DesignMode)
                return true;

            try { return ((IWisejComponent)this).DesignMode; }
            catch (Exception) { return false; }
        }

        /// <summary>A reading at 62% of the scale: enough to show the needle, the arc and the readout.</summary>
        private double DesignTimeSampleValue()
            => Math.Round(_minimum + (_maximum - _minimum) * 0.62, 1);

        #endregion

        #region Helpers

        private static void AddWiredEvent(dynamic config, string descriptor)
        {
            // The base control may already have created config.wiredEvents (pointer/focus events);
            // add to it when present, create it otherwise.
            WiredEvents events = null;
            try { events = config.wiredEvents as WiredEvents; }
            catch (Exception) { events = null; }

            if (events == null)
            {
                events = new WiredEvents();
                config.wiredEvents = events;
            }

            events.Add(new[] { descriptor });
        }

        private void ClampValueIntoRange()
        {
            double clamped = Math.Max(_minimum, Math.Min(_maximum, _value));
            if (clamped != _value)
            {
                _value = clamped;
                SyncAlarmState();
            }
        }

        /// <summary>Adds/removes the custom "alarm" theme state so the mixin can restyle the tile (red border).</summary>
        private void SyncAlarmState()
        {
            try
            {
                bool alarm = _value >= _threshold;
                if (alarm && !HasState(AlarmState)) AddState(AlarmState);
                else if (!alarm && HasState(AlarmState)) RemoveState(AlarmState);
            }
            catch (Exception)
            {
                // states are cosmetic; never let them break a value update
            }
        }

        private static string F(double value)
            => value.ToString(CultureInfo.InvariantCulture);

        #endregion
    }
}
