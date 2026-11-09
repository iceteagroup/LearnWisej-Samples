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
    /// A subject is not a permission: <c>RecordPolicy</c> rechecks the grant every time.
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
        /// Reads the subject from a principal established by <see cref="ApplicationPrincipal"/> -
        /// on either route: the intranet route establishes it from the Windows account mapped by SID,
        /// the partner route from its own sign-in, OpenID Connect or a passkey. Any other principal
        /// (the anonymous Kestrel host principal, the raw Windows host principal) is
        /// <see cref="Anonymous"/> for AccessOps' documents.
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
                identity.FindFirst(ApplicationPrincipal.TenantClaim)?.Value,
                identity.FindFirst(ApplicationPrincipal.GrantClaim)?.Value,
                identity.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray());
        }

        /// <summary>
        /// The account ID of a Windows account that signed in on the intranet route but is not
        /// mapped to any AccessOps account: "win-" and a hash of the SID, so neither the audit nor
        /// the screen ever carries the SID itself. It has no grant; every decision denies it.
        /// </summary>
        public static string UnmappedWindowsAccountId(string sid)
            => "win-" + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(sid ?? "")))[..12].ToLowerInvariant();

        /// <summary>
        /// An opaque pseudonym for any other identifying value (a typed user name, a client
        /// address) under the same key, prefixed with <paramref name="kind"/> ("u-", "a-"): the
        /// login audit can count attempts per name without storing the name.
        /// </summary>
        public static string OpaqueValue(string kind, string value)
            => kind + Convert.ToHexString(HMACSHA256.HashData(_opaqueKey, Encoding.UTF8.GetBytes(value ?? "")))[..12].ToLowerInvariant();

        /// <summary>Sets the audit pseudonym key (called once at startup).</summary>
        public static void UseOpaqueKey(byte[] key)
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
