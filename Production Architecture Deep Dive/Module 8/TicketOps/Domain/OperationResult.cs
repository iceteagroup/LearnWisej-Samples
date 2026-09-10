using System.Collections.Generic;

namespace TicketOps.Domain
{
    /// <summary>
    /// What a service hands back: success or a short, safe explanation.
    /// The message may be shown verbatim, so it must never contain internals.
    /// Unexpected failures are exceptions, not results: the handler catches, logs and shows a generic message.
    /// </summary>
    public sealed class OperationResult<T>
    {
        public bool Succeeded { get; }
        public string Message { get; }
        public T Value { get; }
        public IReadOnlyList<string> Errors { get; }

        private OperationResult(bool succeeded, string message, T value, IReadOnlyList<string> errors)
        {
            Succeeded = succeeded;
            Message = message;
            Value = value;
            Errors = errors ?? new string[0];
        }

        public static OperationResult<T> Ok(T value, string message = null)
            => new OperationResult<T>(true, message ?? "OK", value, null);

        public static OperationResult<T> Fail(string message, params string[] errors)
            => new OperationResult<T>(false, message, default, errors);

        public override string ToString() => Succeeded ? $"OK · {Message}" : $"FAIL · {Message}";
    }
}
