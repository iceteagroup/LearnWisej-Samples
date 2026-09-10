using TicketOps.Domain;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// Who this browser session is. One instance per session, created by <c>AppComposition</c> and handed
    /// to the service on every save — the role the server checks comes from here, never from the command
    /// a client built. (Module 2 owns the full SessionContext; Module 5 only needs the actor.)
    /// </summary>
    public sealed class SessionContext
    {
        public string UserName { get; set; } = "t.nguyen";
        public UserRole Role { get; set; } = UserRole.Technician;

        public override string ToString() => $"{UserName} ({Role})";
    }
}
