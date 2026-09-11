using System.Collections.Specialized;
using TicketOps.Infrastructure;
using TicketOps.Views;
using Wisej.Web;

namespace TicketOps
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup"). Runs once per browser session.
        ///
        /// The startup decision is which registration profile Application.Services should hold — fake (default)
        /// or production (?profile=production on the URL). ServiceRegistration.Apply is idempotent because the
        /// registration table is shared by every session; the Session-lifetime instances it describes are still
        /// created per session. The Form is created with "new" and receives its services through [Inject]
        /// properties, not through its constructor.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            string requested = args?["profile"] ?? Application.QueryString?["profile"];
            ServiceRegistration.Apply(ServiceRegistration.ProfileFrom(requested), out _);

            new TicketWorkflow().Show();
        }
    }
}
