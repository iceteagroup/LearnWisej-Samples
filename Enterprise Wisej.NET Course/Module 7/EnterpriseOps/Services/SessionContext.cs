using System;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// Tenant + user + correlation id for this browser session. Created once per session in the page
    /// constructor and handed to services — never a static (Module 3 explains why).
    /// </summary>
    public class SessionContext
    {
        public string TenantId { get; }
        public UserIdentity User { get; }
        public string CorrelationId { get; private set; }

        public SessionContext(string tenantId, UserIdentity user)
        {
            TenantId = tenantId;
            User = user;
            CorrelationId = NewCorrelationId();
        }

        /// <summary>One correlation id per user action, so every trace line and audit entry can be joined.</summary>
        public string NextCorrelation()
        {
            CorrelationId = NewCorrelationId();
            return CorrelationId;
        }

        private static string NewCorrelationId() => Guid.NewGuid().ToString("N").Substring(0, 8);
    }
}
