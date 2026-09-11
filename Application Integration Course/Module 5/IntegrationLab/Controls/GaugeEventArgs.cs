using System;

namespace IntegrationLab.Controls
{
    /// <summary>
    /// Data for <see cref="SimpleGaugeControl.ThresholdExceeded"/> (data, never behavior).
    /// <see cref="Value"/> is the authoritative server value at the time the event arrived.
    /// </summary>
    public class GaugeThresholdEventArgs : EventArgs
    {
        public GaugeThresholdEventArgs(double value, double threshold)
        {
            this.Value = value;
            this.Threshold = threshold;
        }

        /// <summary>The authoritative server value.</summary>
        public double Value { get; }

        /// <summary>The threshold that was crossed.</summary>
        public double Threshold { get; }
    }
}
