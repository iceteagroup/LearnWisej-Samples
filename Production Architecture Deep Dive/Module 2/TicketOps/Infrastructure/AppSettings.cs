using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// The application-wide settings: the deployable contract in <c>appsettings.json</c>, read <b>once per
    /// process</b> and exposed as an immutable object. Every session receives the same instance through
    /// AppComposition; nobody can change a value at runtime, which is what makes sharing it safe.
    ///
    /// Compare: what differs per connected operator (user, tenant, the theme they switched to) is NOT
    /// here — it is on <c>SessionContext</c>. Wisej.NET's own Default.json values (theme, sessionTimeout)
    /// are read through <c>Application.Configuration</c> and shown next to these on the diagnostics page.
    /// </summary>
    public sealed class AppSettings
    {
        public string Environment { get; }
        public string BuildVersion { get; }
        public string DispatchApiBaseUrl { get; }
        public int UploadLimitMB { get; }
        public string LoggingLevel { get; }
        public string DefaultTenant { get; }
        public string DefaultTheme { get; }
        public IReadOnlyList<string> AvailableThemes { get; }

        /// <summary>Where the values came from — the file path, or "(defaults)" when no file was found.</summary>
        public string LoadedFrom { get; }
        public DateTime LoadedAtUtc { get; }

        private AppSettings(string environment, string buildVersion, string dispatchApiBaseUrl, int uploadLimitMB,
            string loggingLevel, string defaultTenant, string defaultTheme, IReadOnlyList<string> availableThemes, string loadedFrom)
        {
            Environment = environment;
            BuildVersion = buildVersion;
            DispatchApiBaseUrl = dispatchApiBaseUrl;
            UploadLimitMB = uploadLimitMB;
            LoggingLevel = loggingLevel;
            DefaultTenant = defaultTenant;
            DefaultTheme = defaultTheme;
            AvailableThemes = availableThemes;
            LoadedFrom = loadedFrom;
            LoadedAtUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// Reads <c>appsettings.json</c> from the first folder that has one. Called exactly once per process
        /// by <see cref="ProcessScope"/>; a missing or broken file falls back to documented defaults so the
        /// app still starts (the diagnostics page shows "(defaults)" as the source).
        /// </summary>
        public static AppSettings Load(IEnumerable<string> candidateFolders, string defaultTheme)
        {
            string environment = "Training";
            string build = "2.0.0-lab";
            string dispatch = "https://api.ticketops.local/v1";
            int uploadLimit = 25;
            string logging = "Information";
            string defaultTenant = "Contoso";
            var themes = new List<string> { "Bootstrap-4", "Material-3", "FluentDark-5" };
            string loadedFrom = "(defaults)";

            foreach (var folder in candidateFolders ?? new string[0])
            {
                if (string.IsNullOrEmpty(folder))
                    continue;

                string path = Path.Combine(folder, "appsettings.json");
                if (!File.Exists(path))
                    continue;

                try
                {
                    using (var doc = JsonDocument.Parse(File.ReadAllText(path)))
                    {
                        if (doc.RootElement.TryGetProperty("TicketOps", out var section))
                        {
                            environment = ReadString(section, "Environment", environment);
                            build = ReadString(section, "BuildVersion", build);
                            dispatch = ReadString(section, "DispatchApiBaseUrl", dispatch);
                            uploadLimit = ReadInt(section, "UploadLimitMB", uploadLimit);
                            logging = ReadString(section, "LoggingLevel", logging);
                            defaultTenant = ReadString(section, "DefaultTenant", defaultTenant);
                            if (section.TryGetProperty("AvailableThemes", out var list) && list.ValueKind == JsonValueKind.Array)
                            {
                                themes = new List<string>();
                                foreach (var item in list.EnumerateArray())
                                    if (item.ValueKind == JsonValueKind.String) themes.Add(item.GetString());
                            }
                        }
                    }
                    loadedFrom = path;
                }
                catch (JsonException)
                {
                    loadedFrom = path + " (unreadable — defaults used)";
                }
                break;
            }

            return new AppSettings(environment, build, dispatch, uploadLimit, logging, defaultTenant,
                defaultTheme ?? (themes.Count > 0 ? themes[0] : null), themes, loadedFrom);
        }

        private static string ReadString(JsonElement section, string name, string fallback)
            => section.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : fallback;

        private static int ReadInt(JsonElement section, string name, int fallback)
            => section.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.Number && v.TryGetInt32(out int n) ? n : fallback;
    }
}
