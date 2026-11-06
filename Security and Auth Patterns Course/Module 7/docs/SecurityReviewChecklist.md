# Pre-release security review · AccessOps capstone

Walked on 2026-11-06 against the running Module 7 app (`http://localhost:6407` with its test
identity provider on 6417), the test suite (`dotnet test`: 48 passing) and the published output.
"Verified" names how; a check that was only read from configuration says so.

Every finding has a severity, an owner and either a fix or an accepted risk with an expiry.

## Checklist

### Module 1 - trust boundaries

| Check | Result | Evidence |
|---|---|---|
| Every Module 1 boundary has a server-side control | pass | Module 1 `docs/ControlPlacementMap.md`; each control below is a service or the host, not a screen |
| What reaches the browser is decided by a projection | pass | `EmployeeListItem` / `DocumentListItem`; hidden grid columns are serialized (cookbook), so none are used for data |
| Finding F5 | fixed | see below |

### Module 2 - authentication and session identity

| Check | Result | Evidence |
|---|---|---|
| PBKDF2-SHA256 hashing, fixed-time compare, dummy hash for unknown names | pass | `PasswordService`; Module 2 tests |
| Lockout after 5 failures | pass | browser: `LoginThrottle · failures 5 of 5` after five wrong passwords |
| `Application.RefreshSessionId()` after sign-in | pass | Module 2 browser run: session ID changed on sign-in (cookbook) |
| `SessionTimeout` handled (`e.Handled = true`) and `Application.Exit()` on sign-out | pass | Module 2 browser run (61 s idle sign-out), Module 7 sign-out ends both sessions |
| Passkey on HTTPS / AllowedOrigins / durable `IPasskeyCredentialStore` | **not applicable on 4.1.7** | no passkey component in the released package; passkeys are not implemented |

### Module 3 - authorization

| Check | Result | Evidence |
|---|---|---|
| Every service method guarded | pass | `IAuthorize` in `DocumentService`, `ApprovalService`, `AdminService`, `EmployeeDirectoryService`, `PermissionEditorService`; `AuthorizationTests` |
| Department and manager rules below the UI | pass | an ordinary employee asking for another department's document is refused by the service (`DocumentSecurityTests.Forged_download_from_another_department_is_refused_and_nothing_is_streamed`); a manager approving a request outside their reports is refused (browser, Module 6: REQ-4104 → *Refused — audited*) |

### Module 4 - external sign-in (ASP.NET Core OpenID Connect on 4.1.7)

| Check | Result | Evidence |
|---|---|---|
| Client secret out of source | pass | `Oidc:ClientSecret` from user secrets / `Oidc__ClientSecret`; the app refuses to start without it |
| Authority pinned, issuer validated, no multi-tenant "common" endpoint | pass | one `Oidc:Authority`; ID token signature, issuer, audience, expiry and nonce validated by the handler (log line on every sign-in) |
| Registered redirect URI equals the one the app sends | pass | provider config `http://localhost:6407/signin-oidc` = the authorize request's `redirect_uri` |
| Code flow + PKCE, tokens kept on the server | pass | `code_challenge_method=S256` in the authorize URL; `SaveTokens = false` |
| `OAuthClient` items (`WISEJ_OAUTH_<PROVIDER>_CLIENT_SECRET`, `Tenant`/`AllowedTenants`, `ResolvedRedirectUri`) | **not applicable on 4.1.7** | the component does not exist in the released package; the equivalents are the three rows above |
| Finding F1 | fixed | see below |

### Module 5 - data, uploads, downloads, handlers

| Check | Result | Evidence |
|---|---|---|
| Upload limits (`Upload.MaxFileSize` = host body cap) | pass | 21 MB file refused by the control; 21 MB raw POST dropped by the host (`Request body too large`) |
| Downloads only through `DocumentService` | pass | `Application.Download` from the service; no file URL exists |
| Expired / tampered share link refused | pass | `DocumentSecurityTests.A_share_link_is_valid_for_one_hour_and_only_as_issued`; browser: tampered signature and edited expiry 404 |
| Parameterised queries | pass | Module 5 `docs/InjectionAndOutputReview.md` |
| `MessageBox` with `allowHtml: false` | pass (none used) | no `MessageBox.Show`/`AlertBox` in the code; notices are `Label`s (`AllowHtml` off) |
| Finding F2 | fixed | see below |

### Module 6 - auditing, secrets, configuration

| Check | Result | Evidence |
|---|---|---|
| Audit rows for approvals, permission changes, downloads, share links, sign-ins - denials included | pass | admin console audit grid; `AuditTests` |
| Append-only and hash-chained | pass | *Verify chain* → *every entry matches*; `UPDATE`/`DELETE` refused in tests |
| Correlation ID on every command's log lines and audit row | pass | Module 6 `docs/LogRedactionReview.md` |
| No secret in source control | pass | Modules 6-7 hold no secret in any tracked file (the Module 4-5 lab copies carry obviously-demo values in `launchSettings.json`, which Module 6 removes) |
| No secrets or personal data in logs | pass | sink test with a known marker; log search: 0 signatures, 0 JWTs, 0 e-mail addresses |
| Finding F13 | accepted | see below |

### Module 7 - host

| Check | Result | Evidence |
|---|---|---|
| HTTPS redirect + HSTS | pass (in memory) / not applicable on the http lab | `HardeningTests` with `RequireHttps` on; no certificate on this machine |
| `secure: true` in the deployed `Default.json` | pass | publish target; client honours it (navigated to https when set) |
| `validateClient` on | pass | `Application.Configuration.ValidateClient` = true on the documents footer |
| Header set incl. CSP with `frame-ancestors 'none'` on every response | pass | `fetch` of `/Default.html`, the share link and a `429` |
| `.json` not downloadable | pass | `/Default.json`, `/appsettings.json`, `/appsettings.Development.json` → 404 |
| Debug off | pass (configuration only) | `Default.json` has no `debug` key (off by default); `Web.config` has no debug flag; not read back at runtime |
| Cookie options | pass | see `HardeningConfiguration.md` |
| Request limits, rate limits | pass | see `HardeningConfiguration.md` |
| Dated dependency audit | pass | `DependencyAudit.md` |

## Findings

| # | Module | Finding | Severity | Owner | Resolution |
|---|---|---|---|---|---|
| F1 | 4 / 7 | **Logout CSRF.** `GET /account/signout` ended the provider session for any page that made the browser request it. Reproduced: a bare navigation to `/account/signout?reason=signout` signed the user out of Contoso Identity. | Medium | service owner | **Fixed.** The signing-out Wisej.NET session issues a one-time, two-minute ticket (`SignOutTickets`); the start page carries it; the endpoint ends the provider session only for a valid ticket. Re-tested: the bare GET is ignored (`Provider sign-out without a valid ticket ignored`) and the user stays signed in; the real *Sign out* still ends both sessions (the provider asks for the password again). Test `A_provider_sign_out_needs_a_ticket…`. |
| F2 | 5 / 7 | A file over `Upload.MaxFileSize` was refused by the client with **no message and no trace**. | Low | service owner | **Fixed.** `Upload.Error` (`FileTooLarge`) shows the limit and writes a `Refused:Size(client)` audit row. Re-tested with a 21 MB file. |
| F3 | 7 | Responses carried `Server: Kestrel`. | Low | platform team | **Fixed.** `AddServerHeader = false`; header absent. |
| F4 | 7 | The repository `Default.json` has `"secure": false` (the lab is http) and would ship that way. | Medium | service owner | **Fixed.** Publish target writes `"secure": true` and fails the publish if the key is missing; verified in the publish output. |
| F5 | 1 / 7 | Two disabled Module 1 placeholders (*Download contract*, *Approve request*) were still on the Employees screen. | Low | service owner | **Fixed.** Removed; documents and approvals have their own guarded screens. |
| F6 | 4 / 7 | The OIDC hand-off cookie was `SameSite=Lax`. | Low | service owner | **Fixed.** `Strict`; the sign-in round trip still completes. |
| F7 | 7 | `appsettings.Development.json` (with `RequireHttps: false`) was copied to the publish output. | Low | service owner | **Fixed.** `CopyToPublishDirectory = Never`; the publish output holds `appsettings.json` and `Default.json` only. |
| F8 | 7 | The CSP keeps `'unsafe-inline' 'unsafe-eval'` in `script-src`. | Medium | service owner | **Accepted** (the Wisej.NET client requires `'unsafe-eval'`; inline scripts in `Default.html`) until a nonce-based start page replaces the inline scripts. Expiry **2027-05-31**. |
| F9 | 2 / 7 | Login buckets, lockout counters, hand-off handles and sign-out tickets live in process memory. | Medium on a farm / Low on one instance | platform team | **Accepted** for the single-instance deployment; a shared store (distributed cache) is required before a second instance is added. Expiry **2027-03-31** or scale-out, whichever is first. |
| F10 | 6 | *(found during the Module 6 run)* After a grant, the admin grid reset its selection and the next **Revoke** landed on another account. | Medium | service owner | **Fixed** in Module 6 (selection kept across reloads). |
| F11 | 6 | *(Module 6)* The refusal log line printed the user name (`(null)` for work accounts). | Low | service owner | **Fixed** in Module 6 (ids only). |
| F12 | 6 | *(Module 6)* Deciding an already-decided request left no audit row and no log line. | Low | service owner | **Fixed** in Module 6 (`Failed` row + warning under the reference). |
| F13 | 6 / 7 | Password sign-in log lines carry no correlation ID (the audit row does; the sign-in click is not wrapped in a command scope). | Low | service owner | **Accepted** - the audit row is the record of a sign-in. Expiry **2027-01-31**. |
| F14 | 7 | `xunit` 2.9.3 deprecated (Legacy), test SDK/runner outdated. | Low | service owner | **Accepted**, test-only - see `DependencyAudit.md` D2/D3, expiry **2027-03-31**. |

## Sign-off

Every Module 1 boundary has a control enforced by a service or by the host; F1-F7 and F10-F12 are
fixed and re-tested, F8, F9, F13 and F14 are accepted with an owner and an expiry. Items that need
Wisej.NET 4.2 (passkeys, `OAuthClient`) are out of scope for this build.
