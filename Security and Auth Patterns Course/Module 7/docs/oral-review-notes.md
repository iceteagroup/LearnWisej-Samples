# Oral review notes · ten minutes (Module 7)

Graded from the application boundary first, the screen second. Pass: 80 / 100 with no
release-blocking failure.

## 0:00 - 1:30 · The architecture in one picture

- Two IIS applications, one build: `AccessOps:Route = Intranet | Partner` (`docs/route-matrix.md`).
- Three identities, none of them a permission: host principal (WindowsIdentity, page load only),
  app principal (account, tenant, **grant ID**), effective token (inside `RunImpersonated` only).
- Who owns what: directory → groups; provider → external account; credential store → passkeys;
  AccessOps → the grant (`docs/trust-boundaries.md`).

## 1:30 - 4:00 · Allow trace (intranet Reader, case 1)

1. Browser → `GET /` → **401** `WWW-Authenticate: Negotiate` (challenge trace, value redacted).
2. Retry with the token → **200**; `Program.Main` sees `WindowsIdentity` (NTLM here, Kerberos on the domain).
3. SID → `reader1` → `AccessGrants.FindForAccount` issues the grant → `ApplicationPrincipal.Establish`.
4. Click → `RecordPolicy.Read(A, 17)`: grant from the store, account, not revoked, not expired, can read, tenant A → **repository call 1** → audit `allowed`, correlation `rr-01-…`.

Question to expect: *who owns the effective identity?* - the app pool, except inside the one
`RunImpersonated` scope; case 4 shows it comes back after an exception.

## 4:00 - 6:30 · Deny trace (tenant A asks for tenant B's record 17, case 3; stale tab, case 7)

1. Partner sign-in (svc.reports) → grant for tenant A.
2. `RecordPolicy.Read(B, 17)` → reason **tenant** → `AuditService.Deny("policy", …)` → return; **repository calls 0** (the counting repository proves it).
3. Same rule, other reason: tab B signs out → tab A's Export → reason **revoked**, 0 reads, no file, no new grant (seen in the browser: "Export denied · grant revoked · repository reads: 0").

Question to expect: *why before the read?* - the policy is the first statement of the read path;
nothing else can call the repository (Export goes through `RecordPolicy`). *Can an old credential
still reach the service?* - no: the grant is read on every call; the deleted passkey is refused (case 11).

## 6:30 - 9:00 · Failure and rollback

- Injected: node restarted mid-callback with its own key ring → `reason=state`, fixed sentence,
  0 grants; fixed in ~35 s by restoring the shared ring (`docs/runbook.md` §3). Same restart with the
  shared ring: the callback resumed once.
- Stop condition demo: fault injection (replayable hand-off) → cases 9 and 12 fail → KPI 2 → rollback.
- Rollback: partner route off at ingress (503 after 0.3 s) → Reader still reads → case 7 repeated:
  still denied, no export → revocations unchanged → callback grants revoked.

## 9:00 - 10:00 · What is not proven here

Kerberos, SPN, IIS, the second hop, a real authenticator and a second physical node: Fixture /
Harness / Simulated rows with exact rerun plans (`docs/evidence-matrix.md`). Live rows are lab
evidence, not production sign-off.

## Self-score against the rubric

| Criterion | Points | Self-score | Why |
|---|---|---|---|
| Identity architecture and migration decision | 20 | 18 | route matrix, net48 vs ASP.NET Core stated, effective identity explicit; hosting assumptions (IIS) only documented |
| IIS and Windows evidence | 20 | 12 | challenge trace and scoped ACL test with reversion, second-hop decision; no IIS / Kerberos evidence on this PC (half-credit anchor) |
| Application policy and lifecycle | 25 | 25 | server-side tenant deny at 0 calls, revocation across live tabs, session rotation with the old ID refused |
| Federation and credential design | 20 | 18 | account binding, callback replay and credential lifecycle rejections, provenance (4.1.7, no preview); passkeys only with a software authenticator here |
| Operational readiness and defense | 15 | 15 | redacted correlated audit, rehearsed rollback, injected failure with recovery time |
| **Total** | **100** | **88** | no release-blocking failure observed |
