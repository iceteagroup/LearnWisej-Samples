# AccessOps · Security & Auth Patterns · Module 4

**Custom Sign In and Session Security.** The partner route opens on the blue
`AccessOps · Partner sign-in` window of the walkthrough: "Sign in to AccessOps", "Partner route ·
documents and approval requests", **User name**, **Password**, **Sign in**, a status banner and
the `SERVER  LoginThrottle · failures N of 5` line at the bottom. Every refusal - wrong password,
unknown name, disabled account, unapproved tenant, lockout - shows the same red sentence "That user
name and password combination is not valid." A good password shows "Signing in…", the tab
reloads, the session ID rotates on that page load and `ConsolePage` replaces the login view:
`AccessOps · Documents` with the side strip **Documents · Approvals · Sign out**, the Documents
heading with **Export** and the grid **ID · Title · Tenant · Class** (17 Supplier contract —
Fabrikam, 21 Approval request AR-204, 24 Partner onboarding checklist, all Tenant A). **Sign out**
revokes the account's login grants, expires the host cookie over HTTPS and ends the session; the
tab comes back with "You have signed out. Sign in to continue." A tab that was open elsewhere still
shows Export, and its next click answers "Your sign-in has ended. Sign in again to continue." with
zero repository reads.

Solution `AccessOps.slnx` = `AccessOps/` (Wisej.NET web app, `net10.0`, `Wisej-4` 4.1.7,
`Microsoft.Data.Sqlite` 10.0.14) + `AccessOps.Tests/` (xUnit, `Microsoft.AspNetCore.TestHost`
10.0.14) + `PolicyHarness/` (plain .NET console). Started from a copy of Module 1; Module 1's
training verifier, admin console and diagnostics view are replaced by the partner sign-in.
Data (SQLite database, audit-pseudonym key) lives in `AccessOpsData/` beside the project, outside
the web root, git-ignored. Module 5 starts from this folder.

## Run it

```powershell
cd "Security and Auth Patterns Course\Module 4"
dotnet build AccessOps.slnx
dotnet run --project AccessOps --urls https://localhost:6404      # HTTPS dev certificate
dotnet test AccessOps.slnx
dotnet run --project PolicyHarness
```

`https://localhost:6404` needs the ASP.NET Core development certificate to be **trusted** by your
browser (`dotnet dev-certs https --trust`, your decision). To check the sample in a browser that
does not trust it (the course machine's browser pane did not), run a second, plain-HTTP listener
in Development only:

```powershell
$env:AccessOps__LocalHttpVerification = "true"
$env:AccessOps__AllowedOrigins__1 = "http://localhost:6444"
dotnet run --project AccessOps --urls "https://localhost:6404;http://localhost:6444"
```

`AccessOps:LocalHttpVerification` lets only the antiforgery cookie follow the request scheme
(ASP.NET Core refuses to issue a Secure-only antiforgery cookie on plain HTTP); the host ticket stays
`Secure` (Chrome accepts Secure cookies on `http://localhost`). The app refuses to start with it
outside Development. Fresh fixtures: stop the app and delete `AccessOpsData/`.

### Demo accounts (invented, local only, Debug builds only)

| User name | Password | Account | Result |
|---|---|---|---|
| `partner.ana@fabrikam.test` | `Fabrikam-demo-17` | Ana Ruiz, tenant A, Reader | signs in |
| `partner.old@fabrikam.test` | `Fabrikam-demo-old` | disabled, tenant A | generic failure |
| `partner.kai@northwind.test` | `Northwind-demo-23` | tenant C (not approved) | generic failure |

Passwords are PBKDF2-HMAC-SHA256 (210,000 iterations, 16-byte salt) hashes derived when a **Debug**
build seeds the database; a Release build seeds the accounts without any password hash, so no
fixture secret ships.

## What to try

Use two tabs (each tab is its own Wisej.NET session; the lab's "two browser profiles" also gives
each its own cookies).

| Action | Expected result |
|---|---|
| `partner.ana@fabrikam.test` + a wrong password | "That user name and password combination is not valid." (red), `LoginThrottle · failures 1 of 5`, password box empty |
| `partner.old@fabrikam.test` + `Fabrikam-demo-old` | the same sentence (audit: `Disabled`) |
| `partner.kai@northwind.test` + `Northwind-demo-23` | the same sentence (audit: `TenantNotApproved`) |
| `partner.ana…` + five wrong passwords, then `Fabrikam-demo-17` | "failures 5 of 5", then the same sentence: locked for 15 minutes (audit: `LockedOut`) |
| `partner.ana…` + `Fabrikam-demo-17` (fresh name or after 15 min) | "Signing in…", reload, `AccessOps · Documents`; log `RefreshSessionId() · rotated=True · isWebSocket=False` |
| Console: Export (row 17 selected) | `record-A-17.txt` downloads, "Record 17 exported · Supplier contract — Fabrikam" (green) |
| Tab 1 and tab 2 signed in as Ana; tab 1: Sign out | tab 1 reloads: "You have signed out. Sign in to continue.", SERVER "grant revoked · context cleared · Application.Exit()" |
| Tab 2 (stale, fixture **F05**): Export | "Your sign-in has ended. Sign in again to continue." (red); log `export denied reason=grant … repositoryReads=+0`; no new grant |
| Tab 2: refresh (F5), Export | console stays (same session), Export denied again, 0 reads |
| Tab 2: `fetch('/auth/status')` in the console | `401` (the host cookie's grant check) |
| `fetch('/auth/signout', {method:'POST'})` without a token (forged post, **F11**) | `400`, nothing changes |
| WebSocket upgrade to `/ws.wx` with `Origin: https://evil.example` (**F11**) | `403` before Wisej.NET; own origin `101` |
| `/auth/continue?returnUrl=//evil.example` (also `/\evil.example`, `https://evil.example/x`) (**F11**) | redirect to `/` |
| `dotnet test` | 36 tests green (`docs/test-output.txt`) |
| `dotnet run --project PolicyHarness` | one allowed export, every deny at zero repository calls (`docs/policy-harness-output.txt`) |

## Lab deliverables → where

| Deliverable (plan) | Where |
|---|---|
| Login view with generic failures, throttling and disabled-account handling | `Pages/LoginPage.cs` (`btnSignIn_Click`), `Identity/LoginService.cs` (`SignInResult.GenericFailure`, dummy credential, tenant/disabled policy), `Identity/LoginThrottle.cs` (5 per name, 20 per address, 15 min), `Identity/PasswordHasher.cs`, `Security/LoginAudit.cs` (`login_audit`, append-only) |
| Principal establishment with session ID rotation | `Identity/SignInHandoff.cs` (`Park` → `Application.Reload()`; `CompleteOnPageLoad`: `RefreshSessionId()` first, grant recheck, `ApplicationPrincipal.Establish`, boolean `rotated` log), `Identity/ApplicationPrincipal.cs`, `Program.cs` (`ApplicationRefresh` swaps in `ConsolePage`) |
| HTTPS cookie endpoint with redacted Set-Cookie attributes | `Infrastructure/HostAuthentication.cs` (cookie scheme Secure/HttpOnly/SameSite=Strict, antiforgery, `/auth/antiforgery`, `/auth/cookie`, `/auth/signout`, `/auth/status`, `OnValidatePrincipal`), `Infrastructure/CookieHandoffStore.cs` (single-use 60 s handle), `Default.html` (`AccessOpsAuth`); captured lines in `docs/run-trace.txt` |
| GrantStore with revocation enforced across open tabs | `Security/GrantStore.cs` (`G-` + 128 random bits, account, tenant, expiry, revoked), `Documents/ExportService.cs` (grant claim → current grant → tenant → record → repository), `Identity/SignOutService.cs`; F05 in `GrantAndExportTests` and the browser walk |
| Auth-state transition chart and forged post, wrong origin and continuation URL tests | `docs/auth-state-chart.md`; `Infrastructure/WebSocketOriginGuard.cs`, `Security/ReturnUrlPolicy.cs` (+ `/auth/continue`); `AccessOps.Tests/HostEndpointTests.cs` |

Lab steps → code: 2 `LoginPage`/`LoginService`/`LoginThrottle`; 3 `GrantStore`/`ExportService`;
4 `SignInHandoff`/`Program.OnPageLoad`; 5 `Startup.cs` + `HostAuthentication`; 6
`WebSocketOriginGuard`, `ReturnUrlPolicy`; 7–8 the What-to-try rows; 9 `docs/auth-state-chart.md`.

## Verified here / only in the lab environment

Verified on this machine (Windows 11, .NET 10.0.303 SDK, Kestrel, browser pane at 1400x900 plus a
headless Chrome for full-size screenshots): build 0 warnings / 0 errors; 36 tests; PolicyHarness;
every row of "What to try" in two tabs on `http://localhost:6444` (the verification listener, see
Run it); the raw WebSocket upgrades against `https://localhost:6404`; the Set-Cookie attributes in
the log; the log grep for passwords (0 hits).

Not verified here: the browser pane does not trust the HTTPS development certificate
(`ERR_CERT_AUTHORITY_INVALID`), so the UI walk ran on the HTTP verification listener; "two browser
profiles" - the pane has one profile, so both tabs shared the cookie jar (the Wisej.NET sessions,
the grants and the deny are per tab and were observed; the host-cookie side was checked per request
and in `HostEndpointTests`). Counters behind a load balancer and a real TLS-terminating proxy
(`UseForwardedHeaders` is configured for loopback only) are lab-environment items.

## Where this sample differs from the lesson or video

- **Window title:** the video's mock reads "AccessOps — Partner sign-in"; the app uses the course's
  `AccessOps · <Area>` form ("AccessOps · Partner sign-in", "AccessOps · Documents").
- **No area strip:** the Module 4 mock has no `Departments · Employees · …` strip; the console uses
  the mock's side strip (Documents · Approvals · Sign out). Approvals is an empty screen (not built
  in this lab).
- **"Checking…" button state:** the mock greys the button while checking; the check runs in one
  click handler, so the app shows "Signing in…" only after it passed.
- **SERVER line:** kept as the mock shows it. It counts failures for the typed name, unknown names
  included, so it never tells whether an account exists; a production login view would not show it.
- **Sign-out scope:** the video says "Grant G-7f3a… revoked by tab 1 sign-out"; here each sign-in has
  its own grant and sign-out revokes every active grant of the account, which is what makes tab 2's
  stale export fail.
- **Grid selection:** Wisej's Bootstrap-4 theme draws the selected row grey, not the mock's light
  blue.
- **Grant IDs** are `G-` + 32 hex characters, shown as `G-xxxx…` (the mock's `G-7f3a…` is an example).
- **Host cookie hand-off:** the lesson says the session "creates" the handle; here the page load
  creates it and the page's own script posts it to `/auth/cookie` with an antiforgery token
  (`Application.Call`, value passed as an argument).
- **Sign-out sequencing:** `await Application.CallAsync("AccessOpsAuth.signOut", "signout")` (the
  POST to `/auth/signout`) and only then `Application.Exit()` - script sent in the same response as
  `Exit()` never runs.
- **ApplicationPrincipal:** as the lesson says, this copy assigns `Application.User` without rotating;
  `SignInHandoff` rotates once on the load, before `Establish` (Module 1's copy rotated inside).
- **Verification listener:** `AccessOps:LocalHttpVerification` is not part of the lesson; it exists
  only so the sample can be checked in a browser that does not trust the dev certificate.
