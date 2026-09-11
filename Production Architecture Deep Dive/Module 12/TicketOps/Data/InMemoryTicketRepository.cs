using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Data
{
    /// <summary>
    /// In-memory repository seeded with the TicketOps demo tickets. One instance per session (created in
    /// AppComposition), so two browser tabs never share a list — the same reason nothing here is static.
    /// The health check's database probe reads it.
    /// </summary>
    public sealed class InMemoryTicketRepository : ITicketRepository
    {
        private readonly Dictionary<int, Ticket> _tickets = new Dictionary<int, Ticket>();
        private int _nextId;

        public InMemoryTicketRepository()
        {
            foreach (var t in SeedData.Tickets())
                _tickets[t.Id] = t;
            _nextId = _tickets.Keys.Max() + 1;
        }

        public Task<IReadOnlyList<Ticket>> GetAllAsync()
        {
            IReadOnlyList<Ticket> rows = _tickets.Values.Select(Clone).ToList();
            return Task.FromResult(rows);
        }

        public Task<Ticket> FindAsync(int id)
        {
            _tickets.TryGetValue(id, out var ticket);
            return Task.FromResult(ticket == null ? null : Clone(ticket));
        }

        public Task<Ticket> UpsertAsync(Ticket ticket)
        {
            if (ticket == null) throw new ArgumentNullException(nameof(ticket));

            if (ticket.Id == 0)
                ticket.Id = _nextId++;

            _tickets[ticket.Id] = Clone(ticket);
            return Task.FromResult(Clone(ticket));
        }

        private static Ticket Clone(Ticket t) => new Ticket
        {
            Id = t.Id,
            Title = t.Title,
            Priority = t.Priority,
            Status = t.Status,
            HoursLogged = t.HoursLogged,
            CreatedAt = t.CreatedAt,
            ClosedAt = t.ClosedAt
        };
    }

    /// <summary>The tickets the walkthrough video shows in the TicketOps Console.</summary>
    public static class SeedData
    {
        public static IEnumerable<Ticket> Tickets()
        {
            yield return new Ticket { Id = 1041, Title = "Printer offline on floor 2", Priority = TicketPriority.High, HoursLogged = 1.5, Status = TicketStatus.InProgress, CreatedAt = DateTime.Now.AddDays(-2) };
            yield return new Ticket { Id = 1042, Title = "VPN drops every 20 minutes", Priority = TicketPriority.High, HoursLogged = 0, CreatedAt = DateTime.Now.AddDays(-1) };
            yield return new Ticket { Id = 1043, Title = "New starter laptop request", Priority = TicketPriority.Medium, HoursLogged = 0.5, Status = TicketStatus.InProgress, CreatedAt = DateTime.Now.AddHours(-20) };
            yield return new Ticket { Id = 1044, Title = "Shared mailbox permissions", Priority = TicketPriority.Medium, HoursLogged = 0, CreatedAt = DateTime.Now.AddHours(-9) };
            yield return new Ticket { Id = 1045, Title = "Monitor flickers when docked", Priority = TicketPriority.Low, HoursLogged = 0, CreatedAt = DateTime.Now.AddHours(-3) };
            yield return new Ticket { Id = 1046, Title = "Password reset for contractor", Priority = TicketPriority.Low, HoursLogged = 0.25, Status = TicketStatus.Closed, ClosedAt = DateTime.Now.AddHours(-1), CreatedAt = DateTime.Now.AddHours(-2) };
        }
    }
}
