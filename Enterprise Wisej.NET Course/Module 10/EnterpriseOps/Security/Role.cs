namespace EnterpriseOps.Security
{
    /// <summary>
    /// The application's own roles — the middle layer of the identity model (identity · membership · permissions).
    ///
    /// These names are the application's, not the identity provider's. The provider sends group names such as
    /// "EnterpriseOps-Managers" or "APP_EOPS_TECH_FAB"; <see cref="ClaimsMapper"/> turns those into these, so no
    /// authorization code in the application ever depends on one provider's format.
    ///
    /// Technician / Manager / Admin are the three course-wide roles; Auditor and ServiceAccount are added by this
    /// module because the audit screen needs a read-only reviewer and the import job needs a non-human caller.
    /// </summary>
    public enum Role
    {
        /// <summary>Field engineer: sees and edits the work orders of their own tenant. Cannot approve or export.</summary>
        Technician,

        /// <summary>Dispatcher / supervisor: approves work orders and requests exports.</summary>
        Manager,

        /// <summary>Tenant administrator: everything a manager may do, plus diagnostics and export approval.</summary>
        Admin,

        /// <summary>Read-only reviewer: sees work orders and the audit log, changes nothing, exports nothing.</summary>
        Auditor,

        /// <summary>A non-human caller (the nightly import). Edits work orders; never approves, never exports.</summary>
        ServiceAccount
    }
}
