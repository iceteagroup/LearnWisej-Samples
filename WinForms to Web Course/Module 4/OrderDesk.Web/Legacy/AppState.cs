using OrderDesk.Domain;

namespace OrderDesk.Legacy
{
    /// <summary>
    /// Copied from LegacyOrderDesk/Legacy/AppState.cs for the Module 4 demonstration.
    /// Global state the desktop app kept in static fields. Safe when one process serves one user;
    /// shared by EVERY browser session once the same code runs inside a web server.
    /// The migrated app touches it only through Services/StateStores.LegacyStaticStore so the
    /// corruption can be shown next to the web-safe UserContext.
    /// </summary>
    public static class AppState
    {
        // ✕ per-user state in statics — the Module 4 refactoring target
        public static User CurrentUser;
        public static Customer CurrentCustomer;
        public static string ActiveFilter = "Open";
        public static Order CurrentOrder;

        // ✓ immutable, user-independent lookups may stay static
        public static readonly string[] Countries = { "US", "UK", "CA", "DE", "AT", "IE" };
    }
}
