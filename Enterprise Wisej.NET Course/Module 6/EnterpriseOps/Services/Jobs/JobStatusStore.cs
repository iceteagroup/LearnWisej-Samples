using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EnterpriseOps.Services.Jobs
{
    public sealed class JobChangedEventArgs : EventArgs
    {
        public JobChangedEventArgs(Guid jobId, bool isMilestone)
        {
            JobId = jobId;
            IsMilestone = isMilestone;
        }

        public Guid JobId { get; }
        public bool IsMilestone { get; }
    }

    /// <summary>
    /// The job status store: current record + history per job, tenant-scoped reads, and a Changed event
    /// for observers. In production this is a table; here it is an in-memory dictionary that belongs to the
    /// process, not to any session — which is exactly why a reopened page finds its job again.
    /// </summary>
    public interface IJobStatusStore
    {
        JobRecord Create(JobRecord record);
        JobRecord Get(Guid jobId);
        IReadOnlyList<JobRecord> ListAll();
        IReadOnlyList<JobRecord> List(string tenantId);
        void Apply(JobProgress progress);
        void AppendHistory(Guid jobId, string layer, string message);

        /// <summary>Raised on the thread that changed the store — usually the queue worker, never a session thread.</summary>
        event EventHandler<JobChangedEventArgs> Changed;
    }

    public sealed class InMemoryJobStatusStore : IJobStatusStore
    {
        private readonly object _gate = new object();
        private readonly Dictionary<Guid, JobRecord> _jobs = new Dictionary<Guid, JobRecord>();
        private int _nextNumber = 3039;   // the video's IMP-3039 … IMP-3041

        public event EventHandler<JobChangedEventArgs> Changed;

        public JobRecord Create(JobRecord record)
        {
            lock (_gate)
            {
                record.Number = $"IMP-{_nextNumber++}";
                record.CreatedUtc = record.CreatedUtc == default ? DateTime.UtcNow : record.CreatedUtc;
                record.History.Add(new JobHistoryEntry { AtUtc = record.CreatedUtc, Layer = "Queue:", Status = record.Status, Percent = record.Percent, Message = record.Message ?? "Queued." });
                _jobs[record.JobId] = record;
            }
            Changed?.Invoke(this, new JobChangedEventArgs(record.JobId, true));
            return record.Clone();
        }

        public JobRecord Get(Guid jobId)
        {
            lock (_gate)
                return _jobs.TryGetValue(jobId, out var record) ? record.Clone() : null;
        }

        public IReadOnlyList<JobRecord> ListAll()
        {
            lock (_gate)
                return _jobs.Values.OrderByDescending(j => j.CreatedUtc).Select(j => j.Clone()).ToList();
        }

        /// <summary>The tenant boundary: the store never returns a job across it.</summary>
        public IReadOnlyList<JobRecord> List(string tenantId)
        {
            lock (_gate)
                return _jobs.Values.Where(j => j.TenantId == tenantId)
                    .OrderByDescending(j => j.CreatedUtc).Select(j => j.Clone()).ToList();
        }

        public void Apply(JobProgress progress)
        {
            lock (_gate)
            {
                if (!_jobs.TryGetValue(progress.JobId, out var record))
                    return;

                if (progress.IsMilestone)
                {
                    record.Status = progress.Status;
                    if (progress.Percent >= 0) record.Percent = progress.Percent;
                    record.Message = progress.Message;
                    if (progress.Result != null) record.Result = progress.Result.Clone();

                    if (progress.Status == JobStatus.Running && record.StartedUtc == null)
                        record.StartedUtc = DateTime.UtcNow;
                    if (record.IsFinished && record.FinishedUtc == null)
                        record.FinishedUtc = DateTime.UtcNow;

                    record.History.Add(new JobHistoryEntry { AtUtc = DateTime.UtcNow, Layer = "Job:", Status = progress.Status, Percent = progress.Percent, Message = progress.Message });
                }
                // Row-level events (IsMilestone = false) are deliberately NOT recorded: a thousand rows
                // do not belong in a status history. They still raise Changed so the flood is visible.
            }
            Changed?.Invoke(this, new JobChangedEventArgs(progress.JobId, progress.IsMilestone));
        }

        public void AppendHistory(Guid jobId, string layer, string message)
        {
            lock (_gate)
            {
                if (!_jobs.TryGetValue(jobId, out var record))
                    return;
                record.History.Add(new JobHistoryEntry { AtUtc = DateTime.UtcNow, Layer = layer, Status = record.Status, Percent = record.Percent, Message = message });
            }
            Changed?.Invoke(this, new JobChangedEventArgs(jobId, true));
        }
    }

    /// <summary>The sink the queue hands to a job: every milestone goes into the store. Nothing else.</summary>
    public sealed class JobStatusStoreSink : IJobProgressSink
    {
        private readonly IJobStatusStore _store;

        public JobStatusStoreSink(IJobStatusStore store)
        {
            _store = store;
        }

        public Task PublishAsync(JobProgress progress, CancellationToken cancellationToken)
        {
            _store.Apply(progress);
            return Task.CompletedTask;
        }
    }
}
