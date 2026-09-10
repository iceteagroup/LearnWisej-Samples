using System.Collections.Concurrent;
using System.Threading;

namespace OrderDesk.Dialogs
{
    /// <summary>
    /// Counts EditOrderDialog instances that have been constructed and not yet disposed, so a
    /// dialog leak is a number on screen instead of a slow server. On the desktop nobody counted:
    /// the process ended with the user's day and took every forgotten dialog with it. On the
    /// server the session lives for hours and the process for weeks — the count only goes down
    /// when somebody calls Dispose.
    /// </summary>
    public static class DialogTracker
    {
        private static int _liveTotal;
        private static readonly ConcurrentDictionary<string, int> _liveBySession = new ConcurrentDictionary<string, int>();

        /// <summary>Live dialogs across every session in this process.</summary>
        public static int LiveTotal => Volatile.Read(ref _liveTotal);

        /// <summary>Live dialogs created by one browser session.</summary>
        public static int LiveFor(string sessionId)
        {
            return sessionId != null && _liveBySession.TryGetValue(sessionId, out var count) ? count : 0;
        }

        /// <summary>Called from the dialog constructor.</summary>
        public static void Opened(string sessionId)
        {
            Interlocked.Increment(ref _liveTotal);
            if (sessionId != null)
                _liveBySession.AddOrUpdate(sessionId, 1, (_, count) => count + 1);
        }

        /// <summary>Called once from the dialog's Dispose(bool).</summary>
        public static void Released(string sessionId)
        {
            Interlocked.Decrement(ref _liveTotal);
            if (sessionId != null)
                _liveBySession.AddOrUpdate(sessionId, 0, (_, count) => count > 0 ? count - 1 : 0);
        }
    }
}
