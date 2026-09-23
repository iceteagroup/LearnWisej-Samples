using System;

namespace VisualOperationsStudio.Models
{
    /// <summary>
    /// One machine reading, with the range and the unit it is meaningful in. Every surface renders
    /// these instances; none of them keeps a private copy of a value.
    /// </summary>
    public class TelemetrySample
    {
        private double reading;

        public TelemetrySample(string machineName, double reading)
            : this(machineName, machineName, "%", 0, 100, reading)
        {
        }

        public TelemetrySample(string machineName, string caption, string unit, double minimum, double maximum, double reading)
        {
            MachineName = machineName;
            Caption = caption;
            Unit = unit;
            Minimum = minimum;
            Maximum = maximum;
            Reading = reading;
        }

        public string MachineName { get; }

        /// <summary>What the reading is called on screen, for example "Spindle load".</summary>
        public string Caption { get; }

        public string Unit { get; }

        public double Minimum { get; }

        public double Maximum { get; }

        /// <summary>
        /// The reading. The setter clamps into <see cref="Minimum"/>..<see cref="Maximum"/>: a value
        /// pushed outside the range is brought back instead of letting a renderer draw off its surface.
        /// </summary>
        public double Reading
        {
            get => this.reading;
            set
            {
                this.reading = Math.Min(Maximum, Math.Max(Minimum, value));
                Timestamp = DateTime.Now;
            }
        }

        public DateTime Timestamp { get; private set; }

        /// <summary>The reading as a line of text, for the row beside the gauges.</summary>
        public string DisplayValue => $"{Reading:0.#} {Unit}";
    }
}
