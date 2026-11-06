using System;

namespace FieldOps.Services
{
    /// <summary>
    /// The server log. The technician gets a sentence; the detail - exception type, message,
    /// order, session - goes here, to the console of the FieldOps server process.
    /// </summary>
    public static class OpsLog
    {
        public static void Info(string message) =>
            Console.WriteLine($"[FieldOps {DateTime.Now:HH:mm:ss}] {message}");

        public static void Error(string message, Exception ex) =>
            Console.WriteLine($"[FieldOps {DateTime.Now:HH:mm:ss}] ERROR {message}: {ex.GetType().Name}: {ex.Message}");
    }
}
