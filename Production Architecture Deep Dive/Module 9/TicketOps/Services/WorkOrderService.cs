using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TicketOps.Data;
using TicketOps.Domain;
using TicketOps.Infrastructure;

namespace TicketOps.Services
{
    /// <summary>
    /// Search over the work orders. No control, no Text property, no Wisej.NET type: the same method
    /// serves the screen, a unit test and — later — a WebMethod that a widget calls directly.
    /// </summary>
    public sealed class WorkOrderService : IWorkOrderService
    {
        private const int MaxQueryLength = 80;

        private readonly IWorkOrderRepository _repository;
        private readonly ILog _log;

        public WorkOrderService(IWorkOrderRepository repository, ILog log)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<IReadOnlyList<WorkOrder>> SearchAsync(string query)
        {
            // The query typed in the browser is client input: bound it before it is used anywhere.
            string q = (query ?? "").Trim();
            if (q.Length > MaxQueryLength)
                q = q.Substring(0, MaxQueryLength);

            _log.Info(LogLayer.Service, "WorkOrderService.SearchAsync",
                q.Length == 0 ? "no filter → IWorkOrderRepository.GetAllAsync()" : $"\"{q}\" → IWorkOrderRepository.GetAllAsync() then filter");

            var all = await _repository.GetAllAsync();
            if (q.Length == 0)
                return all;

            var matches = all.Where(w => Matches(w, q)).ToList();
            _log.Info(LogLayer.Service, "WorkOrderService.SearchAsync", $"{matches.Count} of {all.Count} match");
            return matches;
        }

        private static bool Matches(WorkOrder w, string q)
        {
            var culture = CultureInfo.InvariantCulture.CompareInfo;
            return culture.IndexOf(w.Title ?? "", q, CompareOptions.IgnoreCase) >= 0
                || culture.IndexOf(w.AssignedTo ?? "", q, CompareOptions.IgnoreCase) >= 0
                || w.Id.ToString(CultureInfo.InvariantCulture).Contains(q);
        }
    }
}
