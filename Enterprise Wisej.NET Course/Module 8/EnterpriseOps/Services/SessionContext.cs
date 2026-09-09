using System;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// Who is acting, for which tenant, under which correlation id. One instance per session, created by
    /// the page and passed to every service call — never a static.
    /// </summary>
    public class SessionContext
    {
        public SessionContext(string tenantId, string tenantName, string userName, string role)
        {
            this.TenantId = tenantId;
            this.TenantName = tenantName;
            this.UserName = userName;
            this.Role = role;
            NextCorrelation();
        }

        public string TenantId { get; }
        public string TenantName { get; }
        public string UserName { get; }
        public string Role { get; }

        /// <summary>The id every trace line of the current operation carries.</summary>
        public string CorrelationId { get; private set; }

        /// <summary>Starts a new operation: a fresh correlation id.</summary>
        public string NextCorrelation()
        {
            this.CorrelationId = Guid.NewGuid().ToString("N").Substring(0, 8);
            return this.CorrelationId;
        }
    }

    /// <summary>The live activity trace every layer writes to (the page renders it in lstTrace).</summary>
    public interface IActivityTrace
    {
        void Write(string line);
    }
}
