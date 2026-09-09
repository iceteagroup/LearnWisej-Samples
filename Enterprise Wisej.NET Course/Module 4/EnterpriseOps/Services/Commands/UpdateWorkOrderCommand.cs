using EnterpriseOps.Domain;

namespace EnterpriseOps.Services.Commands
{
    /// <summary>
    /// Input for Update. <c>Status</c> is optional (null = unchanged) and may not be Completed —
    /// that edge belongs to Approve. <c>ExpectedVersion</c> is the optimistic-concurrency check.
    /// </summary>
    public sealed record UpdateWorkOrderCommand(
        int WorkOrderId,
        string TenantId,
        string UserId,
        string Title,
        string Customer,
        string Site,
        Priority Priority,
        WorkOrderStatus? Status,
        int ExpectedVersion);
}
