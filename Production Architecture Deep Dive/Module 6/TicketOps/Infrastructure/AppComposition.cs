using TicketOps.Data;
using TicketOps.Services;
using TicketOps.Views;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// The one place that knows how the session's object graph is wired ("who gets what").
    /// Program.Main creates one AppComposition per session, so nothing here is static: every browser
    /// session owns its own log, repository and approval service. Screens receive their dependencies
    /// through their constructors — they never new-up a service and never read one from a global.
    ///
    /// Module 8 replaces this hand-written factory with Application.Services (dependency injection);
    /// the rule it enforces stays the same.
    /// </summary>
    public sealed class AppComposition
    {
        /// <summary>
        /// The identity the service records as "decided by". Module 11 takes it from the authenticated
        /// principal; here it is the session's demo approver. It is passed to the service on the trusted
        /// side — the dialog never supplies it.
        /// </summary>
        public const string CurrentUser = "approver@ticketops";

        public ActivityLog Log { get; } = new ActivityLog();
        public InMemoryWorkOrderRepository Repository { get; }
        public IApprovalService Approvals { get; }

        public AppComposition()
        {
            Log.Info(LogLayer.Infrastructure, "AppComposition", "composing the session object graph (nothing static)");
            Repository = new InMemoryWorkOrderRepository(Log);
            Approvals = new ApprovalService(Repository, Log, CurrentUser);
            Log.Info(LogLayer.Infrastructure, "AppComposition", $"IApprovalService → ApprovalService(InMemoryWorkOrderRepository, ActivityLog, \"{CurrentUser}\")");
        }

        public WorkOrderQueue CreateMainView()
        {
            return new WorkOrderQueue(Approvals, Repository, Log);
        }
    }
}
