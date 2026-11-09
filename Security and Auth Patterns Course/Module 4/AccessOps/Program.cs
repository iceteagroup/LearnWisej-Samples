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
        /// the first HTTP load of a browser tab: the session ID lives in the tab's sessionStorage,
        /// so every tab - and every browser profile - is its own session.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            // Application.Reload() after a verified click, or a browser refresh: the page load is
            // the only place where the session ID can be rotated (SignInHandoff).
            Application.ApplicationRefresh += (s, e) => OnPageLoad();

            // Default.html turns the word a sign-out left in sessionStorage into ?ended=signout.
            var signedOut = Application.QueryString["ended"] == "signout";
            Application.MainPage = new LoginPage(signedOut);
        }

        private static void OnPageLoad()
        {
            if (!SignInHandoff.CompleteOnPageLoad())
                return;

            // replace the login view with the console
            var login = Application.MainPage;
            Application.MainPage = new ConsolePage();
            login?.Dispose();
        }
    }
}
