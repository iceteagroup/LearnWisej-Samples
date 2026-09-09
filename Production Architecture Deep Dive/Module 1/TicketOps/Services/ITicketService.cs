using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// The contract the screens call. Everything a ticket "means" (what is valid, when it may close,
    /// how it is persisted) lives behind this interface, so a second screen reuses the same decisions
    /// and a test can exercise them with a fake repository and no browser.
    /// </summary>
    public interface ITicketService
    {
        Task<IReadOnlyList<Ticket>> GetOpenTicketsAsync();

        /// <summary>Creates or updates a ticket. Validation failures come back as a failed result, never as an exception.</summary>
        Task<OperationResult<Ticket>> SaveAsync(TicketDraft draft);

        /// <summary>Closes a ticket if the domain rule allows it ("log hours before closing").</summary>
        Task<OperationResult<Ticket>> CloseAsync(int ticketId);
    }
}
