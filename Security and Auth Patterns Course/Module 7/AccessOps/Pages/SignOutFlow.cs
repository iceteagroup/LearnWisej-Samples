using AccessOps.Services;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// The UI side of signing out. <see cref="SignOutService"/> audits and clears the context at
    /// once; the session ends with Application.Exit() one round trip later:
    ///
    /// 1. <c>Application.Call("accessOpsEnded", callback, word)</c> runs a function in Default.html
    ///    that keeps the word in the tab's sessionStorage (an argument, never concatenated script);
    /// 2. the callback calls Application.Exit();
    /// 3. the client drops its session ID and reloads ("autoReload": true). Default.html turns the
    ///    word into ?ended=... - or, for a session that signed in through the identity provider,
    ///    first sends the browser to /account/signout, which ends the provider's session too and
    ///    comes back to Default.html?ended=... (sign-out covering both sessions).
    /// </summary>
    internal static class SignOutFlow
    {
        public static void SignOut(SignOutReason reason)
        {
            var external = AppHost.Get<IExternalSignIn>().Current != null;
            var word = (reason == SignOutReason.Idle ? "idle" : reason == SignOutReason.TokenExpired ? "expired" : "signout")
                + (external ? "-external" : "");
            // Module 7: the provider sign-out needs a one-time ticket from this session (logout CSRF fix)
            var ticket = external ? AppHost.Get<AccessOps.Infrastructure.SignOutTickets>().Issue() : "";
            AppHost.Get<SignOutService>().SignOut(reason,
                end => Application.Call("accessOpsEnded", _ => end(), word, ticket));
        }
    }
}
