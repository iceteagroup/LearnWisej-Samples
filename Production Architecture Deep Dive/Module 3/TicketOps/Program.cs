using System.Collections.Specialized;
using TicketOps.Infrastructure;
using Wisej.Web;

namespace TicketOps
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup").
        /// One call per browser session: compose the session's object graph, then show the main page.
        /// Module 3 uses a Page instead of the fixed-size Form of the other modules: a Page fills the
        /// browser viewport, so the responsive layout can be watched following the real window size.
        /// Nothing here is static — every session gets its own ActivityLog, repository and services.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            var composition = new AppComposition();
            Application.MainPage = composition.CreateMainView();
        }
    }
}
