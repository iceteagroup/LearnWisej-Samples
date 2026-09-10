using EnterpriseOps.Services;
using System.Collections.Specialized;
using Wisej.Web;

namespace EnterpriseOps
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup").
        /// Module 12: one session = one Release dashboard. Everything environment-specific was read
        /// once, at process start, in Startup.cs — the page only *shows* it.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            // The node counts the sessions it holds: it is what the "sessionStore" health check reports,
            // and it is exactly the state that makes sticky sessions a requirement rather than a tuning knob.
            HealthProbeService.SessionStarted();

            Application.MainPage = new UI.ReleaseDashboardPage();
        }
    }
}
