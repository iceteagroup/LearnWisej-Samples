using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Data
{
    /// <summary>
    /// Persistence contract. It knows nothing about users or permissions: a repository that checked roles
    /// would be a second, inconsistent boundary. Authorization happens once, in the service, before any call here.
    /// </summary>
    public interface ITicketRepository
    {
        Task<IReadOnlyList<Ticket>> GetAllAsync();
        Task<Ticket> FindAsync(int id);
        Task<Ticket> UpsertAsync(Ticket ticket);
        Task<bool> DeleteAsync(int id);
    }
}
