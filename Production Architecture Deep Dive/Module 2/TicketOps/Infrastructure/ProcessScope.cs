using System;
using System.IO;
using Wisej.Web;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// The "runs once per process" moment. Wisej.NET calls <c>Program.Main</c> once per <i>session</i>, so
    /// anything that must happen once per <i>process</i> — reading the configuration contract — is
    /// parked behind a <see cref="Lazy{T}"/> here. The first session to arrive pays for the read; every
    /// later session reuses the same immutable <see cref="AppSettings"/> instance.
    ///
    /// This is the legitimate use of a static: shared, read-only after construction. It holds no session
    /// or user value.
    /// </summary>
    public static class ProcessScope
    {
        private static readonly Lazy<AppSettings> _settings = new Lazy<AppSettings>(LoadSettings, true);

        public static AppSettings Settings => _settings.Value;

        /// <summary>True when the settings object already existed before this call (i.e. another session loaded it).</summary>
        public static bool SettingsAlreadyLoaded => _settings.IsValueCreated;

        private static AppSettings LoadSettings()
        {
            string startupPath = null;
            string configuredTheme = null;
            try
            {
                // Application.StartupPath is the web root (the project folder under dotnet run / F5) — the
                // same folder Wisej reads Default.json from, so appsettings.json sits next to it.
                startupPath = Application.StartupPath;
                configuredTheme = Application.Configuration != null ? Application.Configuration.ThemeName : null;
            }
            catch (Exception)
            {
                // Outside a session (unit tests) there is no Application context; fall back to the folders below.
            }

            return AppSettings.Load(new[]
            {
                startupPath,
                Environment.GetEnvironmentVariable("WEBSITE_PATH"),
                Directory.GetCurrentDirectory(),
                AppContext.BaseDirectory
            }, configuredTheme);
        }
    }
}
