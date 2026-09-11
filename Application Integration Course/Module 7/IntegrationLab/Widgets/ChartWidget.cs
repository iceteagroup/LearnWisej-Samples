using System;
using System.ComponentModel;
using IntegrationLab.Contracts;
using Wisej.Core;
using Wisej.Web;

namespace IntegrationLab.Widgets
{
    /// <summary>
    /// Server-side wrapper around VendorChart (client adapter: wwwroot/chart-init.js).
    ///
    /// Server event handler #3 — <see cref="OnWebEvent"/>, the general entry point.
    ///
    /// Two entry points exist on a Widget:
    ///   • <c>WidgetEvent</c> / <c>OnWidgetEvent(WidgetEventArgs)</c> is the Widget's single
    ///     catch-all for fireWidgetEvent: it only ever sees {Type, Data} pairs the wrapper fired.
    ///   • <c>OnWebEvent(WisejEventArgs)</c> is the general event entry point of EVERY control,
    ///     shared with the framework: focus, resize, pointer, drag/drop … and the "widgetEvent"
    ///     message that carries a fireWidgetEvent call. This is where fireEvent / fireDataEvent
    ///     land for custom controls too. An override must pass every event it does not own to
    ///     base.OnWebEvent(e); swallowing an unknown event breaks framework behavior.
    ///
    /// This class handles pointClicked in OnWebEvent on purpose, to show that path, and keeps
    /// OnWidgetEvent as a fallback: if the incoming parameter shape is not what we expect the
    /// event is forwarded to base, which raises OnWidgetEvent, and the same validation runs there.
    /// </summary>
    [ToolboxItem(true)]
    [DefaultEvent("PointClicked")]
    [Description("Hosts the VendorChart library; only pointClicked reaches the server.")]
    public class ChartWidget : Widget
    {
        private string _seriesLabel = "Orders";
        private string[] _labels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun" };
        private double[] _values = { 30, 52, 41, 68, 47, 80 };
        private string _theme = "light";

        public ChartWidget()
        {
            this.Packages.Add(new Package { Name = "vendor-chart-css", Source = "wwwroot/vendor-chart.css" });   // a stylesheet is a package too
            this.Packages.Add(new Package { Name = "vendor-chart", Source = "wwwroot/vendor-chart.js" });
            this.InitScript = GetResourceString("IntegrationLab.wwwroot.chart-init.js");
            this.WiredEvents = new[] { "pointClicked", "error" };
            this.Size = new System.Drawing.Size(600, 220);
            PushState();
        }

        #region Server-owned state

        [Browsable(false)]
        public string[] Labels => (string[])_labels.Clone();

        [Browsable(false)]
        public double[] Values => (double[])_values.Clone();

        [DefaultValue("Orders")]
        public string SeriesLabel
        {
            get => _seriesLabel;
            set { _seriesLabel = value ?? ""; PushSeries(); }
        }

        /// <summary>"light" or "dark". The vendor cannot switch theme in place: the adapter destroys, recreates and re-wires.</summary>
        [DefaultValue("light")]
        public string Theme
        {
            get => _theme;
            set
            {
                if (value != "light" && value != "dark") throw new ArgumentOutOfRangeException(nameof(Theme), value, "Theme must be \"light\" or \"dark\".");
                if (_theme == value) return;
                _theme = value;
                ((dynamic)this.Options).theme = value;
            }
        }

        #endregion

        #region .NET events

        /// <summary>Raised when the user clicks a point (a drill-down intent) and the payload passed validation.</summary>
        [Description("Raised when the user clicks a point and the payload passed validation.")]
        public event EventHandler<ChartPointEventArgs> PointClicked;

        /// <summary>Every payload received and every rejection, for the WidgetEvent log.</summary>
        [Browsable(false)]
        public event EventHandler<TraceEventArgs> Trace;

        #endregion

        #region Server event handler #3 — OnWebEvent (general entry point, shared with the framework)

        /// <summary>
        /// Processes every client event addressed to this control. We own exactly one: the
        /// "widgetEvent" whose parameters say type == "pointClicked". Everything else — framework
        /// events and widget events we do not own — MUST go to base.OnWebEvent(e).
        /// </summary>
        protected override void OnWebEvent(WisejEventArgs e)
        {
            if (e.Type == "widgetEvent" && TryReadWidgetEvent(e, out string type, out object data) && type == "pointClicked")
            {
                HandlePointClicked(data);
                return;                                  // handled: do not raise WidgetEvent a second time for it
            }

            // Not ours (focus, resize, pointer …), or a shape we did not recognise:
            // let the framework do its job. For a widgetEvent that means base raises OnWidgetEvent.
            base.OnWebEvent(e);
        }

        /// <summary>
        /// Reads {type, data} out of the incoming "widgetEvent" parameters.
        ///
        /// Client side (wisej.Mixins.js): fireWidgetEvent(type, data) is fireDataEvent("widgetEvent", { type, data }).
        /// The framework wires the event as "widgetEvent(Event)", so the data object lands in
        /// e.Parameters.Event → { type, data }. The shape is read dynamically and defensively
        /// (a flat { type, data } is accepted too); when it is not recognised we return false and
        /// the caller falls back to base.OnWebEvent, so OnWidgetEvent still fires and nothing is lost.
        /// </summary>
        private static bool TryReadWidgetEvent(WisejEventArgs e, out string type, out object data)
        {
            type = null;
            data = null;
            try
            {
                dynamic p = e.Parameters;
                if (p == null) return false;

                dynamic ev = PayloadReader.Get(() => p.Event);       // "widgetEvent(Event)" → Parameters.Event = { type, data }
                if (ev == null) ev = p;                                // fallback: a flat { type, data }

                object t = PayloadReader.Get(() => ev.type);
                if (!(t is string s) || s.Length == 0) return false;
                type = s;
                data = PayloadReader.Get(() => ev.data);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Fallback catch-all. Reached only when OnWebEvent forwarded the widgetEvent to base
        /// (unexpected parameter shape) — the same validation and the same typed event.
        /// </summary>
        protected override void OnWidgetEvent(WidgetEventArgs e)
        {
            switch (e.Type)
            {
                case "pointClicked":
                    HandlePointClicked(e.Data);
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
                    base.OnWidgetEvent(e);
                    break;
            }
        }

        private void HandlePointClicked(object payload)
        {
            dynamic data = payload;
            RaiseTrace(TraceDirection.ClientToServer, "pointClicked", "e.Data = " + PayloadReader.ToJson(payload));

            // --- validate every field before it is used ------------------------------------
            if (!PayloadReader.TryInt(PayloadReader.Get(() => data.index), out int index))
            { Reject("pointClicked", "index is missing or not an integer"); return; }

            if (index < 0 || index >= _labels.Length)
            { Reject("pointClicked", $"index out of range ({index}; 0..{_labels.Length - 1})"); return; }

            if (!PayloadReader.TryString(PayloadReader.Get(() => data.label), out string _))
            { Reject("pointClicked", "label is missing or not a string"); return; }

            if (!PayloadReader.TryDouble(PayloadReader.Get(() => data.value), out double _))
            { Reject("pointClicked", "value is missing or not a finite number"); return; }

            // The index is a lookup key: label and value are resolved from SERVER data.
            // The client copies are never trusted.
            PointClicked?.Invoke(this, new ChartPointEventArgs(index, _labels[index], _values[index]));
        }

        private void Reject(string eventName, string reason)
            => RaiseTrace(TraceDirection.Rejected, eventName, "rejected: " + reason);

        #endregion

        private void PushState()
        {
            dynamic options = this.Options;
            options.theme = _theme;
            PushSeries();
        }

        private void PushSeries()
        {
            dynamic options = this.Options;
            options.labels = (string[])_labels.Clone();
            options.series = new[] { new { label = _seriesLabel, values = (double[])_values.Clone() } };   // camelCase already
        }

        private void RaiseTrace(TraceDirection direction, string name, string payload)
            => Trace?.Invoke(this, new TraceEventArgs(direction, name, payload));
    }
}
