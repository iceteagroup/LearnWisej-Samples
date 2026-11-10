# AccessOps · Security & Auth Patterns · Module 6

**Passkeys with the WebAuthn Extension.** The partner sign-in page is the walkthrough's
`AccessOps · Partner sign-in`: "Sign in to AccessOps", "Partner route · approved partner tenants
only", the blue **Sign in with a passkey** button, **Sign in with your organization** (the Module 5
federation) and **Use your AccessOps password** (the Module 4 form, service accounts only), the grey
"NO PASSKEY YET?" panel on the right with the lost-device note, a banner and the status strip
`STATUS … reference xxxx-xxxx`. A passkey sign-in that passes every server-side check shows the
records page with "Signed in as Dana Ellis. Your session was renewed." and "Passkey verified · grant
issued"; every refusal shows "That passkey could not sign you in. Try again or use another sign-in
method." (banner and an `AlertBox` with `allowHtml: false`). **My passkeys** (`AccessOps · My
passkeys`) lists the signed-in user's own passkeys - Label · Added · Last used · Backup eligible -
with **Add a passkey**, **Rename**, **Delete**, "Signed in as …" and the platform-authenticator hint.

Solution `AccessOps.slnx` = `AccessOps/` (`net10.0`, `Wisej-4` 4.1.7, **`Wisej-4-WebAuthn` 4.1.7**,
`Microsoft.AspNetCore.Authentication.OpenIdConnect` 10.0.14, `Microsoft.Data.Sqlite` 10.0.14) +
`AccessOps.TestIdentityProvider/` (Contoso Identity on 6416, test double) + `AccessOps.Tests/` +
`PolicyHarness/`. Started from a copy of Module 5. Data in `AccessOpsData/` (git-ignored).

## Run it

```powershell
cd "Security and Auth Patterns Course\Module 6"
$secret = "lab-only-" + [guid]::NewGuid().ToString("N")
dotnet user-secrets set "Clients:accessops:Secret" $secret --project AccessOps.TestIdentityProvider
dotnet user-secrets set "Oidc:ClientSecret" $secret --project AccessOps
dotnet build AccessOps.slnx
dotnet run --project AccessOps.TestIdentityProvider --urls http://localhost:6416   # terminal 1
dotnet run --project AccessOps --urls https://localhost:6406                       # terminal 2
dotnet test AccessOps.slnx
dotnet run --project PolicyHarness
```

WebAuthn needs a secure context: `https://localhost:6406` (with a trusted dev certificate) or
`http://localhost`. The relying party is in `appsettings.json` → `WebAuthn` (RP ID `localhost`, exact
origins). If your browser does not trust the dev certificate, use the Development verification
listener of Modules 4–5 **and** add its origin to the passkey list for that run only:

```powershell
$env:AccessOps__LocalHttpVerification = "true"; $env:AccessOps__AllowedOrigins__1 = "http://localhost:6446"
$env:WebAuthn__AllowedOrigins__1 = "http://localhost:6446"
dotnet run --project AccessOps --urls "https://localhost:6406;http://localhost:6446"
# provider terminal: Clients__accessops__RedirectUris__1 / PostLogoutRedirectUris__1 = http://localhost:6446/...
```

Without a real authenticator (Windows Hello, Touch ID, a security key) you can drive the ceremonies
with Chrome's **virtual authenticator** (DevTools → More tools → WebAuthn → "Enable virtual
authenticator environment", CTAP2, internal, resident key, user verification) - that is how the
ceremonies were run on the course machine.

### Demo accounts (invented, local only)

A Debug build also prints the first demo sign-in under the sign-in form (`LoginPage.DemoHint`); the literal is compiled out of a Release build.

| Account | How to sign in | Notes |
|---|---|---|
| Dana Ellis - partner account A, tenant A | Contoso Identity `dana.ellis@contoso.example` / `Demo-IdP-2026`, then a passkey | Debug builds seed three **display fixtures** for her ("Work laptop · Windows Hello", "Security key · USB", "Phone · synced passkey"): their private keys were thrown away at seeding, so they can be renamed and deleted but can never sign in |
| Jo Newman - partner account B, tenant A | Contoso Identity `jo.newman@contoso.example` / `Demo-IdP-2026` | account B of the A-for-B test |
| Kai Berg - tenant C (not approved) | Contoso Identity `kai.berg@northwind.example` | refused (Module 5, F08) |
| `svc.reports` | Use your AccessOps password → `Reports-demo-31` | service account (Module 4 form) |

## What to try

| Action | Expected result |
|---|---|
| Open the sign-in page | three buttons, "NO PASSKEY YET?", status "Ready · partner route" |
| Sign in with your organization → Dana → My passkeys | the three display fixtures; the hint says whether the browser offers a built-in authenticator (`WebAuthn.IsUserVerifyingPlatformAuthenticatorAvailableAsync`) |
| Add a passkey → name it → OK → authenticator prompt | new row; banner `Passkey "…" added.`; log `passkey.create … alg ES256 … stored` |
| Select a row → Rename → "Old phone" → OK | banner `Passkey renamed to "Old phone".`; audit `passkey.rename allowed` |
| Select "Old phone" → Delete → Yes | banner `Passkey "Old phone" deleted. It can no longer sign in.`; audit `passkey.delete allowed` |
| Records → Sign out → Sign in with a passkey (outcome 1) | "Signed in as Dana Ellis. Your session was renewed.", "Passkey verified · grant issued"; log `UV=True (extension property UV=False) · counter 1→2 · commit=Updated` and `RefreshSessionId() · rotated=True · method=passkey` |
| Sign in with a passkey with the page's origin removed from `WebAuthn:AllowedOrigins` (outcome 3) | fixed sentence, `reason=OriginRejected`, no grant |
| Sign in with a passkey and cancel the prompt / no authenticator (outcome 5) | fixed sentence (banner + AlertBox), `reason=BrowserFailed`, no grant |
| A's credential for B (outcome 2), replayed response (outcome 4), store races | `dotnet test` - `PasskeyTests` (see `docs/passkeys.md`) |
| An hour-old session → My passkeys | no "Add a passkey"; hint "Sign in again to add a passkey" |

## Lab deliverables → where

| Deliverable (plan) | Where |
|---|---|
| Passkey enrollment and sign-in with the WebAuthn extension | `Pages/CredentialReviewView.cs` (`btnEnrollPasskey_Click`: `WebAuthn.CreateAsync` with ES256 + RS256, `AttestationConveyancePreference.None`), `Pages/LoginPage.cs` (`btnPasskeySignIn_Click`: `WebAuthn.GetAsync(challenge, null, 60000)`), `Identity/PasskeyService.cs`, `Identity/RelyingPartySettings.cs`, `appsettings.json` `WebAuthn` |
| Server-side assertion validation with one-time challenges | `Identity/PasskeyService.cs` (`CompleteSignIn`, `CheckAssertion`, `AuthenticatorFlags`, `CounterPolicy`), `Security/PasskeyChallengeStore.cs` (32 bytes, ceremony, account, 2 min, `TryConsume` once) |
| PasskeyCredentialStore with atomic compare-and-swap commits | `Security/PasskeyCredentialStore.cs` (table `passkey_credentials`, unique RP ID + credential ID, `CommitUse` → Updated / Conflict / NotFound, final deletes), schema in `Infrastructure/AccessOpsDatabase.cs` |
| Five outcomes: one valid sign-in and four rejections, with store race tests | `docs/passkeys.md`, `docs/run-trace.txt`, `AccessOps.Tests/PasskeyTests.cs`, `AccessOps.Tests/SyntheticAuthenticator.cs` |
| Credential review page and account recovery plan | `Pages/CredentialReviewView.cs` (owner from `Application.User`), `docs/passkeys.md` (recovery plan) |

Lab steps → code: 2 `RelyingPartySettings` + `WebAuthn` section; 3 `PasskeyCredentialStore`; 4
`PasskeyChallengeStore`; 5 `CredentialReviewView.btnEnrollPasskey_Click`, `PasskeyService.MayEnroll` /
`CompleteEnrollment`; 6 `LoginPage.btnPasskeySignIn_Click`, `PasskeyService.CompleteSignIn`, then
`SignInHandoff.Park` (reload → `RefreshSessionId()` → `Application.User`); 7 `CredentialReviewView`,
recovery plan; 8–9 `docs/passkeys.md`.

## Verified here / by tests / needs a real authenticator

- **Live, in a headless Chrome with the virtual authenticator** (DevTools protocol, the same browser
  engine as the embedded browser): enrollment through `CreateAsync`, a passkey sign-in through `GetAsync` with
  every server check and an `Updated` commit (outcome 1), the unapproved origin (outcome 3), the
  cancelled/failed prompt (outcome 5), rename and delete on the review page, the platform hint, the
  session-ID rotation after a passkey sign-in, the Module 5 organisation sign-in and both sign-outs.
- **Live, in the Chromium browser:** the sign-in page, the review page with the hint ("This device offers a
  built-in authenticator" - the PC has Windows Hello), the organisation sign-in. The embedded browser was **not**
  used to start a ceremony: it would open the Windows Hello dialog on this PC.
- **By tests only (84 green):** account A's credential for account B (outcome 2), the replayed response
  (outcome 4), the store races, RP ID hash / type / presence / verification / counter refusals,
  enrollment refusals, owner-scoped rename and delete, RS256 keys, the log redaction rule.
- **Needs Windows Hello / a security key / a phone:** a real platform or roaming authenticator, a synced
  passkey with BackupEligibility set, attachment filtering (`AuthenticatorAttachment` serialization is
  still unverified - AccessOps does not rely on it), `https://localhost:6406` in a browser that trusts
  the dev certificate.

## Where this sample differs from the lesson or video

- **Two Wisej-4-WebAuthn 4.1.7 defects (no longer a difference):** this sample found that
  `AuthenticatorData.UserVerified` comes from the wrong flags bit (bit 1 instead of bit 2, so it is
  false for real authenticators) and that `WebAuthn.Validate` rejects genuine ES256 signatures whose r
  or s is shorter than 32 bytes (about 1 in 128). The lesson, lab guide and video were corrected on
  2026-11-10 and now teach the same workarounds this sample uses: `AuthenticatorFlags.Read` on the
  signed `AuthenticatorData.Base64` flags byte for UP / UV, and `Es256Signature.ForValidate` before
  `Validate` (`Identity/Es256Signature.cs`, see `docs/passkeys.md`). Both are to be removed when a
  fixed extension ships: the defects were reported upstream (Standard-Extensions issue #3026) and
  fixed in the extension's source on 2026-11-10, together with a garbled non-ASCII user handle and a
  registration failure on authenticators that append extension outputs; the fix reaches a package
  version later than 4.1.7.
- **"Partner account A":** the video's banner says "Signed in as partner account A"; the sample's
  partner A is Dana Ellis (Module 5's partner), so the banner names her.
- **Banner after a passkey sign-in:** the video shows it on the sign-in page; the Module 4 hand-off
  reloads the tab, so it appears on the records page after the session ID was renewed.
- **"Use your AccessOps password"** opens the Module 4 form, which since Module 5 accepts service
  accounts only (`svc.reports`); the video's wording suggests partners use it too.
- **My passkeys:** a **Records** button (to go back) was added to the mock's toolbar; the last grid
  column (free notes in the video: "this device", "lost on …") is empty - AccessOps stores no notes.
- **Display fixtures:** the review page's three rows are Debug-only fixtures whose private keys do not
  exist (see Demo accounts).
- **ResidentKey "preferred" / attachment Platform** are passed to `CreateAsync` (seven required
  arguments); how the extension serializes the attachment enum is not verified, nothing depends on it.
