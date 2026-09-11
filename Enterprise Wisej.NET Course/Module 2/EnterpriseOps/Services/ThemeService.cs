using System.Collections.Generic;
using EnterpriseOps.Data;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// Owns the current theme map of the migrated app and the 3.5 baseline it is compared against.
    /// Three moves, matching the dossier row "Themes / resources":
    /// upgrade (unmapped) → rollback (theme folder copy restored) → resource mapping (mixin applied).
    /// </summary>
    public class ThemeService
    {
        private readonly ThemeStore _store;
        private readonly ActivityTrace _trace;

        public ThemeService(ThemeStore store, ActivityTrace trace)
        {
            _store = store;
            _trace = trace;
            Baseline = store.Baseline();
            Current = store.UnmappedAfterUpgrade();
        }

        /// <summary>The 3.5 look. Every diff runs against this.</summary>
        public ThemeMap Baseline { get; }

        /// <summary>What the migrated WorkOrdersPage paints with right now.</summary>
        public ThemeMap Current { get; private set; }

        public IReadOnlyList<string> Diff() => ThemeMap.Diff(Baseline, Current);

        /// <summary>Fallback point "theme folder copy": the pre-step copy of Themes/Blue-2019 is back; the engine still shows its defaults until the mixin is mapped.</summary>
        public void RestoreFolderCopy()
        {
            Current = _store.UnmappedAfterUpgrade();
            Current.Name = "Blue-2019 folder copy restored (3.x format — engine defaults until mapped)";
            Current.Source = "Themes/Blue-2019/*.json restored from the pre-step-4 copy";
            _trace.Data($"ThemeStore → Themes/Blue-2019 restored from the pre-step copy; migrated (unmapped) theme discarded");
        }

        /// <summary>Resource mapping: the three tokens ported to a Wisej.NET 4 mixin.</summary>
        public void ApplyMappedMixin()
        {
            Current = _store.MappedMixin();
            _trace.Data($"ThemeStore → {Current.Source}: accent {Current.AccentColor}, radius {Current.CornerRadius}, priority colors {Current.PriorityColors.Count}/3 mapped");
        }
    }
}
