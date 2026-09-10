using System;
using System.Collections.Generic;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Services
{
    /// <summary>What the work-queue page asks for. The tenant is never a parameter — it comes from the context.</summary>
    public sealed class WorkQueueQuery
    {
        public string StatusFilter { get; set; } = "Open";     // "Open" | "All" | "Escalated"
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    /// <summary>
    /// The projection the grid binds to. A row, not an entity: it carries what the screen shows plus the
    /// version token the approve command has to echo back. No navigation properties, no domain behaviour.
    /// </summary>
    public sealed class WorkQueueRow
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Customer { get; set; }
        public string Site { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string AssignedTo { get; set; }
        public string Due { get; set; }
        public int Version { get; set; }
        public bool CanBeApproved { get; set; }

        public static WorkQueueRow From(WorkOrder w) => new WorkQueueRow
        {
            Id = w.Id,
            Title = w.Title,
            Customer = w.Customer,
            Site = w.Site,
            Status = w.Status.ToString(),
            Priority = w.Priority.ToString(),
            AssignedTo = w.AssignedTo ?? "—",
            Due = w.DueUtc.HasValue ? w.DueUtc.Value.ToString("yyyy-MM-dd") : "—",
            Version = w.Version,
            CanBeApproved = w.CanBeApproved,
        };
    }

    /// <summary>One page of rows plus the totals the footer shows. Paging is a server decision, not a grid trick.</summary>
    public sealed class PagedResult<T>
    {
        public List<T> Rows { get; set; } = new List<T>();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int ElapsedMs { get; set; }
    }

    /// <summary>
    /// The approve command. It carries the version the user saw, so the store can reject the write when
    /// somebody else moved the row on (the "stale version" failure path).
    /// </summary>
    public sealed class ApproveWorkOrderCommand
    {
        public int WorkOrderId { get; set; }
        public int ExpectedVersion { get; set; }
        public string Note { get; set; }
    }

    /// <summary>The approve result: the command outcome plus the audit line that was written with it.</summary>
    public sealed class ApproveResult : CommandResult
    {
        public int WorkOrderId { get; set; }
        public int NewVersion { get; set; }
        public string AuditLine { get; set; }
        public DateTime CompletedUtc { get; set; }

        public static ApproveResult Approved(string correlationId, int id, int newVersion, string auditLine, DateTime completedUtc)
            => new ApproveResult
            {
                Succeeded = true,
                CorrelationId = correlationId,
                WorkOrderId = id,
                NewVersion = newVersion,
                AuditLine = auditLine,
                CompletedUtc = completedUtc,
            };

        public static ApproveResult Refused(string correlationId, int id, string auditLine, string error)
        {
            var result = new ApproveResult { Succeeded = false, CorrelationId = correlationId, WorkOrderId = id, AuditLine = auditLine };
            result.Errors.Add(error);
            return result;
        }
    }
}
