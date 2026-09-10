using System;
using TicketOps.Infrastructure;

namespace TicketOps.Security
{
    /// <summary>
    /// Plain C# implementation of <see cref="IUserSession"/>. Nothing here is static and nothing
    /// comes from the client: only <see cref="AuthenticationService"/> calls <see cref="SignIn"/>,
    /// and only after the password was verified on the server.
    /// </summary>
    public sealed class UserSession : IUserSession
    {
        private readonly ILog _log;

        public UserSession(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public IUserContext User { get; private set; }

        public bool IsAuthenticated => User != null;

        public event EventHandler Changed;

        public void SignIn(IUserContext user)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            _log.Info(LogLayer.Session, "UserSession.SignIn", $"identity bound to this session: {user}");
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void SignOut()
        {
            var previous = User;
            User = null;
            _log.Info(LogLayer.Session, "UserSession.SignOut", previous == null ? "no identity to clear" : $"identity cleared ({previous.UserName})");
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
