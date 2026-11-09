using System.Collections.Specialized;
using System.Security.Principal;
using AccessOps.Identity;
using AccessOps.Pages;
using AccessOps.Security;
using Wisej.Web;

namespace AccessOps
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (Default.json "startup"). Runs once per new session, on the
        /// first HTTP load of a browser tab, and branches on the entry point this process serves.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            if (AppHost.Route == AccessOpsRoute.Intranet)
                StartIntranet();
            else
                StartPartner();
        }

        /// <summary>
        /// The intranet route (Modules 2-3): by the time this runs the host has answered the Windows
        /// challenge - a 401 never reaches it. The Windows account is evidence; the grant mapped to its
        /// SID is the permission. The page load establishes the same application principal the partner
        /// route establishes (account, tenant, grant claims), so every later click - a WebSocket event,
        /// where the host principal's token may already be disposed - is decided by RecordPolicy on
        /// the grant, exactly like a partner click.
        /// </summary>
        private static void StartIntranet()
        {
            if (!Application.IsAuthenticated || Application.User?.Identity is not WindowsIdentity windows)
            {
                new HostFaultPage().Show();   // host layer, not an app deny
                return;
            }

            var sid = windows.User?.Value;
            var masked = WindowsName.Redact(windows.Name);
            var grant = AccessGrants.FindForAccount(sid);
            ApplicationPrincipal.Establish(AccessGrants.SignInFor(sid, grant, masked, AppHost.Get<IClock>().Now));

            new MainPage(new IntranetUser(masked, Subject.From(Application.User), grant)).Show();
        }

        /// <summary>The partner route (Modules 4-6): own sign-in, OpenID Connect hand-off, passkeys.</summary>
        private static void StartPartner()
        {
            // Application.Reload() after a verified click, a browser refresh, or the tab coming
            // back from the identity provider in the same session: the page load is the only
            // place where the session ID can be rotated.
            Application.ApplicationRefresh += (s, e) => OnPageLoad();

            // The tab can also come back from the provider into a brand-new session - for example on
            // another node, or after this process restarted: the hand-off handle is in the shared store.
            var external = AppHost.Get<IExternalSignIn>().TryCompleteOnPageLoad();
            if (external == ExternalCompletion.SignedIn)
            {
                Application.MainPage = new ConsolePage();
                return;
            }

            Application.MainPage = new LoginPage(Notice(external));
        }

        private static void OnPageLoad()
        {
            var external = AppHost.Get<IExternalSignIn>().TryCompleteOnPageLoad();
            if (external == ExternalCompletion.SignedIn || (external == ExternalCompletion.None && SignInHandoff.CompleteOnPageLoad()))
            {
                Replace(new ConsolePage());
                return;
            }

            // A refused callback in a tab that is already signed in (a replayed callback, F07) changes
            // nothing: the console and its grant stay as they were. Otherwise the sign-in view shows
            // the one fixed sentence.
            if ((external == ExternalCompletion.Rejected || Application.QueryString["signin"] == "failed")
                && Subject.From(Application.User).IsAnonymous)
                Replace(new LoginPage(LoginNotice.ExternalFailed));
        }

        // Default.html turns the word a sign-out left in sessionStorage into ?ended=signout; the
        // OpenID Connect handler sends every failed callback to ?signin=failed.
        private static LoginNotice Notice(ExternalCompletion external)
        {
            if (external == ExternalCompletion.Rejected || Application.QueryString["signin"] == "failed")
                return LoginNotice.ExternalFailed;
            if (Application.QueryString["ended"] == "signout")
                return LoginNotice.SignedOut;
            return LoginNotice.None;
        }

        /// <summary>Shows another page of the signed-in partner console (Records, My passkeys).</summary>
        internal static void Show(Page page) => Replace(page);

        private static void Replace(Page page)
        {
            var old = Application.MainPage;
            Application.MainPage = page;
            if (old != page)
                old?.Dispose();
        }
    }
}
