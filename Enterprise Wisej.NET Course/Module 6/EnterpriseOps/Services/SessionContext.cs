using System;
using System.Collections.Generic;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// Who is using this session: tenant + user + role. Stored in Application.Session by Program.Main —
    /// per session, never static — so it survives "Reopen page" and is gone when the session is gone.
    /// </summary>
    public sealed class SessionContext
    {
        public SessionContext(string tenantId, string tenantName, string user, string role)
        {
            TenantId = tenantId;
            TenantName = tenantName;
            User = user;
            Role = role;
            SessionCorrelationId = NewCorrelationId();
        }

        public string TenantId { get; }
        public string TenantName { get; }
        public string User { get; }
        public string Role { get; }
        public string SessionCorrelationId { get; }

        /// <summary>Every command gets its own correlation id; the job that a command starts inherits it.</summary>
        public CommandContext NewCommand() => new CommandContext(TenantId, User, Role, NewCorrelationId());

        internal static string NewCorrelationId() => Guid.NewGuid().ToString("N").Substring(0, 8);
    }

    /// <summary>Tenant + user + correlation id — what every service method receives and every trace line shows.</summary>
    public sealed class CommandContext
    {
        public CommandContext(string tenantId, string user, string role, string correlationId)
        {
            TenantId = tenantId;
            User = user;
            Role = role;
            CorrelationId = correlationId;
        }

        public string TenantId { get; }
        public string User { get; }
        public string Role { get; }
        public string CorrelationId { get; }
    }

    /// <summary>Typed result of a command. The UI shows it; it never sees an entity or an exception.</summary>
    public class CommandResult
    {
        public bool Succeeded { get; protected set; }
        public IReadOnlyList<string> Errors { get; protected set; } = Array.Empty<string>();
        public string CorrelationId { get; protected set; }

        public static CommandResult Ok(string correlationId) =>
            new CommandResult { Succeeded = true, CorrelationId = correlationId };

        public static CommandResult Fail(string correlationId, params string[] errors) =>
            new CommandResult { Succeeded = false, CorrelationId = correlationId, Errors = errors };
    }

    public sealed class CommandResult<T> : CommandResult
    {
        public T Value { get; private set; }

        public static CommandResult<T> Ok(string correlationId, T value) =>
            new CommandResult<T> { Succeeded = true, CorrelationId = correlationId, Value = value };

        public static new CommandResult<T> Fail(string correlationId, params string[] errors) =>
            new CommandResult<T> { Succeeded = false, CorrelationId = correlationId, Errors = errors };
    }
}
