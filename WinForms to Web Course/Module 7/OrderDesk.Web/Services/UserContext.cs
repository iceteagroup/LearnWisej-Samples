using OrderDesk.Domain;
using Wisej.Web;

namespace OrderDesk.Services
{
    /// <summary>
    /// Per-session user context (Module 4's replacement for the LegacyOrderDesk AppState statics).
    /// Lives in <see cref="Application.Session"/>, so kelly's tab and dana's tab never see each other's
    /// values. Module 7 uses it for the role checks (guarded download) and the audit trail.
    /// </summary>
    public sealed class UserContext
    {
        public static readonly User[] KnownUsers =
        {
            new User { Id = 1, UserName = "kelly", DisplayName = "Kelly", Role = "Clerk",   Company = "Acme" },
            new User { Id = 2, UserName = "dana",  DisplayName = "Dana",  Role = "Manager", Company = "Acme" },
        };

        public User User { get; set; }

        public string UserName => User?.UserName ?? "anonymous";
        public string Role => User?.Role ?? "-";
        public bool IsInRole(string role) => User != null && string.Equals(User.Role, role, System.StringComparison.OrdinalIgnoreCase);

        /// <summary>The context of the current session — created on first use, stored in Application.Session.</summary>
        public static UserContext Current
        {
            get
            {
                dynamic s = Application.Session;
                if (s.UserContext == null) s.UserContext = new UserContext { User = KnownUsers[0] };
                return (UserContext)s.UserContext;
            }
        }

        /// <summary>Called on sign-out and from Application.ApplicationExit so nothing stale survives the session.</summary>
        public static void Clear()
        {
            try
            {
                dynamic s = Application.Session;
                s.UserContext = null;
            }
            catch { /* the session may already be gone when ApplicationExit runs */ }
        }
    }
}
