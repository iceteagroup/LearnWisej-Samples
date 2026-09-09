using System;
using System.Collections.Generic;
using System.Linq;
using WisejTrainingApp.Models;

namespace WisejTrainingApp.Services
{
    /// <summary>
    /// In-memory ticket store. One instance per user session (MainPage owns it), so two browsers
    /// never share a list — the same rule every Foundations module follows.
    /// </summary>
    public class TicketService
    {
        private readonly List<Ticket> tickets;
        private int nextId;

        public TicketService()
        {
            var today = DateTime.Today;
            tickets = new List<Ticket>
            {
                new Ticket { Id = 1, Title = "Cannot log in to portal",        Customer = "Northwind",            Status = "Open",        Priority = "High",   AssignedTo = "Ada",   CreatedDate = today.AddDays(-6), Description = "User reports a 401 after the password reset." },
                new Ticket { Id = 2, Title = "Invoice PDF shows wrong total",  Customer = "Contoso",              Status = "In Progress", Priority = "Medium", AssignedTo = "Grace", CreatedDate = today.AddDays(-5), Description = "Discount line is doubled on multi-currency invoices." },
                new Ticket { Id = 3, Title = "Export to Excel times out",      Customer = "Fabrikam",             Status = "Open",        Priority = "Medium", AssignedTo = "",      CreatedDate = today.AddDays(-4), Description = "Exports over 10,000 rows never finish." },
                new Ticket { Id = 4, Title = "Add new warehouse location",     Customer = "Adventure Works",      Status = "Closed",      Priority = "Low",    AssignedTo = "Ada",   CreatedDate = today.AddDays(-3), Description = "Configuration request; done." },
                new Ticket { Id = 5, Title = "Email notifications delayed",    Customer = "Tailspin",             Status = "Open",        Priority = "High",   AssignedTo = "Linus", CreatedDate = today.AddDays(-2), Description = "Notifications arrive up to two hours late." },
                new Ticket { Id = 6, Title = "Dashboard chart missing labels", Customer = "Wide World Importers", Status = "In Progress", Priority = "Low",    AssignedTo = "Grace", CreatedDate = today.AddDays(-1), Description = "Axis labels disappear at narrow widths." },
            };
            nextId = 7;
        }

        public List<Ticket> GetTickets()
        {
            return tickets.OrderBy(t => t.Id).ToList();
        }

        public Ticket GetTicket(int id)
        {
            return tickets.FirstOrDefault(t => t.Id == id);
        }

        public void AddTicket(Ticket t)
        {
            t.Id = nextId++;
            if (t.CreatedDate == default) t.CreatedDate = DateTime.Now;
            if (string.IsNullOrEmpty(t.Status)) t.Status = "Open";
            tickets.Add(t);
        }

        public void UpdateTicket(Ticket t)
        {
            var existing = GetTicket(t.Id);
            if (existing == null) throw new InvalidOperationException($"Ticket #{t.Id} does not exist.");
            existing.Title = t.Title;
            existing.Customer = t.Customer;
            existing.Status = t.Status;
            existing.Priority = t.Priority;
            existing.AssignedTo = t.AssignedTo;
            existing.Description = t.Description;
        }

        public void DeleteTicket(int id)
        {
            tickets.RemoveAll(t => t.Id == id);
        }

        /// <summary>Add or update, depending on whether the ticket already has an Id.</summary>
        public void SaveTicket(Ticket t)
        {
            if (t.Id == 0) AddTicket(t); else UpdateTicket(t);
        }

        /// <summary>Tickets that are not Closed — the Dashboard's "Open Tickets" number.</summary>
        public int CountOpen()
        {
            return tickets.Count(t => t.Status != "Closed");
        }

        public int CountOpenFor(string customer)
        {
            return tickets.Count(t => t.Customer == customer && t.Status != "Closed");
        }
    }
}
