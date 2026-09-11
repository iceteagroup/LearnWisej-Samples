using System;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// The layer that produced a log entry.
    /// </summary>
    public enum LogLayer
    {
        UI,
        Service,
        Domain,
        Data,
        Infrastructure,
        Session,
        Client
    }

    public enum LogLevel
    {
        Info,
        Warn,
        Error
    }

    /// <summary>
    /// Cross-cutting logging contract. Every layer may depend on it; it depends on nothing.
    /// Error details go to the log only — the UI shows a safe message.
    /// </summary>
    public interface ILog
    {
        void Info(LogLayer layer, string source, string message);
        void Warn(LogLayer layer, string source, string message);
        void Error(LogLayer layer, string source, Exception exception, string message = null);
    }
}
