using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Domain;
using EnterpriseOps.Services;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// The fake work-order table: session-scoped, seeded once, 150 rows across the three tenants.
    ///
    /// Latency is simulated as a function of the requested page size so the slow-query failure path is
    /// *measured*, not faked: 140 ms + 0.44 ms per requested row. pageSize 50 → about 160 ms; pageSize 5000
    /// → 2,340 ms — the number the lesson uses. The store also has a one-shot failure switch so the
    /// "operation that throws" path can be exercised without a real database.
    /// </summary>
    public sealed class InMemoryWorkOrderStore
    {
        public const int BaseLatencyMs = 140;
        public const double LatencyPerRowMs = 0.44;

        /// <summary>Rough per-row footprint used by the session-memory audit (entity + strings).</summary>
        public const int ApproxBytesPerRow = 320;

        private readonly List<WorkOrder> _rows;
        private readonly Action<string> _trace;

        public InMemoryWorkOrderStore(Action<string> trace)
        {
            _trace = trace;
            _rows = Seed(150);
        }

        public int Count => _rows.Count;

        public long EstimatedBytes => (long)_rows.Count * ApproxBytesPerRow;

        /// <summary>Failure path switch: the next SearchAsync throws the way a dropped connection would.</summary>
        public bool FailNextCall { get; set; }

        public int SimulatedLatencyMs(int pageSize) => BaseLatencyMs + (int)Math.Round(pageSize * LatencyPerRowMs);

        /// <summary>
        /// One page of one tenant's work orders. The correlation id travels in the CommandContext and is
        /// written into every Data: trace line, so the log for one click can be followed down to here.
        /// </summary>
        public async Task<PagedResult<WorkOrder>> SearchAsync(WorkQueueQuery query, CommandContext ctx, CancellationToken ct)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));

            _trace($"Data: SearchAsync tenant={query.TenantId} page={query.Page} pageSize={query.PageSize:N0} · correlation {ctx.CorrelationId}");

            if (FailNextCall)
            {
                FailNextCall = false;
                _trace($"Data: connection lost (simulated) · correlation {ctx.CorrelationId} → throwing to the service");

                // The message deliberately carries the kind of detail a real driver adds — host, pool, spid.
                // It belongs in the server log with the correlation id, never on the user's screen.
                throw new InvalidOperationException(
                    "Connection reset by peer while reading the result set (host sql-prod-02:1433, pool WorkOrders, spid 71).");
            }

            int latency = SimulatedLatencyMs(query.PageSize);
            await Task.Delay(latency, ct);

            List<WorkOrder> tenantRows = _rows
                .Where(w => w.TenantId == query.TenantId)
                .OrderByDescending(w => w.CreatedUtc)
                .ToList();

            List<WorkOrder> page = tenantRows
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            string scan = query.PageSize > 500 ? $" (pageSize {query.PageSize:N0} forced a full scan)" : "";
            _trace($"Data: {page.Count} of {tenantRows.Count} rows materialized in ≈{latency:N0} ms{scan} · correlation {ctx.CorrelationId}");

            return new PagedResult<WorkOrder>
            {
                Rows = page,
                Total = tenantRows.Count,
                Page = query.Page,
                PageSize = query.PageSize,
            };
        }

        #region Seed data (deterministic, so the reviewer sees the same rows every run)

        private static readonly string[] Titles =
        {
            "Boiler inspection", "HVAC filter swap", "Chiller alarm — high pressure", "Elevator quarterly service",
            "Generator load test", "Sprinkler valve leak", "Lighting retrofit — bay 4", "Access control door fault",
            "Pump seal replacement", "Roof drain blocked", "Fire panel battery", "Compressor vibration check",
        };

        private static readonly string[] Sites =
        {
            "Plant A", "Plant B", "Depot North", "Depot South", "HQ", "Warehouse 7", "Terminal 2", "Substation 9",
        };

        private static readonly string[] Technicians = { "ben.tech", "dana.tech", "eli.tech", "" };

        private static List<WorkOrder> Seed(int count)
        {
            var random = new Random(11);
            var rows = new List<WorkOrder>(count);
            DateTime now = DateTime.UtcNow;

            for (int i = 1; i <= count; i++)
            {
                Tenant tenant = Tenant.All[i % Tenant.All.Length];
                var status = (WorkOrderStatus)random.Next(0, 7);
                rows.Add(new WorkOrder
                {
                    Id = 4000 + i,
                    TenantId = tenant.Id,
                    Title = Titles[random.Next(Titles.Length)],
                    Customer = tenant.Name,
                    Site = Sites[random.Next(Sites.Length)],
                    Status = status,
                    Priority = (Priority)random.Next(0, 4),
                    AssignedTo = status == WorkOrderStatus.New ? "" : Technicians[random.Next(Technicians.Length)],
                    CreatedUtc = now.AddHours(-random.Next(1, 24 * 30)),
                    DueUtc = random.Next(4) == 0 ? (DateTime?)null : now.AddDays(random.Next(-3, 14)),
                    Version = random.Next(1, 6),
                });
            }

            return rows;
        }

        #endregion
    }
}
