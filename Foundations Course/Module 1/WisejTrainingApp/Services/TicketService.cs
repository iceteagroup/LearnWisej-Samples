using System.Collections.Generic;
using WisejTrainingApp.Models;

namespace WisejTrainingApp.Services
{
    /// <summary>
    /// The service class both Module 1 readings describe: business logic and data operations live
    /// here, away from the UI. "What Wisej.NET Is" lists it in the key vocabulary and its example
    /// flow ends in a service call; "Getting Started" makes it a habit — the click handler calls
    /// ticketService.Save(ticket) instead of owning the rules itself.
    ///
    /// The screen never learns where a ticket goes. Swap this list for a database in a later module
    /// and Window1.cs does not change.
    /// </summary>
    public class TicketService
    {
        private readonly List<Ticket> tickets = new List<Ticket>();
        private int nextId = 1;

        public int Count => this.tickets.Count;

        public IReadOnlyList<Ticket> GetTickets() => this.tickets;

        /// <summary>
        /// Assigns the id and stores the ticket. This is the "business rule" — it belongs here,
        /// not in btnSaveTicket_Click.
        /// </summary>
        public Ticket Save(Ticket ticket)
        {
            if (ticket.Id == 0)
            {
                ticket.Id = this.nextId++;
                this.tickets.Add(ticket);
            }

            return ticket;
        }
    }
}
