using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace TicketOps.Infrastructure
{
    /// <summary>One entry of ClientProfiles.json as the app reads it back (name + browser width band).</summary>
    public sealed class ClientProfileDefinition
    {
        public string Name { get; init; }
        public int? MinWidth { get; init; }
        public int? MaxWidth { get; init; }

        /// <summary>"Phone ≤ 600", "Tablet 601–1024", "Desktop ≥ 1025".</summary>
        public string Describe()
        {
            if (MinWidth.HasValue && MaxWidth.HasValue) return $"{Name} {MinWidth}–{MaxWidth}";
            if (MaxWidth.HasValue) return $"{Name} ≤ {MaxWidth}";
            if (MinWidth.HasValue) return $"{Name} ≥ {MinWidth}";
            return Name;
        }
    }

    /// <summary>
    /// Reads the application's own ClientProfiles.json (the file Wisej.NET merges over its default profiles)
    /// so the screen can show which profiles exist and tell a known name from an unknown one. The framework
    /// decides the active profile; this class only reports what the file says. Infrastructure, not UI:
    /// no Wisej type is needed to read a JSON file next to the binaries.
    /// </summary>
    public sealed class ClientProfileCatalog
    {
        public const string FileName = "ClientProfiles.json";

        public IReadOnlyList<ClientProfileDefinition> Profiles { get; }
        public string SourcePath { get; }

        private ClientProfileCatalog(IReadOnlyList<ClientProfileDefinition> profiles, string sourcePath)
        {
            Profiles = profiles;
            SourcePath = sourcePath;
        }

        /// <summary>"Phone ≤ 600 · Tablet 601–1024 · Desktop ≥ 1025" (browser widths in CSS pixels).</summary>
        public string Summary => Profiles.Count == 0 ? "no custom profiles" : string.Join(" · ", Profiles.Select(p => p.Describe()));

        public bool Contains(string profileName)
            => Profiles.Any(p => string.Equals(p.Name, profileName, StringComparison.Ordinal));

        /// <summary>
        /// Looks for ClientProfiles.json where Wisej.NET looks for it (the /bin folder), then in the project
        /// folder. A missing file is a warning, not an error: the framework simply falls back to its defaults.
        /// </summary>
        public static ClientProfileCatalog Load(ILog log)
        {
            if (log == null) throw new ArgumentNullException(nameof(log));

            var candidates = new[]
            {
                Path.Combine(AppContext.BaseDirectory, FileName),
                Path.Combine(Directory.GetCurrentDirectory(), FileName)
            };

            string path = candidates.FirstOrDefault(File.Exists);
            if (path == null)
            {
                log.Warn(LogLayer.Infrastructure, "ClientProfileCatalog.Load", $"{FileName} not found next to the binaries — the framework uses its default profiles (Phone, Tablet, Small Desktop, Default)");
                return new ClientProfileCatalog(new List<ClientProfileDefinition>(), null);
            }

            try
            {
                var profiles = Parse(File.ReadAllText(path));
                var catalog = new ClientProfileCatalog(profiles, path);
                log.Info(LogLayer.Infrastructure, "ClientProfileCatalog.Load", $"{FileName}: {catalog.Summary} (browser width, matched top to bottom)");
                return catalog;
            }
            catch (Exception ex)
            {
                log.Error(LogLayer.Infrastructure, "ClientProfileCatalog.Load", ex, $"{FileName} could not be parsed — the framework uses its default profiles");
                return new ClientProfileCatalog(new List<ClientProfileDefinition>(), path);
            }
        }

        /// <summary>Accepts both shapes: {"profiles":[…]} (the framework's) and a bare [...] array.</summary>
        public static IReadOnlyList<ClientProfileDefinition> Parse(string json)
        {
            var options = new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true };
            using var document = JsonDocument.Parse(json, options);

            JsonElement array = document.RootElement;
            if (array.ValueKind == JsonValueKind.Object && array.TryGetProperty("profiles", out var inner))
                array = inner;

            var list = new List<ClientProfileDefinition>();
            if (array.ValueKind != JsonValueKind.Array)
                return list;

            foreach (var item in array.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object || !item.TryGetProperty("name", out var name))
                    continue;

                list.Add(new ClientProfileDefinition
                {
                    Name = name.GetString(),
                    MinWidth = ReadInt(item, "minWidth"),
                    MaxWidth = ReadInt(item, "maxWidth")
                });
            }
            return list;
        }

        private static int? ReadInt(JsonElement item, string property)
        {
            if (item.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out int n))
                return n;
            return null;
        }
    }
}
