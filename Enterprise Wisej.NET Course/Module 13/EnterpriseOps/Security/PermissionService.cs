using System;
using System.Collections.Generic;
using System.Linq;

namespace EnterpriseOps.Security
{
    /// <summary>
    /// The permission names the field workflow needs. Kept as constants so the queue replay and the UI
    /// name the same thing.
    /// </summary>
    public static class Permissions
    {
        public const string CompleteWorkOrder = "workorder.complete";
        public const string OverrideConflict = "workorder.override";   // Manager/Admin: apply a local change over a server change
        public const string CancelWorkOrder = "workorder.cancel";      // dispatcher
    }

    /// <summary>
    /// What the device holds: a snapshot of the user's permissions with the time it was issued.
    /// Offline, the screen can only consult this snapshot; on reconnect it is replaced, never merged.
    /// </summary>
    public class CachedPermissionSet
    {
        public string User;
        public string Role;
        public IReadOnlyCollection<string> Permissions;
        public DateTime IssuedUtc;

        public bool Has(string permission) => Permissions != null && Permissions.Contains(permission);

        public bool IsStale(TimeSpan maxAge) => DateTime.UtcNow - IssuedUtc > maxAge;

        public override string ToString()
            => $"{User} ({Role}) · {string.Join(", ", Permissions ?? Array.Empty<string>())} · issued {IssuedUtc:HH:mm:ss}Z";
    }

    /// <summary>
    /// Server-side identity/permission store (fake). The important production behaviour it reproduces:
    /// permissions can change while a device is offline, so the sync boundary asks this service again
    /// for every queued command instead of trusting the device's cached set.
    /// </summary>
    public class PermissionService
    {
        private readonly object _gate = new object();
        private readonly Dictionary<string, string> _roles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["ana.ops"] = "Manager",
            ["ben.tech"] = "Technician",
            ["cara.admin"] = "Admin",
        };
        private readonly Dictionary<string, HashSet<string>> _grants = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["ana.ops"] = new HashSet<string> { Permissions.CompleteWorkOrder, Permissions.OverrideConflict, Permissions.CancelWorkOrder },
            ["ben.tech"] = new HashSet<string> { Permissions.CompleteWorkOrder },
            ["cara.admin"] = new HashSet<string> { Permissions.CompleteWorkOrder, Permissions.OverrideConflict, Permissions.CancelWorkOrder },
        };

        /// <summary>The server's current answer — what the sync boundary re-evaluates against.</summary>
        public bool IsGranted(string user, string permission)
        {
            lock (_gate)
                return _grants.TryGetValue(user, out var set) && set.Contains(permission);
        }

        public string RoleOf(string user)
        {
            lock (_gate)
                return _roles.TryGetValue(user, out var role) ? role : "Unknown";
        }

        /// <summary>Issues a fresh snapshot for the device. Called at login and on every reconnect.</summary>
        public CachedPermissionSet Issue(string user)
        {
            lock (_gate)
            {
                return new CachedPermissionSet
                {
                    User = user,
                    Role = RoleOf(user),
                    Permissions = _grants.TryGetValue(user, out var set) ? set.ToArray() : Array.Empty<string>(),
                    IssuedUtc = DateTime.UtcNow,
                };
            }
        }

        /// <summary>Failure-path helper: an admin revokes a permission while the technician is offline.</summary>
        public void Revoke(string user, string permission)
        {
            lock (_gate)
                if (_grants.TryGetValue(user, out var set)) set.Remove(permission);
        }

        public void Grant(string user, string permission)
        {
            lock (_gate)
            {
                if (!_grants.TryGetValue(user, out var set))
                    _grants[user] = set = new HashSet<string>();
                set.Add(permission);
            }
        }
    }
}
