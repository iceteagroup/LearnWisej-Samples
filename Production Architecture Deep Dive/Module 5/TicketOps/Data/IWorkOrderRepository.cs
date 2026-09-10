using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Data
{
    /// <summary>
    /// Persistence contract. The service depends on this, never on SqlConnection or a DbContext.
    /// Writes only happen inside a <see cref="IWorkOrderTransaction"/>, so the pipeline can stage the
    /// order and its audit entry and commit them as one unit — or roll both back.
    /// </summary>
    public interface IWorkOrderRepository
    {
        Task<IReadOnlyList<WorkOrder>> GetAllAsync();
        Task<WorkOrder> FindAsync(int id);
        IWorkOrderTransaction BeginTransaction();
    }

    /// <summary>
    /// A unit of work. Nothing staged here is visible to readers until <see cref="CommitAsync"/> returns;
    /// disposing without committing rolls everything back.
    /// </summary>
    public interface IWorkOrderTransaction : IDisposable
    {
        int Number { get; }
        /// <summary>Stages the write and returns the id the row will have.</summary>
        int Upsert(WorkOrder order);
        void Audit(int workOrderId, string action, string actor);
        /// <summary>The single moment "it happened". Throws if the store is unreachable — nothing is applied.</summary>
        Task<WorkOrder> CommitAsync();
    }
}
