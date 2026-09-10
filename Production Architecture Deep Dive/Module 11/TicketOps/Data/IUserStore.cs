using System.Collections.Generic;
using System.Threading.Tasks;

namespace TicketOps.Data
{
    /// <summary>
    /// What the directory knows about a person. <see cref="DemoPassword"/> exists only because this lab has
    /// no identity provider: it is a clearly labelled demo constant, never a production pattern.
    /// </summary>
    public sealed class UserRecord
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public IReadOnlyList<string> Roles { get; set; } = new string[0];
        public string DemoPassword { get; set; }
    }

    /// <summary>The user directory contract. The authentication service depends on this, never on a concrete store.</summary>
    public interface IUserStore
    {
        Task<UserRecord> FindAsync(string userName);
        Task<IReadOnlyList<UserRecord>> GetAllAsync();
    }
}
