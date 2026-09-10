namespace TicketOps.Domain
{
    /// <summary>
    /// Who is signed in to this browser session. Held by the per-session <c>SessionContext</c>
    /// (never a static), handed to services so that every decision — "may this user share that
    /// link?" — is made on the server against a value the browser cannot edit.
    /// </summary>
    public sealed class SessionUser
    {
        public string Name { get; }
        public string Role { get; }

        public SessionUser(string name, string role)
        {
            Name = name;
            Role = role;
        }

        public bool IsManager => Role == "Manager";

        public override string ToString() => $"{Name} ({Role})";
    }
}
