using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketOps.Infrastructure;

namespace TicketOps.Data
{
    /// <summary>
    /// Raised by the user store when the directory is unreachable. Its message is internal (host names)
    /// and belongs in the log; the login screen shows a safe sentence instead.
    /// </summary>
    public sealed class UserStoreOutageException : Exception
    {
        public UserStoreOutageException(string message) : base(message) { }
    }

    /// <summary>
    /// The fake user directory of the lab: three demo accounts, one password each, all equal to the
    /// demo constant "demo". Nothing here is a secret — the point of the module is that even a fake
    /// credential is checked on the server, and that the roles live here, never in the browser.
    /// <see cref="SimulateOutage"/> drives the login gate's error path.
    /// </summary>
    public sealed class InMemoryUserStore : IUserStore
    {
        public const string DemoPassword = "demo";

        private readonly ILog _log;
        private readonly Dictionary<string, UserRecord> _users = new Dictionary<string, UserRecord>(StringComparer.OrdinalIgnoreCase);

        public bool SimulateOutage { get; set; }

        public InMemoryUserStore(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            foreach (var u in DemoAccounts())
                _users[u.UserName] = u;
            _log.Info(LogLayer.Data, "InMemoryUserStore", $"seeded {_users.Count} demo accounts (password \"{DemoPassword}\" for all — lab only)");
        }

        public Task<UserRecord> FindAsync(string userName)
        {
            EnsureAvailable($"LDAP lookup sAMAccountName={userName}");
            _users.TryGetValue(userName ?? string.Empty, out var record);
            _log.Info(LogLayer.Data, "InMemoryUserStore.FindAsync",
                record == null ? "not found" : $"\"{record.UserName}\" found (roles: {string.Join(", ", record.Roles)})");
            return Task.FromResult(record);
        }

        public Task<IReadOnlyList<UserRecord>> GetAllAsync()
        {
            EnsureAvailable("LDAP search (objectClass=user)");
            IReadOnlyList<UserRecord> all = _users.Values.OrderBy(u => u.UserName).ToList();
            return Task.FromResult(all);
        }

        private void EnsureAvailable(string operation)
        {
            if (!SimulateOutage)
                return;

            // What a real directory client would say — and exactly what must not reach the login screen.
            _log.Error(LogLayer.Data, "InMemoryUserStore", null,
                $"outage: {operation} failed — LDAP server dc01.ticketops.local:636 unreachable");
            throw new UserStoreOutageException("LDAP server dc01.ticketops.local:636 unreachable while executing: " + operation);
        }

        /// <summary>The accounts the walkthrough video shows. Roles are the only thing that differs.</summary>
        public static IEnumerable<UserRecord> DemoAccounts()
        {
            yield return new UserRecord { UserName = "l.romero", DisplayName = "L. Romero", Roles = new[] { Security.Roles.Technician }, DemoPassword = DemoPassword };
            yield return new UserRecord { UserName = "m.weber", DisplayName = "M. Weber", Roles = new[] { Security.Roles.Supervisor }, DemoPassword = DemoPassword };
            yield return new UserRecord { UserName = "s.okafor", DisplayName = "S. Okafor", Roles = new[] { Security.Roles.Admin }, DemoPassword = DemoPassword };
        }
    }
}
