using System.Collections.Specialized;
using Wisej.Web;

namespace EnterpriseOps
{
    /// <summary>
    /// The composition root of one browser session.
    ///
    /// It creates the per-session service graph, parks it in <c>Application.Session</c> so any screen of the
    /// session can reach it, and opens the screen. It takes no security decision itself — and, importantly, it
    /// does **not** sign anybody in: the session starts unauthenticated and the screen's first act is to run the
    /// sign-in gate. Nothing in the application can obtain a <c>CommandContext</c> before that happens.
    ///
    /// Never keep session state in a static field: every browser session runs in the same process.
    /// </summary>
    internal static class Program
    {
        /// <summary>Wisej.NET session entry point (configured in Default.json "startup").</summary>
        static void Main(NameValueCollection args)
        {
            var trace = new Diagnostics.ActivityTrace();
            var services = new Services.ServiceRegistry(trace);

            Application.Session.Registry = services;
            Application.MainPage = new UI.AuditLogPage(services);
        }
    }
}
