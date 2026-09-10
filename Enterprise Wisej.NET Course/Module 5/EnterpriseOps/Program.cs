using System.Collections.Specialized;
using EnterpriseOps.Services;
using Wisej.Web;

namespace EnterpriseOps
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup"). It creates the per-session
        /// context first — the grid state, the saved views and the activity trace live there, not in the page —
        /// and then shows the Enterprise Work Queue as the main page.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            var session = SessionContext.Current;
            session.Trace.Write($"Session: {session.UserName} ({session.Role}) @ {session.TenantId} — session {session.SessionId}");

            Application.MainPage = new UI.WorkQueuePage();
        }
    }
}
