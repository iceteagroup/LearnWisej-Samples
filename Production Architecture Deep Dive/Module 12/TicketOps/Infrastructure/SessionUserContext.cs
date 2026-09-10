using System;
using TicketOps.Domain;
using TicketOps.Services;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// Who is signed in to this session. Created once per session in AppComposition — never static, because a
    /// static would make every browser tab on the server the same user. The demo signs in m.weber (Supervisor);
    /// the lab's "sign in as Technician" switch swaps to l.romero to show the access-denied path.
    /// </summary>
    public sealed class SessionUserContext : IUserContext
    {
        private readonly ILog _log;

        public string UserName { get; private set; } = "m.weber";
        public OperatorRole Role { get; private set; } = OperatorRole.Supervisor;

        public SessionUserContext(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _log.Info(LogLayer.Session, "SessionUserContext", $"signed in: {UserName} ({Role}) — this session only");
        }

        public void SignInAs(string userName, OperatorRole role)
        {
            UserName = userName;
            Role = role;
            _log.Info(LogLayer.Session, "SessionUserContext.SignInAs", $"{userName} ({role}) — other sessions are unaffected");
        }
    }
}
