using TicketOps.Data;
using TicketOps.Services;
using TicketOps.Views;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// The one place that knows how the session's object graph is wired ("who gets what").
    /// Program.Main creates one AppComposition per session, so nothing here is static: every browser
    /// session owns its own log, repository, services and user context. Screens receive their dependencies
    /// through their constructors — they never new-up a service and never read one from a global.
    ///
    /// Module 12: the deployment graph. The health check reads HealthCheck.json (FileHealthCheckSource) and
    /// probes the ticket repository; the diagnostics service reads runtime facts through WisejRuntimeInfo and
    /// is role-gated by the session's user context; the load test drives ITicketService like a user would.
    /// </summary>
    public sealed class AppComposition
    {
        public ActivityLog Log { get; } = new ActivityLog();
        public InMemoryTicketRepository Repository { get; }
        public ITicketService Tickets { get; }
        public SessionUserContext User { get; }
        public IRuntimeInfo Runtime { get; }
        public IHealthCheckSource HealthSource { get; }
        public IHealthCheckService Health { get; }
        public IDiagnosticsService Diagnostics { get; }
        public ILoadTestService LoadTest { get; }

        public AppComposition()
        {
            Log.Info(LogLayer.Infrastructure, "AppComposition", "composing the session object graph (nothing static)");

            Repository = new InMemoryTicketRepository(Log);
            Tickets = new TicketService(Repository, Log);
            User = new SessionUserContext(Log);
            Runtime = new WisejRuntimeInfo();
            HealthSource = new FileHealthCheckSource(Log);
            Health = new HealthCheckService(HealthSource, Repository, Runtime, Log);
            Diagnostics = new DiagnosticsService(Runtime, Health, User, Log);
            LoadTest = new LoadTestService(Tickets, Log);

            Log.Info(LogLayer.Infrastructure, "AppComposition", "IHealthCheckService → HealthCheckService(FileHealthCheckSource, InMemoryTicketRepository, WisejRuntimeInfo)");
            Log.Info(LogLayer.Infrastructure, "AppComposition", "IDiagnosticsService → DiagnosticsService(WisejRuntimeInfo, IHealthCheckService, SessionUserContext) · ILoadTestService → LoadTestService(ITicketService)");
        }

        public ReleaseConsole CreateMainView()
        {
            return new ReleaseConsole(Diagnostics, Health, LoadTest, Tickets, Repository, User, Log);
        }
    }
}
