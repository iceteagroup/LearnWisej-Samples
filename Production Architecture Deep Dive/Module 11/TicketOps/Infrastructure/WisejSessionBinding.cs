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
    /// <c>Application.IsAuthenticated</c> agrees with the app.
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
            }
            catch (Exception ex)
            {
                // The app's own session identity is the source of truth; the mirror is secondary.
                log.Error(LogLayer.Session, "WisejSessionBinding", ex, "could not mirror the identity into Application.User");
            }
        }
    }
}
