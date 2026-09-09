using EnterpriseOps.Domain;

namespace EnterpriseOps.Security
{
    public enum UserRole { Technician, Manager, Admin }

    /// <summary>
    /// The one place that knows who may do what. The query service asks it once per projected row
    /// (so the grid can enable or disable the batch button) and the batch workflow asks it again per row
    /// before writing (so a forged or stale flag from the browser can never authorize a change).
    /// </summary>
    public sealed class PermissionService
    {
        public bool CanReassign(UserRole role, WorkOrder order)
        {
            if (!order.IsOpen)
                return false;
            return role == UserRole.Manager || role == UserRole.Admin;
        }

        public bool CanApprove(UserRole role, WorkOrder order)
        {
            return order.OpenApprovalId != null && (role == UserRole.Manager || role == UserRole.Admin);
        }

        public static UserRole RoleOf(string userName)
        {
            switch (userName)
            {
                case "ana.ops": return UserRole.Manager;
                case "cara.admin": return UserRole.Admin;
                default: return UserRole.Technician;
            }
        }
    }
}
