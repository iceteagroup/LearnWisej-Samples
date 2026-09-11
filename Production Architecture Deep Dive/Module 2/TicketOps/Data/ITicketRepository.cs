using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Data
{
    /// <summary>
    /// Persistence contract. Durable business data (tickets) belongs behind this interface — in production a
    /// database — never in session state: session state is the working set of one live console, a ticket
    /// must survive the console disconnecting.
    /// </summary>
    public interface ITicketRepository
    {
        Task<IReadOnlyList<Ticket>> GetByTenantAsync(string tenant);
    }
}
