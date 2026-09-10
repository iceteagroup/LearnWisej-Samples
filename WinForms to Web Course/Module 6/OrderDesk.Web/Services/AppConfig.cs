using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Wisej.Web;

namespace OrderDesk.Services
{
    /// <summary>
    /// App.config moved to Web.config (Module 2). The web project does not reference
    /// System.Configuration.ConfigurationManager, so the appSettings are read with System.Xml.Linq —
    /// small, verifiable, and it works on Windows and Linux alike.
    /// </summary>
    public static class AppConfig
    {
        private static readonly object Gate = new object();
        private static XDocument _doc;
        private static string _appRoot;

        /// <summary>The folder that holds Web.config / Default.json (the project folder under `dotnet run`).</summary>
        public static string AppRoot
        {
            get
            {
                lock (Gate)
                {
                    if (_appRoot != null) return _appRoot;
                    var candidates = new[] { SafeStartupPath(), Directory.GetCurrentDirectory(), AppContext.BaseDirectory };
                    _appRoot = candidates.FirstOrDefault(c => !string.IsNullOrEmpty(c) && File.Exists(Path.Combine(c, "Web.config")))
                               ?? candidates.First(c => !string.IsNullOrEmpty(c));
                    return _appRoot;
                }
            }
        }

        public static string WebConfigPath => Path.Combine(AppRoot, "Web.config");

        /// <summary>&lt;appSettings&gt;&lt;add key="…" value="…"/&gt;</summary>
        public static string Get(string key, string defaultValue = null)
        {
            var add = Doc()?.Root?.Element("appSettings")?.Elements("add")
                .FirstOrDefault(e => string.Equals((string)e.Attribute("key"), key, StringComparison.OrdinalIgnoreCase));
            var value = (string)add?.Attribute("value");
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }

        /// <summary>&lt;connectionStrings&gt;&lt;add name="…" connectionString="…"/&gt;</summary>
        public static string ConnectionString(string name)
        {
            var add = Doc()?.Root?.Element("connectionStrings")?.Elements("add")
                .FirstOrDefault(e => string.Equals((string)e.Attribute("name"), name, StringComparison.OrdinalIgnoreCase));
            return (string)add?.Attribute("connectionString");
        }

        /// <summary>The configured storage root key (relative to the app folder, forward or back slashes).</summary>
        public const string StorageRootKey = "OrderDesk.StorageRoot";

        public static string StorageRootSetting => Get(StorageRootKey, "App_Data");

        private static XDocument Doc()
        {
            lock (Gate)
            {
                if (_doc == null && File.Exists(WebConfigPath))
                    _doc = XDocument.Load(WebConfigPath);
                return _doc;
            }
        }

        private static string SafeStartupPath()
        {
            try { return Application.StartupPath; }
            catch { return null; }
        }
    }
}
