using EnterpriseOps.Domain;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// The three theme states the migration passes through, as token sets the harness can compare.
    /// No real theme files are loaded — the point of the lab is the <b>comparison</b>, and the
    /// migrated WorkOrdersPage paints itself from whichever map is current.
    /// </summary>
    public class ThemeStore
    {
        /// <summary>What the 3.5 app looked like: the custom "Blue-2019" theme. This is the baseline every diff runs against.</summary>
        public ThemeMap Baseline() => new ThemeMap
        {
            Name = "Blue-2019 (Wisej.NET 3.5 custom theme)",
            Source = "Themes/Blue-2019/*.json (3.x format)",
            AccentColor = "#1565D8",
            CornerRadius = 7,
            PriorityColors =
            {
                [Priority.High] = "#C0392B",
                [Priority.Normal] = "#B9770E",
                [Priority.Low] = "#1F8A4C",
            },
            IsMapped = true,
        };

        /// <summary>
        /// What the app looks like right after the package upgrade: the 3.x theme folder does not load on 4,
        /// so the engine falls back to its defaults — grey accent, square corners, no priority colours.
        /// It compiles. It runs. It is wrong.
        /// </summary>
        public ThemeMap UnmappedAfterUpgrade() => new ThemeMap
        {
            Name = "Wisej.NET 4 engine default (Blue-2019 not mapped)",
            Source = "Themes/Blue-2019/*.json ignored by the 4.x engine → Bootstrap-4 defaults",
            AccentColor = "#6B7C8F",
            CornerRadius = 0,
            IsMapped = false,
        };

        /// <summary>After resource mapping: the same three tokens ported to a Wisej.NET 4 mixin over Bootstrap-4.</summary>
        public ThemeMap MappedMixin() => new ThemeMap
        {
            Name = "Blue-2019 mixin (Wisej.NET 4)",
            Source = "Themes/Blue-2019.mixin.json (4.x mixin over Bootstrap-4)",
            AccentColor = "#1565D8",
            CornerRadius = 7,
            PriorityColors =
            {
                [Priority.High] = "#C0392B",
                [Priority.Normal] = "#B9770E",
                [Priority.Low] = "#1F8A4C",
            },
            IsMapped = true,
        };
    }
}
