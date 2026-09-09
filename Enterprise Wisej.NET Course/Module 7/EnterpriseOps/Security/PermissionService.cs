using EnterpriseOps.Domain;
using EnterpriseOps.Services;

namespace EnterpriseOps.Security
{
    /// <summary>
    /// The one place that knows who may escalate. The workflow asks it during the "authorize" step;
    /// the wizard never does — a page that checks roles has hidden a security decision in a Form.
    /// </summary>
    public class PermissionService
    {
        private readonly ActivityTrace _trace;

        public PermissionService(ActivityTrace trace)
        {
            _trace = trace;
        }

        public bool CanEscalate(UserIdentity user, string tenantId, string workOrderTenantId)
        {
            if (tenantId != workOrderTenantId)
            {
                _trace.Write($"Security: {user.UserName} denied — work order belongs to tenant '{workOrderTenantId}', session is '{tenantId}'");
                return false;
            }

            bool allowed = user.Role == UserRole.Manager || user.Role == UserRole.Admin;
            _trace.Write(allowed
                ? $"Security: {user} may escalate in tenant '{tenantId}'"
                : $"Security: {user} denied — escalation requires Manager or Admin");
            return allowed;
        }

        public bool CanApprove(Approver approver)
        {
            return approver != null && (approver.Role == "Supervisor" || approver.Role == "Manager");
        }
    }
}
