using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace FieldOps.Services
{
    /// <summary>
    /// The compatibility rule between this server and the Hybrid clients in the field, per platform:
    /// the oldest client build the server still serves (MinimumClient) and the newest one shipped
    /// (LatestClient), plus where each platform's technicians get the update. Read from
    /// ClientPolicy.json on every check, so a release manager can raise a minimum without a deploy
    /// (the .json is never served as a static file - Startup.cs excludes .json).
    ///
    /// Raise MinimumClient only when the server starts depending on something an older client
    /// cannot do: an extension it never registered, a permission its manifest lacks, a newer
    /// Wisej-4-Hybrid-Native paired with this server's Wisej-4-Hybrid. New screens never need it.
    /// </summary>
    public sealed class ClientReleasePolicy
    {
        public sealed class PlatformRule
        {
            public string Minimum { get; set; }
            public string Latest { get; set; }
            public string StoreName { get; set; }
            public string StoreUrl { get; set; }
        }

        public string ServerVersion { get; set; }
        public string WhatsNew { get; set; }
        public Dictionary<string, PlatformRule> Platforms { get; set; } = new Dictionary<string, PlatformRule>(StringComparer.OrdinalIgnoreCase);

        public static ClientReleasePolicy Load()
        {
            // The content root first (where the release manager edits it), then the build output.
            var path = Path.Combine(Directory.GetCurrentDirectory(), "ClientPolicy.json");
            if (!File.Exists(path)) path = Path.Combine(AppContext.BaseDirectory, "ClientPolicy.json");
            var policy = JsonSerializer.Deserialize<ClientReleasePolicy>(File.ReadAllText(path),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true, ReadCommentHandling = JsonCommentHandling.Skip });
            policy.Platforms = new Dictionary<string, PlatformRule>(policy.Platforms, StringComparer.OrdinalIgnoreCase);
            return policy;
        }

        public PlatformRule Rule(string platform) =>
            Platforms.TryGetValue(platform, out var rule) ? rule : throw new InvalidOperationException($"No client policy for platform '{platform}'.");

        public Version MinimumClient(string platform) => Normalize(Version.Parse(Rule(platform).Minimum));

        public Version LatestClient(string platform) => Normalize(Version.Parse(Rule(platform).Latest));

        /// <summary>"2.6" and "2.6.0.0" are the same release: compare major.minor.build.</summary>
        public static Version Normalize(Version v) => new Version(v.Major, v.Minor, Math.Max(v.Build, 0));
    }
}
