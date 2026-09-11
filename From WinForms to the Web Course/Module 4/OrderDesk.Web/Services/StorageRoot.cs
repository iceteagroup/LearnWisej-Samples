using System;
using System.IO;
using System.Xml.Linq;
using Wisej.Web;

namespace OrderDesk.Services
{
    /// <summary>
    /// The application's storage root: Web.config &lt;appSettings&gt; "OrderDesk.StorageRoot" (default
    /// "App_Data"), resolved under Application.StartupPath — the project folder under dotnet run. The one
    /// place that knows where the per-session temp folders (tmp/) live. Read once and cached under a lock.
    /// System.Configuration.ConfigurationManager is not referenced, so Web.config is read with System.Xml.Linq.
    /// </summary>
    public static class StorageRoot
    {
        public const string SettingKey = "OrderDesk.StorageRoot";
        public const string DefaultFolder = "App_Data";

        private static readonly object Gate = new object();
        private static string _folder;
        private static string _root;

        /// <summary>The folder name as configured in Web.config (or the default when the key is missing or empty).</summary>
        public static string ConfiguredFolder
        {
            get { lock (Gate) { EnsureResolved(); return _folder; } }
        }

        /// <summary>The absolute storage root, e.g. &lt;project&gt;\App_Data.</summary>
        public static string Root
        {
            get { lock (Gate) { EnsureResolved(); return _root; } }
        }

        /// <summary>A path under the storage root: Combine("profiles"), Combine("tmp", sessionId), Combine("exports", user) …</summary>
        public static string Combine(params string[] parts) => Path.Combine(Root, Path.Combine(parts));

        private static void EnsureResolved()
        {
            if (_root != null) return;
            string startup = Application.StartupPath ?? AppContext.BaseDirectory;
            _folder = ReadSetting(startup) ?? DefaultFolder;
            _root = Path.IsPathRooted(_folder) ? _folder : Path.GetFullPath(Path.Combine(startup, _folder));
        }

        private static string ReadSetting(string startup)
        {
            try
            {
                string config = Path.Combine(startup, "Web.config");
                if (!File.Exists(config)) return null;
                foreach (var add in XDocument.Load(config).Descendants("appSettings").Elements("add"))
                {
                    if ((string)add.Attribute("key") != SettingKey) continue;
                    string value = ((string)add.Attribute("value") ?? "").Trim();
                    return value.Length == 0 ? null : value;
                }
                return null;
            }
            catch (Exception)
            {
                return null;   // a malformed Web.config falls back to App_Data; nothing downstream needs to know
            }
        }
    }
}
