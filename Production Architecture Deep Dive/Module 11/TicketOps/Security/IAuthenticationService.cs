using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Security
{
    /// <summary>
    /// The login gate's contract: verify a credential on the server and bind the identity to the session.
    /// Authentication answers "who is this?" once; authorization (<see cref="IPermissionService"/>) is asked
    /// again on every sensitive action.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Wrong user name or wrong password come back as one failed result with the same neutral message
        /// (never "user not found" vs "wrong password"). A store outage is an exception for the handler to catch.
        /// </summary>
        Task<OperationResult<IUserContext>> SignInAsync(string userName, string password);

        void SignOut();
    }
}
