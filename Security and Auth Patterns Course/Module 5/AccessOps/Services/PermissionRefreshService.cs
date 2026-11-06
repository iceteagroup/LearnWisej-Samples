using System;
using System.Collections.Concurrent;

namespace AccessOps.Services
{
    /// <summary>
    /// The admin console's "force re-resolve". Permissions are resolved once at sign-in, so a
    /// revoked permission would otherwise last until the next sign-in. An administrator flags an
    /// employee; each of that employee's sessions rebuilds its set at its next action.
    ///
    /// Process memory, like the throttle: one AccessOps server. A farm needs a shared store.
    /// </summary>
    public sealed class PermissionRefreshService
    {
        private readonly ConcurrentDictionary<int, DateTime> _requests = new();
        private readonly IAuthorize _authorize;
        private readonly IAccessOpsSession _session;
        private readonly PermissionService _permissions;
        private readonly EmployeeRepository _employees;

        public PermissionRefreshService(IAuthorize authorize, IAccessOpsSession session,
            PermissionService permissions, EmployeeRepository employees)
        {
            _authorize = authorize;
            _session = session;
            _permissions = permissions;
            _employees = employees;
        }

        /// <summary>Flags an employee's sessions for re-resolution. Needs ManageUsers.</summary>
        public void Request(int employeeId)
        {
            _authorize.Require(Permission.ManageUsers, "employee " + employeeId);
            _requests[employeeId] = DateTime.UtcNow;
        }

        /// <summary>Rebuilds the current session's permission set when an administrator asked for it.</summary>
        public bool RefreshIfRequested()
        {
            var me = _session.TryGetCurrent();
            if (me == null || !_requests.TryGetValue(me.EmployeeId, out var requested) || requested <= me.PermissionsResolvedUtc)
                return false;

            var employee = _employees.Find(me.EmployeeId);
            _session.Replace(me.WithPermissions(_permissions.ResolveFor(employee), _employees.DirectReports(me.EmployeeId)));
            return true;
        }
    }
}
