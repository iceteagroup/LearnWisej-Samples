using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Data
{
    /// <summary>
    /// Persistence contract for work orders. The service depends on this, never on a SqlConnection or a
    /// DbContext, so the store can change (or be faked) without touching the screen, the binding or a rule.
    /// Implementations hand out copies: the live, bound objects belong to the screen's BindingList.
    /// </summary>
    public interface IWorkOrderRepository
    {
        Task<IReadOnlyList<WorkOrder>> GetAllAsync();
        Task<WorkOrder> FindAsync(int id);
        Task<WorkOrder> UpsertAsync(WorkOrder order);
    }
}
