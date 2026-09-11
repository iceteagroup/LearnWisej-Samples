using System;

namespace EnterpriseOps.Security
{
    public enum Role { Technician, Manager, Admin }

    /// <summary>
    /// Who is using this session: tenant, user, role. One instance per session, created by the page and
    /// stored in <c>Application.Session.Context</c> — never in a static. Flow 9 of the regression harness
    /// checks exactly that: the same instance is visible before and after navigating between pages.
    /// </summary>
    public class SessionContext
    {
        public SessionContext(string tenantId, string userName, Role role)
        {
            TenantId = tenantId;
            UserName = userName;
            Role = role;
            SessionCorrelationId = NewCorrelationId();
        }

        public string TenantId { get; }
        public string UserName { get; }
        public Role Role { get; }

        /// <summary>Identifies the session in the log; every command gets its own id on top of it.</summary>
        public string SessionCorrelationId { get; }

        public static string NewCorrelationId() => Guid.NewGuid().ToString("N").Substring(0, 8);

        public override string ToString() => $"{TenantId}/{UserName} ({Role})";
    }
}
