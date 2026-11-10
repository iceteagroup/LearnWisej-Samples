# AccessOps · Security & Auth Patterns · Module 7

**Production Design and Capstone.** One AccessOps build, started once per entry point. The
intranet route opens on the walkthrough's **`AccessOps · Release review`** (Admin console area of
the area strip): four KPI cards - **Cases run · Denied at a boundary · Rerun plans · Stop conditions
hit** - and the 12-case evidence matrix **# · Case · Route · Fixture · Boundary · Outcome · Repo
calls · Evidence** with the outcome chips (Allow, Deny, Reverted, Rotated, One grant, Safe deny) and
the evidence labels (Live, Harness, Fixture, Simulated). **Run the 12 cases** runs every case against
the real services, the shared store and the running routes; the embedded browserl under the grid shows the
selected case's alias, entry point, expected and actual result, correlation label and rerun plan.
**Rehearse rollback** switches to the rollback view: the stages **Intranet baseline → Test tenant →
Partners 10% → All partners**, the stop-condition banner and the six-step runbook (Step · Rollback
runbook · Owner · Result). The partner route is the Module 6 portal (sign-in, Contoso Identity,
passkeys, Records, My passkeys); both routes end in the same `RecordPolicy.Read`.

Solution `AccessOps.slnx` = `AccessOps/` (`net10.0`, `Wisej-4` 4.1.7, `Wisej-4-WebAuthn` 4.1.7,
`Microsoft.AspNetCore.Authentication.OpenIdConnect` 10.0.14, `Microsoft.AspNetCore.Authentication.Negotiate`
10.0.14, `Microsoft.Data.Sqlite` 10.0.14) + `AccessOps.TestIdentityProvider/` (Contoso Identity on
6417, test double) + `AccessOps.Tests/` (xUnit) + `PolicyHarness/`. Started from a copy of Module 6;
Module 3's intranet route (Negotiate, `HostFaultPage`, SID mapping, `ScopedDocumentReader`,
`IdentityTrace`, `ChallengeTrace`, ACL fixtures, IIS file) was ported in. Data in `AccessOpsData/`
(git-ignored), shared by both processes.

## Run it

```powershell
cd "Security and Auth Patterns Course\Module 7"
$secret = "lab-only-" + [guid]::NewGuid().ToString("N")
dotnet user-secrets set "Clients:accessops:Secret" $secret --project AccessOps.TestIdentityProvider
dotnet user-secrets set "Oidc:ClientSecret" $secret --project AccessOps
powershell -ExecutionPolicy Bypass -File tools\Setup-Fixtures.ps1     # approved root + ACLs for the scoped reads
dotnet build AccessOps.slnx
dotnet run --project AccessOps.TestIdentityProvider -- --urls http://localhost:6417 --Clients:accessops:RedirectUris:1=http://localhost:6447/signin-oidc --Clients:accessops:PostLogoutRedirectUris:1=http://localhost:6447/signout-callback-oidc
dotnet run --no-build --launch-profile "AccessOps (partner)" --project AccessOps -- --urls "https://localhost:6407;http://localhost:6447" --AccessOps:Route=Partner --AccessOps:LocalHttpVerification=true --AccessOps:AllowedOrigins:1=http://localhost:6447 --WebAuthn:AllowedOrigins:1=http://localhost:6447
dotnet run --no-build --launch-profile "AccessOps (intranet)" --project AccessOps -- --urls http://localhost:6408 --AccessOps:Route=Intranet
dotnet test AccessOps.slnx
dotnet run --project PolicyHarness
```

Three terminals (IdP, partner, intranet). Build once, then start both routes with `--no-build`:
they are the same assembly. Open the intranet route at `http://localhost:6408` in Edge or Chrome on
the same PC (silent Negotiate for localhost) and the partner route at `https://localhost:6407`
(trusted dev certificate) or, in a browser that does not trust it, at `http://localhost:6447` - the
Development-only listener of Modules 4-6 (`AccessOps:LocalHttpVerification`, refused outside
Development; it also needs the two extra origins and the IdP's extra redirect URIs above). Without
`--AccessOps:Route` the app refuses to start.

Release publish (one drop for both IIS applications, `"secure": true` written into the published
`Default.json`, six checks): `powershell -ExecutionPolicy Bypass -File tools\Publish-Release.ps1 -Output C:\Temp\accessops-release`.

### Demo accounts (invented, local only)

| Account | Route | How | Notes |
|---|---|---|---|
| your Windows account → `reader1` | intranet | Negotiate (silent on localhost) | Reader, tenant A; `--AccessOps:CurrentWindowsUser=visitor1` maps it to `visitor1` (no read right) |
| Dana Ellis | partner | Contoso Identity `dana.ellis@contoso.example` / `Demo-IdP-2026` (then Approve) | Reader, tenant A |
| Jo Newman | partner | Contoso Identity `jo.newman@contoso.example` / `Demo-IdP-2026` | Reader, tenant A (account B of the passkey cases) |
| Kai Berg | partner | Contoso Identity `kai.berg@northwind.example` | tenant C, not approved: refused |
| `svc.reports` | partner | Use your AccessOps password → `Reports-demo-31` | service account; Debug builds only |

**Fixture set (reconciled):** the record titles differ from module to module (Module 1: 17 "Supplier
contract — Northwind renewal", 18, 21, 24; Module 2/3: 17 "Supplier contract renewal", 18 "Payroll
adjustments Q3", …; Module 4: 17 "Supplier contract — Fabrikam", 21, 24). The cumulative portal keeps
**Module 6's records** - the newest partner chain, which the partner screens show: tenant A 17
"Supplier contract", 22 "Rate card 2026", 31 "Onboarding checklist"; tenant B 17 "Supplier contract —
Tailspin logistics", 19 "Warehouse safety audit" - so tenants A and B both hold record 17. The
intranet accounts keep **Module 2's names** (`reader1`, `visitor1`) beside the partner accounts of
Modules 4-6 (one `accounts` table, `kind = windows | interactive | service`).
Confidential files: Module 3's DOC-201 … DOC-204 plus **DOC-205** "Finance share export (F04)" on
`\\files.contoso.test\finance` (the second hop). Reserved fixtures F01-F12 (`labs/fixtures.tsv`) are
the Fixture column of the matrix.

## What to try

| Action | Expected result |
|---|---|
| Open `http://localhost:6408` | `AccessOps · Release review`, Admin console highlighted, KPIs `0 / 12 · 0 · 0 · 0`, the 12 case rows, status "Ready · 12 cases, run them before promotion" |
| Sign in once on the partner route (Contoso Identity), then **Run the 12 cases** | rows as in `docs/evidence-matrix.md`: 1 Ingress Allow 1 Fixture · 2 Policy Deny 0 Harness · 3 Policy Deny 0 Live · 4 OS token Reverted 1 Fixture · 5 Delegation Deny 0 Fixture · 6 Session Rotated 0 Live · 7 Policy Deny 0 Live · 8 Origin Deny 0 Live · 9 Hand-off One grant 0 Fixture · 10 Callback Deny 0 Fixture · 11 Passkey Deny 0 Harness · 12 Hand-off Safe deny 0 Simulated; KPIs `12 / 12 · 8 · 7 · 0`; status "12 of 12 cases as expected · 8 denied at a boundary with 0 repository calls · no stop condition" |
| Click a row | the detail panel: alias, entry point, correlation `rr-NN-xxxxxx`, Expected, Actual, rerun plan |
| **Rehearse rollback** | stages, amber banner "Rollback drill: no stop condition hit…", steps 1-6: Done · Done · Allow · Deny · no export · Verified · Revoked; status "Rollback rehearsed · partner route off after 0.3 s · Reader still reads · stale tab still denied"; the partner route answers 503 "The AccessOps partner route is turned off. Staff use the intranet route." |
| **Restore partner route** | "Partner route on again · it answers 200" |
| Intranet started with `--AccessOps:FaultInjection:ReplayableHandoff=true`, then Run | cases 9 and 12 **Failed**, "STOP CONDITION: A replay accepted as a fresh sign-in", KPI stop conditions 2, status "2 case(s) did not pass: 9, 12 · stop condition hit", AlertBox (plain text) "2 release check(s) did not pass (case 9, 12). The details are in the audit log."; Rehearse rollback: "Partners 10%" red, "Stop condition hit: A replay accepted as a fresh sign-in." |
| Run with no partner sign-in in the last 12 hours | case 6 Failed ("No partner sign-in in the last 12 hours…") - by design, not observed here (a sign-in had happened) |
| Intranet → Documents → DOC-201 … DOC-205 | Module 3's four sentences and the identity trace; DOC-205 "is on a remote share that AccessOps does not open with your Windows account. Nothing was opened." (chip "Second hop") |
| Partner: two tabs signed in as Dana, Sign out in one, **Export record** in the other | "Export denied · grant revoked", "repository reads: 0" |
| Partner: stop the partner process while the tab is at Contoso Identity's login form, start it again, submit | the callback resumes on the restarted process and signs in once (shared key ring + shared hand-off store) |
| …the same with `--DataProtection:KeyPath=keys-node-b` on the restarted process | "Sign-in did not complete. Try again or contact your administrator.", log `reason=state`, no grant (`docs/runbook.md`) |
| `dotnet test` / `dotnet run --project PolicyHarness` | 101 green (`docs/test-output.txt`) / every case PASS (`docs/policy-harness-output.txt`) |

## Lab deliverables → where

| Deliverable (plan) | Where |
|---|---|
| Trust-boundary diagram and route matrix | `docs/trust-boundaries.md` (Mermaid + identity table: origin, trust, expiry, revoker), `docs/route-matrix.md` (framework target, IIS mode, challenge, host principal, app principal, execution identity, revocation owner per route; role and tenant map) |
| Cumulative AccessOps portal with sanitized configuration, role and tenant map and policy tests | the solution; `Startup.cs` (route switch, forwarded headers, shared key ring, `.json` excluded), `Security/RecordPolicy.cs`, `Security/AccessGrants.cs`, `Infrastructure/HandoffTable.cs`, `Infrastructure/IngressSwitch.cs`, `tools/Publish-Release.ps1`, `docs/production-configuration.md`, `AccessOps.Tests/RecordPolicyTests.cs` |
| 12-case evidence matrix with denying boundary and fixture labels | `Pages/ReleaseReviewView.cs`, `Review/ReleaseReviewRunner.cs`, `Review/EvidenceCase.cs`; `docs/evidence-matrix.md` (the run, rerun plans) |
| Deployment and rollback runbook with owners and stop conditions | `docs/runbook.md`, `Review/RollbackRehearsal.cs` |
| Completed security review checklist and oral review notes scored against the rubric | `docs/security-review-checklist.md`, `docs/oral-review-notes.md` |

Lab steps → code: 0 copy of Module 6 + Module 3 route, PolicyHarness, tenants A/B record 17, F01-F12;
2 `docs/trust-boundaries.md`, `docs/route-matrix.md`; 3 `RecordPolicy.Read` (grant from `GrantStore`,
tenant, record, `AuditService.Deny` before the repository) + `RecordPolicyTests` (allow, tenant,
missing role, expired, revoked); 4 `docs/production-configuration.md`, `Startup.cs`, `Publish-Release.ps1`;
5 `ReleaseReviewView.btnRun_Click` → `ReleaseReviewRunner` (alias, entry point, fixture, expected,
actual, boundary, repository calls, `Correlation.Begin` = `ILogger.BeginScope`, label + rerun plan);
6 injected failure → `docs/runbook.md` §3; 7 `btnRollback_Click` → `RollbackRehearsal`; 8 outcome
chips, failed checks as `AlertBox.Show(…, allowHtml: false)`, details in the log and audit only;
9 checklist and oral notes.

## Verified here / only in the lab environment

Verified on this machine (Windows 11 Pro, `WORKGROUP`, one local account, .NET 10.0.11, Kestrel,
Chromium browser 1400x900 + headless Chrome, 2026-11-09): `dotnet build` 0 warnings / 0 errors; 101 tests
green; PolicyHarness green; both routes opened in the Chromium browser (intranet via Negotiate on 6408,
partner via the 6447 listener); the 12 cases run through the review screen as listed in
`docs/evidence-matrix.md` (12/12, then 10/12 with the injected replay defect); the rollback rehearsed
(503 after 0.3 s, Reader allowed, stale tab denied, revocations kept) and restored; the partner process
restarted mid-callback (resumed once) and replaced by a node with its own key ring (safe failure,
recovered in about 35 s); the old session ID refused after a sign-in; the stale-tab export denied in
the browser; the Documents area (DOC-201 … DOC-205); `/Default.json`, `/appsettings.json`,
`/ingress.json` → 404; the Release publish checks (no review, no demo password, no fault injection in
the dll, `"secure": true` in the published Default.json).

Only in the lab environment (Fixture / Harness / Simulated rows, rerun plans in the matrix): IIS
Windows Authentication and the 500.19 lock, Kerberos and the SPN, the ACL flip between the pool and a
different user, the second hop to a real share, a real authenticator (no passkey ceremony was started
from the embedded browser - it would open Windows Hello on this PC), a second physical node behind a balancer with
sticky sessions and WebSocket (TCP) mode.

## Where this sample differs from the lesson or video

- **Navigation:** the video window has a left menu (Dashboard · Departments · Employees · Documents ·
  Approvals · Admin · Release review); the sample keeps the course's area strip
  (`Departments · Employees · Documents · Approvals · Admin console`) with the review in Admin console.
- **Evidence labels:** the video labels case 11 (passkeys) **Live**; here it is **Harness** - the
  assertions are signed by a software authenticator, not a real one. Hence **Rerun plans 7** (video 6).
- **Live, defined:** cases 3 and 7 use a principal produced by the partner route's real password
  sign-in (`LoginService`) inside the running app, not a browser tab; the browser versions of case 7
  and of the session rotation (case 6, with the old ID refused) were also run by hand (matrix "Also
  observed").
- **Case 12** is a recorded exercise: two store instances over the shared SQLite file stand for two
  nodes; the restart was additionally done for real with the partner process.
- **Video stop condition** ("a replayed callback on node B was accepted as a fresh sign-in") only
  appears with the lab fault injection; the normal run hits none and the rollback is a drill (amber banner).
- **Buttons:** the mock shows no buttons; the sample adds Run the 12 cases, Rehearse rollback,
  Restore partner route and Evidence matrix, and a detail panel for the selected row.
- **Progress:** the video fills the rows one by one; the real run takes about two seconds, so the
  "running" highlight is rarely seen.
- **Lesson code:** `RecordPolicy.Read(tenantId, recordId)` exists as written (reads `Application.User`);
  the services call the overload with an explicit principal. The grant's members are `Tenant` and
  `IClock.Now` (Modules 1-6 names) instead of the lesson's `TenantId` and `_clock.UtcNow`. Deny reasons
  are named (`missing`, `account`, `revoked`, `expired`, `role`, `tenant`) instead of Module 4's `grant`.
- **Intranet grant:** issued per intranet sign-in (page load) from the SID mapping - the same shape and
  store as a partner grant - instead of Module 3's standing grant per account.
- **Admin area:** only on the intranet route, and only in Debug builds running in Development; the
  public partner route has no admin area.
- **Ports and secrets:** the IdP is on 6417 and the client registration lists `https://localhost:6407/…`
  (plus the 6447 URIs on the command line); Module 7 has its own user-secrets IDs
  (`accessops-security-course-m7`, `accessops-security-course-test-idp-m7`).
