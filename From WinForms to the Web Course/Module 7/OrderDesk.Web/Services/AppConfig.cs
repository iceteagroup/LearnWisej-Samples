using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Wisej.Web;

namespace OrderDesk.Services
{
    /// <summary>
    /// Web.config appSettings, read with System.Xml.Linq (System.Configuration is not referenced —
    /// Module 2 decision). Every value has a default so a missing key never stops the app.
    /// </summary>
    public static class AppConfig
    {
        public static string Get(string key, string fallback)
        {
            try
            {
                var path = Path.Combine(BaseFolder, "Web.config");
                if (!File.Exists(path)) return fallback;
                var doc = XDocument.Load(path);
                var value = doc.Root?.Element("appSettings")?.Elements("add")
                    .FirstOrDefault(e => (string)e.Attribute("key") == key)?.Attribute("value")?.Value;
                return string.IsNullOrEmpty(value) ? fallback : value;
            }
            catch (Exception)
            {
                return fallback;
            }
        }

        /// <summary>
        /// The content root. Inside a Wisej session this is Application.StartupPath (the project folder under
        /// dotnet run); outside a session (the /health endpoint) it falls back to the process directory.
        /// </summary>
        public static string BaseFolder
        {
            get
            {
                try
                {
                    var path = Application.StartupPath;
                    if (!string.IsNullOrEmpty(path)) return path;
                }
                catch (Exception)
                {
                    // no session context (health endpoint, background thread) — fall through
                }
                return Directory.GetCurrentDirectory();
            }
        }

        /// <summary>The server-side storage root every file operation is confined to (Module 6 rule).</summary>
        public static string StorageRoot
        {
            get
            {
                var configured = Get("OrderDesk.StorageRoot", "App_Data");
                var root = Path.IsPathRooted(configured) ? configured : Path.Combine(BaseFolder, configured);
                return Path.GetFullPath(root);
            }
        }

        public static string ThemeMixin => Get("OrderDesk.ThemeMixin", "orderdesk");

        public static string Version => Get("OrderDesk.Version", "7.0.0");
    }
}
