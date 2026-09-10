using System;
using Wisej.Web;

namespace OrderDesk.Services
{
    /// <summary>
    /// The cleanup hooks Program.Main wires per session: ApplicationExit (browser closed, session
    /// expired, Application.Exit) and SessionTimeout (about to expire). Everything a user left
    /// behind — the UserContext, locks, temp files, report jobs — is released here; the Sign out
    /// button runs the same UserContext.Clear() on demand.
    /// </summary>
    public static class SessionLifecycle
    {
        public static void OnApplicationExit(MainPage page)
        {
            var id = ShortSessionId();
            UserContext.Clear();
            Console.WriteLine("[OrderDesk] ApplicationExit · session " + id + " · UserContext cleared, per-user state released");
            if (page != null && !page.IsDisposed)
                page.TraceLifecycle("Application.ApplicationExit", "session " + id + " ending → UserContext.Clear()");
        }

        public static void OnSessionTimeout(MainPage page)
        {
            var id = ShortSessionId();
            Console.WriteLine("[OrderDesk] SessionTimeout · session " + id + " about to expire (built-in prolong dialog shown)");
            if (page != null && !page.IsDisposed)
                page.TraceLifecycle("Application.SessionTimeout", "session " + id + " about to expire · e.Handled = false → Wisej shows its prolong dialog");
        }

        public static string ShortSessionId()
        {
            var id = Application.SessionId ?? "";
            return id.Length > 8 ? id.Substring(0, 8) : id;
        }
    }
}
