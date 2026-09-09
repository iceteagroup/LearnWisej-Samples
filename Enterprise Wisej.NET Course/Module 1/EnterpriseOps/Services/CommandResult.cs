using System.Collections.Generic;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// What every application service returns to a screen: did it succeed, and if not, messages that are safe
    /// to show. Expected failures (validation, policy, stale version) travel here; unexpected ones are exceptions
    /// that the handler's catch logs and hides behind a generic message.
    /// </summary>
    public class CommandResult
    {
        public bool Succeeded { get; protected set; }
        public List<string> Errors { get; } = new List<string>();
        public string CorrelationId { get; protected set; }

        public static CommandResult Ok(string correlationId) =>
            new CommandResult { Succeeded = true, CorrelationId = correlationId };

        public static CommandResult Fail(string correlationId, params string[] errors)
        {
            var result = new CommandResult { Succeeded = false, CorrelationId = correlationId };
            result.Errors.AddRange(errors);
            return result;
        }
    }
}
