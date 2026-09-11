using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrderDesk.Reporting
{
    public enum ReportJobStatus
    {
        Queued,
        Running,
        Done,
        Failed,
        Cancelled
    }

    /// <summary>One report job. The queue owns the instance; callers only ever see copies (Clone).</summary>
    public sealed class ReportJob
    {
        public int Id { get; internal set; }
        public string Name { get; internal set; }
        /// <summary>Who asked for it — a user name or a session, never a static "current user".</summary>
        public string Owner { get; internal set; }
        public ReportJobStatus Status { get; internal set; }
        /// <summary>0–100, written by the worker, read by every polling page.</summary>
        public int Progress { get; internal set; }
        public DateTime QueuedAt { get; internal set; }
        public DateTime? StartedAt { get; internal set; }
        public DateTime? FinishedAt { get; internal set; }
        /// <summary>The file name the user sees ("Invoice-batch-1204.pdf").</summary>
        public string ResultFileName { get; internal set; }
        /// <summary>Where the worker stored the result: under StorageRoot.Reports. Null until Done.</summary>
        public string ResultPath { get; internal set; }
        public string Error { get; internal set; }

        internal Func<Action<int>, CancellationToken, byte[]> Work;
        internal string OutputFolder;
        internal CancellationTokenSource Cancellation;

        public bool IsActive => Status == ReportJobStatus.Queued || Status == ReportJobStatus.Running;

        public string ProgressText => Status switch
        {
            ReportJobStatus.Queued => "waiting",
            ReportJobStatus.Running => Progress + "%",
            ReportJobStatus.Done => "100%",
            ReportJobStatus.Cancelled => Progress + "% · stopped",
            _ => Progress + "% · failed"
        };

        public string ResultText => Status switch
        {
            ReportJobStatus.Done => ResultFileName + " · view ▸",
            ReportJobStatus.Failed => Error ?? "failed",
            ReportJobStatus.Cancelled => "—",
            _ => Elapsed()
        };

        private string Elapsed()
        {
            if (StartedAt == null) return "—";
            var seconds = (int)(DateTime.Now - StartedAt.Value).TotalSeconds;
            return $"{seconds}s elapsed";
        }

        internal ReportJob Clone() => new ReportJob
        {
            Id = Id, Name = Name, Owner = Owner, Status = Status, Progress = Progress, QueuedAt = QueuedAt,
            StartedAt = StartedAt, FinishedAt = FinishedAt, ResultFileName = ResultFileName, ResultPath = ResultPath, Error = Error
        };
    }

    /// <summary>Queue totals for the status line.</summary>
    public readonly struct ReportQueueCounts
    {
        public ReportQueueCounts(int queued, int running, int done, int failed, int cancelled)
        {
            Queued = queued; Running = running; Done = done; Failed = failed; Cancelled = cancelled;
        }
        public int Queued { get; }
        public int Running { get; }
        public int Done { get; }
        public int Failed { get; }
        public int Cancelled { get; }
        public int Total => Queued + Running + Done + Failed + Cancelled;

        public override string ToString() =>
            $"{Total} job(s) · {Queued} queued · {Running} running · {Done} done" +
            (Failed > 0 ? $" · {Failed} failed" : "") + (Cancelled > 0 ? $" · {Cancelled} cancelled" : "");
    }

    /// <summary>
    /// A process-wide report queue: one list of jobs for every session (like a database table),
    /// one worker that runs them sequentially, progress and cancellation. Pages poll it with a
    /// Wisej.Web.Timer and every browser sees the same queue — that is the multi-user proof.
    ///
    /// The worker runs on the thread pool (Task.Run), outside any Wisej session, so it must never
    /// touch a control: it only computes bytes, writes them under the storage root and updates the
    /// job record under the lock. Application.StartTask is the right tool for work owned by ONE
    /// session; this queue is owned by the process.
    ///
    /// What a production queue adds on top: a persistent store (jobs survive a restart), retries,
    /// per-user limits, several workers or a separate worker service, result expiry. See docs/ReportQueue.md.
    /// </summary>
    public static class ReportQueue
    {
        private static readonly object Gate = new object();
        private static readonly List<ReportJob> Jobs = new List<ReportJob>();
        private static int _nextId = 1;
        private static Task _worker;

        /// <summary>
        /// Adds a job and starts the worker if it is idle. <paramref name="outputFolder"/> is resolved
        /// by the caller (inside a session) so the worker never needs Application.StartupPath.
        /// </summary>
        public static ReportJob Enqueue(string name, string owner, string resultFileName, string outputFolder, Func<Action<int>, CancellationToken, byte[]> work)
        {
            if (work == null) throw new ArgumentNullException(nameof(work));
            lock (Gate)
            {
                var job = new ReportJob
                {
                    Id = _nextId++, Name = name, Owner = owner ?? "?", Status = ReportJobStatus.Queued, Progress = 0,
                    QueuedAt = DateTime.Now, ResultFileName = resultFileName, Work = work, OutputFolder = outputFolder,
                    Cancellation = new CancellationTokenSource()
                };
                Jobs.Add(job);
                if (_worker == null)
                    _worker = Task.Run(WorkerLoop);
                return job.Clone();
            }
        }

        /// <summary>Every job, newest first — copies, safe to bind to a grid.</summary>
        public static IList<ReportJob> Snapshot()
        {
            lock (Gate) return Jobs.OrderByDescending(j => j.Id).Select(j => j.Clone()).ToList();
        }

        public static ReportJob Find(int id)
        {
            lock (Gate) return Jobs.FirstOrDefault(j => j.Id == id)?.Clone();
        }

        /// <summary>
        /// A queued job is cancelled immediately; a running one is asked to stop and turns
        /// Cancelled when the worker observes the token. Returns false when there is nothing to cancel.
        /// </summary>
        public static bool Cancel(int id)
        {
            lock (Gate)
            {
                var job = Jobs.FirstOrDefault(j => j.Id == id);
                if (job == null || !job.IsActive) return false;
                job.Cancellation.Cancel();
                if (job.Status == ReportJobStatus.Queued)
                {
                    // Never reaches Run, so nobody else disposes the token source.
                    job.Status = ReportJobStatus.Cancelled;
                    job.FinishedAt = DateTime.Now;
                    job.Cancellation.Dispose();
                }
                return true;
            }
        }

        public static ReportQueueCounts Counts()
        {
            lock (Gate)
            {
                return new ReportQueueCounts(
                    Jobs.Count(j => j.Status == ReportJobStatus.Queued),
                    Jobs.Count(j => j.Status == ReportJobStatus.Running),
                    Jobs.Count(j => j.Status == ReportJobStatus.Done),
                    Jobs.Count(j => j.Status == ReportJobStatus.Failed),
                    Jobs.Count(j => j.Status == ReportJobStatus.Cancelled));
            }
        }

        /// <summary>Changes whenever any job's status or progress changes — lets pollers skip a redraw.</summary>
        public static string Signature()
        {
            lock (Gate)
            {
                var sb = new StringBuilder();
                foreach (var j in Jobs)
                    sb.Append(j.Id).Append(':').Append((int)j.Status).Append(':').Append(j.Progress).Append(';');
                return sb.ToString();
            }
        }

        // ── the worker ───────────────────────────────────────────────────────────────────────────

        private static void WorkerLoop()
        {
            while (true)
            {
                ReportJob job;
                lock (Gate)
                {
                    job = Jobs.FirstOrDefault(j => j.Status == ReportJobStatus.Queued);
                    if (job == null)
                    {
                        _worker = null;         // idle: the next Enqueue starts a fresh worker
                        return;
                    }
                    job.Status = ReportJobStatus.Running;
                    job.StartedAt = DateTime.Now;
                }
                Run(job);
            }
        }

        private static void Run(ReportJob job)
        {
            var token = job.Cancellation.Token;
            try
            {
                byte[] result = job.Work(percent => SetProgress(job, percent), token);
                token.ThrowIfCancellationRequested();

                string path = Path.Combine(job.OutputFolder, $"{job.Id:D4}-{job.ResultFileName}");   // server storage, Path.Combine
                Directory.CreateDirectory(job.OutputFolder);
                File.WriteAllBytes(path, result);

                lock (Gate)
                {
                    job.ResultPath = path;
                    job.Progress = 100;
                    job.Status = ReportJobStatus.Done;
                    job.FinishedAt = DateTime.Now;
                }
            }
            catch (OperationCanceledException)
            {
                lock (Gate)
                {
                    job.Status = ReportJobStatus.Cancelled;
                    job.FinishedAt = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                lock (Gate)
                {
                    job.Status = ReportJobStatus.Failed;
                    job.Error = ex.GetType().Name + ": " + ex.Message;
                    job.FinishedAt = DateTime.Now;
                }
            }
            finally
            {
                job.Cancellation.Dispose();
            }
        }

        private static void SetProgress(ReportJob job, int percent)
        {
            lock (Gate)
            {
                if (job.Status == ReportJobStatus.Running)
                    job.Progress = Math.Max(0, Math.Min(100, percent));
            }
        }
    }
}
