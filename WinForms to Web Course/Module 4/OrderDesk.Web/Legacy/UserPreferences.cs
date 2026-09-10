using Microsoft.Win32;

namespace OrderDesk.Legacy
{
    /// <summary>
    /// Copied from LegacyOrderDesk/Legacy/UserPreferences.cs for the Module 4 demonstration.
    /// ✕ Desktop assumption: user preferences in HKEY_CURRENT_USER. On a web server this reads the
    /// SERVER's registry under the service account — the wrong machine and the wrong user. On Linux
    /// Microsoft.Win32.Registry throws PlatformNotSupportedException (the caller catches it).
    /// </summary>
    public static class UserPreferences
    {
        public const string KeyPath = @"Software\LegacyOrderDesk";

        public static string Get(string name, string defaultValue)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(KeyPath))
                return key?.GetValue(name) as string ?? defaultValue;
        }

        public static void Set(string name, string value)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(KeyPath))
                key.SetValue(name, value ?? "");
        }
    }
}
