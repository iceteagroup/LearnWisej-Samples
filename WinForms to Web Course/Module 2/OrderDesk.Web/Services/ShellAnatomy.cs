using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using Wisej.Web;

namespace OrderDesk.Services
{
    /// <summary>One file of the Wisej.NET project shell with the value it has right now.</summary>
    public sealed class ShellPart
    {
        public string File { get; set; }
        public string Role { get; set; }
        public string LiveValue { get; set; }
    }

    /// <summary>
    /// Reads the shell files of this very app at runtime so the "Shell anatomy" card shows live
    /// values instead of a diagram: Default.html, Default.json, Program.cs/Startup.cs, Web.config,
    /// the Wisej-4 package and the browser that replaced the desktop screen.
    /// </summary>
    public static class ShellAnatomy
    {
#if WINDOWS
        public const string TargetFramework = "net10.0-windows";
#else
        public const string TargetFramework = "net10.0";
#endif

        /// <summary>The web root: the folder that holds Default.json and Web.config.</summary>
        public static string Root => Path.GetDirectoryName(AppConfig.ResolvePath()) ?? Directory.GetCurrentDirectory();

        public static string WisejVersion => typeof(Application).Assembly.GetName().Version?.ToString(3) ?? "4.1.0";

        public static List<ShellPart> Read()
        {
            var parts = new List<ShellPart>
            {
                Guard("Default.html", "browser entry point", ReadDefaultHtml),
                Guard("Default.json", "startup settings · first window", ReadDefaultJson),
                Guard("Program.cs / Startup.cs", "host + Wisej middleware", ReadStartup),
                Guard("Web.config", "config from App.config", ReadWebConfig),
                Guard("Wisej-4 package", "replaces System.Windows.Forms", ReadPackage),
                Guard("Browser", "the screen is now a tab", ReadBrowser),
            };
            return parts;
        }

        private static ShellPart Guard(string file, string role, Func<string> read)
        {
            string value;
            try { value = read(); }
            catch (Exception ex) { value = "(" + ex.GetType().Name + ": " + ex.Message + ")"; }
            return new ShellPart { File = file, Role = role, LiveValue = value };
        }

        private static string ReadDefaultHtml()
        {
            var path = Path.Combine(Root, "Default.html");
            if (!File.Exists(path)) return "(not found at " + path + ")";
            var html = File.ReadAllText(path);
            var m = Regex.Match(html, "<script[^>]*src=\"([^\"]+)\"", RegexOptions.IgnoreCase);
            return (m.Success ? "<script src=\"" + m.Groups[1].Value + "\">" : "(no script tag)") + " · " + new FileInfo(path).Length + " bytes · empty <body>";
        }

        private static string ReadDefaultJson()
        {
            var path = Path.Combine(Root, "Default.json");
            if (!File.Exists(path)) return "(not found at " + path + ")";
            using (var doc = JsonDocument.Parse(File.ReadAllText(path), new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true }))
            {
                var root = doc.RootElement;
                string Get(string name) => root.TryGetProperty(name, out var p) ? p.ToString() : null;
                var startup = Get("startup");
                var mainWindow = Get("mainWindow");
                var theme = Get("theme");
                var url = Get("url");
                var first = startup != null ? "startup = " + startup : mainWindow != null ? "mainWindow = " + mainWindow : "(no startup/mainWindow)";
                return first + " · theme " + (theme ?? "?") + " · url " + (url ?? "?") + (startup != null && mainWindow == null ? " · (no mainWindow)" : "");
            }
        }

        private static string ReadStartup()
        {
            var sid = SafeSessionId();
            return "Program.Main(args) → Application.MainPage · session " + sid + " · Kestrel app.UseWisej() · " + RuntimeInformation.FrameworkDescription;
        }

        private static string ReadWebConfig()
        {
            var cfg = AppConfig.Load();
            var cs = cfg.Connection("OrderDesk");
            return "appSettings ×" + cfg.AppSettings.Count + " · connectionStrings ×" + cfg.ConnectionStrings.Count +
                   (cs != null ? " (" + cs.Name + " → " + cs.Server + ")" : "") +
                   " · OrderDesk.StorageRoot = " + cfg.StorageRoot;
        }

        private static string ReadPackage()
        {
            return "Wisej.Framework " + WisejVersion + " (Wisej-4 4.1.0) · " + TargetFramework + " · " + ShortOs();
        }

        private static string ReadBrowser()
        {
            var b = Application.Browser;
            if (b == null) return "(browser info not available yet)";
            var size = b.Size;
            return b.Type + " " + b.Version + " · " + b.OS + " · " + size.Width + "×" + size.Height + " · sessions " + Application.SessionCount;
        }

        private static string SafeSessionId()
        {
            try
            {
                var id = Application.SessionId ?? "";
                return id.Length > 8 ? id.Substring(0, 8) : id;
            }
            catch { return "?"; }
        }

        private static string ShortOs()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return "Windows";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return "Linux";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return "macOS";
            return RuntimeInformation.OSDescription;
        }
    }
}
