using System;
using System.Collections.Generic;
using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// Production profile: permissions evaluated from a role → action matrix (in a real deployment the
    /// matrix would be loaded from the policy store at startup). Same contract as the fake, no test switch.
    /// </summary>
    public sealed class RolePermissionService : IPermissionService
    {
        private const string CloseAction = "ticket.close";
        private const string AssignAnyAction = "ticket.assign";
        private const string AssignSelfAction = "ticket.assign-self";

        private static readonly IReadOnlyDictionary<OperatorRole, string[]> Matrix = new Dictionary<OperatorRole, string[]>
        {
            [OperatorRole.Viewer] = new string[0],
            [OperatorRole.Technician] = new[] { CloseAction, AssignSelfAction },
            [OperatorRole.Manager] = new[] { CloseAction, AssignAnyAction, AssignSelfAction }
        };

        private readonly IUserService _users;

        public RolePermissionService(IUserService users)
        {
            _users = users ?? throw new ArgumentNullException(nameof(users));
        }

        public bool CanClose(Ticket ticket)
        {
            var user = _users.Current;
            return Has(user.Role, CloseAction)
                && (user.Role == OperatorRole.Manager || ticket.AssigneeId == user.Id || !ticket.IsAssigned);
        }

        public bool CanAssign(Ticket ticket, int toOperatorId)
        {
            var user = _users.Current;
            return Has(user.Role, AssignAnyAction) || (Has(user.Role, AssignSelfAction) && toOperatorId == user.Id);
        }

        private static bool Has(OperatorRole role, string action) => Array.IndexOf(Matrix[role], action) >= 0;
    }
}
