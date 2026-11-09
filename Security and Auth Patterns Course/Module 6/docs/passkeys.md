# AccessOps · passkeys: outcomes, races, trace, recovery (Module 6)

## Relying party

| Setting | Lab value | Where |
|---|---|---|
| RP ID | `localhost` (the sign-in host; no scheme, no port) | `appsettings.json` `WebAuthn:RpId` |
| Allowed origins (exact) | `https://localhost:6406` (+ `http://localhost:6446` only for a Development verification run) | `WebAuthn:AllowedOrigins` |
| RP ID hash | SHA-256(`localhost`), compared with `AuthenticatorData.RPIDHash` | `RelyingPartySettings.RpIdHash` |
| Secure context | `https://localhost:6406` (dev certificate) or `http://localhost` (browsers treat localhost as secure) | - |
| User verification | required (`WebAuthn:RequireUserVerification`) | `PasskeyService.CheckAssertion` |
| Recent sign-in for enrollment | 10 minutes (`auth_time` claim) | `WebAuthn:RecentSignInMinutes` |

`WebAuthn.GetAsync` sends no RP ID to the browser, so registration and sign-in run on the same host.

## Five outcomes

"Live" = observed in a headless Chrome with Chrome's **virtual authenticator** (DevTools protocol
`WebAuthn` domain: CTAP2, internal transport, resident key, user verification) against the running
app; the log lines are in `docs/run-trace.txt`. "Test" = `AccessOps.Tests/PasskeyTests.cs`, with
assertions signed by a software ES256/RS256 key and checked by the extension's real `WebAuthn.Validate`.

| # | Outcome | What stopped it | Live | Test | Screen | Grant |
|---|---|---|---|---|---|---|
| 1 | Valid passkey, account A (Dana Ellis) | - (`CommitUse` returned `Updated`) | yes: enrolled with `CreateAsync`, signed out, `GetAsync` → "Signed in as Dana Ellis. Your session was renewed.", status "Passkey verified · grant issued", `reference 0734-bf26`; counter 1→2 | `Outcome1_…` (ES256 and RS256) | Records page | **1 new** |
| 2 | Account A's credential presented for account B (F09) | no stored key of B's user handle validates the signature (`NoMatchingKey`) | no - a genuine authenticator returns its own user handle; needs a tampered response | `Outcome2_…` | fixed sentence | none |
| 3 | Unapproved origin | `ClientData.Origin` not on the exact list (`OriginRejected`) - after a valid signature | yes: app restarted without `http://localhost:6446` in the list, then "Sign in with a passkey" → refused, `reference 5aab-b93e` | `Outcome3_…` | fixed sentence | none (`grants total=3` unchanged) |
| 4 | Replayed response | the challenge was already consumed (`ChallengeRejected`) although `WebAuthn.Validate` still returns true | no - the browser does not hand the signed response to the page twice | `Outcome4_…` | fixed sentence | none |
| 5 | Cancelled prompt | `GetAsync` threw (no authenticator left in the browser), caught; the challenge is burned (`BrowserFailed`) | yes: virtual authenticator removed, then "Sign in with a passkey" → refused, `reference 2626-4bfa` | `Outcome5_…` | fixed sentence (banner + `AlertBox.Show(…, allowHtml: false)`) | none |

The fixed sentence for every refusal: "That passkey could not sign you in. Try again or use another
sign-in method." The reason goes to the audit table and the log, never to the screen. Further checks
covered by tests: RP ID hash, ceremony type, user presence, user verification, tampered signature,
wrong challenge bytes, counter anomaly, enrollment refusals (origin, type, challenge, another
account finishing the ceremony), duplicate credential under a second account.

## Store races (`PasskeyCredentialStore`, tests)

| Race | Result |
|---|---|
| Read the row twice, commit the first read | `Updated` |
| Commit the stale second read | `Conflict` |
| Delete, then commit the read | `NotFound` |
| Row after the delete | still gone (no resurrection) |
| Re-enrolled with the same credential ID, then the stale commit | `NotFound` (new row identity) |
| A sign-in that loses the race to a delete (F10) | refused, no grant |

`CommitUse` is one statement: `UPDATE … SET sign_count, backup_eligible, last_used_at, version = version + 1
WHERE rp_id AND credential_id AND account_id AND row_id AND version` - one row is `Updated`, zero rows
is classified by whether the row identity still exists.

## Redacted trace format

Registration and authentication each write one line; credential ID, user handle and ticket are cut to
their last four characters, the signature, the challenge and the public key never appear. Observed:

```
passkey.create · ticket …3c6c · credential …92I= · handle …fe6f · alg ES256 · type ok · origin ok · stored · reference 1661-b088
passkey.get · ticket …1620 · credential …92I= · handle …fe6f · signature ok · type ok · origin ok · rpIdHash ok · UP=True UV=True (extension property UV=False) · BE=False · counter 1→2 · commit=Updated · grant G-5bb2… · reference 0734-bf26
passkey.signin refused · reason=OriginRejected · ticket …7f93 · handle …fe6f · grants total=3 · reference 5aab-b93e
passkey.signin refused · reason=BrowserFailed · ticket …45af · handle … · grants total=2 · reference 2626-4bfa
```

`PasskeyTests.The_log_carries_no_full_credential_id_user_handle_or_signature` checks the rule.

## Which checks the extension performs, which AccessOps performs

| Check | Who |
|---|---|
| The browser ceremony (`navigator.credentials.create/get`), CBOR decoding of the attestation object, the COSE public key | the extension (`CreateAsync`, `GetAsync`) |
| **The signature** over authenticatorData ‖ SHA-256(clientDataJSON) with the stored public key (ES256, RS256) | the extension (`WebAuthn.Validate`) - the only check it performs |
| One-time challenge, bound to ceremony type, account and 2-minute expiry; compared with `ClientData.Challenge` | AccessOps (`PasskeyChallengeStore`) |
| `ClientData.Type` (`webauthn.create` / `webauthn.get`) | AccessOps |
| `ClientData.Origin` against the exact list | AccessOps |
| `RPIDHash` = SHA-256(RP ID) | AccessOps |
| User presence and user verification (read from the signed flags byte) | AccessOps (`AuthenticatorFlags`) |
| Which credential signed (the response does not say): only the keys stored for the returned user handle under this RP | AccessOps |
| Counter policy (0→0 accepted, otherwise strictly increasing) | AccessOps (`CounterPolicy`) |
| Owner, account state, tenant approval; one grant only after `Updated` | AccessOps |
| Algorithm allowlist at enrollment (ES256, RS256) | AccessOps |
| Attestation | nobody (`AttestationConveyancePreference.None`; the extension has no attestation validation): the verified session is the evidence |

**Extension fact found while building this sample (Wisej-4-WebAuthn 4.1.7):** the embedded
`wisej.ext.WebAuthn.js` reads the user-verified flag from **bit 1** of the flags byte
(`_isBitOn(flags, 1)`); the WebAuthn spec puts UV in **bit 2** (bit 1 is reserved and 0). So
`AuthenticatorData.UserVerified` is false even when the authenticator verified the user - observed live:
`UV=True (extension property UV=False)`. AccessOps reads UP and UV from `AuthenticatorData.Base64`
(the signed bytes) instead; requiring the property would refuse every real passkey.

**Second extension fact (Wisej-4-WebAuthn 4.1.7, decompiled and measured):** `WebAuthn.Validate`
reads an ES256 DER signature by fixed offsets, drops one leading 0x00 from r and from s and
concatenates them. When r or s is a number shorter than 32 bytes the result is not 64 bytes and a
genuine signature fails - 13 of 3,000 signatures that DER-encode r and s in 32/33 bytes, plus every
signature with a 31-byte encoding (roughly 1 in 128 assertions overall). AccessOps re-encodes the same
r and s as 33-byte integers before calling `Validate` (`Identity/Es256Signature.cs`);
`PasskeyTests.Short_ES256_components_validate_after_re_encoding` signs 2,000 assertions: without the
re-encoding some fail, with it none do. Without the fix about one real passkey sign-in in a hundred
would show the refusal sentence for no reason.

## Device-loss recovery plan

1. **Use another way in.** A second enrolled passkey (the security key kept in the office safe), or
   the Module 5 organisation sign-in, or - for a service account - the Module 4 password. A lost
   device never blocks the account, and nothing bypasses a verified sign-in.
2. **Recent verification before re-enrollment.** "Add a passkey" appears only while the current grant
   passes and the sign-in is less than 10 minutes old (`PasskeyService.MayEnroll`); an older session
   signs in again first.
3. **Delete the lost credential** on "My passkeys". The delete is final: a sign-in already in flight
   with that credential gets `NotFound` at the commit and no grant; re-enrolling the same authenticator
   later creates a new row identity.
4. **Every change is written to the account history:** `passkey.enroll`, `passkey.rename`,
   `passkey.delete` and every `passkey.signin` decision go to the append-only `audit` table with the
   opaque subject and a reference support can quote. Support reviews the list with the user; who may
   approve a re-enrollment for someone who has lost every method is the tenant's access administrator,
   through the Module 5 provider's own recovery, never through AccessOps support resetting a passkey.
