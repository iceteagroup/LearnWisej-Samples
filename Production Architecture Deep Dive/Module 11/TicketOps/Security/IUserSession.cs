using System;

namespace TicketOps.Security
{
    /// <summary>
    /// The server-side holder of "who is using this session". One instance per browser session
    /// (created in AppComposition — never static: a static would be shared by every user on the server).
    /// Authentication fills it once at sign-in; every later service call reads it to authorize.
    /// </summary>
    public interface IUserSession
    {
        /// <summary>The signed-in user, or null before the login gate was passed.</summary>
        IUserContext User { get; }

        bool IsAuthenticated { get; }

        /// <summary>Raised on sign-in and sign-out so Infrastructure can mirror the identity into the host (Application.User).</summary>
        event EventHandler Changed;

        void SignIn(IUserContext user);
        void SignOut();
    }
}
