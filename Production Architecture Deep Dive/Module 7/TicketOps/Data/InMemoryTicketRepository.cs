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
    ///
    /// Thread-safe on purpose: the import task inserts rows from a worker thread while the session's
    /// request thread answers "Refresh list". Every access to the dictionary — reads included — takes
    /// <see cref="_gate"/>, and the lock is held only around the dictionary, never around anything that could block.
    /// </summary>
    public sealed class InMemoryTicketRepository : ITicketRepository
    {
        private readonly object _gate = new object();
        private readonly Dictionary<int, Ticket> _tickets = new Dictionary<int, Ticket>();

        public InMemoryTicketRepository()
        {
            foreach (var t in SeedData.Tickets())
                _tickets[t.Id] = t;
        }

        public Task<IReadOnlyList<Ticket>> GetAllAsync()
        {
            IReadOnlyList<Ticket> rows;
            lock (_gate)
                rows = _tickets.Values.Select(Clone).ToList();
            return Task.FromResult(rows);
        }

        public Task<Ticket> FindAsync(int id)
        {
            Ticket ticket;
            lock (_gate)
                _tickets.TryGetValue(id, out ticket);
            return Task.FromResult(ticket == null ? null : Clone(ticket));
        }

        public Task<bool> InsertAsync(Ticket ticket)
        {
            if (ticket == null) throw new ArgumentNullException(nameof(ticket));
            if (ticket.Id <= 0) throw new ArgumentException("An imported ticket must carry its id.", nameof(ticket));

            bool added;
            lock (_gate)
            {
                // Check-and-add under ONE lock: two threads can never both see "not there" and both insert.
                added = !_tickets.ContainsKey(ticket.Id);
                if (added)
                    _tickets[ticket.Id] = Clone(ticket);
            }
            return Task.FromResult(added);
        }

        public Task<int> CountAsync()
        {
            int count;
            lock (_gate)
                count = _tickets.Count;
            return Task.FromResult(count);
        }

        private static Ticket Clone(Ticket t) => new Ticket
        {
            Id = t.Id,
            Title = t.Title,
            Priority = t.Priority,
            Status = t.Status,
            Assignee = t.Assignee,
            DueDate = t.DueDate,
            HoursLogged = t.HoursLogged,
            CreatedAt = t.CreatedAt,
            ClosedAt = t.ClosedAt
        };
    }

    /// <summary>The tickets the TicketOps Console starts with (the same six as Module 1).</summary>
    public static class SeedData
    {
        public static IEnumerable<Ticket> Tickets()
        {
            yield return new Ticket { Id = 1041, Title = "Printer offline on floor 2", Priority = TicketPriority.High, Assignee = "ana", HoursLogged = 1.5, Status = TicketStatus.InProgress, CreatedAt = DateTime.Now.AddDays(-2) };
            yield return new Ticket { Id = 1042, Title = "VPN drops every 20 minutes", Priority = TicketPriority.High, Assignee = "jle", HoursLogged = 0, CreatedAt = DateTime.Now.AddDays(-1) };
            yield return new Ticket { Id = 1043, Title = "New starter laptop request", Priority = TicketPriority.Medium, Assignee = "mrt", HoursLogged = 0.5, Status = TicketStatus.InProgress, CreatedAt = DateTime.Now.AddHours(-20) };
            yield return new Ticket { Id = 1044, Title = "Shared mailbox permissions", Priority = TicketPriority.Medium, Assignee = "pko", HoursLogged = 0, CreatedAt = DateTime.Now.AddHours(-9) };
            yield return new Ticket { Id = 1045, Title = "Monitor flickers when docked", Priority = TicketPriority.Low, Assignee = "sva", HoursLogged = 0, CreatedAt = DateTime.Now.AddHours(-3) };
            yield return new Ticket { Id = 1046, Title = "Password reset for contractor", Priority = TicketPriority.Low, Assignee = "ana", HoursLogged = 0.25, Status = TicketStatus.Closed, ClosedAt = DateTime.Now.AddHours(-1), CreatedAt = DateTime.Now.AddHours(-2) };
        }
    }
}
