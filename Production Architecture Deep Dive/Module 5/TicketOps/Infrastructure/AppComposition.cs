using TicketOps.Data;
using TicketOps.Services;
using TicketOps.Validation;
using TicketOps.Views;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// The one place that knows how the session's object graph is wired ("who gets what").
    /// Program.Main creates one AppComposition per session, so nothing here is static: every browser
    /// session owns its own log, session context, repository, validator and service. Screens receive
    /// their dependencies through their constructors — they never new-up a service or read a global.
    ///
    /// Module 8 replaces this hand-written factory with Application.Services; the rule stays the same.
    /// </summary>
    public sealed class AppComposition
    {
        public ActivityLog Log { get; } = new ActivityLog();
        public SessionContext Session { get; }
        public InMemoryWorkOrderRepository Repository { get; }
        public WorkOrderValidator Validator { get; }
        public IWorkOrderService WorkOrders { get; }

        public AppComposition()
        {
            Log.Info(LogLayer.Infrastructure, "AppComposition", "composing the session object graph (nothing static)");
            Session = new SessionContext();
            Log.Info(LogLayer.Session, "SessionContext", $"actor {Session} — the role the server will check");
            Repository = new InMemoryWorkOrderRepository(Log);
            Validator = new WorkOrderValidator();
            WorkOrders = new WorkOrderService(Repository, Validator, Log);
            Log.Info(LogLayer.Infrastructure, "AppComposition", "IWorkOrderService → WorkOrderService(InMemoryWorkOrderRepository, WorkOrderValidator, ActivityLog)");
        }

        public WorkOrderEditor CreateMainView()
        {
            return new WorkOrderEditor(WorkOrders, Validator, Session, Repository, Log);
        }
    }
}
