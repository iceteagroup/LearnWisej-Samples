using System;
using System.Collections.Generic;

namespace EnterpriseOps.Diagnostics
{
    /// <summary>
    /// The `_log.Error(ex)` of the lab's handler shape. Keeps the exceptions of the session (Module 11 turns
    /// this into a real diagnostics page) and mirrors each one into the activity trace with its correlation id.
    ///
    /// The exception type and message go here — never into the UI. The user sees Resources/UiText.ActionFailed.
    /// </summary>
    public sealed class ErrorLog
    {
        private readonly ActivityTrace _trace;
        private readonly List<LoggedError> _errors = new List<LoggedError>();

        public ErrorLog(ActivityTrace trace)
        {
            _trace = trace;
        }

        public IReadOnlyList<LoggedError> Errors => _errors;

        public void Error(Exception ex, string correlationId = null)
        {
            var entry = new LoggedError(DateTime.UtcNow, correlationId, ex);
            _errors.Add(entry);

            _trace.Diagnostics($"ERROR corr={correlationId ?? "-"} {ex.GetType().Name}: {ex.Message}");
        }
    }

    public sealed class LoggedError
    {
        public LoggedError(DateTime whenUtc, string correlationId, Exception exception)
        {
            WhenUtc = whenUtc;
            CorrelationId = correlationId;
            Exception = exception;
        }

        public DateTime WhenUtc { get; }
        public string CorrelationId { get; }
        public Exception Exception { get; }
    }
}
