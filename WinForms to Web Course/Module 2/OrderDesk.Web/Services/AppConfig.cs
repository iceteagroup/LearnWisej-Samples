using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Wisej.Web;

namespace OrderDesk.Services
{
    /// <summary>One &lt;connectionStrings&gt;&lt;add&gt; entry of Web.config.</summary>
    public sealed class ConnectionStringSetting
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public string ProviderName { get; set; }

        /// <summary>The "Server=" token, for the trace (never log credentials).</summary>
        public string Server
        {
            get
            {
                var token = (ConnectionString ?? "").Split(';')
                    .Select(p => p.Trim())
                    .FirstOrDefault(p => p.StartsWith("Server=", StringComparison.OrdinalIgnoreCase) || p.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase));
                return token == null ? "?" : token.Substring(token.IndexOf('=') + 1);
            }
        }

        public override string ToString() => Name + " → " + ConnectionString + (string.IsNullOrEmpty(ProviderName) ? "" : " (" + ProviderName + ")");
    }

    /// <summary>
    /// ✓ The web-safe replacement for App.config + ConfigurationManager: the settings live in the
    /// project's Web.config (&lt;appSettings&gt; and &lt;connectionStrings&gt;) and are read with
    /// System.Xml.Linq — no System.Configuration reference, works on net10.0 and net10.0-windows.
    /// (Wisej also exposes Application.Configuration, a dynamic view of the same file; the samples
    /// parse the file directly so every value is readable and verifiable in the trace.)
    /// </summary>
    public sealed class AppConfig
    {
        public string FilePath { get; }
        public IReadOnlyDictionary<string, string> AppSettings { get; }
        public IReadOnlyList<ConnectionStringSetting> ConnectionStrings { get; }

        /// <summary>
        /// Finds Web.config: Application.StartupPath (the web root) first, then WEBSITE_PATH
        /// (set by launchSettings.json for Visual Studio), the current directory, the bin folder.
        /// </summary>
        public static string ResolvePath()
        {
            var candidates = new[]
            {
                SafeStartupPath(),
                Environment.GetEnvironmentVariable("WEBSITE_PATH"),
                Directory.GetCurrentDirectory(),
                AppContext.BaseDirectory,
            };
            foreach (var dir in candidates)
            {
                if (string.IsNullOrEmpty(dir)) continue;
                var path = Path.Combine(dir, "Web.config");
                if (File.Exists(path)) return path;
            }
            return Path.Combine(SafeStartupPath() ?? Directory.GetCurrentDirectory(), "Web.config");
        }

        private static string SafeStartupPath()
        {
            try { return Application.StartupPath; } catch { return null; }
        }

        public static AppConfig Load() => new AppConfig(ResolvePath());

        public AppConfig(string path)
        {
            FilePath = path;
            if (!File.Exists(path))
                throw new FileNotFoundException("Web.config not found at " + path, path);

            var doc = XDocument.Load(path);
            var settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var add in doc.Root?.Element("appSettings")?.Elements("add") ?? Enumerable.Empty<XElement>())
            {
                var key = (string)add.Attribute("key");
                if (!string.IsNullOrEmpty(key)) settings[key] = (string)add.Attribute("value") ?? "";
            }
            AppSettings = settings;

            ConnectionStrings = (doc.Root?.Element("connectionStrings")?.Elements("add") ?? Enumerable.Empty<XElement>())
                .Select(add => new ConnectionStringSetting
                {
                    Name = (string)add.Attribute("name"),
                    ConnectionString = (string)add.Attribute("connectionString"),
                    ProviderName = (string)add.Attribute("providerName"),
                })
                .ToList();
        }

        public string Get(string key, string defaultValue = null)
            => AppSettings.TryGetValue(key, out var v) ? v : defaultValue;

        public ConnectionStringSetting Connection(string name)
            => ConnectionStrings.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));

        /// <summary>App.config "ExportFolder" (C:\Orders) became a relative storage root under the web root (Module 6 uses it).</summary>
        public string StorageRoot => Get("OrderDesk.StorageRoot", "App_Data");

        public string StorageRootPath => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(FilePath) ?? ".", StorageRoot));

        public string DefaultTheme => Get("Wisej.DefaultTheme", "Bootstrap-4");
    }
}
