using System;

namespace TicketOps.Security
{
    /// <summary>
    /// Plain C# implementation of <see cref="IUserSession"/>. Nothing here is static and nothing
    /// comes from the client: only <see cref="AuthenticationService"/> calls <see cref="SignIn"/>,
    /// and only after the password was verified on the server.
    /// </summary>
    public sealed class UserSession : IUserSession
    {
        public IUserContext User { get; private set; }

        public bool IsAuthenticated => User != null;

        public event EventHandler Changed;

        public void SignIn(IUserContext user)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void SignOut()
        {
            User = null;
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
