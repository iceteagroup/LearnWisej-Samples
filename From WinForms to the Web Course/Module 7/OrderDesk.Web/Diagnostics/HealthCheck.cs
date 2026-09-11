using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using OrderDesk.Services;
using Wisej.Web;

namespace OrderDesk.Diagnostics
{
    /// <summary>One line of the health report.</summary>
    public sealed class HealthItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Ok { get; set; }

        public override string ToString() => $"{(Ok ? "✓" : "✕")} {Name,-18} {Value}";
    }

    /// <summary>
    /// What a load balancer or an operator needs to know about this instance. The /health endpoint in
    /// Startup.cs has no Wisej session, so it reports only process-level facts (Report(false)); called from
    /// inside a session, Report(true) adds sessions, theme and profile.
    /// Every item is read inside its own try/catch: a health check that throws is worse than one that says ✕.
    /// </summary>
    public static class HealthCheck
    {
        private static readonly DateTime StartedUtc = DateTime.UtcNow;

        public static TimeSpan Uptime => DateTime.UtcNow - StartedUtc;

        public static IList<HealthItem> Report(bool includeSession)
        {
            var items = new List<HealthItem>
            {
                Item("status", () => "healthy", true),
                Item("version", () => AppConfig.Version + " · Wisej-4 4.1.0 · " + Environment.Version, true),
                Item("uptime", () => Uptime.ToString(@"d\.hh\:mm\:ss", CultureInfo.InvariantCulture), true),
                Item("storage root", StorageRootStatus),
                Item("log folder", LogFolderStatus),
                Item("client profiles", ClientProfilesStatus),
                Item("theme mixin", MixinStatus),
            };

            if (includeSession)
            {
                items.Add(Item("sessions", () => Application.SessionCount.ToString(CultureInfo.InvariantCulture) + " live in this process", true));
                items.Add(Item("theme", () => ThemeService.CurrentName, true));
                items.Add(Item("profile", () => ResponsiveLayout.Describe(Application.ActiveProfile), true));
                items.Add(Item("web socket", () => Application.IsWebSocket ? "yes" : "no (long polling)", true));
            }
            return items;
        }

        /// <summary>The process-level report as a small JSON document for the /health endpoint.</summary>
        public static string Json()
        {
            var sb = new StringBuilder("{");
            bool first = true;
            bool allOk = true;
            foreach (var item in Report(includeSession: false))
            {
                if (!item.Ok) allOk = false;
                if (!first) sb.Append(',');
                first = false;
                sb.Append('"').Append(Escape(item.Name)).Append("\":{\"ok\":").Append(item.Ok ? "true" : "false")
                  .Append(",\"value\":\"").Append(Escape(item.Value)).Append("\"}");
            }
            sb.Append(",\"ok\":").Append(allOk ? "true" : "false").Append('}');
            return sb.ToString();
        }

        public static string Text(bool includeSession)
        {
            var sb = new StringBuilder();
            foreach (var item in Report(includeSession))
                sb.AppendLine(item.ToString());
            return sb.ToString().TrimEnd();
        }

        private static HealthItem Item(string name, Func<string> read, bool ok)
        {
            try { return new HealthItem { Name = name, Value = read(), Ok = ok }; }
            catch (Exception ex) { return new HealthItem { Name = name, Value = ex.GetType().Name + ": " + ex.Message, Ok = false }; }
        }

        private static HealthItem Item(string name, Func<HealthItem> read)
        {
            try { var item = read(); item.Name = name; return item; }
            catch (Exception ex) { return new HealthItem { Name = name, Value = ex.GetType().Name + ": " + ex.Message, Ok = false }; }
        }

        private static HealthItem StorageRootStatus()
        {
            var root = AppConfig.StorageRoot;
            Directory.CreateDirectory(root);
            var probe = Path.Combine(root, ".health-" + Guid.NewGuid().ToString("N") + ".tmp");
            File.WriteAllText(probe, "ok");
            File.Delete(probe);
            return new HealthItem { Value = root + " (writable)", Ok = true };
        }

        private static HealthItem LogFolderStatus()
        {
            var folder = AppLog.LogFolder;
            bool exists = Directory.Exists(folder);
            int files = exists ? Directory.GetFiles(folder, "orderdesk-*.log").Length : 0;
            return new HealthItem { Value = exists ? $"{folder} · {files} file(s)" : folder + " (created on first write)", Ok = true };
        }

        private static HealthItem ClientProfilesStatus()
        {
            var next = Path.Combine(AppContext.BaseDirectory, "ClientProfiles.json");
            var source = Path.Combine(AppConfig.BaseFolder, "ClientProfiles.json");
            bool ok = File.Exists(next) || File.Exists(source);
            return new HealthItem { Value = ok ? (File.Exists(next) ? next : source) : "ClientProfiles.json not found", Ok = ok };
        }

        private static HealthItem MixinStatus()
        {
            var name = AppConfig.ThemeMixin + ".mixin.theme";
            var next = Path.Combine(AppContext.BaseDirectory, "Themes", name);
            var source = Path.Combine(AppConfig.BaseFolder, "Themes", name);
            bool ok = File.Exists(next) || File.Exists(source);
            return new HealthItem { Value = ok ? "Themes/" + name : name + " not found", Ok = ok };
        }

        private static string Escape(string s) => (s ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
