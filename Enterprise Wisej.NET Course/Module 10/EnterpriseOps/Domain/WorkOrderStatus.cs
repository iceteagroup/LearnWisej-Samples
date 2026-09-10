namespace EnterpriseOps.Domain
{
    /// <summary>The work order lifecycle shared by every module of the course.</summary>
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
