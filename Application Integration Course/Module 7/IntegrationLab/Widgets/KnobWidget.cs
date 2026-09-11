using System;
using System.ComponentModel;
using IntegrationLab.Contracts;
using Wisej.Web;

namespace IntegrationLab.Widgets
{
    /// <summary>
    /// Server-side wrapper around the jQuery-style VendorKnob plugin (client adapter: wwwroot/knob-init.js).
    ///
    /// Server event handler #2 lives on the PAGE, not here: Window1 subscribes to the raw
    /// <c>WidgetEvent</c> (<c>knob.WidgetEvent += knob_WidgetEvent</c>) and switches on
    /// <c>e.Type</c> — the catch-all path the video shows. This class only owns the state,
    /// offers the payload validation (<see cref="TryReadValueChanged"/>) and lets the page
    /// commit a validated client value (<see cref="AcceptClientValue"/>).
    /// </summary>
    [ToolboxItem(true)]
    [DefaultProperty("Value")]
    [Description("Hosts the VendorKnob jQuery plugin; the page handles its WidgetEvent.")]
    public class KnobWidget : Widget
    {
        private double _value = 50;
        private double _minimum = 0;
        private double _maximum = 100;
        private double _step = 1;
        private string _label = "Pressure";
        private string _units = "";

        public KnobWidget()
        {
            // load order matters: the plugin throws if jQuery is not there first
            this.Packages.Add(new Package { Name = "jquery-lite", Source = "wwwroot/jquery-lite.js" });
            this.Packages.Add(new Package { Name = "vendor-knob", Source = "wwwroot/vendor-knob.js" });
            this.InitScript = GetResourceString("IntegrationLab.wwwroot.knob-init.js");
            this.WiredEvents = new[] { "valueChanged", "error" };
            this.Size = new System.Drawing.Size(200, 200);
            PushState();
        }

        #region Server-owned state

        [DefaultValue(50.0)]
        public double Value
        {
            get => _value;
            set
            {
                Validate(value, out string why);
                if (why != null) throw new ArgumentOutOfRangeException(nameof(Value), value, why);
                if (_value == value) return;
                _value = value;
                dynamic options = this.Options;
                options.value = value;                  // → update(options) → vendor setValue → knobchange → valueChanged {source:"server"}
            }
        }

        [DefaultValue(0.0)]
        public double Minimum { get => _minimum; set { if (value >= _maximum) throw new ArgumentOutOfRangeException(nameof(Minimum)); _minimum = value; ((dynamic)this.Options).min = value; } }

        [DefaultValue(100.0)]
        public double Maximum { get => _maximum; set { if (value <= _minimum) throw new ArgumentOutOfRangeException(nameof(Maximum)); _maximum = value; ((dynamic)this.Options).max = value; } }

        [DefaultValue(1.0)]
        public double Step { get => _step; set { if (value <= 0) throw new ArgumentOutOfRangeException(nameof(Step)); _step = value; ((dynamic)this.Options).step = value; } }

        [DefaultValue("Pressure")]
        public string Label { get => _label; set { _label = value ?? ""; ((dynamic)this.Options).label = _label; } }

        [DefaultValue("")]
        public string Units { get => _units; set { _units = value ?? ""; ((dynamic)this.Options).units = _units; } }

        #endregion

        /// <summary>
        /// Validates a raw <c>valueChanged</c> payload field by field and shapes it into the DTO.
        /// Returns false with a reason when anything is missing, of the wrong type or outside the
        /// server-owned range. Nothing in the payload is trusted before this returns true.
        /// </summary>
        public bool TryReadValueChanged(object payload, out KnobValueEventArgs args, out string reason)
        {
            args = null;
            dynamic data = payload;

            if (!PayloadReader.TryDouble(PayloadReader.Get(() => data.value), out double value))
            { reason = "value is missing or not a finite number"; return false; }

            Validate(value, out reason);
            if (reason != null) return false;

            if (!PayloadReader.TryString(PayloadReader.Get(() => data.source), out string source) || (source != "user" && source != "server"))
            { reason = "source must be \"user\" or \"server\""; return false; }

            args = new KnobValueEventArgs(value, source);
            return true;
        }

        /// <summary>
        /// Commits a validated client value as server state WITHOUT going through the Value setter
        /// (which would render it back and echo another valueChanged). The Options copy is kept in
        /// sync so a later re-render shows the same number; the vendor ignores an unchanged value.
        /// </summary>
        public void AcceptClientValue(KnobValueEventArgs e)
        {
            if (_value == e.Value) return;
            _value = e.Value;
            dynamic options = this.Options;
            options.value = e.Value;
        }

        /// <summary>Call("pulse") — an imperative vendor behavior (visual acknowledgement).</summary>
        public void Pulse() => this.Call("pulse");

        private void Validate(double value, out string why)
        {
            why = null;
            if (double.IsNaN(value) || double.IsInfinity(value)) why = "value must be a finite number";
            else if (value < _minimum || value > _maximum) why = $"value {PayloadReader.F(value)} outside {PayloadReader.F(_minimum)}..{PayloadReader.F(_maximum)}";
            else if (Math.Abs((value - _minimum) / _step - Math.Round((value - _minimum) / _step)) > 1e-6) why = $"value {PayloadReader.F(value)} is not on the step grid ({PayloadReader.F(_step)})";
        }

        private void PushState()
        {
            dynamic options = this.Options;
            options.value = _value;
            options.min = _minimum;
            options.max = _maximum;
            options.step = _step;
            options.label = _label;
            options.units = _units;
            options.color = "#1a86ff";
        }
    }
}
