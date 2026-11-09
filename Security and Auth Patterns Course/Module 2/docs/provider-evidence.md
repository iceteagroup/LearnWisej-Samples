# SPN and ticket evidence — or an explicit undetermined result (Module 2 deliverable 3, fixture F01)

Rule (lesson): claim **Kerberos** only when two pieces of evidence agree for the same moment —
`klist` on the client shows a service ticket for `HTTP/accessops.contoso.test`, **and** the server's
logon record (Security log event 4624 for the request's logon) names `Kerberos` as the
authentication package. Claim **NTLM** only from the same pair (no ticket + package `NTLM`).
Anything less is **undetermined**. A `WWW-Authenticate: Negotiate` header proves neither.

## Result on this machine (2026-11-09): **undetermined — Kerberos cannot be claimed**

| Evidence | Command | Observed |
|---|---|---|
| Machine | `(Get-CimInstance Win32_ComputerSystem).PartOfDomain` | `False`, workgroup `WORKGROUP` |
| HTTP SPN | `setspn -Q HTTP/localhost` | `Ldap Error(0x51 -- Server Down): ldap_connect` — no directory to ask |
| Client tickets | `klist` | `Cached Tickets: (0)` |
| Server token | `Application.User.Identity.AuthenticationType` (diagnostics view, trace) | `NTLM` |
| Server logon record | Security log 4624 | not read: needs elevation (not used for this run) |

Recorded result: **provider undetermined for the lab claim** (workgroup: a challenge followed by 200
was observed, Kerberos cannot be claimed; the server-side token reports the `NTLM` package and the
client holds no ticket, which is consistent with NTLM, but the logon record was not correlated).

## Procedure in the lab environment

On the **domain client**, as `CONTOSO\reader1`, in a fresh logon session:

```powershell
klist purge                                   # start without cached tickets
setspn -Q HTTP/accessops.contoso.test         # exactly one account must hold the SPN
#   Registered for: CN=WEB01,...  (machine account: kernel mode, useAppPoolCredentials=false)
#   none        -> Negotiate falls back to NTLM
#   duplicate   -> KRB_AP_ERR_MODIFIED / 401 loop: fix with the directory team only
Start-Process msedge https://accessops.contoso.test/      # canonical FQDN, never an IP or alias
klist | Select-String "HTTP/accessops.contoso.test"       # service ticket present? note the time
```

On the **IIS server**, for the same minute:

```powershell
Get-WinEvent -FilterHashtable @{LogName='Security'; Id=4624; StartTime=(Get-Date).AddMinutes(-5)} |
  Where-Object { $_.Properties[5].Value -eq 'reader1' } |
  Select-Object TimeCreated, @{n='LogonType';e={$_.Properties[8].Value}}, @{n='Package';e={$_.Properties[10].Value}}
```

| klist ticket for `HTTP/accessops.contoso.test` | 4624 package | Write |
|---|---|---|
| present | `Kerberos` | **Kerberos** |
| absent | `NTLM` | **NTLM** (then check: SPN missing? FQDN not in the intranet zone / `AuthServerAllowlist`? client off the domain network?) |
| anything else, or not correlated in time | | **undetermined** |

Repeat with a second browser policy or host as the instructor sets it (lab step 3), and record each
result separately. Never paste the `Authorization` value into the evidence.
