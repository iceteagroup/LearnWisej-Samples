using System;
using EnterpriseOps.Services;
using EnterpriseOps.Services.Commands;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// Translates what the database throws into what the application returns: a named result code,
    /// a user message and an audit line. Applied once, at the boundary, so a form, a batch and a job all
    /// receive the same codes. The full table is docs/ErrorMappingTable.md.
    /// </summary>
    public static class ErrorMap
    {
        public const string StateInvalid = "WO_STATE_INVALID";
        public const string Concurrency = "WO_CONCURRENCY";
        public const string NumberInUse = "WO_NUMBER_IN_USE";
        public const string ReferenceInvalid = "WO_REFERENCE_INVALID";
        public const string NotFound = "WO_NOT_FOUND";
        public const string ValidationFailed = "VALIDATION_FAILED";
        public const string PermissionDenied = "PERMISSION_DENIED";
        public const string DbTimeout = "DB_TIMEOUT";
        public const string DbUnavailable = "DB_UNAVAILABLE";
        public const string Unexpected = "UNEXPECTED";

        /// <summary>One mapped failure: the code, what the user reads, what the audit row records.</summary>
        public sealed class Mapped
        {
            public string Code { get; set; }
            public string UserMessage { get; set; }
            public string AuditDetail { get; set; }
        }

        public static Mapped Map(Exception exception, string operation, string correlationId)
        {
            // Unwrap the EF Core envelope: the provider exception is the interesting one.
            var sqlite = exception as SqliteException ?? exception.InnerException as SqliteException;

            if (exception is DbUpdateConcurrencyException)
                return new Mapped
                {
                    Code = Concurrency,
                    UserMessage = "This work order was changed by someone else while you were editing it. Reload and try again.",
                    AuditDetail = $"{operation}: optimistic-concurrency check failed (0 rows updated)",
                };

            if (exception is DbUpdateException && sqlite != null && sqlite.SqliteErrorCode == 19)   // SQLITE_CONSTRAINT
            {
                if (sqlite.Message.IndexOf("UNIQUE", StringComparison.OrdinalIgnoreCase) >= 0)
                    return new Mapped
                    {
                        Code = NumberInUse,
                        UserMessage = "That work order number is already in use for this tenant. Choose another number.",
                        AuditDetail = $"{operation}: UNIQUE constraint UX_WorkOrders_Tenant_Number",
                    };

                if (sqlite.Message.IndexOf("FOREIGN KEY", StringComparison.OrdinalIgnoreCase) >= 0)
                    return new Mapped
                    {
                        Code = ReferenceInvalid,
                        UserMessage = "The work order refers to a tenant that no longer exists.",
                        AuditDetail = $"{operation}: FOREIGN KEY constraint (TenantId)",
                    };

                return new Mapped
                {
                    Code = ValidationFailed,
                    UserMessage = "The work order could not be saved because a database rule rejected it.",
                    AuditDetail = $"{operation}: CHECK/NOT NULL constraint (sqlite extended code {sqlite.SqliteExtendedErrorCode})",
                };
            }

            if (exception is OperationCanceledException || exception is TimeoutException)
                return new Mapped
                {
                    Code = DbTimeout,
                    UserMessage = $"The operation could not be completed in time. Try again, or quote correlation id {correlationId} to support.",
                    AuditDetail = $"{operation}: command timeout elapsed inside the transaction",
                };

            if (sqlite != null)
                return new Mapped
                {
                    Code = DbUnavailable,
                    UserMessage = $"The database is not available right now. Quote correlation id {correlationId} to support.",
                    AuditDetail = $"{operation}: sqlite error {sqlite.SqliteErrorCode}",
                };

            return new Mapped
            {
                Code = Unexpected,
                UserMessage = $"The action could not be completed. Quote correlation id {correlationId} to support.",
                AuditDetail = $"{operation}: {exception.GetType().Name}",      // the type, never the stack trace
            };
        }

        /// <summary>The domain/validation rejections that never reach the database still get a code and a line.</summary>
        public static CommandResult Rejected(string code, string userMessage, string correlationId, IActivityTrace trace)
        {
            trace.Trace(TraceLayer.Service, $"CommandResult.Fail {code} → \"{userMessage}\"");
            return CommandResult.Fail(userMessage, code, correlationId);
        }
    }
}
