namespace EnterpriseOps.Domain
{
    /// <summary>The shared EnterpriseOps status vocabulary (same in every module).</summary>
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
