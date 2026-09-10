namespace TicketOps.Diagnostics
{
    /// <summary>
    /// ⚠ The bug the static-state audit removes — kept in <c>Diagnostics/</c> as a <b>probe</b> so the two-tab
    /// test can show it happening. In the junior app this was <c>Workspace.CurrentUser</c>: a static field
    /// meant to hold "the current user". A static belongs to the class, and there is one class per server
    /// process, not one per session — so whichever tab wrote last wins, and every other tab reads that
    /// tab's user. No service in this sample reads it; it exists only to be displayed as
    /// "⚠ legacy static probe" on the Application panel, where its value visibly comes from the OTHER tab.
    ///
    /// It is at least thread-safe (a lock), which is the audit's minimum for anything shared — but being
    /// thread-safe does not make it correct: the value has the wrong <i>scope</i>. Delete it in production
    /// (see docs/ProductionReadinessNote.md).
    /// </summary>
    public static class StaticLeakProbe
    {
        private static readonly object _gate = new object();
        private static string _lastWriter = "(nothing written yet)";

        /// <summary>Whatever the last session to press the button wrote — regardless of who is reading.</summary>
        public static string LastWriter
        {
            get { lock (_gate) return _lastWriter; }
        }

        public static void Write(string user, string shortSessionId)
        {
            lock (_gate) _lastWriter = $"{user} (written by session {shortSessionId})";
        }
    }
}
