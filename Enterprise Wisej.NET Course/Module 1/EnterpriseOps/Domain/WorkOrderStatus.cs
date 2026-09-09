namespace EnterpriseOps.Domain
{
    /// <summary>
    /// Lifecycle of a work order. Shared by every module of the course — do not rename members,
    /// later modules (work queue, escalation wizard, offline sync) depend on them.
    /// </summary>
    public enum WorkOrderStatus
    {
        New,
        Assigned,
        InProgress,
        OnHold,
        Escalated,
        Completed,
        Cancelled
    }
}
