using System.Collections.Generic;
using AccessOps.Identity;
using AccessOps.Security;

namespace AccessOps.Documents
{
    /// <summary>The outcome of a protected document operation.</summary>
    public sealed class DocumentResult
    {
        /// <summary>The sentence a deny shows when the subject's grant is no longer valid (signed out, expired).</summary>
        public const string EndedMessage = "Your sign-in has ended. Sign in again to continue.";

        /// <summary>The sentence every other deny shows (tenant, record, role).</summary>
        public const string DeniedMessage = "Access denied.";

        /// <summary>The sentence an unexpected failure shows.</summary>
        public const string FailedMessage = "The document could not be loaded. Try again later.";

        private DocumentResult() { }

        /// <summary>"allowed", "denied" or "failed".</summary>
        public string Decision { get; private init; }

        /// <summary>
        /// Why it was denied ("missing", "account", "revoked", "expired", "role", "tenant", "record";
        /// "grant" from the list check). For the audit event and the evidence matrix only.
        /// </summary>
        public string Reason { get; private init; }

        /// <summary>The document, only when allowed.</summary>
        public Document Document { get; private init; }

        /// <summary>Correlates the result with its audit event and log lines.</summary>
        public string CorrelationId { get; private init; }

        /// <summary>True when the operation was allowed.</summary>
        public bool IsAllowed => this.Decision == "allowed";

        /// <summary>The one sentence the screen shows for a refusal.</summary>
        public string Message => this.Decision == "failed" ? FailedMessage : this.GrantEnded ? EndedMessage : DeniedMessage;

        /// <summary>True when the refusal means the sign-in itself no longer works (missing, revoked, expired grant).</summary>
        public bool GrantEnded => this.Reason == "grant" || RecordPolicy.IsGrantEnded(this.Reason);

        /// <summary>An allowed result.</summary>
        public static DocumentResult Allowed(Document document, string correlationId)
            => new DocumentResult { Decision = "allowed", Document = document, CorrelationId = correlationId };

        /// <summary>The policy allowed the operation (no document attached: the scoped file read).</summary>
        public static DocumentResult Authorized(string correlationId)
            => new DocumentResult { Decision = "allowed", CorrelationId = correlationId };

        /// <summary>A denied result.</summary>
        public static DocumentResult Denied(string reason, string correlationId)
            => new DocumentResult { Decision = "denied", Reason = reason, CorrelationId = correlationId };

        /// <summary>A failed result (the repository threw).</summary>
        public static DocumentResult Failed(string correlationId)
            => new DocumentResult { Decision = "failed", Reason = "repository", CorrelationId = correlationId };
    }

    /// <summary>
    /// The Documents list: the tenant's titles for the grid, only while the subject's current
    /// grant allows reading. The tenant comes from the grant, never from the screen. Reading a
    /// record's content goes through <see cref="RecordPolicy"/> (via <see cref="ExportService"/>).
    /// </summary>
    public sealed class DocumentService
    {
        private readonly GrantStore _grants;
        private readonly IDocumentRepository _repository;
        private readonly AuditService _audit;

        /// <summary>Creates the service.</summary>
        public DocumentService(GrantStore grants, IDocumentRepository repository, AuditService audit)
        {
            _grants = grants;
            _repository = repository;
            _audit = audit;
        }

        /// <summary>The documents of the subject's granted tenant, or null when the grant does not allow reading.</summary>
        public IReadOnlyList<DocumentListItem> List(Subject subject)
        {
            var reason = _grants.Check(subject, out var grant);
            if (reason != null)
            {
                _audit.Write(new AuditEvent(subject?.Opaque ?? "anonymous", "list", "documents", null, "denied", reason, AuditService.NewCorrelationId()));
                return null;
            }

            return _repository.List(grant.Tenant);
        }
    }
}
