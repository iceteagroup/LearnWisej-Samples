namespace TicketOps.Domain
{
    public enum OperatorRole
    {
        Viewer,
        Technician,
        Manager
    }

    /// <summary>
    /// The person using the console. Per-user state like "who is signed in" belongs to a Session
    /// service (IUserService) — never to a Shared one and never to a static.
    /// </summary>
    public sealed class Operator
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public OperatorRole Role { get; set; }
        public string Email { get; set; }

        public override string ToString() => $"{Name} ({Role})";
    }
}
