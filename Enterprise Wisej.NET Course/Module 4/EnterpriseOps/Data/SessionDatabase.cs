using System;
using System.Linq;
using System.Threading;
using EnterpriseOps.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// The per-session database: one open in-memory SQLite connection ("DataSource=:memory:") that lives as
    /// long as the session, and a factory for the short-lived DbContexts that use it. An in-memory SQLite
    /// database exists only while its connection is open, which is exactly why the connection is
    /// session-long while every DbContext is per-operation. Disposed with the page.
    ///
    /// Also serialises database work per session (<see cref="Gate"/>): a SqliteConnection is not safe for
    /// concurrent use, and a Timer tick or an awaited handler could otherwise overlap a click.
    /// </summary>
    public sealed class SessionDatabase : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<EnterpriseOpsDbContext> _options;
        private readonly IActivityTrace _trace;
        private int _instanceCounter;
        private int _liveContexts;

        public SemaphoreSlim Gate { get; } = new SemaphoreSlim(1, 1);

        /// <summary>Contexts created so far in this session.</summary>
        public int ContextsCreated => _instanceCounter;

        /// <summary>Contexts created and not yet disposed — should read 0 between operations.</summary>
        public int LiveContexts => _liveContexts;

        public SessionDatabase(IActivityTrace trace)
        {
            _trace = trace;
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<EnterpriseOpsDbContext>()
                .UseSqlite(_connection)
                .Options;

            using (var context = CreateContext("schema + seed"))
            {
                context.Database.EnsureCreated();
                int rows = SeedData.Seed(context);
                _trace.Trace(TraceLayer.Data, $"SQLite :memory: connection opened · EnsureCreated() · seeded {rows} work orders across 3 tenants");
            }
        }

        /// <summary>
        /// A new unit of work. Callers dispose it when the operation ends — the trace shows the pair.
        /// </summary>
        public EnterpriseOpsDbContext CreateContext(string purpose)
        {
            int number = Interlocked.Increment(ref _instanceCounter);
            Interlocked.Increment(ref _liveContexts);
            var context = new EnterpriseOpsDbContext(_options, number);
            _trace.Trace(TraceLayer.Data, $"DbContext #{number} created (short-lived · {purpose})");
            return context;
        }

        /// <summary>Called by the services in their finally block so the trace can show the disposal.</summary>
        public void Release(EnterpriseOpsDbContext context, string note = null)
        {
            int tracked = context.ChangeTracker.Entries().Count();
            context.Dispose();
            Interlocked.Decrement(ref _liveContexts);
            _trace.Trace(TraceLayer.Data, $"DbContext #{context.InstanceNumber} disposed ({tracked} tracked entities released{(note == null ? "" : " · " + note)}) · live contexts: {_liveContexts}");
        }

        public void Dispose()
        {
            Gate.Dispose();
            _connection.Dispose();     // closing the connection deletes the in-memory database
        }
    }
}
