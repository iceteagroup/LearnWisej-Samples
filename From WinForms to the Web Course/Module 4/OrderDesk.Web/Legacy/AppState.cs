using OrderDesk.Domain;

namespace OrderDesk.Legacy
{
    /// <summary>
    /// Copied as-is from LegacyOrderDesk/AppState.cs. This is the Module 4 refactoring target:
    /// on the desktop each user had their own process, so these statics were private to that
    /// user; on the server every browser session shares one process and therefore one slot.
    /// The console keeps the class so the "Legacy statics" mode can show the corruption live.
    /// </summary>
    public static class AppState
    {
        public static string CurrentUser;                 // ✕ one slot for the whole server — the second sign-in overwrites the first
        public static string CurrentCompany = "Acme";     // ✕ per-user state in a static: shared by every session
        public static Customer CurrentCustomer;           // ✕ sam's selection silently replaces kelly's
        public static OrderStatus? CurrentFilter;         // ✕ a filter changed in tab B re-filters tab A on its next refresh
        public static string LastSearch;                  // ✕ per-user workflow state

        // ✓ immutable, user-independent lookup data — a static like this stays safe on the server
        public static readonly string[] Countries = { "US", "UK", "CA", "DE" };
    }
}
