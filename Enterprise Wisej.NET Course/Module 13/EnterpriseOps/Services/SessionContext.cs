using System;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// Per-session identity: who is using this browser tab, for which tenant. Created once in the page
    /// constructor — never in a static.
    /// </summary>
    public class SessionContext
    {
        public string TenantId { get; set; }
        public string TenantName { get; set; }
        public string User { get; set; }
        public string Role { get; set; }

        /// <summary>A new command context for one user action: same tenant and user, a fresh correlation id.</summary>
        public CommandContext NewCommand()
            => new CommandContext(TenantId, User, Guid.NewGuid().ToString("N").Substring(0, 8));
    }

    /// <summary>What every service call carries. Services never read Application.Session themselves.</summary>
    public class CommandContext
    {
        public CommandContext(string tenantId, string user, string correlationId)
        {
            TenantId = tenantId;
            User = user;
            CorrelationId = correlationId;
        }

        public string TenantId { get; }
        public string User { get; }
        public string CorrelationId { get; }

        /// <summary>The same tenant/user with a new correlation id — used when a queued command is replayed.</summary>
        public CommandContext ForReplay(string correlationId) => new CommandContext(TenantId, User, correlationId);
    }
}
