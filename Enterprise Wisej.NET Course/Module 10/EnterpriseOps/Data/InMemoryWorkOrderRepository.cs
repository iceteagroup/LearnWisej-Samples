using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Data
{
    public interface IWorkOrderRepository
    {
        /// <summary>Every row, whatever the tenant. The **caller** is responsible for the tenant filter — and the
        /// tenant guard in the service is what proves it happened. A repository that filtered silently would hide
        /// the mistake instead of failing it.</summary>
        IReadOnlyList<WorkOrder> All();

        IReadOnlyList<WorkOrder> ForTenant(string tenantId);

        WorkOrder Find(int id);

        void Save(WorkOrder order);
    }

    /// <summary>
    /// The in-memory stand-in for the work order table. One instance per session, created by the
    /// <see cref="Services.ServiceRegistry"/>; nothing here is static, so two sessions never share rows.
    /// </summary>
    public sealed class InMemoryWorkOrderRepository : IWorkOrderRepository
    {
        private readonly ActivityTrace _trace;
        private readonly List<WorkOrder> _rows;

        public InMemoryWorkOrderRepository(ActivityTrace trace)
        {
            _trace = trace;
            _rows = SeedData.WorkOrders();
        }

        public IReadOnlyList<WorkOrder> All() => _rows;

        public IReadOnlyList<WorkOrder> ForTenant(string tenantId)
        {
            var rows = _rows.Where(r => StringComparer.Ordinal.Equals(r.TenantId, tenantId)).ToList();
            _trace?.Data($"work orders for tenant '{tenantId}' → {rows.Count} rows (of {_rows.Count} in the store)");
            return rows;
        }

        public WorkOrder Find(int id) => _rows.FirstOrDefault(r => r.Id == id);

        public void Save(WorkOrder order)
        {
            order.Version++;
            _trace?.Data($"saved WO-{order.Id:0000} (version {order.Version})");
        }
    }
}
