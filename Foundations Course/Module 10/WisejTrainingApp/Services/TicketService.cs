using System;
using System.Collections.Generic;
using System.Linq;
using WisejTrainingApp.Models;

namespace WisejTrainingApp.Services
{
    public class TicketSummary
    {
        public int Total { get; set; }
        public int Open { get; set; }
        public int InProgress { get; set; }
        public int Closed { get; set; }
    }

    // Ticket operations live here, not in button clicks. The list itself lives in a fake repository.
    public class TicketService
    {
        private readonly TicketRepository repository = new TicketRepository();
        private readonly TicketValidator validator = new TicketValidator();

        public List<Ticket> GetTickets()
        {
            return repository.GetAll();
        }

        public void AddTicket(Ticket ticket)
        {
            Validate(ticket);
            ticket.CreatedDate = DateTime.Now;
            repository.Add(ticket);
        }

        public void UpdateTicket(Ticket ticket)
        {
            Validate(ticket);
            repository.Update(ticket);
        }

        public void DeleteTicket(int id)
        {
            repository.Delete(id);
        }

        public TicketSummary GetSummary()
        {
            List<Ticket> all = repository.GetAll();
            return new TicketSummary
            {
                Total = all.Count,
                Open = all.Count(t => t.Status == "Open"),
                InProgress = all.Count(t => t.Status == "In Progress"),
                Closed = all.Count(t => t.Status == "Closed"),
            };
        }

        private void Validate(Ticket ticket)
        {
            ValidationResult result = validator.Validate(ticket);
            if (!result.IsValid)
                throw new InvalidOperationException(result.Message);
        }
    }
}
