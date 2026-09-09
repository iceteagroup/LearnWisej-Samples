using System;
using System.IO;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// Finds the project's <c>docs/</c> folder — the capstone package the Capstone Review screen reads
    /// and self-checks. With <c>dotnet run</c> from the project folder, <c>Application.StartupPath</c> is
    /// the project folder; the fallback walks up from the binary for the F5 / published cases.
    /// </summary>
    public sealed class DocsFolder
    {
        public DocsFolder(string startupPath)
        {
            Root = Locate(startupPath);
        }

        /// <summary>Absolute path of the docs folder, or null when it could not be found (a visible failure, not a crash).</summary>
        public string Root { get; }

        public bool Found => Root != null;

        public string PathOf(string relativeDocPath)
        {
            if (Root == null) return null;
            string trimmed = relativeDocPath.Replace('/', Path.DirectorySeparatorChar);
            if (trimmed.StartsWith("docs" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                trimmed = trimmed.Substring(5);
            return Path.Combine(Root, trimmed);
        }

        public bool Exists(string relativeDocPath)
        {
            string full = PathOf(relativeDocPath);
            return full != null && File.Exists(full);
        }

        public string ReadAllText(string relativeDocPath)
        {
            string full = PathOf(relativeDocPath);
            return full != null && File.Exists(full) ? File.ReadAllText(full) : null;
        }

        public long SizeOf(string relativeDocPath)
        {
            string full = PathOf(relativeDocPath);
            return full != null && File.Exists(full) ? new FileInfo(full).Length : 0;
        }

        private static string Locate(string startupPath)
        {
            var candidates = new[] { startupPath, AppContext.BaseDirectory, Directory.GetCurrentDirectory() };
            foreach (var start in candidates)
            {
                if (string.IsNullOrEmpty(start)) continue;
                var dir = new DirectoryInfo(start);
                for (int depth = 0; dir != null && depth < 6; depth++, dir = dir.Parent)
                {
                    string probe = Path.Combine(dir.FullName, "docs", "index.json");
                    if (File.Exists(probe))
                        return Path.Combine(dir.FullName, "docs");
                }
            }
            return null;
        }
    }
}
