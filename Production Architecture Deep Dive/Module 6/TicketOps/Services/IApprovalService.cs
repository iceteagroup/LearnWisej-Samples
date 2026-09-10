using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Dialogs;
using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// The contract the Work Orders screen calls once — and only once — the approval dialog closed with
    /// a confirmed result. "A dialog gathers intent; a service executes the transaction": everything an
    /// approval means (is the order still pending? are comments required? what gets written together?)
    /// lives behind this interface, so a bulk job or an API endpoint can approve without a dialog and a
    /// test can exercise it with a fake repository and no browser.
    /// </summary>
    public interface IApprovalService
    {
        /// <summary>The queue the screen shows: pending first, then decided orders.</summary>
        Task<IReadOnlyList<WorkOrder>> GetQueueAsync();

        /// <summary>
        /// Applies a confirmed dialog result to a work order as one transaction (status + audit + notification
        /// commit together or not at all). Expected refusals — the result is not confirmed, comments are missing
        /// on a rejection, the order was already decided — come back as a failed result, never as an exception.
        /// A store outage is an exception: nothing was written, the caller logs it and shows a safe message.
        /// </summary>
        Task<OperationResult<WorkOrder>> ApplyAsync(int workOrderId, ApprovalDialogResult result);

        /// <summary>The same transaction for callers that have no dialog (bulk job, API): execute a command.</summary>
        Task<OperationResult<WorkOrder>> ExecuteAsync(ApprovalCommand command);
    }
}
