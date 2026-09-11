using TicketOps.Data;
using TicketOps.Services;
using TicketOps.Views;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// The one place that knows how the session's object graph is wired ("who gets what").
    /// Program.Main creates one AppComposition per session, so nothing here is static: every browser
    /// session owns its own log, repository and services. Screens receive their dependencies through
    /// their constructors — they never new-up a service and never read one from a global.
    ///
    /// What is per session and what is shared:
    /// <list type="bullet">
    ///   <item>per session: the log, the repository (and the tickets it holds), TicketService, ImportService, the page,
    ///         the running import (its CancellationTokenSource is an ImportPage instance field);</item>
    ///   <item>shared by every session: the CSV file on disk (read-only), the TicketImportRules constants, the code.</item>
    /// </list>
    /// The import task started by this session's page writes to this session's repository only, so the lock inside
    /// the repository is for the two threads of ONE session (import task + request thread), not for two users.
    /// </summary>
    public sealed class AppComposition
    {
        public ActivityLog Log { get; } = new ActivityLog();
        public ITicketService Tickets { get; }
        public IImportService Import { get; }
        public ImportFileLocator Files { get; }

        public AppComposition()
        {
            var repository = new InMemoryTicketRepository();
            Tickets = new TicketService(repository, Log);
            Import = new ImportService(repository, Log);
            Files = new ImportFileLocator(Log);
        }

        public ImportPage CreateMainView()
        {
            return new ImportPage(Import, Tickets, Files, Log);
        }
    }
}
