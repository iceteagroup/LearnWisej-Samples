using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// The two halves of the server-confirmed clipboard copy (the video's <c>TicketLinkService</c>).
    ///
    /// 1. <see cref="BuildLinkAsync"/> — before the browser touches the clipboard: re-load the work order
    ///    (the id came from the screen and is client input), apply the domain rule, build and sign the
    ///    canonical URL. Refusals come back as a failed result, never as an exception.
    /// 2. <see cref="ConfirmCopiedAsync"/> — after the browser reported success: write the audit entry.
    ///    A copy the browser did not confirm is never recorded.
    /// </summary>
    public interface ITicketLinkService
    {
        Task<OperationResult<TicketLink>> BuildLinkAsync(int workOrderId, SessionUser user);

        Task ConfirmCopiedAsync(int workOrderId, SessionUser user);
    }
}
