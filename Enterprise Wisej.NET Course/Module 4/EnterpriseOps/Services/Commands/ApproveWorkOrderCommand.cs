namespace EnterpriseOps.Services.Commands
{
    /// <summary>
    /// Intent as data: everything the Approve operation needs — id, tenant, user, comment and the version
    /// the user was looking at. A plain object, so the handler can be tested with an in-memory database
    /// and no Form at all.
    /// The walkthrough's record uses a Guid id and a byte[] row version; the course domain keeps the int
    /// <c>Id</c> and int <c>Version</c> token from the cookbook, so this command does too.
    /// </summary>
    public sealed record ApproveWorkOrderCommand(
        int WorkOrderId,
        string TenantId,
        string UserId,
        string Comment,
        int ExpectedVersion);
}
