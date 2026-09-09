using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// Everything a service needs to know about *who* is asking: tenant, user, role, correlation id.
    /// Immutable and passed explicitly — services never reach for session state on their own.
    /// </summary>
    public sealed class CommandContext
    {
        public CommandContext(string tenantId, string userName, UserRole role, string correlationId)
        {
            TenantId = tenantId;
            UserName = userName;
            Role = role;
            CorrelationId = correlationId;
        }

        public string TenantId { get; }
        public string UserName { get; }
        public UserRole Role { get; }
        public string CorrelationId { get; }

        public override string ToString() => $"tenant={TenantId} user={UserName} corr={CorrelationId}";
    }
}
