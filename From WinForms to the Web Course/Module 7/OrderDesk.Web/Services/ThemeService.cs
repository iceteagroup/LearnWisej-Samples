using System;
using System.Collections.Generic;
using Wisej.Web;

namespace OrderDesk.Services
{
    /// <summary>
    /// Theming after parity: the same forms, restyled. The theme comes from Default.json ("theme") and every
    /// Themes/*.mixin.theme file is merged over it at startup. Application.LoadTheme would swap the theme for
    /// EVERY session in the process — a deployment's house style, never a per-user preference.
    /// </summary>
    public static class ThemeService
    {
        /// <summary>The name of the active theme (reported by the health check).</summary>
        public static string CurrentName
        {
            get
            {
                try { return Application.Theme?.Name ?? "(none)"; }
                catch (Exception) { return "(unknown)"; }
            }
        }
    }
}
