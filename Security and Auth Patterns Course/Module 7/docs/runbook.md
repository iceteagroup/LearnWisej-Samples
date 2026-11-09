# Deployment and rollback runbook · AccessOps (Module 7)

Roles (invented): **Release lead** (owns go / no-go), **Platform** (IIS, balancer, key ring),
**Directory team** (accounts, groups, SPNs), **Partner IdP admin**, **App team** (AccessOps),
**Data owner** (grants, records), **Security** (credentials, incident revocations).

## 1. Release steps

| # | Step | Owner | Evidence |
|---|---|---|---|
| 1 | `tools/Publish-Release.ps1 -Output <drop>`; all six checks OK | App team | publish log |
| 2 | Deploy the drop to both IIS applications; set `AccessOps__Route` per application; the same `DataProtection__KeyPath`, `AccessOps__DataRoot` and application name on every node | Platform | `AccessOps route: … key ring: …` startup line on every node |
| 3 | Intranet baseline: a Reader opens A / 17 (case 1); PolicyHarness green | App team | evidence row 1, `docs/policy-harness-output.txt` |
| 4 | Test tenant: run the 12 cases (Release review) - every row as expected, 0 stop conditions | Release lead | `docs/evidence-matrix.md` |
| 5 | Partners 10 % (balancer weight), then all partners - repeat cases 3, 7, 8, 9, 12 after each step and after one node recycle | Release lead | evidence rows |

## 2. Stop conditions (agreed before rollout)

Any one stops promotion and starts the rollback (section 4):

1. A cross-tenant read, or a deny that happens after the repository ran (repo calls > 0 on a deny row).
2. A replayed callback or assertion accepted as a fresh sign-in (case 9 or 12 with more grants than expected, case 11 replay accepted).
3. A deleted credential or a revoked grant that still opens a protected action (case 7, case 11 deleted).
4. Credential material in a log, the audit table, a hidden grid column or `Application.Browser.UserData`.

The review screen evaluates 1-4 after every run (KPI "Stop conditions hit"). Rehearsed with the lab
fault injection (`AccessOps:FaultInjection:ReplayableHandoff=true`): cases 9 and 12 failed with
stop condition 2, KPI 2, and the rollback view showed "Partners 10%" in red.

## 3. Injected failure: node restart during a provider callback, without the shared key ring

**What was injected (2026-11-09, really, on this PC):** a partner sign-in was started (the tab at
Contoso Identity's login form); the partner process was stopped and a replacement "node B" started
with its **own** key ring (`--DataProtection:KeyPath=keys-node-b`) - the classic multi-node mistake.
The form was then submitted.

**What the user saw:** `AccessOps · Partner sign-in`, banner "Sign-in did not complete. Try again or
contact your administrator.", status "Sign-in did not complete · no grant created".

**Incident matrix, from the first failing boundary:**

| First observable failure | Boundary | Check | What it showed here |
|---|---|---|---|
| 401 at ingress | Ingress challenge | challenge trace, IIS status triplet (401.1/401.2), `setspn -Q`, `klist` | not this incident (intranet route unaffected) |
| authenticated, then a business deny | Policy | audit `policy:<reason>`, repository calls = 0 | not this incident |
| local file works, remote share fails | Effective token / delegation | `RunImpersonated` scope, SPN, delegation, ACL | not this incident |
| **callback never completes the sign-in** | **Callback route** | canonical URL, exact redirect URI, forwarded headers, **key ring / application name**, `OnRemoteFailure` reason | `external sign-in failed at the callback · reason=state · grants total=31` (node B could not unprotect the state written with node A's keys); login audit `ExternalRefused … state` |
| passkey rejected after the prompt | Passkey validation | RP ID, origin, challenge, `ClientData.Type`, store | not this incident |

**Recovery:** stop node B, start the node with the shared key ring (`DataProtection:KeyPath=keys`),
the user signs in again.

| | Time (local) |
|---|---|
| node B started with its own key ring | 18:20:50 |
| failed callback (fixed sentence, 0 grants) | ≈ 18:21:15 |
| node B stopped, shared-ring node started | 18:21:25 |
| the user's retry signed in (audit `session.rotate rotated · oidc`) | 18:21:49 |

**Recovery time:** about 35 s from the failed callback to a working sign-in (24 s from the start of
the fix). **Affected user paths:** partner OpenID Connect sign-ins in flight during the window (state
and correlation cookies), host cookies and antiforgery tokens issued with the other ring (rejected,
re-issued on the next sign-in), and every Wisej.NET session on the replaced node (node loss).
**Not affected:** the intranet route, grants and revocations, passkey challenges and credentials,
password sign-ins started after the fix. No grant was issued by the failed callback.

**Control observed working:** the same restart **with** the shared key ring, mid-flow, completed
the sign-in once (`ID token validated … parked`, `rotated=True · method=oidc`, `grants total=2`).

## 4. Rollback (rehearsed)

Roll back code and configuration, **never the grant store**: revocations made since the release
must survive. Rehearsed with **Release review → Rehearse rollback** (2026-11-09, both with and
without a stop condition):

| Step | Action | Owner | Result in the rehearsal |
|---|---|---|---|
| 1 | Stop promotion and page the owners | Release lead | Done (audit `release.stop`) |
| 2 | Turn the partner route off at ingress (`Ingress:PartnerRoute = Off`; lab: `AccessOpsData/ingress.json`, reloaded without a restart; production: the balancer rule or stop the Partner site) | Platform | Done - the partner route answered **503** "The AccessOps partner route is turned off. Staff use the intranet route." after **0.3 s** |
| 3 | Intranet Reader reads a permitted record | App team | Allow (401 Negotiate → 200, A / 17 read) |
| 4 | Repeat case 7: revoked grant, stale tab | App team | Deny · no export (`policy:revoked`, repository calls 0, new grants 0) |
| 5 | Earlier revocations still in place after the rollback | Data owner | Verified (revoked-grant count not lower than before step 2) |
| 6 | Revoke the grant(s) the run's callbacks issued | Security | Revoked |

Restore: **Restore partner route** (or `Ingress:PartnerRoute = On`) - "Partner route on again · it
answers 200". An existing Wisej.NET session of a partner tab survived the off/on switch (the switch
stops requests, it ends nothing) - which is why step 6 and the grant check, not the ingress switch,
are what end access.

## 5. Emergency actions and owners

| Situation | Exact action | Owner |
|---|---|---|
| A partner account is compromised | revoke all grants of the account (`GrantStore.RevokeAllForAccount`, or the account signs out everywhere); disable the user at the provider; delete its passkeys | Security + Partner IdP admin |
| A staff account is compromised | disable it in the directory; revoke its AccessOps grants; remove the SID mapping | Directory team + Security |
| A replay was accepted (stop condition 2) | rollback section 4; revoke the grants issued since the release by callbacks; inspect `login_audit` by correlation | Release lead + Security |
| Key ring exposed | rotate the Data Protection key ring (new key, revoke the old one), which invalidates host cookies and in-flight callbacks; users sign in again | Platform |
| The provider is down | partner sign-in shows the fixed sentence (bounded by the handler's back-channel timeout); service accounts keep the password form; no retry loop | Partner IdP admin |
