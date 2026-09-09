using System.Collections.Generic;

namespace EnterpriseOps.Security
{
    public enum Role { Technician, Manager, Admin }

    /// <summary>The operations the Command Center authorises. Checked in services — never in a handler, never on the client.</summary>
    public enum Permission
    {
        ViewWorkQueue,
        ApproveWorkOrder,
        RunDiagnostics,
        ReviewGeneratedCode,     // sign a generated-code review (reviewer, not author)
        VerifyCapstonePackage,
    }

    /// <summary>Who is acting. Lives in the per-session SessionContext, never in a static field.</summary>
    public sealed class UserIdentity
    {
        public string Name { get; set; }
        public Role Role { get; set; }

        public override string ToString() => $"{Name} ({Role})";
    }

    /// <summary>The three course users (same in every module).</summary>
    public static class KnownUsers
    {
        public static UserIdentity AnaOps => new UserIdentity { Name = "ana.ops", Role = Role.Manager };
        public static UserIdentity BenTech => new UserIdentity { Name = "ben.tech", Role = Role.Technician };
        public static UserIdentity CaraAdmin => new UserIdentity { Name = "cara.admin", Role = Role.Admin };

        public static UserIdentity ByName(string name)
        {
            switch (name)
            {
                case "ben.tech": return BenTech;
                case "cara.admin": return CaraAdmin;
                default: return AnaOps;
            }
        }
    }

    /// <summary>
    /// The permission matrix (docs/DocumentationIndex.md → PermissionMatrix). Immutable data, so a
    /// static readonly field is fine — the checklist objects to static *state*, not static constants.
    /// </summary>
    public sealed class PermissionService
    {
        private static readonly Dictionary<Role, HashSet<Permission>> Matrix = new Dictionary<Role, HashSet<Permission>>
        {
            [Role.Technician] = new HashSet<Permission> { Permission.ViewWorkQueue },
            [Role.Manager] = new HashSet<Permission> { Permission.ViewWorkQueue, Permission.ApproveWorkOrder, Permission.RunDiagnostics, Permission.ReviewGeneratedCode, Permission.VerifyCapstonePackage },
            [Role.Admin] = new HashSet<Permission> { Permission.ViewWorkQueue, Permission.ApproveWorkOrder, Permission.RunDiagnostics, Permission.ReviewGeneratedCode, Permission.VerifyCapstonePackage },
        };

        public bool IsGranted(UserIdentity user, Permission permission)
        {
            return user != null && Matrix.TryGetValue(user.Role, out var granted) && granted.Contains(permission);
        }

        /// <summary>Returns null when allowed, otherwise the reason — services put it straight into CommandResult.Errors.</summary>
        public string Check(UserIdentity user, Permission permission)
        {
            if (IsGranted(user, permission))
                return null;
            return $"{user?.Name ?? "anonymous"} ({user?.Role.ToString() ?? "no role"}) is not allowed to {Describe(permission)}";
        }

        private static string Describe(Permission p)
        {
            switch (p)
            {
                case Permission.ApproveWorkOrder: return "approve work orders";
                case Permission.RunDiagnostics: return "run diagnostics";
                case Permission.ReviewGeneratedCode: return "sign a generated-code review";
                case Permission.VerifyCapstonePackage: return "verify the capstone package";
                default: return "view the work queue";
            }
        }
    }
}
