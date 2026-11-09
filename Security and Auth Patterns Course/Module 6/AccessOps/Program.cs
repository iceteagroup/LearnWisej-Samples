using System.Collections.Specialized;
using AccessOps.Identity;
using AccessOps.Pages;
using Wisej.Web;

namespace AccessOps
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (Default.json "startup"). Runs once per new session, on
        /// the first HTTP load of a browser tab.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            // Application.Reload() after a verified click, a browser refresh, or the tab coming
            // back from the identity provider in the same session: the page load is the only
            // place where the session ID can be rotated.
            Application.ApplicationRefresh += (s, e) => OnPageLoad();

            // The tab can also come back from the provider into a brand-new session.
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

        /// <summary>Shows another page of the signed-in console (Records, My passkeys).</summary>
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
