using System;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// Per-session state: the tenant, the signed-in user and the command that is running right now.
    ///
    /// One instance per browser session, created in Program.Main and stored in Application.Session — never
    /// in a static field (two users would share it). Every command a screen sends to a service starts with
    /// <see cref="BeginCommand"/>, which mints a fresh correlation id.
    /// </summary>
    public sealed class SessionContext
    {
        private SessionContext(Tenant tenant, UserIdentity user)
        {
            Tenant = tenant;
            User = user;
            SessionStartedUtc = DateTime.UtcNow;
        }

        public static SessionContext CreateFor(Tenant tenant, UserIdentity user) => new SessionContext(tenant, user);

        /// <summary>What the Designer and a parameterless page constructor get: the default tenant and manager.</summary>
        public static SessionContext CreateDefault() => CreateFor(Tenants.Contoso, KnownUsers.AnaOps);

        public Tenant Tenant { get; }
        public UserIdentity User { get; private set; }
        public DateTime SessionStartedUtc { get; }
        public CommandContext CurrentCommand { get; private set; }

        /// <summary>Simulated sign-in switch (the lab's "refresh as ben.tech" path). Real authentication arrives in Module 7.</summary>
        public void SignInAs(UserIdentity user)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
        }

        /// <summary>Starts a new unit of work: same tenant and user, new correlation id.</summary>
        public CommandContext BeginCommand()
        {
            CurrentCommand = new CommandContext(Tenant.Id, User.UserName, User.Role, NewCorrelationId());
            return CurrentCommand;
        }

        private static string NewCorrelationId() => Guid.NewGuid().ToString("N").Substring(0, 8);
    }
}
