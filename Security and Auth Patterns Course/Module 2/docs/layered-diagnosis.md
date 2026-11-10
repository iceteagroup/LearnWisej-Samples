# Layered diagnosis note — IIS failure vs application deny (Module 2 deliverables 4 and 5)

Work outward in: **which layer answered?** Each row says what you see, where the evidence is, who
owns the fix, and what was observed on this machine.

| # | Layer | Symptom | Evidence | Owner / fix | Observed here (Kestrel, 2026-11-09) |
|---|---|---|---|---|---|
| 1 | **Host challenge** — a 401 before `Program.Main` | browser prompt, 401 loop, or a plain 401 page; the Wisej.NET app never starts a session | IIS log `sc-status=401` with `sc-substatus` (1 = credentials not accepted / handshake leg, 2 = server configuration, 3 = ACL on the resource) and `sc-win32-status` (e.g. `0x8009030C` = logon failed, `0x80090302` = SEC_E_UNSUPPORTED_FUNCTION; IIS logs it in decimal: 2148074252 / 2148074242), Edge DevTools scheme + status | host / browser: SPN (`setspn -Q`), `AuthServerAllowlist`, intranet zone, proxy affinity — **not** the Wisej button | `curl http://localhost:6402/` → **401** `WWW-Authenticate: Negotiate`; the AccessOps log shows the `ChallengeTrace` line and **no** `Program.Main` trace line: the app never ran |
| 2 | **IIS configuration lock** — 500.19 | "HTTP Error 500.19 – Internal Server Error, This configuration section cannot be used at this path" right after authentication settings went into `web.config` | the error page names the config file, the line and the locked section (`system.webServer/security/authentication/...`) | server owner: remove the section from `web.config`, set the same values at `apphost` level (`iis/windows-auth.location.xml`, `appcmd ... /commit:apphost`) or in IIS Manager. Do not unlock sections. Code unchanged. | not reproducible: no IIS here. Lab environment only. |
| 2b | **Host misconfigured but reachable** | the app loads but there is no Windows identity (Anonymous left on, Windows Authentication not enabled, a proxy that strips the handshake) | `Program.Main`: `Application.IsAuthenticated == false` or `Application.User.Identity` is not a `WindowsIdentity` | host owner; the app shows `HostFaultPage` ("Windows sign-in did not reach AccessOps … a server configuration problem, not a decision about your access") and decides nothing | run with `--AccessOps:WindowsAuthentication=Off`: the Chromium browser showed **AccessOps · Sign-in problem** with that text |
| 3 | **Application deny** — host sign-in succeeded, the grant says no | the page loads with the right Windows account; Open → alert "You do not have access to this document."; content box stays empty | audit event `open A / 17 denied grant` (opaque subject, correlation ID), `CountingDocumentRepository.ReadCount` unchanged, diagnostics view: `NTLM`/`Kerberos` · `True` · masked account | AccessOps grant owner (admin console / access request). This is the **wanted** outcome for an account without Reader. | run with `--AccessOps:CurrentWindowsUser=visitor1`: footer `Windows sign-in: DEVBOX\d•v · Grant: none`, grid empty (the list is decided by the service too), Open → **"You do not have access to this document."**, `Repository reads: 0`, audit `list documents denied grant` and `open A / 17 denied grant` |
| 3b | **Allowed** | Open shows the record | audit `open A / 17 allowed`, ReadCount +1 | — | default run (`reader1`): Open → "Renewal terms for the tenant A supplier contract, effective next quarter.", `Repository reads: 1` |
| 4 | **Second hop** | the page works, a file share or SQL integrated call made *as the user* fails | the first hop's provider (`provider-evidence.md`: NTLM cannot be delegated), constrained delegation target, backend ACL | Module 3 | — |

## The application deny, with the rule in the PolicyHarness

- App: `Program.Main` → `AccessGrants.FindForAccount(windows.User?.Value)` → `null` for visitor1 →
  `DocumentService.Read(subject, "A", 17)` denies with reason `grant` **before** the repository runs.
- UI: `AlertBox.Show("You do not have access to this document.", MessageBoxIcon.Warning, allowHtml: false)`
  — one generic sentence; the reason is only in the audit event.
- Harness (`PolicyHarness/Program.cs`, output `policy-harness-output.txt`):
  `PASS authenticated, no Reader (M2) CONTOSO\visitor1, A / 17   denied   repository calls +0`.

## Rollback settings

IIS: `appcmd restore backup "accessops-m2-before"`, or clear the two sections at `apphost` level for
`Lab Intranet` (`iis-publish-and-rollback.md`, step 5) and confirm health. Baseline = server defaults
(Anonymous on, Windows off) for a newly created site. This machine: nothing to roll back (no
machine setting was changed; stop the process). The lab mapping of this Windows account is a row in
`AccessOpsData/accessops.db` (`windows_accounts`), rewritten at every start from
`AccessOps:CurrentWindowsUser`; delete `AccessOpsData/` to start from fresh fixtures.
