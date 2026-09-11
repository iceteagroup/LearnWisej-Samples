using OrderDesk.Domain;

namespace OrderDesk.Legacy
{
    /// <summary>
    /// Copied as-is from LegacyOrderDesk/AppState.cs: the ported OrdersForm still reads these
    /// statics. On the server every browser session shares them (an open item for Module 4).
    /// </summary>
    public static class AppState
    {
        public static string CurrentUser;
        public static string CurrentCompany = "Acme";
        public static Customer CurrentCustomer;
        public static OrderStatus? CurrentFilter;
        public static string LastSearch;

        public static readonly string[] Countries = { "US", "UK", "CA", "DE" };
    }
}
