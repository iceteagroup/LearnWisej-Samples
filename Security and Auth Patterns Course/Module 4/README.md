# AccessOps · Security & Auth Patterns · Module 4

Local lab build for **Module 4 · OAuth and OpenID Connect Integration**. The sign-in page from the
walkthrough - *Sign in with your work account →*, the *SERVICE ACCOUNTS ONLY* password row, the
`STATUS · authorization code flow · PKCE` strip - the portal it lands on (*SessionContext built*,
*signed in via Contoso Identity*), and a local **test identity provider** so the whole round trip
runs on localhost without an external account or a real client secret.

> **Wisej.NET version - read this first.** The lesson teaches the `Wisej.Web.OAuthClient`
> component (and Module 2 the `Passkey` component), both introduced in **Wisej.NET 4.2**. These
> samples target the released **Wisej-4 4.1.7**, which has neither. External sign-in here uses
> **ASP.NET Core's OpenID Connect handler** (authorization code flow with PKCE) configured in
> `Startup.cs` before `app.UseWisej()`, and the Wisej.NET session maps the validated identity at
> the next page load. The pages call one seam, [`IExternalSignIn`](AccessOps/Services/IExternalSignIn.cs),
> so moving to `OAuthClient` when 4.2 ships replaces one class.

## Run it - two servers

```bash
# 1. the test identity provider (TEST DOUBLE - never deploy), port 6414
cd "Security and Auth Patterns Course/Module 4/AccessOps.TestIdentityProvider"
dotnet run --urls http://localhost:6414

# 2. AccessOps, port 6404
cd "Security and Auth Patterns Course/Module 4/AccessOps"
dotnet run -f net10.0 --urls http://localhost:6404
```

```bash
cd "Security and Auth Patterns Course/Module 4"
dotnet test          # 21 tests
```

Open <http://localhost:6404>. Demo identities at the provider are in
[AccessOps.TestIdentityProvider/Data/users.json](AccessOps.TestIdentityProvider/Data/users.json)
(`dana.ellis@…` - linked to a local Operations manager, `jo.newman@…` - unknown locally, provisioned
on first login, `theo.grant@…` - linked to a disabled employee). The service account `svc.reports`
and its demo password are in [AccessOps/Data/seed-credentials.json](AccessOps/Data/seed-credentials.json).
The client secret both servers share is an obviously-demo value supplied by the environment through
each project's `Properties/launchSettings.json` (`Oidc__ClientSecret`, `Clients__accessops__Secret`).

## The test identity provider

[AccessOps.TestIdentityProvider](AccessOps.TestIdentityProvider/Program.cs) ("Contoso Identity") is a
test double, marked as such in its code, its pages and its log, and it refuses to start outside the
Development environment. It implements: discovery (`/.well-known/openid-configuration`), JWKS (an RSA
key generated at start-up, in memory), `/connect/authorize` (exact redirect-URI match, PKCE S256
mandatory, the sign-in page, a simulated second factor, single sign-on through its own session
cookie), `/connect/token` (one-time 60-second codes, PKCE verification, client authentication, RS256 ID
tokens, refresh-token rotation), `/connect/userinfo`, `/connect/revoke`, `/connect/endsession`, and one
protected API (`/api/hr/profile`) AccessOps calls with the access token.

## What to try

| Action | Expected result |
|---|---|
| **Sign in with your work account**, `dana.ellis@…` | Contoso Identity's page (scopes `openid profile email offline_access hr.read`), *Approve the sign-in on your phone*, then the portal: *Dana Ellis · Department manager · Operations · signed in via Contoso Identity*, status `Signed in via Contoso Identity`, `sub 4f21-9ac0 · 3 documents in scope` |
| **Sign out**, then sign in again | *You have signed out of AccessOps and of Contoso Identity.* - and the provider asks for the password again |
| `jo.newman@…` | Provisioned on first login: `1 of 5 entries`, *SessionContext built · provisioned, no roles* |
| `theo.grant@…` | *Sign-in did not complete. If this keeps happening, contact your administrator.* (audit `ExternalRejected`) |
| Open `/signin-oidc?code=forged&state=forged` | *Sign-in did not complete. Try again.* - the handler refused the state |
| `svc.reports` on the password row | The service-account session (`Signed in with a service account`); `d.ortiz` there gets the generic sentence |

## Lab tasks → where in the code

| Deliverable | Where |
|---|---|
| OpenID Connect configuration (code flow, PKCE, scopes) with the secret out of source | [OpenIdConnectSetup.cs](AccessOps/Infrastructure/OpenIdConnectSetup.cs), `Oidc` in [appsettings.json](AccessOps/appsettings.json), `Oidc__ClientSecret` from the environment |
| Success and failure handling (the `Authenticated` / `AuthenticationFailed` roles) | `OnTokenValidated` / `OnRemoteFailure` in OpenIdConnectSetup; `TryCompleteOnPageLoad` in [WisejExternalSignIn.cs](AccessOps/Infrastructure/WisejExternalSignIn.cs); neutral sentences in [Program.cs](AccessOps/Program.cs) |
| Claim mapping by issuer + subject, first-login provisioning with no rights in the same transaction as the audit row, disabled account refused | [ExternalIdentityMapper.cs](AccessOps/Services/ExternalIdentityMapper.cs), `external_logins` table |
| `SessionContext` built locally, permissions from `PermissionService`, fresh session ID | `LoginService.SignInExternal`, `WisejSession.Establish` (page load → `RefreshSessionId()` first) |
| Downstream API with the access token; refresh and expiry | [HrProfileClient.cs](AccessOps/Services/HrProfileClient.cs), `GetAccessTokenAsync` in WisejExternalSignIn, the expiry sign-out in [DashboardView.cs](AccessOps/Pages/DashboardView.cs) |
| Local login fallback for service accounts only | `IsServiceAccount` check in [LoginService.cs](AccessOps/Services/LoginService.cs), [SignInPage.cs](AccessOps/Pages/SignInPage.cs) |
| Sign-out covering both sessions | [SignOutFlow.cs](AccessOps/Pages/SignOutFlow.cs) → Default.html → `/account/signout` (cookie + OpenID Connect sign-out → provider end-session) |
| Where the secret lives, where tokens are kept, how long, who can revoke | [docs/ExternalSignInNotes.md](docs/ExternalSignInNotes.md) |
| The seam for the 4.2 component | [IExternalSignIn.cs](AccessOps/Services/IExternalSignIn.cs) |

## Verified

Both servers in the browser pane (`accessops-idp-m4` was a temporary launch entry for port 6414):
every row of *What to try*, plus a token refresh (provider access-token lifetime set to 70 s for the
test, then restored to 300) and the refresh-impossible path (provider restarted → *Your sign-in has
expired*). Logs and the audit rows are in [docs/ExternalSignInNotes.md](docs/ExternalSignInNotes.md).
`dotnet test`: 21/21.

## Answers to the review questions

1. **A captured authorization code.** Exchanging it needs the PKCE code verifier, which never left the
   server, and the client secret; the code is also single-use and expires in 60 seconds. With the
   OpenID Connect handler PKCE is on by default (`UsePkce = true`); the test provider makes S256
   mandatory.
2. **Five minutes at the multi-factor prompt and the Wisej.NET session evicted.** The callback is an
   ASP.NET Core endpoint with no Wisej.NET session; it parks the validated identity and sends the
   browser to Default.html. A brand-new session runs `Program.Main`, which redeems the hand-off just
   as `ApplicationRefresh` would have. What must be true: the hand-off is redeemable on any page load
   (both `Main` and `ApplicationRefresh` call `TryCompleteOnPageLoad`), and it outlives the wait (two
   minutes here - longer waits fail safe with *Sign-in did not complete*).
3. **Signed straight back in after Sign out.** The provider's session survived; the sign-out did not go
   to the end-session endpoint. Here that is `Results.SignOut(..., OpenIdConnectDefaults.AuthenticationScheme)`
   in `/account/signout`; with the 4.2 component it is `SignOut(..., signOutAtProvider: true, ...)`.

## Where this sample differs from the lesson

- **`OAuthClient` (Wisej.NET 4.2) is not used.** Not built as taught: the `oauth` component on the
  sign-in page, `Provider`/`Authority`/`ClientId` properties, `ResolvedRedirectUri`, `SignIn()`, the
  `ProviderResolved`/`SigningIn`/`Authenticating`/`Authenticated`/`AuthenticationFailed`/`TokenExpired`/`SignedOut`
  events, `OAuthPrincipal`, `GetAccessToken()`/`RefreshAccessToken()`, `SignOut(revokeTokens, signOutAtProvider, ...)`,
  the `WISEJ_OAUTH_<PROVIDER>_CLIENT_SECRET` / `settings.oauth` / `WISEJ_OAUTH_SECRETS_FILE` secret
  chain and Popup mode. Each lab deliverable is met with ASP.NET Core's OpenID Connect handler and
  the code named above. Affected deliverables: *OAuthClient configuration with secrets kept out of
  source* (met with `Oidc:*` configuration and `Oidc__ClientSecret`), *Authenticated and
  AuthenticationFailed handling* (met with the handler events and the page-load hand-off).
- **No `Application.User`.** The 4.2 component sets `Application.User`; this sample keeps the identity
  in the `SessionContext` only, which is what every AccessOps service reads.
- **Replayed callback → `Expired`.** The lab names the component's `OAuthError.Expired`. Here a
  replayed or forged callback is refused by the handler (state cannot be unprotected / correlation
  cookie gone); the browser check used a forged state. A replayed hand-off handle is refused by
  `ExternalHandoffStore` (unit test).
- **Test identity provider instead of Keycloak/Entra.** The lab suggests a Keycloak realm or an Entra
  tenant; no external account may be used here, so the module ships its own test double.
- **Remember-me removed.** Module 2's *Remember this device* is gone: the Module 4 sign-in page has no
  such control, and a device token would be a second, local way in beside the provider.
- **Portal.** The video's portal lists Finance, Operations and Security documents for an Operations
  manager; the department rule from Module 3 still applies, so Dana Ellis sees the three Operations
  documents.
- **Idle sign-out of an external session** also ends the provider session (both sessions), as the
  explicit sign-out does - otherwise a shared machine would sign the next person in silently.
- **Passkeys (Wisej.NET 4.2).** Not implemented; see Module 2.
