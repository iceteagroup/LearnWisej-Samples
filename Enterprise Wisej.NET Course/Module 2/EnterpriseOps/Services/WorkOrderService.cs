using System;
using System.Linq;
using EnterpriseOps.Data;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Services
{
    /// <summary>The three tabs of the TicketOps Work Orders screen, as the server understands them.</summary>
    public enum QueueBucket { Open, InProgress, Done }

    public class WorkQueueQuery
    {
        public QueueBucket Bucket { get; set; } = QueueBucket.Open;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    /// <summary>Projection for the grid — the entity never reaches the UI.</summary>
    public class WorkQueueRow
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Priority Priority { get; set; }
        public string PriorityText => Priority.ToString();
        public string State { get; set; }
        public string Due { get; set; }
    }

    public class SaveWorkOrderCommand
    {
        public string Title { get; set; }
        public string Customer { get; set; }
        public string Site { get; set; }
        public Priority Priority { get; set; } = Priority.Normal;
    }

    public class ApproveWorkOrderCommand
    {
        public int WorkOrderId { get; set; }
        public int Version { get; set; }
    }

    /// <summary>
    /// The behaviour the migration must keep unchanged: which rows each tab shows (decided on the server),
    /// what a new work order needs, and how a stale version is refused. Flows 2–5 of the regression harness
    /// call exactly these methods — so does the migrated WorkOrdersPage.
    /// </summary>
    public class WorkOrderService
    {
        private readonly FakeWorkOrderStore _store;
        private readonly ActivityTrace _trace;

        public WorkOrderService(FakeWorkOrderStore store, ActivityTrace trace)
        {
            _store = store;
            _trace = trace;
        }

        public static bool InBucket(WorkOrderStatus status, QueueBucket bucket)
        {
            return bucket switch
            {
                QueueBucket.Open => status == WorkOrderStatus.New || status == WorkOrderStatus.Assigned || status == WorkOrderStatus.OnHold,
                QueueBucket.InProgress => status == WorkOrderStatus.InProgress || status == WorkOrderStatus.Escalated,
                _ => status == WorkOrderStatus.Completed || status == WorkOrderStatus.Cancelled
            };
        }

        public static string StateText(WorkOrderStatus status)
        {
            return status switch
            {
                WorkOrderStatus.InProgress => "in progress",
                WorkOrderStatus.OnHold => "on hold",
                _ => status.ToString().ToLowerInvariant()
            };
        }

        /// <summary>The tab filter runs here, on the server — the same place it ran on Wisej.NET 3.5.</summary>
        public PagedResult<WorkQueueRow> Query(WorkQueueQuery query, CommandContext ctx)
        {
            var matching = _store.All()
                .Where(w => w.TenantId == ctx.TenantId && InBucket(w.Status, query.Bucket))
                .OrderBy(w => w.DueUtc ?? DateTime.MaxValue)
                .ThenBy(w => w.Id)
                .ToList();

            var page = matching
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(w => new WorkQueueRow
                {
                    Id = w.Id,
                    Title = w.Title,
                    Priority = w.Priority,
                    State = StateText(w.Status),
                    Due = w.DueUtc.HasValue ? w.DueUtc.Value.ToString("yyyy-MM-dd") : "—"
                })
                .ToList();

            _trace.Data($"FakeWorkOrderStore → tenant {ctx.TenantId}, bucket {query.Bucket} → {matching.Count} of {_store.Count} rows (filter ran on the server)");

            return new PagedResult<WorkQueueRow> { Rows = page, Total = matching.Count, Page = query.Page, PageSize = query.PageSize };
        }

        /// <summary>How many rows the store holds for a tenant and bucket — the harness uses it to prove the filter was not done client-side.</summary>
        public int CountInStore(string tenantId, QueueBucket bucket)
        {
            return _store.All().Count(w => w.TenantId == tenantId && InBucket(w.Status, bucket));
        }

        public int StoreCount => _store.Count;

        public CommandResult Save(SaveWorkOrderCommand command, CommandContext ctx)
        {
            var errors = new System.Collections.Generic.List<string>();
            if (string.IsNullOrWhiteSpace(command.Title))
                errors.Add("Title is required");
            if (command.Title != null && command.Title.Length > 80)
                errors.Add("Title must be 80 characters or fewer");

            if (errors.Count > 0)
            {
                _trace.Service($"SaveWorkOrderCommand rejected → {string.Join("; ", errors)} (nothing written)");
                return CommandResult.Fail(ctx, errors.ToArray());
            }

            var created = _store.Add(new WorkOrder
            {
                TenantId = ctx.TenantId,
                Title = command.Title.Trim(),
                Customer = string.IsNullOrWhiteSpace(command.Customer) ? "Walk-in" : command.Customer,
                Site = string.IsNullOrWhiteSpace(command.Site) ? "—" : command.Site,
                Priority = command.Priority,
                Status = WorkOrderStatus.New,
                AssignedTo = "",
                DueUtc = DateTime.UtcNow.AddDays(3)
            });

            _trace.Data($"FakeWorkOrderStore.Add → work order {created.Id} \"{created.Title}\" (New, v{created.Version}) for {ctx.TenantId}");
            return CommandResult.Ok(ctx, $"work order {created.Id} created");
        }

        /// <summary>"Approve" accepts a work order into the queue: New → Assigned, and the Version moves on. A stale Version is refused.</summary>
        public CommandResult Approve(ApproveWorkOrderCommand command, CommandContext ctx)
        {
            var workOrder = _store.Find(command.WorkOrderId);
            if (workOrder == null || workOrder.TenantId != ctx.TenantId)
                return CommandResult.Fail(ctx, $"work order {command.WorkOrderId} not found for tenant {ctx.TenantId}");

            if (workOrder.Version != command.Version)
            {
                _trace.Service($"ApproveWorkOrderCommand {workOrder.Id} rejected → stale version v{command.Version} (current v{workOrder.Version})");
                return CommandResult.Fail(ctx, $"stale version v{command.Version} — the work order is at v{workOrder.Version}; reload and retry");
            }

            if (workOrder.Status == WorkOrderStatus.New)
                workOrder.Status = WorkOrderStatus.Assigned;
            workOrder.Version++;

            _trace.Data($"FakeWorkOrderStore → work order {workOrder.Id} approved, now {StateText(workOrder.Status)} v{workOrder.Version}");
            return CommandResult.Ok(ctx, $"work order {workOrder.Id} approved (v{workOrder.Version})");
        }

        public int CurrentVersion(int workOrderId) => _store.Find(workOrderId)?.Version ?? -1;
    }
}
