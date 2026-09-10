using System;
using TicketOps.Data;
using TicketOps.Diagnostics;
using TicketOps.Services;
using TicketOps.Views;
using Wisej.Web;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// The one place that knows how the session's object graph is wired ("who gets what").
    /// Program.Main creates one AppComposition per session, so every browser session owns its own log,
    /// its own <see cref="SessionContext"/>, its own repositories and services. Screens receive their
    /// dependencies through their constructors — they never new-up a service and never read one from a global.
    ///
    /// Three lifetimes meet here, and the module is about telling them apart:
    ///   • process  — <see cref="ProcessScope.Settings"/> (read once, immutable) and <see cref="SharedTicketStore.Instance"/> (thread-safe):
    ///                the same object for every session, so it is only ever shared read-only or under a lock;
    ///   • session  — everything created in this constructor: log, SessionContext, directory, repository, services, the view;
    ///   • request  — nothing is kept here; a click's locals die with the round-trip.
    ///
    /// This hand-written factory IS the "registration with session lifetime" of the lab: one instance per
    /// session, dropped with the session (Wisej.NET releases the session's object graph when it ends; nothing
    /// static holds a reference back into it). Module 8 replaces it with Application.Services and
    /// ServiceLifetime.Session; the rule it enforces stays the same.
    /// </summary>
    public sealed class AppComposition
    {
        public ActivityLog Log { get; } = new ActivityLog();
        public AppSettings Settings { get; }
        public SessionContext Context { get; }
        public InMemoryUserDirectory Users { get; }
        public InMemoryTicketRepository Tickets { get; }
        public ISessionService Session { get; }
        public ITicketService TicketService { get; }
        public IDiagnosticsService DiagnosticsService { get; }

        public AppComposition()
        {
            Log.Info(LogLayer.Infrastructure, "AppComposition", "composing the session object graph (nothing per-user is static)");

            // 1. Process scope — shared, read-only. The first session pays for the read; later ones reuse it.
            bool reused = ProcessScope.SettingsAlreadyLoaded;
            Settings = ProcessScope.Settings;
            Log.Info(LogLayer.Infrastructure, "ProcessScope.Settings",
                reused
                    ? $"AppSettings reused (loaded once per process at {Settings.LoadedAtUtc.ToLocalTime():HH:mm:ss} from {Settings.LoadedFrom})"
                    : $"AppSettings loaded now — first session of this process — from {Settings.LoadedFrom}");

            // 2. Session scope — this connection's own context, filled from the Wisej.NET session and the defaults.
            Context = CreateSessionContext(Settings);
            long contexts = SharedCounters.ContextCreated();
            Log.Info(LogLayer.Session, "AppComposition", $"new SessionContext {Context} · contexts created on this server: {contexts}");

            // Also park it in Wisej.NET's per-session bag, for code that cannot be constructor-injected
            // (a static helper, a legacy Form). Same instance, same lifetime — the diagnostics page checks it.
            try
            {
                Application.Session.SessionContext = Context;
                Log.Info(LogLayer.Session, "Application.Session", "SessionContext also parked in Application.Session (per-session bag) — same instance");
            }
            catch (Exception ex)
            {
                Log.Warn(LogLayer.Session, "Application.Session", $"could not park the context: {ex.GetType().Name}");
            }

            // 3. Session-scoped services, all constructor-injected with THIS session's context.
            Users = new InMemoryUserDirectory(Log);
            Tickets = new InMemoryTicketRepository(SharedTicketStore.Instance, Log);
            Session = new SessionService(Context, Users, Settings, Log);
            TicketService = new TicketService(Context, Tickets, Log);
            DiagnosticsService = new DiagnosticsService(Settings, new WisejRuntimeInfo(), Context, SharedTicketStore.Instance, Log);
            Log.Info(LogLayer.Infrastructure, "AppComposition",
                "ISessionService → SessionService(SessionContext, InMemoryUserDirectory, AppSettings) · ITicketService → TicketService(SessionContext, InMemoryTicketRepository → SharedTicketStore) · IDiagnosticsService → DiagnosticsService(AppSettings, WisejRuntimeInfo, SessionContext)");

            // Wisej.NET session lifecycle events: a refresh keeps the session (and this graph); exit ends it.
            try
            {
                Application.ApplicationRefresh += Application_ApplicationRefresh;
                Application.ApplicationExit += Application_ApplicationExit;
            }
            catch (Exception ex)
            {
                Log.Warn(LogLayer.Session, "Application events", $"could not subscribe: {ex.GetType().Name}");
            }
        }

        public SessionDiagnostics CreateMainView()
        {
            return new SessionDiagnostics(Context, Session, TicketService, DiagnosticsService, Users, Log);
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

        private void Application_ApplicationRefresh(object sender, EventArgs e)
        {
            Log.Info(LogLayer.Session, "Application.ApplicationRefresh",
                $"browser refresh — same session {Context.ShortId}: SessionContext, log and services are kept (no new composition)");
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            // Nobody sees this line (the session is going away) — it is here to show where per-session
            // resources would be released: dispose what the SessionContext holds, flush anything pending.
            Log.Info(LogLayer.Session, "Application.ApplicationExit", $"session {Context.ShortId} ending — per-session graph released");
        }
    }
}
