using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// The data-access boundary. Services depend on this interface; Module 4 swaps the in-memory store for
    /// EF Core behind the same contract. Every method takes the tenant id — there is no "all tenants" query.
    /// </summary>
    public interface IWorkOrderRepository
    {
        Task<WorkOrder> FindAsync(string tenantId, int id);
        Task<IReadOnlyList<WorkOrder>> QueryAsync(string tenantId, Func<WorkOrder, bool> filter);
        Task SaveAsync(WorkOrder order);
        int Count { get; }
    }
}
