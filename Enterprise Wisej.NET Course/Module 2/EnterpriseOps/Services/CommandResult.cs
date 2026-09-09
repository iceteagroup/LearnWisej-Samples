using System.Collections.Generic;

namespace EnterpriseOps.Services
{
    /// <summary>Typed outcome of a command. The UI shows it; it never inspects entities or exceptions to find out what happened.</summary>
    public class CommandResult
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = "";
        public List<string> Errors { get; } = new List<string>();
        public string CorrelationId { get; set; }

        public string ErrorText => string.Join("; ", Errors);

        public static CommandResult Ok(CommandContext ctx, string message)
        {
            return new CommandResult { Succeeded = true, Message = message, CorrelationId = ctx.CorrelationId };
        }

        public static CommandResult Fail(CommandContext ctx, params string[] errors)
        {
            var result = new CommandResult { Succeeded = false, CorrelationId = ctx.CorrelationId };
            result.Errors.AddRange(errors);
            result.Message = result.ErrorText;
            return result;
        }
    }

    /// <summary>A page of projected rows plus the numbers the grid footer needs.</summary>
    public class PagedResult<T>
    {
        public List<T> Rows { get; set; } = new List<T>();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
