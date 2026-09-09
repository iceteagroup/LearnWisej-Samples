using System;
using System.Collections.Generic;
using System.Linq;
using WisejTrainingApp.Models;

namespace WisejTrainingApp.Services
{
    /// <summary>
    /// The service (repository) layer — lesson s16 §3. Get / save / add / update / delete live here,
    /// never in button clicks. In this lab the "persisted data" is an in-memory list held by this
    /// instance; one instance per user session (a field on the form), never a static.
    ///
    /// GetTickets() returns a fresh list of copies, so what the screen binds to (business state) and
    /// what the service holds (persisted data) are two different things — exactly the distinction
    /// lesson s16 §7 draws. SaveTicket() is the only way values get from the screen into the store.
    /// Later the same methods could call a database; the form would not change.
    /// </summary>
    public class TicketService
    {
        private readonly List<Ticket> tickets = new List<Ticket>();
        private int nextId;

        public TicketService()
        {
            Seed();
        }

        /// <summary>A snapshot of every ticket (copies — edit them freely, then SaveTicket to persist).</summary>
        public List<Ticket> GetTickets()
        {
            return tickets.Select(Copy).ToList();
        }

        /// <summary>A copy of one ticket, or null when the id is unknown.</summary>
        public Ticket GetTicket(int id)
        {
            Ticket stored = tickets.FirstOrDefault(t => t.Id == id);
            return stored == null ? null : Copy(stored);
        }

        /// <summary>Adds a new ticket; assigns the next Id and the creation date when missing.</summary>
        public void AddTicket(Ticket ticket)
        {
            Validate(ticket);

            ticket.Id = ++nextId;
            if (ticket.CreatedDate == default(DateTime))
                ticket.CreatedDate = DateTime.Today;

            tickets.Add(Copy(ticket));
        }

        /// <summary>Copies the values of an existing ticket into the store.</summary>
        public void UpdateTicket(Ticket ticket)
        {
            Validate(ticket);

            Ticket stored = tickets.FirstOrDefault(t => t.Id == ticket.Id);
            if (stored == null)
                throw new ArgumentException($"Ticket #{ticket.Id} does not exist.");

            CopyInto(ticket, stored);
        }

        public void DeleteTicket(int id)
        {
            tickets.RemoveAll(t => t.Id == id);
        }

        /// <summary>
        /// Add or update — the one method the Save Ticket button calls. Validation happens here, in the
        /// service, so the rule is the same whoever calls it; the UI only decides how to show the message.
        /// </summary>
        public void SaveTicket(Ticket ticket)
        {
            if (ticket == null)
                throw new ArgumentException("No ticket is selected.");

            if (ticket.Id == 0)
                AddTicket(ticket);
            else
                UpdateTicket(ticket);
        }

        public int Count => tickets.Count;

        public static readonly string[] Statuses = { "Open", "In Progress", "Closed" };
        public static readonly string[] Priorities = { "Low", "Medium", "High" };

        #region Rules and helpers

        private static void Validate(Ticket ticket)
        {
            if (ticket == null)
                throw new ArgumentException("No ticket is selected.");

            if (string.IsNullOrWhiteSpace(ticket.Title))
                throw new ArgumentException("Title is required.");

            if (!Statuses.Contains(ticket.Status))
                throw new ArgumentException("Status must be Open, In Progress or Closed.");

            if (!string.IsNullOrEmpty(ticket.Priority) && !Priorities.Contains(ticket.Priority))
                throw new ArgumentException("Priority must be Low, Medium or High.");
        }

        private static Ticket Copy(Ticket source)
        {
            var copy = new Ticket();
            CopyInto(source, copy);
            return copy;
        }

        private static void CopyInto(Ticket source, Ticket target)
        {
            target.Id = source.Id;
            target.Title = source.Title;
            target.Customer = source.Customer;
            target.Status = source.Status;
            target.Priority = source.Priority;
            target.AssignedTo = source.AssignedTo;
            target.CreatedDate = source.CreatedDate;
            target.Description = source.Description;
        }

        private void Seed()
        {
            DateTime today = DateTime.Today;

            AddTicket(new Ticket { Title = "Login page times out on VPN", Customer = "Northwind", Status = "Open", Priority = "High", AssignedTo = "Ada Lovelace", CreatedDate = today.AddDays(-6), Description = "Users on the corporate VPN get a timeout after entering credentials. Reproduced on two laptops." });
            AddTicket(new Ticket { Title = "Invoice PDF missing logo", Customer = "Contoso", Status = "In Progress", Priority = "Medium", AssignedTo = "Grace Hopper", CreatedDate = today.AddDays(-5), Description = "The exported invoice shows a blank box where the customer logo should be." });
            AddTicket(new Ticket { Title = "Add CSV export to the orders grid", Customer = "Fabrikam", Status = "Open", Priority = "Low", AssignedTo = "Linus Torvalds", CreatedDate = today.AddDays(-4), Description = "Feature request: export the filtered orders grid to CSV with the visible columns only." });
            AddTicket(new Ticket { Title = "Dashboard totals off by one day", Customer = "Adventure Works", Status = "In Progress", Priority = "High", AssignedTo = "Ada Lovelace", CreatedDate = today.AddDays(-3), Description = "Daily totals use UTC midnight instead of the customer's local midnight." });
            AddTicket(new Ticket { Title = "Password reset email in wrong language", Customer = "Tailspin", Status = "Closed", Priority = "Medium", AssignedTo = "Margaret Hamilton", CreatedDate = today.AddDays(-2), Description = "German users received the English template. Fixed by reading the culture from the user profile." });
            AddTicket(new Ticket { Title = "Slow search with more than 10k products", Customer = "Wide World Importers", Status = "Open", Priority = "High", AssignedTo = "Grace Hopper", CreatedDate = today.AddDays(-1), Description = "Product search takes 8+ seconds on the large catalogue. Needs an index on the name column." });
        }

        #endregion
    }
}
