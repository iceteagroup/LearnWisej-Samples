using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// The contract the screens call. Module 2's version is tenant-aware: which tickets you see and who a
    /// new ticket is stamped with come from the injected <see cref="SessionContext"/>, not from a parameter
    /// the screen has to remember and not from a static.
    /// </summary>
    public interface ITicketService
    {
        /// <summary>Open tickets of this session's tenant.</summary>
        Task<IReadOnlyList<Ticket>> GetOpenTicketsAsync();

        /// <summary>Creates a ticket stamped with this session's tenant and operator. Validation failures are results, not exceptions.</summary>
        Task<OperationResult<Ticket>> CreateForCurrentUserAsync(string title);
    }
}
