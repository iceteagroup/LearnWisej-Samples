using System.Collections.Generic;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Data
{
    /// <summary>What the store gives back when a save is attempted: the row as it now stands, and whether it saved.</summary>
    public sealed class StoreSaveResult
    {
        /// <summary>False when the expected version no longer matched — another session saved first.</summary>
        public bool Saved { get; init; }

        /// <summary>The current row (a copy), whether the save landed or was rejected. Never null.</summary>
        public WorkOrder Current { get; init; }

        /// <summary>The version the caller expected to still be current.</summary>
        public int ExpectedVersion { get; init; }
    }

    /// <summary>
    /// The data-access boundary. Module 4 swaps this in-memory store for EF Core behind the same contract:
    /// <see cref="TrySave"/> becomes an <c>UPDATE … WHERE Version = @expected</c> and its zero-rows-affected
    /// answer is the same optimistic concurrency check.
    ///
    /// <see cref="FindById"/> deliberately does NOT filter by tenant: the sample uses it to prove that the
    /// <c>TenantGuard</c> in the service rejects a cross-tenant read even when the row was found. Every other
    /// read is tenant-scoped, so the guard is the second line of defence, not the only one.
    /// </summary>
    public interface IWorkOrderStore
    {
        WorkOrder FindById(int id);

        IReadOnlyList<WorkOrder> QueryByTenant(string tenantId, string search, WorkOrderStatus? status);

        StoreSaveResult TrySave(int id, string title, WorkOrderStatus status, int expectedVersion, string modifiedBy);

        int Count { get; }
    }
}
