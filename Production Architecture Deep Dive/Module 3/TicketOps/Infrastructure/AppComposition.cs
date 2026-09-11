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
    /// </summary>
    public sealed class AppComposition
    {
        /// <summary>The signed-in agent the workspace acts as (the "Mine" chip filters on it).</summary>
        public const string CurrentUser = "S. Patel";

        public ActivityLog Log { get; } = new ActivityLog();
        public ITicketService Tickets { get; }

        public AppComposition()
        {
            var repository = new InMemoryTicketRepository();
            Tickets = new TicketService(repository, Log, CurrentUser);
        }

        public MainPage CreateMainView()
        {
            return new MainPage(Tickets, Log);
        }
    }
}
