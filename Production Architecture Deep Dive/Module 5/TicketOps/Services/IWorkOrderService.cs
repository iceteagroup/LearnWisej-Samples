using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Validation;

namespace TicketOps.Services
{
    /// <summary>
    /// The contract the editor calls. The whole save pipeline sits behind <see cref="SaveAsync"/>:
    /// validate → rules and authorization → persist atomically → confirm. The screen never learns how.
    /// </summary>
    public interface IWorkOrderService
    {
        Task<IReadOnlyList<WorkOrder>> GetWorkOrdersAsync();

        /// <summary>
        /// The safe save pipeline. Validation and rule failures come back as an Invalid result with every
        /// error collected; an unreachable store surfaces as an exception AFTER the transaction rolled back,
        /// so the caller can promise the user that nothing was partially written.
        /// </summary>
        Task<SaveResult> SaveAsync(SaveWorkOrderCommand command, SessionContext session);
    }
}
