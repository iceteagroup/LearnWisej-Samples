using System;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// What every command carries: who is acting, in which tenant, under which correlation id. Services
    /// never read the tenant or the user from the UI — they read it from here.
    /// </summary>
    public sealed class CommandContext
    {
        public string TenantId { get; }
        public string UserName { get; }
        public UserRole Role { get; }
        public string CorrelationId { get; }

        public CommandContext(string tenantId, string userName, UserRole role, string correlationId)
        {
            TenantId = tenantId;
            UserName = userName;
            Role = role;
            CorrelationId = correlationId;
        }

        /// <summary>Eight hex characters, like the "correlation 4e9d20b7" the walkthrough's report shows.</summary>
        public static string NewCorrelationId() => Guid.NewGuid().ToString("N").Substring(0, 8);
    }
}
