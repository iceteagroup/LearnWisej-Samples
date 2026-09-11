using System;
using System.Collections.Generic;
using WisejTrainingApp.Models;

namespace WisejTrainingApp.Services
{
    public class TicketService
    {
        private readonly List<Ticket> tickets = new List<Ticket>();

        public TicketService()
        {
            // A few tickets to start with — kept in memory for this lab.
            tickets.Add(new Ticket { Id = 1, Title = "Login page times out", Customer = "Northwind", Status = "Open", Priority = "High", AssignedTo = "Ada", CreatedDate = DateTime.Today.AddDays(-4), Description = "Users on the VPN get a timeout after signing in." });
            tickets.Add(new Ticket { Id = 2, Title = "Invoice PDF missing logo", Customer = "Contoso", Status = "In Progress", Priority = "Medium", AssignedTo = "Grace", CreatedDate = DateTime.Today.AddDays(-3), Description = "The exported invoice shows a blank box instead of the logo." });
            tickets.Add(new Ticket { Id = 3, Title = "Add CSV export to orders", Customer = "Fabrikam", Status = "Open", Priority = "Low", AssignedTo = "Linus", CreatedDate = DateTime.Today.AddDays(-2), Description = "Export the orders grid to CSV." });
            tickets.Add(new Ticket { Id = 4, Title = "Password reset email in wrong language", Customer = "Tailspin", Status = "Closed", Priority = "Medium", AssignedTo = "Margaret", CreatedDate = DateTime.Today.AddDays(-1), Description = "German users received the English email." });
        }

        public List<Ticket> GetTickets()
        {
            return tickets;
        }

        public void SaveTicket(Ticket ticket)
        {
            // Later this could save to a database.
            // For Lab 4, update the object in memory and refresh the UI.
        }
    }
}
