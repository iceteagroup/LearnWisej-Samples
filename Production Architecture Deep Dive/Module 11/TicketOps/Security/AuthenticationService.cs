using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TicketOps.Data;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using TicketOps.Services;

namespace TicketOps.Security
{
    /// <summary>
    /// The demo authentication service. The credential check is deliberately simple (demo constants in an
    /// in-memory store, compared in fixed time) because the lesson is WHERE it happens — on the server,
    /// before an identity is bound to the session — not how passwords are stored. Production replaces this
    /// class with a real identity provider (OpenID Connect / Windows authentication); nothing else changes,
    /// because the rest of the app only knows <see cref="IUserSession"/> and <see cref="IUserContext"/>.
    /// </summary>
    public sealed class AuthenticationService : IAuthenticationService
    {
        private readonly IUserStore _users;
        private readonly IUserSession _session;
        private readonly IAuditService _audit;
        private readonly ILog _log;

        public AuthenticationService(IUserStore users, IUserSession session, IAuditService audit, ILog log)
        {
            _users = users ?? throw new ArgumentNullException(nameof(users));
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _audit = audit ?? throw new ArgumentNullException(nameof(audit));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<OperationResult<IUserContext>> SignInAsync(string userName, string password)
        {
            userName = (userName ?? string.Empty).Trim();
            password = password ?? string.Empty;

            // The password itself is never logged — not here, not in the audit trail.
            _log.Info(LogLayer.Service, "AuthenticationService.SignInAsync", $"→ IUserStore.FindAsync(\"{HtmlPolicy.Encode(userName)}\") · password: {password.Length} chars, not logged");

            if (userName.Length == 0 || password.Length == 0)
            {
                _log.Warn(LogLayer.Service, "AuthenticationService.SignInAsync", "rejected: empty user name or password");
                return OperationResult<IUserContext>.Fail(Strings.SignInFailed);
            }

            var record = await _users.FindAsync(userName);
            if (record == null || !PasswordMatches(record, password))
            {
                // One neutral message for both cases: the response must not tell an attacker which half was right.
                string why = record == null ? "unknown user" : "wrong password";
                _log.Warn(LogLayer.Service, "AuthenticationService.SignInAsync", $"rejected: {why} → the user sees the same neutral message either way");
                _audit.Denied(null, "SignIn", HtmlPolicy.Encode(userName), why);
                return OperationResult<IUserContext>.Fail(Strings.SignInFailed);
            }

            _log.Info(LogLayer.Service, "AuthenticationService.SignInAsync", "password compared on the server (fixed-time) → match");

            var user = new UserContext(record.UserName, record.DisplayName, record.Roles);
            _session.SignIn(user);                                   // identity → server session (the browser cannot forge this)
            _audit.Success(user, "SignIn", "session", "session established");

            return OperationResult<IUserContext>.Ok(user, $"Signed in as {user.DisplayName}.");
        }

        public void SignOut()
        {
            var user = _session.User;
            _log.Info(LogLayer.Service, "AuthenticationService.SignOut", user == null ? "nobody signed in" : $"→ IUserSession.SignOut ({user.UserName})");
            _session.SignOut();
            if (user != null)
                _audit.Success(user, "SignOut", "session", "session identity cleared");
        }

        /// <summary>
        /// Fixed-time comparison so the response time does not leak how many characters matched.
        /// Production compares a salted hash from the identity provider, never a stored plain password.
        /// </summary>
        private static bool PasswordMatches(UserRecord record, string password)
        {
            byte[] expected = Encoding.UTF8.GetBytes(record.DemoPassword ?? string.Empty);
            byte[] actual = Encoding.UTF8.GetBytes(password);
            return expected.Length == actual.Length && CryptographicOperations.FixedTimeEquals(expected, actual);
        }
    }
}
