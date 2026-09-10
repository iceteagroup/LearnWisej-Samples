using System.Collections.Generic;

namespace TicketOps.Security
{
    /// <summary>
    /// Answers "can this user do X?" from the user's roles. Two callers, two purposes:
    /// the screen asks it to decide what to SHOW (a courtesy for honest users),
    /// the service asks it to decide what to ALLOW (the actual security control).
    /// The same answer, but only the second one is a boundary.
    /// </summary>
    public interface IPermissionService
    {
        /// <summary>True when the user holds a role that grants the permission. Null (not signed in) is always false.</summary>
        bool Can(IUserContext user, Permission permission);

        /// <summary>Every permission the user holds — the screen uses it to enable/disable controls.</summary>
        IReadOnlyList<Permission> PermissionsOf(IUserContext user);

        /// <summary>The roles that grant a permission — used in denial messages and the permission matrix.</summary>
        IReadOnlyList<string> RolesGranting(Permission permission);
    }
}
