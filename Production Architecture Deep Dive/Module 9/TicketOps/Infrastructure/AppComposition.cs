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
    /// Module 8 replaces this hand-written factory with Application.Services; the rule it enforces stays the same.
    /// </summary>
    public sealed class AppComposition
    {
        // Demo only. In production the key comes from configuration / a secret store — never from a script or Default.html.
        private const string DemoLinkSigningKey = "ticketops-demo-link-signing-key";

        public ActivityLog Log { get; } = new ActivityLog();
        public SessionContext Session { get; }
        public InMemoryWorkOrderRepository Repository { get; }
        public IWorkOrderService WorkOrders { get; }
        public IAuditLogService Audit { get; }
        public ITicketLinkService Links { get; }
        public BrowserApi Browser { get; }

        public AppComposition()
        {
            Log.Info(LogLayer.Infrastructure, "AppComposition", "composing the session object graph (nothing static)");
            Session = new SessionContext(new SessionUser("L. Romero", "Technician"));
            Log.Info(LogLayer.Session, "AppComposition", $"session user → {Session.User}");
            Repository = new InMemoryWorkOrderRepository(Log);
            WorkOrders = new WorkOrderService(Repository, Log);
            Audit = new AuditLogService(Log);
            Links = new TicketLinkService(Repository, Audit, DemoLinkSigningKey, Log);
            Browser = new BrowserApi(Log);
            Log.Info(LogLayer.Infrastructure, "AppComposition",
                "ITicketLinkService → TicketLinkService(InMemoryWorkOrderRepository, AuditLogService, signing key); BrowserApi owns every Eval");
        }

        public WorkOrdersView CreateMainView()
        {
            return new WorkOrdersView(WorkOrders, Links, Audit, Browser, Session, Repository, Log);
        }
    }
}
