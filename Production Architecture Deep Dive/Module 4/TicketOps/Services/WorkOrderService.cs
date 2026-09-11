using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketOps.Data;
using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// The in-memory implementation of <see cref="IWorkOrderService"/>. "Fake" describes the storage, not
    /// the rules: validation, the filter and the commit/rollback decisions are the real ones, so the
    /// screen keeps working when the repository becomes a database.
    ///
    /// No control, no Text property, no BindingSource: the service works on plain <see cref="WorkOrder"/>
    /// objects; because they notify, whatever it changes shows up in the bound grid and detail fields
    /// without the screen copying anything.
    /// </summary>
    public sealed class WorkOrderService : IWorkOrderService
    {
        private const int MaxTitleLength = 80;
        private const decimal MaxCost = 250000m;

        private readonly IWorkOrderRepository _repository;

        public WorkOrderService(IWorkOrderRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<IReadOnlyList<WorkOrder>> LoadAsync()
        {
            var rows = await _repository.GetAllAsync();
            foreach (var o in rows)
                o.AcceptChanges();                       // a freshly loaded row is not a change
            return rows;
        }

        public IReadOnlyList<WorkOrder> Filter(IEnumerable<WorkOrder> orders, WorkOrderQuery query)
        {
            if (orders == null) throw new ArgumentNullException(nameof(orders));
            query = query ?? WorkOrderQuery.Everything;
            return orders.Where(query.Matches).ToList();
        }

        public async Task<OperationResult<WorkOrder>> SaveAsync(WorkOrder order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            var errors = Validate(order);
            if (errors.Count > 0)
            {
                // Expected outcome: the screen shows the sentence; the edits stay on screen and the row stays dirty.
                return OperationResult<WorkOrder>.Fail(errors[0], errors.ToArray());
            }

            var stored = await _repository.UpsertAsync(order);

            // Only NOW does the edited object become the saved state. Had the repository thrown, IsDirty would still be true.
            order.Id = stored.Id;
            order.AcceptChanges();
            return OperationResult<WorkOrder>.Ok(order, $"Work order #{order.Id} saved.");
        }

        public OperationResult<WorkOrder> Discard(WorkOrder order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            if (!order.IsDirty)
                return OperationResult<WorkOrder>.Fail("There are no unsaved changes to discard.");

            order.RejectChanges();
            return OperationResult<WorkOrder>.Ok(order, $"Changes to work order #{order.Id} discarded.");
        }

        private static List<string> Validate(WorkOrder order)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(order.Title))
                errors.Add("Title is required.");
            else if (order.Title.Trim().Length > MaxTitleLength)
                errors.Add($"Title must be {MaxTitleLength} characters or fewer.");
            if (order.Cost < 0 || order.Cost > MaxCost)
                errors.Add($"Cost must be between $0 and {MaxCost:N0}.");
            if (order.Status == WorkOrderStatus.InProgress && string.IsNullOrWhiteSpace(order.AssignedTo))
                errors.Add("A work order in progress needs someone assigned.");
            return errors;
        }
    }
}
