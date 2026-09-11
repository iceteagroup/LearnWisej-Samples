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
    /// </summary>
    public sealed class AppComposition
    {
        public ActivityLog Log { get; } = new ActivityLog();
        public SessionContext Session { get; }
        public WorkOrderValidator Validator { get; }
        public IWorkOrderService WorkOrders { get; }

        public AppComposition()
        {
            Session = new SessionContext();
            Validator = new WorkOrderValidator();
            WorkOrders = new WorkOrderService(new InMemoryWorkOrderRepository(Log), Validator, Log);
        }

        public WorkOrderEditor CreateMainView()
        {
            return new WorkOrderEditor(WorkOrders, Validator, Session, Log);
        }
    }
}
