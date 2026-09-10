using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using OrderDesk.Services;

namespace OrderDesk.Diagnostics
{
    /// <summary>
    /// Application logging for the server. On the desktop a log next to the .exe belonged to one user; here
    /// one process serves every session, so each line carries the level and the message and the file lives
    /// under the storage root (App_Data/logs/orderdesk-yyyyMMdd.log) where the deployment mounts a volume.
    /// The same line goes to System.Diagnostics.Trace so a hosting trace listener (IIS, container stdout)
    /// sees it too.
    /// </summary>
    public static class AppLog
    {
        private static readonly object Gate = new object();

        public static string LogFolder => Path.Combine(AppConfig.StorageRoot, "logs");

        public static string CurrentFile => Path.Combine(LogFolder, "orderdesk-" + DateTime.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".log");

        /// <summary>Appends one line and returns the file it went to (or null when the file could not be written).</summary>
        public static string Write(string level, string message)
        {
            var line = $"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)}Z [{level}] {message}";
            Trace.WriteLine(line, "OrderDesk");
            try
            {
                lock (Gate)
                {
                    Directory.CreateDirectory(LogFolder);
                    File.AppendAllText(CurrentFile, line + Environment.NewLine);
                    return CurrentFile;
                }
            }
            catch (Exception)
            {
                // Logging must never take the request down; the Trace line already went out.
                return null;
            }
        }

        public static void Info(string message) => Write("INFO", message);

        public static void Warn(string message) => Write("WARN", message);

        public static void Error(string message, Exception ex) => Write("ERROR", message + (ex == null ? "" : " — " + ex.GetType().Name + ": " + ex.Message));
    }
}
