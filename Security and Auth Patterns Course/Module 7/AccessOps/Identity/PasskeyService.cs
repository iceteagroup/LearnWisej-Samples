using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using AccessOps.Infrastructure;
using AccessOps.Security;
using Microsoft.Extensions.Logging;
using Wisej.Ext.WebAuthn;

namespace AccessOps.Identity
{
    /// <summary>Why a passkey ceremony ended. For the audit and the tests - never shown on screen.</summary>
    public enum PasskeyOutcome
    {
        /// <summary>Every check passed (sign-in: the commit returned Updated and a grant exists).</summary>
        Accepted,
        /// <summary>The challenge was unknown, already used, expired, of the wrong ceremony or account, or its bytes differ.</summary>
        ChallengeRejected,
        /// <summary>No stored key of the returned user handle validates the signature.</summary>
        NoMatchingKey,
        /// <summary>ClientData.Type is not the ceremony's type.</summary>
        WrongType,
        /// <summary>ClientData.Origin is not on the exact allowlist.</summary>
        OriginRejected,
        /// <summary>AuthenticatorData.RPIDHash is not SHA-256 of the RP ID.</summary>
        RpIdMismatch,
        /// <summary>UserPresent is false.</summary>
        UserNotPresent,
        /// <summary>UserVerified is false and the policy requires it.</summary>
        UserNotVerified,
        /// <summary>A non-zero counter that did not increase.</summary>
        CounterAnomaly,
        /// <summary>The conditional commit found a changed row.</summary>
        CommitConflict,
        /// <summary>The conditional commit found the row gone (deleted or re-enrolled).</summary>
        CommitNotFound,
        /// <summary>The owning account may not sign in (disabled, unapproved tenant, service account).</summary>
        AccountRefused,
        /// <summary>The browser call threw: cancelled, timed out, no credential, insecure context.</summary>
        BrowserFailed,
        /// <summary>Enrollment: no current grant, or the sign-in is not recent.</summary>
        NotAllowed,
        /// <summary>Enrollment: an algorithm other than ES256 / RS256.</summary>
        UnsupportedAlgorithm,
        /// <summary>Enrollment: the RP ID + credential ID pair already exists.</summary>
        Duplicate
    }

    /// <summary>The answer of a passkey ceremony.</summary>
    public sealed class PasskeyResult
    {
        /// <summary>The one sentence every refused passkey sign-in shows.</summary>
        public const string SignInRefused = "That passkey could not sign you in. Try again or use another sign-in method.";

        /// <summary>The one sentence every refused enrollment shows.</summary>
        public const string EnrollRefused = "The passkey was not added. Try again.";

        /// <summary>True for <see cref="PasskeyOutcome.Accepted"/>.</summary>
        public bool Succeeded => this.Outcome == PasskeyOutcome.Accepted;

        /// <summary>The real outcome (audit only).</summary>
        public PasskeyOutcome Outcome { get; init; }

        /// <summary>A short reference the screen may show and the audit carries ("7c1e-04b9").</summary>
        public string Reference { get; init; }

        /// <summary>Sign-in: the verified sign-in to park. Enrollment: null.</summary>
        public VerifiedSignIn SignIn { get; init; }

        /// <summary>Enrollment: the stored credential.</summary>
        public PasskeyCredential Credential { get; init; }
    }

    /// <summary>What an enrollment needs to call <c>WebAuthn.CreateAsync</c>.</summary>
    public sealed class PasskeyEnrollment
    {
        /// <summary>The one-time challenge (bound to the account and "webauthn.create").</summary>
        public PasskeyChallenge Challenge { get; init; }

        /// <summary>The relying party.</summary>
        public RelyingParty RelyingParty { get; init; }

        /// <summary>The user entity: the opaque ASCII user handle, never an e-mail address.</summary>
        public PublicKeyCredentialUserEntity User { get; init; }
    }

    /// <summary>
    /// The flags byte of the signed authenticator data, read by AccessOps itself (WebAuthn
    /// spec: bit 0 UP, bit 2 UV, bit 3 BE, bit 4 BS). Fact (Wisej-4-WebAuthn 4.1.7, embedded
    /// wisej.ext.WebAuthn.js): the client sets <c>AuthenticatorData.UserVerified</c> from bit 1 (a
    /// reserved bit, always 0), so the property is false even when the authenticator verified the
    /// user. The raw byte is covered by the signature <c>WebAuthn.Validate</c> checked, so reading it
    /// here is as trustworthy as the signature.
    /// </summary>
    public sealed class AuthenticatorFlags
    {
        /// <summary>UP: the user was present (bit 0).</summary>
        public bool UserPresent { get; init; }

        /// <summary>UV: the user was verified - biometric or PIN (bit 2).</summary>
        public bool UserVerified { get; init; }

        /// <summary>BE: the credential may be backed up / synced (bit 3).</summary>
        public bool BackupEligible { get; init; }

        /// <summary>Reads the flags from standard-base64 authenticator data, or null when it is too short.</summary>
        public static AuthenticatorFlags Read(string authenticatorDataBase64)
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(authenticatorDataBase64 ?? "");
            }
            catch (FormatException)
            {
                return null;
            }
            if (data.Length < 37)
                return null;
            var flags = data[32];
            return new AuthenticatorFlags
            {
                UserPresent = (flags & 0x01) != 0,
                UserVerified = (flags & 0x04) != 0,
                BackupEligible = (flags & 0x08) != 0
            };
        }
    }

    /// <summary>The counter rule, written down.</summary>
    public static class CounterPolicy
    {
        /// <summary>
        /// Many authenticators always report 0, so 0 followed by 0 is accepted. Any other value
        /// must be greater than the stored one; a non-zero counter that does not increase is an
        /// anomaly (a cloned authenticator, or a replay) and is refused.
        /// </summary>
        public static bool Accepts(long stored, long received)
            => (stored == 0 && received == 0) || received > stored;
    }

    /// <summary>
    /// Passkeys for AccessOps with the Wisej-4-WebAuthn 4.1.7 extension. The extension runs the
    /// browser ceremonies (<c>WebAuthn.CreateAsync</c>, <c>WebAuthn.GetAsync</c>) and verifies one
    /// signature (<c>WebAuthn.Validate</c>). Everything else is here: the one-time challenge, the
    /// ceremony type, the exact origin, the RP ID hash, user presence and verification, the owner,
    /// the counter policy and the compare-and-swap commit - and only an <c>Updated</c> commit
    /// creates a grant.
    /// </summary>
    public sealed class PasskeyService
    {
        /// <summary>The ceremony type of a registration.</summary>
        public const string CreateType = "webauthn.create";

        /// <summary>The ceremony type of an authentication.</summary>
        public const string SignInType = "webauthn.get";

        private readonly RelyingPartySettings _rp;
        private readonly PasskeyChallengeStore _challenges;
        private readonly PasskeyCredentialStore _store;
        private readonly AccountStore _accounts;
        private readonly GrantStore _grants;
        private readonly AuditService _audit;
        private readonly IClock _clock;
        private readonly ILogger<PasskeyService> _log;

        /// <summary>Creates the service.</summary>
        public PasskeyService(RelyingPartySettings rp, PasskeyChallengeStore challenges, PasskeyCredentialStore store,
            AccountStore accounts, GrantStore grants, AuditService audit, IClock clock, ILogger<PasskeyService> log)
        {
            _rp = rp;
            _challenges = challenges;
            _store = store;
            _accounts = accounts;
            _grants = grants;
            _audit = audit;
            _clock = clock;
            _log = log;
        }

        /// <summary>The relying party settings in use.</summary>
        public RelyingPartySettings RelyingParty => _rp;

        // ---- sign-in -------------------------------------------------------------------------

        /// <summary>A new one-time challenge for <c>WebAuthn.GetAsync</c>.</summary>
        public PasskeyChallenge BeginSignIn() => _challenges.Issue(SignInType);

        /// <summary>The prompt was cancelled or the browser call failed: burn the challenge, refuse.</summary>
        public PasskeyResult BrowserFailed(PasskeyChallenge challenge, string ceremony)
        {
            _challenges.Discard(challenge?.Id);
            return Refuse(ceremony == CreateType ? "passkey.enroll" : "passkey.signin", PasskeyOutcome.BrowserFailed, challenge?.Id, null, null);
        }

        /// <summary>
        /// Checks the response of <c>WebAuthn.GetAsync</c>. Order: consume the challenge (a failed
        /// attempt burns it too), try only the keys stored for the returned user handle under this RP
        /// and keep the one <c>WebAuthn.Validate</c> accepts, then type, origin, RP ID hash, presence,
        /// verification and counter, then the conditional commit. Only <c>Updated</c> creates the grant.
        /// </summary>
        public PasskeyResult CompleteSignIn(string ticketId, CredentialsResponse r)
        {
            const string action = "passkey.signin";
            var fresh = _challenges.TryConsume(ticketId, SignInType, r?.ClientData?.Challenge);
            if (!fresh)
                return Refuse(action, PasskeyOutcome.ChallengeRejected, ticketId, r, null);

            var credential = _store.ForUserHandle(_rp.RpId, r.UserHandle)
                .FirstOrDefault(c => SignatureValid(c, r));
            if (credential == null)
                return Refuse(action, PasskeyOutcome.NoMatchingKey, ticketId, r, null);

            var outcome = CheckAssertion(r, credential);
            if (outcome != PasskeyOutcome.Accepted)
                return Refuse(action, outcome, ticketId, r, credential);

            var received = (long)(uint)r.SignatureCounter;
            var raw = AuthenticatorFlags.Read(r.AuthenticatorData.Base64);
            var commit = _store.CommitUse(credential, received, r.AuthenticatorData.BackupEligibility || raw.BackupEligible);
            if (commit != CommitOutcome.Updated)
                return Refuse(action, commit == CommitOutcome.Conflict ? PasskeyOutcome.CommitConflict : PasskeyOutcome.CommitNotFound, ticketId, r, credential);

            var account = _accounts.FindById(credential.AccountId);
            if (account == null || account.Disabled || !account.TenantApproved || account.IsService)
                return Refuse(action, PasskeyOutcome.AccountRefused, ticketId, r, credential);

            // the Module 4 hand-off starts here: one grant, then park → reload → rotate → principal
            var grant = _grants.Issue(account.Id, account.Tenant, account.Role);
            var reference = Reference();
            _audit.Write(new AuditEvent(new Subject(account.Id, account.Tenant, grant.Id, new[] { account.Role }).Opaque,
                action, "credential …" + Redact.Tail(credential.CredentialId, 4), "-", "allowed", null, reference));
            _log.LogInformation("passkey.get · ticket …{Ticket} · credential …{Credential} · handle …{Handle} · signature ok · type ok · origin ok · rpIdHash ok · UP={UP} UV={UV} (extension property UV={ExtensionUV}) · BE={BE} · counter {Stored}→{Received} · commit=Updated · grant {Grant} · reference {Reference}",
                Redact.Tail(ticketId, 4), Redact.Tail(credential.CredentialId, 4), Redact.Tail(credential.UserHandle, 4),
                raw.UserPresent, raw.UserVerified, r.AuthenticatorData.UserVerified, raw.BackupEligible, credential.SignCount, received, grant.Display, reference);

            return new PasskeyResult
            {
                Outcome = PasskeyOutcome.Accepted,
                Reference = reference,
                SignIn = new VerifiedSignIn
                {
                    AccountId = account.Id,
                    TenantId = account.Tenant,
                    GrantId = grant.Id,
                    Roles = new[] { account.Role },
                    DisplayName = account.DisplayName,
                    Method = "passkey",
                    VerifiedAt = _clock.Now
                }
            };
        }

        /// <summary>The checks around the signature: type, exact origin, RP ID hash, presence, verification, counter.</summary>
        public PasskeyOutcome CheckAssertion(CredentialsResponse r, PasskeyCredential credential)
        {
            if (!string.Equals(r.ClientData?.Type, SignInType, StringComparison.Ordinal))
                return PasskeyOutcome.WrongType;
            if (r.ClientData.Origin == null || !_rp.AllowedOrigins.Contains(r.ClientData.Origin))
                return PasskeyOutcome.OriginRejected;
            if (r.AuthenticatorData?.RPIDHash == null || !CryptographicOperations.FixedTimeEquals(r.AuthenticatorData.RPIDHash, _rp.RpIdHash))
                return PasskeyOutcome.RpIdMismatch;
            // UP and UV from the signed flags byte (see AuthenticatorFlags for why not the property)
            var flags = AuthenticatorFlags.Read(r.AuthenticatorData.Base64);
            if (flags == null || !flags.UserPresent || !r.AuthenticatorData.UserPresent)
                return PasskeyOutcome.UserNotPresent;
            if (_rp.RequireUserVerification && !flags.UserVerified)
                return PasskeyOutcome.UserNotVerified;
            if (!CounterPolicy.Accepts(credential.SignCount, (uint)r.SignatureCounter))
                return PasskeyOutcome.CounterAnomaly;
            return PasskeyOutcome.Accepted;
        }

        // WebAuthn.Validate checks only the signature; it throws for EdDSA and on malformed input.
        // ES256 signatures are re-encoded first (Es256Signature: the extension's DER reading drops short r / s).
        private static bool SignatureValid(PasskeyCredential credential, CredentialsResponse r)
        {
            try
            {
                var signature = credential.Algorithm == (int)COSEAlgorithmIdentifier.ES256 ? Es256Signature.ForValidate(r.Signature) : r.Signature;
                return WebAuthn.Validate(credential.Key, r.AuthenticatorData?.Base64, r.ClientData?.Base64, signature);
            }
            catch (Exception)
            {
                return false;
            }
        }

        // ---- enrollment ------------------------------------------------------------------------

        /// <summary>
        /// True when <paramref name="principal"/> may add a passkey now: its current grant passes and
        /// its sign-in is recent (the <c>auth_time</c> claim).
        /// </summary>
        public bool MayEnroll(ClaimsPrincipal principal)
        {
            var subject = Subject.From(principal);
            if (subject.IsAnonymous || _grants.Check(subject, out _) != null)
                return false;
            var authTime = principal.FindFirst(ApplicationPrincipal.AuthTimeClaim)?.Value;
            if (!long.TryParse(authTime, NumberStyles.Integer, CultureInfo.InvariantCulture, out var seconds))
                return false;
            return _clock.Now - DateTimeOffset.FromUnixTimeSeconds(seconds) <= _rp.RecentSignIn;
        }

        /// <summary>Starts an enrollment for the signed-in account, or null when it may not enroll.</summary>
        public PasskeyEnrollment BeginEnrollment(ClaimsPrincipal principal)
        {
            if (!MayEnroll(principal))
                return null;
            var subject = Subject.From(principal);
            var account = _accounts.FindById(subject.AccountId);
            var handle = _store.UserHandleFor(account.Id);
            return new PasskeyEnrollment
            {
                Challenge = _challenges.Issue(CreateType, account.Id),
                RelyingParty = new RelyingParty(_rp.RpId, _rp.RpName),
                User = new PublicKeyCredentialUserEntity(handle, account.UserName, account.DisplayName)
            };
        }

        /// <summary>
        /// Checks the response of <c>WebAuthn.CreateAsync</c> (challenge, "webauthn.create", origin,
        /// algorithm) and stores the labelled credential for the account the challenge is bound to.
        /// CreateAsync performs no attestation check: the verified session is the evidence.
        /// </summary>
        public PasskeyResult CompleteEnrollment(ClaimsPrincipal principal, string ticketId, CredentialsResponse r, string label)
        {
            const string action = "passkey.enroll";
            var subject = Subject.From(principal);
            if (subject.IsAnonymous || _grants.Check(subject, out _) != null)
                return Refuse(action, PasskeyOutcome.NotAllowed, ticketId, r, null, subject);
            if (!_challenges.TryConsume(ticketId, CreateType, r?.ClientData?.Challenge, subject.AccountId))
                return Refuse(action, PasskeyOutcome.ChallengeRejected, ticketId, r, null, subject);
            if (!string.Equals(r.ClientData.Type, CreateType, StringComparison.Ordinal))
                return Refuse(action, PasskeyOutcome.WrongType, ticketId, r, null, subject);
            if (r.ClientData.Origin == null || !_rp.AllowedOrigins.Contains(r.ClientData.Origin))
                return Refuse(action, PasskeyOutcome.OriginRejected, ticketId, r, null, subject);

            var key = r.AuthenticatorData?.PublicKey;
            COSEAlgorithmIdentifier algorithm;
            try
            {
                algorithm = key.Algorithm;
            }
            catch (Exception)
            {
                return Refuse(action, PasskeyOutcome.UnsupportedAlgorithm, ticketId, r, null, subject);
            }
            if (algorithm != COSEAlgorithmIdentifier.ES256 && algorithm != COSEAlgorithmIdentifier.RS256)
                return Refuse(action, PasskeyOutcome.UnsupportedAlgorithm, ticketId, r, null, subject);

            var credential = new PasskeyCredential
            {
                RowId = Guid.NewGuid().ToString("N"),
                RpId = _rp.RpId,
                CredentialId = key.CredentialID,
                AccountId = subject.AccountId,
                UserHandle = _store.UserHandleFor(subject.AccountId),
                PublicKeyJson = JSON.Stringify(key),
                Algorithm = (int)algorithm,
                SignCount = (uint)r.SignatureCounter,
                BackupEligible = false,                 // CreateAsync does not fill the flags
                Label = string.IsNullOrWhiteSpace(label) ? "Passkey" : label.Trim()
            };
            if (!_store.Add(credential))
                return Refuse(action, PasskeyOutcome.Duplicate, ticketId, r, null, subject);

            var reference = Reference();
            _audit.Write(new AuditEvent(subject.Opaque, action, "credential …" + Redact.Tail(credential.CredentialId, 4), "-", "allowed", null, reference));
            _log.LogInformation("passkey.create · ticket …{Ticket} · credential …{Credential} · handle …{Handle} · alg {Algorithm} · type ok · origin ok · stored · reference {Reference}",
                Redact.Tail(ticketId, 4), Redact.Tail(credential.CredentialId, 4), Redact.Tail(credential.UserHandle, 4), algorithm, reference);
            return new PasskeyResult { Outcome = PasskeyOutcome.Accepted, Reference = reference, Credential = credential };
        }

        // ---- review ----------------------------------------------------------------------------

        /// <summary>Renames one of the signed-in owner's credentials (owner from the principal) and audits it.</summary>
        public bool Rename(ClaimsPrincipal owner, string rowId, string label)
        {
            var subject = Subject.From(owner);
            var ok = !subject.IsAnonymous && !string.IsNullOrWhiteSpace(label) && _store.Rename(subject.AccountId, rowId, label.Trim());
            _audit.Write(new AuditEvent(subject.Opaque, "passkey.rename", "credential row …" + Redact.Tail(rowId, 4), "-", ok ? "allowed" : "denied", ok ? null : "owner", AuditService.NewCorrelationId()));
            return ok;
        }

        /// <summary>Deletes one of the signed-in owner's credentials (final) and audits it.</summary>
        public bool Delete(ClaimsPrincipal owner, string rowId)
        {
            var subject = Subject.From(owner);
            var ok = !subject.IsAnonymous && _store.Delete(subject.AccountId, rowId);
            _audit.Write(new AuditEvent(subject.Opaque, "passkey.delete", "credential row …" + Redact.Tail(rowId, 4), "-", ok ? "allowed" : "denied", ok ? null : "owner", AuditService.NewCorrelationId()));
            return ok;
        }

        // ---- helpers ---------------------------------------------------------------------------

        private PasskeyResult Refuse(string action, PasskeyOutcome outcome, string ticketId, CredentialsResponse r, PasskeyCredential credential, Subject subject = null)
        {
            var reference = Reference();
            _audit.Write(new AuditEvent(subject?.Opaque ?? "anonymous", action,
                credential == null ? "credential -" : "credential …" + Redact.Tail(credential.CredentialId, 4), "-", "denied", outcome.ToString(), reference));
            _log.LogWarning("{Action} refused · reason={Reason} · ticket …{Ticket} · handle …{Handle} · grants total={Grants} · reference {Reference}",
                action, outcome, Redact.Tail(ticketId, 4), Redact.Tail(r?.UserHandle, 4), _grants.CountAll(), reference);
            return new PasskeyResult { Outcome = outcome, Reference = reference };
        }

        private static string Reference()
        {
            var hex = Convert.ToHexString(RandomNumberGenerator.GetBytes(4)).ToLowerInvariant();
            return hex[..4] + "-" + hex[4..];
        }
    }
}
