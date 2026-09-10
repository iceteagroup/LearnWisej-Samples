using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wisej.Web;

namespace OrderDesk.Migration
{
    /// <summary>One of the six files that make a Wisej.NET project shell, and the WinForms piece it replaces.</summary>
    public sealed class ShellFile
    {
        public string FileName { get; set; }
        public string Replaces { get; set; }     // the WinForms counterpart
        public string Role { get; set; }
    }

    /// <summary>
    /// The shell anatomy from the lesson, as data, plus a reader that shows the REAL file from the running
    /// project (Application.StartupPath is the project folder under dotnet run / F5 — the same files the
    /// console lists are the ones serving it).
    /// </summary>
    public static class ShellAnatomy
    {
        public const int PreviewLines = 14;

        public static readonly IReadOnlyList<ShellFile> Files = new List<ShellFile>
        {
            new ShellFile { FileName = "Default.html", Replaces = "the .exe window frame", Role = "Browser entry page: loads the Wisej.NET client (wisej.wx) into an empty body" },
            new ShellFile { FileName = "Default.json", Replaces = "Application.Run(new OrdersForm())", Role = "Names the startup method (\"startup\") or the first window (\"mainWindow\"), theme, debug" },
            new ShellFile { FileName = "Program.cs", Replaces = "Program.Main + EnableVisualStyles", Role = "Server-side session entry: Main(NameValueCollection) sets Application.MainPage — once per browser session" },
            new ShellFile { FileName = "Startup.cs", Replaces = "(nothing — the .exe owned the process)", Role = "ASP.NET Core host: CreateBuilder → UseWisej() → UseFileServer → Run(); Kestrel owns the process" },
            new ShellFile { FileName = "Web.config", Replaces = "App.config", Role = "appSettings + connectionStrings for the web host (read with XDocument, not ConfigurationManager)" },
            new ShellFile { FileName = "OrderDesk.Web.csproj", Replaces = "LegacyOrderDesk.csproj (UseWindowsForms)", Role = "Microsoft.NET.Sdk.Web, net10.0 + net10.0-windows, PackageReference Wisej-4 4.1.0" },
        };

        /// <summary>Absolute path of a shell file in the running project.</summary>
        public static string PathOf(string fileName) => Path.Combine(Application.StartupPath, fileName);

        /// <summary>The first <see cref="PreviewLines"/> lines of the real file, or the reason it could not be read.</summary>
        public static string Preview(string fileName, out int totalLines)
        {
            totalLines = 0;
            string path = PathOf(fileName);
            if (!File.Exists(path))
                return $"{path}\r\n(not found — Application.StartupPath is not the project folder in this host)";

            string[] lines = File.ReadAllLines(path);
            totalLines = lines.Length;
            var head = lines.Take(PreviewLines).Select(l => l.Replace("\t", "    "));
            string text = string.Join(Environment.NewLine, head);
            if (totalLines > PreviewLines)
                text += Environment.NewLine + $"… ({totalLines - PreviewLines} more lines)";
            return text;
        }
    }
}
