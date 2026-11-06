using AccessOps.Services;
using Wisej.Web;

namespace AccessOps.Infrastructure
{
    /// <summary>
    /// <see cref="IAccessOpsSession"/> over Wisej.NET: the context lives in
    /// <c>Application.Session</c>, which is per browser session (per tab, with the default
    /// sessionStorage setting). One instance serves every session - it holds no state itself and
    /// always reads the session of the thread that calls it.
    /// </summary>
    public sealed class WisejSession : IAccessOpsSession
    {
        private const string Key = "SessionContext";
        private const string PendingKey = "PendingSessionContext";

        public SessionContext Current => TryGetCurrent() ?? throw new NotSignedInException();

        public SessionContext TryGetCurrent() => Application.Session[Key] as SessionContext;

        public string ClientAddress => Application.UserHostAddress ?? "unknown";

        public string UserAgent => Application.UserAgent ?? "";

        /// <summary>
        /// A new session ID first, then the identity. Verified on Wisej-4 4.1.7: called during a
        /// page load (an HTTP request) <c>Application.RefreshSessionId()</c> hands the browser the new
        /// ID and the session carries on; called from a WebSocket event (a button click) the browser
        /// never adopted the new ID and its next request started a fresh session. So a sign-in that
        /// arrives on a click parks the verified context - it is not the session identity yet - and
        /// reloads; <see cref="CompletePendingSignIn"/> finishes the job on that page load.
        /// </summary>
        public void Establish(SessionContext context)
        {
            if (Application.IsWebSocket)
            {
                Application.Session[Key] = null;
                Application.Session[PendingKey] = context;
                Application.Reload();
                return;
            }

            Application.RefreshSessionId();
            Application.Session[Key] = context;
        }

        /// <summary>
        /// Called from Application.ApplicationRefresh (a page load): new session ID, then the parked
        /// identity becomes the session's identity. Returns true when it did.
        /// </summary>
        public bool CompletePendingSignIn()
        {
            if (Application.Session[PendingKey] is not SessionContext pending)
                return false;

            Application.Session[PendingKey] = null;
            Application.RefreshSessionId();
            Application.Session[Key] = pending;
            return true;
        }

        public void Replace(SessionContext context)
        {
            if (TryGetCurrent() != null)
                Application.Session[Key] = context;
        }

        public void Clear()
        {
            Application.Session[Key] = null;
            Application.Session[PendingKey] = null;
        }

        public void End() => Application.Exit();
    }
}
