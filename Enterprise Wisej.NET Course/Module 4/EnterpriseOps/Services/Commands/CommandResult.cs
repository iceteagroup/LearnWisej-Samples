using System.Collections.Generic;

namespace EnterpriseOps.Services.Commands
{
    /// <summary>
    /// What every command returns — success or a safe message with an error code, never a raw exception.
    /// The shape from the walkthrough (<c>Success · UserMessage · ErrorCode</c>) plus the course-wide
    /// <c>CorrelationId</c> and <c>Errors</c>, and <c>Detail</c> for the banner's second line.
    /// </summary>
    public sealed class CommandResult
    {
        public bool Success { get; private set; }

        /// <summary>Safe to show to the user as-is. Never contains SQL, a stack trace or an entity.</summary>
        public string UserMessage { get; private set; }

        /// <summary>Stable, searchable code (WO_STATE_INVALID, WO_CONCURRENCY, …). Null on success.</summary>
        public string ErrorCode { get; private set; }

        /// <summary>The second line of the banner: "committed · audit written · correlation 7c41aa90".</summary>
        public string Detail { get; private set; }

        /// <summary>Quote this to support; the audit row and the trace carry the same id.</summary>
        public string CorrelationId { get; private set; }

        /// <summary>Field-level validation messages, when there are any.</summary>
        public IReadOnlyList<string> Errors { get; private set; }

        /// <summary>The work order's version after a committed change (v8 → v9), when relevant.</summary>
        public int? NewVersion { get; private set; }

        public int? WorkOrderId { get; private set; }

        private CommandResult() { }

        public static CommandResult Ok(string message, string correlationId, int? workOrderId = null, int? newVersion = null)
            => new CommandResult
            {
                Success = true,
                UserMessage = message,
                Detail = $"committed · audit written · correlation {correlationId}",
                CorrelationId = correlationId,
                WorkOrderId = workOrderId,
                NewVersion = newVersion,
                Errors = new string[0],
            };

        public static CommandResult Fail(string message, string code, string correlationId, IReadOnlyList<string> errors = null, string detail = null)
            => new CommandResult
            {
                Success = false,
                UserMessage = message,
                ErrorCode = code,
                Detail = detail ?? $"{code} · transaction rolled back · nothing persisted",
                CorrelationId = correlationId,
                Errors = errors ?? new string[0],
            };

        public override string ToString()
            => Success ? $"CommandResult.Ok({UserMessage})" : $"CommandResult.Fail({ErrorCode}: {UserMessage})";
    }
}
