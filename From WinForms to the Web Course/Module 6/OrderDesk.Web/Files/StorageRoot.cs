using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Wisej.Web;

namespace OrderDesk.Files
{
    /// <summary>
    /// ✓ The configured server storage root — the web replacement for every "C:\Orders\…" literal
    /// in LegacyOrderDesk. Web.config → appSettings["OrderDesk.StorageRoot"] (default "App_Data")
    /// is resolved under Application.StartupPath with Path.Combine, so the same code runs on
    /// Windows (App_Data\uploads) and Linux (App_Data/uploads). Sub-folders are created on demand
    /// and every file name that comes from a browser is sanitized before it touches the disk.
    /// </summary>
    public static class StorageRoot
    {
        private const string ConfigKey = "OrderDesk.StorageRoot";
        private const string DefaultRoot = "App_Data";

        private static readonly object Gate = new object();
        private static string _root;

        /// <summary>The configured value as written in Web.config (relative or absolute).</summary>
        public static string Configured { get; private set; } = DefaultRoot;

        /// <summary>
        /// The absolute storage root. Resolved once, on the first call, which happens inside a
        /// session (MainPage.Load) so Application.StartupPath is available; background workers
        /// then reuse the cached value.
        /// </summary>
        public static string Root
        {
            get
            {
                lock (Gate)
                {
                    if (_root == null)
                    {
                        Configured = ReadConfigured();
                        _root = Path.IsPathRooted(Configured)
                            ? Path.GetFullPath(Configured)
                            : Path.GetFullPath(Path.Combine(Application.StartupPath, Configured));   // ✓ Path.Combine, never "\\"
                        Directory.CreateDirectory(_root);
                    }
                    return _root;
                }
            }
        }

        /// <summary>Files that arrived from a browser (Upload). Never served back as-is.</summary>
        public static string Uploads => Ensure("uploads");

        /// <summary>Generated spreadsheets waiting for Application.Download.</summary>
        public static string Exports => Ensure("exports");

        /// <summary>Results of queued report jobs (PDF), read back by View / Download.</summary>
        public static string Reports => Ensure("reports");

        /// <summary>Full path of an uploaded file, with the browser-supplied name sanitized.</summary>
        public static string UploadPath(string fileName) => Path.Combine(Uploads, SafeFileName(fileName));

        public static string ExportPath(string fileName) => Path.Combine(Exports, SafeFileName(fileName));

        public static string ReportPath(string fileName) => Path.Combine(Reports, SafeFileName(fileName));

        /// <summary>
        /// Reduces a browser-supplied file name to a plain leaf name: strips any directory part
        /// (Path.GetFileName handles both separators), rejects "..", and replaces characters the
        /// host file system does not accept. The caller never gets a path that can leave the root.
        /// </summary>
        public static string SafeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("A file name is required.", nameof(fileName));

            // Browsers normally send a leaf name; a hostile client may not. Cut both separator styles.
            string leaf = fileName.Replace('\\', '/');
            leaf = Path.GetFileName(leaf);
            if (leaf.Length == 0 || leaf == "." || leaf == "..")
                throw new ArgumentException($"'{fileName}' is not a valid file name.", nameof(fileName));

            var invalid = Path.GetInvalidFileNameChars();
            var chars = leaf.Select(c => invalid.Contains(c) ? '_' : c).ToArray();
            return new string(chars);
        }

        /// <summary>A one-line description for the trace: where the root is and what it contains.</summary>
        public static string Describe()
        {
            string root = Root;
            int files = Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories).Count();
            return $"{root}  ({ConfigKey} = \"{Configured}\" · {files} file(s) · separator '{Path.DirectorySeparatorChar}')";
        }

        private static string Ensure(string subFolder)
        {
            string path = Path.Combine(Root, subFolder);
            Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Reads appSettings/OrderDesk.StorageRoot from Web.config with System.Xml.Linq — the web
        /// project does not reference System.Configuration (see the cookbook).
        /// </summary>
        private static string ReadConfigured()
        {
            try
            {
                string configPath = Path.Combine(Application.StartupPath, "Web.config");
                if (!File.Exists(configPath)) return DefaultRoot;

                var doc = XDocument.Load(configPath);
                var value = doc.Root?.Element("appSettings")?.Elements("add")
                    .FirstOrDefault(e => (string)e.Attribute("key") == ConfigKey)?.Attribute("value")?.Value;
                return string.IsNullOrWhiteSpace(value) ? DefaultRoot : value.Trim();
            }
            catch (Exception)
            {
                // A broken Web.config must not stop the app: fall back to App_Data next to the project.
                return DefaultRoot;
            }
        }
    }
}
