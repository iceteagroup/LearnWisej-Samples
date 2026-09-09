namespace EnterpriseOps.Domain
{
    /// <summary>Lifecycle of a work order (shared vocabulary across every module of the course).</summary>
    public enum WorkOrderStatus { New, Assigned, InProgress, OnHold, Escalated, Completed, Cancelled }

    public enum Priority { Low, Normal, High, Critical }
}
