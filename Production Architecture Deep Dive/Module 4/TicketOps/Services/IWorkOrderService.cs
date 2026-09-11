using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// The contract the Work Orders screen calls. Everything a work order "means" — what is valid, when
    /// its edits become the saved state, which rows match a search — lives behind this interface, so the
    /// handlers stay thin and a test can exercise the workflow with a fake repository and no browser.
    /// No Wisej.NET type appears here: the service hands back plain objects and results; the screen
    /// binds them.
    /// </summary>
    public interface IWorkOrderService
    {
        /// <summary>Loads every work order as a fresh, clean (not dirty) observable object.</summary>
        Task<IReadOnlyList<WorkOrder>> LoadAsync();

        /// <summary>
        /// Applies the query to the given rows and returns the matches, in order. Pure and synchronous:
        /// it filters the live objects the screen already holds (so unsaved edits survive a filter), it
        /// does not go back to the store. An empty result is a normal outcome, not a failure.
        /// </summary>
        IReadOnlyList<WorkOrder> Filter(IEnumerable<WorkOrder> orders, WorkOrderQuery query);

        /// <summary>
        /// Validates the edited object and, if valid, persists it and accepts its changes (IsDirty → false).
        /// Validation failures come back as a failed result with the edits left in place, never as an exception.
        /// </summary>
        Task<OperationResult<WorkOrder>> SaveAsync(WorkOrder order);

        /// <summary>Rolls the object back to its last saved values (IsDirty → false). Nothing is persisted.</summary>
        OperationResult<WorkOrder> Discard(WorkOrder order);
    }
}
