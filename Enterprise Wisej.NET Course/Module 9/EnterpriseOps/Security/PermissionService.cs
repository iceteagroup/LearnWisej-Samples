using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Services;

namespace EnterpriseOps.Security
{
    public sealed class PermissionDecision
    {
        public bool Allowed { get; set; }
        public Permission Permission { get; set; }
        public Role Role { get; set; }
        public string UserName { get; set; }
        public string Reason { get; set; }
    }

    public interface IPermissionService
    {
        /// <summary>Decides from the SESSION identity only. Nothing from the request payload is consulted.</summary>
        PermissionDecision Check(SessionContext session, Permission permission);
    }

    /// <summary>
    /// The role → permission matrix. Small on purpose: the point of Module 9 is *where* it runs
    /// (on the server, before the command), not how rich it is. The audit and the trace print
    /// the decision so the reviewer can see that the palette suggested and the server decided.
    /// </summary>
    public sealed class PermissionService : IPermissionService
    {
        private static readonly Dictionary<Role, Permission[]> Matrix = new Dictionary<Role, Permission[]>
        {
            [Role.Technician] = new[] { Permission.WorkQueueOpen, Permission.WorkOrderEscalate },
            [Role.Manager]    = new[] { Permission.WorkQueueOpen, Permission.WorkOrderEscalate, Permission.WorkOrderApprove, Permission.WorkOrderReassign, Permission.ImportCreate },
            [Role.Admin]      = new[] { Permission.WorkQueueOpen, Permission.WorkOrderEscalate, Permission.WorkOrderApprove, Permission.WorkOrderReassign, Permission.ImportCreate, Permission.DiagnosticsView },
        };

        private readonly ActivityTrace _trace;

        public PermissionService(ActivityTrace trace)
        {
            _trace = trace;
        }

        public PermissionDecision Check(SessionContext session, Permission permission)
        {
            string userName = session.UserName;
            Role role = session.Role;
            bool allowed = Matrix.TryGetValue(role, out Permission[] granted) && granted.Contains(permission);
            string name = PermissionNames.Of(permission);
            string requires = string.Join(" or ", Matrix.Where(kv => kv.Value.Contains(permission)).Select(kv => kv.Key.ToString()));
            var decision = new PermissionDecision
            {
                Allowed = allowed,
                Permission = permission,
                Role = role,
                UserName = userName,
                Reason = allowed ? $"{role} holds {name}" : $"{name} requires {requires}; {userName} is {role}",
            };

            _trace.Security($"PermissionService {userName} ({role}, role from session) → {name} → {(allowed ? "ALLOWED" : "DENIED")} · {decision.Reason}");
            return decision;
        }

        public static IEnumerable<Permission> PermissionsOf(Role role)
            => Matrix.TryGetValue(role, out Permission[] granted) ? granted : new Permission[0];
    }
}
