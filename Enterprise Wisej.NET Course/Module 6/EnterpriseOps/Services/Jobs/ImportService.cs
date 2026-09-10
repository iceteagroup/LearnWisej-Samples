using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Data;

namespace EnterpriseOps.Services.Jobs
{
    /// <summary>What the grid binds to. A projection — the page never sees a JobRecord's live object.</summary>
    public sealed class JobQueueRow
    {
        public Guid JobId { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Progress { get; set; }
        public string Rows { get; set; }
        public string StartedBy { get; set; }
        public string Started { get; set; }
    }

    /// <summary>
    /// The application service the Import Center calls. Every decision the lab cares about lives here and can
    /// be reviewed without opening the designer:
    ///   * who may start or cancel a job (Security:)
    ///   * which tenant's jobs a session may see (the store is process-wide; the boundary is enforced here)
    ///   * what a job record says before the worker ever touches it
    ///
    /// It is created per session (it holds the session's ActivityTrace) but owns nothing durable: the queue,
    /// the store, the notifications and the work-order table come from <see cref="JobInfrastructure"/> and
    /// outlive every page and every session. That is the answer to "who owns the job after the session
    /// closes?" — these four, never the screen.
    /// </summary>
    public sealed class ImportService
    {
        private readonly IJobQueue _queue;
        private readonly IJobStatusStore _store;
        private readonly INotificationService _notifications;
        private readonly IImportFileSource _files;
        private readonly FakeWorkOrderRepository _workOrders;
        private readonly RetryPolicy _retry;
        private readonly ActivityTrace _trace;

        public ImportService(ActivityTrace trace)
            : this(JobInfrastructure.Queue, JobInfrastructure.Store, JobInfrastructure.Notifications,
                   JobInfrastructure.Files, JobInfrastructure.WorkOrders, JobInfrastructure.Retry, trace)
        {
        }

        public ImportService(IJobQueue queue, IJobStatusStore store, INotificationService notifications,
            IImportFileSource files, FakeWorkOrderRepository workOrders, RetryPolicy retry, ActivityTrace trace)
        {
            _queue = queue;
            _store = store;
            _notifications = notifications;
            _files = files;
            _workOrders = workOrders;
            _retry = retry;
            _trace = trace;
        }

        public RetryPolicy RetryPolicy => _retry;

        public int WorkOrderCount(string tenantId) => _workOrders.CountFor(tenantId);

        public IReadOnlyList<ImportFileInfo> ListFiles() => _files.ListFiles();

        #region Commands

        /// <summary>
        /// Starts an import: check the permission, write the job record, enqueue it, return the record.
        /// It does NOT run anything — the method returns in microseconds and the worker picks the job up on
        /// its own thread. The screen that called this can be closed one millisecond later.
        /// </summary>
        public CommandResult<JobRecord> StartImport(StartImportCommand command, CommandContext ctx)
        {
            if (!CanStartImports(ctx.Role))
            {
                _trace.Add($"Security: {ctx.User} ({ctx.Role}) may not start imports — Manager or Admin only. [{ctx.CorrelationId}]");
                return CommandResult<JobRecord>.Fail(ctx.CorrelationId, "Starting an import requires the Manager or Admin role.");
            }

            var file = _files.ListFiles().FirstOrDefault(f => f.FileName == command.FileName);
            if (file == null)
            {
                _trace.Add($"Service: rejected — '{command.FileName}' is not in the drop folder. [{ctx.CorrelationId}]");
                return CommandResult<JobRecord>.Fail(ctx.CorrelationId, $"'{command.FileName}' is not in the import drop folder.");
            }

            var definition = new ImportJobDefinition
            {
                FileName = command.FileName,
                PublishEveryRow = command.PublishEveryRow,
            };

            var job = new ImportWorkOrdersJob(definition, ctx.TenantId, _files, _workOrders, _retry);
            var record = new JobRecord
            {
                JobId = job.JobId,
                Type = "ImportWorkOrders",
                Description = $"Work order import — {command.FileName}",
                Input = command.FileName,
                TenantId = ctx.TenantId,
                StartedBy = ctx.User,
                CorrelationId = ctx.CorrelationId,
                CreatedUtc = DateTime.UtcNow,
            };

            var queued = _queue.Enqueue(job, record);

            _trace.Add($"Service: enqueued {queued.Number} \"{queued.Description}\" for {ctx.User}@{ctx.TenantId} [{ctx.CorrelationId}]");
            _trace.Add($"Queue: {_queue.PendingCount} pending · worker {(_queue.IsWorkerBusy ? "busy" : "idle")} · the request thread is free again.");
            if (command.PublishEveryRow)
                _trace.Add("Service: ANTI-PATTERN requested — the job will publish a progress event per row.");

            return CommandResult<JobRecord>.Ok(ctx.CorrelationId, queued);
        }

        /// <summary>
        /// Requests cancellation. This sets a flag and returns: nothing is stopped here. The job checks the
        /// flag between batches, finishes the batch it is in, records what it did and moves to Canceled.
        /// </summary>
        public CommandResult CancelJob(CancelJobCommand command, CommandContext ctx)
        {
            var record = GetJob(command.JobId, ctx);
            if (record == null)
                return CommandResult.Fail(ctx.CorrelationId, "That job does not exist in this tenant.");

            if (!CanCancel(ctx, record))
            {
                _trace.Add($"Security: {ctx.User} ({ctx.Role}) may not cancel {record.Number} started by {record.StartedBy}. [{ctx.CorrelationId}]");
                return CommandResult.Fail(ctx.CorrelationId, "You can only cancel your own jobs (Managers and Admins can cancel any).");
            }

            if (record.IsFinished)
            {
                _trace.Add($"Service: {record.Number} is already {record.Status} — nothing to cancel. [{ctx.CorrelationId}]");
                return CommandResult.Fail(ctx.CorrelationId, $"{record.Number} already finished ({record.Status}).");
            }

            if (!_queue.TryCancel(record.JobId, ctx.User))
                return CommandResult.Fail(ctx.CorrelationId, $"{record.Number} was already cancelling.");

            _trace.Add($"Service: cancel flag set on {record.Number} — the job stops after the current batch, never mid-row. [{ctx.CorrelationId}]");
            return CommandResult.Ok(ctx.CorrelationId);
        }

        #endregion

        #region Queries (tenant boundary lives here)

        /// <summary>
        /// The store is process-wide and holds every tenant's jobs; this is where the boundary is enforced.
        /// The trace reports how many rows were filtered out so the reviewer can see it happening.
        /// </summary>
        public IReadOnlyList<JobRecord> ListJobs(CommandContext ctx, bool trace = false)
        {
            var mine = _store.List(ctx.TenantId);
            if (trace)
            {
                int hidden = _store.ListAll().Count - mine.Count;
                _trace.Add($"Data: {mine.Count} job(s) for tenant {ctx.TenantId}; {hidden} belonging to other tenants were not returned.");
            }
            return mine;
        }

        public IReadOnlyList<JobQueueRow> ListJobRows(CommandContext ctx) =>
            ListJobs(ctx).Select(ToRow).ToList();

        public static JobQueueRow ToRow(JobRecord record) => new JobQueueRow
        {
            JobId = record.JobId,
            Number = record.Number,
            Description = record.Description,
            Status = Describe(record.Status),
            Progress = record.Percent + "%",
            Rows = record.Result == null ? "" : $"{record.Result.Imported:n0}/{record.Result.TotalRows:n0}",
            StartedBy = record.StartedBy,
            Started = record.StartedUtc?.ToLocalTime().ToString("HH:mm:ss") ?? record.CreatedUtc.ToLocalTime().ToString("HH:mm:ss"),
        };

        /// <summary>A single job, tenant-checked. A job id from another tenant returns null, not a record.</summary>
        public JobRecord GetJob(Guid jobId, CommandContext ctx)
        {
            var record = _store.Get(jobId);
            if (record == null)
                return null;
            if (record.TenantId != ctx.TenantId)
            {
                _trace.Add($"Security: {ctx.User} asked for a job of tenant {record.TenantId} — refused. [{ctx.CorrelationId}]");
                return null;
            }
            return record;
        }

        /// <summary>The job this session should be watching: the newest one that is still queued or running.</summary>
        public JobRecord ActiveJob(CommandContext ctx) =>
            ListJobs(ctx).FirstOrDefault(j => j.IsActive);

        public static string Describe(JobStatus status) => status switch
        {
            JobStatus.CompletedWithErrors => "Completed w/ errors",
            _ => status.ToString(),
        };

        #endregion

        #region Notifications

        public IReadOnlyList<Notification> ListNotifications(CommandContext ctx) =>
            _notifications.For(ctx.TenantId, ctx.User, ctx.Role);

        public int UnreadCount(CommandContext ctx) =>
            _notifications.UnreadCount(ctx.TenantId, ctx.User, ctx.Role);

        public int MarkNotificationsRead(CommandContext ctx)
        {
            int count = _notifications.MarkAllRead(ctx.TenantId, ctx.User, ctx.Role);
            _trace.Add($"Service: {count} notification(s) marked read for {ctx.User}@{ctx.TenantId}.");
            return count;
        }

        #endregion

        private static bool CanStartImports(string role) =>
            role == "Manager" || role == "Admin";

        private static bool CanCancel(CommandContext ctx, JobRecord record) =>
            CanStartImports(ctx.Role) || record.StartedBy == ctx.User;
    }
}
