using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketOps.Domain;
using TicketOps.Infrastructure;

namespace TicketOps.Data
{
    /// <summary>
    /// Raised by the data layer when the directory is unreachable. Its message is deliberately internal
    /// (host names, ports): it belongs in the log, and the screen must not show it.
    /// </summary>
    public sealed class DirectoryOutageException : Exception
    {
        public DirectoryOutageException(string message) : base(message) { }
    }

    /// <summary>
    /// Fake user store with four operators and three tenants. The list is immutable after construction,
    /// so it could safely be shared by every session; here one instance per session keeps the trace
    /// per session and lets the lab's outage switch affect only the tab that pressed it.
    /// </summary>
    public sealed class InMemoryUserDirectory : IUserDirectory
    {
        private readonly ILog _log;
        private readonly IReadOnlyList<UserAccount> _users = new List<UserAccount>
        {
            new UserAccount("Alice Rivera", "Contoso", "Fabrikam"),
            new UserAccount("Bob Chen", "Contoso"),
            new UserAccount("Sara Patel", "Northwind", "Fabrikam"),
            new UserAccount("Jae Kim", "Fabrikam")
        };

        /// <summary>The lab's error-path switch: while true, every call fails like an unreachable LDAP host would.</summary>
        public bool SimulateOutage { get; set; }

        public InMemoryUserDirectory(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public Task<IReadOnlyList<UserAccount>> GetUsersAsync()
        {
            EnsureAvailable("(&(objectClass=user)(memberOf=TicketOps-Operators))");
            _log.Info(LogLayer.Data, "InMemoryUserDirectory.GetUsersAsync", $"{_users.Count} operators");
            return Task.FromResult(_users);
        }

        public Task<UserAccount> FindAsync(string name)
        {
            EnsureAvailable($"(&(objectClass=user)(displayName={name}))");
            var user = _users.FirstOrDefault(u => string.Equals(u.Name, name, StringComparison.OrdinalIgnoreCase));
            _log.Info(LogLayer.Data, "InMemoryUserDirectory.FindAsync", user == null ? $"'{name}' not found" : $"{user}");
            return Task.FromResult(user);
        }

        public Task<IReadOnlyList<string>> GetTenantsAsync()
        {
            EnsureAvailable("(objectClass=organizationalUnit)");
            IReadOnlyList<string> tenants = _users.SelectMany(u => u.Tenants).Distinct().OrderBy(t => t).ToList();
            _log.Info(LogLayer.Data, "InMemoryUserDirectory.GetTenantsAsync", $"{tenants.Count} tenants");
            return Task.FromResult(tenants);
        }

        private void EnsureAvailable(string query)
        {
            if (!SimulateOutage)
                return;

            // What a real directory client would say — and exactly what must not reach the user.
            _log.Error(LogLayer.Data, "InMemoryUserDirectory", null,
                $"outage: LDAP query {query} failed — LDAP://dc01.ticketops.local:636 did not answer (timeout 5000 ms)");
            throw new DirectoryOutageException("The LDAP server LDAP://dc01.ticketops.local:636 is not available while executing: " + query);
        }
    }
}
