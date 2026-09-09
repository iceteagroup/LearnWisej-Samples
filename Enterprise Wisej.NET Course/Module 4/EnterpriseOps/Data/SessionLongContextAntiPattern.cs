using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Services;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// DELIBERATELY WRONG — the lifetime the lesson warns about: a DbContext stored in a field for the
    /// life of the session. It accumulates tracked entities, serves stale data (EF Core does not overwrite
    /// an entity it already tracks when the same row is queried again), and would fail with a concurrency
    /// exception the moment it tried to save. The "Wrong lifetime" button drives it; nothing else does.
    /// </summary>
    public sealed class SessionLongContextAntiPattern : IDisposable
    {
        private readonly SessionDatabase _database;
        private readonly IActivityTrace _trace;
        private EnterpriseOpsDbContext _contextKeptForHours;      // the field that should not exist
        private DateTime _createdUtc;

        public SessionLongContextAntiPattern(SessionDatabase database, IActivityTrace trace)
        {
            _database = database;
            _trace = trace;
        }

        public bool IsAlive => _contextKeptForHours != null;

        /// <summary>First click: create the long-lived context and read a work order through it.</summary>
        public async Task<string> ReadThroughLongLivedContextAsync(string tenantId, int workOrderId, CancellationToken ct)
        {
            await _database.Gate.WaitAsync(ct);
            try
            {
                if (_contextKeptForHours == null)
                {
                    _contextKeptForHours = _database.CreateContext("SESSION-LONG (anti-pattern)");
                    _createdUtc = DateTime.UtcNow;
                    _trace.Trace(TraceLayer.Data, $"DbContext #{_contextKeptForHours.InstanceNumber} stored in a field — it will not be disposed until the session ends");
                }

                // Tracked query: the first read caches the entity; later reads of the same key return the
                // cached instance with its OLD values, even though the SQL ran again.
                var stale = await _contextKeptForHours.WorkOrders.FirstAsync(x => x.TenantId == tenantId && x.Id == workOrderId, ct);
                int tracked = _contextKeptForHours.ChangeTracker.Entries().Count();

                // The truth, from a short-lived context.
                var fresh = _database.CreateContext("fresh read (correct)");
                try
                {
                    var current = await fresh.WorkOrders.AsNoTracking().FirstAsync(x => x.TenantId == tenantId && x.Id == workOrderId, ct);

                    string verdict = stale.Version == current.Version
                        ? "same for now — approve or update it, then click again"
                        : $"STALE: the field-held context still says {stale.Status} v{stale.Version}, the database says {current.Status} v{current.Version}";

                    _trace.Trace(TraceLayer.Data, $"session-long DbContext #{_contextKeptForHours.InstanceNumber}: {stale.Number} {stale.Status} v{stale.Version} · tracked entities {tracked} · alive {(DateTime.UtcNow - _createdUtc).TotalSeconds:0}s");
                    _trace.Trace(TraceLayer.Data, $"fresh DbContext: {current.Number} {current.Status} v{current.Version}");
                    return verdict;
                }
                finally
                {
                    _database.Release(fresh);
                }
            }
            finally
            {
                _database.Gate.Release();
            }
        }

        /// <summary>Recovery: dispose the field-held context; from now on, one context per command.</summary>
        public void Dispose()
        {
            if (_contextKeptForHours == null)
                return;

            _database.Release(_contextKeptForHours, "the anti-pattern context, finally disposed");
            _contextKeptForHours = null;
        }
    }
}
