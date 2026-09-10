using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// The contract the dashboard and the detail screen call. Decisions (which orders count where, whether a
    /// status may advance) live behind it, so both screens — and a test — get the same answers.
    /// </summary>
    public interface IWorkOrderService
    {
        Task<DashboardSnapshot> GetDashboardAsync();

        Task<WorkOrder> FindAsync(int id);

        /// <summary>Moves a work order to its next status if the domain rule allows it. A refusal is a result, not an exception.</summary>
        Task<OperationResult<WorkOrder>> AdvanceStatusAsync(int id);
    }
}
