using System;
using System.Collections.Generic;
using AccessOps.Identity;
using AccessOps.Security;
using Microsoft.Extensions.Logging;

namespace AccessOps.Documents
{
    /// <summary>The outcome of a protected document operation.</summary>
    public sealed class DocumentResult
    {
        /// <summary>The one sentence every deny shows, whatever the reason.</summary>
        public const string DeniedMessage = "Access denied.";

        /// <summary>The one sentence an unexpected failure shows.</summary>
        public const string FailedMessage = "The document could not be loaded. Try again later.";

        private DocumentResult() { }

        /// <summary>"allowed", "denied" or "failed".</summary>
        public string Decision { get; private init; }

        /// <summary>Why it was denied ("grant", "role", "tenant", "record"). For the audit event only.</summary>
        public string Reason { get; private init; }

        /// <summary>The document, only when allowed.</summary>
        public Document Document { get; private init; }

        /// <summary>Correlates the result with its audit event and log lines.</summary>
        public string CorrelationId { get; private init; }

        public bool IsAllowed => this.Decision == "allowed";

        public static DocumentResult Allowed(Document document, string correlationId)
            => new DocumentResult { Decision = "allowed", Document = document, CorrelationId = correlationId };

        public static DocumentResult Denied(string reason, string correlationId)
            => new DocumentResult { Decision = "denied", Reason = reason, CorrelationId = correlationId };

        public static DocumentResult Failed(string correlationId)
            => new DocumentResult { Decision = "failed", Reason = "repository", CorrelationId = correlationId };
    }

    /// <summary>
    /// The service boundary every document path crosses - the Open click, the Export job and any
    /// later download or API. It reads the current grant and compares the tenant before the
    /// repository runs; a role claim, a visible button or an authenticated flag never decides.
    /// </summary>
    public sealed class DocumentService
    {
        private readonly GrantStore _grants;
        private readonly IDocumentRepository _repository;
        private readonly AuditLog _audit;
        private readonly IClock _clock;
        private readonly ILogger<DocumentService> _logger;

        public DocumentService(GrantStore grants, IDocumentRepository repository, AuditLog audit, IClock clock, ILogger<DocumentService> logger)
        {
            _grants = grants;
            _repository = repository;
            _audit = audit;
            _clock = clock;
            _logger = logger;
        }

        /// <summary>
        /// Reads one record for <paramref name="subject"/>. Denied when the grant is missing,
        /// belongs to another account, is revoked or expired, lacks read access, or covers a
        /// different tenant than <paramref name="tenant"/>; only then does the repository run.
        /// Every outcome writes one audit event.
        /// </summary>
        public DocumentResult Read(Subject subject, string tenant, int recordId, string action = "read")
        {
            var correlationId = AuditLog.NewCorrelationId();
            var resource = $"{tenant} / {recordId}";

            var grant = _grants.Find(subject?.GrantId);               // current state, not a copy
            string reason = null;
            if (grant is null || grant.Revoked || grant.ExpiresAt <= _clock.Now
                || !StringComparer.Ordinal.Equals(grant.AccountId, subject.AccountId))
                reason = "grant";
            else if (!grant.CanRead)
                reason = "role";
            else if (!StringComparer.Ordinal.Equals(grant.Tenant, tenant))
                reason = "tenant";

            if (reason != null)
            {
                _audit.Write(new AuditEvent(subject?.Opaque ?? "anonymous", action, resource, null, "denied", reason, correlationId));
                return DocumentResult.Denied(reason, correlationId);
            }

            // only now does the repository run
            Document document;
            try
            {
                document = _repository.Read(tenant, recordId);
            }
            catch (Exception ex)
            {
                // No subject, tenant, session or token in this line: the correlation ID links it
                // to the audit event, which carries the opaque subject.
                _logger.LogError(ex, "Document repository failed. action={Action} correlation={CorrelationId}", action, correlationId);
                _audit.Write(new AuditEvent(subject.Opaque, action, resource, null, "failed", "repository", correlationId));
                return DocumentResult.Failed(correlationId);
            }

            if (document is null)
            {
                _audit.Write(new AuditEvent(subject.Opaque, action, resource, null, "denied", "record", correlationId));
                return DocumentResult.Denied("record", correlationId);
            }

            _audit.Write(new AuditEvent(subject.Opaque, action, resource, document.Classification, "allowed", null, correlationId));
            return DocumentResult.Allowed(document, correlationId);
        }

        /// <summary>
        /// The documents of the subject's granted tenant, or null when the grant does not allow
        /// reading. The tenant comes from the grant, never from the screen.
        /// </summary>
        public IReadOnlyList<DocumentListItem> List(Subject subject)
        {
            var grant = _grants.Find(subject?.GrantId);
            string reason = null;
            if (grant is null || grant.Revoked || grant.ExpiresAt <= _clock.Now
                || !StringComparer.Ordinal.Equals(grant.AccountId, subject.AccountId))
                reason = "grant";
            else if (!grant.CanRead)
                reason = "role";

            if (reason != null)
            {
                _audit.Write(new AuditEvent(subject?.Opaque ?? "anonymous", "list", "documents", null, "denied", reason, AuditLog.NewCorrelationId()));
                return null;
            }

            return _repository.List(grant.Tenant);
        }
    }
}
