namespace EnterpriseOps.Services
{
    /// <summary>
    /// Travels with every service call: which tenant, which user, and the correlation id that ties the
    /// click, the service decision, the data access, the log entry and the user-facing message together.
    /// </summary>
    public sealed record CommandContext(string TenantId, string UserName, string CorrelationId);
}
