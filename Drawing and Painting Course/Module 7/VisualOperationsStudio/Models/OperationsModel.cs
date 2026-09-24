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
            Cpu = new TelemetrySample("PUMP-04", "CPU · PUMP-04", "%", 0, 100, 72);

            Assets = new List<AssetStatus>
            {
                new AssetStatus("PUMP-04", 72, AssetStatus.TrendFor(0.4)),
                new AssetStatus("MIX-11", 91, AssetStatus.TrendFor(1.6)),
                new AssetStatus("DRY-02", 38, AssetStatus.TrendFor(2.7)),
                new AssetStatus("PACK-07", 64, AssetStatus.TrendFor(3.9)),
            };

            Topology = TopologyScene.CreateAssets();
            Metrics = new RenderMetrics();
        }

        /// <summary>The reading the painted gauge shows. The grid's PUMP-04 row shows the same number.</summary>
        public TelemetrySample Cpu { get; }

        public IEnumerable<TelemetrySample> Readings
        {
            get { yield return Cpu; }
        }

        public List<AssetStatus> Assets { get; }

        public TopologyScene Topology { get; }

        public RenderMetrics Metrics { get; }

        /// <summary>
        /// The one set of severity thresholds. Every surface asks this, so none of them can drift.
        /// </summary>
        public Severity SeverityOf(TelemetrySample sample) => SeverityOfLoad((int)sample.Reading);

        public Severity SeverityOfLoad(int load) =>
            load > AssetStatus.CriticalAbove ? Severity.Critical :
            load > AssetStatus.WarningAbove ? Severity.Warning : Severity.Normal;

        /// <summary>How many assets are in each state, for the accessible table.</summary>
        public Dictionary<Severity, int> AssetSeverityCounts()
        {
            var counts = new Dictionary<Severity, int>
            {
                { Severity.Normal, 0 },
                { Severity.Warning, 0 },
                { Severity.Critical, 0 },
            };

            foreach (var asset in Assets)
                counts[asset.State]++;

            return counts;
        }

        /// <summary>
        /// Every value the graphics show, as one line of text. A reading that exists only inside a
        /// picture has been lost for part of the audience.
        /// </summary>
        public string AccessibleTable()
        {
            var parts = new List<string>();
            foreach (var asset in Assets)
                parts.Add($"{asset.Asset} {asset.Load}% {asset.StateWord}");

            return "Accessible data table · " + string.Join(" · ", parts);
        }
    }

    public enum Severity
    {
        Normal,
        Warning,
        Critical,
    }
}
