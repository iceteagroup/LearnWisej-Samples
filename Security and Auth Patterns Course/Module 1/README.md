# AccessOps · Security & Auth Patterns · Module 1

**Identity Boundaries in Wisej.NET.** AccessOps opens on the blue `AccessOps · Sign in` window with the
area strip `Departments · Employees · Documents · Approvals · Admin console`. After a demo sign-in the
Documents screen of the walkthrough appears: a request bar with **Tenant** and **Record** boxes and the
**Open** / **Export** buttons, the grid **Record · Title · Classification · Tenant** with tenant A's four
documents (17 Supplier contract — Northwind renewal, 18 Q3 travel policy, 21 Board minutes — October,
24 Onboarding checklist) and a status line that turns idle, ok (green), deny (red) or warn (amber). Every
read goes through `DocumentService.Read(subject, tenant, recordId)`, which checks the current grant and
the tenant before a read-counting repository runs. The admin console revokes and restores grants, shows
the audit trail and `CountingDocumentRepository.ReadCount`, and in Debug builds opens the
`AccessOps · Identity diagnostics` view (authentication type, `IsAuthenticated`, redacted subject, host
request principal, effective OS identity).

Solution: `AccessOps.slnx` = `AccessOps/` (Wisej.NET web app, `net10.0`, `Wisej-4` 4.1.7,
`Microsoft.Data.Sqlite` 10.0.14) + `PolicyHarness/` (plain .NET console, no Wisej). Data (SQLite database,
audit-pseudonym key) is created in `AccessOpsData/` beside the project, outside the web root, and is
git-ignored. Modules 2 and 4 start from this folder.

## Run it

```powershell
cd "Security and Auth Patterns Course\Module 1"
dotnet build AccessOps.slnx
dotnet run --project AccessOps --urls http://localhost:6401     # Debug build: http://localhost:6401
dotnet run --project PolicyHarness                               # the policy without Wisej, IIS or an IdP
```

No IIS, domain, identity provider or secret is needed. To start from fresh fixtures, stop the app and
delete `AccessOpsData/`. The Debug console prints one `AccessOps.IdentityTrace` line per lifecycle point
(startup, refresh, click, StartTask, Task.Run) — that is where the identity matrix comes from.

### Demo accounts (invented, local only, Debug builds only)

| User name | Secret | Account | Grant |
|---|---|---|---|
| `reader.a` | `reader-demo-17` | `acct …7f3a`, tenant A | Reader grant `…21`, tenant A, read |
| `unassigned` | `unassigned-demo` | `acct …19c8`, tenant A | none |
| `admin` | `admin-demo-21` | `acct …d4b7`, tenant A | Admin grant `…44`, administer, no read |

`TestVerifier` is a **training mock that never ships**: it holds SHA-256 hashes of these demo secrets and
exists only inside `#if DEBUG`. A Release build has no verifier and answers every sign-in with
"Sign-in is not available in this build." Module 4 replaces it with AccessOps' own credential store.

### Prove the diagnostics view is not packaged

```powershell
dotnet publish AccessOps/AccessOps.csproj -c Release -o $env:TEMP\accessops-m1-release
Select-String -Path $env:TEMP\accessops-m1-release\AccessOps.dll -Pattern IdentityDiagnosticsView,TestVerifier -SimpleMatch -Quiet   # False
```

Result recorded in `docs/release-check.md`.

## What to try

Use two browser tabs (each tab is its own Wisej.NET session) or two browser profiles.

| Action | Expected result |
|---|---|
| Tab 1: sign in as `reader.a` with a wrong secret | "Sign-in failed." (red) |
| Tab 1: sign in as `reader.a` / `reader-demo-17` | The page reloads (new session ID), Documents shows four rows, "Tenant A · 4 documents · signed in as a Reader" |
| Tab 1: Open with Tenant `A`, Record `17` | "Record 17 opened · Supplier contract — Northwind renewal" (green); ReadCount +1 |
| Tab 1: Tenant `B`, Record `17`, Open (fixture **F02**) | "Access denied." (red), no record content; audit `open B / 17 denied tenant`; ReadCount +0 |
| Tab 1: Tenant `A`, click row 17 | "Tenant A · record 17 selected" |
| Tab 1: Export | "Record 17 exported · Supplier contract — Northwind renewal", `record-A-17.txt` downloads (the job ran on `Task.Run` with an explicit subject) |
| Tab 2: Admin console (not signed in) | "Access denied."; in Debug the "Identity diagnostics" button is there |
| Tab 2: Identity diagnostics (not signed in) | `(none)` · `False` · "none: host principal returned" · "not authenticated (Kestrel)" · the process account; "No override: the getter falls back to the host principal." |
| Tab 1: Admin console → Identity diagnostics (as reader) | `Portal` · `True` · "acct …7f3a · tenant A" · same host principal and **same OS identity** as tab 2; "The session override answers Application.User." |
| Tab 2: Documents → sign in as `admin` / `admin-demo-21`, then Admin console | Grants `…44 admin` and `…21 reader.a`, both Active; ReadCount; the audit list |
| Tab 2: select grant `…21`, Revoke | "Reader grant …21 revoked." (amber); state Revoked; audit `grant.revoke` |
| Tab 1 (screen untouched, Export still visible): Export (fixture **F05**) | "Access denied." (red); audit `export A / 17 denied grant`; ReadCount +0 |
| Tab 2: Restore | "Reader grant …21 restored." (green) |
| Tab 2: Fail next read, then Tab 1: Open | "The document could not be loaded. Try again later." (amber); the log has the exception with the correlation ID and no identity; audit `failed repository` |
| New tab: sign in as `unassigned` / `unassigned-demo` | Documents: empty grid, "Access denied."; Open → "Access denied."; audit reason `grant`; ReadCount +0 |
| `dotnet run --project PolicyHarness` | One allowed read and five denies (tenant boundary F02, missing role, unknown, expired, revoked) at zero repository calls — `docs/policy-harness-output.txt` |

## Lab deliverables → where

| Deliverable (plan) | Where |
|---|---|
| AccessOps solution with tenant A and B fixtures and a read-counting repository | `AccessOps.slnx`; fixtures in `AccessOps/Infrastructure/AccessOpsDatabase.cs` (tenants A/B, record 17 in both with different contents, reader / unassigned / admin accounts, grants); `AccessOps/Documents/CountingDocumentRepository.cs` (`ReadCount`) |
| DocumentService resource gate with grant, tenant and record checks | `AccessOps/Documents/DocumentService.cs` (`Read`, `List`), `AccessOps/Security/GrantStore.cs` (`Find` reads the current row), `AccessOps/Security/AuditLog.cs` (append-only `audit` table) |
| Identity diagnostics view, removed from the packaged build | `AccessOps/Pages/IdentityDiagnosticsView.cs`, `AccessOps/Identity/IdentityProbe.cs` (both `#if DEBUG`); proof in `docs/release-check.md` |
| Four-column identity matrix for request, session, operation and outcome | `docs/identity-matrix.md`; trust note `docs/trust-and-identity.md` |
| PolicyHarness run with one allowed read and two denies at zero repository calls | `PolicyHarness/Program.cs`; output `docs/policy-harness-output.txt` |

Lab steps → code: sign-in click `Pages/SignInView.cs` (`btnSignIn_Click`) → `Identity/SignInFlow.cs`
(pending key, `Application.Reload()`) → `Program.cs` `Application.ApplicationRefresh` →
`Identity/ApplicationPrincipal.cs` (`RefreshSessionId()` first, then `Application.User`);
`Application.StartTask` in `Pages/DocumentsView.cs` (`btnOpen_Click`); `Task.Run` with an explicit
`Subject` in `Documents/ExportJob.cs`; `Identity/Subject.cs` (`From`, `Opaque`, `Redacted`).

## Verified here / only in the lab environment

Verified on this machine (Windows 11 workgroup PC, .NET 10, Kestrel, Chromium browser at 1400x900):
build (0 warnings, 0 errors), PolicyHarness, every row of "What to try" in two tabs, the session ID
rotation on the sign-in reload (the browser's `sessionStorage` adopted the new ID), the trace for
startup / click / StartTask / refresh / Task.Run, the Release publish without the diagnostics view and
without a verifier.

Not verifiable here (and not needed for Module 1): a host principal other than anonymous (IIS Windows
Authentication is Module 2), a per-user Windows token (impersonation is Module 3).

## Where this sample differs from the lesson or video

- **Sign-in screen, admin console:** the video shows only the Documents screen and the diagnostics view.
  The sign-in card (User name, Secret, Sign in) and the admin console (grants, audit, ReadCount, "Fail
  next read") are built in the same style because the lab needs them; the video shows the revocation only
  as a callout ("Admin console · another session").
- **ReadCount on screen:** the video draws `CountingDocumentRepository.ReadCount` as an overlay beside the
  window; here it is in the admin console's repository strip.
- **Selected-row status:** the run scene's "Tenant A · record 17 selected · the screen has not changed" is
  narration of that moment; the app says "Tenant A · record 17 selected".
- **Failure path:** an unexpected repository failure is produced with the test repository's
  "Fail next read" seam (an injected exception), since nothing in a healthy SQLite file fails on demand.
- **Grid selection:** Wisej's Bootstrap-4 theme draws the selected row grey with a focus outline, not the
  mock's light blue with a left accent bar.
- **ApplicationPrincipal:** the pack's `common/ApplicationPrincipal.cs` sets `Application.User` and then
  calls `RefreshSessionId()`; this sample (like the lesson) rotates first, then sets the principal — and
  only on a page load, because from a WebSocket click the browser never adopts the new ID.
- **Audit inside the service:** the video's `ExportJob.cs` writes the audit event in the job; here
  `DocumentService` writes one event per decision for every path, so the job cannot forget it.
- **Grant display:** account IDs are shown with their last four characters (`acct …7f3a`), grant IDs with
  their last two (`…21`), matching the video.
- **PolicyHarness** targets `net10.0` (the pack's targets `net8.0`; the rule is identical) and adds the
  pack's missing-role, unknown and expired cases next to F02 and the revoked grant.
- **Effective OS identity** shows the real process account of whoever runs the sample (the video shows
  the invented `DEVBOX\dev`).
