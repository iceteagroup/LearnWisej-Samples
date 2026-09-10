using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace OrderDesk.Legacy
{
    /// <summary>
    /// ✕ The desktop way to read configuration. System.Configuration.ConfigurationManager reads
    /// "&lt;entry assembly&gt;.config" from the folder of the executable: App.config is compiled into
    /// LegacyOrderDesk.exe.config and sits next to LegacyOrderDesk.exe on the user's PC.
    ///
    /// This class does exactly what ConfigurationManager does — look for that file next to the
    /// entry assembly — without referencing System.Configuration (the web project does not have
    /// it). On the web server there is no exe and no per-user file, so the lookup fails; that
    /// failure is the Module 2 demo. The web-safe replacement is Services/AppConfig (Web.config).
    /// </summary>
    public static class AppConfig
    {
        /// <summary>Where the file lived on the desktop (for the trace).</summary>
        public const string DesktopConfigPath = @"C:\Program Files\LegacyOrderDesk\LegacyOrderDesk.exe.config";

        /// <summary>Where ConfigurationManager would look on THIS machine: next to the entry assembly.</summary>
        public static string ServerConfigPath
        {
            get
            {
                var location = Assembly.GetEntryAssembly()?.Location;
                if (string.IsNullOrEmpty(location))
                    location = Path.Combine(AppContext.BaseDirectory, "OrderDesk.dll");
                return location + ".config";
            }
        }

        /// <summary>ConfigurationManager.AppSettings[key] — throws when the config file is not there.</summary>
        public static string GetAppSetting(string key)
        {
            var doc = Open();
            return doc.Root?.Element("appSettings")?.Elements("add")
                .FirstOrDefault(e => (string)e.Attribute("key") == key)?.Attribute("value")?.Value;
        }

        /// <summary>ConfigurationManager.ConnectionStrings[name] — throws when the config file is not there.</summary>
        public static string GetConnectionString(string name)
        {
            var doc = Open();
            return doc.Root?.Element("connectionStrings")?.Elements("add")
                .FirstOrDefault(e => (string)e.Attribute("name") == name)?.Attribute("connectionString")?.Value;
        }

        private static XDocument Open()
        {
            var path = ServerConfigPath;
            if (!File.Exists(path))
                throw new FileNotFoundException(
                    "Configuration file '" + Path.GetFileName(path) + "' was not found next to the server assembly. " +
                    "App.config is a desktop artifact: the build copies it as LegacyOrderDesk.exe.config beside the .exe on the user's PC. " +
                    "On the web server there is no exe and no per-user file — configuration moved to Web.config.", path);
            return XDocument.Load(path);
        }
    }
}
