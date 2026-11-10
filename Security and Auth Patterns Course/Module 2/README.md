# AccessOps · Security & Auth Patterns · Module 2

**IIS and Browser Integrated Authentication.** AccessOps is the intranet route now: there is no sign-in
form. The browser answers the host's Windows challenge (`401 Negotiate` → `200`) before any Wisej.NET
code runs, and `Program.Main` opens the blue `AccessOps · Confidential documents` window with the
area strip `Departments · Employees · Documents · Approvals · Admin console`. On the left, **Tenant A ·
Documents** lists Record · Title · Class (17 Supplier contract renewal *Confidential*, 18 Payroll
adjustments Q3 *Restricted*, 21 Site access audit *Internal*, 24 Department budget draft
*Confidential*) above the strip **Windows sign-in: `DOMAIN\re•••1` · Grant: `Reader · tenant A`**; on the
right, **Record 17** with its title, the **Open** button, the content box ("Content appears here after
the service allows the read.") and **Repository reads: N**. The Windows account is evidence; the
AccessOps grant mapped to its SID is the permission. An account without Reader passes the same
challenge and gets the alert "You do not have access to this document." with zero repository reads.

Solution: `AccessOps.slnx` = `AccessOps/` (Wisej.NET web app, `net10.0`, `Wisej-4` 4.1.7,
`Microsoft.Data.Sqlite` 10.0.14, `Microsoft.AspNetCore.Authentication.Negotiate` 10.0.14) +
`PolicyHarness/` (plain .NET console). IIS artefacts in `iis/`, written lab deliverables in `docs/`.
Data (SQLite, audit key) in `AccessOpsData/` (git-ignored). Built from Module 1; Module 3 starts here.

## Run it

```powershell
cd "Security and Auth Patterns Course\Module 2"
dotnet build AccessOps.slnx
dotnet run --project AccessOps --urls http://localhost:6402                                        # reader1 (allowed)
dotnet run --project AccessOps --urls http://localhost:6402 -- --AccessOps:CurrentWindowsUser=visitor1   # the application deny
dotnet run --project AccessOps --urls http://localhost:6402 -- --AccessOps:WindowsAuthentication=Off     # the host fault page
dotnet run --project PolicyHarness
```

Open `http://localhost:6402` in Edge or Chrome on the same Windows machine. Chromium-based browsers
answer the Negotiate challenge for `localhost` silently with your Windows logon (NTLM for a local
account). Command line: `curl.exe --negotiate -u : http://localhost:6402/` or
`Invoke-WebRequest http://localhost:6402/ -UseDefaultCredentials -AllowUnencryptedAuthentication`
(PowerShell 7 refuses default credentials over `http` without that switch). Headless Chrome needs
`--auth-server-allowlist=localhost`.

### Demo accounts (invented, local only)

There are no passwords: Windows signs you in. At every start the Windows account **running the app**
(the same one your browser uses on localhost) is mapped by SID to one AccessOps account, chosen with
`AccessOps:CurrentWindowsUser`:

| `CurrentWindowsUser` | AccessOps account | Grant | Lab role |
|---|---|---|---|
| `reader1` (default) | `acct …7f3a`, tenant A | Reader grant `…21`, tenant A, read | `CONTOSO\reader1` |
| `visitor1` | `acct …19c8`, tenant A | none | `CONTOSO\visitor1` (second account without Reader) |
| `admin` | `acct …d4b7`, tenant A | Admin grant `…44`, administer, no read | revoke/restore grants in the admin console |

**Why a switch and not a second Windows account:** this machine has no second local account whose
password the lab may use, and creating Windows accounts is a machine change the sample must not make.
The mapping switch shows the same thing the lab shows with `CONTOSO\visitor1`: a request the host
authenticated, a Windows identity in `Application.User`, and an application deny. In a domain, map
two real accounts in the `windows_accounts` table instead (SID → account), or revoke reader1's grant in
the admin console (run once as `admin`, revoke `…21`, restart as `reader1`: deny with reason `grant`).

## What to try

| Action | Expected result |
|---|---|
| `curl.exe -D - -o NUL http://localhost:6402/` | `HTTP/1.1 401 Unauthorized`, `WWW-Authenticate: Negotiate`; the app log shows a `ChallengeTrace` line and **no** `Program.Main` line — the 401 happened before the app (host layer) |
| Open `http://localhost:6402` (reader1) | no prompt; log: `GET / -> 401 offered=Negotiate`, `Negotiate [redacted] -> 401`, `Negotiate [redacted] -> 200 identity=NTLM · …\ma•••e`; the Documents screen with four rows, footer `Grant: Reader · tenant A`, `Repository reads: 0` (red) |
| Open | content box: "Renewal terms for the tenant A supplier contract, effective next quarter."; `Repository reads: 1` (green); audit `open A / 17 allowed` |
| Click row 21, Open | "Record 21 · Site access audit", its content, reads +1 |
| Admin console (reader1) | "Access denied." (red); in Debug the **Identity diagnostics** button |
| Identity diagnostics | `NTLM` · `True` · masked account · `acct …7f3a · tenant A` · Application.User token (see below) · `NTLM · DOMAIN\ma•••e` · the process account; "No session override: Application.User is the host's Windows principal. The grant decides." |
| Restart with `--AccessOps:CurrentWindowsUser=visitor1`, open the page | same silent challenge; footer `Grant: none`; empty grid (the list is decided by the service too); **Record 17** |
| Open (visitor1) | alert **"You do not have access to this document."**; content box unchanged; `Repository reads: 0`; audit `open A / 17 denied grant` |
| Restart with `--AccessOps:WindowsAuthentication=Off` | **AccessOps · Sign-in problem**: "Windows sign-in did not reach AccessOps … a server configuration problem, not a decision about your access." (`HostFaultPage`) |
| `dotnet run --project PolicyHarness` | one allowed read, seven denies at zero repository calls, including `authenticated, no Reader (M2)` — `docs/policy-harness-output.txt` |

## Lab deliverables → where

| Deliverable (plan) | Where |
|---|---|
| Effective IIS authentication settings for the intranet and public routes | `docs/effective-iis-settings.md`; `iis/windows-auth.location.xml` (from the pack) applied to `Lab Intranet`; `docs/iis-publish-and-rollback.md` (publish, baseline, apply, health, rollback) |
| Redacted challenge trace with scheme, status and follow-up outcome | `AccessOps/Security/ChallengeTrace.cs` (writes the trace); observed trace in `docs/challenge-trace.md` |
| SPN and ticket evidence or an explicit undetermined-provider result | `docs/provider-evidence.md` — **undetermined** on this workgroup PC, procedure for the domain |
| Host-authenticated user denied by the AccessOps policy, with a PolicyHarness case for the same rule | `Program.cs` → `Security/AccessGrants.cs` (`FindForAccount`, `SubjectForAccount`) → `Documents/DocumentService.cs`; `Pages/DocumentsView.cs` (`AlertBox.Show(..., allowHtml: false)`); `PolicyHarness/Program.cs` + `docs/policy-harness-output.txt` |
| Layered diagnosis note separating IIS failure from application deny, with the rollback settings | `docs/layered-diagnosis.md` |

Lab steps → code/docs: 0 Module 1 base + green harness; 2 publish to `Lab Intranet` + baseline →
`iis-publish-and-rollback.md` §1–2; 3 scoped location, 500.19 → `iis/windows-auth.location.xml`,
§3; 4 `Program.Main` (`Application.IsAuthenticated`, `Application.User`, `WindowsIdentity`,
`HostFaultPage`, `AccessGrants.FindForAccount` from the SID) → `AccessOps/Program.cs`,
`Pages/HostFaultPage.cs`, `Infrastructure/WindowsAccountMap.cs`; 5 challenge + diagnostics view →
`ChallengeTrace`, `Pages/IdentityDiagnosticsView.cs`; 6 SPN/klist/F01 → `provider-evidence.md`;
7 visitor deny + harness case; 8 every path → `layered-diagnosis.md`; 9 restore + attach → §5.

## Verified here / only in the lab environment

Verified on this machine (Windows 11 Pro, `WORKGROUP`, local account, .NET 10.0.11 runtime, Kestrel,
Chromium browser at 1400x900 and headless Chrome 1400x900, 2026-11-09):
- build 0 warnings / 0 errors; PolicyHarness green; Release publish has no `IdentityDiagnosticsView`
  and ships `Microsoft.AspNetCore.Authentication.Negotiate.dll` with an in-process `web.config`;
- the Negotiate challenge on Kestrel: anonymous `401 WWW-Authenticate: Negotiate` before
  `Program.Main`; the **Chromium browser completed it silently** (Chromium, `localhost`), as did headless
  Chrome with `--auth-server-allowlist=localhost`, `curl --negotiate -u :` and PowerShell
  `-UseDefaultCredentials`; the provider inside the token was `NTLM`; the WebSocket upgrade
  (`/ws.wx`) went through the same 401/401/101 handshake and Wisej used WebSocket;
- `Program.Main` with a `WindowsIdentity`, the grant from the SID, the allowed read (reads 1), the
  application deny for visitor1 (alert, reads 0, audit `denied grant`), the `HostFaultPage` with
  Windows authentication off, the diagnostics view.

Only in the lab environment (not run here, documented): IIS publish, the scoped location settings,
500.19, IIS log fields, `Negotiate, NTLM` as two headers, Kerberos, the SPN query and the
`klist`/4624 correlation (recorded as **undetermined** here), browser policy (`AuthServerAllowlist`),
proxies, extended protection, a second real Windows account.

## Where this sample differs from the lesson or video

- **Host:** Kestrel + Negotiate here, IIS in the video/lab. Same `Program.Main`; on IIS in-process
  `AddNegotiate()` defers to IIS Windows Authentication (Microsoft docs, not run here).
- **Area strip:** the video window (`AccessOps — Confidential documents`, em dash) has no area strip;
  this sample keeps the course's app bar `AccessOps · Confidential documents` and the area strip.
- **`DocumentsPage(grant).Show()`:** the video's `Program.Main` creates a `DocumentsPage`; here
  `new MainPage(user).Show()` hosts `DocumentsView` (the Module 1 structure). The video's
  `_documents.Open` + `AccessDeniedException` is Module 1's `DocumentService.Read` returning a result.
- **Deny alert:** the video draws a toast "Access denied / You do not have access to this document.";
  Wisej's `AlertBox` shows one message with a warning icon: "You do not have access to this document."
- **visitor1 sees no titles:** the video lists the four documents for the account without a grant;
  here the list goes through `DocumentService.List`, which denies without a grant, so the grid is
  empty and the record panel shows "Record 17" without a title. Titles of confidential documents are
  data too.
- **Repository reads** is the process-wide `ReadCount`, so after earlier reads it is not 0 for
  visitor1 unless the app was restarted (the deny never increments it).
- **Fixtures:** tenant A's titles follow the Module 2 video (Module 1 used Northwind/Q3 travel/Board
  minutes/Onboarding). Accounts are `reader1`/`visitor1`/`admin` (Module 1: `reader.a`/`unassigned`/`admin`,
  same IDs).
- **No sign-in form, no Export:** the intranet route has no form sign-in (Module 1's `TestVerifier`,
  `SignInFlow`, `SignInView`, `ApplicationPrincipal` are gone) and the video screen has only Open
  (Module 1's `ExportJob` is gone).
- **Masked account:** shown as `DOMAIN\ma•••e` (domain, first two characters, `•••`, last character),
  the video's `CONTOSO\re•••1` pattern; the challenge trace never contains a header value.
- **`Application.User` on a WebSocket event** (observed): Wisej returns the `WindowsIdentity` captured
  on the page load, whose token Kestrel disposes with the HTTP connection that authenticated it: in the
  Chromium browser `Name`/`User` threw `ObjectDisposedException` on the first click; in headless Chrome
  (connection still open) it was usable. So the sample maps the SID once in `Program.Main` and keeps
  the subject, and the diagnostics view shows the state as "Application.User token". Module 3 uses
  the WebSocket request's own identity (`IHttpContextAccessor`) instead.
- **PolicyHarness** targets `net10.0` and adds two Module 2 cases (no Reader; unmapped SID).
