using System;
using System.Collections.Generic;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Services
{
    public sealed class LeakResult
    {
        public int RowsRetained { get; init; }
        public long HeapBeforeBytes { get; init; }
        public long HeapAfterBytes { get; init; }
        public long GrowthBytes => Math.Max(0, HeapAfterBytes - HeapBeforeBytes);
        public string CorrelationId { get; init; }
    }

    /// <summary>
    /// DELIBERATELY LEAKY. A well-meant "cache the big report so the second open is instant" that keeps a
    /// 50,000-row list in a per-session field for the whole session. One user: 10–15 MB. Two hundred users:
    /// the server. The session-memory audit flags it; Release() is the fix — and it is also called from the
    /// page's Disposed handler, which is the disposal review in code.
    /// </summary>
    public sealed class ReportCacheService
    {
        public const int ApproxBytesPerRow = 240;

        private readonly StructuredLog _log;
        private readonly Action<string> _trace;

        // The mistake: a collection in a field, filled once, cleared never.
        private List<WorkQueueRow> _cached;
        private long _measuredGrowthBytes;

        public ReportCacheService(StructuredLog log, Action<string> trace)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _trace = trace ?? throw new ArgumentNullException(nameof(trace));
        }

        public int RetainedRows => _cached?.Count ?? 0;

        /// <summary>The larger of the model estimate and what the GC actually measured.</summary>
        public long RetainedBytesEstimate => _cached == null ? 0 : Math.Max((long)_cached.Count * ApproxBytesPerRow, _measuredGrowthBytes);

        public string Lifetime => _cached == null ? "released" : "whole session — never cleared";

        public LeakResult Retain(int rows, CommandContext ctx)
        {
            if (rows <= 0) throw new ArgumentOutOfRangeException(nameof(rows));
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));

            _trace($"Service: ReportCacheService.Retain({rows:N0}) · correlation {ctx.CorrelationId} — building the 'all work orders' report and keeping it in a field");

            long before = GC.GetTotalMemory(forceFullCollection: true);

            var list = new List<WorkQueueRow>(rows);
            var random = new Random(rows);
            for (int i = 0; i < rows; i++)
            {
                list.Add(new WorkQueueRow
                {
                    Id = 100000 + i,
                    Title = "Report row " + i.ToString("D6"),
                    Site = "Site " + (i % 97),
                    Status = (WorkOrderStatus)(i % 7),
                    Priority = (Priority)(i % 4),
                    AssignedTo = "tech." + random.Next(1, 40),
                    DueUtc = DateTime.UtcNow.AddDays(i % 30),
                });
            }
            _cached = list;

            long after = GC.GetTotalMemory(forceFullCollection: true);
            _measuredGrowthBytes = Math.Max(0, after - before);

            var result = new LeakResult { RowsRetained = rows, HeapBeforeBytes = before, HeapAfterBytes = after, CorrelationId = ctx.CorrelationId };

            _log.Write(LogLevel.Warning, "RetainReport", ctx.CorrelationId, new
            {
                rows,
                heapBeforeBytes = before,
                heapAfterBytes = after,
                growthBytes = result.GrowthBytes,
                lifetime = Lifetime,
                tenant = ctx.TenantId,
                user = ctx.UserName,
            });

            _trace($"Service: retained {rows:N0} rows · managed heap {before / 1048576.0:N1} MB → {after / 1048576.0:N1} MB (+{result.GrowthBytes / 1048576.0:N1} MB) · correlation {ctx.CorrelationId}");
            return result;
        }

        /// <summary>The fix: drop the reference. Called by the recovery button and by the page's Disposed handler.</summary>
        public void Release()
        {
            if (_cached == null)
                return;

            int rows = _cached.Count;
            _cached = null;
            _measuredGrowthBytes = 0;
            _trace($"Service: ReportCacheService.Release() → {rows:N0} rows dropped; the GC can reclaim them");
        }
    }
}
