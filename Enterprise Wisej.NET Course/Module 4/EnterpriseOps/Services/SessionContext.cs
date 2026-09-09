using System;
using EnterpriseOps.Security;
using EnterpriseOps.Services.Commands;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// Per-session state: the current tenant and user, and the correlation-id factory. One instance per
    /// Wisej session (created by the page), never static — Module 3's static-state audit says why.
    /// </summary>
    public sealed class SessionContext
    {
        public string SessionId { get; }
        public string TenantId { get; set; } = "fabrikam";
        public string UserId { get; set; } = KnownUsers.Manager;
        public Role Role => KnownUsers.RoleOf(UserId);

        /// <summary>How long a command may hold its transaction before it is cancelled and mapped to DB_TIMEOUT.</summary>
        public TimeSpan CommandTimeout { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>The correlation id of the last command built — shown in the header bar.</summary>
        public string LastCorrelationId { get; private set; } = "—";

        public SessionContext(string sessionId)
        {
            SessionId = sessionId;
        }

        /// <summary>A new correlation id per command: 8 hex characters, easy to quote to support.</summary>
        public CommandContext NewCommandContext()
        {
            LastCorrelationId = Guid.NewGuid().ToString("N").Substring(0, 8);
            return new CommandContext(TenantId, UserId, Role, LastCorrelationId);
        }
    }
}
