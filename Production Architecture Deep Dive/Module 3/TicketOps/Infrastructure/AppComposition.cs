using TicketOps.Data;
using TicketOps.Services;
using TicketOps.Views;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// The one place that knows how the session's object graph is wired ("who gets what").
    /// Program.Main creates one AppComposition per session, so nothing here is static: every browser
    /// session owns its own log, repository, service and profile catalog. Screens receive their
    /// dependencies through their constructors — they never new-up a service and never read one from a global.
    ///
    /// Module 3: the same graph as Module 1 plus the <see cref="ClientProfileCatalog"/> (what ClientProfiles.json
    /// defines) and the current user the "Mine" chip filters on. Module 8 replaces this hand-written factory
    /// with Application.Services; the rule it enforces stays the same.
    /// </summary>
    public sealed class AppComposition
    {
        /// <summary>The signed-in agent the workspace acts as (a fake user store arrives in Module 11).</summary>
        public const string CurrentUser = "S. Patel";

        public ActivityLog Log { get; } = new ActivityLog();
        public InMemoryTicketRepository Repository { get; }
        public ITicketService Tickets { get; }
        public ClientProfileCatalog Profiles { get; }

        public AppComposition()
        {
            Log.Info(LogLayer.Infrastructure, "AppComposition", "composing the session object graph (nothing static)");
            Repository = new InMemoryTicketRepository(Log);
            Tickets = new TicketService(Repository, Log, CurrentUser);
            Profiles = ClientProfileCatalog.Load(Log);
            Log.Info(LogLayer.Infrastructure, "AppComposition", $"ITicketService → TicketService(InMemoryTicketRepository, ActivityLog, user \"{CurrentUser}\")");
        }

        public MainPage CreateMainView()
        {
            return new MainPage(Tickets, Repository, Profiles, Log);
        }
    }
}
