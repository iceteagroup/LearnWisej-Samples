using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace OrderDesk.Services
{
    /// <summary>
    /// The one place that knows where files live on the server: the storage root from Web.config
    /// (`OrderDesk.StorageRoot` = App_Data) combined with the app folder. Every path is built with
    /// Path.Combine and created on demand — no C:\…, no backslashes, so the same code runs in a
    /// Linux container (case-sensitive names, forward slashes).
    /// </summary>
    public static class DocumentStorage
    {
        public const string ImportsFolder = "imports";
        public const string ReportsFolder = "reports";
        public const string SampleImportFile = "sample-import.csv";

        /// <summary>The resolved storage root, created if missing.</summary>
        public static string Root
        {
            get
            {
                var setting = AppConfig.StorageRootSetting
                    .Replace('\\', Path.DirectorySeparatorChar)
                    .Replace('/', Path.DirectorySeparatorChar);
                var root = Path.IsPathRooted(setting) ? setting : Path.Combine(AppConfig.AppRoot, setting);
                Directory.CreateDirectory(root);
                return root;
            }
        }

        public static string Imports => Ensure(Path.Combine(Root, ImportsFolder));
        public static string Reports => Ensure(Path.Combine(Root, ReportsFolder));

        /// <summary>A path relative to the app folder, always with forward slashes — what the trace shows.</summary>
        public static string Display(string fullPath)
        {
            var root = AppConfig.AppRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var p = fullPath;
            if (p.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                p = p.Substring(root.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            return p.Replace('\\', '/');
        }

        /// <summary>Uploaded names are untrusted: keep the file name only and strip anything odd.</summary>
        public static string SafeFileName(string uploadedName, string fallback = "upload.csv")
        {
            var name = Path.GetFileName((uploadedName ?? "").Replace('\\', '/'));
            var sb = new StringBuilder();
            foreach (var ch in name)
                sb.Append(char.IsLetterOrDigit(ch) || ch == '.' || ch == '-' || ch == '_' ? ch : '_');
            var clean = sb.ToString().Trim('.', '_');
            return string.IsNullOrEmpty(clean) ? fallback : clean;
        }

        public static string TimeStamp() => DateTime.Now.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture);

        /// <summary>Everything under the storage root, newest first, relative names.</summary>
        public static List<string> ListFiles()
        {
            var root = Root;
            return Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.LastWriteTimeUtc)
                .Select(f => Display(f.FullName) + "  (" + f.Length.ToString("N0", CultureInfo.InvariantCulture) + " bytes)")
                .ToList();
        }

        /// <summary>App_Data/sample-import.csv — the 5-row file the reviewer downloads and uploads back. Recreated if missing.</summary>
        public static string EnsureSampleImport()
        {
            var path = Path.Combine(Root, SampleImportFile);
            if (!File.Exists(path))
                File.WriteAllText(path, SampleImportCsv, new UTF8Encoding(false));
            return path;
        }

        /// <summary>
        /// Same columns LocalExport.ToCsv writes (Order,Customer,Owner,Total,Status,Date). Rows 2004
        /// (unknown customer) and 2005 (no owner) are meant to be rejected by the import validation.
        /// </summary>
        public const string SampleImportCsv =
            "Order,Customer,Owner,Total,Status,Date\r\n" +
            "2001,\"Northwind Traders\",\"Dana\",1250.00,Open,2026-09-10\r\n" +
            "2002,\"Contoso Ltd\",\"Priya\",980.50,Shipped,2026-09-09\r\n" +
            "2003,\"Fabrikam Inc\",\"Sam\",415.00,Open,2026-09-09\r\n" +
            "2004,\"Initech\",\"Kelly\",2200.00,Open,2026-09-08\r\n" +
            "2005,\"Globex Corp\",\"\",640.00,Invoiced,2026-09-08\r\n";

        private static string Ensure(string folder)
        {
            Directory.CreateDirectory(folder);
            return folder;
        }
    }
}
