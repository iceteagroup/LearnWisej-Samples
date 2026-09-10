using System.Collections.Specialized;
using TicketOps.Infrastructure;

namespace TicketOps
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup").
        /// Runs ONCE PER SESSION — a new browser connection starts its own instance, like a second launch of
        /// a desktop app. So this is where the per-session object graph (SessionContext, services, the main
        /// view) is composed. Once-per-process work (reading appsettings.json) is not here: it lives behind
        /// ProcessScope, which the first session triggers and every later session reuses.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            var composition = new AppComposition();
            composition.CreateMainView().Show();
        }
    }
}
