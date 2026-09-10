using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// What the Work Orders screen asks for. The global search box hands its text to
    /// <see cref="SearchAsync"/>; the service bounds and normalizes it (the text is client input).
    /// </summary>
    public interface IWorkOrderService
    {
        /// <summary>Returns the work orders matching <paramref name="query"/> (all of them when it is empty).</summary>
        Task<IReadOnlyList<WorkOrder>> SearchAsync(string query);
    }
}
