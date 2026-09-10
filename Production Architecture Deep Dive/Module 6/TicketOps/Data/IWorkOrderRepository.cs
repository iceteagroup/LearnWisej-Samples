using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Data
{
    /// <summary>
    /// Persistence contract for work orders and their approval trail. The service depends on this,
    /// never on a connection or a DbContext, so the store can be faked here and become SQL later
    /// without touching a screen or a rule.
    /// </summary>
    public interface IWorkOrderRepository
    {
        Task<IReadOnlyList<WorkOrder>> GetAllAsync();
        Task<WorkOrder> FindAsync(int id);
        Task<IReadOnlyList<ApprovalRecord>> GetAuditTrailAsync();

        /// <summary>
        /// Opens the transaction boundary of an approval: every write inside it is applied on
        /// <see cref="IWorkOrderTransaction.CommitAsync"/> or not at all. Disposing without a commit rolls back.
        /// </summary>
        IWorkOrderTransaction BeginTransaction();
    }

    /// <summary>
    /// One all-or-nothing unit of work. Writes are buffered; <see cref="CommitAsync"/> applies them together.
    /// A throw before the commit completes leaves the store exactly as it was.
    /// </summary>
    public interface IWorkOrderTransaction : IDisposable
    {
        void Update(WorkOrder workOrder);
        void RecordAudit(ApprovalRecord record);
        void QueueNotification(string recipient, string message);
        Task CommitAsync();
    }
}
