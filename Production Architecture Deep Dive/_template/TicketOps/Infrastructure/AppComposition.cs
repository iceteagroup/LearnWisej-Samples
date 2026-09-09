using TicketOps.Views;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// The one place that knows how the session's object graph is wired ("who gets what").
    /// Program.Main creates one AppComposition per session, so nothing here is static: every browser
    /// session owns its own log, repositories and services. Screens receive their dependencies through
    /// their constructors — they never new-up a service and never read one from a global.
    ///
    /// Module N: extend this class with the module's repositories and services.
    /// </summary>
    public sealed class AppComposition
    {
        public ActivityLog Log { get; } = new ActivityLog();

        public AppComposition()
        {
            Log.Info(LogLayer.Infrastructure, "AppComposition", "session object graph composed (nothing static)");
        }

        public MainView CreateMainView()
        {
            return new MainView(Log);
        }
    }
}
