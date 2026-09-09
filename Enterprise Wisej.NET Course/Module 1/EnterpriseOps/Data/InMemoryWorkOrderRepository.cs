using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// The fake store every module uses until Module 4: ~50 seeded work orders across three tenants, held per
    /// session. A tiny delay simulates a real round trip so the "Refreshing…" state is visible.
    /// Traces every query with the tenant filter and the elapsed time.
    /// </summary>
    public sealed class InMemoryWorkOrderRepository : IWorkOrderRepository
    {
        private const int SimulatedLatencyMs = 60;

        private readonly Dictionary<int, WorkOrder> _rows;
        private readonly ActivityTrace _trace;

        public InMemoryWorkOrderRepository(ActivityTrace trace)
        {
            _trace = trace;
            _rows = SeedData.WorkOrders().ToDictionary(o => o.Id);
        }

        public int Count => _rows.Count;

        public async Task<WorkOrder> FindAsync(string tenantId, int id)
        {
            await Task.Delay(SimulatedLatencyMs / 4);

            // Tenant isolation is enforced here, not trusted to the caller.
            return _rows.TryGetValue(id, out WorkOrder order) && order.TenantId == tenantId ? order : null;
        }

        public async Task<IReadOnlyList<WorkOrder>> QueryAsync(string tenantId, Func<WorkOrder, bool> filter)
        {
            var clock = Stopwatch.StartNew();
            await Task.Delay(SimulatedLatencyMs);

            List<WorkOrder> result = _rows.Values
                .Where(o => o.TenantId == tenantId)
                .Where(filter)
                .ToList();

            _trace.Data($"InMemoryWorkOrderRepository.QueryAsync(tenant={tenantId}) → {result.Count} of {_rows.Count} rows in {clock.ElapsedMilliseconds} ms");
            return result;
        }

        public Task SaveAsync(WorkOrder order)
        {
            _rows[order.Id] = order;
            return Task.CompletedTask;
        }
    }
}
