using TicketOps.Data;
using TicketOps.Services;
using TicketOps.Views;
using Wisej.Web;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// The one place that knows how the session's object graph is wired ("who gets what").
    /// Program.Main creates one AppComposition per session, so nothing here is static: every browser
    /// session owns its own log, session context, localization, repository, service and theme switcher.
    /// Screens receive their dependencies through their constructors — they never new-up a service and
    /// never read one from a global.
    ///
    /// Module 10: the SessionContext carries this operator's theme and culture; the LocalizationService
    /// owns the culture and the resources; the ThemeSwitcher is the only code that names a theme.
    /// </summary>
    public sealed class AppComposition
    {
        public ActivityLog Log { get; } = new ActivityLog();
        public SessionContext Session { get; }
        public ILocalizationService Localization { get; }
        public InMemoryWorkOrderRepository Repository { get; }
        public IWorkOrderService WorkOrders { get; }
        public ThemeSwitcher Themes { get; }

        public AppComposition()
        {
            Log.Info(LogLayer.Infrastructure, "AppComposition", "composing the session object graph (nothing static)");

            Session = new SessionContext { SessionId = Application.SessionId };
            Localization = new LocalizationService(Session, Log);
            Repository = new InMemoryWorkOrderRepository(Log);
            WorkOrders = new WorkOrderService(Repository, Localization, Log);
            Themes = new ThemeSwitcher(Session, Log);

            Log.Info(LogLayer.Infrastructure, "AppComposition",
                "ILocalizationService → LocalizationService(SessionContext) · IWorkOrderService → WorkOrderService(InMemoryWorkOrderRepository, ILocalizationService) · ThemeSwitcher(SessionContext)");
        }

        public OperationsDashboard CreateMainView()
        {
            return new OperationsDashboard(WorkOrders, Localization, Themes, Repository, Log);
        }
    }
}
