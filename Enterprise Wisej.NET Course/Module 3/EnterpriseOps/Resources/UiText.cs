namespace EnterpriseOps.Resources
{
    /// <summary>
    /// The sentences the screens show. Statics, deliberately: an immutable string shared by every session is
    /// exactly what a static is for — immutable reference data, not a finding in the static-state audit.
    /// Anything that varies per user — a name, a tenant, a correlation id — is composed at the call site from
    /// the session or the command context, never stored here.
    /// </summary>
    internal static class UiText
    {
        public static readonly string ActionFailed = "The action could not be completed. Check the log for details.";
        public static readonly string SelectAWorkOrder = "Select a work order in the queue first.";
        public static readonly string OpenBeforeSaving = "Open a work order before saving.";
        public static readonly string NotYourTenant = "That work order belongs to another customer.";
    }
}
