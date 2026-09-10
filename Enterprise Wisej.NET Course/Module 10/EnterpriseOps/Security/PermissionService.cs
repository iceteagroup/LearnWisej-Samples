using System;
using EnterpriseOps.Diagnostics;

namespace EnterpriseOps.Security
{
    /// <summary>
    /// The permission service — the single place that answers "may this caller do this?".
    ///
    /// The shape is the lesson's: a tenant guard and a role store go in through the constructor,
    /// <see cref="Demand"/> checks the tenant first and then the store, and it throws with the missing
    /// permission named. Two things the lab adds around that shape:
    ///
    ///  1. **Every Demand is audited**, granted or denied. The service that executes a command must not be able
    ///     to succeed without leaving a record, so the record is written here, where the decision is taken.
    ///  2. **Has does not audit.** A screen asking "should I show this button?" has attempted nothing; if probes
    ///     wrote entries, the trail would be noise and a real denial would hide in it.
    ///
    /// Where is authorization enforced? Here — and this object is reached only from services, never from a
    /// button handler that decides on its own whether to call it.
    /// </summary>
    public sealed class PermissionService : IPermissionService
    {
        private readonly ITenantGuard _tenantGuard;
        private readonly IRolePermissionStore _roles;
        private readonly IAuditLog _audit;
        private readonly ActivityTrace _trace;

        public PermissionService(ITenantGuard tenantGuard, IRolePermissionStore roles, IAuditLog audit, ActivityTrace trace)
        {
            _tenantGuard = tenantGuard;
            _roles = roles;
            _audit = audit;
            _trace = trace;
        }

        /// <summary>
        /// Throws unless the caller may perform the permission — and writes the audit entry either way.
        /// The lesson's body, with the audit and the trace woven in:
        /// tenant guard first (when a resource tenant is given), then the role store.
        /// </summary>
        public void Demand(CommandContext context, Permission permission, string resourceTenantId = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            try
            {
                Evaluate(context, permission, resourceTenantId, probe: false);
            }
            catch (CrossTenantAccessException ex)
            {
                _audit.Write(context, permission.ToString(), AuditResult.Denied, resourceTenantId,
                    $"cross-tenant: session '{ex.SessionTenantId}' requested '{ex.RequestedTenantId}'");
                throw;
            }
            catch (UnauthorizedAccessException)
            {
                _audit.Write(context, permission.ToString(), AuditResult.Denied, resourceTenantId,
                    $"missing permission for roles {context.RoleList}");
                throw;
            }

            _audit.Write(context, permission.ToString(), AuditResult.Ok, resourceTenantId,
                $"granted through roles {context.RoleList}");
        }

        /// <summary>
        /// The lesson writes this as <c>try { Demand(…); return true; } catch { return false; }</c>. Same answer,
        /// same order of checks — but routed to <see cref="Evaluate"/> so a screen adapting itself does not fill
        /// the audit log with denials nobody attempted.
        /// </summary>
        public bool Has(CommandContext context, Permission permission, string resourceTenantId = null)
        {
            try
            {
                Evaluate(context, permission, resourceTenantId, probe: true);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        /// <summary>
        /// The decision itself: tenant first, then roles. A probe (<c>Has</c>) takes exactly the same path and
        /// stays silent; a demand narrates itself into the trace so a reviewer can see where the check happened.
        /// </summary>
        private void Evaluate(CommandContext context, Permission permission, string resourceTenantId, bool probe)
        {
            if (resourceTenantId != null)
                _tenantGuard.DemandTenant(context, resourceTenantId);

            if (!_roles.UserHasPermission(context.UserId, context.TenantId, permission))
            {
                if (!probe)
                    _trace?.Security($"PermissionService.Demand({permission}) DENIED for {context.UserId}@{context.TenantId} roles={context.RoleList}");
                throw new UnauthorizedAccessException($"Missing permission: {permission}");
            }

            if (!probe)
                _trace?.Security($"PermissionService.Demand({permission}) granted for {context.UserId}@{context.TenantId} roles={context.RoleList}");
        }
    }
}
