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
    /// Production profile: the same contract, real persistence. This lab build is a stand-in — it has
    /// no database, so it logs the T-SQL it would execute and applies the change to a local
    /// <see cref="TicketTable"/> so the screen keeps working. The screen, the presenter and the tests
    /// cannot tell it from <see cref="FakeTicketService"/>.
    /// </summary>
    public sealed class SqlTicketService : ITicketService
    {
        private readonly TicketTable _standIn = new TicketTable();
        private readonly ILog _log;

        public SqlTicketService(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public Task<IReadOnlyList<Ticket>> GetOpenTicketsAsync()
        {
            Execute("SqlTicketService.GetOpenTicketsAsync", "SELECT Id, Title, Priority, Status, HoursLogged, AssigneeId FROM dbo.Tickets WHERE Status <> 'Closed' ORDER BY Priority DESC, Id");
            IReadOnlyList<Ticket> open = _standIn.All()
                .Where(t => t.Status != TicketStatus.Closed)
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.Id)
                .ToList();
            return Task.FromResult(open);
        }

        public Task<Ticket> FindAsync(int ticketId)
        {
            Execute("SqlTicketService.FindAsync", $"SELECT * FROM dbo.Tickets WHERE Id = @id  (@id = {ticketId})");
            return Task.FromResult(_standIn.Find(ticketId));
        }

        public Task<OperationResult<Ticket>> CloseAsync(int ticketId, string reason)
        {
            var ticket = _standIn.Find(ticketId);
            if (ticket == null)
                return Task.FromResult(OperationResult<Ticket>.Fail("The ticket no longer exists. Refresh the list."));

            if (!ticket.CanClose(out string why))
            {
                _log.Warn(LogLayer.Domain, "Ticket.CanClose", $"#{ticketId} rejected: {why}");
                return Task.FromResult(OperationResult<Ticket>.Fail(why));
            }

            ticket.Close(reason);
            Execute("SqlTicketService.CloseAsync", $"UPDATE dbo.Tickets SET Status = 'Closed', ClosedAt = SYSDATETIME(), CloseReason = @reason WHERE Id = {ticketId}");
            var saved = _standIn.Save(ticket);
            return Task.FromResult(OperationResult<Ticket>.Ok(saved, $"Ticket #{ticketId} closed."));
        }

        public Task<OperationResult<Ticket>> AssignAsync(int ticketId, int operatorId)
        {
            var ticket = _standIn.Find(ticketId);
            if (ticket == null)
                return Task.FromResult(OperationResult<Ticket>.Fail("The ticket no longer exists. Refresh the list."));
            if (ticket.Status == TicketStatus.Closed)
                return Task.FromResult(OperationResult<Ticket>.Fail("A closed ticket cannot be reassigned."));

            ticket.AssignTo(operatorId);
            Execute("SqlTicketService.AssignAsync", $"UPDATE dbo.Tickets SET AssigneeId = {operatorId}, Status = '{ticket.Status}' WHERE Id = {ticketId}");
            var saved = _standIn.Save(ticket);
            return Task.FromResult(OperationResult<Ticket>.Ok(saved, $"Ticket #{ticketId} assigned."));
        }

        /// <summary>The stand-in's "execute": log the statement a real driver would send.</summary>
        private void Execute(string source, string sql)
        {
            _log.Info(LogLayer.Data, source, "would run: " + sql);
        }
    }
}
