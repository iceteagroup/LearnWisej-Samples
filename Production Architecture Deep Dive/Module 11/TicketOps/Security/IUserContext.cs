using System.Collections.Generic;

namespace TicketOps.Security
{
    /// <summary>
    /// Who the caller is, as established by the login gate and bound to the server session.
    /// Services read it from <see cref="IUserSession"/> — never from anything the browser sent
    /// (a hidden field, a query argument, a control's Tag). If the client can name who it is,
    /// an attacker can rename themselves Supervisor.
    /// </summary>
    public interface IUserContext
    {
        string UserName { get; }
        string DisplayName { get; }
        IReadOnlyCollection<string> Roles { get; }
        bool IsInRole(string role);
    }
}
