using Microsoft.Win32;

namespace LegacyOrderDesk.Settings
{
    /// <summary>
    /// User preferences in HKEY_CURRENT_USER\Software\LegacyOrderDesk. Fine for a desktop app;
    /// on a web server "current user" is the service account, so every visitor would share
    /// (and overwrite) the same keys. Module 4 moves these to a per-user profile store.
    /// </summary>
    public sealed class RegistrySettings
    {
        private const string KeyPath = @"Software\LegacyOrderDesk";

        public string GridDensity { get; set; } = "Comfortable";
        public string ExportFolder { get; set; } = @"C:\Orders";
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }

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
