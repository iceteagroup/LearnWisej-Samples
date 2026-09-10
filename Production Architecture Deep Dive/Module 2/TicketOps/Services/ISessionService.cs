using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// Every change to this session's <see cref="SessionContext"/> goes through here, so the rules
    /// ("you may only switch to a tenant you belong to", "the theme must be one the deployment offers")
    /// live in one place and the screen stays thin. Expected refusals come back as failed results,
    /// never as exceptions.
    /// </summary>
    public interface ISessionService
    {
        Task<IReadOnlyList<UserAccount>> GetUsersAsync();
        Task<IReadOnlyList<string>> GetTenantsAsync();

        /// <summary>Signs this session in as another operator and moves it to that operator's default tenant.</summary>
        Task<OperationResult<SessionContext>> SignInAsync(string userName);

        /// <summary>Switches this session's tenant if the current operator is a member of it.</summary>
        Task<OperationResult<SessionContext>> SwitchTenantAsync(string tenant);

        /// <summary>Records the theme this session chose (the screen applies it; the service only decides).</summary>
        OperationResult<SessionContext> SelectTheme(string theme);

        /// <summary>Remembers the selected ticket for this session only.</summary>
        void SelectTicket(int? ticketId);

        /// <summary>Progress path: builds a throwaway context the way a new session would, to show ids never collide.</summary>
        SessionContext SimulateAnotherSession(int number);
    }
}
