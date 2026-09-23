using System.Collections.Generic;
using VisualOperationsStudio.Diagnostics;

namespace VisualOperationsStudio.Models
{
    /// <summary>
    /// One model behind all four surfaces. Before this existed, the painted gauge, the painted cells,
    /// the Canvas topology and the PNG export each kept their own thresholds and their own idea of what
    /// was selected - move the warning line once and the exported image quietly contradicted the screen
    /// it was generated from. That is not a rendering bug; it is four copies of one business rule.
    ///
    /// It lives on the session (one instance per <see cref="VisualOperationsPage"/>), never in a static
    /// field, which would be one plant shared by every user of the application.
    /// </summary>
    public class OperationsModel
    {
        public OperationsModel()
        {
            SpindleLoad = new TelemetrySample("Line 3 Press", "Line 3 - Spindle load", "%", 0, 100, 34);
            CoolantTemp = new TelemetrySample("Line 3 Press", "Line 3 - Coolant temp", "°C", 0, 120, 61);
            CycleTime = new TelemetrySample("Line 3 Press", "Line 3 - Cycle time", "s", 0, 90, 47);

            Machines = MachineStatusGenerator.Generate(1000);
            Topology = TopologyScene.CreatePlant();
            Metrics = new RenderMetrics();
        }

        public TelemetrySample SpindleLoad { get; }

        public TelemetrySample CoolantTemp { get; }

        public TelemetrySample CycleTime { get; }

        public IEnumerable<TelemetrySample> Readings
        {
            get
            {
                yield return SpindleLoad;
                yield return CoolantTemp;
                yield return CycleTime;
            }
        }

        public List<MachineStatus> Machines { get; }

        public TopologyScene Topology { get; }

        public RenderMetrics Metrics { get; }

        /// <summary>
        /// The one set of severity thresholds. Every surface asks this, so none of them can drift.
        /// </summary>
        public Severity SeverityOf(TelemetrySample sample, double warning, double critical) =>
            sample.Reading >= critical ? Severity.Critical :
            sample.Reading >= warning ? Severity.Warning : Severity.Normal;

        public Severity SeverityOfHealth(int health) =>
            health < MachineStatus.CriticalBelow ? Severity.Critical :
            health < MachineStatus.WarningBelow ? Severity.Warning : Severity.Normal;

        /// <summary>How many machines are in each state, for the accessible table.</summary>
        public Dictionary<Severity, int> MachineSeverityCounts()
        {
            var counts = new Dictionary<Severity, int>
            {
                { Severity.Normal, 0 },
                { Severity.Warning, 0 },
                { Severity.Critical, 0 },
            };

            foreach (var machine in Machines)
                counts[SeverityOfHealth(machine.Health)]++;

            return counts;
        }
    }

    public enum Severity
    {
        Normal,
        Warning,
        Critical,
    }
}
