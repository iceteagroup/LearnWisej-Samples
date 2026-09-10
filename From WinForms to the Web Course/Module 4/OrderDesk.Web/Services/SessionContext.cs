using Wisej.Web;

namespace OrderDesk.Services
{
    /// <summary>
    /// ✓ The session-context pattern from the lesson: callers write
    /// <c>SessionContext.Current.CurrentFilter</c> exactly as they used to write
    /// <c>AppState.CurrentFilter</c>, but the value is stored in <see cref="Application.Session"/>,
    /// which Wisej.NET keeps per browser session. Nobody outside this class knows WHERE the
    /// context is stored, so swapping the bag for a distributed cache later touches one file.
    ///
    /// Quick fix vs typed context: the quick fix is to redirect every static to
    /// <c>Application.Session.CurrentFilter</c> (dynamic, untyped, scattered). The typed context
    /// keeps the members discoverable, gives one place to clear on logout (<see cref="Reset"/>)
    /// and one place to add cleanup hooks.
    /// </summary>
    public static class SessionContext
    {
        // Application.Session is a dynamic bag AND a Dictionary<string, object>; the lesson uses
        // the dynamic form (Application.Session.UserContext). A missing member reads as null.
        private const string Key = "UserContext";

        /// <summary>The context of the calling browser session — created lazily on first access.</summary>
        public static UserSessionContext Current
        {
            get
            {
                dynamic session = Application.Session;
                UserSessionContext context = session.UserContext as UserSessionContext;
                if (context == null)
                {
                    context = new UserSessionContext();
                    session.UserContext = context;
                }
                return context;
            }
        }

        /// <summary>True once this session has signed in (never looks at any other session).</summary>
        public static bool IsSignedIn => Current.IsSignedIn;

        /// <summary>
        /// Logout / timeout: drop the whole context so the next access starts anonymous.
        /// Only THIS session is affected — compare Legacy.AppState, where clearing the statics
        /// signs out every user on the server.
        /// </summary>
        public static void Reset()
        {
            Application.Session[Key] = null;
        }
    }
}
