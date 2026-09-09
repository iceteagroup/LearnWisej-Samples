namespace EnterpriseOps.Security
{
    /// <summary>
    /// Every server-side command the palette can trigger maps to exactly one permission.
    /// The names are what the trace and the audit log print.
    /// </summary>
    public enum Permission
    {
        WorkOrderApprove,
        WorkOrderReassign,
        WorkOrderEscalate,
        WorkQueueOpen,
        ImportCreate,
        DiagnosticsView,
    }

    public static class PermissionNames
    {
        public static string Of(Permission permission)
        {
            switch (permission)
            {
                case Permission.WorkOrderApprove: return "WorkOrder.Approve";
                case Permission.WorkOrderReassign: return "WorkOrder.Reassign";
                case Permission.WorkOrderEscalate: return "WorkOrder.Escalate";
                case Permission.WorkQueueOpen: return "WorkQueue.Open";
                case Permission.ImportCreate: return "Import.Create";
                case Permission.DiagnosticsView: return "Diagnostics.View";
                default: return permission.ToString();
            }
        }
    }
}
