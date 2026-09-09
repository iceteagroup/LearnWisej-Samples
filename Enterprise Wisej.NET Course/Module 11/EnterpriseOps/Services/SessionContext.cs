using System;
using System.Diagnostics;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// Per-session state: who is signed in, for which tenant, since when — and the factory for correlation ids.
    /// Created once in Program.Main, stored in Application.Session, handed to the page. Never static.
    /// </summary>
    public sealed class SessionContext
    {
        private readonly Stopwatch _startup;

        private SessionContext(string sessionId)
        {
            SessionId = sessionId ?? "";
            StartedUtc = DateTime.UtcNow;
            _startup = Stopwatch.StartNew();
        }

        public string SessionId { get; }
        public DateTime StartedUtc { get; }
        public AppUser User { get; set; }
        public Tenant Tenant { get; set; }

        public TimeSpan SessionAge => DateTime.UtcNow - StartedUtc;

        /// <summary>The lesson's demo session: ana.ops (Manager) working the fabrikam tenant.</summary>
        public static SessionContext Start(string sessionId)
        {
            return new SessionContext(sessionId)
            {
                User = AppUser.Directory[0],
                Tenant = Tenant.Fabrikam,
            };
        }

        /// <summary>Stops the startup stopwatch (Program.Main → main page Load) and returns the milliseconds.</summary>
        public long StopStartupTimer()
        {
            _startup.Stop();
            return _startup.ElapsedMilliseconds;
        }

        /// <summary>Eight hex characters: short enough to read over the phone, unique enough for one day of logs.</summary>
        public string NewCorrelationId() => Guid.NewGuid().ToString("N").Substring(0, 8);

        /// <summary>A new command context for one user action: tenant + user + a fresh correlation id.</summary>
        public CommandContext NewCommand() => new CommandContext(Tenant.Id, User.UserName, NewCorrelationId());
    }
}
