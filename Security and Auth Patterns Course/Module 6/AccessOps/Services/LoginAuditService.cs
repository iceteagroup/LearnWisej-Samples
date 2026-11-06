namespace AccessOps.Services
{
    /// <summary>The real reason behind a sign-in result. The screen never shows it; the audit trail does.</summary>
    public enum LoginOutcome
    {
        Success,
        RememberedDevice,
        BadCredential,
        UnknownUser,
        LockedOut,
        Disabled,
        SignOutExplicit,
        SignOutIdle,
        NotServiceAccount,
        ExternalSuccess,
        ExternalProvisioned,
        ExternalRejected,
        ExternalFailed,
        TokenExpired
    }

    /// <summary>
    /// Sign-in attempts and sign-outs, written to the audit trail (Module 6 replaces Module 2's
    /// separate login_audit table with rows in audit_entry). The user name tried is the target;
    /// never the password - not even a failed one.
    /// </summary>
    public sealed class LoginAuditService
    {
        private readonly AuditService _audit;
        private readonly IAccessOpsSession _session;

        public LoginAuditService(AuditService audit, IAccessOpsSession session)
        {
            _audit = audit;
            _session = session;
        }

        public void Write(string userNameTried, string clientAddress, LoginOutcome outcome)
        {
            var me = _session.TryGetCurrent();
            var ctx = me != null && (outcome == LoginOutcome.SignOutExplicit || outcome == LoginOutcome.SignOutIdle || outcome == LoginOutcome.TokenExpired)
                ? CommandContext.For(me)
                : CommandContext.Anonymous("anonymous", clientAddress, _session.UserAgent);

            var name = userNameTried ?? "";
            if (name.Length > 64)
                name = name.Substring(0, 64);

            var action = outcome is LoginOutcome.SignOutExplicit or LoginOutcome.SignOutIdle or LoginOutcome.TokenExpired
                ? "Session.SignOut" : "Session.SignIn";
            if (action == "Session.SignOut" && me != null)
                _audit.Record(ctx, action, "Employee", me.EmployeeId.ToString(System.Globalization.CultureInfo.InvariantCulture), outcome.ToString());
            else
                _audit.Record(ctx, action, "Account", name, outcome.ToString());
        }
    }
}
