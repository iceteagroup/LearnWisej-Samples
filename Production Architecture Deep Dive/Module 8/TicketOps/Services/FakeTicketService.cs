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
    /// build one with <c>new FakeTicketService(log)</c> and no container.
    /// </summary>
    public sealed class FakeTicketService : ITicketService
    {
        private readonly TicketTable _table = new TicketTable();
        private readonly ILog _log;

        public FakeTicketService(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public Task<IReadOnlyList<Ticket>> GetOpenTicketsAsync()
        {
            IReadOnlyList<Ticket> open = _table.All()
                                               .Where(t => t.Status != TicketStatus.Closed)
                                               .OrderByDescending(t => t.Priority)
                                               .ThenBy(t => t.Id)
                                               .ToList();
            return Task.FromResult(open);
        }

        public Task<Ticket> FindAsync(int ticketId)
        {
            return Task.FromResult(_table.Find(ticketId));
        }

        public Task<OperationResult<Ticket>> CloseAsync(int ticketId, string reason)
        {
            var ticket = _table.Find(ticketId);
            if (ticket == null)
                return Task.FromResult(OperationResult<Ticket>.Fail("The ticket no longer exists. Refresh the list."));

            if (!ticket.CanClose(out string why))
            {
                _log.Warn(LogLayer.Domain, "Ticket.CanClose", $"#{ticketId} rejected: {why}");
                return Task.FromResult(OperationResult<Ticket>.Fail(why));
            }

            ticket.Close(reason);
            var saved = _table.Save(ticket);
            return Task.FromResult(OperationResult<Ticket>.Ok(saved, $"Ticket #{ticketId} closed."));
        }

        public Task<OperationResult<Ticket>> AssignAsync(int ticketId, int operatorId)
        {
            var ticket = _table.Find(ticketId);
            if (ticket == null)
                return Task.FromResult(OperationResult<Ticket>.Fail("The ticket no longer exists. Refresh the list."));
            if (ticket.Status == TicketStatus.Closed)
                return Task.FromResult(OperationResult<Ticket>.Fail("A closed ticket cannot be reassigned."));

            ticket.AssignTo(operatorId);
            var saved = _table.Save(ticket);
            return Task.FromResult(OperationResult<Ticket>.Ok(saved, $"Ticket #{ticketId} assigned."));
        }
    }
}
