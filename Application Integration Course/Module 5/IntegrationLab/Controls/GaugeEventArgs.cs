using System;

namespace IntegrationLab.Controls
{
    /// <summary>
    /// Data for <see cref="SimpleGaugeControl.ThresholdExceeded"/>. Everything here is
    /// copied from the wired-event payload the client class sent (data, never behavior);
    /// <see cref="Value"/> is the authoritative server value at the time the event arrived.
    /// </summary>
    public class GaugeThresholdEventArgs : EventArgs
    {
        public GaugeThresholdEventArgs(double value, double reportedValue, double threshold)
        {
            this.Value = value;
            this.ReportedValue = reportedValue;
            this.Threshold = threshold;
        }

        /// <summary>The authoritative server value.</summary>
        public double Value { get; }

        /// <summary>The value the client class reported in its payload (for contract checks).</summary>
        public double ReportedValue { get; }

        /// <summary>The threshold that was crossed.</summary>
        public double Threshold { get; }
    }

    public enum TraceDirection { ServerToClient, ClientToServer, Server }

    /// <summary>
    /// One line of the client/server trace shown by the lab UI.
    /// </summary>
    public class TraceEventArgs : EventArgs
    {
        public TraceEventArgs(TraceDirection direction, string name, string payload, bool fromRender = false)
        {
            this.Direction = direction;
            this.Name = name;
            this.Payload = payload;
            this.FromRender = fromRender;
        }

        public TraceDirection Direction { get; }
        public string Name { get; }
        public string Payload { get; }

        /// <summary>
        /// True when the line was raised from inside <see cref="SimpleGaugeControl.OnWebRender"/>,
        /// i.e. while Wisej.NET is serializing the response. The page queues these lines and
        /// appends them to the ListBox on the next request instead of touching controls mid-render.
        /// </summary>
        public bool FromRender { get; }
    }
}
