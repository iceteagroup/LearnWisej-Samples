using System;
using System.Collections.Generic;
using System.Linq;
using WisejTrainingApp.Models;

namespace WisejTrainingApp.Services
{
    /// <summary>
    /// The service layer the lesson keeps business logic in. The Tickets page and the dialog never
    /// touch the list directly: they call AddTicket / UpdateTicket / DeleteTicket / SaveTicket.
    ///
    /// The list is an instance field, so each user session (each TicketsWindow) owns its own data —
    /// never a static field, which would be shared by every browser tab on the server.
    /// </summary>
    public class TicketService
    {
        private readonly List<Ticket> tickets;

        public TicketService()
        {
            tickets = new List<Ticket>
            {
                new Ticket { Id = 1, Title = "Login page times out",           Customer = "Northwind",            Status = "Open",        Priority = "High",   AssignedTo = "Ada",    CreatedDate = DateTime.Today.AddDays(-6), Description = "Users on the VPN see a timeout after 30 s." },
                new Ticket { Id = 2, Title = "Invoice PDF missing logo",       Customer = "Contoso",              Status = "In Progress", Priority = "Medium", AssignedTo = "Grace",  CreatedDate = DateTime.Today.AddDays(-5), Description = "The header image is not embedded in exported invoices." },
                new Ticket { Id = 3, Title = "Add dark theme option",          Customer = "Fabrikam",             Status = "Open",        Priority = "Low",    AssignedTo = "",       CreatedDate = DateTime.Today.AddDays(-4), Description = "Feature request from the support portal." },
                new Ticket { Id = 4, Title = "Grid sorting resets on refresh", Customer = "Adventure Works",      Status = "Closed",      Priority = "Medium", AssignedTo = "Linus",  CreatedDate = DateTime.Today.AddDays(-3), Description = "Fixed by keeping the sort column in session state." },
                new Ticket { Id = 5, Title = "Export to Excel is slow",        Customer = "Tailspin",             Status = "In Progress", Priority = "High",   AssignedTo = "Ada",    CreatedDate = DateTime.Today.AddDays(-2), Description = "20 000 rows take 40 s; should stream instead of buffering." },
                new Ticket { Id = 6, Title = "Wrong currency on summary",      Customer = "Wide World Importers", Status = "Open",        Priority = "Medium", AssignedTo = "Grace",  CreatedDate = DateTime.Today.AddDays(-1), Description = "Summary card shows USD for a EUR customer." },
            };
        }

        /// <summary>A copy of the list, ordered by Id, so callers cannot change the store by accident.</summary>
        public List<Ticket> GetTickets()
        {
            return tickets.OrderBy(t => t.Id).ToList();
        }

        public Ticket GetTicket(int id)
        {
            return tickets.FirstOrDefault(t => t.Id == id);
        }

        /// <summary>Adds a ticket and assigns the next Id. The caller does not choose Ids.</summary>
        public void AddTicket(Ticket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException(nameof(ticket));

            ticket.Id = tickets.Count == 0 ? 1 : tickets.Max(t => t.Id) + 1;
            if (ticket.CreatedDate == default)
                ticket.CreatedDate = DateTime.Now;

            tickets.Add(ticket);
        }

        /// <summary>Replaces the stored ticket that has the same Id.</summary>
        public void UpdateTicket(Ticket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException(nameof(ticket));

            int index = tickets.FindIndex(t => t.Id == ticket.Id);
            if (index < 0)
                throw new InvalidOperationException($"Ticket #{ticket.Id} does not exist.");

            tickets[index] = ticket;
        }

        public void DeleteTicket(int id)
        {
            tickets.RemoveAll(t => t.Id == id);
        }

        /// <summary>Add or update — the one-call form the lesson's Save handler uses.</summary>
        public void SaveTicket(Ticket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException(nameof(ticket));

            if (ticket.Id == 0 || GetTicket(ticket.Id) == null)
                AddTicket(ticket);
            else
                UpdateTicket(ticket);
        }
    }
}
