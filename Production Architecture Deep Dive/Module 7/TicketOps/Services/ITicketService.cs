using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// What the screen asks about tickets while (and after) an import runs. Reads only in this module —
    /// the writes come from <see cref="IImportService"/>, through the same repository.
    /// </summary>
    public interface ITicketService
    {
        Task<IReadOnlyList<Ticket>> GetOpenTicketsAsync();

        Task<int> CountAsync();
    }
}
