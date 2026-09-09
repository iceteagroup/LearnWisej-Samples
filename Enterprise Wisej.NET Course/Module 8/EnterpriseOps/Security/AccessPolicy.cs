using EnterpriseOps.Domain;
using EnterpriseOps.Services;

namespace EnterpriseOps.Security
{
    /// <summary>
    /// Tenant isolation for read operations. The components in this module render whatever they are
    /// given, so the decision "may this user see this work order" must be taken here, on the server,
    /// before any data is handed to a control or a widget.
    /// </summary>
    public class AccessPolicy
    {
        private readonly IActivityTrace _trace;

        public AccessPolicy(IActivityTrace trace)
        {
            _trace = trace;
        }

        /// <summary>True when the work order belongs to the caller's tenant; otherwise the reason it was denied.</summary>
        public bool CanView(SessionContext context, WorkOrder workOrder, out string reason)
        {
            if (workOrder == null)
            {
                reason = "work order not found";
                _trace.Write($"Security: {reason}");
                return false;
            }

            if (workOrder.TenantId != context.TenantId)
            {
                reason = $"permission denied — work order {workOrder.Id} belongs to tenant '{workOrder.TenantId}', session tenant is '{context.TenantId}'";
                _trace.Write($"Security: {reason}");
                return false;
            }

            reason = null;
            _trace.Write($"Security: tenant check ok — {context.UserName} ({context.Role}) may view work order {workOrder.Id} of '{workOrder.TenantId}'");
            return true;
        }
    }
}
