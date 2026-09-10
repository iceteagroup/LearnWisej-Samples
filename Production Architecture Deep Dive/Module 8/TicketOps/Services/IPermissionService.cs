using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// Answers "may the current operator do X?". Lifetime: Session — it depends on the session-scoped
    /// <see cref="IUserService"/>, and a service can never outlive a collaborator it captured
    /// (a Shared permission service holding a Session user service would leak one user's identity to all).
    /// </summary>
    public interface IPermissionService
    {
        bool CanClose(Ticket ticket);

        bool CanAssign(Ticket ticket, int toOperatorId);
    }
}
