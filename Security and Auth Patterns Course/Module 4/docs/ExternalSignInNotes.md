# External sign-in - where everything lives

The lab's closing note: where the secret lives, where the tokens are kept, how long they live and
who can revoke them. Values below are the lab's; production values are in the right-hand column.

| Item | Lab (this sample) | Production |
|---|---|---|
| Client id | `Oidc:ClientId` in appsettings.json (`accessops`) - not a secret | Same, per environment |
| Client secret | `Oidc:ClientSecret`, supplied by the environment: `Oidc__ClientSecret` in `Properties/launchSettings.json` (an obviously-demo value, Development profile only). Never in appsettings.json or source. Module 6 moves it to user secrets / environment only. | A secret store or environment variable set by the deployment; rotated on a schedule with an overlap window (Module 6) |
| Authority | `Oidc:Authority` = `http://localhost:6414`, the lab's test identity provider (`RequireHttpsMetadata` false in appsettings.Development.json only) | The real provider's https issuer; `RequireHttpsMetadata` true (the default) |
| Registered redirect URI | `http://localhost:6404/signin-oidc` (the handler's callback path) | `https://<host>/signin-oidc`, registered exactly |
| Registered post-logout URI | `http://localhost:6404/signout-callback-oidc` | `https://<host>/signout-callback-oidc` |
| Authorization code | Single use, 60 s, bound to the PKCE verifier (S256) | Provider policy |
| State / nonce | Created and checked by the OpenID Connect handler; correlation and nonce cookies (HttpOnly, SameSite=Lax) | Same; `Secure` once on https |
| ID token | Validated by the handler (signature against the provider's JWKS, issuer, audience, expiry, nonce) and then discarded - not stored | Same |
| Access token | Server memory: `Application.Session["ExternalTokens"]`. 5 minutes. Used for the HR directory API; refreshed in the back channel within 60 s of expiry | Provider policy |
| Refresh token | Server memory, same place. 8 hours, rotated on every use by the test provider. Lost when the session ends | Provider policy; revocable at the provider |
| Hand-off handle | Server memory (`ExternalHandoffStore`), 2 minutes, one redemption; the browser holds only a random handle in an HttpOnly cookie | Same; a shared store when there is more than one server |
| Who can revoke | The provider: ending the IdP session (sign-out) deletes that user's refresh tokens; an administrator can disable the local employee (refused at the next sign-in) | The provider's admin console; the local disable flag |

Nothing token-shaped is ever rendered: no token is put on a control, in the page, in a URL or in
a cookie (the OpenID Connect handler runs with `SaveTokens = false`).

## What the browser check showed (2026-11-06, both servers running)

| Path | Result |
|---|---|
| Dana Ellis signs in | Contoso Identity sign-in page → simulated second factor → back on AccessOps, `Signed in via Contoso Identity`, `sub 4f21-9ac0 · 3 documents in scope`, job title from the HR API (access token) |
| Sign out | AccessOps session ended, IdP log `endsession: IdP session of 4f21-9ac0-dana-ellis ended`, back on *You have signed out of AccessOps and of Contoso Identity.* |
| Sign in again | The provider asks for credentials again (no silent sign-in) |
| Jo Newman (unknown locally) | Provisioned: `1 of 5 entries`, *SessionContext built · provisioned, no roles*, department *Unassigned* |
| Theo Grant (disabled) | *Sign-in did not complete. If this keeps happening, contact your administrator.*; audit `ExternalRejected` |
| Forged callback `/signin-oidc?code=forged&state=forged` | *Sign-in did not complete. Try again.*; log `External sign-in failed: Unable to unprotect the message.State.` |
| `d.ortiz` on the password form | Generic sentence; audit `UnknownUser` (no password is stored for people any more) |
| `svc.reports` on the password form | Signed in, `Signed in with a service account` |
| Token refresh (provider lifetime set to 70 s for the test) | IdP log `token: refresh for 4f21-9ac0-dana-ellis (refresh token rotated)` |
| Refresh impossible (provider restarted, refresh token gone) | Session signed out of both: *Your sign-in has expired. Sign in again to continue.*; audit `TokenExpired` |
