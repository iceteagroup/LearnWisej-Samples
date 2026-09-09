using System;

namespace IntegrationLab.Contracts
{
    /// <summary>
    /// DTO for the <c>thresholdCrossed</c> payload: <c>{ value: number, level: "warn" | "high" }</c>.
    /// Primitives only, copied from the validated client payload — data, never behavior.
    /// Becomes <see cref="Widgets.GaugeWidget.ThresholdCrossed"/>.
    /// </summary>
    public sealed class GaugeThresholdEventArgs : EventArgs
    {
        public GaugeThresholdEventArgs(double value, string level)
        {
            this.Value = value;
            this.Level = level;
        }

        /// <summary>The reading at the moment of the crossing (validated: finite, within Minimum..Maximum).</summary>
        public double Value { get; }

        /// <summary>Which line was crossed: "warn" (WarnAt) or "high" (Threshold).</summary>
        public string Level { get; }

        public override string ToString() => $"{{ Value={Value:0.##}, Level={Level} }}";
    }
}
