using System;

namespace AccessOps.Services
{
    public enum SignOutReason { Explicit, Idle }

    /// <summary>
    /// The one sign-out routine. Called by the Sign out button and by the
    /// Application.SessionTimeout handler, so the two can never drift apart. Order: audit while the
    /// identity is still known, revoke the remembered devices, clear the context, end the session
    /// (Application.Exit through <see cref="IAccessOpsSession.End"/>).
    /// </summary>
    public sealed class SignOutService
    {
        private readonly IAccessOpsSession _session;
        private readonly LoginAuditService _audit;
        private readonly RememberedDeviceService _devices;

        public SignOutService(IAccessOpsSession session, LoginAuditService audit, RememberedDeviceService devices)
        {
            _session = session;
            _audit = audit;
            _devices = devices;
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
                _audit.Write(me.UserName, _session.ClientAddress,
                    reason == SignOutReason.Idle ? LoginOutcome.SignOutIdle : LoginOutcome.SignOutExplicit);

                // a remembered device would sign the next person straight back in: revoke it
                _devices.RevokeAll(me.EmployeeId);
            }

            _session.Clear();

            if (endSession == null)
                _session.End();
            else
                endSession(_session.End);
        }
    }
}
