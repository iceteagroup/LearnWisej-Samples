using System;
using System.Security.Principal;
using TicketOps.Security;
using Wisej.Web;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// The one place where the application's identity meets the Wisej.NET host. Services never touch
    /// <see cref="Application"/>; they read <see cref="IUserSession"/>. This adapter mirrors that session
    /// identity into <c>Application.User</c> (an <see cref="IPrincipal"/>) so the framework's own
    /// <c>Application.IsAuthenticated</c> agrees with the app, and logs both for the trace.
    ///
    /// Wisej.NET APIs used here (from the Wisej.Framework XML docs; verify at runtime):
    /// Application.User (get/set IPrincipal), Application.IsAuthenticated, Application.IsSecure, Application.SessionId.
    /// </summary>
    public static class WisejSessionBinding
    {
        public static void Bind(IUserSession session, ILog log)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (log == null) throw new ArgumentNullException(nameof(log));

            session.Changed += (s, e) => Mirror(session, log);
        }

        private static void Mirror(IUserSession session, ILog log)
        {
            try
            {
                Application.User = session.User as IPrincipal;      // UserContext implements IPrincipal; null on sign-out
                log.Info(LogLayer.Session, "WisejSessionBinding",
                    $"Application.User ← {(session.User == null ? "(anonymous)" : session.User.UserName)} · Application.IsAuthenticated = {Application.IsAuthenticated}");
            }
            catch (Exception ex)
            {
                // The app's own session identity is the source of truth; the mirror is diagnostics.
                log.Error(LogLayer.Session, "WisejSessionBinding", ex, "could not mirror the identity into Application.User");
            }
        }

        /// <summary>A short, non-secret handle for the trace ("session a9f3c2…") — never the full id.</summary>
        public static string ShortSessionId()
        {
            string id = Application.SessionId ?? string.Empty;
            return id.Length <= 6 ? id : id.Substring(0, 6) + "…";
        }

        public static string Describe()
        {
            return $"Application.IsAuthenticated = {Application.IsAuthenticated} · Application.User = {(Application.User?.Identity?.Name ?? "(anonymous)")} · Application.IsSecure = {Application.IsSecure} · session {ShortSessionId()}";
        }
    }
}
