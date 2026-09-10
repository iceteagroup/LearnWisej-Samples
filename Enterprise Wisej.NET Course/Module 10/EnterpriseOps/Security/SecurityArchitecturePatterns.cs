using System;

namespace EnterpriseOps.Security
{
    /// <summary>
    /// The named actions this application can be asked to perform. Authorization talks about permissions,
    /// never about roles: a service asks "may this caller ExportData?", not "is this caller a Manager?".
    /// That is what lets the permission matrix change without touching a single service.
    ///
    /// The first five members are the lesson's listing, in the lesson's order. The last two are added by the
    /// lab, because the audit screen is itself a gated surface and a large export needs a second pair of eyes.
    /// </summary>
    public enum Permission
    {
        ViewWorkOrders,
        EditWorkOrders,
        ApproveWorkOrders,
        ExportData,
        AdminDiagnostics,

        // Added by the lab (the lesson listing stops at AdminDiagnostics):
        /// <summary>Read the audit log. An audit trail that everyone can read is a second data leak.</summary>
        ViewAuditLog,

        /// <summary>Approve someone else's pending export (dual control). Never held by the requester alone.</summary>
        ApproveExport
    }

    /// <summary>
    /// The one question the application asks about authorization, and the only place it is answered.
    ///
    /// <see cref="Demand"/> is what a **service** calls before it executes: it throws, so the action cannot
    /// continue by accident. <see cref="Has"/> is what a **screen** calls to adapt itself: it answers, so a
    /// button can be hidden. A screen that only calls <c>Has</c> is a convenience, not a control — the failure
    /// path in this lab shows exactly what happens when the button lies and the service does not.
    /// </summary>
    public interface IPermissionService
    {
        /// <summary>
        /// Throws <see cref="UnauthorizedAccessException"/> unless the caller may perform <paramref name="permission"/>.
        /// When <paramref name="resourceTenantId"/> is given, the tenant guard runs first: a user who holds a role in
        /// tenant A gets nothing in tenant B. Every call is written to the audit log, granted or denied.
        /// </summary>
        void Demand(CommandContext context, Permission permission, string resourceTenantId = null);

        /// <summary>
        /// The same evaluation without the exception, for screens that adapt themselves. It does **not** write an
        /// audit entry: a screen asking "may I show this button?" has not attempted anything.
        /// </summary>
        bool Has(CommandContext context, Permission permission, string resourceTenantId = null);
    }

    /// <summary>
    /// Roles → permissions, per tenant. Kept behind an interface because in production it is a table (or a cache
    /// in front of one) that an administrator edits, while the lab keeps it in memory.
    /// </summary>
    public interface IRolePermissionStore
    {
        /// <summary>True when the user, through the roles they hold **in that tenant**, is granted the permission.</summary>
        bool UserHasPermission(string userId, string tenantId, Permission permission);
    }

    /// <summary>
    /// The tenant-first rule of the model: before any role is consulted, the tenant of the record being touched
    /// must match the tenant on the command context — and the context's tenant came from the token, not the UI.
    /// </summary>
    public interface ITenantGuard
    {
        void DemandTenant(CommandContext context, string resourceTenantId);
    }
}
