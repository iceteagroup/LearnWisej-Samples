using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Data
{
    /// <summary>
    /// Persistence contract. Services depend on this, never on a connection or a DbContext,
    /// so the store can change (or be faked, as here) without touching a screen or a rule.
    /// </summary>
    public interface IWorkOrderRepository
    {
        Task<IReadOnlyList<WorkOrder>> GetAllAsync();
        Task<WorkOrder> FindAsync(int id);
    }
}
