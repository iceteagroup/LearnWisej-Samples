using System.Collections.Generic;
using System.Linq;

namespace EnterpriseOps.Services
{
    /// <summary>The typed outcome of a service call: succeeded or a list of errors, always with the correlation id.</summary>
    public class CommandResult
    {
        protected CommandResult(bool succeeded, string correlationId, IReadOnlyList<string> errors)
        {
            this.Succeeded = succeeded;
            this.CorrelationId = correlationId;
            this.Errors = errors ?? new string[0];
        }

        public bool Succeeded { get; }
        public string CorrelationId { get; }
        public IReadOnlyList<string> Errors { get; }

        public string ErrorText => string.Join("; ", this.Errors);

        public static CommandResult Ok(string correlationId) => new CommandResult(true, correlationId, null);

        public static CommandResult Fail(string correlationId, params string[] errors)
            => new CommandResult(false, correlationId, errors);
    }

    /// <summary>A <see cref="CommandResult"/> that carries a value when it succeeded.</summary>
    public class CommandResult<T> : CommandResult
    {
        private CommandResult(bool succeeded, string correlationId, IReadOnlyList<string> errors, T value)
            : base(succeeded, correlationId, errors)
        {
            this.Value = value;
        }

        public T Value { get; }

        public static CommandResult<T> Ok(string correlationId, T value) => new CommandResult<T>(true, correlationId, null, value);

        public static new CommandResult<T> Fail(string correlationId, params string[] errors)
            => new CommandResult<T>(false, correlationId, errors.ToArray(), default(T));
    }

    /// <summary>One page of a server-side paged query.</summary>
    public class PagedResult<T>
    {
        public PagedResult(IReadOnlyList<T> rows, int total, int page, int pageSize)
        {
            this.Rows = rows;
            this.Total = total;
            this.Page = page;
            this.PageSize = pageSize;
        }

        public IReadOnlyList<T> Rows { get; }
        public int Total { get; }
        public int Page { get; }
        public int PageSize { get; }
    }
}
