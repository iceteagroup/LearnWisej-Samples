using Microsoft.Win32;

namespace OrderDesk.Legacy
{
    /// <summary>
    /// Copied as-is from LegacyOrderDesk/Settings/RegistrySettings.cs so the console can show
    /// what "HKCU" means once the code runs on a server: the registry of the service account on
    /// the server machine — one hive shared by every visitor, and no registry at all on Linux.
    /// The web replacements are Services/UserProfileStore.cs and Services/BrowserPreferences.cs.
    /// </summary>
    public sealed class RegistrySettings
    {
        private const string KeyPath = @"Software\LegacyOrderDesk";

        public string GridDensity { get; set; } = "Comfortable";
        public string ExportFolder { get; set; } = @"C:\Orders";
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }

        // ✕ Registry.CurrentUser on the server is the service account's hive, not the visitor's desktop
        public static RegistrySettings Load()
        {
            var settings = new RegistrySettings();
            using (var key = Registry.CurrentUser.OpenSubKey(KeyPath))
            {
                if (key == null) return settings;
                settings.GridDensity = key.GetValue("GridDensity", settings.GridDensity) as string ?? settings.GridDensity;
                settings.ExportFolder = key.GetValue("ExportFolder", settings.ExportFolder) as string ?? settings.ExportFolder;
                settings.WindowWidth = (int)key.GetValue("WindowWidth", 0);
                settings.WindowHeight = (int)key.GetValue("WindowHeight", 0);
            }
            return settings;
        }

        // ✕ every session writes the same keys — the last visitor's preferences win for everyone
        public static void Save(RegistrySettings settings)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(KeyPath))
            {
                key.SetValue("GridDensity", settings.GridDensity);
                key.SetValue("ExportFolder", settings.ExportFolder);
                key.SetValue("WindowWidth", settings.WindowWidth);
                key.SetValue("WindowHeight", settings.WindowHeight);
            }
        }
    }
}
