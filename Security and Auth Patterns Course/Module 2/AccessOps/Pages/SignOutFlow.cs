using AccessOps.Services;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// The UI side of signing out. <see cref="SignOutService"/> audits, revokes and clears the
    /// context at once; the session itself ends with Application.Exit() one round trip later:
    ///
    /// 1. <c>Application.Call("accessOpsEnded", callback, reason)</c> runs a function in Default.html
    ///    that keeps the reason in the tab's sessionStorage (a fixed word, passed as an argument -
    ///    never concatenated into script);
    /// 2. the callback calls Application.Exit();
    /// 3. the client drops its session ID and - Default.json "autoReload": true - reloads, and the
    ///    script in Default.html turns the kept reason into ?ended=..., which selects the sentence on
    ///    the new login page.
    ///
    /// Verified on 4.1.7: script sent with Application.Eval in the same request as Application.Exit
    /// never ran - the exit won - and without autoReload the client calls window.close().
    /// </summary>
    internal static class SignOutFlow
    {
        public static void SignOut(SignOutReason reason)
        {
            var word = reason == SignOutReason.Idle ? "idle" : "signout";
            AppHost.Get<SignOutService>().SignOut(reason,
                end => Application.Call("accessOpsEnded", _ => end(), word));
        }
    }
}
