using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using TicketOps.Services;
using Wisej.Core;
using Wisej.Web;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// The one place that knows the theme names and how the running app changes its look. Screens call
    /// <see cref="Apply"/> with a name; nothing else in the code base mentions "Bootstrap-4".
    ///
    /// <c>Application.Theme = theme</c> re-skins every open window of THIS session without rebuilding a control —
    /// an operator's dark mode must not turn every other operator's console dark, which is what the process-wide
    /// <c>Application.LoadTheme(name)</c> would do. The theme is the built-in JSON embedded in Wisej.Framework.dll
    /// with our mixin (the chip appearance) merged in, because the mixin step of LoadTheme is not public API.
    /// The choice is remembered in the per-session <see cref="SessionContext"/> and mirrored into
    /// <c>Application.Session</c>, so the dashboard reopens the way the operator left it.
    ///
    /// Infrastructure may reference Wisej.NET; Domain and Services never do.
    /// </summary>
    public sealed class ThemeSwitcher
    {
        public const string Light = "Bootstrap-4";
        public const string Dark = "BootstrapDark-4";

        private const string EmbeddedThemePrefix = "Wisej.Platform.Themes.";
        private const string MixinFile = "TicketOps.mixin.theme";

        private readonly SessionContext _session;
        private readonly ILog _log;
        private readonly Dictionary<string, ClientTheme> _sessionThemes = new Dictionary<string, ClientTheme>(StringComparer.OrdinalIgnoreCase);   // per session, not static

        public ThemeSwitcher(SessionContext session, ILog log)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        /// <summary>The name of the theme the session currently renders with (Default.json's "theme" until someone switches).</summary>
        public string Current
        {
            get
            {
                try { return Application.Theme?.Name ?? Light; }
                catch { return Light; }
            }
        }

        public bool IsDark => string.Equals(Current, Dark, StringComparison.OrdinalIgnoreCase);

        /// <summary>Re-skins this session with the named built-in theme and remembers the choice.</summary>
        public void Apply(string themeName)
        {
            if (string.IsNullOrWhiteSpace(themeName))
                throw new ArgumentException("A theme name is required.", nameof(themeName));

            Application.Theme = BuildSessionTheme(themeName);
            Remember(themeName);
        }

        /// <summary>
        /// Called when the dashboard loads: re-applies a theme this session chose earlier (survives a browser refresh,
        /// because the session — and its Application.Session bag — survives it). Nothing saved → keep Default.json's theme.
        /// </summary>
        public void RestoreSaved()
        {
            string saved = _session.Theme ?? ReadSessionBag();
            if (string.IsNullOrEmpty(saved) || string.Equals(saved, Current, StringComparison.OrdinalIgnoreCase))
                return;

            Apply(saved);
        }

        #region Session theme: embedded built-in JSON + our mixin

        private ClientTheme BuildSessionTheme(string themeName)
        {
            if (_sessionThemes.TryGetValue(themeName, out var cached))
                return cached;

            string json = ReadEmbeddedThemeJson(themeName);
            json = MergeMixin(json);

            var theme = new ClientTheme(themeName, json);
            _sessionThemes[themeName] = theme;
            return theme;
        }

        /// <summary>The built-in themes ship as embedded resources of Wisej.Framework.dll ("Wisej.Platform.Themes.Bootstrap-4.theme").</summary>
        private static string ReadEmbeddedThemeJson(string themeName)
        {
            var assembly = typeof(Application).Assembly;
            using (var stream = assembly.GetManifestResourceStream(EmbeddedThemePrefix + themeName + ".theme"))
            {
                if (stream == null)
                    throw new InvalidOperationException($"Theme '{themeName}' is not embedded in {assembly.GetName().Name}.");

                using (var reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Merges Themes/TicketOps.mixin.theme (colours + the "chip" appearance) into the theme.
        /// The csproj copies /Themes next to the binaries, so the file is read from the application base directory.
        /// A missing mixin is logged, not thrown: the app still re-skins, the chips just lose their tint.
        /// </summary>
        private string MergeMixin(string themeJson)
        {
            string path = Path.Combine(AppContext.BaseDirectory, "Themes", MixinFile);
            if (!File.Exists(path))
            {
                _log.Warn(LogLayer.Infrastructure, "ThemeSwitcher.MergeMixin", $"{MixinFile} not found under {AppContext.BaseDirectory} → theme without the chip appearance");
                return themeJson;
            }

            var options = new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true };
            var theme = JsonNode.Parse(themeJson, null, options).AsObject();
            var mixin = JsonNode.Parse(File.ReadAllText(path), null, options).AsObject();

            foreach (string section in new[] { "colors", "appearances" })
            {
                if (!(mixin[section] is JsonObject source))
                    continue;

                if (!(theme[section] is JsonObject target))
                {
                    target = new JsonObject();
                    theme[section] = target;
                }

                foreach (var entry in source)
                    target[entry.Key] = entry.Value?.DeepClone();
            }

            return theme.ToJsonString();
        }

        #endregion

        private void Remember(string themeName)
        {
            _session.Theme = themeName;
            try
            {
                Application.Session.TicketOpsTheme = themeName;        // dynamic per-session bag
            }
            catch (Exception ex)
            {
                _log.Warn(LogLayer.Session, "ThemeSwitcher.Remember", $"Application.Session not writable here ({ex.GetType().Name}) — the SessionContext copy is authoritative");
            }
        }

        private string ReadSessionBag()
        {
            try
            {
                return Application.Session.TicketOpsTheme as string;
            }
            catch
            {
                return null;
            }
        }
    }
}
