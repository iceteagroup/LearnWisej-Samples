using System;
using System.Collections.Generic;
using Wisej.Web;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// Per-session identity: who is using this Release dashboard, for which tenant, and the
    /// correlation id every trace line and command carries. Created once per session in the page
    /// constructor and parked in Application.Session — never in a static.
    /// </summary>
    public class SessionContext
    {
        public string TenantId = "contoso";
        public string User = "ana.ops";
        public string Role = "Manager";
        public string CorrelationId = Guid.NewGuid().ToString("N").Substring(0, 8);
        public string SessionId;             // Application.SessionId — the thing sticky sessions pin

        public CommandContext NewCommand() => new CommandContext
        {
            TenantId = TenantId,
            User = User,
            CorrelationId = CorrelationId,
        };
    }

    /// <summary>What every service command carries: tenant + user + correlation id.</summary>
    public class CommandContext
    {
        public string TenantId;
        public string User;
        public string CorrelationId;
    }

    /// <summary>Typed result every command returns to the UI — never an entity, never an exception.</summary>
    public class CommandResult
    {
        public bool Succeeded;
        public List<string> Errors = new List<string>();
        public string CorrelationId;
        public string Message;

        public static CommandResult Ok(string correlationId, string message = null)
            => new CommandResult { Succeeded = true, CorrelationId = correlationId, Message = message };

        public static CommandResult Fail(string correlationId, params string[] errors)
            => new CommandResult { Succeeded = false, CorrelationId = correlationId, Errors = new List<string>(errors) };
    }

    /// <summary>
    /// Tiny factory that hands the page its per-session services. Nothing user- or tenant-specific
    /// is stored in a static; process-wide facts (HostConfiguration, HealthProbeService) are.
    /// </summary>
    public static class ServiceRegistry
    {
        public static SessionContext CreateSessionContext()
        {
            var ctx = new SessionContext { SessionId = Application.SessionId };
            Application.Session.Context = ctx;      // the per-user bag (dynamic)
            return ctx;
        }
    }
}
