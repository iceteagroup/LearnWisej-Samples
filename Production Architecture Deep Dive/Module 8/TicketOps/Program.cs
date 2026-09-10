using System.Collections.Specialized;
using TicketOps.Infrastructure;
using TicketOps.Views;
using Wisej.Web;

namespace TicketOps
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup"). Runs once per browser session.
        ///
        /// Module 8: the hand-written AppComposition is gone. The startup decision is which registration
        /// profile Application.Services should hold — fake (default) or production (?profile=production on
        /// the URL). ServiceRegistration.Apply is idempotent because the registration table is shared by
        /// every session; the Session-lifetime instances it describes are still created per session.
        /// The Form is created with "new" and receives its services through [Inject] properties, not
        /// through its constructor.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            string requested = args?["profile"] ?? Application.QueryString?["profile"];
            var profile = ServiceRegistration.Apply(ServiceRegistration.ProfileFrom(requested), out bool changed);

            // The session log is itself a Session service: the first resolve creates this session's ActivityLog.
            var log = Application.Services.GetService<ILog>();
            log.Info(LogLayer.Session, "Program.Main",
                $"session {Application.SessionId} started · profile {profile.DisplayName} · {(changed ? "registered now" : "registration already in place (application-wide)")} · {Application.SessionCount} active session(s)");
            foreach (var entry in profile.Entries)
                log.Info(LogLayer.Infrastructure, "ServiceRegistration", $"{entry.ServiceName} → {entry.ImplementationName} · {entry.Lifetime}");

            new TicketWorkflow().Show();
        }
    }
}
