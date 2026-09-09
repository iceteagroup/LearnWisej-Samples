using System;

namespace IntegrationLab.Controls
{
    /// <summary>
    /// Data for <see cref="SimpleGauge.ThresholdExceeded"/> and <see cref="SimpleGauge.RangeChanged"/>.
    /// Everything here is copied from the event payload the client widget sent (data, never behavior).
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

        /// <summary>The value the client widget reported in its payload (for contract checks).</summary>
        public double ReportedValue { get; }
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

    /// <summary>
    /// Data for <see cref="SimpleGauge.LeakDetected"/>: the client adapter found a
    /// <c>debugDump</c> object in its options and measured what actually reached the browser.
    /// </summary>
    public class LeakDetectedEventArgs : EventArgs
    {
        public LeakDetectedEventArgs(int bytes, int keys, string sample)
        {
            this.Bytes = bytes;
            this.Keys = keys;
            this.Sample = sample;
        }

        /// <summary>Length of <c>JSON.stringify(options.debugDump)</c> in the browser.</summary>
        public int Bytes { get; }

        /// <summary>Number of top-level keys the browser received.</summary>
        public int Keys { get; }

        /// <summary>A few of the keys the browser can now read (e.g. "customer.taxId").</summary>
        public string Sample { get; }
    }

    public enum TraceDirection { ServerToClient, ClientToServer, Server }

    /// <summary>One line of the client/server trace shown by the lab UI.</summary>
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
