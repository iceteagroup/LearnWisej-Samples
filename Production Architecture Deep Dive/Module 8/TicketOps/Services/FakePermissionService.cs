using System;
using TicketOps.Domain;
using TicketOps.Infrastructure;

namespace TicketOps.Services
{
    /// <summary>
    /// Fake profile: the role rules in their simplest form, plus a switch a test can flip.
    /// That switch (<see cref="DenyEverything"/>) is what makes a fake more useful than the real thing in a
    /// test — the presenter's "denied" branch can be reached without arranging roles and tickets.
    /// Rules: a Viewer may do nothing; a Technician may close and take tickets assigned to them (or
    /// unassigned ones); a Manager may do everything.
    /// </summary>
    public sealed class FakePermissionService : IPermissionService
    {
        private readonly IUserService _users;
        private readonly ILog _log;

        public FakePermissionService(IUserService users, ILog log)
        {
            _users = users ?? throw new ArgumentNullException(nameof(users));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        /// <summary>Test steering: when true every check answers "no".</summary>
        public bool DenyEverything { get; set; }

        public bool CanClose(Ticket ticket)
        {
            var user = _users.Current;
            bool allowed = !DenyEverything && (user.Role == OperatorRole.Manager
                || (user.Role == OperatorRole.Technician && (ticket.AssigneeId == user.Id || !ticket.IsAssigned)));
            _log.Info(LogLayer.Service, "FakePermissionService.CanClose",
                $"{user} on #{ticket.Id} → {(allowed ? "allowed" : "denied")}{(DenyEverything ? " (DenyEverything)" : "")}");
            return allowed;
        }

        public bool CanAssign(Ticket ticket, int toOperatorId)
        {
            var user = _users.Current;
            bool allowed = !DenyEverything && (user.Role == OperatorRole.Manager
                || (user.Role == OperatorRole.Technician && toOperatorId == user.Id));
            _log.Info(LogLayer.Service, "FakePermissionService.CanAssign",
                $"{user} assigning #{ticket.Id} to operator {toOperatorId} → {(allowed ? "allowed" : "denied")}");
            return allowed;
        }
    }
}
