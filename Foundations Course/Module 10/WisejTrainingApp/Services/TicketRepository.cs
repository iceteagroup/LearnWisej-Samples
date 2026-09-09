using System;
using System.Collections.Generic;
using System.Linq;
using WisejTrainingApp.Models;

namespace WisejTrainingApp.Services
{
    /// <summary>
    /// The "fake repository" the lesson names (s46 §3: "TicketService -> CRUD logic and a fake repository").
    /// It is the only class that owns the ticket list. A production version swaps this file for a database
    /// repository (EF Core, Dapper, a REST client) and nothing above it — service, dialog, grid — changes.
    ///
    /// The list is an instance field: each user session (each Window1) gets its own repository through
    /// its own TicketService. Never a static field — that would be shared by every browser tab on the server.
    /// </summary>
    public class TicketRepository
    {
        private readonly List<Ticket> tickets;

        public TicketRepository()
        {
            // The six tickets every module seeds (same customers, same Ids) plus two more so the capstone
            // dashboard has something to count in every status.
            tickets = new List<Ticket>
            {
                new Ticket { Id = 1, Title = "Login page times out",           Customer = "Northwind",            Status = "Open",        Priority = "High",   AssignedTo = "Ada",    CreatedDate = DateTime.Today.AddDays(-6), Description = "Users on the VPN see a timeout after 30 s." },
                new Ticket { Id = 2, Title = "Invoice PDF missing logo",       Customer = "Contoso",              Status = "In Progress", Priority = "Medium", AssignedTo = "Grace",  CreatedDate = DateTime.Today.AddDays(-5), Description = "The header image is not embedded in exported invoices." },
                new Ticket { Id = 3, Title = "Add dark theme option",          Customer = "Fabrikam",             Status = "Open",        Priority = "Low",    AssignedTo = "",       CreatedDate = DateTime.Today.AddDays(-4), Description = "Feature request from the support portal." },
                new Ticket { Id = 4, Title = "Grid sorting resets on refresh", Customer = "Adventure Works",      Status = "Closed",      Priority = "Medium", AssignedTo = "Linus",  CreatedDate = DateTime.Today.AddDays(-3), Description = "Fixed by keeping the sort column in session state." },
                new Ticket { Id = 5, Title = "Export to Excel is slow",        Customer = "Tailspin",             Status = "In Progress", Priority = "High",   AssignedTo = "Ada",    CreatedDate = DateTime.Today.AddDays(-2), Description = "20 000 rows take 40 s; should stream instead of buffering." },
                new Ticket { Id = 6, Title = "Wrong currency on summary",      Customer = "Wide World Importers", Status = "Open",        Priority = "Medium", AssignedTo = "Grace",  CreatedDate = DateTime.Today.AddDays(-1), Description = "Summary card shows USD for a EUR customer." },
                new Ticket { Id = 7, Title = "Password reset email not sent",  Customer = "Northwind",            Status = "Closed",      Priority = "High",   AssignedTo = "Linus",  CreatedDate = DateTime.Today.AddDays(-1), Description = "SMTP relay rejected the sender; fixed the From address." },
                new Ticket { Id = 8, Title = "Attachment upload limit too low", Customer = "Contoso",             Status = "Open",        Priority = "Low",    AssignedTo = "",       CreatedDate = DateTime.Today,             Description = "Customers cannot attach 12 MB screenshots; raise the limit to 25 MB." },
            };
        }

        /// <summary>A copy of the list, ordered by Id, so callers cannot change the store by accident.</summary>
        public List<Ticket> GetAll()
        {
            return tickets.OrderBy(t => t.Id).ToList();
        }

        public Ticket Get(int id)
        {
            return tickets.FirstOrDefault(t => t.Id == id);
        }

        /// <summary>Stores the ticket and assigns the next Id — the repository, not the caller, chooses Ids.</summary>
        public void Add(Ticket ticket)
        {
            ticket.Id = tickets.Count == 0 ? 1 : tickets.Max(t => t.Id) + 1;
            tickets.Add(ticket);
        }

        /// <summary>Replaces the stored ticket that has the same Id.</summary>
        public void Update(Ticket ticket)
        {
            int index = tickets.FindIndex(t => t.Id == ticket.Id);
            if (index < 0)
                throw new InvalidOperationException($"Ticket #{ticket.Id} does not exist.");

            tickets[index] = ticket;
        }

        /// <summary>Removes the ticket; returns false when there was nothing to remove.</summary>
        public bool Delete(int id)
        {
            return tickets.RemoveAll(t => t.Id == id) > 0;
        }
    }
}
