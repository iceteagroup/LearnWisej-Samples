using System;
using System.Security.Claims;
using System.Security.Principal;
using AccessOps.Documents;
using AccessOps.Identity;
using Microsoft.Extensions.Logging;
using Wisej.Web;

namespace AccessOps.Security
{
    /// <summary>
    /// The one check both routes call before any protected read (Module 7). It reads the grant
    /// named by the principal's <c>grant</c> claim from the durable <see cref="GrantStore"/> - the
    /// row as it is now, never a copy kept by a screen, a claim or the Wisej.NET session - and
    /// refuses a missing grant, another account's grant, a revoked or expired grant, a grant
    /// without the read right and a tenant other than the grant's. Every refusal writes
    /// <see cref="AuditService.Deny"/> and returns before the repository is touched, so a deny is
    /// provable as "repository calls = 0". The intranet route (Windows host principal mapped to an
    /// AccessOps grant) and the partner route (own sign-in, OpenID Connect, passkey) end in the same
    /// application principal and therefore in this same decision.
    /// </summary>
    public sealed class RecordPolicy
    {
        /// <summary>The boundary name a policy deny carries in the audit and the evidence matrix.</summary>
        public const string Boundary = "policy";

        private readonly GrantStore _grants;
        private readonly IDocumentRepository _records;
        private readonly AuditService _audit;
        private readonly IClock _clock;
        private readonly ILogger<RecordPolicy> _logger;

        /// <summary>Creates the policy over the shared store.</summary>
        public RecordPolicy(GrantStore grants, IDocumentRepository records, AuditService audit, IClock clock, ILogger<RecordPolicy> logger)
        {
            _grants = grants;
            _records = records;
            _audit = audit;
            _clock = clock;
            _logger = logger;
        }

        /// <summary>
        /// The lesson's shape: reads one record for the session's <c>Application.User</c>; null
        /// when the policy denies (the repository never ran) or the tenant has no such record.
        /// </summary>
        public Document Read(string tenantId, int recordId)
            => Read(Application.User, tenantId, recordId).Document;

        /// <summary>Reads one record for <paramref name="principal"/>; the result says why when it is refused.</summary>
        public DocumentResult Read(IPrincipal principal, string tenantId, int recordId, string action = "record.read")
        {
            var subject = Subject.From(principal);
            var correlationId = AuditService.NewCorrelationId();
            var decision = Decide(subject, tenantId, out var grant);
            if (decision != null)
            {
                _audit.Deny(Boundary, grant?.Id ?? subject.GrantId, tenantId, recordId, decision, subject.Opaque, action, correlationId);
                return DocumentResult.Denied(decision, correlationId);   // repository never runs
            }

            var resource = $"{tenantId} / {recordId}";
            Document document;
            try
            {
                document = _records.Read(tenantId, recordId);
            }
            catch (Exception ex)
            {
                // the exception goes to the server log only; the screen shows one fixed sentence
                _logger.LogError(ex, "Record read failed in the repository. correlation={CorrelationId}", correlationId);
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
        /// The same decision without a repository read, for work that is not a record read (the
        /// intranet route's scoped file read): grant, read right and tenant, one audit event.
        /// <paramref name="resource"/> names the object for the audit ("DOC-201").
        /// </summary>
        public DocumentResult Authorize(Subject subject, string tenantId, string resource, string action)
        {
            var decision = Decide(subject, tenantId, out _);
            var correlationId = AuditService.NewCorrelationId();
            _audit.Write(new AuditEvent(subject?.Opaque ?? "anonymous", action, $"{tenantId} / {resource}", null,
                decision == null ? "allowed" : "denied", decision == null ? null : Boundary + ":" + decision, correlationId));
            return decision == null ? DocumentResult.Authorized(correlationId) : DocumentResult.Denied(decision, correlationId);
        }

        /// <summary>
        /// Null when the subject's current grant allows reading <paramref name="tenantId"/>;
        /// otherwise the reason: "missing", "account", "revoked", "expired", "role" or "tenant".
        /// </summary>
        public string Decide(Subject subject, string tenantId, out Grant grant)
        {
            grant = _grants.Find(subject?.GrantId);                      // durable store, every call
            if (grant is null)
                return "missing";
            if (!StringComparer.Ordinal.Equals(grant.AccountId, subject.AccountId))
                return "account";
            if (grant.Revoked)
                return "revoked";
            if (grant.ExpiresAt <= _clock.Now)
                return "expired";
            if (!grant.CanRead)
                return "role";
            if (!StringComparer.Ordinal.Equals(grant.Tenant, tenantId))
                return "tenant";
            return null;
        }

        /// <summary>True for the reasons that mean "this sign-in no longer works" (the screen says so).</summary>
        public static bool IsGrantEnded(string reason)
            => reason is "missing" or "account" or "revoked" or "expired";
    }
}
