using AccessOps.Infrastructure;
using AccessOps.Security;
using Microsoft.Extensions.Logging;
using Wisej.Web;

namespace AccessOps.Identity
{
    /// <summary>
    /// The two halves of an AccessOps sign-in. The click (a WebSocket event) only verifies and
    /// parks the <see cref="VerifiedSignIn"/>; the page load that <c>Application.Reload()</c>
    /// causes rotates the session ID, establishes the principal and asks the browser to fetch the
    /// host cookie. The parked value is not an identity yet: nothing reads the pending key except
    /// <see cref="CompleteOnPageLoad"/>.
    /// </summary>
    public static class SignInHandoff
    {
        /// <summary>The <c>Application.Session</c> key of the verified, not yet established sign-in.</summary>
        public const string PendingKey = "PendingSignIn";

        /// <summary>
        /// Runs on the sign-in click after <see cref="LoginService.SignIn"/> succeeded: parks the
        /// result and reloads the tab. No <c>RefreshSessionId()</c> here - from a WebSocket click
        /// the browser never adopts the new ID.
        /// </summary>
        public static void Park(VerifiedSignIn signIn)
        {
            Application.Session[PendingKey] = signIn;
            Application.Reload();
        }

        /// <summary>
        /// Runs in <c>Application.ApplicationRefresh</c> (a page load, HTTP). Takes the parked
        /// sign-in, clears the key, calls <c>Application.RefreshSessionId()</c> first, rechecks the
        /// grant, then <see cref="ApplicationPrincipal.Establish"/>. Returns true when the caller
        /// should replace the login view with the console.
        /// </summary>
        public static bool CompleteOnPageLoad()
        {
            if (Application.Session[PendingKey] is not VerifiedSignIn signIn)
                return false;

            Application.Session[PendingKey] = null;
            return EstablishOnPageLoad(signIn);
        }

        /// <summary>
        /// The shared second half of every sign-in route (password, OpenID Connect hand-off): on a
        /// page load, rotate the session ID, recheck the grant, set <c>Application.User</c> and ask
        /// the browser to fetch the host cookie. Returns false when the grant is no longer active.
        /// </summary>
        public static bool EstablishOnPageLoad(VerifiedSignIn signIn)
        {
            var log = AppHost.Get<ILoggerFactory>().CreateLogger("AccessOps.SignIn");

            var before = Application.SessionId;
            Application.RefreshSessionId();                       // new ID first (HTTP load)
            var rotated = !string.Equals(before, Application.SessionId, System.StringComparison.Ordinal);
            log.LogInformation("page load · RefreshSessionId() · rotated={Rotated} · isWebSocket={IsWebSocket} · method={Method}", rotated, Application.IsWebSocket, signIn.Method);

            // the grant could have been revoked between the click and this load
            if (!AppHost.Get<GrantStore>().IsActive(signIn.GrantId))
            {
                log.LogWarning("Pending sign-in dropped: grant {Grant} is no longer active", Grant.Shorten(signIn.GrantId));
                return false;
            }

            ApplicationPrincipal.Establish(signIn);

            // Module 7: durable evidence for the release review (case 6). Opaque subject and the
            // outcome only - neither session ID is stored anywhere.
            AppHost.Get<AuditService>().Write(new AuditEvent(Subject.From(Application.User).Opaque, "session.rotate", "session", null,
                rotated ? "rotated" : "not-rotated", signIn.Method, AuditService.NewCorrelationId()));
            log.LogInformation("ApplicationPrincipal.Establish · subject={Subject} · grant={Grant}",
                Subject.From(Application.User).Opaque, Grant.Shorten(signIn.GrantId));

            // The host cookie cannot ride a WebSocket frame: the browser fetches it from the HTTPS
            // endpoint with a single-use handle (an argument, never concatenated into script).
            var handle = AppHost.Get<CookieHandoffStore>().Create(signIn);
            Application.Call("AccessOpsAuth.issueCookie", handle);
            return true;
        }
    }
}
