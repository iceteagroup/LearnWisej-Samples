using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wisej.Web;

namespace OrderDesk.Services
{
    /// <summary>One user's server-side profile — what LegacyOrderDesk kept in HKCU\Software\LegacyOrderDesk.</summary>
    public sealed class UserProfile
    {
        [JsonPropertyName("userName")] public string UserName { get; set; }
        [JsonPropertyName("lastCustomer")] public string LastCustomer { get; set; }
        [JsonPropertyName("lastFilter")] public string LastFilter { get; set; }
        [JsonPropertyName("theme")] public string Theme { get; set; } = "Bootstrap-4";
        [JsonPropertyName("savedUtc")] public DateTime SavedUtc { get; set; }
        [JsonPropertyName("savedFromSession")] public string SavedFromSession { get; set; }
    }

    /// <summary>
    /// ✓ The web-safe replacement for registry preferences: a per-user JSON file under
    /// App_Data/users/&lt;name&gt;.json, owned by the server and keyed by the signed-in user — not by
    /// the machine or the service account. A database row is the same idea with roaming and audit;
    /// device-only UI preferences may go to browser storage instead (docs/user-session-context.md).
    /// </summary>
    public sealed class UserSettingsStore
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions { WriteIndented = true };

        public string Root { get; }

        public UserSettingsStore()
        {
            // Application.StartupPath = the web app root; App_Data is the storage root Web.config names (Module 6 reads the key).
            Root = Path.Combine(Application.StartupPath, "App_Data", "users");
        }

        public string PathFor(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("A user name is required.", nameof(userName));
            foreach (var c in Path.GetInvalidFileNameChars()) userName = userName.Replace(c, '_');
            return Path.Combine(Root, userName.ToLowerInvariant() + ".json");
        }

        /// <summary>A path relative to the app root, for the trace (App_Data/users/kelly.json).</summary>
        public string RelativePath(string userName)
            => Path.GetRelativePath(Application.StartupPath, PathFor(userName)).Replace('\\', '/');

        public UserProfile Load(string userName)
        {
            var path = PathFor(userName);
            if (!File.Exists(path)) return null;
            return JsonSerializer.Deserialize<UserProfile>(File.ReadAllText(path), Options);
        }

        public string Save(UserProfile profile)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            Directory.CreateDirectory(Root);
            var path = PathFor(profile.UserName);
            profile.SavedUtc = DateTime.UtcNow;
            File.WriteAllText(path, JsonSerializer.Serialize(profile, Options));
            return path;
        }
    }
}
