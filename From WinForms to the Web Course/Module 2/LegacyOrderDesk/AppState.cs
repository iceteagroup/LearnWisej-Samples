using OrderDesk.Domain;

namespace LegacyOrderDesk
{
    /// <summary>
    /// Global state the way a one-process-per-user desktop app keeps it: static fields.
    /// Safe on the desktop (each user has their own process), a data leak on a server where
    /// every browser session shares the process. This is the Module 1 "✕ static current user"
    /// item and the Module 4 refactoring target.
    /// </summary>
    public static class AppState
    {
        public static string CurrentUser;
        public static string CurrentCompany = "Acme";
        public static Customer CurrentCustomer;
        public static OrderStatus? CurrentFilter;
        public static string LastSearch;

        // Immutable lookup data — this kind of static is fine on the server too (Module 4).
        public static readonly string[] Countries = { "US", "UK", "CA", "DE" };
    }
}
