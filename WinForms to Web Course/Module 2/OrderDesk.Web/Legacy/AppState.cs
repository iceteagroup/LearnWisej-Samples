using OrderDesk.Domain;

namespace OrderDesk.Legacy
{
    /// <summary>
    /// Copied unchanged from LegacyOrderDesk/Legacy/AppState.cs so the ported OrdersPage compiles
    /// with the same lines the desktop form had (parity first). Safe when one process serves one
    /// user; shared by EVERY browser session once the same code runs inside a web server — the
    /// Module 4 refactoring target. Module 2 only carries it across and marks it.
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
