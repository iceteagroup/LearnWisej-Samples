using System.Collections.Generic;

namespace EnterpriseOps.Security
{
    public enum Role { Technician, Manager, Admin }

    /// <summary>The three course users. Identity is per session (SessionContext.User), never static.</summary>
    public sealed class AppUser
    {
        public string UserName { get; private set; }
        public string DisplayName { get; private set; }
        public Role Role { get; private set; }

        public static readonly IReadOnlyList<AppUser> Directory = new[]
        {
            new AppUser { UserName = "ana.ops",    DisplayName = "Ana (operations manager)", Role = Role.Manager },
            new AppUser { UserName = "ben.tech",   DisplayName = "Ben (field technician)",   Role = Role.Technician },
            new AppUser { UserName = "cara.admin", DisplayName = "Cara (administrator)",     Role = Role.Admin },
        };

        public override string ToString() => $"{UserName}  ·  {Role}";
    }
}
