using System;
using EnterpriseOps.Diagnostics;

namespace EnterpriseOps.Security
{
    /// <summary>
    /// Thrown when a command reaches for a record that belongs to another tenant. It is an
    /// <see cref="UnauthorizedAccessException"/> so a service that already catches "not allowed" catches this too,
    /// and it carries the correlation id so the audit entry, the trace line and the toast all agree.
    /// </summary>
    public sealed class CrossTenantAccessException : UnauthorizedAccessException
    {
        public CrossTenantAccessException(CommandContext context, string requestedTenantId)
            : base($"Cross-tenant access denied: this session is on '{context?.TenantId}'.")
        {
            CorrelationId = context?.CorrelationId;
            SessionTenantId = context?.TenantId;
            RequestedTenantId = requestedTenantId;
        }

        public string CorrelationId { get; }
        public string SessionTenantId { get; }
        public string RequestedTenantId { get; }
    }

    /// <summary>
    /// The tenant guard. Every service compares the tenant on the context with the tenant of the record it is
    /// about to touch and rejects a mismatch **before** any business logic or role lookup runs.
    ///
    /// The tenant on the context comes from the "tid" claim by way of the session, so a tenant id typed into a
    /// query string, chosen in a combo box, or edited in a hidden field cannot get past this method.
    /// </summary>
    public sealed class TenantGuard : ITenantGuard
    {
        private readonly ActivityTrace _trace;

        public TenantGuard(ActivityTrace trace)
        {
            _trace = trace;
        }

        public void DemandTenant(CommandContext context, string resourceTenantId)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (!StringComparer.Ordinal.Equals(context.TenantId, resourceTenantId))
            {
                _trace?.Security($"TenantGuard REJECTED — session tenant '{context.TenantId}' ≠ requested '{resourceTenantId}' (corr {context.CorrelationId})");
                throw new CrossTenantAccessException(context, resourceTenantId);
            }

            _trace?.Security($"TenantGuard ok — '{resourceTenantId}' is the session tenant");
        }
    }
}
