# AccessOps · Security & Auth Patterns · Module 5

**Federation with OpenID Connect.** The partner route opens on `AccessOps · Partner sign-in` as the
walkthrough draws it: "Sign in to AccessOps", "Partner route · documents, approvals and records for
your tenant", the blue **Sign in with your work account** button (C badge, arrow), "You sign in at
your organisation. AccessOps never sees the password.", the divider **SERVICE ACCOUNTS ONLY** with
the Module 4 password row underneath (account name, password, Sign in) and the bottom strip
`STATUS  Ready … partner route · https`. The button takes the tab to **Contoso Identity · test
provider** (purple bar, "AccessOps is requesting" `openid` `profile` `email`, e-mail and password,
Continue, then "Approve the sign-in on your phone" with Approve / Deny). Back in AccessOps the page
load maps issuer + subject to a local account, creates one grant, rotates the session ID and shows
`AccessOps · Records`: Dana Ellis, "Reader · Tenant A · signed in via Contoso Identity", the grid
Record · Tenant · Class (Record 17 · Supplier contract, Record 22 · Rate card 2026, Record 31 ·
Onboarding checklist), **Export record**, **Sign out**, status "Signed in via Contoso Identity" and
the note `grant G-xxxx… · tenant from AccessOps`. Every refused sign-in shows the orange "Sign-in did
not complete. Try again or contact your administrator."

Solution `AccessOps.slnx` = `AccessOps/` (`net10.0`, `Wisej-4` 4.1.7, `Microsoft.Data.Sqlite` 10.0.14,
`Microsoft.AspNetCore.Authentication.OpenIdConnect` 10.0.14) + `AccessOps.TestIdentityProvider/`
(Contoso Identity, a test double - never deploy) + `AccessOps.Tests/` (xUnit) + `PolicyHarness/`.
Started from a copy of Module 4. Data in `AccessOpsData/` (git-ignored). Module 6 starts from here.

## Run it

Two throwaway secrets, one value: the test provider's client secret and AccessOps' `Oidc:ClientSecret`.
They live in user secrets (Development) or the environment - never in a tracked file.

```powershell
cd "Security and Auth Patterns Course\Module 5"
$secret = "lab-only-" + [guid]::NewGuid().ToString("N")
dotnet user-secrets set "Clients:accessops:Secret" $secret --project AccessOps.TestIdentityProvider
dotnet user-secrets set "Oidc:ClientSecret" $secret --project AccessOps
dotnet build AccessOps.slnx

dotnet run --project AccessOps.TestIdentityProvider --urls http://localhost:6415    # terminal 1
dotnet run --project AccessOps --urls https://localhost:6405                        # terminal 2
dotnet test AccessOps.slnx
dotnet run --project PolicyHarness
```

User secrets load only because `AccessOps.csproj` sets `GenerateAssemblyInfo` to **true** (the
Wisej.NET template's `false` drops the `UserSecretsId` attribute). Without a secret AccessOps refuses
to start ("Oidc:ClientSecret is not configured…"). Instead of user secrets you can set
`Oidc__ClientSecret` and `Clients__accessops__Secret` in the environment.

`Oidc` in `appsettings.json` holds `Authority` (`http://localhost:6415`), `ClientId` (`accessops`),
`CallbackPath`, `Scopes` and `RequireHttpsMetadata: false` - **false only because the lab provider runs
on plain HTTP**; the handler's default (true) stays for any real provider. The provider registration
(`AccessOps.TestIdentityProvider/appsettings.json`) has one exact callback,
`https://localhost:6405/signin-oidc`. Behind a TLS-terminating proxy `app.UseForwardedHeaders()`
(configured for `Hosting:KnownProxies`) keeps the `https` origin, or the handler would build an
`http://` redirect URI the provider refuses.

**Browser that does not trust the dev certificate** (as on the course machine): add the Development
verification listener of Module 4 and register its callback at the provider for that run only:

```powershell
$env:AccessOps__LocalHttpVerification = "true"; $env:AccessOps__AllowedOrigins__1 = "http://localhost:6445"
dotnet run --project AccessOps --urls "https://localhost:6405;http://localhost:6445"
# provider terminal:
$env:Clients__accessops__RedirectUris__1 = "http://localhost:6445/signin-oidc"
$env:Clients__accessops__PostLogoutRedirectUris__1 = "http://localhost:6445/signout-callback-oidc"
dotnet run --project AccessOps.TestIdentityProvider --urls http://localhost:6415
```

### Demo accounts (invented, local only)

| Where | Sign-in | Secret | Maps to |
|---|---|---|---|
| Contoso Identity | `dana.ellis@contoso.example` | `Demo-IdP-2026` | Dana Ellis, tenant A, Reader (`external_logins`) |
| Contoso Identity | `kai.berg@northwind.example` | `Demo-IdP-2026` | Kai Berg, tenant C - **not approved** (F08) |
| Contoso Identity | `jo.newman@contoso.example` | `Demo-IdP-2026` | nobody (no `external_logins` row) |
| AccessOps, Service accounts only | `svc.reports` | `Reports-demo-31` | Reports service, tenant A (Debug builds seed the hash) |
| AccessOps, Service accounts only | `partner.ana@fabrikam.test` | `Fabrikam-demo-17` | interactive account → generic failure |

The provider's tenant `contoso-lab` is disposable: users come from `Data/users.json`, every session,
code, token and signing key lives in memory, a restart wipes it.

## What to try

| Action | Expected result |
|---|---|
| Sign in with your work account → Dana → Continue → Approve | `AccessOps · Records`, Dana Ellis, "Signed in via Contoso Identity"; log `external sign-in completed … grants total=1` and `RefreshSessionId() · rotated=True` |
| Export record | `record-A-17.txt`, status "Record 17 exported", note "repository reads: 1" |
| **F06**: start an attempt in a second client, put its `state` into this tab's authorize URL at the provider, sign in and approve | "Sign-in did not complete…", log `reason=correlation`, grants unchanged |
| **F07**: after a success, open `http://localhost:6415/lab/replay-last-callback` (test-provider helper: the same callback URL again) | the console stays as it was; log `reason=correlation`; still one grant |
| **F08**: sign in as `kai.berg@northwind.example`, Approve | "Sign-in did not complete…", log `reason=unapproved_tenant`, no grant |
| **Provider denial**: sign in, then **Deny** (or "Cancel and return to AccessOps") | "Sign-in did not complete…", log `reason=access_denied`, no grant |
| Service accounts only: `svc.reports` / `Reports-demo-31` | "Reports service · Reader · Tenant A · signed in with a service account" |
| Service accounts only: `partner.ana@fabrikam.test` / `Fabrikam-demo-17` | "That user name and password combination is not valid." (audit `NotAllowedHere`) |
| Two tabs signed in as Dana; tab 1 Sign out | tab 1 goes through the provider's end-session and returns: "You have signed out. Sign in to continue."; provider log `endsession: IdP session … ended` |
| Tab 2: Export record | "Export denied · grant revoked", note "repository reads: 0" |
| Sign in again | Contoso Identity asks for the e-mail and password (the provider session is gone) |

## Lab deliverables → where

| Deliverable (plan) | Where |
|---|---|
| Cookie and OpenID Connect handler configuration in Startup.cs with secrets out of source | `Infrastructure/OpenIdConnectSetup.cs` (`AddAccessOpsOpenIdConnect`: cookie scheme, `AddOpenIdConnect` with `ResponseType` code, `UsePkce`, `SaveTokens = false`, `MapInboundClaims = false`, `RequireHttpsMetadata` from configuration, `ResponseMode` query, Lax correlation/nonce cookies; `MapAccessOpsAccountEndpoints`: `UseAuthentication`, `/account/signin`, `/account/signout`), `Startup.cs` (`UseForwardedHeaders`, both before `UseWisej()`), `appsettings.json` `Oidc`, `AccessOps.csproj` (`GenerateAssemblyInfo` true, `UserSecretsId`) |
| Issuer and subject account mapper creating one grant per sign-in | `Identity/ExternalIdentityMapper.cs` (`external_logins`, approved tenant, roles from AccessOps), `Infrastructure/ExternalHandoffStore.cs` (one-time 2-minute handle), `Infrastructure/WisejExternalSignIn.cs` (`TryCompleteOnPageLoad`: `Application.Cookies`, expire, redeem once, map, one grant, `SignInHandoff.EstablishOnPageLoad`), `Program.cs` (called from `Main` and `ApplicationRefresh`) |
| Local-login fallback for service accounts | `Identity/LoginService.cs` (`MayUsePasswordForm` = service accounts only), `Pages/LoginPage.cs` (Service accounts only row) |
| Five transaction outcomes: one success and four rejections with no grant | `docs/oidc-outcomes.md`, `docs/run-trace.txt`; `AccessOps.Tests/ExternalSignInTests.cs` (forged state, provider denial, mapper refusals, one-time hand-off, challenge parameters) |
| Sanitized sequence diagram and sign-out covering both sessions | `docs/sequence-diagram.md`; sign-out: `Pages/ConsolePage.cs` (`btnSignOut_Click`), `Default.html` (`signout-external` → antiforgery form post to `/account/signout`), `docs/oidc-outcomes.md` |

Lab steps → code: 2 `appsettings.json` + user secrets + provider registration; 3 `AddAccessOpsOpenIdConnect`,
`UseForwardedHeaders`; 4 `MapAccessOpsAccountEndpoints`, `LoginPage.btnWorkAccount_Click` →
`IExternalSignIn.Begin()`; 5 `OpenIdConnectSetup.HandOffToWisej`, `OnRemoteFailure`/`OnAccessDenied`
→ `BackToSignIn`; 6 `WisejExternalSignIn.TryCompleteOnPageLoad`; 7 `LoginService`; 8–9 What to try,
`docs/`.

## Verified here / only in the lab environment

Verified on this machine: build 0 warnings / 0 errors; 51 tests; PolicyHarness; every row of "What to
try" - the success, F06, F07, F08 and the denial both in the Chromium browser (two tabs for the
revoked-grant row) and again in a scripted headless Chrome run on a fresh database whose logs are
`docs/run-trace.txt`; sign-out ending the provider session (the next sign-in asked for credentials);
a log grep for the client secret, `code=`, `state=`, `nonce=`, verifier, token names, the
protected-state prefix and the demo passwords: **0 hits** in both logs.

Not verified here: a real identity provider, TLS on the provider (the lab provider is plain HTTP, hence
`RequireHttpsMetadata: false`), a real TLS-terminating proxy in front of AccessOps, the UI walk on
`https://localhost:6405` (the Chromium browser does not trust the dev certificate; the walk ran on the
HTTP verification listener with its callback registered for that run only).

## Where this sample differs from the lesson or video

- **Lesson snippet vs. lab:** the lesson's `Startup.cs` excerpt uses `Scope.Add("openid"); Scope.Add("profile")`;
  the lab asks for `openid profile email`, which the sample requests (`Oidc:Scopes`).
- **Response mode:** the test provider answers with a query redirect, so `ResponseMode` is `Query`; the
  handler then sends no `response_mode` parameter at all.
- **Handler logging:** `Microsoft.AspNetCore.Authentication` is logged at `Error` only: at `Warning` the
  handler writes the correlation cookie's name (it contains the correlation ID of the attempt). AccessOps
  writes its own allowlisted reason instead.
- **F07 helper:** `/lab/replay-last-callback` is a test-double convenience on the provider, not part of
  OpenID Connect - it replays the browser's last callback URL so the lab can show the refusal.
- **Replayed callback in a signed-in tab:** the console stays (nothing changes); in a signed-out tab the
  sign-in view shows the fixed sentence.
- **Status texts** follow the run scene ("Signed in via Contoso Identity", "Export denied · grant
  revoked", "repository reads: 0"); "Redirecting to Contoso Identity" is shown only for the moment
  before the navigation.
- **Mock's note line:** the video shows `grant g-…7c1`; the app shows the real shortened grant ID
  (`G-xxxx…`).
- **Window:** the app fills the browser; the mock is a fixed-size window. Grid selection is grey (Wisej
  Bootstrap-4 theme).
- **The provider's page** is the test double's own HTML (the video's "Contoso Identity" mock, with the
  Approve/Deny second factor and a "Cancel and return to AccessOps" link); the scopes, colours and texts
  follow the mock.
