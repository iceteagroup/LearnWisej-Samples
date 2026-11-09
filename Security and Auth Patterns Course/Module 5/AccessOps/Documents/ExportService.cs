using System;
using System.Security.Principal;
using AccessOps.Identity;
using AccessOps.Security;
using Microsoft.Extensions.Logging;

namespace AccessOps.Documents
{
    /// <summary>
    /// The Export path. Before the repository runs it checks, in this order: the grant named by
    /// the principal's <c>grant</c> claim (current row: exists, same account, not revoked, not
    /// expired, may read), the tenant asked for against the grant's tenant, and the record. A
    /// role claim, a visible Export button or an authenticated flag decides nothing - which is why
    /// a tab that was open before sign-out is denied with zero repository reads (fixture F05).
    /// </summary>
    public sealed class ExportService
    {
        private readonly GrantStore _grants;
        private readonly IDocumentRepository _repository;
        private readonly CountingDocumentRepository _counter;
        private readonly AuditLog _audit;
        private readonly ILogger<ExportService> _logger;

        /// <summary>Creates the service.</summary>
        public ExportService(GrantStore grants, IDocumentRepository repository, CountingDocumentRepository counter, AuditLog audit, ILogger<ExportService> logger)
        {
            _grants = grants;
            _repository = repository;
            _counter = counter;
            _audit = audit;
            _logger = logger;
        }

        /// <summary>Exports one record for <paramref name="principal"/> (<c>Application.User</c>).</summary>
        public DocumentResult Export(IPrincipal principal, string tenant, int recordId)
        {
            var subject = Subject.From(principal);
            var correlationId = AuditLog.NewCorrelationId();
            var resource = $"{tenant} / {recordId}";
            var readsBefore = _counter.ReadCount;

            // 1. the grant from the principal's grant claim - its current state
            var reason = _grants.Check(subject, out var grant);
            // 2. the tenant asked for must be the grant's tenant
            if (reason == null && !StringComparer.Ordinal.Equals(grant.Tenant, tenant))
                reason = "tenant";
            // 3. the record: a positive number (existence is the repository's answer below)
            if (reason == null && recordId <= 0)
                reason = "record";

            if (reason != null)
                return Deny(subject, resource, reason, correlationId, readsBefore);

            // only now does the repository run
            Document document;
            try
            {
                document = _repository.Read(tenant, recordId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Export failed in the repository. correlation={CorrelationId}", correlationId);
                _audit.Write(new AuditEvent(subject.Opaque, "export", resource, null, "failed", "repository", correlationId));
                return DocumentResult.Failed(correlationId);
            }

            if (document is null)
                return Deny(subject, resource, "record", correlationId, readsBefore);

            _audit.Write(new AuditEvent(subject.Opaque, "export", resource, document.Classification, "allowed", null, correlationId));
            _logger.LogInformation("export allowed grant={Grant} repositoryReads=+{Reads} correlation={CorrelationId}",
                grant.Display, _counter.ReadCount - readsBefore, correlationId);
            return DocumentResult.Allowed(document, correlationId);
        }

        private DocumentResult Deny(Subject subject, string resource, string reason, string correlationId, int readsBefore)
        {
            _audit.Write(new AuditEvent(subject?.Opaque ?? "anonymous", "export", resource, null, "denied", reason, correlationId));
            _logger.LogInformation("export denied reason={Reason} grant={Grant} repositoryReads=+{Reads} correlation={CorrelationId}",
                reason, Grant.Shorten(subject?.GrantId), _counter.ReadCount - readsBefore, correlationId);
            return DocumentResult.Denied(reason, correlationId);
        }
    }
}
