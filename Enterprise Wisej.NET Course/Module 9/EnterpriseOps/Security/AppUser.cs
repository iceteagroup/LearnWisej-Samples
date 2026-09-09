using System;
using System.Linq;

namespace EnterpriseOps.Security
{
    public enum Role { Technician, Manager, Admin }

    /// <summary>
    /// The course's three users. The role comes from the server-side session, never from
    /// anything the browser sends (see ClientCommandService.ExecuteTrustingClient for the
    /// anti-pattern that breaks this rule on purpose).
    /// </summary>
    public sealed class AppUser
    {
        public string UserName { get; }
        public string DisplayName { get; }
        public Role Role { get; }

        private AppUser(string userName, string displayName, Role role)
        {
            UserName = userName;
            DisplayName = displayName;
            Role = role;
        }

        public static readonly AppUser[] Directory =
        {
            new AppUser("ana.ops",    "Ana Ops",    Role.Manager),
            new AppUser("ben.tech",   "Ben Tech",   Role.Technician),
            new AppUser("cara.admin", "Cara Admin", Role.Admin),
        };

        public static AppUser Find(string userName)
            => Directory.FirstOrDefault(u => string.Equals(u.UserName, userName, StringComparison.OrdinalIgnoreCase))
               ?? throw new ArgumentException($"Unknown user '{userName}'.", nameof(userName));

        public override string ToString() => $"{UserName} ({Role})";
    }
}
