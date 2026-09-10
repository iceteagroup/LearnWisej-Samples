using EnterpriseOps.Domain;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// A typed command — what the screen asks for, never an entity. It carries the tenant the client believed
    /// it was working in: the service does not <b>trust</b> that value, it <b>checks</b> it. The TenantGuard
    /// compares it with the tenant on the <c>CommandContext</c>, which came from the session, and a mismatch
    /// is rejected before any business logic runs. A tampered browser value ends there.
    /// </summary>
    public sealed record SaveWorkOrderCommand(
        int WorkOrderId,
        string TenantId,
        string Title,
        WorkOrderStatus Status,
        ConcurrencyToken ExpectedVersion);
}
