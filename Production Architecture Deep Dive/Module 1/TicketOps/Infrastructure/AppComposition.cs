using TicketOps.Data;
using TicketOps.Services;
using TicketOps.Views;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// The one place that knows how the session's object graph is wired ("who gets what").
    /// Program.Main creates one AppComposition per session, so nothing here is static: every browser
    /// session owns its own log, repository and service. Screens receive their dependencies through
    /// their constructors — they never new-up a service and never read one from a global.
    ///
    /// Module 8 replaces this hand-written factory with Application.Services (dependency injection);
    /// the rule it enforces stays the same.
    /// </summary>
    public sealed class AppComposition
    {
        public ActivityLog Log { get; } = new ActivityLog();
        public InMemoryTicketRepository Repository { get; }
        public ITicketService Tickets { get; }

        public AppComposition()
        {
            Log.Info(LogLayer.Infrastructure, "AppComposition", "composing the session object graph (nothing static)");
            Repository = new InMemoryTicketRepository(Log);
            Tickets = new TicketService(Repository, Log);
            Log.Info(LogLayer.Infrastructure, "AppComposition", "ITicketService → TicketService(InMemoryTicketRepository, ActivityLog)");
        }

        public TicketEditor CreateMainView()
        {
            return new TicketEditor(Tickets, Repository, Log);
        }
    }
}
