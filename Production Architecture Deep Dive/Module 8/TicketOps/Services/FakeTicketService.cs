using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketOps.Data;
using TicketOps.Domain;
using TicketOps.Infrastructure;

namespace TicketOps.Services
{
    /// <summary>
    /// Fake profile: predictable in-memory data, no I/O. "Fake" describes the storage, not the rules —
    /// the close rule is <see cref="Ticket.CanClose"/>, the same one the production-shaped service applies.
    /// Constructor injection: everything this class needs is visible in its signature, so a test can
    /// build one with <c>new FakeTicketService(log, new DataStoreHealth())</c> and no container.
    /// </summary>
    public sealed class FakeTicketService : ITicketService
    {
        private readonly TicketTable _table = new TicketTable();
        private readonly DataStoreHealth _health;
        private readonly ILog _log;

        public FakeTicketService(ILog log, DataStoreHealth health)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _health = health ?? throw new ArgumentNullException(nameof(health));
            _log.Info(LogLayer.Data, "FakeTicketService", $"seeded {_table.Count} tickets (in-memory, one table per session)");
        }

        public Task<IReadOnlyList<Ticket>> GetOpenTicketsAsync()
        {
            _health.EnsureAvailable(_log, "FakeTicketService.GetOpenTicketsAsync", "read Tickets");
            var all = _table.All();
            IReadOnlyList<Ticket> open = all.Where(t => t.Status != TicketStatus.Closed)
                                            .OrderByDescending(t => t.Priority)
                                            .ThenBy(t => t.Id)
                                            .ToList();
            _log.Info(LogLayer.Data, "FakeTicketService.GetOpenTicketsAsync", $"{open.Count} open of {all.Count} rows (in-memory)");
            return Task.FromResult(open);
        }

        public Task<Ticket> FindAsync(int ticketId)
        {
            _health.EnsureAvailable(_log, "FakeTicketService.FindAsync", $"read Tickets #{ticketId}");
            var ticket = _table.Find(ticketId);
            _log.Info(LogLayer.Data, "FakeTicketService.FindAsync", ticket == null ? $"#{ticketId} not found" : $"#{ticketId} found");
            return Task.FromResult(ticket);
        }

        public Task<OperationResult<Ticket>> CloseAsync(int ticketId, string reason)
        {
            _health.EnsureAvailable(_log, "FakeTicketService.CloseAsync", $"update Tickets #{ticketId}");
            var ticket = _table.Find(ticketId);
            if (ticket == null)
                return Task.FromResult(OperationResult<Ticket>.Fail("The ticket no longer exists. Refresh the list."));

            if (!ticket.CanClose(out string why))
            {
                _log.Warn(LogLayer.Domain, "Ticket.CanClose", $"#{ticketId} rejected: {why}");
                return Task.FromResult(OperationResult<Ticket>.Fail(why));
            }

            ticket.Close(reason);
            _log.Info(LogLayer.Domain, "Ticket.Close", $"#{ticketId} status → Closed (\"{reason}\")");
            var saved = _table.Save(ticket);
            _log.Info(LogLayer.Data, "FakeTicketService.CloseAsync", $"#{ticketId} written to the in-memory table");
            return Task.FromResult(OperationResult<Ticket>.Ok(saved, $"Ticket #{ticketId} closed."));
        }

        public Task<OperationResult<Ticket>> AssignAsync(int ticketId, int operatorId)
        {
            _health.EnsureAvailable(_log, "FakeTicketService.AssignAsync", $"update Tickets #{ticketId}");
            var ticket = _table.Find(ticketId);
            if (ticket == null)
                return Task.FromResult(OperationResult<Ticket>.Fail("The ticket no longer exists. Refresh the list."));
            if (ticket.Status == TicketStatus.Closed)
                return Task.FromResult(OperationResult<Ticket>.Fail("A closed ticket cannot be reassigned."));

            ticket.AssignTo(operatorId);
            _log.Info(LogLayer.Domain, "Ticket.AssignTo", $"#{ticketId} assignee → operator {operatorId}");
            var saved = _table.Save(ticket);
            _log.Info(LogLayer.Data, "FakeTicketService.AssignAsync", $"#{ticketId} written to the in-memory table");
            return Task.FromResult(OperationResult<Ticket>.Ok(saved, $"Ticket #{ticketId} assigned."));
        }
    }
}
