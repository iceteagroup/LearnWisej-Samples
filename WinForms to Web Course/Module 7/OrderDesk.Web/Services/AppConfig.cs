using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace OrderDesk.Services
{
    /// <summary>
    /// Configuration for the web app (Module 2 moved App.config → Web.config). Reads &lt;appSettings&gt; and
    /// &lt;connectionStrings&gt; with System.Xml.Linq (System.Configuration.ConfigurationManager is not
    /// referenced). Module 7 adds the deployment rule: an environment variable overrides the file, so the
    /// same build runs on IIS, Linux or in a container without editing Web.config —
    /// <c>OrderDesk.StorageRoot</c> ← <c>ORDERDESK_STORAGEROOT</c>.
    /// </summary>
    public static class AppConfig
    {
        private static readonly object Gate = new object();
        private static Dictionary<string, string> _settings;
        private static Dictionary<string, string> _connectionStrings;
        private static string _loadedFrom;

        public static string LoadedFrom { get { EnsureLoaded(); return _loadedFrom; } }

        /// <summary>The value as written in Web.config (null when the key is missing).</summary>
        public static string Get(string key)
        {
            EnsureLoaded();
            return _settings.TryGetValue(key, out var v) ? v : null;
        }

        public static string ConnectionString(string name)
        {
            EnsureLoaded();
            return _connectionStrings.TryGetValue(name, out var v) ? v : null;
        }

        public static IEnumerable<string> ConnectionStringNames { get { EnsureLoaded(); return _connectionStrings.Keys; } }

        /// <summary>ORDERDESK_STORAGEROOT for "OrderDesk.StorageRoot": upper-case, dots → underscores.</summary>
        public static string EnvironmentVariableName(string key) => key.Replace('.', '_').ToUpperInvariant();

        /// <summary>The environment override, or null when the variable is not set.</summary>
        public static string Override(string key)
        {
            var value = Environment.GetEnvironmentVariable(EnvironmentVariableName(key));
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        /// <summary>Environment variable first, Web.config second, then the caller's default.</summary>
        public static string Effective(string key, string defaultValue = null)
            => Override(key) ?? Get(key) ?? defaultValue;

        /// <summary>Storage root resolved under the application folder unless the configured value is absolute.</summary>
        public static string StorageRootPath(string startupPath)
        {
            var root = Effective("OrderDesk.StorageRoot", "App_Data");
            return Path.IsPathRooted(root) ? root : Path.GetFullPath(Path.Combine(startupPath, root));
        }

        private static void EnsureLoaded()
        {
            if (_settings != null) return;
            lock (Gate)
            {
                if (_settings != null) return;
                var settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                var conns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                string path = FindWebConfig();
                if (path != null)
                {
                    var doc = XDocument.Load(path);
                    foreach (var add in doc.Root?.Element("appSettings")?.Elements("add") ?? Enumerable.Empty<XElement>())
                        settings[(string)add.Attribute("key") ?? ""] = (string)add.Attribute("value") ?? "";
                    foreach (var add in doc.Root?.Element("connectionStrings")?.Elements("add") ?? Enumerable.Empty<XElement>())
                        conns[(string)add.Attribute("name") ?? ""] = (string)add.Attribute("connectionString") ?? "";
                }
                _loadedFrom = path ?? "(Web.config not found)";
                _connectionStrings = conns;
                _settings = settings;
            }
        }

        /// <summary>Web.config sits in the project folder (dotnet run) or next to the published app.</summary>
        private static string FindWebConfig()
        {
            var candidates = new List<string>();
            var websitePath = Environment.GetEnvironmentVariable("WEBSITE_PATH");
            if (!string.IsNullOrEmpty(websitePath)) candidates.Add(Path.Combine(websitePath, "Web.config"));
            candidates.Add(Path.Combine(Directory.GetCurrentDirectory(), "Web.config"));
            candidates.Add(Path.Combine(AppContext.BaseDirectory, "Web.config"));
            return candidates.FirstOrDefault(File.Exists);
        }
    }
}
