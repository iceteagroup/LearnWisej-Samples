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
    /// In-memory repository seeded with the tickets and the activity feed the Module 3 walkthrough shows.
    /// One instance per session (created in AppComposition), so two browser tabs never share a list — the
    /// same reason nothing here is static. <see cref="SimulateOutage"/> lets the lab show the error path
    /// without a real database.
    /// </summary>
    public sealed class InMemoryTicketRepository : ITicketRepository
    {
        private readonly ILog _log;
        private readonly Dictionary<int, Ticket> _tickets = new Dictionary<int, Ticket>();
        private readonly List<TicketEvent> _events = new List<TicketEvent>();
        private int _nextTicketId;
        private int _nextEventId;

        public bool SimulateOutage { get; set; }

        public InMemoryTicketRepository(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            foreach (var t in SeedData.Tickets())
                _tickets[t.Id] = t;
            foreach (var e in SeedData.Events())
                _events.Add(e);
            _nextTicketId = _tickets.Keys.Max() + 1;
            _nextEventId = _events.Max(e => e.Id) + 1;
            _log.Info(LogLayer.Data, "InMemoryTicketRepository", $"seeded {_tickets.Count} tickets, {_events.Count} activity events (in-memory, per session)");
        }

        public Task<IReadOnlyList<Ticket>> GetAllAsync()
        {
            EnsureAvailable("SELECT * FROM Tickets");
            _log.Info(LogLayer.Data, "InMemoryTicketRepository.GetAllAsync", $"{_tickets.Count} rows");
            IReadOnlyList<Ticket> rows = _tickets.Values.Select(Clone).ToList();
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
            EnsureAvailable(ticket.Id == 0 ? "INSERT INTO Tickets" : $"UPDATE Tickets WHERE Id={ticket.Id}");

            if (ticket.Id == 0)
                ticket.Id = _nextTicketId++;

            _tickets[ticket.Id] = Clone(ticket);
            _log.Info(LogLayer.Data, "InMemoryTicketRepository.UpsertAsync", $"#{ticket.Id} written ({_tickets.Count} rows)");
            return Task.FromResult(Clone(ticket));
        }

        public Task<IReadOnlyList<TicketEvent>> GetEventsAsync()
        {
            EnsureAvailable("SELECT * FROM TicketEvents ORDER BY At DESC");
            IReadOnlyList<TicketEvent> rows = _events.OrderByDescending(e => e.At).Select(Clone).ToList();
            _log.Info(LogLayer.Data, "InMemoryTicketRepository.GetEventsAsync", $"{rows.Count} events");
            return Task.FromResult(rows);
        }

        public Task<TicketEvent> AppendEventAsync(TicketEvent ticketEvent)
        {
            if (ticketEvent == null) throw new ArgumentNullException(nameof(ticketEvent));
            EnsureAvailable("INSERT INTO TicketEvents");

            ticketEvent.Id = _nextEventId++;
            _events.Add(Clone(ticketEvent));
            _log.Info(LogLayer.Data, "InMemoryTicketRepository.AppendEventAsync", $"event #{ticketEvent.Id} for ticket #{ticketEvent.TicketId} written");
            return Task.FromResult(Clone(ticketEvent));
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
            Priority = t.Priority,
            Status = t.Status,
            Assignee = t.Assignee,
            HoursLogged = t.HoursLogged,
            Notes = t.Notes,
            IsEscalated = t.IsEscalated,
            CreatedAt = t.CreatedAt,
            ClosedAt = t.ClosedAt
        };

        private static TicketEvent Clone(TicketEvent e) => new TicketEvent
        {
            Id = e.Id,
            TicketId = e.TicketId,
            Text = e.Text,
            At = e.At
        };
    }

    /// <summary>The tickets and the activity feed the Module 3 walkthrough video shows in the Ticket Workspace.</summary>
    public static class SeedData
    {
        public static IEnumerable<Ticket> Tickets()
        {
            var now = DateTime.Now;
            yield return new Ticket { Id = 1001, Title = "Login page returns 500 error", Priority = TicketPriority.High, Assignee = "A. Rivera", HoursLogged = 0.5, Status = TicketStatus.InProgress, CreatedAt = now.AddDays(-2), Notes = "Reproduced on the staging cluster after the 3.2 deploy. Stack trace in the ops channel." };
            yield return new Ticket { Id = 1002, Title = "CSV export missing columns", Priority = TicketPriority.Medium, Assignee = "M. Chen", HoursLogged = 0, CreatedAt = now.AddDays(-1), Notes = "Assignee and Due columns are dropped when more than 50 tickets are exported." };
            yield return new Ticket { Id = 1003, Title = "Dashboard slow on Safari", Priority = TicketPriority.Medium, Assignee = "S. Patel", HoursLogged = 2.25, Status = TicketStatus.InProgress, CreatedAt = now.AddHours(-20), Notes = "Safari 17 renders the dashboard grid at ~4 fps when more than 200 rows are loaded." };
            yield return new Ticket { Id = 1004, Title = "Add dark theme toggle", Priority = TicketPriority.Low, Assignee = "J. Kim", HoursLogged = 0, CreatedAt = now.AddHours(-9), Notes = "Feature request from the night-shift team." };
            yield return new Ticket { Id = 1005, Title = "Password reset email delayed", Priority = TicketPriority.High, Assignee = "A. Rivera", HoursLogged = 1.0, IsEscalated = true, CreatedAt = now.AddHours(-5), Notes = "Escalated: resets take 20+ minutes to arrive during the morning peak." };
            yield return new Ticket { Id = 1006, Title = "Attachment preview fails on Edge", Priority = TicketPriority.Medium, Assignee = "M. Chen", HoursLogged = 0, CreatedAt = now.AddHours(-3), Notes = "PDF previews show a blank frame; download works." };
            yield return new Ticket { Id = 1007, Title = "Kiosk screen stuck on splash", Priority = TicketPriority.Low, Assignee = "", HoursLogged = 0, CreatedAt = now.AddMinutes(-40), Notes = "Reported by the lobby reception; unassigned." };
        }

        public static IEnumerable<TicketEvent> Events()
        {
            var now = DateTime.Now;
            yield return new TicketEvent { Id = 1, TicketId = 1003, Text = "S. Patel commented on #1003", At = now.AddMinutes(-2) };
            yield return new TicketEvent { Id = 2, TicketId = 1003, Text = "#1003 status changed to In Progress", At = now.AddMinutes(-18) };
            yield return new TicketEvent { Id = 3, TicketId = 1003, Text = "M. Chen attached safari-trace.har to #1003", At = now.AddHours(-1) };
            yield return new TicketEvent { Id = 4, TicketId = 1005, Text = "#1005 escalated to High", At = now.AddHours(-3) };
            yield return new TicketEvent { Id = 5, TicketId = 1001, Text = "A. Rivera logged 0.5 h on #1001", At = now.AddHours(-6) };
            yield return new TicketEvent { Id = 6, TicketId = 1007, Text = "#1007 created by reception (unassigned)", At = now.AddMinutes(-40) };
        }
    }
}
