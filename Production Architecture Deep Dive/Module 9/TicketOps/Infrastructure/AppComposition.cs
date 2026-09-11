using TicketOps.Data;
using TicketOps.Domain;
using TicketOps.Services;
using TicketOps.Views;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// The one place that knows how the session's object graph is wired ("who gets what").
    /// Program.Main creates one AppComposition per session, so nothing here is static: every browser
    /// session owns its own log, user, repository, services and browser helper. Screens receive their
    /// dependencies through their constructors — they never new-up a service and never read one from a global.
    ///
    /// Module 9 additions: the signed-link service (with a demo signing key that never reaches the browser),
    /// the audit log, and BrowserApi — the single place that talks to browser-only APIs.
    /// </summary>
    public sealed class AppComposition
    {
        // Demo only. In production the key comes from configuration / a secret store — never from a script or Default.html.
        private const string DemoLinkSigningKey = "ticketops-demo-link-signing-key";

        public ActivityLog Log { get; } = new ActivityLog();
        public SessionContext Session { get; }
        public IWorkOrderService WorkOrders { get; }
        public IAuditLogService Audit { get; }
        public ITicketLinkService Links { get; }
        public BrowserApi Browser { get; }

        public AppComposition()
        {
            Session = new SessionContext(new SessionUser("L. Romero", "Technician"));
            var repository = new InMemoryWorkOrderRepository();
            WorkOrders = new WorkOrderService(repository);
            Audit = new AuditLogService();
            Links = new TicketLinkService(repository, Audit, DemoLinkSigningKey, Log);
            Browser = new BrowserApi(Log);
        }

        public WorkOrdersView CreateMainView()
        {
            return new WorkOrdersView(WorkOrders, Links, Audit, Browser, Session, Log);
        }
    }
}
