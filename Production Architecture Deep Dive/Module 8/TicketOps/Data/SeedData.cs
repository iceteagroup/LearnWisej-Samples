using System;
using System.Collections.Generic;
using TicketOps.Domain;

namespace TicketOps.Data
{
    /// <summary>
    /// The demo data every fake starts from. Pure functions returning fresh objects: a static method
    /// that holds no state is fine — a static field holding tickets would be shared by every session.
    /// </summary>
    public static class SeedData
    {
        public static IEnumerable<Operator> Operators()
        {
            yield return new Operator { Id = 1, Name = "Dana Reyes", Role = OperatorRole.Technician, Email = "dana.reyes@ticketops.local" };
            yield return new Operator { Id = 2, Name = "Marco Lindqvist", Role = OperatorRole.Technician, Email = "marco.lindqvist@ticketops.local" };
            yield return new Operator { Id = 3, Name = "Priya Natarajan", Role = OperatorRole.Manager, Email = "priya.natarajan@ticketops.local" };
            yield return new Operator { Id = 4, Name = "Sam Okafor", Role = OperatorRole.Viewer, Email = "sam.okafor@ticketops.local" };
        }

        /// <summary>Operator 1 (Dana Reyes, Technician) is the default signed-in user of a new session.</summary>
        public const int DefaultOperatorId = 1;

        public static IEnumerable<Ticket> Tickets()
        {
            yield return new Ticket { Id = 1041, Title = "Printer offline on floor 2", Priority = TicketPriority.High, HoursLogged = 1.5, AssigneeId = 1, Status = TicketStatus.InProgress, CreatedAt = DateTime.Now.AddDays(-2) };
            yield return new Ticket { Id = 1042, Title = "VPN drops every 20 minutes", Priority = TicketPriority.High, HoursLogged = 0, AssigneeId = 1, CreatedAt = DateTime.Now.AddDays(-1) };
            yield return new Ticket { Id = 1043, Title = "New starter laptop request", Priority = TicketPriority.Medium, HoursLogged = 0.5, AssigneeId = 2, Status = TicketStatus.InProgress, CreatedAt = DateTime.Now.AddHours(-20) };
            yield return new Ticket { Id = 1044, Title = "Shared mailbox permissions", Priority = TicketPriority.Medium, HoursLogged = 0.25, CreatedAt = DateTime.Now.AddHours(-9) };
            yield return new Ticket { Id = 1045, Title = "Monitor flickers when docked", Priority = TicketPriority.Low, HoursLogged = 0, CreatedAt = DateTime.Now.AddHours(-3) };
            yield return new Ticket { Id = 1046, Title = "Password reset for contractor", Priority = TicketPriority.Low, HoursLogged = 0.25, AssigneeId = 2, Status = TicketStatus.Closed, ClosedAt = DateTime.Now.AddHours(-1), CloseReason = "Reset done", CreatedAt = DateTime.Now.AddHours(-2) };
        }
    }
}
