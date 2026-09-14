using System;
using Wisej.Web;

namespace WisejPerfLab.Diagnostics
{
    /// <summary>
    /// Finds the current session PERF buffer. <c>Application.Session</c> is a per-session bag, so the
    /// probe can be a stateless singleton in Microsoft DI and still write into the right timeline —
    /// including from inside <c>Application.StartTask</c>, which keeps the session context.
    /// </summary>
    public static class SessionPerfLog
    {
        private const string Key = "PerfLog";

        /// <summary>
        /// The buffer of the current session, or null when there is none (the host thread that seeds the
        /// database at start, for example, runs outside any session).
        /// </summary>
        public static PerfLogBuffer Current
        {
            get
            {
                try
                {
                    var existing = Application.Session[Key] as PerfLogBuffer;
                    if (existing != null)
                        return existing;

                    var created = new PerfLogBuffer();
                    Application.Session[Key] = created;
                    return created;
                }
                catch (Exception)
                {
                    // No session on this thread: the record still reaches the console logger.
                    return null;
                }
            }
        }
    }
}
