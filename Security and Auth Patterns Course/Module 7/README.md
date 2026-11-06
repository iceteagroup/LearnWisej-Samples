# AccessOps · Security & Auth Patterns · Module 7

Local lab build for **Module 7 · Hardening Checklist and Capstone**. The confidential documents
screen from the walkthrough - *AccessOps · Confidential documents*, the grid *Document · Title ·
Department · Class* with `DOC-…` references, the connection banner, and the footer strip with the
session ID's home, the cookie flags and **Share link** - on a host that sets HTTPS + HSTS, the
security headers and a Content-Security-Policy the Wisej.NET client runs under, flags its cookies,
caps request bodies, keeps `.json` from being downloaded, rate limits sign-in and share links, and a
dated dependency audit and a seven-module review with every finding resolved.

> **Wisej.NET version.** These samples target the released **Wisej-4 4.1.7**. The lesson teaches the
> `Passkey` and `OAuthClient` components introduced in Wisej.NET 4.2; on 4.1.7 passkeys are not
> implemented and external sign-in uses ASP.NET Core's OpenID Connect handler. The capstone hardening
> therefore covers the password sign-in (service accounts) and the OpenID Connect sign-in, not
> passkeys - see *Where this sample differs*.

## Run it - two servers

Uses the same user secrets as Module 6 (set them once, see the Module 6 README).

```bash
cd "Security and Auth Patterns Course/Module 7/AccessOps.TestIdentityProvider"
dotnet run --urls http://localhost:6417          # TEST DOUBLE - never deploy

cd "Security and Auth Patterns Course/Module 7/AccessOps"
dotnet run -f net10.0 --urls http://localhost:6407
```

```bash
cd "Security and Auth Patterns Course/Module 7"
dotnet test          # 48 tests, including the hardening middleware run in memory with HTTPS on
dotnet publish AccessOps -c Release -o <a folder outside the repository>   # the published Default.json gets "secure": true
```

The lab stays on `http://localhost`: `appsettings.Development.json` turns off only
`Hardening:RequireHttps`. No certificate is needed or installed; the HTTPS redirect and HSTS are
exercised by `AccessOps.Tests/HardeningTests.cs` against a real host name.

## What to try

| Action | Expected result |
|---|---|
| Open the app, then in the browser's console `fetch('/Default.html', {cache:'no-store'})` and read the headers | `content-security-policy` (with `frame-ancestors 'none'`), `x-content-type-options: nosniff`, `referrer-policy: strict-origin-when-cross-origin`, `x-frame-options: DENY`, `permissions-policy`; no `server` |
| `fetch('/Default.json')`, `/appsettings.json`, `/appsettings.Development.json` | 404 each |
| Sign in with the work account as `ruth.okoye@…` → Documents | Banner *Plain HTTP — development profile · headers set · client validated · …*; footer `session ID: sessionStorage · secure: false · validateClient: true · own cookies: HttpOnly; SameSite=Strict` (over HTTPS: the green banner and `Secure;` in the flags) |
| Drop a 21 MB file on **Upload document** | *… was refused: it is larger than the 20 MB limit. Nothing was uploaded.*; audit `Refused:Size(client)` |
| **Share link**, open it | 200 `application/pdf` with the full header set |
| Request a share link 13 times in a row | ten answers, then `429` (with the headers), each refusal an audit row `RateLimited` |
| Service account sign-in with seven wrong passwords | the same sentence every time; the log shows five `BadCredential`, then `RateLimited` |
| Navigate to `/account/signout?reason=signout` by hand | nothing happens to your sessions (`Provider sign-out without a valid ticket ignored`); **Sign out** in the app still ends both |
| `Hardening:CspMode` = `ReportOnly` | the same policy as `Content-Security-Policy-Report-Only`; violations arrive at `/csp-report` and are logged |

## Lab tasks → where in the code

| Deliverable | Where |
|---|---|
| HTTPS, HSTS and security header configuration | [Infrastructure/Hardening.cs](AccessOps/Infrastructure/Hardening.cs) (`AddAccessOpsHardening`, `UseAccessOpsHardening`, the CSP and `/csp-report`), `Hardening` in [appsettings.json](AccessOps/appsettings.json), the `SecureWisejClientOnPublish` target in [AccessOps.csproj](AccessOps/AccessOps.csproj); [docs/HardeningConfiguration.md](docs/HardeningConfiguration.md) |
| Cookie and session hardening settings | cookie policy in `Hardening.cs`, the `Strict` hand-off cookie in [OpenIdConnectSetup.cs](AccessOps/Infrastructure/OpenIdConnectSetup.cs) / [WisejExternalSignIn.cs](AccessOps/Infrastructure/WisejExternalSignIn.cs), `validateClient` / `sessionTimeout` / `secure` in [Default.json](AccessOps/Default.json), Kestrel limits, `Upload.Error` in [DocumentsView.cs](AccessOps/Pages/DocumentsView.cs) |
| Rate limiting on login and share links | [LoginRateLimiter.cs](AccessOps/Services/LoginRateLimiter.cs) taken in `LoginService.SignIn` before the hash; the `share-link` token-bucket policy and its audited `OnRejected` in `Hardening.cs`, `.RequireRateLimiting(...)` in [Startup.cs](AccessOps/Startup.cs) |
| Dependency audit report | [docs/DependencyAudit.md](docs/DependencyAudit.md) |
| Completed security review checklist with fixes | [docs/SecurityReviewChecklist.md](docs/SecurityReviewChecklist.md); fixes: [SignOutTickets.cs](AccessOps/Infrastructure/SignOutTickets.cs) (logout CSRF), `uploadDocument_Error`, `AddServerHeader = false`, the publish target, the Employees placeholders removed |

## Verified

- `dotnet build`: 0 errors, 0 warnings. `dotnet test`: 48/48 - `HardeningTests` runs the real
  middleware in memory with `RequireHttps` on (308 to https, HSTS `max-age=31536000`, the header set,
  report-only header name, `Secure; HttpOnly; SameSite=Lax` on an ASP.NET Core cookie, 429 after the
  bucket with an audit row, headers on the 429) and checks the login bucket and the sign-out
  tickets.
- Browser pane (`accessops-m7` + temporary `accessops-idp-m7`): every row of *What to try*, first
  with `CspMode = ReportOnly` across sign-in, the provider round trip, dashboard, documents
  (upload, download, share dialog), approvals (approve), employees, admin console and sign-out - no
  violation reported - then with the policy enforced (sign-in, provider round trip, dashboard,
  documents with the upload refusal, employees, admin console, sign-out). No server errors.
- `dotnet publish`: `AccessOps: published Default.json has "secure": true`; with `"secure": true` on
  the http lab the 4.1.7 client switched itself to `https://localhost:6407/` (no certificate, so it
  failed) - the key is honoured. The repository file is back to `false`.
- Dependency audit run 2026-11-06: no vulnerable package (direct or transitive), one deprecated and
  two outdated test-only packages, decisions recorded.

## Answers to the review questions

1. **Share link vs Sign in.** The share link is a real URL - `GET /share/{id}` - so ASP.NET Core's
   rate-limiting middleware sees each request and can partition by address. *Sign in* is a click:
   every Wisej.NET event of every screen arrives at the same application endpoint (`/app.wx` or the
   WebSocket) inside an existing session, so middleware cannot tell a login from a grid scroll. The
   login limit lives in `LoginService`, keyed by user name **and** client address, taken before the
   hash is computed, refused with the same sentence as a wrong password and audited - and, on more
   than one server, held in a store they share.
2. **`script-src 'self'`.** The Wisej.NET client evaluates script it receives (`'unsafe-eval'`) and
   `Default.html` carries inline functions; the client also writes inline styles (`style-src
   'unsafe-inline'`). Under `script-src 'self'` the page loads `wisej.wx` and then stops. In
   report-only mode the same policy is sent as `Content-Security-Policy-Report-Only`: nothing breaks,
   and each refused eval or inline script is posted to `/csp-report` and logged, so the missing
   sources are known before anyone enforces the policy.
3. **HttpOnly and SameSite on the session cookie.** There is no session cookie: by default the
   Wisej.NET session ID is in `sessionStorage` (`Wisej.SessionId`, one per tab; `document.cookie` is
   empty). It is protected by TLS and `"secure": true`, by `validateClient` (a replayed ID from
   another browser gets a new session), by `Application.RefreshSessionId()` after sign-in, and by
   keeping script injection out of the page (text encoded by default, no `AllowHtml` on user text, a
   CSP). Cookie flags apply to the real cookies - here the hand-off cookie and the OIDC correlation
   and nonce cookies.

## Where this sample differs from the lesson

- **Passkey / OAuthClient (Wisej.NET 4.2).** The lesson teaches the `Passkey` and `OAuthClient`
  components introduced in Wisej.NET 4.2; these samples target the released 4.1.7, so passkeys are
  not implemented and external sign-in uses ASP.NET Core's OpenID Connect handler. Lab deliverables
  affected: *Cookie and session hardening settings* (the "keep cookies allowed for the OAuthClient
  Redirect-mode binder cookie" step becomes the OIDC handler's correlation and nonce cookies) and the
  *Completed security review checklist* (the Module 2 passkey items and the Module 4 `OAuthClient`
  items - `WISEJ_OAUTH_<PROVIDER>_CLIENT_SECRET`, `Tenant`/`AllowedTenants`, `ResolvedRedirectUri` -
  are marked not applicable, with the OIDC equivalents checked instead).
- **HTTPS in the lab.** The run is on `http://localhost` without a certificate (none is created or
  trusted). HTTPS redirect and HSTS are on in the production settings and verified in memory; the
  video's green "Encrypted over HTTPS and WSS" banner appears only when the request is HTTPS
  (`Application.IsSecure`).
- **Banner and footer are computed.** They read `Application.IsSecure` and
  `Application.Configuration`, so on the lab they say *Plain HTTP — development profile* and
  `secure: false`; the video shows the deployed values.
- **HSTS value.** The video shows `max-age=31536000; includeSubDomains`; the sample omits
  `includeSubDomains` by default, as the lesson advises, until every subdomain serves TLS
  (`Hardening:HstsIncludeSubDomains`).
- **Review findings.** The video's two findings (a department filter in the grid, a connection
  string in a committed file) do not occur in this build - Modules 3 and 6 put those in the service
  and in user secrets. The checklist records the findings this review actually produced.
- **Dependency audit.** The video shows one high transitive finding; this run found none - the one
  the course hit (`SQLitePCLRaw` 2.1.11 via `Microsoft.Data.Sqlite` 10.0.0) was fixed in Module 1 and
  is recorded as D1.
- **Document ids.** The grid shows `DOC-<id>` from the database (`DOC-4471`, `DOC-9004`, …), not the
  video's `DOC-4417`/`DOC-4392`.
