using System;

namespace IntegrationLab.Widgets
{
    /// <summary>
    /// Data for <see cref="TemperatureGauge.ThresholdExceeded"/> and
    /// <see cref="TemperatureGauge.RangeChanged"/>. Everything here is copied
    /// from the event payload the client widget sent (data, never behavior).
    /// </summary>
    public class GaugeEventArgs : EventArgs
    {
        public GaugeEventArgs(double value, string range, double reportedValue)
        {
            this.Value = value;
            this.Range = range;
            this.ReportedValue = reportedValue;
        }

        /// <summary>The authoritative server value at the time the event was raised.</summary>
        public double Value { get; }

        /// <summary>Range name reported by the widget: "normal", "warm" or "high".</summary>
        public string Range { get; }

        /// <summary>The value the client widget reported in its payload.</summary>
        public double ReportedValue { get; }
    }

    /// <summary>
    /// Data for <see cref="TemperatureGauge.WidgetError"/>: the client adapter
    /// caught a vendor failure and reported it instead of crashing the page.
    /// </summary>
    public class GaugeErrorEventArgs : EventArgs
    {
        public GaugeErrorEventArgs(string phase, string message)
        {
            this.Phase = phase;
            this.Message = message;
        }

        public string Phase { get; }
        public string Message { get; }
    }

    public enum TraceDirection { ServerToClient, ClientToServer }

    /// <summary>
    /// One message that crossed the wire, for the Sensor Monitor's trace.
    /// </summary>
    public class TraceEventArgs : EventArgs
    {
        public TraceEventArgs(TraceDirection direction, string name, string payload)
        {
            this.Direction = direction;
            this.Name = name;
            this.Payload = payload;
        }

        public TraceDirection Direction { get; }
        public string Name { get; }
        public string Payload { get; }
    }
}
