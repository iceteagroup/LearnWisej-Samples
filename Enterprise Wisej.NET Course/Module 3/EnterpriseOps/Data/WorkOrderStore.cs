using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// The in-memory stand-in for the work order table. Unlike the error log or the session context,
    /// this one IS shared by every session on the server — it has to be, or two browser tabs could never
    /// collide on the same record and there would be nothing for optimistic concurrency to catch. That makes
    /// it the interesting entry in the static-state audit:
    ///
    ///  • what it holds — rows for every tenant, no per-user or per-session data;
    ///  • how it is synchronized — one lock around every read and write, so a save cannot interleave with a read;
    ///  • how it avoids leaking — callers get <b>copies</b> (<see cref="WorkOrder.Clone"/>), never the stored
    ///    instance, so a session that mutates what it was handed cannot corrupt the store or another session;
    ///  • how tenants stay apart — reads are tenant-scoped, and the one deliberately unscoped read
    ///    (<see cref="FindById"/>) is only ever reached through the <c>TenantGuard</c>.
    ///
    /// The version column is the concurrency token: <see cref="TrySave"/> compares it and increments it inside
    /// the same lock, which is what an <c>UPDATE … WHERE Version = @expected</c> does in one statement.
    /// </summary>
    public sealed class WorkOrderStore : IWorkOrderStore
    {
        /// <summary>
        /// Application-scoped on purpose — the one static in this sample that is allowed to hold data, because
        /// the data belongs to the application and not to a user.
        /// </summary>
        [SharedState(StateScope.Application,
            holds: "work order rows for every tenant — no per-user, per-session or per-tab data",
            synchronization: "one lock around every read and write; callers receive clones, never stored instances")]
        public static readonly WorkOrderStore Shared = new WorkOrderStore();

        private readonly object _gate = new object();
        private readonly Dictionary<int, WorkOrder> _rows;

        private WorkOrderStore()
        {
            _rows = SeedData.WorkOrders().ToDictionary(o => o.Id);
        }

        public int Count
        {
            get { lock (_gate) return _rows.Count; }
        }

        /// <summary>
        /// Finds a row by id across every tenant. The only caller is <c>WorkOrderService.OpenAsync</c>, which
        /// hands the row's tenant to the <c>TenantGuard</c> before it looks at anything else — so a cross-tenant
        /// read ends in a rejection instead of a silent empty result.
        /// </summary>
        public WorkOrder FindById(int id)
        {
            lock (_gate)
                return _rows.TryGetValue(id, out WorkOrder row) ? row.Clone() : null;
        }

        /// <summary>The tenant-scoped read every screen uses. There is no "all tenants" query.</summary>
        public IReadOnlyList<WorkOrder> QueryByTenant(string tenantId, string search, WorkOrderStatus? status)
        {
            lock (_gate)
            {
                return _rows.Values
                    .Where(o => StringComparer.Ordinal.Equals(o.TenantId, tenantId))
                    .Where(o => status == null || o.Status == status.Value)
                    .Where(o => string.IsNullOrEmpty(search)
                                || o.Title.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                                || o.Id.ToString().StartsWith(search, StringComparison.Ordinal))
                    .OrderBy(o => o.Id)
                    .Select(o => o.Clone())
                    .ToList();
            }
        }

        /// <summary>
        /// The optimistic concurrency check, in one atomic step: compare the version the caller loaded with the
        /// version the row carries now, and only then write. A second session that saved in between moved the
        /// version on, so the comparison fails and nothing is overwritten. The caller gets the current row back
        /// either way — the conflict dialog needs it to show what changed.
        /// </summary>
        public StoreSaveResult TrySave(int id, string title, WorkOrderStatus status, int expectedVersion, string modifiedBy)
        {
            lock (_gate)
            {
                if (!_rows.TryGetValue(id, out WorkOrder row))
                    return new StoreSaveResult { Saved = false, Current = null, ExpectedVersion = expectedVersion };

                if (row.Version != expectedVersion)
                    return new StoreSaveResult { Saved = false, Current = row.Clone(), ExpectedVersion = expectedVersion };

                row.Title = title;
                row.Status = status;
                row.Version = row.Version + 1;
                row.ModifiedBy = modifiedBy;
                row.ModifiedUtc = DateTime.UtcNow;

                return new StoreSaveResult { Saved = true, Current = row.Clone(), ExpectedVersion = expectedVersion };
            }
        }
    }
}
