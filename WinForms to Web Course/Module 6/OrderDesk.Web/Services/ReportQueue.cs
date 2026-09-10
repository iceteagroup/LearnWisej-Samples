using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using OrderDesk.Domain;
using Wisej.Web;

namespace OrderDesk.Services
{
    public enum ReportJobStatus { Queued, Running, Done, Failed }

    /// <summary>One report job: the invoice PDF of one order, written under App_Data/reports.</summary>
    public sealed class ReportJob
    {
        public int Id { get; internal set; }
        public int OrderId { get; internal set; }
        /// <summary>First 8 characters of the session that queued the job — the job list shows whose it is.</summary>
        public string Session { get; internal set; }
        public ReportJobStatus Status { get; internal set; }
        public string FileName => "Invoice-" + OrderId + ".pdf";
        public string Path { get; internal set; }
        public int Bytes { get; internal set; }
        public string Error { get; internal set; }
        public DateTime Enqueued { get; internal set; }
        public DateTime? Started { get; internal set; }
        public DateTime? Finished { get; internal set; }

        public string StatusText
        {
            get
            {
                switch (Status)
                {
                    case ReportJobStatus.Running: return "▶ Running";
                    case ReportJobStatus.Done: return "✓ Done";
                    case ReportJobStatus.Failed: return "✖ Failed";
                    default: return "● Queued";
                }
            }
        }
    }

    /// <summary>
    /// The queue in front of the report engine. Process-wide (static) on purpose: every session enqueues
    /// into the same ConcurrentQueue and ONE worker — started with Application.StartTask — serializes
    /// generation, so an engine that cannot run many reports at once is protected and the UI shows job
    /// status instead of freezing. Progress reaches every open page through <see cref="Changed"/>; the
    /// page pushes it to its browser with Application.Update(page, …). Jobs carry the session that
    /// queued them; the PDF bytes are written under the storage root (App_Data/reports), never to a
    /// user path.
    /// </summary>
    public static class ReportQueue
    {
        /// <summary>Simulated engine time per report, so the reviewer can watch Queued → Running → Done.</summary>
        public static int EngineDelayMs = 700;

        private static readonly ConcurrentQueue<ReportJob> Pending = new ConcurrentQueue<ReportJob>();
        private static readonly List<ReportJob> All = new List<ReportJob>();
        private static readonly object Gate = new object();
        private static int _running;      // 0/1 — exactly one worker
        private static int _nextId;

        /// <summary>Raised on the worker thread whenever a job changes status (Running, Done, Failed).</summary>
        public static event Action<ReportJob> Changed;

        public static bool IsWorkerRunning => Volatile.Read(ref _running) == 1;
        public static int PendingCount => Pending.Count;

        public static ReportJob Enqueue(int orderId, string session)
        {
            var job = new ReportJob
            {
                Id = Interlocked.Increment(ref _nextId),
                OrderId = orderId,
                Session = session,
                Status = ReportJobStatus.Queued,
                Enqueued = DateTime.Now,
            };
            lock (Gate) All.Add(job);
            Pending.Enqueue(job);
            EnsureWorker();
            return job;
        }

        /// <summary>The most recent jobs (all sessions), oldest first.</summary>
        public static List<ReportJob> Snapshot(int max = 40)
        {
            lock (Gate) return All.Skip(Math.Max(0, All.Count - max)).ToList();
        }

        public static string Summary()
        {
            lock (Gate)
            {
                int q = All.Count(j => j.Status == ReportJobStatus.Queued);
                int r = All.Count(j => j.Status == ReportJobStatus.Running);
                int d = All.Count(j => j.Status == ReportJobStatus.Done);
                int f = All.Count(j => j.Status == ReportJobStatus.Failed);
                return q + " queued · " + r + " running · " + d + " done" + (f > 0 ? " · " + f + " failed" : "") + " · worker " + (IsWorkerRunning ? "busy" : "idle");
            }
        }

        private static void EnsureWorker()
        {
            if (Interlocked.CompareExchange(ref _running, 1, 0) != 0) return;
            // One background task inside the Wisej application context: it may call Application.Update
            // to push UI changes; it never touches a control directly.
            Application.StartTask(Worker);
        }

        private static void Worker()
        {
            try
            {
                while (Pending.TryDequeue(out var job))
                {
                    job.Status = ReportJobStatus.Running;
                    job.Started = DateTime.Now;
                    Raise(job);
                    try
                    {
                        var order = OrderStore.Shared().Find(job.OrderId);
                        if (order == null) throw new InvalidOperationException("Order " + job.OrderId + " not found.");
                        var bytes = PdfWriter.Invoice(order, job.Session);
                        var path = System.IO.Path.Combine(DocumentStorage.Reports, job.FileName);
                        File.WriteAllBytes(path, bytes);
                        Thread.Sleep(EngineDelayMs);                       // the slow engine we are protecting
                        job.Path = path;
                        job.Bytes = bytes.Length;
                        job.Status = ReportJobStatus.Done;
                    }
                    catch (Exception ex)
                    {
                        job.Error = ex.Message;
                        job.Status = ReportJobStatus.Failed;
                    }
                    job.Finished = DateTime.Now;
                    Raise(job);
                }
            }
            finally
            {
                Interlocked.Exchange(ref _running, 0);
                if (!Pending.IsEmpty) EnsureWorker();                    // a job slipped in while we were leaving
            }
        }

        private static void Raise(ReportJob job)
        {
            var handlers = Changed;
            if (handlers == null) return;
            foreach (var h in handlers.GetInvocationList().Cast<Action<ReportJob>>())
            {
                try { h(job); }
                catch { /* a disposed page must not stop the queue */ }
            }
        }
    }
}
