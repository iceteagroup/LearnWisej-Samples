using TicketOps.Domain;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// Per-session facts: who is signed in. Created once per browser session by AppComposition and handed
    /// to the screen — never a static, so two tabs never see each other's user. In Module 11 this is what
    /// a real login fills in; here it is the technician the walkthrough video follows.
    /// </summary>
    public sealed class SessionContext
    {
        public SessionUser User { get; }

        public SessionContext(SessionUser user)
        {
            User = user;
        }
    }
}
