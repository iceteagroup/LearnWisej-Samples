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
    /// Two mechanisms, both re-skinning every open window without rebuilding a control:
    ///  - <c>Application.LoadTheme(name)</c> swaps the GLOBAL theme: every session of this process re-skins,
    ///    and the framework merges the /Themes/*.mixin.theme files (the chip appearance) for us
    ///    (verified in the Integration course; the shape the course brief asks for);
    ///  - <c>Application.Theme = new ClientTheme(name, json)</c> assigns a theme to THIS session only
    ///    (XML docs: "update the current session using the new custom theme") — what a per-operator dark mode
    ///    needs when two operators share one server. The JSON is the built-in theme embedded in
    ///    Wisej.Framework.dll with our mixin merged in by hand, because the mixin step of LoadTheme is not
    ///    public API.
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

        /// <summary>Re-skins the app with the named built-in theme and remembers the choice for this session.</summary>
        public void Apply(string themeName, bool sessionOnly)
        {
            if (string.IsNullOrWhiteSpace(themeName))
                throw new ArgumentException("A theme name is required.", nameof(themeName));

            if (sessionOnly)
            {
                Application.Theme = BuildSessionTheme(themeName);
                _log.Info(LogLayer.Infrastructure, "ThemeSwitcher.Apply",
                    $"Application.Theme = new ClientTheme(\"{themeName}\", embedded JSON + {MixinFile}) · this session only (a second tab keeps its theme)");
            }
            else
            {
                // Global: the framework swaps the theme for every session of this process and applies the default mixins.
                Application.LoadTheme(themeName);
                _log.Info(LogLayer.Infrastructure, "ThemeSwitcher.Apply",
                    $"Application.LoadTheme(\"{themeName}\") · global: every session of this process re-skins, no control was rebuilt");
            }

            Remember(themeName);
        }

        /// <summary>
        /// Called when the dashboard loads: re-applies a theme this session chose earlier (survives a browser refresh,
        /// because the session — and its Application.Session bag — survives it). Nothing saved → keep Default.json's theme.
        /// </summary>
        public void RestoreSaved()
        {
            string saved = _session.Theme ?? ReadSessionBag();
            if (string.IsNullOrEmpty(saved))
            {
                _log.Info(LogLayer.Session, "ThemeSwitcher.RestoreSaved", $"no saved theme for this session → Default.json theme \"{Current}\"");
                return;
            }

            if (string.Equals(saved, Current, StringComparison.OrdinalIgnoreCase))
            {
                _log.Info(LogLayer.Session, "ThemeSwitcher.RestoreSaved", $"saved theme \"{saved}\" already active");
                return;
            }

            _log.Info(LogLayer.Session, "ThemeSwitcher.RestoreSaved", $"saved theme \"{saved}\" → re-applying before the screen paints");
            Apply(saved, sessionOnly: true);
        }

        #region Session-only theme: embedded built-in JSON + our mixin

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
        /// What LoadTheme does for us: merge Themes/TicketOps.mixin.theme (colours + the "chip" appearance) into the theme.
        /// The csproj copies /Themes next to the binaries, so the file is read from the application base directory.
        /// A missing mixin is traced, not thrown: the app still re-skins, the chips just lose their tint.
        /// </summary>
        private string MergeMixin(string themeJson)
        {
            string path = Path.Combine(AppContext.BaseDirectory, "Themes", MixinFile);
            if (!File.Exists(path))
            {
                _log.Warn(LogLayer.Infrastructure, "ThemeSwitcher.MergeMixin", $"{MixinFile} not found under {AppContext.BaseDirectory} → session theme without the chip appearance");
                return themeJson;
            }

            var options = new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true };
            var theme = JsonNode.Parse(themeJson, null, options).AsObject();
            var mixin = JsonNode.Parse(File.ReadAllText(path), null, options).AsObject();

            int merged = 0;
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
                {
                    target[entry.Key] = entry.Value?.DeepClone();
                    merged++;
                }
            }

            _log.Info(LogLayer.Infrastructure, "ThemeSwitcher.MergeMixin", $"{MixinFile}: {merged} colour/appearance entries merged into the session theme");
            return theme.ToJsonString();
        }

        #endregion

        private void Remember(string themeName)
        {
            _session.Theme = themeName;
            try
            {
                Application.Session.TicketOpsTheme = themeName;        // dynamic per-session bag
                _log.Info(LogLayer.Session, "ThemeSwitcher.Remember", $"SessionContext.Theme = \"{themeName}\" · Application.Session.TicketOpsTheme = \"{themeName}\"");
            }
            catch (Exception ex)
            {
                _log.Warn(LogLayer.Session, "ThemeSwitcher.Remember", $"SessionContext.Theme = \"{themeName}\" · Application.Session not writable here ({ex.GetType().Name}) — the SessionContext copy is authoritative");
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
