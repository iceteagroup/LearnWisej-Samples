using System;
using System.Collections.Generic;
using Wisej.Web;

namespace OrderDesk.Services
{
    /// <summary>
    /// Theming after parity: the same forms, restyled. Application.LoadTheme swaps the theme for EVERY
    /// session in the process (it replaces the shared theme object), which is what a deployment wants
    /// for its house style — and what a per-user preference must not do.
    /// </summary>
    public static class ThemeService
    {
        /// <summary>The themes embedded in Wisej-4 4.1.0 (COOKBOOK, verified list).</summary>
        public static readonly IReadOnlyList<string> BuiltIn = new[]
        {
            "Bootstrap-4", "BootstrapDark-4", "Blue-1", "Blue-2", "Blue-3", "Classic-2",
            "Clear-1", "Clear-2", "Clear-3", "FluentDark-5", "FluentLight-5", "Graphite-3",
            "Material-3", "Material-4", "MaterialDark-4", "Vista-2"
        };

        public static string CurrentName
        {
            get
            {
                try { return Application.Theme?.Name ?? "(none)"; }
                catch (Exception) { return "(unknown)"; }
            }
        }

        /// <summary>Loads a built-in theme by name. Default mixins (every Themes/*.mixin.theme) stay applied.</summary>
        public static void Apply(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("A theme name is required.", nameof(name));
            Application.LoadTheme(name);
        }

        /// <summary>
        /// Reloads the current theme with an explicit mixin list: Application.LoadTheme(name, mixins), where
        /// mixins are the file names in /Themes without the ".mixin.theme" extension. With mixins = null the
        /// framework applies every mixin file it finds; passing the list makes the dependency explicit.
        /// </summary>
        public static void ApplyMixin(string mixinName)
        {
            if (string.IsNullOrWhiteSpace(mixinName)) throw new ArgumentException("A mixin name is required.", nameof(mixinName));
            Application.LoadTheme(CurrentName, new[] { mixinName });
        }

        /// <summary>
        /// Reads back the one value the orderdesk mixin changes (button radius) so the console can prove the
        /// merge happened instead of guessing from the screenshot. Returns null when the theme has no value.
        /// </summary>
        public static string ButtonRadius()
        {
            try
            {
                var value = Application.Theme.GetStyle<object>("button", "radius", "default");
                return value?.ToString();
            }
            catch (Exception ex)
            {
                return "GetStyle threw " + ex.GetType().Name;
            }
        }
    }
}
