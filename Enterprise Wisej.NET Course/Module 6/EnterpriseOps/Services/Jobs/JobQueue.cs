using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EnterpriseOps.Services.Jobs
{
    /// <summary>
    /// The background queue abstraction: starting a job enqueues it; a worker picks it up. The executing
    /// thread is independent of the request that asked for it — and of the session, which may be gone.
    /// </summary>
    public interface IJobQueue
    {
        JobRecord Enqueue(IBackgroundJob job, JobRecord record);
        bool TryCancel(Guid jobId, string requestedBy);
        int PendingCount { get; }
        bool IsWorkerBusy { get; }
    }

    /// <summary>
    /// One worker, one job at a time (so a second import sits at "Queued" while the first runs, as in the
    /// video). The queue owns a CancellationTokenSource per job: "Cancel job" from any session of the tenant
    /// sets that flag; the job honours it between batches. The worker is a plain Task.Run — it has NO Wisej
    /// session, cannot touch controls, and does not need to: it writes to the store and the notification
    /// service; observers do the pushing in their own session context.
    /// </summary>
    public sealed class InMemoryJobQueue : IJobQueue
    {
        private sealed class QueuedJob
        {
            public IBackgroundJob Job;
            public Guid JobId;
            public string TenantId;
            public string StartedBy;
            public CancellationTokenSource Cancellation = new CancellationTokenSource();
        }

        private readonly IJobStatusStore _store;
        private readonly INotificationService _notifications;
        private readonly BlockingCollection<QueuedJob> _pending = new BlockingCollection<QueuedJob>();
        private readonly ConcurrentDictionary<Guid, QueuedJob> _byId = new ConcurrentDictionary<Guid, QueuedJob>();
        private int _workerStarted;
        private volatile bool _busy;

        public InMemoryJobQueue(IJobStatusStore store, INotificationService notifications)
        {
            _store = store;
            _notifications = notifications;
        }

        public int PendingCount => _pending.Count;
        public bool IsWorkerBusy => _busy;

        public JobRecord Enqueue(IBackgroundJob job, JobRecord record)
        {
            record.JobId = job.JobId;
            record.Status = JobStatus.Queued;
            record.Percent = 0;
            record.Message = "Queued.";
            var created = _store.Create(record);

            var queued = new QueuedJob { Job = job, JobId = job.JobId, TenantId = record.TenantId, StartedBy = record.StartedBy };
            _byId[job.JobId] = queued;
            _pending.Add(queued);
            EnsureWorker();
            return created;
        }

        /// <summary>Sets the flag. Nothing stops by itself: the job decides when it is safe to stop.</summary>
        public bool TryCancel(Guid jobId, string requestedBy)
        {
            if (!_byId.TryGetValue(jobId, out var queued) || queued.Cancellation.IsCancellationRequested)
                return false;

            queued.Cancellation.Cancel();
            _store.AppendHistory(jobId, "Queue:", $"Cancel requested by {requestedBy} — flag set; the job will stop after the current batch.");
            return true;
        }

        private void EnsureWorker()
        {
            if (Interlocked.Exchange(ref _workerStarted, 1) == 0)
                Task.Run(WorkerLoopAsync);
        }

        private async Task WorkerLoopAsync()
        {
            foreach (var queued in _pending.GetConsumingEnumerable())
            {
                _busy = true;
                try
                {
                    await RunOneAsync(queued);
                }
                finally
                {
                    _busy = false;
                    _byId.TryRemove(queued.JobId, out _);
                    queued.Cancellation.Dispose();
                }
            }
        }

        private async Task RunOneAsync(QueuedJob queued)
        {
            var record = _store.Get(queued.JobId);
            var sink = new NotifyingSink(_store, _notifications, record);

            if (queued.Cancellation.IsCancellationRequested)
            {
                // Cancelled while still queued: nothing ran, say so.
                await sink.PublishAsync(new JobProgress(queued.JobId, JobStatus.Canceled, 0, "Canceled before it started."), CancellationToken.None);
                return;
            }

            _store.AppendHistory(queued.JobId, "Queue:", $"Worker picked up {record.Number} (thread {Environment.CurrentManagedThreadId}).");
            try
            {
                await queued.Job.RunAsync(sink, queued.Cancellation.Token);
            }
            catch (OperationCanceledException)
            {
                await sink.PublishAsync(new JobProgress(queued.JobId, JobStatus.Canceled, record.Percent, "Canceled."), CancellationToken.None);
            }
            catch (Exception ex)
            {
                // An unexpected exception is terminal for the job — and it is recorded, never swallowed.
                await sink.PublishAsync(new JobProgress(queued.JobId, JobStatus.Failed, record.Percent, $"Failed: {ex.GetType().Name}: {ex.Message}"), CancellationToken.None);
            }
        }

        /// <summary>
        /// Store first, then decide whether the milestone deserves a notification: validated, and every final
        /// state. Batch milestones are progress, not news — they reach the bell only as the final summary.
        /// </summary>
        private sealed class NotifyingSink : IJobProgressSink
        {
            private readonly JobStatusStoreSink _storeSink;
            private readonly IJobStatusStore _store;
            private readonly INotificationService _notifications;
            private readonly JobRecord _record;

            public NotifyingSink(IJobStatusStore store, INotificationService notifications, JobRecord record)
            {
                _store = store;
                _storeSink = new JobStatusStoreSink(store);
                _notifications = notifications;
                _record = record;
            }

            public async Task PublishAsync(JobProgress progress, CancellationToken cancellationToken)
            {
                await _storeSink.PublishAsync(progress, cancellationToken);

                string title = null;
                if (progress.Status == JobStatus.Running && progress.Message.StartsWith("Validated", StringComparison.Ordinal))
                    title = $"{_record.Number} — milestone: validated";
                else if (progress.Status == JobStatus.Completed)
                    title = $"{_record.Number} — completed";
                else if (progress.Status == JobStatus.CompletedWithErrors)
                    title = $"{_record.Number} — completed with errors";
                else if (progress.Status == JobStatus.Failed)
                    title = $"{_record.Number} — failed";
                else if (progress.Status == JobStatus.Canceled)
                    title = $"{_record.Number} — canceled";

                if (title == null)
                    return;

                _notifications.Publish(_record.TenantId, _record.StartedBy, null, _record.JobId, title, progress.Message);
                _store.AppendHistory(_record.JobId, "Notify:", $"Notification stored for {_record.StartedBy}@{_record.TenantId}: \"{title}\".");
            }
        }
    }
}
