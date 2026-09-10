using System;
using System.IO;
using System.Text.Json;

namespace OrderDesk.Services
{
    /// <summary>The settings LegacyOrderDesk kept in HKCU, now owned by the server per user.</summary>
    public sealed class UserProfile
    {
        /// <summary>
        /// Grid density (Comfortable · Compact · Dense). The browser's localStorage is the primary store
        /// (a device-bound UI preference); this is the roaming copy, written by the Profile store button and
        /// by the Browser storage button while a user is signed in.
        /// </summary>
        public string GridDensity { get; set; } = "Comfortable";

        /// <summary>
        /// Was "C:\Orders" — a path on the user's PC. On the server it is a folder name under the
        /// application's storage root (&lt;StorageRoot&gt;/&lt;ExportFolder&gt;/&lt;user&gt;); exports are then
        /// streamed to the browser with Application.Download (Module 6), never written to a client path.
        /// </summary>
        public string ExportFolder { get; set; } = "exports";

        public DateTime? SavedOn { get; set; }
    }

    /// <summary>
    /// ✓ Server-side, per-user profile files: &lt;StorageRoot&gt;/profiles/&lt;user&gt;.json written with
    /// System.Text.Json. This is the "server user-profile store" destination of the lesson —
    /// settings that must follow the user to any device but do not need auditing (those go to a
    /// database table). The registry replacement for everything that is not a pure UI preference.
    /// </summary>
    public static class UserProfileStore
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions { WriteIndented = true };

        /// <summary>&lt;StorageRoot&gt;/profiles — see Services/StorageRoot.cs (Web.config OrderDesk.StorageRoot, default App_Data).</summary>
        public static string Root => StorageRoot.Combine("profiles");

        public static string PathFor(string userName) => Path.Combine(Root, SafeFileName(userName) + ".json");

        /// <summary>The server folder this user's exports are staged in — the replacement for C:\Orders.</summary>
        public static string ExportRootFor(string userName, UserProfile profile) =>
            StorageRoot.Combine(profile.ExportFolder ?? "exports", SafeFileName(userName));

        public static UserProfile Load(string userName)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentException("A profile belongs to a signed-in user.", nameof(userName));
            string path = PathFor(userName);
            if (!File.Exists(path)) return new UserProfile();
            return JsonSerializer.Deserialize<UserProfile>(File.ReadAllText(path), Options) ?? new UserProfile();
        }

        /// <summary>Writes the profile and returns the file path (shown in the console).</summary>
        public static string Save(string userName, UserProfile profile)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentException("A profile belongs to a signed-in user.", nameof(userName));
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            Directory.CreateDirectory(Root);
            profile.SavedOn = DateTime.Now;
            string path = PathFor(userName);
            File.WriteAllText(path, JsonSerializer.Serialize(profile, Options));
            return path;
        }

        private static string SafeFileName(string userName)
        {
            var chars = userName.ToLowerInvariant().ToCharArray();
            var invalid = Path.GetInvalidFileNameChars();
            for (int i = 0; i < chars.Length; i++)
                if (Array.IndexOf(invalid, chars[i]) >= 0) chars[i] = '_';
            return new string(chars);
        }
    }
}
