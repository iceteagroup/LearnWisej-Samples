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
        /// so every tab - and every browser profile - is its own session with its own subject.
        /// </summary>
        static void Main(NameValueCollection args)
        {
#if DEBUG
            IdentityTrace.Write("Startup · Program.Main");
            Application.ApplicationStart += (s, e) => IdentityTrace.Write("Startup · ApplicationStart");
#endif
            // A browser reload or Application.Reload() in the same session: this page load is the
            // only place where the session ID can be rotated (see ApplicationPrincipal).
            Application.ApplicationRefresh += (s, e) => OnPageLoad();

            Application.MainPage = new MainPage();
        }

        private static void OnPageLoad()
        {
#if DEBUG
            IdentityTrace.Write("Refresh · ApplicationRefresh (before)");
#endif
            if (SignInFlow.CompletePending())
                (Application.MainPage as MainPage)?.OnSignedIn();
#if DEBUG
            IdentityTrace.Write("Refresh · ApplicationRefresh (after)");
#endif
        }
    }
}
