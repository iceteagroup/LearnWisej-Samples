using System.Collections.Specialized;
using EnterpriseOps.Data;
using EnterpriseOps.Security;
using EnterpriseOps.Services;
using Wisej.Web;

namespace EnterpriseOps
{
    /// <summary>
    /// The composition root of one browser session, and the first thing the capstone defense points at.
    ///
    /// It does three things: create the per-session <see cref="SessionContext"/> (tenant + signed-in user +
    /// the services built for them), park it in <c>Application.Session</c> so both screens of the session can
    /// reach it, and open the Command Center. No business decision is taken here — that is what
    /// <c>EnterpriseOps.Services</c> is for.
    ///
    /// Nothing session-shaped is static: every browser session runs in the same process, and this module's own
    /// review checklist (rule Q2) rejects the alternative.
    /// </summary>
    internal static class Program
    {
        /// <summary>Wisej.NET session entry point (configured in Default.json "startup").</summary>
        static void Main(NameValueCollection args)
        {
            // A session starts as ana.ops (Manager) on contoso. "Review as ben.tech" on either screen switches
            // the identity, so the permission-denied paths can be seen without opening a second browser.
            SessionContext session = SessionContext.CreateFor(
                InMemoryWorkOrderStore.Tenants[0],
                KnownUsers.AnaOps,
                Application.StartupPath);       // where docs/ is looked for first (docs: Application.StartupPath)

            Application.Session.Context = session;

            Application.MainPage = new UI.CommandCenterDashboard(session);
        }
    }
}
