using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Services.Commands;
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

        /// <summary>Reading the audit log is a permission: the service decides, the screen renders the answer.</summary>
        Task<AuditQueryResult> GetAuditAsync(CommandContext context, int? workOrderId, int take, CancellationToken cancellationToken);
    }
}
