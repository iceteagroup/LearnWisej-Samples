using System;
using AccessOps.Infrastructure;
using AccessOps.Security;

namespace AccessOps.Identity
{
    /// <summary>The answer the login view gets: success, or the one generic sentence.</summary>
    public sealed class SignInResult
    {
        /// <summary>The one sentence every refusal shows - bad password, unknown name, disabled account, unapproved tenant, lockout.</summary>
        public const string GenericFailure = "That user name and password combination is not valid.";

        /// <summary>True when the sign-in transaction completed and a grant exists.</summary>
        public bool Succeeded { get; init; }

        /// <summary>What the screen shows (null on success).</summary>
        public string Message { get; init; }

        /// <summary>The real outcome. For the audit and the tests only - never shown.</summary>
        public LoginOutcome Outcome { get; init; }

        /// <summary>The verified sign-in to park (success only).</summary>
        public VerifiedSignIn SignIn { get; init; }

        /// <summary>Failures now counted for the typed name (the mock's "SERVER" line).</summary>
        public int Failures { get; init; }

        /// <summary>Correlates the result with its login_audit row.</summary>
        public string CorrelationId { get; init; }
    }

    /// <summary>
    /// The partner sign-in transaction: throttle, verify, resolve the account and tenant, apply
    /// policy, create the login grant - and only then hand a <see cref="VerifiedSignIn"/> back.
    /// Any failed step ends the attempt with no state left behind and the same sentence. The
    /// password goes nowhere: not into the session, a log message, an audit row or an exception.
    /// </summary>
    public sealed class LoginService
    {
        private readonly AccountStore _accounts;
        private readonly PasswordHasher _hasher;
        private readonly LoginThrottle _throttle;
        private readonly GrantStore _grants;
        private readonly LoginAudit _audit;
        private readonly IClock _clock;

        /// <summary>Creates the service.</summary>
        public LoginService(AccountStore accounts, PasswordHasher hasher, LoginThrottle throttle, GrantStore grants, LoginAudit audit, IClock clock)
        {
            _accounts = accounts;
            _hasher = hasher;
            _throttle = throttle;
            _grants = grants;
            _audit = audit;
            _clock = clock;
        }

        /// <summary>The normalized sign-in name (trimmed, lower case).</summary>
        public static string Normalize(string userName) => (userName ?? "").Trim().ToLowerInvariant();

        /// <summary>
        /// Runs the sign-in transaction for <paramref name="userName"/> / <paramref name="password"/>
        /// from <paramref name="clientAddress"/> (<c>Application.UserHostAddress</c>).
        /// </summary>
        public SignInResult SignIn(string userName, string password, string clientAddress)
        {
            var name = Normalize(userName);
            var correlationId = AuditLog.NewCorrelationId();
            var nameKey = Subject.OpaqueValue("u-", name);
            var addressKey = Subject.OpaqueValue("a-", clientAddress ?? "unknown");

            // 1. throttle first. A locked name still costs one hash, like every other attempt.
            if (_throttle.IsLocked(nameKey, addressKey))
            {
                _hasher.Verify(password, _hasher.DummyHash);
                return Refuse(nameKey, addressKey, LoginOutcome.LockedOut, correlationId, countFailure: false);
            }

            // 2. verify: an unknown name is checked against the dummy credential (same cost)
            var account = _accounts.FindByUserName(name);
            var matches = _hasher.Verify(password, account?.PasswordHash ?? _hasher.DummyHash);
            if (account == null)
                return Refuse(nameKey, addressKey, LoginOutcome.UnknownUser, correlationId);
            if (!matches || account.PasswordHash == null)
                return Refuse(nameKey, addressKey, LoginOutcome.BadCredential, correlationId);

            // 3. account and tenant policy: a verified password is not the right to use AccessOps
            if (account.Disabled)
                return Refuse(nameKey, addressKey, LoginOutcome.Disabled, correlationId);
            if (!account.TenantApproved)
                return Refuse(nameKey, addressKey, LoginOutcome.TenantNotApproved, correlationId);
            if (!MayUsePasswordForm(account))
                return Refuse(nameKey, addressKey, LoginOutcome.NotAllowedHere, correlationId);

            // 4. the grant, before the principal exists
            _throttle.Reset(nameKey);
            var grant = _grants.Issue(account.Id, account.Tenant, account.Role);
            _audit.Write(nameKey, addressKey, LoginOutcome.Succeeded, correlationId, grant.Display);

            return new SignInResult
            {
                Succeeded = true,
                Outcome = LoginOutcome.Succeeded,
                CorrelationId = correlationId,
                SignIn = new VerifiedSignIn
                {
                    AccountId = account.Id,
                    TenantId = account.Tenant,
                    GrantId = grant.Id,
                    Roles = new[] { account.Role },
                    DisplayName = account.DisplayName,
                    Method = "password",
                    VerifiedAt = _clock.Now
                }
            };
        }

        /// <summary>
        /// Which accounts may use the password form. Module 5: only named service accounts
        /// (svc.reports). Partners sign in at their own organisation; an interactive account gets
        /// the generic failure even with the right password, so the fallback is never a second way in.
        /// </summary>
        private static bool MayUsePasswordForm(Account account) => account.IsService;

        private SignInResult Refuse(string nameKey, string addressKey, LoginOutcome outcome, string correlationId, bool countFailure = true)
        {
            if (countFailure)
                _throttle.RecordFailure(nameKey, addressKey);
            _audit.Write(nameKey, addressKey, outcome, correlationId);
            return new SignInResult
            {
                Succeeded = false,
                Message = SignInResult.GenericFailure,
                Outcome = outcome,
                Failures = _throttle.Failures(nameKey),
                CorrelationId = correlationId
            };
        }

        /// <summary>Failures counted for a typed name (for the login view's server line).</summary>
        public int FailuresFor(string userName) => _throttle.Failures(Subject.OpaqueValue("u-", Normalize(userName)));
    }
}
