using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// The contract the screens call. Every method authorizes the CURRENT SESSION USER itself, before it
    /// touches the repository: hiding a button is a courtesy, this interface is the boundary.
    /// Expected outcomes (a rule says no) are failed results; an authorization denial is thrown as
    /// <see cref="System.UnauthorizedAccessException"/> so it can never be mistaken for success.
    /// </summary>
    public interface ITicketService
    {
        /// <summary>Requires Permission.ViewTickets.</summary>
        Task<IReadOnlyList<Ticket>> GetTicketsAsync();

        /// <summary>Requires Permission.AddNote. The note is stored as text; rendering it safely is the screen's job (HtmlPolicy).</summary>
        Task<OperationResult<Ticket>> AddNoteAsync(int ticketId, string note);

        /// <summary>Requires Permission.CloseTicket (Supervisor or Admin).</summary>
        Task<OperationResult<Ticket>> CloseAsync(int ticketId);

        /// <summary>Requires Permission.DeleteTicket (Supervisor or Admin). Denied → UnauthorizedAccessException, audited.</summary>
        Task<OperationResult<int>> DeleteAsync(int ticketId);
    }
}
