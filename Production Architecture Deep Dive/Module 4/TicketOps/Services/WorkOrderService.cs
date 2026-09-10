using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketOps.Data;
using TicketOps.Domain;
using TicketOps.Infrastructure;

namespace TicketOps.Services
{
    /// <summary>
    /// The in-memory implementation of <see cref="IWorkOrderService"/>. "Fake" describes the storage, not
    /// the rules: validation, the filter and the commit/rollback decisions are the real ones, so the
    /// screen keeps working when the repository becomes a database.
    ///
    /// Notice what is NOT here: no control, no Text property, no BindingSource. The service works on plain
    /// <see cref="WorkOrder"/> objects; because they notify, whatever it changes shows up in the bound
    /// grid and detail fields without the screen copying anything.
    /// </summary>
    public sealed class WorkOrderService : IWorkOrderService
    {
        private const int MaxTitleLength = 80;
        private const decimal MaxCost = 250000m;

        private static readonly string[] ImportTitles =
        {
            "Inspect roof drains", "Service backup generator", "Replace fire extinguishers", "Repair parking gate",
            "Clean cooling tower", "Test emergency lighting", "Re-seal loading dock door", "Calibrate boiler controls",
            "Replace lobby carpet", "Fix conference room AV"
        };

        private static readonly string[] ImportPeople = { "R. Alvarez", "T. Nguyen", "S. Patel", "J. Kim", "M. Chen", "" };

        private readonly IWorkOrderRepository _repository;
        private readonly ILog _log;

        public WorkOrderService(IWorkOrderRepository repository, ILog log)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<IReadOnlyList<WorkOrder>> LoadAsync()
        {
            _log.Info(LogLayer.Service, "WorkOrderService.LoadAsync", "→ IWorkOrderRepository.GetAllAsync()");
            var rows = await _repository.GetAllAsync();
            foreach (var o in rows)
                o.AcceptChanges();                       // a freshly loaded row is not a change
            _log.Info(LogLayer.Service, "WorkOrderService.LoadAsync", $"{rows.Count} work orders, all clean (IsDirty = false)");
            return rows;
        }

        public IReadOnlyList<WorkOrder> Filter(IEnumerable<WorkOrder> orders, WorkOrderQuery query)
        {
            if (orders == null) throw new ArgumentNullException(nameof(orders));
            query = query ?? WorkOrderQuery.Everything;

            var all = orders as IReadOnlyList<WorkOrder> ?? orders.ToList();
            var matches = all.Where(query.Matches).ToList();

            if (matches.Count == 0 && all.Count > 0)
                _log.Info(LogLayer.Service, "WorkOrderService.Filter", $"{query} → 0 of {all.Count} match — an empty result, not an error");
            else
                _log.Info(LogLayer.Service, "WorkOrderService.Filter", $"{query} → {matches.Count} of {all.Count} match (WorkOrderQuery.Matches, in memory; master list untouched)");
            return matches;
        }

        public async Task<OperationResult<WorkOrder>> SaveAsync(WorkOrder order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            _log.Info(LogLayer.Service, "WorkOrderService.SaveAsync", $"validate {order}");

            var errors = Validate(order);
            if (errors.Count > 0)
            {
                // Expected outcome: the screen shows the sentence; the edits stay on screen and the row stays dirty.
                _log.Warn(LogLayer.Service, "WorkOrderService.SaveAsync", $"rejected: {string.Join("; ", errors)} — edits kept, IsDirty stays true");
                return OperationResult<WorkOrder>.Fail(errors[0], errors.ToArray());
            }

            _log.Info(LogLayer.Service, "WorkOrderService.SaveAsync", $"valid → IWorkOrderRepository.UpsertAsync(#{order.Id})");
            var stored = await _repository.UpsertAsync(order);

            // Only NOW does the edited object become the saved state. Had the repository thrown, IsDirty would still be true.
            order.Id = stored.Id;
            order.AcceptChanges();
            _log.Info(LogLayer.Domain, "WorkOrder.AcceptChanges", $"#{order.Id} snapshot taken → IsDirty = false");
            return OperationResult<WorkOrder>.Ok(order, $"Work order #{order.Id} saved.");
        }

        public OperationResult<WorkOrder> Discard(WorkOrder order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            if (!order.IsDirty)
            {
                _log.Info(LogLayer.Service, "WorkOrderService.Discard", $"#{order.Id} has no unsaved changes — nothing to discard");
                return OperationResult<WorkOrder>.Fail("There are no unsaved changes to discard.");
            }

            order.RejectChanges();
            _log.Info(LogLayer.Domain, "WorkOrder.RejectChanges", $"#{order.Id} restored to the saved snapshot — every setter raised PropertyChanged, nothing persisted");
            return OperationResult<WorkOrder>.Ok(order, $"Changes to work order #{order.Id} discarded.");
        }

        public async Task<IReadOnlyList<WorkOrder>> ImportBatchAsync(int firstNumber, int count)
        {
            var created = new List<WorkOrder>(count);
            for (int i = 0; i < count; i++)
            {
                int n = firstNumber + i;
                var order = new WorkOrder
                {
                    Title = $"{ImportTitles[n % ImportTitles.Length]} (import {n:00})",
                    Status = (WorkOrderStatus)(n % 3),                         // Open / Scheduled / InProgress
                    Priority = (WorkOrderPriority)(n % 3),
                    AssignedTo = ImportPeople[n % ImportPeople.Length],
                    DueDate = DateTime.Today.AddDays((n * 7) % 30 - 3),
                    Cost = 150m + (n * 37) % 1900
                };
                var stored = await _repository.UpsertAsync(order);
                order.Id = stored.Id;
                order.AcceptChanges();
                created.Add(order);
            }

            _log.Info(LogLayer.Service, "WorkOrderService.ImportBatchAsync", $"{count} generated work orders written (#{created[0].Id}–#{created[created.Count - 1].Id}), returned clean");
            return created;
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
