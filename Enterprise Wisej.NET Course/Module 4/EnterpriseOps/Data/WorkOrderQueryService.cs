using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Domain;
using EnterpriseOps.Services;
using EnterpriseOps.Services.Queries;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// The read side: read-only projections, AsNoTracking, one short-lived DbContext per query, always
    /// filtered by tenant. Nothing returned here can be saved, which is the point.
    /// </summary>
    public sealed class WorkOrderQueryService : IWorkOrderQueryService
    {
        private readonly SessionDatabase _database;
        private readonly IActivityTrace _trace;

        public WorkOrderQueryService(SessionDatabase database, IActivityTrace trace)
        {
            _database = database;
            _trace = trace;
        }

        public async Task<PagedResult<WorkQueueRow>> SearchAsync(WorkQueueQuery query, CancellationToken cancellationToken)
        {
            await _database.Gate.WaitAsync(cancellationToken);
            var context = _database.CreateContext("Search query");
            try
            {
                IQueryable<WorkOrder> rows = context.WorkOrders.AsNoTracking().Where(x => x.TenantId == query.TenantId);

                if (!string.IsNullOrWhiteSpace(query.Search))
                {
                    string term = query.Search.Trim();
                    rows = rows.Where(x => EF.Functions.Like(x.Title, $"%{term}%")
                                        || EF.Functions.Like(x.Number, $"%{term}%")
                                        || EF.Functions.Like(x.Customer, $"%{term}%")
                                        || EF.Functions.Like(x.Site, $"%{term}%"));
                }

                if (query.Status.HasValue)
                    rows = rows.Where(x => x.Status == query.Status.Value);

                int total = await rows.CountAsync(cancellationToken);
                var page = await rows
                    .OrderBy(x => x.Id)
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .Select(x => new WorkQueueRow          // projection: the entity never leaves this method
                    {
                        Id = x.Id,
                        Number = x.Number,
                        Title = x.Title,
                        Customer = x.Customer,
                        Site = x.Site,
                        Status = x.Status.ToString(),
                        Priority = x.Priority.ToString(),
                        AssignedTo = x.AssignedTo,
                        DueUtc = x.DueUtc,
                        Version = x.Version,
                    })
                    .ToListAsync(cancellationToken);

                _trace.Trace(TraceLayer.Data, $"SELECT … WHERE TenantId='{query.TenantId}'{(string.IsNullOrWhiteSpace(query.Search) ? "" : $" AND (Title|Number|Customer|Site LIKE '%{query.Search.Trim()}%')")} → {page.Count} of {total} rows (AsNoTracking, projected to WorkQueueRow)");

                return new PagedResult<WorkQueueRow> { Rows = page, Total = total, Page = query.Page, PageSize = query.PageSize };
            }
            finally
            {
                _database.Release(context);
                _database.Gate.Release();
            }
        }

        public async Task<WorkOrderHeader> GetHeaderAsync(string tenantId, int workOrderId, CancellationToken cancellationToken)
        {
            await _database.Gate.WaitAsync(cancellationToken);
            var context = _database.CreateContext("Header query");
            try
            {
                var header = await context.WorkOrders.AsNoTracking()
                    .Where(x => x.TenantId == tenantId && x.Id == workOrderId)
                    .Select(x => new WorkOrderHeader
                    {
                        Id = x.Id,
                        TenantId = x.TenantId,
                        Number = x.Number,
                        Title = x.Title,
                        Customer = x.Customer,
                        Site = x.Site,
                        Status = x.Status,
                        Priority = x.Priority,
                        Version = x.Version,
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                _trace.Trace(TraceLayer.Data, header == null
                    ? $"SELECT header WHERE TenantId='{tenantId}' AND Id={workOrderId} → no row"
                    : $"SELECT header WHERE TenantId='{tenantId}' AND Id={workOrderId} → {header.Number} {header.Status} v{header.Version} (AsNoTracking)");
                return header;
            }
            finally
            {
                _database.Release(context);
                _database.Gate.Release();
            }
        }

        public async Task<List<AuditLogRow>> GetAuditAsync(string tenantId, int? workOrderId, int take, CancellationToken cancellationToken)
        {
            await _database.Gate.WaitAsync(cancellationToken);
            var context = _database.CreateContext("Audit query");
            try
            {
                IQueryable<AuditEntry> rows = context.AuditEntries.AsNoTracking().Where(x => x.TenantId == tenantId);
                if (workOrderId.HasValue)
                    rows = rows.Where(x => x.WorkOrderId == workOrderId.Value);

                var list = await rows
                    .OrderByDescending(x => x.Id)
                    .Take(take)
                    .Select(x => new AuditLogRow
                    {
                        TimestampUtc = x.TimestampUtc,
                        Action = x.Action,
                        Outcome = x.Outcome,
                        ErrorCode = x.ErrorCode,
                        WorkOrderId = x.WorkOrderId,
                        UserId = x.UserId,
                        CorrelationId = x.CorrelationId,
                        Detail = x.Detail,
                    })
                    .ToListAsync(cancellationToken);

                _trace.Trace(TraceLayer.Data, $"SELECT AuditEntries WHERE TenantId='{tenantId}'{(workOrderId.HasValue ? $" AND WorkOrderId={workOrderId}" : "")} ORDER BY Id DESC → {list.Count} rows");
                return list;
            }
            finally
            {
                _database.Release(context);
                _database.Gate.Release();
            }
        }

        public async Task<List<int>> GetApprovalCandidatesAsync(string tenantId, int take, CancellationToken cancellationToken)
        {
            await _database.Gate.WaitAsync(cancellationToken);
            var context = _database.CreateContext("Batch candidates query");
            try
            {
                // On purpose: InProgress rows (will commit) and OnHold rows (will be rejected) — one transaction each.
                var ids = await context.WorkOrders.AsNoTracking()
                    .Where(x => x.TenantId == tenantId && (x.Status == WorkOrderStatus.InProgress || x.Status == WorkOrderStatus.OnHold))
                    .OrderBy(x => x.Id)
                    .Take(take)
                    .Select(x => x.Id)
                    .ToListAsync(cancellationToken);

                _trace.Trace(TraceLayer.Data, $"SELECT Id WHERE TenantId='{tenantId}' AND Status IN (InProgress, OnHold) → {ids.Count} candidates");
                return ids;
            }
            finally
            {
                _database.Release(context);
                _database.Gate.Release();
            }
        }
    }
}
