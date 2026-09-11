using System;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// Session-scoped implementation of <see cref="ILog"/> that writes to the server console.
    /// Plain C#: no Wisej.NET dependency, so services can be exercised without a control.
    /// Error details (exception type, message, stack) are written here and never reach the screen.
    /// </summary>
    public sealed class ActivityLog : ILog
    {
        public void Info(LogLayer layer, string source, string message) => Write(LogLevel.Info, layer, source, message, null);
        public void Warn(LogLayer layer, string source, string message) => Write(LogLevel.Warn, layer, source, message, null);

        public void Error(LogLayer layer, string source, Exception exception, string message = null)
            => Write(LogLevel.Error, layer, source, message ?? exception?.Message, exception);

        private static void Write(LogLevel level, LogLayer layer, string source, string message, Exception exception)
        {
            var line = $"{DateTime.Now:HH:mm:ss.fff} {level.ToString().ToUpperInvariant()} [{layer}] {source}: {message}";
            if (exception != null)
                line += Environment.NewLine + exception;
            Console.WriteLine(line);
        }
    }
}
