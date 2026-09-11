using TicketOps.Data;
using TicketOps.Security;
using TicketOps.Services;
using TicketOps.Views;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// The one place that knows how the session's object graph is wired ("who gets what").
    /// Program.Main creates one AppComposition per session, so nothing here is static: every browser
    /// session owns its own log, user session, repositories, audit trail and services. Screens receive
    /// their dependencies through their constructors — they never new-up a service and never read one
    /// from a global.
    ///
    /// Module 11: the main view is the LOGIN GATE. The Work Orders screen is created only after
    /// IAuthenticationService bound an identity to the session, and every service it calls re-checks
    /// that identity through IPermissionService.
    /// </summary>
    public sealed class AppComposition
    {
        public ActivityLog Log { get; } = new ActivityLog();
        public UserSession Session { get; }
        public IPermissionService Permissions { get; }
        public IAuditService Audit { get; }
        public IAuthenticationService Authentication { get; }
        public ITicketService Tickets { get; }

        public AppComposition()
        {
            Session = new UserSession();
            Audit = new AuditLog(Log);
            Permissions = new PermissionService(Log);
            Authentication = new AuthenticationService(new InMemoryUserStore(), Session, Audit, Log);
            Tickets = new TicketService(new InMemoryTicketRepository(), Session, Permissions, Audit, Log);

            // Infrastructure mirrors the session identity into the Wisej.NET host (Application.User).
            WisejSessionBinding.Bind(Session, Log);
        }

        /// <summary>The first screen of the session is the login gate.</summary>
        public LoginView CreateMainView() => CreateLoginView();

        public LoginView CreateLoginView()
        {
            return new LoginView(Authentication, Log, CreateWorkOrdersView);
        }

        public WorkOrdersView CreateWorkOrdersView()
        {
            return new WorkOrdersView(Tickets, Permissions, Audit, Session, Authentication, Log, CreateLoginView);
        }
    }
}
