namespace TicketOps.Security
{
    /// <summary>
    /// The sensitive things a user can do in the TicketOps Console. Services ask
    /// <see cref="IPermissionService"/> "can THIS user do THIS" for one of these before acting.
    /// A permission is not a role: roles are what the directory knows about a person, permissions
    /// are what the application needs to decide. <see cref="PermissionService"/> maps one to the other.
    /// </summary>
    public enum Permission
    {
        ViewTickets,
        AddNote,
        CloseTicket,
        DeleteTicket,
        ViewAuditTrail
    }

    /// <summary>The demo roles. Production reads roles from the identity provider, not from constants.</summary>
    public static class Roles
    {
        public const string Technician = "Technician";
        public const string Supervisor = "Supervisor";
        public const string Admin = "Admin";
    }
}
