using System.Collections.Generic;

namespace EnterpriseOps.Domain
{
    /// <summary>
    /// The three theme tokens the TicketOps screens depend on, in the form the regression harness
    /// compares: accent colour, corner radius and the per-priority colours of the work-order grid.
    /// Colours are HTML hex strings so the domain does not depend on System.Drawing.
    /// </summary>
    public class ThemeMap
    {
        public string Name { get; set; }
        public string Source { get; set; }        // where the tokens come from (folder / mixin file)
        public string AccentColor { get; set; }   // "#1565D8"
        public int CornerRadius { get; set; }     // px
        public Dictionary<Priority, string> PriorityColors { get; set; } = new Dictionary<Priority, string>();

        /// <summary>True once the custom 3.x theme has been ported to a Wisej.NET 4 mixin.</summary>
        public bool IsMapped { get; set; }

        public ThemeMap Clone()
        {
            return new ThemeMap
            {
                Name = Name,
                Source = Source,
                AccentColor = AccentColor,
                CornerRadius = CornerRadius,
                PriorityColors = new Dictionary<Priority, string>(PriorityColors),
                IsMapped = IsMapped
            };
        }

        /// <summary>
        /// The "visual diff": every token of <paramref name="candidate"/> that differs from the baseline,
        /// as the one-line messages the harness and the trace print.
        /// </summary>
        public static IReadOnlyList<string> Diff(ThemeMap baseline, ThemeMap candidate)
        {
            var diff = new List<string>();

            if (!string.Equals(baseline.AccentColor, candidate.AccentColor, System.StringComparison.OrdinalIgnoreCase))
                diff.Add($"accent color lost ({candidate.AccentColor} ≠ baseline {baseline.AccentColor})");

            if (baseline.CornerRadius != candidate.CornerRadius)
                diff.Add($"corner radius {candidate.CornerRadius} (baseline {baseline.CornerRadius})");

            int missing = 0;
            foreach (var pair in baseline.PriorityColors)
            {
                if (!candidate.PriorityColors.TryGetValue(pair.Key, out string color)
                    || !string.Equals(color, pair.Value, System.StringComparison.OrdinalIgnoreCase))
                    missing++;
            }
            if (missing > 0)
                diff.Add($"priority colors dropped ({missing}/{baseline.PriorityColors.Count} missing)");

            return diff;
        }
    }
}
