using System.IO;
using System.Reflection;
using Wisej.Core;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// Builds a <see cref="ClientTheme"/> for ONE session from a theme embedded in Wisej.Framework.dll.
    ///
    /// Why not <c>Application.LoadTheme(name)</c>? Because LoadTheme swaps the theme for <b>every</b> session
    /// on the server (the Theming course verified it) — the framework-level version of the static trap this
    /// module is about. <c>Application.Theme</c>, on the other hand, is a per-session property: assigning a
    /// theme object to it restyles only the browser that asked. So the session's theme is loaded here, from
    /// the same JSON the framework ships (resource <c>Wisej.Platform.Themes.&lt;Name&gt;.theme</c>), and the
    /// screen assigns it to <c>Application.Theme</c>.
    ///
    /// Stateless on purpose: no cache, no "current theme" — per-session data lives on SessionContext.
    /// </summary>
    public static class ThemeCatalog
    {
        /// <summary>Returns a fresh ClientTheme for the named built-in theme, or null when the build does not embed it.</summary>
        public static ClientTheme Load(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            Assembly framework = typeof(ClientTheme).Assembly;
            string resource = $"Wisej.Platform.Themes.{name}.theme";
            using (Stream stream = framework.GetManifestResourceStream(resource))
            {
                if (stream == null)
                    return null;

                using (var reader = new StreamReader(stream))
                    return new ClientTheme(name, reader.ReadToEnd());
            }
        }
    }
}
