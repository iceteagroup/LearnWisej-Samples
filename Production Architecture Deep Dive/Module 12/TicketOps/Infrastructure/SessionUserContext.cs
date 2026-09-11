using TicketOps.Domain;
using TicketOps.Services;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// Who is signed in to this session. Created once per session in AppComposition — never static, because a
    /// static would make every browser tab on the server the same user. The capstone signs in m.weber (Supervisor);
    /// the login gate of Module 11 is where a real identity would come from.
    /// </summary>
    public sealed class SessionUserContext : IUserContext
    {
        public string UserName { get; } = "m.weber";
        public OperatorRole Role { get; } = OperatorRole.Supervisor;
    }
}
