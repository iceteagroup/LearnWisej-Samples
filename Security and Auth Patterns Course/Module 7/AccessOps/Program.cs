using System;
using System.Collections.Specialized;
using AccessOps.Infrastructure;
using AccessOps.Pages;
using AccessOps.Services;
using Wisej.Web;

namespace AccessOps
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (Default.json "startup"). Runs once per browser tab: the
        /// session ID lives in the tab's sessionStorage, so every tab is its own session.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            // Idle limit: Default.json "sessionTimeout" (seconds). Our sign-out replaces the
            // built-in prolong dialog.
            Application.SessionTimeout += (s, e) =>
            {
                e.Handled = true;
                SignOutFlow.SignOut(SignOutReason.Idle);
            };

            // Page loads (HTTP, not WebSocket) are where the session ID can be renewed and where the
            // external sign-in hand-off cookie can be read and expired.
            Application.ApplicationRefresh += (s, e) => OnPageLoad();

            // The provider callback may land in a brand-new session (the tab's session timed out
            // while the user was at the provider): a fresh Main completes the hand-off just the same.
            var external = AppHost.Get<IExternalSignIn>().TryCompleteOnPageLoad();
            if (external == ExternalCompletion.SignedIn)
            {
                Application.MainPage = new ConsolePage();
                return;
            }

            var ended = external == ExternalCompletion.Rejected ? "rejected" : Application.QueryString["ended"];
            Application.MainPage = new SignInPage(NoticeFor(ended));
        }

        private static void OnPageLoad()
        {
            var session = (WisejSession)AppHost.Get<IAccessOpsSession>();
            var signedIn = session.CompletePendingSignIn();

            if (!signedIn)
            {
                var external = AppHost.Get<IExternalSignIn>().TryCompleteOnPageLoad();
                signedIn = external == ExternalCompletion.SignedIn;
                if (Application.MainPage is SignInPage page)
                    page.ShowNotice(NoticeFor(external == ExternalCompletion.Rejected ? "rejected" : Application.QueryString["ended"]));
            }

            if (signedIn)
            {
                var old = Application.MainPage;
                Application.MainPage = new ConsolePage();
                old?.Dispose();
            }
        }

        // The query value only picks one of a few fixed sentences; nothing it carries is displayed.
        private static string NoticeFor(string ended) => ended switch
        {
            "idle" => IdleSentence(Application.Configuration.SessionTimeout),
            "signout" => "You have signed out of AccessOps and of " + AppHost.Get<IExternalSignIn>().ProviderName + ".",
            "rejected" => "Sign-in did not complete. If this keeps happening, contact your administrator.",
            "failed" => "Sign-in did not complete. Try again.",
            "expired" => "Your sign-in has expired. Sign in again to continue.",
            _ => null
        };

        private static string IdleSentence(int seconds)
        {
            var minutes = Math.Max(1, seconds / 60);
            return $"Your session ended after {minutes} minute{(minutes == 1 ? "" : "s")} of inactivity. Sign in to continue.";
        }
    }
}
