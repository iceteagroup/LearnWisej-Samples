using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Data
{
    /// <summary>
    /// Fake user store with four operators and three tenants. The list is immutable after construction,
    /// so it could safely be shared by every session.
    /// </summary>
    public sealed class InMemoryUserDirectory : IUserDirectory
    {
        private readonly IReadOnlyList<UserAccount> _users = new List<UserAccount>
        {
            new UserAccount("Alice Rivera", "Contoso", "Fabrikam"),
            new UserAccount("Bob Chen", "Contoso"),
            new UserAccount("Sara Patel", "Northwind", "Fabrikam"),
            new UserAccount("Jae Kim", "Fabrikam")
        };

        public Task<IReadOnlyList<UserAccount>> GetUsersAsync()
        {
            return Task.FromResult(_users);
        }

        public Task<UserAccount> FindAsync(string name)
        {
            var user = _users.FirstOrDefault(u => string.Equals(u.Name, name, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(user);
        }

        public Task<IReadOnlyList<string>> GetTenantsAsync()
        {
            IReadOnlyList<string> tenants = _users.SelectMany(u => u.Tenants).Distinct().OrderBy(t => t).ToList();
            return Task.FromResult(tenants);
        }
    }
}
