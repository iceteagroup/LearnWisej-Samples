using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Domain;
using EnterpriseOps.Services;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// Per-session fake repository for work orders. Seeds 48 rows across the three tenants so the
    /// queue looks real; the escalation workflow updates Status and Version through it.
    /// </summary>
    public class InMemoryWorkOrderStore
    {
        private readonly ActivityTrace _trace;
        private readonly List<WorkOrder> _rows = SeedData.WorkOrders();

        public InMemoryWorkOrderStore(ActivityTrace trace)
        {
            _trace = trace;
        }

        public IReadOnlyList<WorkOrder> ForTenant(string tenantId)
        {
            return _rows.Where(w => w.TenantId == tenantId).OrderBy(w => w.Id).ToList();
        }

        public WorkOrder Find(int id)
        {
            return _rows.FirstOrDefault(w => w.Id == id);
        }

        /// <summary>The persist step: status → Escalated, version bumped. Returns the new version.</summary>
        public int MarkEscalated(int id, int expectedVersion)
        {
            var row = Find(id) ?? throw new InvalidOperationException($"WO-{id} not found");
            if (row.Version != expectedVersion)
                throw new InvalidOperationException($"WO-{id} is stale: version {row.Version} on the server, {expectedVersion} in the command");

            row.Status = WorkOrderStatus.Escalated;
            row.Version++;
            _trace.Write($"Data: WO-{id} status → Escalated, version {expectedVersion} → {row.Version}");
            return row.Version;
        }

        /// <summary>Compensation for a failed persist: put the row back.</summary>
        public void Revert(int id, WorkOrderStatus status, int version)
        {
            var row = Find(id);
            if (row == null) return;
            row.Status = status;
            row.Version = version;
            _trace.Write($"Data: WO-{id} reverted to {status}, version {version}");
        }
    }
}
