using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>Who is signed in to this session and with which role. One per session, never static.</summary>
    public interface IUserContext
    {
        string UserName { get; }
        OperatorRole Role { get; }
    }
}
