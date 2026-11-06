using System;

namespace AccessOps.Services
{
    public enum SignOutReason { Explicit, Idle, TokenExpired }

    /// <summary>
    /// The one sign-out routine. Called by the Sign out button and by the
    /// Application.SessionTimeout handler, so the two can never drift apart. Order: audit while the
    /// identity is still known, clear the context, end the session
    /// (Application.Exit through <see cref="IAccessOpsSession.End"/>).
    /// </summary>
    public sealed class SignOutService
    {
        private readonly IAccessOpsSession _session;
        private readonly LoginAuditService _audit;

        public SignOutService(IAccessOpsSession session, LoginAuditService audit)
        {
            _session = session;
            _audit = audit;
        }

        /// <param name="reason">Explicit (the button) or Idle (Application.SessionTimeout).</param>
        /// <param name="endSession">
        /// How to end the session once the identity is gone. Null ends it at once; the UI passes a
        /// continuation so the browser can be told the reason first (see SignOutFlow).
        /// </param>
        public void SignOut(SignOutReason reason, Action<Action> endSession = null)
        {
            var me = _session.TryGetCurrent();
            if (me != null)
            {
                _audit.Write(me.UserName ?? "employee " + me.EmployeeId, _session.ClientAddress,
                    reason == SignOutReason.Idle ? LoginOutcome.SignOutIdle
                    : reason == SignOutReason.TokenExpired ? LoginOutcome.TokenExpired
                    : LoginOutcome.SignOutExplicit);
            }

            _session.Clear();

            if (endSession == null)
                _session.End();
            else
                endSession(_session.End);
        }
    }
}
