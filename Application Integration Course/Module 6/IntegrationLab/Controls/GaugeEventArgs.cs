using System;

namespace IntegrationLab.Controls
{
    /// <summary>
    /// Data for <see cref="SimpleGauge.ThresholdExceeded"/> and <see cref="SimpleGauge.RangeChanged"/>.
    /// Everything here is copied from the event payload the client widget sent (data, never behavior).
    /// </summary>
    public class GaugeEventArgs : EventArgs
    {
        public GaugeEventArgs(double value, string range)
        {
            this.Value = value;
            this.Range = range;
        }

        /// <summary>The authoritative server value at the time the event was raised.</summary>
        public double Value { get; }

        /// <summary>Range name reported by the widget: "normal", "warm" or "high".</summary>
        public string Range { get; }
    }

    /// <summary>
    /// Data for <see cref="SimpleGauge.WidgetError"/>: the client adapter caught a vendor
    /// failure and reported it instead of crashing the page.
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

    /// <summary>One line of the command trace: a command, result or event that crossed the wire.</summary>
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
