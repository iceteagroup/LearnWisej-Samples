using System;
using System.IO;

namespace FieldOps.Services
{
    /// <summary>
    /// The server log. The technician gets a sentence; the detail - exception type, message,
    /// order, session - goes here: the console of the FieldOps server process, and, for the Local
    /// app inside the Hybrid client (no console there), a file on the device.
    /// </summary>
    public static class OpsLog
    {
        private static readonly object _gate = new object();

        /// <summary>Set by the Local app: also append every line to this file.</summary>
        public static string FilePath { get; set; }

        public static void Info(string message) => Write(message);

        public static void Error(string message, Exception ex) =>
            Write($"ERROR {message}: {ex.GetType().Name}: {ex.Message}");

        private static void Write(string message)
        {
            var line = $"[FieldOps {DateTime.Now:HH:mm:ss}] {message}";
            Console.WriteLine(line);
            if (FilePath != null)
            {
                try { lock (_gate) File.AppendAllText(FilePath, line + Environment.NewLine); }
                catch (IOException) { /* logging must never break the screen */ }
            }
        }
    }
}
