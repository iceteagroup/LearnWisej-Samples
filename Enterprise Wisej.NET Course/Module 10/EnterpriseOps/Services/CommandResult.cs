using System.Collections.Generic;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// What a service hands back to a screen: whether it worked, what to say if it did not, and the correlation
    /// id that ties the answer to the audit entry and the trace lines.
    ///
    /// Two kinds of "no" are distinguished on purpose. A <see cref="Denied"/> result is an authorization answer —
    /// the user may see a precise message ("you don't have permission to export data"), because naming the
    /// permission helps them ask for it and helps the help desk. A failure is everything else, and the user gets
    /// a generic message plus the correlation id, while the details stay in the log.
    /// </summary>
    public class CommandResult
    {
        protected CommandResult(bool succeeded, bool denied, string summary, string correlationId, IReadOnlyList<string> errors)
        {
            Succeeded = succeeded;
            Denied = denied;
            Summary = summary;
            CorrelationId = correlationId;
            Errors = errors ?? new List<string>();
        }

        public bool Succeeded { get; }

        /// <summary>True when a permission check refused the command. Nothing was executed.</summary>
        public bool Denied { get; }

        public string Summary { get; }
        public string CorrelationId { get; }
        public IReadOnlyList<string> Errors { get; }

        public string FirstError => Errors.Count > 0 ? Errors[0] : Summary;

        public static CommandResult Ok(string summary, string correlationId)
            => new CommandResult(true, false, summary, correlationId, null);

        public static CommandResult Refused(string reason, string correlationId)
            => new CommandResult(false, true, "denied", correlationId, new List<string> { reason });

        public static CommandResult Failed(string reason, string correlationId)
            => new CommandResult(false, false, "failed", correlationId, new List<string> { reason });
    }
}
