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
    /// </summary>
    public sealed class AppComposition
    {
        /// <summary>
        /// The identity the service records as "decided by". It is passed to the service on the trusted
        /// side — the dialog never supplies it.
        /// </summary>
        public const string CurrentUser = "approver@ticketops";

        public ActivityLog Log { get; } = new ActivityLog();
        public IApprovalService Approvals { get; }

        public AppComposition()
        {
            Approvals = new ApprovalService(new InMemoryWorkOrderRepository(), Log, CurrentUser);
        }

        public WorkOrderQueue CreateMainView()
        {
            return new WorkOrderQueue(Approvals, Log);
        }
    }
}
