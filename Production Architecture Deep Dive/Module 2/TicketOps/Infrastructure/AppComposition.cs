using TicketOps.Data;
using TicketOps.Diagnostics;
using TicketOps.Services;
using TicketOps.Views;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// The one place that knows how the session's object graph is wired ("who gets what").
    /// Program.Main creates one AppComposition per session, so every browser session owns its own log,
    /// its own <see cref="SessionContext"/>, its own repositories and services. Screens receive their
    /// dependencies through their constructors — they never new-up a service and never read one from a global.
    ///
    /// Three lifetimes meet here:
    ///   • process  — <see cref="ProcessScope.Settings"/> (read once, immutable) and <see cref="SharedTicketStore.Instance"/> (thread-safe):
    ///                the same object for every session, so it is only ever shared read-only or under a lock;
    ///   • session  — everything created in this constructor: log, SessionContext, directory, repository, services, the view;
    ///   • request  — nothing is kept here; a click's locals die with the round-trip.
    ///
    /// This hand-written factory is the "registration with session lifetime": one instance per session,
    /// dropped with the session (nothing static holds a reference back into it).
    /// </summary>
    public sealed class AppComposition
    {
        public ActivityLog Log { get; } = new ActivityLog();
        public AppSettings Settings { get; }
        public SessionContext Context { get; }
        public ISessionService Session { get; }
        public ITicketService TicketService { get; }
        public IDiagnosticsService DiagnosticsService { get; }

        public AppComposition()
        {
            // Process scope — shared, read-only. The first session pays for the read; later ones reuse it.
            Settings = ProcessScope.Settings;

            // Session scope — this connection's own context, filled from the Wisej.NET session and the defaults.
            Context = CreateSessionContext(Settings);
            SharedCounters.ContextCreated();

            // Session-scoped services, all constructor-injected with THIS session's context.
            var users = new InMemoryUserDirectory();
            var tickets = new InMemoryTicketRepository(SharedTicketStore.Instance);
            Session = new SessionService(Context, users, Settings, Log);
            TicketService = new TicketService(Context, tickets);
            DiagnosticsService = new DiagnosticsService(Settings, new WisejRuntimeInfo(), Context, SharedTicketStore.Instance);
        }

        public SessionDiagnostics CreateMainView()
        {
            return new SessionDiagnostics(Context, Session, TicketService, DiagnosticsService, Log);
        }

        /// <summary>
        /// The moment "per session" values are born: copied from the Wisej.NET session (id, theme, client
        /// profile) and from the shared defaults (tenant). After this, only ISessionService changes them.
        /// </summary>
        private static SessionContext CreateSessionContext(AppSettings settings)
        {
            var runtime = new WisejRuntimeInfo();
            return new SessionContext
            {
                SessionId = WisejRuntimeInfo.ReadSessionId(),
                CurrentUser = "Alice Rivera",                          // Module 11 replaces this with the authenticated identity
                Tenant = settings.DefaultTenant,
                Theme = runtime.ActiveThemeName ?? settings.DefaultTheme,
                ClientProfile = runtime.ActiveClientProfile ?? "Default"
            };
        }
    }
}
