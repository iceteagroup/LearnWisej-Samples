using System;
using System.IO;
using OrderDesk.Services;
using Wisej.Web;

namespace OrderDesk.Security
{
    /// <summary>
    /// The only way a file leaves the server. On the desktop "open file" meant the user's own disk; on the
    /// web a file name arrives from the browser and names a path on a shared server. The guard resolves
    /// the name under the storage root, rejects rooted paths and traversal, checks the permission, audits,
    /// and only then calls Application.Download with the resolved path.
    /// </summary>
    public static class DownloadGuard
    {
        /// <summary>Resolves a relative name under the storage root or throws — never returns a path outside it.</summary>
        public static string Resolve(string requestedName)
        {
            if (string.IsNullOrWhiteSpace(requestedName))
                throw new ArgumentException("A file name is required.", nameof(requestedName));
            if (Path.IsPathRooted(requestedName))
                throw new UnauthorizedAccessException($"Rooted paths are not accepted: '{requestedName}'.");
            if (requestedName.IndexOf("..", StringComparison.Ordinal) >= 0)
                throw new UnauthorizedAccessException($"Path traversal rejected: '{requestedName}'.");

            string root = AppConfig.StorageRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            string full = Path.GetFullPath(Path.Combine(root, requestedName));
            if (!full.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException($"'{requestedName}' resolves outside the storage root.");
            return full;
        }

        /// <summary>
        /// Permission → resolve → exists → Application.Download(path). Every outcome is audited; the caller
        /// shows the exception text in the banner.
        /// </summary>
        public static string Download(string requestedName)
        {
            AuthService.Demand(AuthService.PermissionDownload);
            string full;
            try
            {
                full = Resolve(requestedName);
            }
            catch (Exception ex)
            {
                AuditLog.Record("download", $"REJECTED '{requestedName}' — {ex.Message}", allowed: false);
                throw;
            }
            if (!File.Exists(full))
            {
                AuditLog.Record("download", $"not found '{requestedName}'", allowed: false);
                throw new FileNotFoundException($"'{requestedName}' does not exist under the storage root.", full);
            }

            var fileName = Path.GetFileName(full);
            Application.Download(full, fileName);
            AuditLog.Record("download", $"{requestedName} ({new FileInfo(full).Length:N0} bytes)", allowed: true);
            return full;
        }

        /// <summary>Writes bytes under the storage root (creating the sub-folder) and returns the relative name.</summary>
        public static string Store(string relativeName, byte[] content)
        {
            var full = Resolve(relativeName);
            Directory.CreateDirectory(Path.GetDirectoryName(full));
            File.WriteAllBytes(full, content);
            return relativeName;
        }
    }
}
