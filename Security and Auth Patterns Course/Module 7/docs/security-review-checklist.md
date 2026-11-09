# Security review checklist · completed for the AccessOps release (Module 7)

From the course pack's `checklists/security_review_checklists.md`. **[x]** done with the evidence
named; **[~]** done on a fixture or stand-in here, rerun plan in `docs/evidence-matrix.md`;
**[ ]** not possible on this machine, with the reason.

## Before lab delivery

- [x] Pin Wisej and .NET versions and label preview exercises - `AccessOps.csproj` pins Wisej-4 / Wisej-4-WebAuthn 4.1.7 and ASP.NET Core 10.0.14; no 4.2 preview code; every negative case has live or offline evidence (`docs/evidence-matrix.md`).
- [x] Two test users, two tenants with matching record IDs, a clean browser profile, a disposable HTTPS environment - tenants A and B both hold record 17; Dana / Jo / svc.reports (partner), reader1 / visitor1 (intranet); headless Chrome profiles are temporary; `https://localhost:6407` with the dev certificate.
- [ ] Prepare a domain client and server before demonstrating Kerberos - workgroup PC, no domain: NTLM only (case 1 labelled Fixture). ACL fixtures and baseline permissions: `tools/Setup-Fixtures.ps1`, `Module 3/docs/acl-fixtures.md`.
- [ ] LMS import, score commit and resume - not part of the sample (site course).

## IIS and identity

- [~] Windows feature installation, auth mode and scope, challenge order, canonical host, provider - `iis/windows-auth.location.xml` (Anonymous off, Windows on, Negotiate then NTLM); here Kestrel Negotiate: 401 Negotiate → 200, provider NTLM (case 1). IIS is not installed.
- [ ] SPNs, ticket decryption identity, extended protection, proxy connection behaviour - needs the domain and IIS (case 1 / 5 rerun plans).
- [x] Request principal, Wisej principal and effective token verified independently; reversion on error; narrow scope - case 4 (after = before after `FileNotFoundException`), identity trace on Documents, `RunImpersonated` only in `ScopedDocumentReader`.
- [x] Delegation targets approved or a restricted identity - no delegation (`AccessOps:DelegationTargets` empty); DOC-205 refused before any scope (case 5); decision in `docs/route-matrix.md`.

## Application sign in and authorization

- [x] Supported identity system, rate limits, generic failures, recovery - OpenID Connect handler, passkeys via the WebAuthn extension, PBKDF2 password form for service accounts with `LoginThrottle`; one fixed sentence per failure; passkey recovery plan in `Module 6/docs/passkeys.md`.
- [x] Tenant and resource authorization before every sensitive read; jobs, downloads and WebSockets use the same policy - `RecordPolicy.Read` on both routes, Export goes through it, scoped reads use `RecordPolicy.Authorize`; 17 policy tests; cases 2, 3, 7.
- [x] Rotate Wisej session IDs after authentication; absolute and idle expiry; recheck revoked grants - `RefreshSessionId()` on the sign-in page load (case 6, old ID refused in the browser); grant 8 h absolute, `sessionTimeout` 1200 s; grant read per call (case 7).
- [x] Host cookies issued/cleared via HTTPS responses; Secure, HttpOnly, SameSite, path, key ownership - `/auth/cookie`, `/auth/signout`; `AccessOps.Auth` Secure, HttpOnly, SameSite=Strict, path /; one shared key ring + application name.
- [x] Antiforgery, origin checks, local return paths; stale tabs, copied grants, replay, proxy header abuse - case 8 (forged POST 400, foreign-origin upgrade 403, external returnUrl → /), case 7, foreign grant claim test, forwarded headers from known proxies only (defaults cleared).

## Federation and passkeys

- [x] Pinned issuer and redirect URIs; state, code exchange, claims; issuer + subject mapping - `Oidc:Authority`, one exact redirect URI per registration, forged state refused (case 10), `external_logins` mapping, unapproved tenant refused.
- [x] Tokens server-side, correlation material redacted, three logouts observed separately - `SaveTokens = false`; correlation cookie name kept out of the log (`Microsoft.AspNetCore.Authentication` at Error); local, host-cookie and provider sign-out (Module 5; sign-out observed again here).
- [~] RP ID and exact origins; verified bootstrap for enrollment; account recovery - `WebAuthn` section, `MayEnroll` (current grant + recent sign-in); real authenticator not used here (case 11 Harness).
- [x] Replay handling and atomic commits against concurrent deletion; algorithms and authenticator policy - single-use challenges in the shared store, CAS commit, delete race tests; ES256 + RS256; the two 4.1.7 defects worked around.

## Deployment and operations

- [~] Negative matrix after a node recycle; same policy on all nodes, ingress and workers - the partner process was restarted mid-callback (resumed once) and the matrix re-run afterwards; second node simulated (case 12).
- [x] Rollback owner, tested steps, stop conditions, incident revocation actions - `docs/runbook.md`, rehearsed twice (with and without a stop condition).
- [~] Versions, audit retention and key / secret / certificate rotations - versions pinned; key-ring rotation and secret source documented (`docs/production-configuration.md`); retention for the audit table is a production decision (proposed: 400 days, then export) - not enforced by the sample.
- [x] Diagnostics routes and synthetic accounts removed or restricted - release review, software authenticator, demo hashes, display passkeys and fault injection are `#if DEBUG` + Development; the publish step checks the dll; the test IdP is never published.

**Result for the release:** no release-blocking failure in the live and harness evidence; sign-off
for production still needs the Fixture rows (1, 4, 5, 9, 10) and rows 2, 11, 12 rerun on the real
topology.
