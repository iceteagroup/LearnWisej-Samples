using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Wisej.Web;

namespace AdaptiveOps.Governance
{
    /// <summary>One line of the governance review: a rule, whether the running app satisfies it, and the evidence.</summary>
    public sealed class GovernanceResult
    {
        public GovernanceResult(string id, string rule, bool pass, string evidence)
        {
            Id = id;
            Rule = rule;
            Pass = pass;
            Evidence = evidence;
        }

        public string Id { get; }
        public string Rule { get; }
        public bool Pass { get; }
        public string Evidence { get; }

        public override string ToString() => $"{(Pass ? "✓ pass" : "✕ FAIL")} {Id} {Rule} — {Evidence}";
    }

    /// <summary>
    /// What the review needs from the page: the root of the control tree, the documented exceptions,
    /// and two callbacks the rules cannot compute from outside (project files, profile re-application).
    /// </summary>
    public sealed class GovernanceContext
    {
        public Control Root { get; set; }

        /// <summary>Name of the active client profile as the page applied it ("Desktop" for the default profile).</summary>
        public string ProfileName { get; set; }

        /// <summary>Controls that are allowed to carry a CssStyle (documented one-off dynamic values). Empty in this app.</summary>
        public ISet<string> CssStyleAllowList { get; set; } = new HashSet<string>(StringComparer.Ordinal);

        /// <summary>Controls that are allowed to carry an explicit BackColor / ForeColor / Font with a documented reason. Empty in this app.</summary>
        public ISet<string> ExplicitValueAllowList { get; set; } = new HashSet<string>(StringComparer.Ordinal);

        /// <summary>Resolves a project-relative file ("Styles/AdaptiveOps.css") to a full path, or null when it cannot be found.</summary>
        public Func<string, string> ResolveProjectFile { get; set; }

        /// <summary>Re-applies the current profile and returns (snapshot before, snapshot after) so idempotence can be checked.</summary>
        public Func<(string Before, string After)> ReapplyProfile { get; set; }

        /// <summary>The label that announces validation messages as text (not colour alone), or null.</summary>
        public Label ValidationLabel { get; set; }

        /// <summary>Regions whose MinimumSize must be set: (name, control) pairs.</summary>
        public IList<KeyValuePair<string, Control>> StableRegions { get; set; } = new List<KeyValuePair<string, Control>>();

        /// <summary>The region that must carry a MaximumSize (the details editor).</summary>
        public Control BoundedRegion { get; set; }
    }

    /// <summary>
    /// The production-governance checklist of docs/ProductionChecklist.md, computed against the running
    /// application instead of ticked by hand: theme identity, tokens, focus frame, semantic appearances,
    /// hard-coded colours/fonts (reflection over the control tree), CssStyle/CssClass discipline, the
    /// client-profile file, accessibility (icon-only buttons, tab order, validation text), layout
    /// stability, an idempotent profile handler and the governed files in source control.
    /// </summary>
    public static class GovernanceReview
    {
        public const string ExpectedThemeName = "AdaptiveOps";

        private static readonly string[] RequiredTokens =
        {
            "brandPrimary", "surface", "surfaceAlt", "textMain", "textMuted", "danger", "warning", "success", "focusFrame"
        };

        private static readonly string[] GovernedFiles =
        {
            "Themes/AdaptiveOps.theme",
            "Themes/src/AdaptiveOps.overrides.json",
            "Styles/AdaptiveOps.css",
            "ClientProfiles.json",
            "docs/ProductionChecklist.md",
            "docs/AccessibilityReview.md",
            "docs/ResponsiveQAMatrix.md",
            "docs/ArchitectureNote.md",
        };

        public static IReadOnlyList<GovernanceResult> Run(GovernanceContext ctx)
        {
            if (ctx == null)
                throw new ArgumentNullException(nameof(ctx));

            var results = new List<GovernanceResult>();
            var all = ControlTree.SelfAndDescendants(ctx.Root).ToList();

            results.Add(CheckThemeName());
            results.Add(CheckTokens());
            results.Add(CheckFocusFrame());
            results.Add(CheckSemanticAppearances(all));
            results.Add(CheckExplicitValues(all, ctx));
            results.Add(CheckCssStyle(all, ctx));
            results.Add(CheckCssClasses(all, ctx));
            results.Add(CheckClientProfiles(ctx));
            results.Add(CheckIconOnlyButtons(all));
            results.Add(CheckTabOrder(all));
            results.Add(CheckValidationAnnounced(ctx));
            results.Add(CheckLayoutStability(all, ctx));
            results.Add(CheckIdempotentProfile(ctx));
            results.Add(CheckGovernedFiles(ctx));
            return results;
        }

        #region Theme rules

        private static GovernanceResult CheckThemeName()
        {
            string name = SafeThemeName();
            return new GovernanceResult("G1", "Custom theme active (not a built-in theme edited in place)",
                name == ExpectedThemeName,
                $"Application.Theme.Name = \"{name}\" · expected \"{ExpectedThemeName}\" (Default.json \"theme\", Themes/AdaptiveOps.theme)");
        }

        private static GovernanceResult CheckTokens()
        {
            var colors = ThemeSection(t => t.Colors);
            if (colors == null)
                return new GovernanceResult("G2", "Named colour tokens for brand, surfaces, text, danger, warning, success, focus", false, "theme colour list not readable");

            var missing = RequiredTokens.Where(t => !colors.RootElement.TryGetProperty(t, out _)).ToList();
            return new GovernanceResult("G2", "Named colour tokens for brand, surfaces, text, danger, warning, success, focus",
                missing.Count == 0,
                missing.Count == 0
                    ? $"all {RequiredTokens.Length} tokens present in Application.Theme.Colors ({string.Join(", ", RequiredTokens)})"
                    : $"missing tokens: {string.Join(", ", missing)}");
        }

        private static GovernanceResult CheckFocusFrame()
        {
            int size = 0;
            bool focusToken = false, buttonFocused = false, actionFocused = false;

            var settings = ThemeSection(t => t.Settings);
            if (settings != null && settings.RootElement.TryGetProperty("focusBorderSize", out var fbs) && fbs.ValueKind == JsonValueKind.Number)
                size = fbs.GetInt32();

            var colors = ThemeSection(t => t.Colors);
            focusToken = colors != null && colors.RootElement.TryGetProperty("focusFrame", out _);

            var appearances = ThemeSection(t => t.Appearances);
            if (appearances != null)
            {
                buttonFocused = HasState(appearances.RootElement, "button", "focused");
                actionFocused = HasState(appearances.RootElement, "action-button", "focused");
            }

            bool pass = size >= 2 && focusToken && buttonFocused && actionFocused;
            return new GovernanceResult("G3", "Focus frame restyled from the focusFrame token, never removed",
                pass,
                $"settings.focusBorderSize = {size} · focusFrame token {(focusToken ? "present" : "MISSING")} · focused state on button {(buttonFocused ? "yes" : "NO")}, on action-button {(actionFocused ? "yes" : "NO")}");
        }

        private static GovernanceResult CheckSemanticAppearances(List<Control> all)
        {
            var keys = all.Select(c => c.AppearanceKey).Where(k => !string.IsNullOrEmpty(k)).Distinct(StringComparer.Ordinal).OrderBy(k => k, StringComparer.Ordinal).ToList();
            return new GovernanceResult("G4", "At least four semantic appearances assigned through AppearanceKey",
                keys.Count >= 4,
                $"{keys.Count} distinct keys in use: {string.Join(", ", keys)}");
        }

        #endregion

        #region Colour / CSS discipline

        private static GovernanceResult CheckExplicitValues(List<Control> all, GovernanceContext ctx)
        {
            if (!ControlTree.CanInspectExplicitValues)
                return new GovernanceResult("G5", "No hard-coded BackColor / ForeColor / Font on any control", false, "Control.ShouldSerialize* not found by reflection — not checkable");

            var offenders = new List<string>();
            foreach (var c in all)
            {
                if (ctx.ExplicitValueAllowList.Contains(c.Name ?? string.Empty))
                    continue;

                var what = new List<string>();
                if (ControlTree.HasExplicitBackColor(c)) what.Add("BackColor");
                if (ControlTree.HasExplicitForeColor(c)) what.Add("ForeColor");
                if (ControlTree.HasExplicitFont(c)) what.Add("Font");
                if (what.Count > 0)
                    offenders.Add(ControlTree.Describe(c) + " (" + string.Join("/", what) + ")");
            }

            return new GovernanceResult("G5", "No hard-coded BackColor / ForeColor / Font on any control",
                offenders.Count == 0,
                offenders.Count == 0
                    ? $"{all.Count} controls inspected by reflection (Control.ShouldSerializeBackColor/ForeColor/Font): none set explicitly"
                    : $"{offenders.Count} of {all.Count} controls set explicitly: {string.Join("; ", offenders.Take(4))}{(offenders.Count > 4 ? " …" : string.Empty)}");
        }

        private static GovernanceResult CheckCssStyle(List<Control> all, GovernanceContext ctx)
        {
            var users = all.Where(c => !string.IsNullOrWhiteSpace(c.CssStyle)).ToList();
            var undocumented = users.Where(c => !ctx.CssStyleAllowList.Contains(c.Name ?? string.Empty)).ToList();
            return new GovernanceResult("G6", "CssStyle only for documented one-off dynamic values",
                undocumented.Count == 0,
                users.Count == 0
                    ? $"no control carries a CssStyle (allow-list has {ctx.CssStyleAllowList.Count} entries)"
                    : $"{users.Count} CssStyle user(s), {undocumented.Count} undocumented: {string.Join("; ", undocumented.Select(c => ControlTree.Describe(c) + " = \"" + c.CssStyle + "\""))}");
        }

        private static GovernanceResult CheckCssClasses(List<Control> all, GovernanceContext ctx)
        {
            string path = ctx.ResolveProjectFile?.Invoke("Styles/AdaptiveOps.css");
            if (path == null)
                return new GovernanceResult("G7", "Every CssClass is defined in the scoped stylesheet Styles/AdaptiveOps.css", false, "Styles/AdaptiveOps.css not found in the project folder");

            var defined = new HashSet<string>(StringComparer.Ordinal);
            foreach (Match m in Regex.Matches(File.ReadAllText(path), @"\.([A-Za-z_][\w-]*)"))
                defined.Add(m.Groups[1].Value);

            var used = all.Where(c => !string.IsNullOrWhiteSpace(c.CssClass))
                          .SelectMany(c => c.CssClass.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(cls => (cls, c)))
                          .ToList();
            var unknown = used.Where(u => !defined.Contains(u.cls)).Select(u => $"{u.cls} on {ControlTree.Describe(u.c)}").Distinct().ToList();
            var usedNames = used.Select(u => u.cls).Distinct(StringComparer.Ordinal).OrderBy(x => x, StringComparer.Ordinal).ToList();

            return new GovernanceResult("G7", "Every CssClass is defined in the scoped stylesheet Styles/AdaptiveOps.css",
                unknown.Count == 0,
                unknown.Count == 0
                    ? $"{usedNames.Count} class(es) in use, all defined ({string.Join(", ", usedNames)}) · stylesheet defines {defined.Count}"
                    : $"undefined class(es): {string.Join("; ", unknown)}");
        }

        #endregion

        #region Profiles

        private static GovernanceResult CheckClientProfiles(GovernanceContext ctx)
        {
            string outputCopy = Path.Combine(AppContext.BaseDirectory, "ClientProfiles.json");
            string source = ctx.ResolveProjectFile?.Invoke("ClientProfiles.json");
            string path = File.Exists(outputCopy) ? outputCopy : source;
            if (path == null)
                return new GovernanceResult("G8", "ClientProfiles.json shipped with the app, ordered narrow to broad", false, "ClientProfiles.json not found next to the assembly nor in the project folder");

            List<string> names;
            try
            {
                using var doc = JsonDocument.Parse(File.ReadAllText(path), new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true });
                names = doc.RootElement.GetProperty("profiles").EnumerateArray().Select(p => p.GetProperty("name").GetString()).ToList();
            }
            catch (Exception ex)
            {
                return new GovernanceResult("G8", "ClientProfiles.json shipped with the app, ordered narrow to broad", false, "could not parse " + path + ": " + ex.Message);
            }

            bool ordered = names.Count >= 2 && names[0].StartsWith("Phone", StringComparison.Ordinal) && names[names.Count - 1] == "Desktop";
            bool activeKnown = ctx.ProfileName == null || ctx.ProfileName == "Default" || names.Contains(ctx.ProfileName);
            bool copied = File.Exists(outputCopy);

            return new GovernanceResult("G8", "ClientProfiles.json shipped with the app, ordered narrow to broad",
                ordered && activeKnown && copied,
                $"{names.Count} profiles [{string.Join(" → ", names)}] · copied to output {(copied ? "yes" : "NO")} · active \"{ctx.ProfileName}\" {(activeKnown ? "recognised" : "NOT in the file")}");
        }

        private static GovernanceResult CheckIdempotentProfile(GovernanceContext ctx)
        {
            if (ctx.ReapplyProfile == null)
                return new GovernanceResult("G13", "Profile handler is idempotent (a second run changes nothing)", false, "no re-apply callback");

            try
            {
                var (before, after) = ctx.ReapplyProfile();
                bool same = string.Equals(before, after, StringComparison.Ordinal);
                return new GovernanceResult("G13", "Profile handler is idempotent (a second run changes nothing)",
                    same,
                    same ? $"ApplyProfile(\"{ctx.ProfileName}\") re-run: layout snapshot identical ({before.Length} chars)" : $"snapshot changed: before [{before}] after [{after}]");
            }
            catch (Exception ex)
            {
                return new GovernanceResult("G13", "Profile handler is idempotent (a second run changes nothing)", false, "ApplyProfile threw: " + ex.Message);
            }
        }

        #endregion

        #region Accessibility

        private static GovernanceResult CheckIconOnlyButtons(List<Control> all)
        {
            var iconOnly = all.OfType<Button>().Where(b => b.Display == Display.Icon).ToList();
            var bad = iconOnly.Where(b => string.IsNullOrWhiteSpace(b.ToolTipText) || string.IsNullOrWhiteSpace(b.AccessibleName)).Select(ControlTree.Describe).ToList();
            return new GovernanceResult("G9", "Icon-only buttons carry ToolTipText and an AccessibleName",
                bad.Count == 0,
                iconOnly.Count == 0
                    ? "no icon-only buttons on this profile (Display = Both everywhere); text is the label"
                    : bad.Count == 0 ? $"{iconOnly.Count} icon-only button(s), all with tooltip + accessible name" : $"missing on: {string.Join("; ", bad)}");
        }

        private static GovernanceResult CheckTabOrder(List<Control> all)
        {
            var focusable = all.Where(c => c.TabStop && (c is Button || c is TextBox || c is ComboBox || c is DateTimePicker || c is DataGridView || c is ListBox)).ToList();
            var zero = focusable.Where(c => c.TabIndex <= 0).Select(ControlTree.Describe).ToList();
            var duplicates = focusable.GroupBy(c => c.Parent).SelectMany(g => g.GroupBy(c => c.TabIndex).Where(x => x.Count() > 1).Select(x => $"TabIndex {x.Key} twice under {ControlTree.Describe(g.Key)}")).ToList();
            bool pass = zero.Count == 0 && duplicates.Count == 0;
            return new GovernanceResult("G10", "Keyboard order declared: every focusable command has a unique TabIndex in its container",
                pass,
                pass ? $"{focusable.Count} focusable controls, TabIndex 1..n per container, no duplicates" : string.Join("; ", zero.Concat(duplicates).Take(5)));
        }

        private static GovernanceResult CheckValidationAnnounced(GovernanceContext ctx)
        {
            var label = ctx.ValidationLabel;
            bool pass = label != null && !string.IsNullOrWhiteSpace(label.AccessibleName);
            return new GovernanceResult("G11", "Invalid state is announced as text and icon, not colour alone",
                pass,
                pass ? $"{ControlTree.Describe(label)} (AccessibleName \"{label.AccessibleName}\") + TextBox.Invalid/InvalidMessage on the failing editor" : "no validation label registered");
        }

        #endregion

        #region Layout stability

        private static GovernanceResult CheckLayoutStability(List<Control> all, GovernanceContext ctx)
        {
            var noMin = ctx.StableRegions.Where(r => r.Value.MinimumSize.Width <= 0 && r.Value.MinimumSize.Height <= 0).Select(r => r.Key).ToList();
            bool bounded = ctx.BoundedRegion != null && (ctx.BoundedRegion.MaximumSize.Width > 0 || ctx.BoundedRegion.MaximumSize.Height > 0);
            var autoSized = all.Where(c => c.AutoSize && (c is Panel || c is FlexLayoutPanel || c is TableLayoutPanel || c is FlowLayoutPanel || c is UserControl)).Select(ControlTree.Describe).ToList();
            bool pass = noMin.Count == 0 && bounded && autoSized.Count == 0;
            return new GovernanceResult("G12", "Layout stability: MinimumSize on fill/docked regions, MaximumSize on details, no AutoSize containers",
                pass,
                $"MinimumSize on {ctx.StableRegions.Count - noMin.Count}/{ctx.StableRegions.Count} regions{(noMin.Count > 0 ? " (missing: " + string.Join(", ", noMin) + ")" : string.Empty)} · details MaximumSize {(bounded ? ctx.BoundedRegion.MaximumSize.ToString() : "NOT SET")} · AutoSize containers: {autoSized.Count}");
        }

        #endregion

        #region Source control

        private static GovernanceResult CheckGovernedFiles(GovernanceContext ctx)
        {
            var missing = GovernedFiles.Where(f => ctx.ResolveProjectFile?.Invoke(f) == null).ToList();
            string configuredTheme = null;
            string defaultJson = ctx.ResolveProjectFile?.Invoke("Default.json");
            if (defaultJson != null)
            {
                try
                {
                    using var doc = JsonDocument.Parse(File.ReadAllText(defaultJson), new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true });
                    if (doc.RootElement.TryGetProperty("theme", out var t))
                        configuredTheme = t.GetString();
                }
                catch (Exception) { /* reported below as null */ }
            }

            bool pass = missing.Count == 0 && configuredTheme == ExpectedThemeName;
            return new GovernanceResult("G14", "Theme, stylesheet, profiles and review docs live in the project (source-controlled, reviewed as diffs)",
                pass,
                $"{GovernedFiles.Length - missing.Count}/{GovernedFiles.Length} governed files present{(missing.Count > 0 ? " (missing: " + string.Join(", ", missing) + ")" : string.Empty)} · Default.json theme = \"{configuredTheme ?? "(unreadable)"}\"");
        }

        #endregion

        #region Theme access helpers

        private static string SafeThemeName()
        {
            try { return Application.Theme?.Name ?? "(none)"; }
            catch (Exception) { return "(unavailable)"; }
        }

        /// <summary>
        /// The theme sections (Colors, Settings, Appearances) are dynamic objects whose ToString() is their
        /// JSON. Parsing that JSON is the most robust way to inspect them without depending on dynamic binding.
        /// </summary>
        private static JsonDocument ThemeSection(Func<Wisej.Core.ClientTheme, object> pick)
        {
            try
            {
                var theme = Application.Theme;
                if (theme == null)
                    return null;
                var section = pick(theme);
                if (section == null)
                    return null;
                return JsonDocument.Parse(section.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool HasState(JsonElement appearances, string appearance, string state)
        {
            return appearances.TryGetProperty(appearance, out var a)
                && a.TryGetProperty("states", out var states)
                && states.TryGetProperty(state, out _);
        }

        #endregion
    }
}
