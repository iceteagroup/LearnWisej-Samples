using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Data
{
    /// <summary>
    /// In-memory repository seeded with the TicketOps work orders. One instance per session (created in
    /// AppComposition), so two browser tabs never share a list.
    /// </summary>
    public sealed class InMemoryTicketRepository : ITicketRepository
    {
        private readonly Dictionary<int, Ticket> _tickets = new Dictionary<int, Ticket>();

        public InMemoryTicketRepository()
        {
            foreach (var t in SeedData.Tickets())
                _tickets[t.Id] = t;
        }

        public Task<IReadOnlyList<Ticket>> GetAllAsync()
        {
            IReadOnlyList<Ticket> rows = _tickets.Values.OrderBy(t => t.Id).Select(Clone).ToList();
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

            _tickets[ticket.Id] = Clone(ticket);
            return Task.FromResult(Clone(ticket));
        }

        public Task<bool> DeleteAsync(int id) => Task.FromResult(_tickets.Remove(id));

        private static Ticket Clone(Ticket t) => new Ticket
        {
            Id = t.Id,
            Title = t.Title,
            Status = t.Status,
            Note = t.Note,
            CreatedAt = t.CreatedAt,
            ClosedAt = t.ClosedAt,
            ClosedBy = t.ClosedBy
        };
    }

    /// <summary>The work orders the walkthrough video shows (#2002 "Repair loading dock pump" is the one it tries to delete).</summary>
    public static class SeedData
    {
        public static IEnumerable<Ticket> Tickets()
        {
            yield return new Ticket { Id = 2001, Title = "Replace conveyor belt sensor", Status = TicketStatus.InProgress, CreatedAt = DateTime.Now.AddDays(-3) };
            yield return new Ticket { Id = 2002, Title = "Repair loading dock pump", Status = TicketStatus.Open, CreatedAt = DateTime.Now.AddDays(-1) };
            yield return new Ticket { Id = 2003, Title = "Calibrate boiler pressure gauge", Status = TicketStatus.Open, CreatedAt = DateTime.Now.AddHours(-20) };
            yield return new Ticket { Id = 2004, Title = "Inspect forklift charger", Status = TicketStatus.InProgress, Note = "Charger trips the breaker after ~40 min.", CreatedAt = DateTime.Now.AddHours(-9) };
            yield return new Ticket { Id = 2005, Title = "Warehouse door seal replacement", Status = TicketStatus.Open, CreatedAt = DateTime.Now.AddHours(-3) };
            yield return new Ticket { Id = 2006, Title = "Emergency light test — bay 4", Status = TicketStatus.Closed, ClosedAt = DateTime.Now.AddHours(-1), ClosedBy = "m.weber", CreatedAt = DateTime.Now.AddHours(-2) };
        }
    }
}
