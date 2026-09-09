using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EnterpriseOps.Data;

namespace EnterpriseOps.Services.WorkQueues
{
    public sealed class AntiPatternMeasurement
    {
        public IReadOnlyList<WorkQueueRow> Rows { get; set; }
        public int EntitiesMaterialized { get; set; }
        public long MaterializeMs { get; set; }
        public long ProjectMs { get; set; }
        public int PayloadBytes { get; set; }
    }

    /// <summary>
    /// The pattern the video's "production problem" scene shows — <c>grid.DataSource = db.WorkOrders.ToList();</c> —
    /// kept in the sample on purpose so its cost can be measured in the trace next to one page. Every tenant row
    /// is materialized, projected and sized as JSON: that is what would cross the wire and sit in the session.
    /// </summary>
    public sealed class LoadEverythingAntiPattern
    {
        private readonly WorkOrderStore _store;
        private readonly WorkQueueQueryService _projector;
        private readonly ActivityTrace _trace;

        public LoadEverythingAntiPattern(WorkOrderStore store, WorkQueueQueryService projector, ActivityTrace trace)
        {
            _store = store;
            _projector = projector;
            _trace = trace;
        }

        public async Task<AntiPatternMeasurement> LoadEverythingAsync(string tenantId)
        {
            _trace.Write("Service: LoadEverything — grid.DataSource = db.WorkOrders.ToList()  ← the anti-pattern, measured");

            var sw = Stopwatch.StartNew();
            var entities = _store.Snapshot().Where(o => o.TenantId == tenantId).OrderBy(o => o.Id).ToList();
            long materializeMs = sw.ElapsedMilliseconds;

            sw.Restart();
            var rows = entities.Select(_projector.Project).ToList();
            long projectMs = sw.ElapsedMilliseconds;

            int bytes = JsonSerializer.SerializeToUtf8Bytes(rows).Length;

            _trace.Write($"Data: {entities.Count:N0} entities materialized in {materializeMs} ms, projected in {projectMs} ms; " +
                         $"payload ≈ {bytes / 1024.0:N0} KB → browser, memory, session");

            await Task.Delay(15).ConfigureAwait(false);

            return new AntiPatternMeasurement
            {
                Rows = rows,
                EntitiesMaterialized = entities.Count,
                MaterializeMs = materializeMs,
                ProjectMs = projectMs,
                PayloadBytes = bytes,
            };
        }
    }
}
