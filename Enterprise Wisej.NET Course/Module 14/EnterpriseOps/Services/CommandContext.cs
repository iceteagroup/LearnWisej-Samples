using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// Everything a service needs to know about <em>who</em> is asking: tenant, user (with role) and the
    /// correlation id of this unit of work. Immutable and passed explicitly — a service never reaches into
    /// <c>Application.Session</c> on its own, which is what makes it testable without a browser
    /// (checklist Q6) and impossible to satisfy from a static field (checklist Q2).
    /// </summary>
    public sealed class CommandContext
    {
        public CommandContext(string tenantId, UserIdentity user, string correlationId)
        {
            TenantId = tenantId;
            User = user;
            CorrelationId = correlationId;
        }

        public string TenantId { get; }

        public UserIdentity User { get; }

        public string CorrelationId { get; }

        public string UserName => User?.Name;

        public override string ToString() => $"tenant={TenantId} user={UserName} corr={CorrelationId}";
    }
}
