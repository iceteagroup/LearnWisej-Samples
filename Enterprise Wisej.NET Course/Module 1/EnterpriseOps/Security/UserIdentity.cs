namespace EnterpriseOps.Security
{
    /// <summary>Who is signed in. Immutable; the SessionContext swaps the whole identity when the user changes.</summary>
    public sealed class UserIdentity
    {
        public UserIdentity(string userName, UserRole role, string displayName)
        {
            UserName = userName;
            Role = role;
            DisplayName = displayName;
        }

        public string UserName { get; }
        public UserRole Role { get; }
        public string DisplayName { get; }

        public override string ToString() => $"{UserName} · {Role}";
    }

    /// <summary>The users every module of the course signs in as.</summary>
    public static class KnownUsers
    {
        public static readonly UserIdentity AnaOps = new UserIdentity("ana.ops", UserRole.Manager, "Ana Ops");
        public static readonly UserIdentity BenTech = new UserIdentity("ben.tech", UserRole.Technician, "Ben Tech");
        public static readonly UserIdentity CaraAdmin = new UserIdentity("cara.admin", UserRole.Admin, "Cara Admin");
    }
}
