using System.Collections.Specialized;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;
using EnterpriseOps.Services;
using Wisej.Web;

namespace EnterpriseOps
{
    /// <summary>
    /// The composition root of one browser session.
    ///
    /// It does three things and nothing else: create the per-session <see cref="SessionContext"/> (tenant +
    /// signed-in user), park it in <c>Application.Session</c> so any screen of the session can reach it, and
    /// open the reference screen. No business decision is taken here — that is what Services are for.
    ///
    /// Never keep session state in a static field: every browser session runs in the same process.
    /// </summary>
    internal static class Program
    {
        /// <summary>Wisej.NET session entry point (configured in Default.json "startup").</summary>
        static void Main(NameValueCollection args)
        {
            // Until Module 7 adds real authentication, a session starts as ana.ops (Manager) on contoso.
            SessionContext session = SessionContext.CreateFor(Tenants.Contoso, KnownUsers.AnaOps);
            Application.Session.Context = session;

            Application.MainPage = new UI.CommandCenterDashboard(session);
        }
    }
}
