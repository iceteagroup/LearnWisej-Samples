using System;
using System.Collections.Generic;
using System.Linq;
using TicketOps.Infrastructure;

namespace TicketOps.Security
{
    /// <summary>
    /// Role → permission table. The table is constant data (the same for every session), which is the
    /// one kind of static this course allows; the USER is never stored here — callers pass it in.
    ///
    /// Least privilege: a Technician can look and annotate, a Supervisor can also close and delete,
    /// an Admin can additionally read the audit trail. Nothing is granted by default.
    /// </summary>
    public sealed class PermissionService : IPermissionService
    {
        private static readonly IReadOnlyDictionary<Permission, string[]> Grants = new Dictionary<Permission, string[]>
        {
            [Permission.ViewTickets] = new[] { Roles.Technician, Roles.Supervisor, Roles.Admin },
            [Permission.AddNote] = new[] { Roles.Technician, Roles.Supervisor, Roles.Admin },
            [Permission.CloseTicket] = new[] { Roles.Supervisor, Roles.Admin },
            [Permission.DeleteTicket] = new[] { Roles.Supervisor, Roles.Admin },
            [Permission.ViewAuditTrail] = new[] { Roles.Supervisor, Roles.Admin }
        };

        private readonly ILog _log;

        public PermissionService(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public bool Can(IUserContext user, Permission permission)
        {
            if (user == null)
            {
                _log.Warn(LogLayer.Service, "PermissionService.Can", $"{permission} → denied (not signed in)");
                return false;
            }

            var granting = RolesGranting(permission);
            bool allowed = granting.Any(user.IsInRole);
            if (allowed)
                _log.Info(LogLayer.Service, "PermissionService.Can", $"{user} {permission} → allowed");
            else
                _log.Warn(LogLayer.Service, "PermissionService.Can", $"{user} {permission} → denied (needs {string.Join(" or ", granting)})");
            return allowed;
        }

        public IReadOnlyList<Permission> PermissionsOf(IUserContext user)
        {
            if (user == null)
                return new Permission[0];
            return Grants.Where(g => g.Value.Any(user.IsInRole)).Select(g => g.Key).ToList();
        }

        public IReadOnlyList<string> RolesGranting(Permission permission)
            => Grants.TryGetValue(permission, out var roles) ? roles : new string[0];
    }
}
