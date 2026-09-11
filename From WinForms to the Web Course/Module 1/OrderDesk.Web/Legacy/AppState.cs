using OrderDesk.Domain;

namespace OrderDesk.Legacy
{
    /// <summary>
    /// Copied as-is from LegacyOrderDesk/AppState.cs. On the desktop each user had their own
    /// process; on the server every browser session shares one process and therefore one CurrentUser.
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
