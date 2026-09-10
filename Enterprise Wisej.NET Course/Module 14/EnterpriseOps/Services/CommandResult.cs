using System.Collections.Generic;
using System.Linq;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// What every application service returns to a screen: did it succeed, and if not, messages that are safe
    /// to show a user. Expected failures (permission denied, stale version, a missing deliverable) travel here;
    /// unexpected ones stay exceptions that the handler's catch logs behind a generic message.
    /// </summary>
    public class CommandResult
    {
        public bool Succeeded { get; protected set; }

        public List<string> Errors { get; } = new List<string>();

        public string CorrelationId { get; protected set; }

        public string ErrorText => string.Join(" · ", Errors);

        public static CommandResult Ok(string correlationId) =>
            new CommandResult { Succeeded = true, CorrelationId = correlationId };

        public static CommandResult Fail(string correlationId, params string[] errors)
        {
            var result = new CommandResult { Succeeded = false, CorrelationId = correlationId };
            result.Errors.AddRange(errors.Where(e => !string.IsNullOrEmpty(e)));
            return result;
        }
    }
}
