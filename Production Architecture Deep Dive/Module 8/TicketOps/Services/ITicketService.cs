using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// Load and transition tickets. The screen and the presenter depend on this contract only; the
    /// registration profile decides whether <see cref="FakeTicketService"/> (in-memory) or
    /// <see cref="SqlTicketService"/> (production-shaped) stands behind it. Lifetime: Session — the
    /// ticket list is per user.
    /// </summary>
    public interface ITicketService
    {
        Task<IReadOnlyList<Ticket>> GetOpenTicketsAsync();

        /// <summary>Returns the ticket or null when it does not exist.</summary>
        Task<Ticket> FindAsync(int ticketId);

        /// <summary>Closes the ticket if the domain rule allows it. A rule saying no is a failed result, never an exception.</summary>
        Task<OperationResult<Ticket>> CloseAsync(int ticketId, string reason);

        Task<OperationResult<Ticket>> AssignAsync(int ticketId, int operatorId);
    }
}
