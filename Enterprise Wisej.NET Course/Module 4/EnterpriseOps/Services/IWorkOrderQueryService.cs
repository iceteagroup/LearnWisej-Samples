using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Services.Queries;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The read side of the data boundary: read-only projections shaped for screens. Separate from the
    /// command service so that neither has to know about the other's needs.
    /// </summary>
    public interface IWorkOrderQueryService
    {
        Task<PagedResult<WorkQueueRow>> SearchAsync(WorkQueueQuery query, CancellationToken cancellationToken);
        Task<WorkOrderHeader> GetHeaderAsync(string tenantId, int workOrderId, CancellationToken cancellationToken);
        Task<List<AuditLogRow>> GetAuditAsync(string tenantId, int? workOrderId, int take, CancellationToken cancellationToken);
        Task<List<int>> GetApprovalCandidatesAsync(string tenantId, int take, CancellationToken cancellationToken);
    }
}
