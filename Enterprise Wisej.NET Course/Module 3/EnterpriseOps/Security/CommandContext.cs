using System;

namespace EnterpriseOps.Security
{
    /// <summary>
    /// Request-scoped state: built by the handler from the <see cref="SessionContext"/> when a command
    /// starts, then passed explicitly into services, repositories and background jobs. Every layer knows
    /// the tenant and the correlation id without reaching back into the session — a background job in
    /// particular cannot, because the session may end before the job does.
    /// </summary>
    public sealed record CommandContext(
        string UserId,
        string TenantId,
        string CorrelationId,
        DateTimeOffset RequestedAt,
        string CommandName)
    {
        public override string ToString() => $"{CommandName} · {UserId}@{TenantId} · correlation {CorrelationId}";
    }
}
