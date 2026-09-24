using System;
using System.Collections.Generic;
using System.Drawing;

namespace VisualOperationsStudio.Models
{
    /// <summary>
    /// One asset in the capstone grid. The number is a load: higher is worse, which is why the state
    /// thresholds here read the opposite way round from the health score the Module 3 grid showed.
    /// The painted cells read their values from this object in one step, rather than searching a
    /// collection while the grid is scrolling.
    /// </summary>
    public class AssetStatus
    {
        /// <summary>Above this load the asset is a warning.</summary>
        public const int WarningAbove = 60;

        /// <summary>Above this load the asset is critical.</summary>
        public const int CriticalAbove = 85;

        public AssetStatus(string asset, int load, IReadOnlyList<double> trend)
        {
            Asset = asset;
            Load = load;
            Trend = trend ?? new double[0];
        }

        public string Asset { get; }

        /// <summary>Percentage of capacity in use. Higher is worse.</summary>
        public int Load { get; }

        /// <summary>The readings the Trend cell draws. Fewer than two means there is no line to draw.</summary>
        public IReadOnlyList<double> Trend { get; }

        public Severity State =>
            Load > CriticalAbove ? Severity.Critical :
            Load > WarningAbove ? Severity.Warning : Severity.Normal;

        public Color Tone => ToneFor(Load);

        /// <summary>The chip's text: a shape as well as a colour, so it is not colour-only.</summary>
        public string StateChip =>
            Load > CriticalAbove ? "▲ crit" :
            Load > WarningAbove ? "● warn" : "✓ ok";

        /// <summary>The same state as markup, for the chip column.</summary>
        public string StateHtml
        {
            get
            {
                var tone = ColorTranslator.ToHtml(Tone);
                return "<span style=\"display:inline-block;font-size:10.5px;font-weight:800;" +
                       $"color:{tone};background:{tone}1a;border:1px solid {tone}55;border-radius:999px;padding:2px 8px\">" +
                       $"{StateChip}</span>";
            }
        }

        /// <summary>The word a screen reader gets, beside the colour the eye gets.</summary>
        public string StateWord =>
            Load > CriticalAbove ? "critical" :
            Load > WarningAbove ? "warn" : "ok";

        public static Color ToneFor(int load) =>
            load > CriticalAbove ? Color.FromArgb(224, 90, 90) :
            load > WarningAbove ? Color.FromArgb(232, 161, 60) : Color.FromArgb(31, 157, 107);

        /// <summary>
        /// The trend series the sparkline draws, generated from a fixed seed so every run of the sample
        /// shows the same shape.
        /// </summary>
        public static double[] TrendFor(double seed)
        {
            var points = new double[9];
            for (var i = 0; i < points.Length; i++)
            {
                var v = 0.5 + 0.42 * Math.Sin(seed + i * 0.9) * Math.Cos(seed * 0.7 + i * 0.35);
                points[i] = Math.Min(0.95, Math.Max(0.05, v)) * 100.0;
            }

            return points;
        }
    }
}
