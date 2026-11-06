# AccessOps · Security & Auth Patterns · Module 6

Local lab build for **Module 6 · Auditing, Secrets and Configuration**. The approvals screen from
the walkthrough - *Approve*, *Reject*, `N audited today`, the banner *Request REQ-4471 approved.
Recorded as audit entry N.* and the status `reference xxxx-xxxx` - backed by an append-only,
hash-chained `audit_entry` table, a correlation ID minted per command and carried on every log line,
secrets that live in user secrets / the environment instead of tracked files, and a log sink that
redacts what must never be written.

> **Wisej.NET version.** These samples target the released **Wisej-4 4.1.7**. The Wisej.NET 4.2
> `Passkey` and `OAuthClient` components the course teaches in Modules 2 and 4 are not available;
> passkeys are not implemented and external sign-in uses ASP.NET Core's OpenID Connect handler (see
> Module 4). Module 6's lab moves *the OIDC client secret* out of source control - here that is the
> secret of the ASP.NET Core OpenID Connect handler (`Oidc:ClientSecret`), not an `OAuthClient`
> property.

## Set the secrets once, then run - two servers

The app refuses to start without its three secrets (`Oidc:ClientSecret is not configured…`). They
live in your user profile, outside the repository:

```bash
cd "Security and Auth Patterns Course/Module 6/AccessOps"
dotnet user-secrets set "ConnectionStrings:AccessOps" "Data Source=../AccessOpsData/accessops.db"
dotnet user-secrets set "Oidc:ClientSecret" "<a long random value>"
dotnet user-secrets set "ShareLinks:SigningKey" "<base64 of 32 random bytes>"

cd "../AccessOps.TestIdentityProvider"
dotnet user-secrets set "Clients:accessops:Secret" "<the same value as Oidc:ClientSecret>"
```

```bash
cd "Security and Auth Patterns Course/Module 6/AccessOps.TestIdentityProvider"
dotnet run --urls http://localhost:6416          # TEST DOUBLE - never deploy

cd "Security and Auth Patterns Course/Module 6/AccessOps"
dotnet run -f net10.0 --urls http://localhost:6406
```

```bash
cd "Security and Auth Patterns Course/Module 6"
dotnet test          # 39 tests
```

Module 7 uses the same user-secrets ids, so this is a one-time step for both modules. Work-account
users: `jamie.mercer@…` (Finance manager, approves Ruth Okoye's requests) and `sofia.reyes@…`
(Administrator: permission editor and audit trail); demo passwords in
[users.json](AccessOps.TestIdentityProvider/Data/users.json).

## What to try

| Action | Expected result |
|---|---|
| Jamie Mercer → Approvals, REQ-4471 → **Approve** | Banner *Request REQ-4471 approved. Recorded as audit entry N.*; status *Approved — audited* · `reference xxxx-xxxx`; `audited today` goes up by one |
| **Reject** on the already-approved REQ-4471 | *That request could not be rejected. Reference xxxx-xxxx.* - a `Failed` audit row and a warning log line under that reference |
| REQ-4104 (raised by someone who is not Jamie's report) → **Approve** | *You are not allowed to do that…*; status *Refused — audited*; a `Denied` row |
| Sofia Reyes → Admin console: select Jamie Mercer, `ViewAuditTrail`, **Grant**, then **Revoke** | *Granted ViewAuditTrail for Jamie Mercer · audit entry N · takes effect at their next sign-in or re-resolve*; the change and its row commit together |
| **Verify chain** | *Audit chain verified · every entry matches* |
| Documents → **Download**, **Share link (1 h)**, open the link, tamper with it | `Document.Download`, `Document.Share`, `Document.ShareOpen` rows (`Allowed`, then `Denied` for the tampered link) |
| `AccessOpsData/logs/accessops-YYYYMMDD.jsonl` | Every line of a command carries the same `CorrelationId` and `UserId`; no password, token, signature, cookie or e-mail address |
| Start the app with a secret removed | It stops at start-up naming the missing key and where to supply it |

## Lab tasks → where in the code

| Deliverable | Where |
|---|---|
| AuditService and append-only audit table | [AuditService.cs](AccessOps/Services/AuditService.cs) (only writer, gapless `seq`, SHA-256 chain, `VerifyChain`); table + no-update/no-delete triggers in [AccessOpsDatabase.cs](AccessOps/Infrastructure/AccessOpsDatabase.cs); callers `ApprovalService`, `PermissionEditorService`, `DocumentService`, `ShareLinkEndpoint`, `LoginAuditService`, `ExternalIdentityMapper`; [docs/AuditTrail.md](docs/AuditTrail.md) |
| Correlation ID in every command and log line | [CommandContext.cs](AccessOps/Services/CommandContext.cs) (minted at the top of the handler, actor and address from `SessionContext`), [Command.cs](AccessOps/Pages/Command.cs) (`ILogger.BeginScope`), passed into every service call; [RedactingLogger.cs](AccessOps/Infrastructure/RedactingLogger.cs) writes the scope on each line |
| Secrets moved to environment configuration | `UserSecretsId` in both csproj files, the start-up checks in [OpenIdConnectSetup.cs](AccessOps/Infrastructure/OpenIdConnectSetup.cs), `AccessOpsDatabase.FromConfiguration`, `ShareLinkService`; `launchSettings.json` and `appsettings.json` hold none; `.json` excluded from the file server in [Startup.cs](AccessOps/Startup.cs) |
| Secret rotation procedure | [docs/SecretRotation.md](docs/SecretRotation.md) (owner, store, expiry, two-valid-credentials overlap per secret; `ShareLinks:PreviousSigningKey` implements the overlap for the signing key) |
| Log redaction review | [docs/LogRedactionReview.md](docs/LogRedactionReview.md) - real lines from one approval and one download, every field marked |
| Client address behind a proxy | `UseForwardedHeaders()` with `Hosting:KnownProxies` in [Startup.cs](AccessOps/Startup.cs) |

## Verified

- `dotnet build`: 0 errors, 0 warnings. `dotnet test`: 39/39, including `UPDATE`/`DELETE` refused
  by the table, the chain naming the first altered row after the owner drops the trigger, the
  approval and its row sharing a correlation ID, the `Denied` and `Failed` rows, the grant/revoke
  transaction, the signing-key overlap and the sink test with a known secret marker.
- Fresh start with no user secrets: `Unhandled exception. System.InvalidOperationException:
  Oidc:ClientSecret is not configured…`. With the secrets set: `Audit chain check at start-up: intact`.
- Browser pane (`accessops-m6` + temporary `accessops-idp-m6`): every row of *What to try*. Sign-out
  ended both sessions (the provider asked for the password again), the share-link probes were
  `fetch` calls from the page (200 `application/pdf`, tampered signature 404, extended expiry 404).
  No console errors other than those two deliberate 404s; no server errors.
- Found and fixed during the run: the admin grid lost its selection after a grant, so the next
  **Revoke** landed on the first row (now the selection is kept); the refusal log printed `(null)`
  for a work-account user's name (names removed from the line); the already-decided failure wrote
  nothing (now a `Failed` row and a warning); the share command stayed open while the dialog was
  shown (now it ends when the link is issued); work-account employees were missing from the admin
  console (now listed as `work account`).

## Answers to the review questions

1. **What the application's own login can still do.** It cannot change or delete a row, but it can
   *insert*: append fabricated rows that never happened, with a correct `seq` and a correct hash,
   because the hash is not a signature - anyone who can insert can compute it. The verification pass
   does not flag those; it only proves that nothing *before* them was altered or removed. What
   catches them is the business data disagreeing with the trail (an `Approval.Approve` row for a
   request still `Pending`) and copying each day's last hash somewhere the login cannot write.
   If the attacker has the *owner* login instead, they can drop the trigger and rewrite row 3; the
   pass then names row 3 - its stored hash no longer matches its fields - and every row after it
   fails too, so *Verify chain* reports `Audit chain BROKEN at entry 3`.
2. **A background task after sign-out.** From the `CommandContext` the event handler minted and
   passed into the service - it holds the correlation ID, the actor id and the address as plain
   values, so work started with `Application.StartTask` keeps them after the session ends. A task
   that reads `IAccessOpsSession.Current` instead gets `NotSignedInException` (or another user's
   context, on a thread that is not the session's), and the audit row is written with no actor and
   no correlation - the exact row an investigation needs.
3. **The secret removed in a later commit.** It is still in the repository's history, every clone,
   fork and CI cache. The incident is closed only when the secret has been rotated and the old value
   revoked at the provider, the provider's logs have been checked for use of the old value, and the
   new value lives only in the secret store - see the last section of
   [SecretRotation.md](docs/SecretRotation.md).

## Where this sample differs from the lesson

- **INSERT-only by triggers.** SQLite has no logins or grants; the sample refuses `UPDATE` and
  `DELETE` with triggers and documents the `GRANT INSERT, SELECT` / `DENY UPDATE, DELETE` a server
  database uses ([AuditTrail.md](docs/AuditTrail.md)).
- **Correlation ID format.** `xxxx-xxxx` (32 random bits) as in the video's `7f3c-9ad2`, instead of
  a full `Guid` or the W3C trace id; over the Wisej.NET WebSocket the trace id is per connection,
  not per click.
- **Audit numbers.** The video's row for the approval is entry 4471 in a long-running trail; a
  fresh sample database starts at 1. The video's actor column shows `u-2081 · j.mercer`; the sample
  shows the stable id only (`u-2201`) and keeps names out of the trail and the logs.
- **The incident trace.** The video's trace includes an *Authorize allowed* line and an export
  streamed by `DocumentService` inside the approval; the sample logs refusals, not every allowed
  check, and an approval does not export a document.
- **Background work.** No approval is finished by `Application.StartTask` in this sample; the
  services take the `CommandContext` as a parameter so such a task would be correct (review
  answer 2).
- **Currency.** Amounts are shown as `€ 18,400` (the Module 3 seed); the video shows `£18,400`.
- **OIDC client secret.** It is the ASP.NET Core OpenID Connect handler's secret, not the 4.2
  `OAuthClient` component's (see the note at the top). Lab deliverable affected: *Secrets moved to
  environment configuration* - satisfied with the handler's `Oidc:ClientSecret`.
