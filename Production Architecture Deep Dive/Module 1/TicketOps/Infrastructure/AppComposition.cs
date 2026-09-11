using TicketOps.Data;
using TicketOps.Diagnostics;
using TicketOps.Services;
using TicketOps.Views;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// The one place that knows how the session's object graph is wired ("who gets what").
    /// Program.Main creates one AppComposition per session, so nothing here is static: every browser
    /// session owns its own log, repository and service. Screens receive their dependencies through
    /// their constructors — they never new-up a service and never read one from a global.
    /// </summary>
    public sealed class AppComposition
    {
        public ActivityLog Log { get; } = new ActivityLog();
        public ITicketService Tickets { get; }

        public AppComposition()
        {
            var repository = new InMemoryTicketRepository(Log);
            Tickets = new TicketService(repository, Log);
        }

        public TicketEditor CreateMainView()
        {
            return new TicketEditor(Tickets, Log);
        }
    }
}
