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
            // Idle limit: Default.json "sessionTimeout" (seconds). We replace the built-in prolong
            // dialog with the same sign-out the button uses - a prolong button anyone walking past
            // can click defeats the limit.
            Application.SessionTimeout += (s, e) =>
            {
                e.Handled = true;
                SignOutFlow.SignOut(SignOutReason.Idle);
            };

            // Page loads (HTTP, not WebSocket) are where the session ID can be renewed and where an
            // HttpOnly cookie can be written. See WisejSession.Establish.
            Application.ApplicationRefresh += (s, e) => OnPageLoad();

            if (TrySignInRememberedDevice())
            {
                Application.MainPage = new ConsolePage();
                return;
            }

            var ended = Application.QueryString["ended"];
            Application.MainPage = new LoginPage(NoticeFor(ended), MetaFor(ended));
        }

        private static void OnPageLoad()
        {
            var session = (WisejSession)AppHost.Get<IAccessOpsSession>();
            if (session.CompletePendingSignIn())
            {
                var login = Application.MainPage;
                Application.MainPage = new ConsolePage();
                login?.Dispose();
            }

            WritePendingDeviceCookie();
        }

        private static bool TrySignInRememberedDevice()
        {
            var token = Application.Cookies[RememberedDeviceService.CookieName];
            if (string.IsNullOrEmpty(token))
                return false;

            var employeeId = AppHost.Get<RememberedDeviceService>().Redeem(token);
            if (employeeId == null)
                return false;

            return AppHost.Get<LoginService>().SignInRememberedDevice(employeeId.Value).Succeeded;
        }

        private static void WritePendingDeviceCookie()
        {
            if (Application.Session["PendingDeviceCookie"] is not PendingDeviceCookie pending)
                return;

            Application.Session["PendingDeviceCookie"] = null;
            Application.Cookies.Add(
                RememberedDeviceService.CookieName, pending.Token, null, pending.ExpiresUtc,
                true, true, Wisej.Base.Cookie.SameSiteMode.Strict);
        }

        // The query value only picks one of two fixed sentences; nothing it carries is displayed.
        private static string NoticeFor(string ended) => ended switch
        {
            "idle" => IdleSentence(Application.Configuration.SessionTimeout),
            "signout" => "You have signed out. Sign in to continue.",
            _ => null
        };

        private static string IdleSentence(int seconds)
        {
            var minutes = Math.Max(1, seconds / 60);
            return $"Your session ended after {minutes} minute{(minutes == 1 ? "" : "s")} of inactivity. Sign in to continue.";
        }

        private static string MetaFor(string ended) => ended switch
        {
            "idle" => "e.Handled = true · context cleared · sign-out audited · Application.Exit()",
            "signout" => "context cleared · sign-out audited · Application.Exit()",
            _ => null
        };
    }
}
