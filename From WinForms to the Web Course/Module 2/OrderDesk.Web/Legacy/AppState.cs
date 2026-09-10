using OrderDesk.Domain;

namespace OrderDesk.Legacy
{
    /// <summary>
    /// Copied as-is from LegacyOrderDesk/AppState.cs so the first slice can show WHY it is a
    /// migration task: on the desktop each user had their own process and these statics were
    /// private; on the server every browser session shares one process and therefore one
    /// CurrentUser. Module 4 replaces this class with a typed session context.
    /// </summary>
    public static class AppState
    {
        public static string CurrentUser;                 // ✕ shared by every session on the server
        public static string CurrentCompany = "Acme";     // ✕
        public static Customer CurrentCustomer;           // ✕
        public static OrderStatus? CurrentFilter;         // ✕
        public static string LastSearch;                  // ✕

        // ✓ immutable lookup data — a static like this stays safe on the server
        public static readonly string[] Countries = { "US", "UK", "CA", "DE" };
    }
}
