using System;
using System.Collections.Generic;
using EnterpriseOps.Security;
using Wisej.Web;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// Per-session identity: who is signed in, for which tenant, and the correlation id every
    /// boundary crossing carries. Created once per session in the page constructor and parked in
    /// Application.Session — never in a static.
    ///
    /// SECURITY: this is the ONLY place the user name, the role and the tenant come from.
    /// Nothing that arrives from JavaScript is allowed to change any of the three.
    /// </summary>
    public sealed class SessionContext
    {
        public string TenantId { get; private set; } = "contoso";
        public string UserName { get; private set; }
        public string DisplayName { get; private set; }
        public Role Role { get; private set; }

        /// <summary>Stable per session; every command carries it into the trace and the audit log.</summary>
        public string CorrelationId { get; } = NewCorrelationId();

        public string SessionId { get; set; }

        public void SignInAs(string userName)
        {
            AppUser user = AppUser.Find(userName);
            UserName = user.UserName;
            DisplayName = user.DisplayName;
            Role = user.Role;
        }

        /// <summary>
        /// A fresh command context per boundary crossing. The correlation id the browser SENT is
        /// only echoed for log correlation; the tenant and the user always come from here.
        /// </summary>
        public CommandContext NewCommand(string clientCorrelationId = null)
            => new CommandContext
            {
                TenantId = TenantId,
                UserName = UserName,
                Role = Role,
                CorrelationId = string.IsNullOrEmpty(clientCorrelationId) ? NewCorrelationId() : clientCorrelationId,
            };

        public static string NewCorrelationId() => Guid.NewGuid().ToString("N").Substring(0, 8);
    }

    /// <summary>What every service command carries: tenant + user + role + correlation id.</summary>
    public sealed class CommandContext
    {
        public string TenantId;
        public string UserName;
        public Role Role;
        public string CorrelationId;
    }

    /// <summary>Typed result every service command returns to the UI — never an entity, never an exception.</summary>
    public sealed class CommandResult
    {
        public bool Succeeded;
        public string Code = ResultCodes.Ok;
        public string Message;
        public string CorrelationId;
        public List<string> Errors = new List<string>();

        public static CommandResult Ok(string correlationId, string message)
            => new CommandResult { Succeeded = true, Code = ResultCodes.Ok, CorrelationId = correlationId, Message = message };

        public static CommandResult Fail(string correlationId, string code, string message)
            => new CommandResult
            {
                Succeeded = false,
                Code = code,
                CorrelationId = correlationId,
                Message = message,
                Errors = new List<string> { message },
            };
    }

    /// <summary>
    /// The named failure codes of the interop contract. They are part of the published contract
    /// (docs/InteropContract.md): the browser may branch on them, so they never change silently and
    /// they never carry an exception message or a stack trace.
    /// </summary>
    public static class ResultCodes
    {
        public const string Ok = "OK";
        public const string MalformedPayload = "MALFORMED_PAYLOAD";
        public const string UnknownCommand = "UNKNOWN_COMMAND";
        public const string InvalidTarget = "INVALID_TARGET";
        public const string InvalidState = "INVALID_STATE";
        public const string PermissionDenied = "PERMISSION_DENIED";
        public const string ServerError = "SERVER_ERROR";
    }

    /// <summary>
    /// Tiny per-session factory. Nothing user- or tenant-specific lives in a static: the page owns
    /// one graph of services, and the graph dies with the session.
    /// </summary>
    public static class ServiceRegistry
    {
        public static SessionContext CreateSessionContext()
        {
            var context = new SessionContext();

            // The sample has no login screen: every session is the course's technician, who may
            // escalate and open the work queue but not approve — so the palette's "approve"
            // answers PERMISSION_DENIED, as in the walkthrough.
            context.SignInAs("ben.tech");

            try { context.SessionId = Application.SessionId; }
            catch (Exception) { context.SessionId = "design-time"; }
            try { Application.Session.Context = context; }       // the per-user dynamic bag
            catch (Exception) { /* design time / no session */ }
            return context;
        }
    }
}
