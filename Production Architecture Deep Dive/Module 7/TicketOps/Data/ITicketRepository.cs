using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Data
{
    /// <summary>
    /// Persistence contract. The services depend on this, never on SqlConnection or a DbContext, so the
    /// store can change (or be faked) without touching a screen or a rule.
    ///
    /// Implementations must be safe to call from two threads at once — the import task writes while the
    /// session's request thread reads (the "Refresh list" click).
    /// </summary>
    public interface ITicketRepository
    {
        Task<IReadOnlyList<Ticket>> GetAllAsync();

        Task<Ticket> FindAsync(int id);

        /// <summary>
        /// Adds a ticket with the id it carries. Returns false — atomically, under the store's lock — when
        /// that id already exists, so "duplicate id" is a per-row outcome and never a race.
        /// </summary>
        Task<bool> InsertAsync(Ticket ticket);

        Task<int> CountAsync();
    }
}
