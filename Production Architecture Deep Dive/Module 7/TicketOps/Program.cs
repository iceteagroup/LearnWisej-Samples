using System.Collections.Specialized;
using TicketOps.Infrastructure;

namespace TicketOps
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup").
        /// One call per browser session: compose the session's object graph, then open the main view.
        /// Nothing here is static — every session gets its own ActivityLog, repository and services.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            var composition = new AppComposition();
            composition.CreateMainView().Show();
        }
    }
}
