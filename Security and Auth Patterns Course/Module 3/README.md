# AccessOps · Security & Auth Patterns · Module 3

**Windows Impersonation and Delegation.** The intranet route of Module 2 (Windows sign-in through the
host's Negotiate challenge, the grant mapped from the account SID) now opens on the confidential-documents
reader: the blue `AccessOps · Confidential documents` window with the area strip, the list **Document ·
Title · Folder · Last read** (DOC-201 Salary bands 2027 `labuser-only`, DOC-202 Vendor contract archive
`pool-only`, DOC-203 Audit sample (F03) `labuser-only`, DOC-204 Tenant B legal hold `tenant-b`), the
**Identity trace** table (Document · Step · App subject · Request identity · Effective token · Outcome)
and the status line. Clicking a document sends only its ID: `DocumentCatalog.ResolveApprovedPath` runs
the Module 1 resource policy first and maps the ID to a canonical path under one approved root, then
`ScopedDocumentReader.Read` reads the file inside `WindowsIdentity.RunImpersonated` with the request's
Windows token and records the before / inside / after rows. Four plain messages: "DOC-201 opened ·
effective identity back to the pool", "DOC-202 could not be opened with your Windows account. The
request was logged.", "DOC-203 could not be opened. The file is missing; the request was logged.",
"DOC-204 is outside your access. Nothing was opened." — no path, SID or exception text on screen.

Solution: `AccessOps.slnx` = `AccessOps/` (`net10.0`, `Wisej-4` 4.1.7, `Microsoft.Data.Sqlite` 10.0.14,
`Microsoft.AspNetCore.Authentication.Negotiate` 10.0.14) + `PolicyHarness/`. Fixture script
`tools/Setup-Fixtures.ps1`, IIS location file `iis/` (from Module 2), written deliverables in `docs/`.
Data and fixture files in `AccessOpsData/` (git-ignored). Built from Module 2 (its IIS, challenge and
provider docs stay in `Module 2/docs`).

## Run it

```powershell
cd "Security and Auth Patterns Course\Module 3"
powershell -ExecutionPolicy Bypass -File tools\Setup-Fixtures.ps1     # approved root + ACLs (re-runnable; -Remove to clean up)
dotnet build AccessOps.slnx
dotnet run --project AccessOps --urls http://localhost:6403
dotnet run --project PolicyHarness
```

Open `http://localhost:6403` in Edge/Chrome on the same machine (silent Negotiate for localhost; see
Module 2). The Debug console prints one `AccessOps.IdentityTrace` line per row, with the SIDs, the
token's logon group, the path and the exception — the detail the screen leaves out.

In the lab environment: `tools\Setup-Fixtures.ps1 -PoolAccount "IIS APPPOOL\AccessOps Intranet" -LabUser "LAB\dana.ortiz"`,
publish as in `Module 2/docs/iis-publish-and-rollback.md`, and set `AccessOps:ApprovedRoot` if the
fixtures live elsewhere.

### Demo accounts (invented, local only)

As in Module 2: no passwords; the Windows account running the app is mapped to `reader1` (Reader,
tenant A) — `--AccessOps:CurrentWindowsUser=visitor1` for an account without a grant (every document
is then "outside your access", zero scopes opened — the same rule as the harness case "no Reader (M3)";
not clicked through in the app for this module).

## What to try

| Action | Expected result |
|---|---|
| Open the page | the list with four documents, "—" in Last read, empty Identity trace, status "Ready" |
| Click **DOC-201** | rows `Before · DEVBOX\you · Readable as pool`, `Inside · you · Impersonation · Read 18 KB`, `After · you · Scope closed` (tinted); chip **Read as user**; status (green) "DOC-201 opened · effective identity back to the pool" |
| Click **DOC-202** | `Before · Access denied (ACL)`, `Inside · … · Impersonation · Access denied (ACL)`, `After · Reverted`; chip **Denied (ACL)**; status (red) "DOC-202 could not be opened with your Windows account. The request was logged." |
| Click **DOC-203** (F03) | rows labelled `DOC-203 F03`: `Before · File missing`, `Inside · Impersonation · FileNotFoundException`, `After · Reverted`; chip **File missing**; status (amber) "DOC-203 could not be opened. The file is missing; the request was logged." |
| Click **DOC-204** | one row `Policy · Denied before any scope`; chip **Not allowed**; status (red) "DOC-204 is outside your access. Nothing was opened."; audit `scoped-read B / DOC-204 denied tenant`, no `file.read` |
| App log | per row `identity-trace … effective=… effectiveSid=… level=None/Impersonation logon=… outcome=… detail=<path or exception>`; audit `scoped-read` (policy) + `file.read` (outcome) with one correlation ID |
| `dotnet run --project PolicyHarness` | Module 1/2 cases plus six Module 3 cases: allowed DOC-201 and approved F03 open a scope; tenant B, no Reader, unknown ID, a catalog row escaping the root: denied at **zero scopes** — `docs/policy-harness-output.txt` |

## Lab deliverables → where

| Deliverable (plan) | Where |
|---|---|
| ScopedDocumentReader with a server-resolved canonical path | `AccessOps/Documents/ScopedDocumentReader.cs` (from the pack's `net8/WindowsScopedRead.cs`), `Documents/DocumentCatalog.cs` (`ResolveApprovedPath`: policy → catalog → canonical path under the root), `Documents/DocumentService.cs` (`Authorize`), `Infrastructure/ConfidentialFileStore.cs`, `tools/Setup-Fixtures.ps1`, `docs/acl-fixtures.md` |
| Before, inside and after identity and outcome matrix | `Documents/IdentityTrace.cs`; `docs/identity-outcome-matrix.md` (observed here + lab expected) |
| Forced-failure reversion trace | `docs/forced-failure-reversion-trace.md` (F03, observed) |
| Impersonate setting and StartTask comparison across HTTP and WebSocket | `docs/impersonate-setting-and-starttask.md` (.NET Framework: documented; .NET 10: observed with `Identity/ImpersonationObserver.cs`) |
| Second-hop classification and migration decision for the remote resource | `docs/second-hop-decision.md` (F04; decision: restricted service identity + explicit audit) |

Lab steps → code/docs: 0 Module 2 + harness; 2 fixture folders → `Setup-Fixtures.ps1`; 3
`DocumentCatalog.ResolveApprovedPath`; 4 `ScopedDocumentReader.Read` (`RunImpersonated(identity.AccessToken, …)`);
net48 shape → `impersonate-setting-and-starttask.md` §A; 5 `IdentityTrace` (Name/User/ImpersonationLevel,
identities disposed) → matrix; 6 F03 → reversion trace; 7 impersonate on/off, StartTask, HTTP/WebSocket,
F04 → §B–C, `second-hop-decision.md`; 8 four plain messages → `Pages/DocumentsView.cs`; 9 harness +
matrix marking + decision.

## Verified here / only in the lab environment

Verified on this machine (Windows 11 Pro, `WORKGROUP`, one local account, .NET 10, Kestrel Negotiate,
Chromium browser 1400x900 + headless Chrome, 2026-11-09): build 0 warnings / 0 errors; Release publish has
no `IdentityDiagnosticsView` / `ImpersonationObserver`; PolicyHarness green; all four "Show every path"
outcomes in the Chromium browser with the exact status texts; `RunImpersonated` with the **WebSocket
request's** `WindowsIdentity` (via `IHttpContextAccessor`) gives `ImpersonationLevel = Impersonation`
inside and reverts on return, on the ACL exception and on F03 (after = before); the policy denial opens no
scope; an ACL denial on an existing file and a missing file are told apart; with `"impersonate": true` on
.NET 10 the events fire in the order ImpersonationBegin → ThreadBegin → ImpersonationEnd → ThreadEnd, for
requests, WebSocket events and `StartTask`, with WebSocket on and off, and nothing is impersonated.

Only in the lab environment: the ACL **flip** between the pool and a different lab user (one machine,
one account: the NTLM loopback token is the same logon session — `docs/acl-fixtures.md`), IIS Windows
Authentication, the .NET Framework sandbox (`Application.UserIdentity`, default impersonation,
`using (identity.Impersonate())`), Kerberos, delegation and the second hop (F04).

## Where this sample differs from the lesson or video

- **Navigation:** the video window has a left menu (Dashboard · Documents · Approvals · Employees);
  the sample keeps the course's area strip (`Departments · Employees · Documents · Approvals · Admin console`).
- **Identities in the trace:** the video shows `LAB\dana.ortiz` vs `IIS APPPOOL\AccessOps`; here the
  pool and the user are the same account (`DEVBOX\…`), and only `· Impersonation` marks the inside row.
  Hence DOC-201 reads "Readable as pool" before the scope (video: "Access denied (ACL)"), and DOC-202 is
  denied before *and* inside (video intro chip "Read as pool"): local mode cannot flip the outcome.
- **Before row for F03:** the video shows only Inside/After for DOC-203; the sample records Before too
  (the lab compares before and after).
- **Folder of DOC-204:** the video column says "outside grant"; the real fixture folder is `tenant-b`.
- **App subject:** `acct …7f3a` (redacted account ID, as in Modules 1–2) instead of the video's `acct 4127`.
- **Document IDs are strings** (`DOC-201`); the lesson's code takes `int documentId`. The signature is
  otherwise the lesson's: `Read(HttpContext context, string documentId)`.
- **How a read starts:** the video shows no button; a click on a document row runs the read.
- **Request identity source:** the lesson's code reads `context.User`; the sample passes the current
  request from `IHttpContextAccessor` (the WebSocket request), because `Application.User` on a WebSocket
  event is the page-load identity whose token may already be disposed (observed, Module 2 README).
- **Baseline read:** the before row opens (does not read) the file as the process — a lab probe, so the
  matrix has a baseline outcome; production code would not touch the file outside the scope.
- **Document list:** lists all four catalog IDs (the lab needs DOC-204 to show the policy deny); the
  read itself is always decided by the policy.
- **Logon group in the log:** the trace log adds the token's logon-type group (INTERACTIVE/NETWORK/
  SERVICE/BATCH), which is how the loopback-token fact was found.
- `IdentityTrace` is the Module 3 class; Module 1–2's Debug lifecycle logger is renamed `LifecycleTrace`.
