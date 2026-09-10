using System.Collections.Specialized;
using EnterpriseOps.Services;
using Wisej.Web;

namespace EnterpriseOps
{
    /// <summary>
    /// The composition root of one browser session.
    ///
    /// It creates the per-session <see cref="SessionContext"/> (tenant + signed-in user), parks it in
    /// <c>Application.Session</c> so any screen of the session can reach it, and opens the module's screen.
    /// No business decision is taken here — that is what Services are for — and no component is configured
    /// here either: <c>StatusTimeline</c> and <c>WorkOrderChartWidget</c> register their own resources.
    ///
    /// Never keep session state in a static field: every browser session runs in the same process.
    /// </summary>
    internal static class Program
    {
        /// <summary>Wisej.NET session entry point (configured in Default.json "startup").</summary>
        static void Main(NameValueCollection args)
        {
            // The walkthrough's session: fabrikam, signed in as the operations manager.
            var session = new SessionContext("fabrikam", "Fabrikam Field Services", "ana.ops", "Manager");
            Application.Session.Context = session;

            Application.MainPage = new UI.WorkOrderHistoryPage(session);
        }
    }
}
