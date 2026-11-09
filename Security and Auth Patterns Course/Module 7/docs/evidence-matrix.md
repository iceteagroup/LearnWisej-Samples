# 12-case evidence matrix · AccessOps release review (Module 7)

Run on 2026-11-09 with **Admin console → Release review → Run the 12 cases** on the intranet route
(`http://localhost:6408`, Kestrel Negotiate, signed in as this PC's account mapped to `reader1`),
with the partner route (`https://localhost:6407` + `http://localhost:6447`) and Contoso Identity
(`http://localhost:6417`) running, all on one Windows 11 workgroup PC. The rows below are the
`evidence case=…` log lines of that run (`AccessOps.ReleaseReview`), unchanged except for layout.
Correlation labels come from `Correlation.Begin` (`ILogger.BeginScope`); every audit row the case
caused carries the same label.

Labels: **Live** - decided by the running AccessOps on the shared store, through a real HTTP request
to a running route or a principal produced by that route's real sign-in code; **Harness** - the
policy in-process with a constructed principal or a software authenticator (the PolicyHarness
rules); **Fixture** - needs IIS, the domain, a live provider or a real authenticator, ran here on a
fixture or stand-in; **Simulated** - second node / restart as a recorded exercise.

| # | Case | Route · entry point | Fixture | Test alias | Expected | Actual | Boundary | Outcome | Repo calls | Correlation | Evidence |
|---|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Intranet Reader reads a permitted record | Intranet · `http://localhost:6408` | F01 | reader1 (Windows account mapped by SID) | 401 with WWW-Authenticate Negotiate, then 200; the Reader reads A / 17 | `401 Negotiate -> 200 (provider NTLM, Kestrel); A / 17 read` | Ingress | Allow | 1 | rr-01-618046 | Fixture |
| 2 | Host-authenticated user without Reader | Intranet · principal built in-process | — | visitor1 (host-authenticated, no Reader) | deny at the policy, 0 repository calls | `A / 17 refused: policy:role` | Policy | Deny | 0 | rr-02-01d7d3 | Harness |
| 3 | Tenant A requests tenant B record 17 | Partner · password sign-in (`LoginService`) → `RecordPolicy.Read` | F02 | svc.reports · tenant A | deny at the policy before the repository | `B / 17 refused: policy:tenant` | Policy | Deny | 0 | rr-03-ff56d3 | Live |
| 4 | Scoped read, forced error, token reverts | Intranet · `ScopedDocumentReader` with the WebSocket request's Windows token | F03 | reader1 (request Windows token) | DOC-203 fails inside `RunImpersonated`; after = before | `inside: request token (Impersonation) · FileNotFoundException; after = before (name and level): yes` | OS token | Reverted | 1 (scoped file call) | rr-04-7449b6 | Fixture |
| 5 | Local share works, remote share denied | Intranet · `DocumentCatalog` | F04 | reader1 (request Windows token) | DOC-201 reads inside the scope; DOC-205 on `\\files.contoso.test` refused before any scope | `DOC-201 read inside the scope (18426 bytes); DOC-205 refused before any scope: delegation` | Delegation | Deny | 0 | rr-05-a9f121 | Fixture |
| 6 | Sign-in rotates the session ID | Partner · page load (`SignInHandoff`) | — | the last partner sign-in (Dana Ellis, OpenID Connect) | `session.rotate = rotated` | `rotated at 18:21:49 (oidc sign-in, audit f082e705c325); neither ID is stored` | Session | Rotated | 0 | rr-06-381b69 | Live |
| 7 | Revoked grant from a stale tab | Partner · `ExportService` with the stale principal | F05 | svc.reports · two tabs | deny, 0 repository calls, no export, no new grant | `sign-out revoked 1 grant(s); export refused: policy:revoked; new grants 0; no file` | Policy | Deny | 0 | rr-07-b7fba2 | Live |
| 8 | External return URL and forged post | Partner · `https://localhost:6407` (HTTP) | F11 | a page on https://evil.example | `/`, 400, 403, no state change | `returnUrl -> "/"; forged POST -> 400; foreign-origin upgrade -> 403; new grants 0` | Origin | Deny | 0 | rr-08-… (see note 3) | Live |
| 9 | Provider callback repeated | Partner · hand-off (`WisejExternalSignIn.Complete`) | F07 | Dana Ellis (Contoso Identity fixture) | one grant; the repeat issues none | `first: 1 grant; repeat: replayed_handoff; grants +1` | Hand-off | One grant | 0 | rr-09-839f97 | Fixture |
| 10 | Unapproved tenant or bad state | Partner · `/signin-oidc` (HTTP) + hand-off | F06 F08 | Kai Berg (tenant C) · a forged callback | handler fails (`signin=failed`); tenant C refused; no grant | `forged state -> 302 /Default.html?signin=failed; tenant C identity -> unapproved_tenant; grants +0` | Callback | Deny | 0 | rr-10-c752e7 | Fixture |
| 11 | Passkey replay, wrong owner, deleted | Partner · `PasskeyService.CompleteSignIn` | F09 F10 | Dana Ellis + Jo Newman (software authenticator) | one verified sign-in; replay, wrong owner and deleted credential refused, no commit, no grant | `valid: Accepted; replay: ChallengeRejected; wrong owner: NoMatchingKey; deleted: NoMatchingKey; grants +1 (the valid one only)` | Passkey | Deny | 0 | rr-11-98bcf7 | Harness |
| 12 | Callback after node loss | Partner · two node instances over the shared store | F12 | Dana Ellis · node A lost, node B | consumed callback refused on node B; a parked one resumes once | `consumed on A, retried on B: replayed_handoff; parked on A, resumed on B: 1 grant, again: replayed_handoff; grants +2` | Hand-off | Safe deny | 0 | rr-12-9c1eac | Simulated |

KPIs on screen after the run: **Cases run 12 / 12 · Denied at a boundary 8 · Rerun plans 7 · Stop
conditions hit 0**; status line "12 of 12 cases as expected · 8 denied at a boundary with 0
repository calls · no stop condition".

## Also observed in the browser (not by the review screen)

| Case | What was done | Observed |
|---|---|---|
| 6 | Partner tab: stored the anonymous session ID, signed in with the password form (svc.reports), then put the old ID back into `sessionStorage["Wisej.SessionId"]` and reloaded | sign-in rotated the ID; the old ID was **not** accepted: the tab got a third, new ID and the anonymous sign-in page (the signed-in ID was not reachable with it) |
| 7 | Two partner tabs signed in as Dana (Contoso Identity); Sign out in tab B; **Export record** in tab A | tab A: status "Export denied · grant revoked", note "repository reads: 0"; log `deny boundary=policy … reason=revoked`, `export denied reason=revoked … repositoryReads=+0`, all with `Correlation=click-f79c51` |
| 12 | Partner tab waiting at Contoso Identity's login form; partner process **stopped and started again**; then the form submitted | the callback reached the restarted process, the state and correlation cookie were unprotected with the shared key ring, the hand-off came from the shared store: `ID token validated … parked`, `RefreshSessionId() · rotated=True · method=oidc`, `external sign-in completed … grants total=2` - signed in once |
| 12 (failure) | the same, but the restarted node used its **own** key ring (`--DataProtection:KeyPath=keys-node-b`) | `external sign-in failed at the callback · reason=state`, the fixed sentence "Sign-in did not complete. Try again or contact your administrator.", status "no grant created" - a safe deny (runbook incident) |
| Documents (intranet) | DOC-201 … DOC-205 clicked | the four Module 3 sentences plus DOC-205 "is on a remote share that AccessOps does not open with your Windows account. Nothing was opened." (chip "Second hop") |

## A stop condition, injected

With `--AccessOps:FaultInjection:ReplayableHandoff=true` (Debug + Development only: a hand-off
redeem reads the row without deleting it) the same run ended **10 of 12**: case 9 `first: 1 grant;
repeat: ANOTHER grant; grants +2` and case 12 `consumed on A, retried on B: GRANT; … again: GRANT;
grants +4`, both **Failed** with "STOP CONDITION: A replay accepted as a fresh sign-in"; KPI **Stop
conditions hit 2**; status "2 case(s) did not pass: 9, 12 · stop condition hit"; AlertBox (plain
text) "2 release check(s) did not pass (case 9, 12). The details are in the audit log." The rollback
view then showed **Partners 10%** in red and "Stop condition hit: A replay accepted as a fresh
sign-in." (`docs/runbook.md`).

## Rerun plans (Fixture / Harness / Simulated rows)

| # | Rerun on the real topology | A result that blocks sign-off |
|---|---|---|
| 1 | Publish the intranet route to IIS with `iis/windows-auth.location.xml` on the canonical host; from a domain client sign in as `LAB\reader1`, record `401 Negotiate → 200` in the challenge trace and `klist` showing an `HTTP/<host>` ticket (provider = Kerberos only with that ticket), then open A / 17. | a 200 without a challenge, NTLM where Kerberos is required, or A / 17 not readable |
| 2 | Sign in on the IIS intranet route as a second domain account outside the Reader group (`LAB\visitor1`) and open A / 17. Locally: `--AccessOps:CurrentWindowsUser=visitor1`. | any record content, or a deny with repository calls > 0 |
| 4 | `tools/Setup-Fixtures.ps1 -PoolAccount "IIS APPPOOL\AccessOps Intranet" -LabUser LAB\dana.ortiz`: DOC-201 denied before / allowed inside, DOC-203 `FileNotFoundException` inside, the after row = the pool identity. | an after row that is not the pool identity |
| 5 | On the domain: `setspn -Q cifs/files.contoso.test`, the service ticket and the share ACL for DOC-205, constrained delegation off and on. | the app reaching the remote share with the user's token without an approved delegation decision |
| 9 | Through the real provider: sign in, copy the `/signin-oidc` callback URL, replay it in the same browser; then repeat with two nodes. | a second grant |
| 10 | Start a sign-in in browser A and complete its callback in browser B (F06); sign in as a user of an unapproved tenant (F08). | a principal or a grant |
| 11 | Windows Hello or a security key on `https://localhost:6407` with a trusted certificate (or Chrome's virtual authenticator): enroll, sign in, replay the captured response, present the credential with another account's user handle, delete it on My passkeys and try again. | any accepted replay, wrong-owner or deleted-credential sign-in |
| 12 | Two partner nodes behind the balancer with the shared key ring and store; stop node A at the provider's login form, complete the sign-in on node B, then replay the callback. | two grants, or a resumed sign-in on a node without the shared ring |

## Notes

1. **Live is not live sign-off.** Live rows ran end to end on this PC; the production caution of the
   walkthrough applies: rows 1, 2, 4, 5, 9, 10, 11 and 12 rerun on the real topology before release.
2. Case 1's "Repo calls 1" is the allowed read; case 4's "1" is the one scoped file call (it failed
   inside the scope, as F03 intends). Every deny shows 0.
3. In this run the log lines of case 8 were written after three `await`ed HTTP calls and carried **no**
   logging scope (the first run had `Correlation=rr-08-82fd41`); the audit row of the case still has
   its label because the review passes it explicitly. See the README (facts).
4. All aliases are invented fixtures; no session ID, handle, token, password or SID appears in any row
   or in the audit table (the review's fourth stop condition scans the audit table for every handle,
   credential ID, user handle and grant ID the run handled).
