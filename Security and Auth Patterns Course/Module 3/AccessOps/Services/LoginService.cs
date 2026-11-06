using System;
using System.Linq;

namespace AccessOps.Services
{
    public sealed class LoginResult
    {
        private LoginResult(bool succeeded, LoginOutcome outcome, int employeeId)
        {
            this.Succeeded = succeeded;
            this.Outcome = outcome;
            this.EmployeeId = employeeId;
        }

        public bool Succeeded { get; }

        /// <summary>The real reason. For the audit table only; the screen shows one sentence.</summary>
        public LoginOutcome Outcome { get; }

        public int EmployeeId { get; }

        public static LoginResult Verified(int employeeId, LoginOutcome outcome) => new(true, outcome, employeeId);
        public static LoginResult Refused(LoginOutcome outcome) => new(false, outcome, 0);
    }

    /// <summary>
    /// The only path that signs anybody in. Throttle, verify, audit every outcome; on success a
    /// fresh session ID and only then the <see cref="SessionContext"/>.
    /// </summary>
    public sealed class LoginService
    {
        private readonly EmployeeRepository _employees;
        private readonly PasswordService _passwords;
        private readonly LoginThrottle _throttle;
        private readonly LoginAuditService _audit;
        private readonly IAccessOpsSession _session;
        private readonly PermissionService _permissions;

        public LoginService(EmployeeRepository employees, PasswordService passwords, LoginThrottle throttle,
            LoginAuditService audit, IAccessOpsSession session, PermissionService permissions)
        {
            _permissions = permissions;
            _employees = employees;
            _passwords = passwords;
            _throttle = throttle;
            _audit = audit;
            _session = session;
        }

        /// <summary>Failures recorded against a user name in the current window (for the footer).</summary>
        public int FailuresFor(string userName) => _throttle.FailuresFor(Normalize(userName));

        public LoginResult SignIn(string userName, string password)
        {
            var name = Normalize(userName);
            var address = _session.ClientAddress;

            // locked: refused before the hash is computed
            if (_throttle.IsLocked(name, address))
                return Refuse(name, address, LoginOutcome.LockedOut);

            // an unknown name still pays for one full derivation, so it is not faster to refuse
            var stored = _employees.FindCredential(name);
            var verified = _passwords.Verify(password ?? "", stored ?? StoredCredential.Dummy);

            if (stored == null)
            {
                _throttle.RegisterFailure(name, address);
                return Refuse(name, address, LoginOutcome.UnknownUser);
            }
            if (!verified)
            {
                _throttle.RegisterFailure(name, address);
                return Refuse(name, address, LoginOutcome.BadCredential);
            }

            var employee = _employees.Find(stored.EmployeeId);
            if (employee == null || employee.IsDisabled)
                return Refuse(name, address, LoginOutcome.Disabled);

            _throttle.Reset(name);
            _audit.Write(name, address, LoginOutcome.Success);
            _session.Establish(BuildContext(employee, "Password"));
            return LoginResult.Verified(employee.Id, LoginOutcome.Success);
        }

        /// <summary>Signs in from a remembered-device token already redeemed by the caller.</summary>
        public LoginResult SignInRememberedDevice(int employeeId)
        {
            var address = _session.ClientAddress;
            var employee = _employees.Find(employeeId);
            if (employee == null || employee.IsDisabled)
                return Refuse(employee?.UserName ?? "(device token)", address, LoginOutcome.Disabled);

            _audit.Write(employee.UserName, address, LoginOutcome.RememberedDevice);
            _session.Establish(BuildContext(employee, "RememberedDevice"));
            return LoginResult.Verified(employee.Id, LoginOutcome.RememberedDevice);
        }

        private SessionContext BuildContext(EmployeeAccount employee, string method)
        {
            var roles = (employee.Roles ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return new SessionContext(
                employee.Id, employee.UserName, employee.DisplayName, employee.Department, employee.IsManager,
                roles, DateTime.UtcNow, _session.ClientAddress, _session.UserAgent, method,
                _permissions.ResolveFor(employee), _employees.DirectReports(employee.Id), DateTime.UtcNow);
        }

        private LoginResult Refuse(string name, string address, LoginOutcome outcome)
        {
            _audit.Write(name, address, outcome);
            return LoginResult.Refused(outcome);
        }

        private static string Normalize(string userName) => (userName ?? "").Trim().ToLowerInvariant();
    }
}
