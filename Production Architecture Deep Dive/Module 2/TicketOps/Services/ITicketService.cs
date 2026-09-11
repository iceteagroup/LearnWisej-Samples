using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// The contract the screens call. Module 2's version is tenant-aware: which tickets you see comes from
    /// the injected <see cref="SessionContext"/>, not from a parameter the screen has to remember and not
    /// from a static.
    /// </summary>
    public interface ITicketService
    {
        /// <summary>Open tickets of this session's tenant.</summary>
        Task<IReadOnlyList<Ticket>> GetOpenTicketsAsync();
    }
}
