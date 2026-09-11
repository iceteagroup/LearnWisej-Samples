using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TicketOps.Data;
using TicketOps.Domain;

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

        public WorkOrderService(IWorkOrderRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<IReadOnlyList<WorkOrder>> SearchAsync(string query)
        {
            // The query typed in the browser is client input: bound it before it is used anywhere.
            string q = (query ?? "").Trim();
            if (q.Length > MaxQueryLength)
                q = q.Substring(0, MaxQueryLength);

            var all = await _repository.GetAllAsync();
            if (q.Length == 0)
                return all;

            return all.Where(w => Matches(w, q)).ToList();
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
