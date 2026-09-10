using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Diagnostics;

namespace EnterpriseOps.Security
{
    /// <summary>How a cell of the permission matrix reads. The lab's matrix has all three kinds.</summary>
    public enum Grant
    {
        /// <summary>The role never gets this permission.</summary>
        Denied,

        /// <summary>The role always gets this permission (inside its own tenant).</summary>
        Granted,

        /// <summary>
        /// The role gets it, but a rule outside the matrix still applies — here: an export above the row
        /// threshold needs a second person's <see cref="Permission.ApproveExport"/>. The store answers "yes";
        /// the service that executes the command enforces the condition and audits it.
        /// </summary>
        Conditional
    }

    /// <summary>
    /// Roles → permissions (the matrix) and users → roles per tenant (membership).
    ///
    /// Two tables, on purpose. The matrix is a product decision that a security reviewer reads and signs off;
    /// membership is an operational fact that changes every week. Keeping them apart is what lets the matrix be
    /// written **before** the code, and what makes "who may approve?" answerable without reading a service.
    ///
    /// Membership is keyed by user **and tenant**: the same person may be a dispatcher in one tenant and a
    /// read-only auditor in another, and holding a role in tenant A must grant nothing in tenant B.
    /// </summary>
    public sealed class RolePermissionStore : IRolePermissionStore
    {
        /// <summary>
        /// The permission matrix. Rows are permissions, columns are roles — the artifact in
        /// <c>docs/PermissionMatrix.md</c>, in code so the two can never drift apart.
        /// </summary>
        private static readonly Dictionary<Role, Dictionary<Permission, Grant>> Matrix = BuildMatrix();

        private readonly ActivityTrace _trace;

        /// <summary>user id + tenant id → the roles held there. Populated from verified claims at sign-in.</summary>
        private readonly Dictionary<string, List<Role>> _membership = new Dictionary<string, List<Role>>(StringComparer.OrdinalIgnoreCase);

        public RolePermissionStore(ActivityTrace trace)
        {
            _trace = trace;
        }

        /// <summary>
        /// Records what the mapped claims said this user holds in this tenant. Called once per sign-in and again
        /// after <see cref="SessionContext.RefreshPermissions"/>, so a role change lands here without a new login.
        /// </summary>
        public void SetMembership(string userId, string tenantId, IEnumerable<Role> roles)
        {
            var list = (roles ?? Enumerable.Empty<Role>()).Distinct().ToList();
            _membership[Key(userId, tenantId)] = list;
            _trace?.Security($"RolePermissionStore — membership {userId}@{tenantId} = {(list.Count == 0 ? "(none)" : string.Join("+", list))}");
        }

        /// <summary>The lesson's signature: user, tenant, permission — three facts, one boolean.</summary>
        public bool UserHasPermission(string userId, string tenantId, Permission permission)
        {
            if (!_membership.TryGetValue(Key(userId, tenantId), out List<Role> roles) || roles.Count == 0)
                return false;

            return roles.Any(role => GrantFor(role, permission) != Grant.Denied);
        }

        /// <summary>The roles this user holds in this tenant — for the screen and the audit detail, not for a decision.</summary>
        public IReadOnlyList<Role> RolesFor(string userId, string tenantId)
            => _membership.TryGetValue(Key(userId, tenantId), out List<Role> roles) ? roles : (IReadOnlyList<Role>)Array.Empty<Role>();

        public static Grant GrantFor(Role role, Permission permission)
            => Matrix.TryGetValue(role, out Dictionary<Permission, Grant> row) && row.TryGetValue(permission, out Grant grant)
                ? grant
                : Grant.Denied;

        /// <summary>Every permission the role gets, for the sign-in screen's "this is what you will be able to do".</summary>
        public static IReadOnlyList<Permission> PermissionsFor(Role role)
            => AllPermissions.Where(p => GrantFor(role, p) != Grant.Denied).ToList();

        public static IReadOnlyList<Permission> AllPermissions
            => Enum.GetValues(typeof(Permission)).Cast<Permission>().ToList();

        public static IReadOnlyList<Role> AllRoles
            => Enum.GetValues(typeof(Role)).Cast<Role>().ToList();

        private static string Key(string userId, string tenantId) => $"{userId}@{tenantId}";

        private static Dictionary<Role, Dictionary<Permission, Grant>> BuildMatrix()
        {
            Dictionary<Permission, Grant> Row(params (Permission Permission, Grant Grant)[] cells)
            {
                var row = AllPermissions.ToDictionary(p => p, _ => Grant.Denied);
                foreach (var cell in cells) row[cell.Permission] = cell.Grant;
                return row;
            }

            return new Dictionary<Role, Dictionary<Permission, Grant>>
            {
                // Field engineer: works the queue of their own tenant. No approvals, no exports, no audit log.
                [Role.Technician] = Row(
                    (Permission.ViewWorkOrders, Grant.Granted),
                    (Permission.EditWorkOrders, Grant.Granted)),

                // Dispatcher: approves work, and may request an export — large ones need a second person.
                [Role.Manager] = Row(
                    (Permission.ViewWorkOrders, Grant.Granted),
                    (Permission.EditWorkOrders, Grant.Granted),
                    (Permission.ApproveWorkOrders, Grant.Granted),
                    (Permission.ExportData, Grant.Conditional),
                    (Permission.ViewAuditLog, Grant.Granted)),

                // Tenant administrator: the second pair of eyes, plus diagnostics.
                [Role.Admin] = Row(
                    (Permission.ViewWorkOrders, Grant.Granted),
                    (Permission.EditWorkOrders, Grant.Granted),
                    (Permission.ApproveWorkOrders, Grant.Granted),
                    (Permission.ExportData, Grant.Conditional),
                    (Permission.AdminDiagnostics, Grant.Granted),
                    (Permission.ViewAuditLog, Grant.Granted),
                    (Permission.ApproveExport, Grant.Granted)),

                // Compliance auditor: reads the queue and the trail, changes nothing, exports nothing —
                // the walkthrough's "j.kim · ExportData · DENIED" row. They *may* release someone else's
                // export, which is the point: the person who signs a bulk data movement off is deliberately
                // not a person who can ask for one.
                [Role.Auditor] = Row(
                    (Permission.ViewWorkOrders, Grant.Granted),
                    (Permission.ViewAuditLog, Grant.Granted),
                    (Permission.ApproveExport, Grant.Granted)),

                // The nightly import: edits, and that is all. A service account with ExportData is a data breach
                // waiting for a mis-configured job.
                [Role.ServiceAccount] = Row(
                    (Permission.ViewWorkOrders, Grant.Granted),
                    (Permission.EditWorkOrders, Grant.Granted)),
            };
        }
    }
}
