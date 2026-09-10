using System.Collections.Specialized;
using EnterpriseOps.Services;
using Wisej.Web;

namespace EnterpriseOps
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup").
        ///
        /// The startup stopwatch starts here, in SessionContext.Start, and is stopped by
        /// DiagnosticsPage.Load — that span is the "Startup" row of the performance budget table.
        /// The SessionContext lives in Application.Session (per user, never static), so every service
        /// the page creates works for this session's tenant and user only.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            SessionContext session = SessionContext.Start(Application.SessionId);
            Application.Session.Context = session;

            Application.MainPage = new UI.DiagnosticsPage();
        }
    }
}
