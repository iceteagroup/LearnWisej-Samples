using System.Collections.Specialized;
using OrderDesk.Services;
using Wisej.Web;

namespace OrderDesk
{
    /// <summary>
    /// Wisej.NET session entry point (Default.json "startup": "OrderDesk.Program.Main, OrderDesk").
    /// This replaces the WinForms Program.Main with Application.EnableVisualStyles()/Application.Run(...):
    /// there is no process to run — the host (Startup.cs) owns the process, and this method
    /// runs once per browser session to show the first view.
    ///
    /// Module 7 adds the session-lifecycle hooks of the security review: every session is registered
    /// (the /health probe counts them), <see cref="Application.SessionTimeout"/> and
    /// <see cref="Application.ApplicationExit"/> are audit-logged, and the per-session
    /// <see cref="UserContext"/> is cleared when the session ends so no stale user context survives.
    /// </summary>
    internal static class Program
    {
        static void Main(NameValueCollection args)
        {
            string sessionId = Application.SessionId;
            SessionRegistry.Register(sessionId);

            // Fired before the client-side timeout expires. The built-in "prolong session?" dialog still
            // shows (Handled stays false); the app only leaves an audit trail — "session lifecycle" in the checklist.
            Application.SessionTimeout += (s, e) =>
            {
                AuditLog.Add(UserContext.Current.UserName, sessionId, "session about to time out (Application.SessionTimeout) — prolong dialog shown");
            };

            // Fired when the session ends (timeout, browser closed, Application.Exit): clear per-user state.
            Application.ApplicationExit += (s, e) =>
            {
                AuditLog.Add(UserContext.Current.UserName, sessionId, "session ended (Application.ApplicationExit) — UserContext cleared");
                UserContext.Clear();
                SessionRegistry.Unregister(sessionId);
            };

            Application.MainPage = new MainPage();
        }
    }
}
