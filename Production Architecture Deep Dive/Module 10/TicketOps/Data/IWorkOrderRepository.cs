using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Data
{
    /// <summary>
    /// Persistence contract. The service depends on this, never on a driver, so the store can change
    /// (or be faked) without touching a screen or a rule.
    /// </summary>
    public interface IWorkOrderRepository
    {
        Task<IReadOnlyList<WorkOrder>> GetAllAsync();
        Task<WorkOrder> FindAsync(int id);
        Task<WorkOrder> UpsertAsync(WorkOrder order);
    }
}
