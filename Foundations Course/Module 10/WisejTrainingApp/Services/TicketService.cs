using System;
using System.Collections.Generic;
using System.Linq;
using WisejTrainingApp.Models;

namespace WisejTrainingApp.Services
{
    /// <summary>The four numbers the Dashboard cards show.</summary>
    public class TicketSummary
    {
        public int Total { get; set; }
        public int Open { get; set; }
        public int InProgress { get; set; }
        public int Closed { get; set; }
    }

    /// <summary>
    /// The service layer the course keeps business logic in (s46 §2: "TicketService handles ticket operations
    /// instead of putting all logic in UI events"). The Tickets screen, the dialog and the Dashboard never touch
    /// the list: they call this class, and this class talks to the fake repository.
    ///
    /// Same public API as Modules 4, 5 and 7 — GetTickets / GetTicket / AddTicket / UpdateTicket / DeleteTicket /
    /// SaveTicket — plus GetSummary() for the capstone dashboard.
    /// </summary>
    public class TicketService
    {
        private readonly TicketRepository repository;
        private readonly TicketValidator validator;

        public TicketService()
            : this(new TicketRepository(), new TicketValidator())
        {
        }

        public TicketService(TicketRepository repository, TicketValidator validator)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public List<Ticket> GetTickets()
        {
            return repository.GetAll();
        }

        public Ticket GetTicket(int id)
        {
            return repository.Get(id);
        }

        /// <summary>Validates, stamps CreatedDate, stores. The repository assigns the Id.</summary>
        public void AddTicket(Ticket t)
        {
            Guard(t);

            if (t.CreatedDate == default)
                t.CreatedDate = DateTime.Now;

            repository.Add(t);
        }

        /// <summary>Validates and replaces the stored ticket with the same Id.</summary>
        public void UpdateTicket(Ticket t)
        {
            Guard(t);
            repository.Update(t);
        }

        public void DeleteTicket(int id)
        {
            if (!repository.Delete(id))
                throw new InvalidOperationException($"Ticket #{id} does not exist.");
        }

        /// <summary>Add or update — the one-call form the course's Save handler uses.</summary>
        public void SaveTicket(Ticket t)
        {
            if (t == null)
                throw new ArgumentNullException(nameof(t));

            if (t.Id == 0 || GetTicket(t.Id) == null)
                AddTicket(t);
            else
                UpdateTicket(t);
        }

        /// <summary>Counts for the Dashboard cards, computed from the repository every time it is asked.</summary>
        public TicketSummary GetSummary()
        {
            var all = repository.GetAll();
            return new TicketSummary
            {
                Total = all.Count,
                Open = all.Count(t => t.Status == "Open"),
                InProgress = all.Count(t => t.Status == "In Progress"),
                Closed = all.Count(t => t.Status == "Closed"),
            };
        }

        /// <summary>Tickets that name the given company — used by the Customers screen.</summary>
        public List<Ticket> GetTicketsForCustomer(string company)
        {
            return repository.GetAll()
                .Where(t => string.Equals(t.Customer, company, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// The server-side guard (s42 §1: never rely only on the UI). The dialog already validated; if a caller
        /// bypasses the dialog the same rules still apply.
        /// </summary>
        private void Guard(Ticket t)
        {
            if (t == null)
                throw new ArgumentNullException(nameof(t));

            var result = validator.Validate(t);
            if (!result.IsValid)
                throw new InvalidOperationException("Ticket rejected by TicketValidator: " + string.Join(" ", result.Errors));
        }
    }
}
