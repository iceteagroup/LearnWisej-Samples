using System;

namespace VisualOperationsStudio.Models
{
    /// <summary>
    /// One machine reading. Every surface on <see cref="VisualOperationsPage"/> renders this instance;
    /// none of them keeps a private copy of the value.
    /// </summary>
    public class TelemetrySample
    {
        private double reading;

        public TelemetrySample(string machineName, double reading)
        {
            MachineName = machineName;
            Reading = reading;
        }

        public string MachineName { get; }

        /// <summary>
        /// Percentage of rated load. The setter clamps: a value pushed outside 0-100 is brought back
        /// into range instead of letting a renderer draw off its surface.
        /// </summary>
        public double Reading
        {
            get => this.reading;
            set
            {
                this.reading = Math.Min(100.0, Math.Max(0.0, value));
                Timestamp = DateTime.Now;
            }
        }

        public DateTime Timestamp { get; private set; }
    }
}
