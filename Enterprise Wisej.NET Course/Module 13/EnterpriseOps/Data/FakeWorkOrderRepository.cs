using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// The server's system of record — an in-memory table standing in for the central database.
    /// Per session on purpose (the page creates it), so two browser tabs do not share a "server".
    ///
    /// Only the server-side <see cref="Services.WorkOrderService"/> talks to it. The device never reaches
    /// this class directly: offline work goes through the local cache and the command queue, and is replayed
    /// through the same service. (The "anti-pattern" button on the page shows what happens when a screen
    /// ignores that rule.)
    /// </summary>
    public class FakeWorkOrderRepository
    {
        private readonly object _gate = new object();
        private readonly Dictionary<int, WorkOrder> _rows;

        public FakeWorkOrderRepository()
        {
            _rows = SeedData.WorkOrders().ToDictionary(w => w.Id);
        }

        public int Count
        {
            get { lock (_gate) return _rows.Count; }
        }

        /// <summary>Returns a copy, never the stored instance — callers cannot bypass the version bump.</summary>
        public WorkOrder Find(string tenantId, int id)
        {
            lock (_gate)
            {
                return _rows.TryGetValue(id, out var row) && row.TenantId == tenantId ? row.Clone() : null;
            }
        }

        /// <summary>The rows a technician is allowed to cache: their own open assignments for one tenant.</summary>
        public List<WorkOrder> FindAssigned(string tenantId, string user)
        {
            lock (_gate)
            {
                return _rows.Values
                    .Where(w => w.TenantId == tenantId && w.AssignedTo == user
                                && w.Status != WorkOrderStatus.Completed)
                    .OrderBy(w => w.DueUtc ?? DateTime.MaxValue).ThenBy(w => w.Id)
                    .Select(w => w.Clone())
                    .ToList();
            }
        }

        /// <summary>
        /// Applies a mutation under optimistic concurrency: the caller states the version it read, the store
        /// refuses if the row moved on. Returns the new version, or -1 on a version mismatch.
        /// </summary>
        public int Update(string tenantId, int id, int expectedVersion, Action<WorkOrder> change)
        {
            lock (_gate)
            {
                if (!_rows.TryGetValue(id, out var row) || row.TenantId != tenantId)
                    throw new KeyNotFoundException($"WO-{id} does not exist for tenant '{tenantId}'.");

                if (row.Version != expectedVersion)
                    return -1;

                change(row);
                row.Version++;
                row.LastChangedUtc = DateTime.UtcNow;
                return row.Version;
            }
        }
    }
}
