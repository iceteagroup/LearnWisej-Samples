using System;
using EnterpriseOps.Diagnostics;

namespace EnterpriseOps.Security
{
    public interface ITenantGuard
    {
        /// <summary>Rejects the call unless the entity's tenant is the tenant on the command context.</summary>
        void DemandTenant(CommandContext context, string entityTenantId);
    }

    /// <summary>Thrown by <see cref="TenantGuard"/>. Carries the correlation id so the audit and the toast agree.</summary>
    public sealed class CrossTenantAccessException : UnauthorizedAccessException
    {
        public string CorrelationId { get; }
        public string SessionTenantId { get; }
        public string RequestedTenantId { get; }

        public CrossTenantAccessException(CommandContext context, string requestedTenantId)
            : base("Cross-tenant access denied.")
        {
            CorrelationId = context?.CorrelationId;
            SessionTenantId = context?.TenantId;
            RequestedTenantId = requestedTenantId;
        }
    }

    /// <summary>
    /// The tenant guard from the walkthrough: every service compares the tenant on the context with the
    /// tenant of the record being touched and rejects a mismatch before any business logic runs. The tenant
    /// on the context comes from the session, so a dropdown value changed in the browser cannot pass it.
    /// </summary>
    public sealed class TenantGuard : ITenantGuard
    {
        private readonly IActivityTrace _trace;

        public TenantGuard(IActivityTrace trace)
        {
            _trace = trace;
        }

        public void DemandTenant(CommandContext context, string entityTenantId)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (!StringComparer.Ordinal.Equals(context.TenantId, entityTenantId))
            {
                _trace?.Write($"Security: TenantGuard REJECTED — session tenant '{context.TenantId}' ≠ requested '{entityTenantId}' (correlation {context.CorrelationId})");
                throw new CrossTenantAccessException(context, entityTenantId);
            }

            _trace?.Write($"Security: TenantGuard ok — '{entityTenantId}' matches the session tenant (correlation {context.CorrelationId})");
        }
    }
}
