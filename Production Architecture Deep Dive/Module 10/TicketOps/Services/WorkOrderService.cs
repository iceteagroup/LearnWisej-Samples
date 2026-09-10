using System;
using System.Threading.Tasks;
using TicketOps.Data;
using TicketOps.Domain;
using TicketOps.Infrastructure;

namespace TicketOps.Services
{
    /// <summary>
    /// The in-memory implementation of <see cref="IWorkOrderService"/>. The storage is fake; the rule is real.
    /// The service takes the localization service so the sentences it hands back are already in the
    /// operator's language — the domain gives it a resource KEY, it returns TEXT. No Wisej.NET type anywhere.
    /// </summary>
    public sealed class WorkOrderService : IWorkOrderService
    {
        private readonly IWorkOrderRepository _repository;
        private readonly ILocalizationService _localization;
        private readonly ILog _log;

        public WorkOrderService(IWorkOrderRepository repository, ILocalizationService localization, ILog log)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<DashboardSnapshot> GetDashboardAsync()
        {
            _log.Info(LogLayer.Service, "WorkOrderService.GetDashboardAsync", "→ IWorkOrderRepository.GetAllAsync()");
            var orders = await _repository.GetAllAsync();
            var snapshot = new DashboardSnapshot(orders);
            _log.Info(LogLayer.Service, "WorkOrderService.GetDashboardAsync",
                $"{orders.Count} work orders · open {snapshot.CountOf(WorkOrderStatus.Open)} · in progress {snapshot.CountOf(WorkOrderStatus.InProgress)} · blocked {snapshot.CountOf(WorkOrderStatus.Blocked)} · done {snapshot.CountOf(WorkOrderStatus.Done)}");
            return snapshot;
        }

        public Task<WorkOrder> FindAsync(int id)
        {
            _log.Info(LogLayer.Service, "WorkOrderService.FindAsync", $"#{id} → IWorkOrderRepository.FindAsync");
            return _repository.FindAsync(id);
        }

        public async Task<OperationResult<WorkOrder>> AdvanceStatusAsync(int id)
        {
            _log.Info(LogLayer.Service, "WorkOrderService.AdvanceStatusAsync", $"#{id} → IWorkOrderRepository.FindAsync");
            var order = await _repository.FindAsync(id);
            if (order == null)
                return OperationResult<WorkOrder>.Fail(_localization.Text("Rule.WorkOrderMissing"));

            // The rule lives on the domain object and answers with a resource key; the service turns it into words.
            if (!order.CanAdvance(out string reasonKey))
            {
                _log.Warn(LogLayer.Domain, "WorkOrder.CanAdvance", $"#{id} rejected: {reasonKey} (resolved for {_localization.Culture.Name})");
                return OperationResult<WorkOrder>.Fail(_localization.Text(reasonKey));
            }

            var before = order.Status;
            order.Advance();
            _log.Info(LogLayer.Domain, "WorkOrder.Advance", $"#{id} {before} → {order.Status}");
            var saved = await _repository.UpsertAsync(order);

            return OperationResult<WorkOrder>.Ok(saved, _localization.Format("Message.StatusAdvanced", saved.Id, _localization.StatusText(saved.Status)));
        }
    }
}
