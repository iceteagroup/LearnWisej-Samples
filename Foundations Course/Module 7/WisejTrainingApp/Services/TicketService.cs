using System;
using System.Collections.Generic;
using WisejTrainingApp.Models;

namespace WisejTrainingApp.Services
{
    public class TicketService
    {
        private readonly List<Ticket> tickets = new List<Ticket>();
        private int nextId = 1;

        public TicketService()
        {
            AddTicket(new Ticket { Title = "Login page times out", Customer = "Northwind", Status = "Open", Priority = "High", AssignedTo = "Ada", Description = "Users on the VPN get a timeout after signing in." });
            AddTicket(new Ticket { Title = "Invoice PDF missing logo", Customer = "Contoso", Status = "In Progress", Priority = "Medium", AssignedTo = "Grace", Description = "The exported invoice shows a blank box instead of the logo." });
            AddTicket(new Ticket { Title = "Add CSV export to orders", Customer = "Fabrikam", Status = "Open", Priority = "Low", AssignedTo = "Linus", Description = "Export the orders grid to CSV." });
        }

        public List<Ticket> GetTickets()
        {
            return tickets;
        }

        public void AddTicket(Ticket ticket)
        {
            ticket.Id = nextId++;
            ticket.CreatedDate = DateTime.Today;
            tickets.Add(ticket);
        }

        public void UpdateTicket(Ticket ticket)
        {
            Ticket stored = tickets.Find(t => t.Id == ticket.Id);
            if (stored == null)
                return;

            stored.Title = ticket.Title;
            stored.Customer = ticket.Customer;
            stored.Status = ticket.Status;
            stored.Priority = ticket.Priority;
            stored.AssignedTo = ticket.AssignedTo;
            stored.Description = ticket.Description;
        }

        public void DeleteTicket(int id)
        {
            tickets.RemoveAll(t => t.Id == id);
        }
    }
}
