using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Domain;
using TicketOps.Infrastructure;

namespace TicketOps.Services
{
    /// <summary>
    /// The screen's decisions, moved out of the Form. Plain C#: it takes its five services (and the log)
    /// by constructor, exposes methods the Form calls, and returns a <see cref="WorkflowResult"/> the Form
    /// renders. No control, no session, no browser — which is why Diagnostics/PresenterTestRunner can
    /// drive it with fakes, and why the same object works unchanged behind the fake and the production
    /// registration profile.
    ///
    /// Order of a workflow: input check → does the ticket exist → may this operator → domain rule
    /// (inside the ticket service) → audit → notify. Every step that says "no" is an outcome, not an exception.
    /// </summary>
    public sealed class TicketWorkflowPresenter
    {
        private readonly ITicketService _tickets;
        private readonly IUserService _users;
        private readonly IPermissionService _permissions;
        private readonly INotificationService _notifications;
        private readonly IAuditLogService _audit;
        private readonly ILog _log;

        public TicketWorkflowPresenter(
            ITicketService tickets,
            IUserService users,
            IPermissionService permissions,
            INotificationService notifications,
            IAuditLogService audit,
            ILog log)
        {
            _tickets = tickets ?? throw new ArgumentNullException(nameof(tickets));
            _users = users ?? throw new ArgumentNullException(nameof(users));
            _permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
            _notifications = notifications ?? throw new ArgumentNullException(nameof(notifications));
            _audit = audit ?? throw new ArgumentNullException(nameof(audit));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public Operator CurrentOperator => _users.Current;

        public Task<IReadOnlyList<Ticket>> LoadOpenTicketsAsync()
        {
            _log.Info(LogLayer.Service, "TicketWorkflowPresenter.LoadOpenTicketsAsync", "→ ITicketService.GetOpenTicketsAsync()");
            return _tickets.GetOpenTicketsAsync();
        }

        public async Task<WorkflowResult> CloseAsync(int ticketId, string reason)
        {
            var user = _users.Current;
            _log.Info(LogLayer.Service, "TicketWorkflowPresenter.CloseAsync", $"#{ticketId} as {user}");

            if (string.IsNullOrWhiteSpace(reason))
            {
                _log.Warn(LogLayer.Service, "TicketWorkflowPresenter.CloseAsync", "rejected before any service call: no reason given");
                return WorkflowResult.Invalid("Give a reason before closing.");
            }

            var ticket = await _tickets.FindAsync(ticketId);
            if (ticket == null)
            {
                _log.Warn(LogLayer.Service, "TicketWorkflowPresenter.CloseAsync", $"#{ticketId} not found");
                return WorkflowResult.NotFound($"Ticket #{ticketId} no longer exists. Refresh the list.");
            }

            if (!_permissions.CanClose(ticket))
            {
                _log.Warn(LogLayer.Service, "TicketWorkflowPresenter.CloseAsync", $"IPermissionService.CanClose(#{ticketId}) → denied for {user}");
                return WorkflowResult.Denied($"{user.Name} ({user.Role}) may not close ticket #{ticketId}.");
            }

            var closed = await _tickets.CloseAsync(ticketId, reason.Trim());
            if (!closed.Succeeded)
            {
                _log.Warn(LogLayer.Service, "TicketWorkflowPresenter.CloseAsync", $"ITicketService.CloseAsync said no: {closed.Message}");
                return WorkflowResult.Invalid(closed.Message);
            }

            var entry = _audit.Record("close", ticketId, user.Id, reason.Trim());
            _log.Info(LogLayer.Service, "TicketWorkflowPresenter.CloseAsync",
                $"IAuditLogService.Record(close, #{ticketId}, operator {user.Id}) → entry {entry.Sequence} of {_audit.Count} (Shared: one trail for every session)");

            int recipient = closed.Value.IsAssigned ? closed.Value.AssigneeId : user.Id;
            _notifications.Notify(recipient, $"Ticket #{ticketId} closed by {user.Name}: {reason.Trim()}");

            return WorkflowResult.Ok(closed.Value, $"Ticket #{ticketId} closed.");
        }

        public async Task<WorkflowResult> AssignToMeAsync(int ticketId)
        {
            var user = _users.Current;
            _log.Info(LogLayer.Service, "TicketWorkflowPresenter.AssignToMeAsync", $"#{ticketId} → {user}");

            var ticket = await _tickets.FindAsync(ticketId);
            if (ticket == null)
                return WorkflowResult.NotFound($"Ticket #{ticketId} no longer exists. Refresh the list.");

            if (ticket.AssigneeId == user.Id)
                return WorkflowResult.Invalid($"Ticket #{ticketId} is already assigned to you.");

            if (!_permissions.CanAssign(ticket, user.Id))
            {
                _log.Warn(LogLayer.Service, "TicketWorkflowPresenter.AssignToMeAsync", $"IPermissionService.CanAssign(#{ticketId}) → denied for {user}");
                return WorkflowResult.Denied($"{user.Name} ({user.Role}) may not take ticket #{ticketId}.");
            }

            var assigned = await _tickets.AssignAsync(ticketId, user.Id);
            if (!assigned.Succeeded)
                return WorkflowResult.Invalid(assigned.Message);

            var entry = _audit.Record("assign", ticketId, user.Id, $"to operator {user.Id}");
            _log.Info(LogLayer.Service, "TicketWorkflowPresenter.AssignToMeAsync",
                $"IAuditLogService.Record(assign, #{ticketId}, operator {user.Id}) → entry {entry.Sequence} of {_audit.Count}");

            _notifications.Notify(user.Id, $"Ticket #{ticketId} is now assigned to you.");

            return WorkflowResult.Ok(assigned.Value, $"Ticket #{ticketId} assigned to {user.Name}.");
        }
    }
}
