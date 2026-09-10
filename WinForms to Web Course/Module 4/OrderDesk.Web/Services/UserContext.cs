using System;
using OrderDesk.Domain;
using Wisej.Web;

namespace OrderDesk.Services
{
    /// <summary>
    /// The typed per-session replacement for the AppState statics (Module 4).
    /// Storage is Application.Session (one dynamic bag per browser session); callers read
    /// UserContext.Current without knowing where it lives, and Clear() is the single place that
    /// forgets everything on sign-out, ApplicationExit and SessionTimeout.
    /// </summary>
    public sealed class UserContext
    {
        public User User { get; set; }
        public Customer CurrentCustomer { get; set; }
        public OrderStatus? ActiveFilter { get; set; }
        public string LastSearch { get; set; }
        public DateTime CreatedUtc { get; } = DateTime.UtcNow;

        /// <summary>A short identity so the trace can show that two sessions hold two different objects.</summary>
        public string Handle => "UserContext#" + (GetHashCode() & 0xffff).ToString("x4");

        public bool IsSignedIn => User != null;

        /// <summary>The current session's context, created on first use.</summary>
        public static UserContext Current
        {
            get
            {
                dynamic s = Application.Session;
                if (s.UserContext == null)
                    s.UserContext = new UserContext();
                return (UserContext)s.UserContext;
            }
        }

        /// <summary>True when the session bag already holds a context (Current would create one).</summary>
        public static bool Exists
        {
            get
            {
                dynamic s = Application.Session;
                return s.UserContext != null;
            }
        }

        /// <summary>Forget everything about the user of THIS session only — other sessions are untouched.</summary>
        public static void Clear()
        {
            dynamic s = Application.Session;
            s.UserContext = null;
        }
    }
}
