using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketOps.Data
{
    /// <summary>
    /// The fake user directory of the lab: three demo accounts, all with the password the walkthrough video
    /// types ("secret123"). Nothing here is a secret — the point of the module is that even a fake credential
    /// is checked on the server, and that the roles live here, never in the browser.
    /// </summary>
    public sealed class InMemoryUserStore : IUserStore
    {
        public const string DemoPassword = "secret123";

        private readonly Dictionary<string, UserRecord> _users = new Dictionary<string, UserRecord>(StringComparer.OrdinalIgnoreCase);

        public InMemoryUserStore()
        {
            foreach (var u in DemoAccounts())
                _users[u.UserName] = u;
        }

        public Task<UserRecord> FindAsync(string userName)
        {
            _users.TryGetValue(userName ?? string.Empty, out var record);
            return Task.FromResult(record);
        }

        public Task<IReadOnlyList<UserRecord>> GetAllAsync()
        {
            IReadOnlyList<UserRecord> all = _users.Values.OrderBy(u => u.UserName).ToList();
            return Task.FromResult(all);
        }

        /// <summary>The accounts of the lab. Roles are the only thing that differs.</summary>
        public static IEnumerable<UserRecord> DemoAccounts()
        {
            yield return new UserRecord { UserName = "l.romero", DisplayName = "L. Romero", Roles = new[] { Security.Roles.Technician }, DemoPassword = DemoPassword };
            yield return new UserRecord { UserName = "m.weber", DisplayName = "M. Weber", Roles = new[] { Security.Roles.Supervisor }, DemoPassword = DemoPassword };
            yield return new UserRecord { UserName = "s.okafor", DisplayName = "S. Okafor", Roles = new[] { Security.Roles.Admin }, DemoPassword = DemoPassword };
        }
    }
}
