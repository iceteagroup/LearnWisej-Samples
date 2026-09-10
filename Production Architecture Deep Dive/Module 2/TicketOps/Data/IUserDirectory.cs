using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Data
{
    /// <summary>
    /// The user store (in production: Active Directory / an identity provider). It answers "who is this
    /// person and which tenants may they work in" — the user scope. It never knows about sessions.
    /// </summary>
    public interface IUserDirectory
    {
        Task<IReadOnlyList<UserAccount>> GetUsersAsync();
        Task<UserAccount> FindAsync(string name);
        Task<IReadOnlyList<string>> GetTenantsAsync();
    }
}
