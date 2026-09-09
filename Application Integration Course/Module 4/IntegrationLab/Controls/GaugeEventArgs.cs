using System;

namespace IntegrationLab.Controls
{
    /// <summary>
    /// Data for <see cref="SimpleGauge.ThresholdExceeded"/>. Everything here is copied from the
    /// event payload the client adapter sent (data, never behavior).
    /// </summary>
    public class GaugeEventArgs : EventArgs
    {
        public GaugeEventArgs(double value, double reportedValue)
        {
            this.Value = value;
            this.ReportedValue = reportedValue;
        }

        /// <summary>The authoritative server value at the time the event was raised.</summary>
        public double Value { get; }

        /// <summary>The value the client adapter reported in its payload (for contract checks).</summary>
        public double ReportedValue { get; }
    }

    /// <summary>
    /// Data for <see cref="SimpleGauge.ValueChanged"/>: the browser has rendered a new Value.
    /// </summary>
    public class GaugeValueChangedEventArgs : GaugeEventArgs
    {
        public GaugeValueChangedEventArgs(double value, double reportedValue, double previous)
            : base(value, reportedValue)
        {
            this.Previous = previous;
        }

        /// <summary>The value the browser was showing before this update.</summary>
        public double Previous { get; }
    }

    /// <summary>
    /// Data for <see cref="SimpleGauge.WidgetError"/>: the client adapter caught a vendor failure
    /// and reported it instead of crashing the page.
    /// </summary>
    public class GaugeErrorEventArgs : EventArgs
    {
        public GaugeErrorEventArgs(string phase, string message)
        {
            this.Phase = phase;
            this.Message = message;
        }

        /// <summary>Where it failed: "init", "update" or "animate".</summary>
        public string Phase { get; }

        public string Message { get; }
    }

    public enum TraceDirection { ServerToClient, ClientToServer, Server }

    /// <summary>
    /// One line of the client/server trace (diagnostics; see <see cref="SimpleGauge.Trace"/>).
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
