using System;

namespace EnterpriseOps.Security
{
    public enum Role
    {
        Technician,
        Manager,
        Admin
    }

    public enum Operation
    {
        Create,
        Update,
        Approve,
        ReadAudit
    }

    /// <summary>The three course users. Roles are looked up here; the UI only shows names.</summary>
    public static class KnownUsers
    {
        public const string Manager = "ana.ops";
        public const string Technician = "ben.tech";
        public const string Admin = "cara.admin";

        public static readonly string[] All = { Manager, Technician, Admin };

        public static Role RoleOf(string userId)
        {
            switch (userId)
            {
                case Manager: return Role.Manager;
                case Admin: return Role.Admin;
                case Technician: return Role.Technician;
                default: throw new ArgumentException($"Unknown user '{userId}'.", nameof(userId));
            }
        }
    }

    /// <summary>
    /// Server-side authorization, checked by the command service before any transaction opens.
    /// A disabled button is a courtesy; this is the rule. Returns a reason instead of throwing so the
    /// service can map it to PERMISSION_DENIED and audit it.
    /// </summary>
    public static class Permissions
    {
        public static bool IsAllowed(Role role, Operation operation, out string reason)
        {
            reason = null;
            switch (operation)
            {
                case Operation.Approve:
                    if (role == Role.Manager || role == Role.Admin)
                        return true;
                    reason = "Only a manager or an administrator can approve a work order.";
                    return false;

                case Operation.ReadAudit:
                    if (role == Role.Manager || role == Role.Admin)
                        return true;
                    reason = "Only a manager or an administrator can read the audit log.";
                    return false;

                default:
                    return true;      // Create / Update: every signed-in user
            }
        }
    }
}
