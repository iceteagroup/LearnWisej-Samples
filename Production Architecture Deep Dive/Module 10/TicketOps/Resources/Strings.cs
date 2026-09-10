using System.Globalization;
using System.Resources;

namespace TicketOps.Resources
{
    /// <summary>
    /// The typed accessor over Resources/Strings.resx (+ Strings.de.resx). User-facing text lives in the .resx
    /// files, never inline in handlers; code references KEYS and the ResourceManager resolves them for a culture.
    ///
    /// Two ways in:
    ///  - the typed properties (<see cref="ActionFailed"/>, …) resolve for the current request thread's UI culture,
    ///    which Wisej.NET sets from Application.CurrentCulture — the same shape every module's handler uses;
    ///  - <see cref="Get(string, CultureInfo)"/> / <see cref="TryGet"/> take an explicit culture and are what
    ///    ILocalizationService builds on (with the traced fallback for missing keys).
    /// The messages are safe to show: they explain what happened without leaking connection strings or table names.
    /// </summary>
    public static class Strings
    {
        /// <summary>Base name = default namespace + folder + file (TicketOps.Resources.Strings), set explicitly in the csproj.</summary>
        public static readonly ResourceManager ResourceManager =
            new ResourceManager("TicketOps.Resources.Strings", typeof(Strings).Assembly);

        public static string AppTitle => Get("App.Title");
        public static string ActionFailed => Get("Message.ActionFailed");
        public static string Saved => Get("Message.Saved");

        /// <summary>Resolves a key for the current UI culture; a missing key yields "[key]" so a screen never shows an empty label.</summary>
        public static string Get(string key) => Get(key, CultureInfo.CurrentUICulture);

        public static string Get(string key, CultureInfo culture)
            => TryGet(key, culture, out string text) ? text : "[" + key + "]";

        public static bool TryGet(string key, CultureInfo culture, out string text)
        {
            text = string.IsNullOrEmpty(key) ? null : ResourceManager.GetString(key, culture);
            return text != null;
        }
    }
}
