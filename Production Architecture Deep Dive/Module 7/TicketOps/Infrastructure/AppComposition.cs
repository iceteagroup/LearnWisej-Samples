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
    /// Module 7 — what is per session and what is shared:
    /// <list type="bullet">
    ///   <item>per session: the log, the repository (and the tickets it holds), TicketService, ImportService, the page,
    ///         the running import (its CancellationTokenSource is an ImportPage instance field);</item>
    ///   <item>shared by every session: the CSV files on disk (read-only), the TicketImportRules constants, the code.</item>
    /// </list>
    /// The import task started by this session's page writes to this session's repository only, so two tabs
    /// importing at once never touch the same dictionary — the lock inside the repository is for the two
    /// threads of ONE session (import task + request thread), not for two users.
    /// </summary>
    public sealed class AppComposition
    {
        public ActivityLog Log { get; } = new ActivityLog();
        public InMemoryTicketRepository Repository { get; }
        public ITicketService Tickets { get; }
        public IImportService Import { get; }
        public ImportFileLocator Files { get; }

        public AppComposition()
        {
            Log.Info(LogLayer.Infrastructure, "AppComposition", "composing the session object graph (nothing static)");
            Repository = new InMemoryTicketRepository(Log);
            Tickets = new TicketService(Repository, Log);
            Import = new ImportService(Repository, Log);
            Files = new ImportFileLocator(Log);
            Log.Info(LogLayer.Infrastructure, "AppComposition",
                "IImportService → ImportService(InMemoryTicketRepository, ActivityLog) · ITicketService → TicketService(same repository)");
        }

        public ImportPage CreateMainView()
        {
            return new ImportPage(Import, Tickets, Repository, Files, Log);
        }
    }
}
