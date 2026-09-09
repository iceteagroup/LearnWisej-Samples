namespace EnterpriseOps.Security
{
    public enum UserRole { Technician, Manager, Admin }

    /// <summary>The signed-in user as the services see it (never a static — it hangs off SessionContext).</summary>
    public class UserIdentity
    {
        public string UserName;
        public UserRole Role;

        public override string ToString() => $"{UserName} ({Role})";
    }

    /// <summary>The course's three users. The workflow resolves EscalationCommand.RequestedBy through it.</summary>
    public static class UserDirectory
    {
        public static readonly UserIdentity AnaOps = new UserIdentity { UserName = "ana.ops", Role = UserRole.Manager };
        public static readonly UserIdentity BenTech = new UserIdentity { UserName = "ben.tech", Role = UserRole.Technician };
        public static readonly UserIdentity CaraAdmin = new UserIdentity { UserName = "cara.admin", Role = UserRole.Admin };

        public static UserIdentity Find(string userName) => userName switch
        {
            "ana.ops" => AnaOps,
            "ben.tech" => BenTech,
            "cara.admin" => CaraAdmin,
            _ => null,
        };
    }
}
