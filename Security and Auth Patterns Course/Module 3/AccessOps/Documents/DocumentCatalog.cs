using System;
using System.IO;
using System.Text.RegularExpressions;
using AccessOps.Identity;
using AccessOps.Infrastructure;

namespace AccessOps.Documents
{
    /// <summary>Thrown when the application decision refuses a document; no scope is ever opened after it.</summary>
    public sealed class AccessDeniedException : Exception
    {
        /// <summary>Creates the exception with the audit reason (never shown to the user).</summary>
        public AccessDeniedException(string reason, string correlationId)
            : base("Access denied.")
        {
            this.Reason = reason;
            this.CorrelationId = correlationId;
        }

        /// <summary>"grant", "role", "tenant", "record", "id" or "path" - for the audit and the trace.</summary>
        public string Reason { get; }

        /// <summary>Correlates the deny with its audit event.</summary>
        public string CorrelationId { get; }
    }

    /// <summary>
    /// Maps a server-side document ID to a canonical file path under the one approved root -
    /// only after the Module 1 resource policy (grant, tenant, record) allowed it. The browser never
    /// sends a path; anything that resolves outside the approved root is refused.
    /// </summary>
    public sealed class DocumentCatalog
    {
        private static readonly Regex DocumentIdFormat = new Regex("^DOC-[0-9]{3}$", RegexOptions.CultureInvariant);

        private readonly Subject _subject;
        private readonly ConfidentialFileStore _files;
        private readonly DocumentService _policy;

        /// <summary>A catalog for one subject (the app subject of the session).</summary>
        public DocumentCatalog(Subject subject, ConfidentialFileStore files, DocumentService policy)
        {
            _subject = subject;
            _files = files;
            _policy = policy;
        }

        /// <summary>The correlation ID of the last policy decision (links the audit and the trace).</summary>
        public string CorrelationId { get; private set; }

        /// <summary>
        /// Policy first, then the path: refuses an unknown or malformed ID, any grant/tenant
        /// failure, and any path that is not strictly below the approved root. Throws
        /// <see cref="AccessDeniedException"/>; returns the canonical path otherwise. It does not
        /// check that the file exists - a missing approved file (F03) fails inside the scope.
        /// </summary>
        public string ResolveApprovedPath(string documentId)
        {
            if (documentId == null || !DocumentIdFormat.IsMatch(documentId))
                throw Deny("id");

            var entry = _files.Find(documentId);
            if (entry == null)
                throw Deny("record");

            // 1. the application decision (grant, read right, tenant) - audited
            var decision = _policy.Authorize(_subject, entry.Tenant, documentId, "scoped-read");
            this.CorrelationId = decision.CorrelationId;
            if (!decision.IsAllowed)
                throw new AccessDeniedException(decision.Reason, decision.CorrelationId);

            // 2. the canonical path under the approved root
            var root = _files.ApprovedRoot.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            var path = Path.GetFullPath(Path.Combine(root, entry.Folder, entry.FileName));
            if (!path.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                throw new AccessDeniedException("path", decision.CorrelationId);

            return path;
        }

        private AccessDeniedException Deny(string reason)
        {
            this.CorrelationId = Security.AuditLog.NewCorrelationId();
            return new AccessDeniedException(reason, this.CorrelationId);
        }
    }
}
