using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Data
{
    /// <summary>
    /// Persistence contract. The service depends on this, never on SqlConnection or a DbContext,
    /// so the database can change (or be faked) without touching a screen or a rule.
    /// </summary>
    public interface ITicketRepository
    {
        Task<IReadOnlyList<Ticket>> GetAllAsync();
        Task<Ticket> FindAsync(int id);
        Task<Ticket> UpsertAsync(Ticket ticket);

        /// <summary>The activity feed, newest first.</summary>
        Task<IReadOnlyList<TicketEvent>> GetEventsAsync();
        Task<TicketEvent> AppendEventAsync(TicketEvent ticketEvent);
    }
}
