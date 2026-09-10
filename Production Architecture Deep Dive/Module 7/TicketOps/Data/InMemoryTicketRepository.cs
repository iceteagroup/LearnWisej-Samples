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
    /// In-memory repository seeded with the TicketOps demo tickets. One instance per session (created in
    /// AppComposition), so two browser tabs never share a list — the same reason nothing here is static.
    ///
    /// Thread-safe on purpose: in Module 7 the import task inserts rows from a worker thread while the
    /// session's request thread answers "Refresh list". Every access to the dictionary — reads included —
    /// takes <see cref="_gate"/>, and the lock is held only around the dictionary, never around logging or
    /// anything that could block. <see cref="SimulateOutage"/> is read by the worker and written by the UI,
    /// so it is volatile.
    /// </summary>
    public sealed class InMemoryTicketRepository : ITicketRepository
    {
        private readonly ILog _log;
        private readonly object _gate = new object();
        private readonly Dictionary<int, Ticket> _tickets = new Dictionary<int, Ticket>();
        private volatile bool _simulateOutage;

        /// <summary>The lab's outage switch. Written by the request thread, read by the import task.</summary>
        public bool SimulateOutage
        {
            get => _simulateOutage;
            set => _simulateOutage = value;
        }

        public InMemoryTicketRepository(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            Seed();
            _log.Info(LogLayer.Data, "InMemoryTicketRepository", $"seeded {Count} tickets (in-memory, per session, lock-guarded)");
        }

        private int Count
        {
            get { lock (_gate) return _tickets.Count; }
        }

        public Task<IReadOnlyList<Ticket>> GetAllAsync()
        {
            EnsureAvailable("SELECT * FROM Tickets");
            IReadOnlyList<Ticket> rows;
            lock (_gate)
                rows = _tickets.Values.Select(Clone).ToList();
            _log.Info(LogLayer.Data, "InMemoryTicketRepository.GetAllAsync", $"{rows.Count} rows (read under the lock, request thread)");
            return Task.FromResult(rows);
        }

        public Task<Ticket> FindAsync(int id)
        {
            EnsureAvailable($"SELECT * FROM Tickets WHERE Id={id}");
            Ticket ticket;
            lock (_gate)
                _tickets.TryGetValue(id, out ticket);
            return Task.FromResult(ticket == null ? null : Clone(ticket));
        }

        public Task<bool> InsertAsync(Ticket ticket)
        {
            if (ticket == null) throw new ArgumentNullException(nameof(ticket));
            if (ticket.Id <= 0) throw new ArgumentException("An imported ticket must carry its id.", nameof(ticket));

            EnsureAvailable($"INSERT INTO Tickets (Id={ticket.Id})");

            bool added;
            int count;
            lock (_gate)
            {
                // Check-and-add under ONE lock: two threads can never both see "not there" and both insert.
                added = !_tickets.ContainsKey(ticket.Id);
                if (added)
                    _tickets[ticket.Id] = Clone(ticket);
                count = _tickets.Count;
            }

            if (added)
                _log.Info(LogLayer.Data, "InMemoryTicketRepository.InsertAsync", $"#{ticket.Id} written ({count} rows)");
            else
                _log.Warn(LogLayer.Data, "InMemoryTicketRepository.InsertAsync", $"#{ticket.Id} already exists — not written");

            return Task.FromResult(added);
        }

        public Task<int> CountAsync()
        {
            EnsureAvailable("SELECT COUNT(*) FROM Tickets");
            return Task.FromResult(Count);
        }

        public Task ResetAsync()
        {
            EnsureAvailable("TRUNCATE TABLE Tickets");
            lock (_gate)
            {
                _tickets.Clear();
                Seed();
            }
            _log.Info(LogLayer.Data, "InMemoryTicketRepository.ResetAsync", $"store reset → {Count} seeded tickets");
            return Task.CompletedTask;
        }

        private void Seed()
        {
            lock (_gate)
            {
                foreach (var t in SeedData.Tickets())
                    _tickets[t.Id] = t;
            }
        }

        private void EnsureAvailable(string statement)
        {
            if (!_simulateOutage)
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
