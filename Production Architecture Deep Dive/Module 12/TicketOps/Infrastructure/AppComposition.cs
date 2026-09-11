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
    /// is role-gated by the session's user context.
    /// </summary>
    public sealed class AppComposition
    {
        public ActivityLog Log { get; } = new ActivityLog();
        public SessionUserContext User { get; }
        public IHealthCheckService Health { get; }
        public IDiagnosticsService Diagnostics { get; }

        public AppComposition()
        {
            User = new SessionUserContext();
            var runtime = new WisejRuntimeInfo();
            Health = new HealthCheckService(new FileHealthCheckSource(), new InMemoryTicketRepository(), runtime, Log);
            Diagnostics = new DiagnosticsService(runtime, Health, User, Log);
        }

        public ReleaseConsole CreateMainView()
        {
            return new ReleaseConsole(Diagnostics, User, Log);
        }
    }
}
