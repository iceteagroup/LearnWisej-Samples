using EnterpriseOps.Security;

namespace EnterpriseOps.Services.Commands
{
    /// <summary>
    /// Who is doing what, for whom: tenant + user + role + correlation id. Built by the
    /// <see cref="SessionContext"/> per command, carried through the service, the audit row and the trace.
    /// </summary>
    public sealed class CommandContext
    {
        public string TenantId { get; }
        public string UserId { get; }
        public Role Role { get; }
        public string CorrelationId { get; }

        public CommandContext(string tenantId, string userId, Role role, string correlationId)
        {
            TenantId = tenantId;
            UserId = userId;
            Role = role;
            CorrelationId = correlationId;
        }

        public override string ToString() => $"{TenantId}/{UserId} ({Role}) corr {CorrelationId}";
    }
}
