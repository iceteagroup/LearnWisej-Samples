using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace AccessOps.Identity
{
    /// <summary>
    /// The subject of a protected operation, as data: account ID, tenant, grant ID and roles.
    /// Captured on the click with <see cref="From"/> and handed to work that runs outside the
    /// session (a raw <c>Task.Run</c> has no <c>Application.User</c> to read).
    /// A subject is not a permission: <c>DocumentService</c> rechecks the grant every time.
    /// </summary>
    public sealed class Subject
    {
        private static byte[] _opaqueKey = RandomNumberGenerator.GetBytes(32);

        /// <summary>The subject used when no AccessOps principal was established.</summary>
        public static readonly Subject Anonymous = new Subject(null, null, null, new string[0]);

        public Subject(string accountId, string tenant, string grantId, IReadOnlyList<string> roles)
        {
            this.AccountId = accountId;
            this.Tenant = tenant;
            this.GrantId = string.IsNullOrEmpty(grantId) ? null : grantId;
            this.Roles = roles ?? new string[0];
        }

        /// <summary>Immutable account ID, or null for <see cref="Anonymous"/>.</summary>
        public string AccountId { get; }

        /// <summary>The account's tenant at sign-in.</summary>
        public string Tenant { get; }

        /// <summary>The grant the decision must read, or null.</summary>
        public string GrantId { get; }

        /// <summary>Role names at sign-in. Shown to people, never used to decide a read.</summary>
        public IReadOnlyList<string> Roles { get; }

        /// <summary>True when no AccessOps principal was established.</summary>
        public bool IsAnonymous => this.AccountId == null;

        /// <summary>
        /// Opaque subject for audit events: an HMAC of the account ID under a per-process key
        /// (set from the data folder at startup), so audit rows can be correlated without
        /// carrying the account ID, a name, a session ID or a token.
        /// </summary>
        public string Opaque => this.IsAnonymous
            ? "anonymous"
            : "s-" + Convert.ToHexString(HMACSHA256.HashData(_opaqueKey, Encoding.UTF8.GetBytes(this.AccountId)))[..12].ToLowerInvariant();

        /// <summary>The account ID cut to its last four characters ("acct …7f3a").</summary>
        public string Redacted => this.IsAnonymous ? "none" : "acct …" + Redact.Tail(this.AccountId, 4);

        /// <summary>
        /// Reads the subject from a principal established by <see cref="ApplicationPrincipal"/>.
        /// Any other principal (the anonymous Kestrel host principal, a Windows principal from the
        /// host) is <see cref="Anonymous"/> for AccessOps' documents.
        /// </summary>
        public static Subject From(IPrincipal principal)
        {
            if (principal?.Identity is not ClaimsIdentity identity
                || !identity.IsAuthenticated
                || identity.AuthenticationType != ApplicationPrincipal.AuthenticationType)
                return Anonymous;

            var id = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(id))
                return Anonymous;

            return new Subject(
                id,
                identity.FindFirst("tenant")?.Value,
                identity.FindFirst("grant")?.Value,
                identity.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray());
        }

        /// <summary>Sets the audit pseudonym key (called once at startup).</summary>
        internal static void UseOpaqueKey(byte[] key)
        {
            if (key == null || key.Length < 32)
                throw new ArgumentException("The opaque-subject key needs 32 bytes.", nameof(key));
            _opaqueKey = key;
        }
    }

    /// <summary>Shortens identifiers for display and traces.</summary>
    public static class Redact
    {
        /// <summary>The last <paramref name="count"/> characters of <paramref name="value"/>.</summary>
        public static string Tail(string value, int count)
        {
            if (string.IsNullOrEmpty(value))
                return "";
            return value.Length <= count ? value : value[^count..];
        }
    }
}
