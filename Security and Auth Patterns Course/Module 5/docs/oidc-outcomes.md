# AccessOps · five transaction outcomes, sign-out and versions (Module 5)

Observed 2026-11-09 on a fresh database (`AccessOpsData/` deleted before the run), AccessOps on
`http://localhost:6445` (the Development verification listener beside `https://localhost:6405`,
see README) and Contoso Identity on `http://localhost:6415`. The grant count is `GrantStore.CountAll()`
as the server logged it right after each outcome (`docs/run-trace.txt`).

| # | Outcome | Fixture | How it was produced | What stopped it | Screen | Grants after |
|---|---|---|---|---|---|---|
| 1 | Changed state | **F06** | A second client (Node.js, "another browser") started its own attempt at `/account/signin`; its `state` was put into this browser's authorize URL at Contoso Identity; Dana signed in and approved | the handler: the correlation cookie for that state is not in this browser (`reason=correlation`) | "Sign-in did not complete. Try again or contact your administrator." | **0** |
| 2 | Success | - | "Sign in with your work account" → Contoso Identity (Dana Ellis, second factor approved) → `/signin-oidc` → page load | - | `AccessOps · Records`: Dana Ellis, "Reader · Tenant A · signed in via Contoso Identity", status "Signed in via Contoso Identity", note `grant G-xxxx… · tenant from AccessOps` | **1** |
| 3 | Replayed callback | **F07** | The same tab was sent to the exact callback URL again (same code, same state) with the test provider's lab helper `/lab/replay-last-callback` | the handler: the correlation cookie was consumed by the first callback (`reason=correlation`); the code was also already redeemed at the provider; a replayed hand-off handle finds nothing (`ExternalSignInTests.F07_…`) | the signed-in tab keeps its console; nothing changed | **1** |
| 4 | Provider denial | - | After sign-out: Contoso Identity asked for credentials again, Dana typed them, then **Deny** on the second-factor step → `error=access_denied` | `OnAccessDenied` (`reason=access_denied`) | the fixed sentence | **1** |
| 5 | Unapproved tenant | **F08** | Kai Berg (`kai.berg@northwind.example`) signed in at Contoso Identity and approved; the ID token validated | `ExternalIdentityMapper`: the external login maps to an account in tenant C, which AccessOps has not approved (`reason=unapproved_tenant`) | the fixed sentence | **1** |

No refusal creates a grant, writes the callback query, a code, a state, a nonce, a verifier or the
client secret to a log, or shows the reason on screen. The reasons come from a fixed list
(`OpenIdConnectSetup.Classify`, `ExternalIdentityMapper.Map`): `state`, `correlation`,
`access_denied`, `provider_error`, `other`, `replayed_handoff`, `unknown_login`, `disabled`,
`unapproved_tenant`, `service_account`.

## Sign-out: two sessions, verified separately

| Result | Evidence |
|---|---|
| Local: grants revoked, Wisej.NET session ended | `login … outcome=SignedOut … +1 revoked`, `POST /auth/signout` expired the host cookie; a second tab signed in as Dana (browser pane walk) then got "Export denied · grant revoked" with note "repository reads: 0" (`export denied reason=grant … repositoryReads=+0`) |
| Provider: session ended | `POST /account/signout · ending the provider session`, provider log `endsession: IdP session of 4f21-…-dana-ellis ended`, the tab came back to "You have signed out. Sign in to continue."; the **next sign-in asked for the e-mail and password** (headless run step 9 and the pane walk) |
| Refresh tokens | not requested (`offline_access` is not in the scopes), so there was nothing to revoke; the provider's `/connect/revoke` exists for a client that keeps one |

## Fixtures used

F06, F07, F08 (fixture IDs from `course_materials/labs/fixtures.tsv`), the provider denial (the test
provider's Deny button), the success path, the second tab for the revoked grant (F05 carried over
from Module 4), and the service-account fallback (`svc.reports` signs in; `partner.ana@fabrikam.test`
with its right password gets the generic sentence, audit `NotAllowedHere`).

## Versions

| Component | Version |
|---|---|
| .NET SDK / runtime | 10.0.303 / 10.0.11 |
| `Wisej-4` | 4.1.7 |
| `Microsoft.AspNetCore.Authentication.OpenIdConnect` | 10.0.14 (pulls `Microsoft.IdentityModel.Protocols.OpenIdConnect` 8.x; the authorize request reports `x-client-ver=8.19.2.0`) |
| `Microsoft.Data.Sqlite` | 10.0.14 |
| `Microsoft.AspNetCore.TestHost` (tests) | 10.0.14 |
| Test provider | `AccessOps.TestIdentityProvider` in this solution (RS256 ID tokens, PKCE S256 required, query response mode) |
