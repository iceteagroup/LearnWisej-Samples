using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Domain;
using EnterpriseOps.Services;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// EF Core-backed repository over one short-lived DbContext. It owns no transaction: the command
    /// service does, because only the service can see the whole operation.
    /// </summary>
    public sealed class WorkOrderRepository : IWorkOrderRepository
    {
        private readonly EnterpriseOpsDbContext _context;
        private readonly IActivityTrace _trace;

        public WorkOrderRepository(EnterpriseOpsDbContext context, IActivityTrace trace)
        {
            _context = context;
            _trace = trace;
        }

        public async Task<WorkOrder> LoadAsync(string tenantId, int workOrderId, CancellationToken cancellationToken)
        {
            var workOrder = await _context.WorkOrders
                .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == workOrderId, cancellationToken);

            _trace.Trace(TraceLayer.Data,
                workOrder == null
                    ? $"SELECT WorkOrders WHERE TenantId='{tenantId}' AND Id={workOrderId} → no row"
                    : $"SELECT WorkOrders WHERE TenantId='{tenantId}' AND Id={workOrderId} → {workOrder.Number} {workOrder.Status} v{workOrder.Version} (tracked)");
            return workOrder;
        }

        public void Add(WorkOrder workOrder)
        {
            _context.WorkOrders.Add(workOrder);
            _trace.Trace(TraceLayer.Data, $"INSERT WorkOrders {workOrder.Number} '{workOrder.Title}' (pending until SaveChanges)");
        }

        public void AddAudit(AuditEntry entry)
        {
            _context.AuditEntries.Add(entry);
        }

        public void ExpectVersion(WorkOrder workOrder, int expectedVersion)
        {
            // The concurrency check is done by the database, not by an if-statement: EF Core emits
            // UPDATE … WHERE Id = @id AND Version = @expected. Someone else's commit makes that 0 rows.
            _context.Entry(workOrder).Property(x => x.Version).OriginalValue = expectedVersion;
            workOrder.Version = expectedVersion + 1;
        }
    }
}
