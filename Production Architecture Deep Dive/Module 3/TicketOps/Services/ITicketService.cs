using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// The contract the Ticket Workspace calls. Everything a ticket "means" (what is valid, when it may
    /// close, what a filter chip selects, how it is persisted) lives behind this interface, so the same
    /// decisions serve the desktop, tablet and phone arrangements of the screen — and a unit test.
    /// </summary>
    public interface ITicketService
    {
        Task<IReadOnlyList<Ticket>> GetOpenTicketsAsync();

        /// <summary>Free text + one chip. A query that is too short comes back as a failed result, never as an exception.</summary>
        Task<OperationResult<IReadOnlyList<Ticket>>> SearchAsync(TicketFilter filter);

        /// <summary>Creates or updates a ticket. Validation failures come back as a failed result, never as an exception.</summary>
        Task<OperationResult<Ticket>> SaveAsync(TicketDraft draft);

        /// <summary>Closes a ticket if the domain rule allows it ("log hours before closing").</summary>
        Task<OperationResult<Ticket>> CloseAsync(int ticketId);

        /// <summary>The activity feed, newest first, optionally filtered by text (same 2-character rule as tickets).</summary>
        Task<OperationResult<IReadOnlyList<TicketEvent>>> GetActivityAsync(string filterText);
    }
}
