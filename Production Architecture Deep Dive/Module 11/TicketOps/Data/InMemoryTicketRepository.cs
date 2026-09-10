using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketOps.Domain;
using TicketOps.Infrastructure;

namespace TicketOps.Data
{
    /// <summary>
    /// Raised by the data layer when the store is unreachable. Its message is deliberately internal
    /// (host names, table names): it belongs in the log, and the screen must not show it.
    /// </summary>
    public sealed class DataOutageException : Exception
    {
        public DataOutageException(string message) : base(message) { }
    }

    /// <summary>
    /// In-memory repository seeded with the TicketOps work orders. One instance per session (created in
    /// AppComposition), so two browser tabs never share a list. <see cref="SimulateOutage"/> lets the lab
    /// show the error path without a real database.
    /// </summary>
    public sealed class InMemoryTicketRepository : ITicketRepository
    {
        private readonly ILog _log;
        private readonly Dictionary<int, Ticket> _tickets = new Dictionary<int, Ticket>();

        public bool SimulateOutage { get; set; }

        public InMemoryTicketRepository(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            foreach (var t in SeedData.Tickets())
                _tickets[t.Id] = t;
            _log.Info(LogLayer.Data, "InMemoryTicketRepository", $"seeded {_tickets.Count} work orders (in-memory, per session)");
        }

        public Task<IReadOnlyList<Ticket>> GetAllAsync()
        {
            EnsureAvailable("SELECT * FROM Tickets");
            _log.Info(LogLayer.Data, "InMemoryTicketRepository.GetAllAsync", $"{_tickets.Count} rows");
            IReadOnlyList<Ticket> rows = _tickets.Values.OrderBy(t => t.Id).Select(Clone).ToList();
            return Task.FromResult(rows);
        }

        public Task<Ticket> FindAsync(int id)
        {
            EnsureAvailable($"SELECT * FROM Tickets WHERE Id={id}");
            _tickets.TryGetValue(id, out var ticket);
            _log.Info(LogLayer.Data, "InMemoryTicketRepository.FindAsync", ticket == null ? $"#{id} not found" : $"#{id} found");
            return Task.FromResult(ticket == null ? null : Clone(ticket));
        }

        public Task<Ticket> UpsertAsync(Ticket ticket)
        {
            if (ticket == null) throw new ArgumentNullException(nameof(ticket));
            EnsureAvailable($"UPDATE Tickets WHERE Id={ticket.Id}");

            _tickets[ticket.Id] = Clone(ticket);
            _log.Info(LogLayer.Data, "InMemoryTicketRepository.UpsertAsync", $"#{ticket.Id} written ({_tickets.Count} rows)");
            return Task.FromResult(Clone(ticket));
        }

        public Task<bool> DeleteAsync(int id)
        {
            EnsureAvailable($"DELETE FROM Tickets WHERE Id={id}");
            bool removed = _tickets.Remove(id);
            _log.Info(LogLayer.Data, "InMemoryTicketRepository.DeleteAsync", removed ? $"#{id} deleted ({_tickets.Count} rows left)" : $"#{id} not found");
            return Task.FromResult(removed);
        }

        private void EnsureAvailable(string statement)
        {
            if (!SimulateOutage)
                return;

            // What a real driver would say — and exactly what must not reach the user.
            _log.Error(LogLayer.Data, "InMemoryTicketRepository", null,
                $"outage: {statement} failed — timeout connecting to sql01:1433 (TicketOps.dbo.Tickets)");
            throw new DataOutageException("Timeout connecting to sql01:1433 while executing: " + statement);
        }

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

    /// <summary>The work orders the walkthrough video shows (#2002 "Repair loading dock pump" is the one it deletes — or tries to).</summary>
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
