# ACL fixtures — what the demo needs, and what this machine could build

Script: `tools/Setup-Fixtures.ps1` (re-runnable; `-Remove` resets the ACLs as owner and deletes the
folder). One approved root, `AccessOpsData/confidential` (git-ignored, outside the web root):

| Folder / file | Catalog | Lab mode (`-PoolAccount`, `-LabUser`) | Local mode (default, this machine) |
|---|---|---|---|
| `labuser-only/DOC-201-salary-bands-2027.txt` (18 426 bytes) | DOC-201, tenant A | inheritance removed; Administrators + SYSTEM Full, **LabUser Read**, pool nothing | inherits the root: your account Full → readable by process and user token |
| `pool-only/DOC-202-vendor-contract-archive.txt` | DOC-202, tenant A | inheritance removed; Administrators + SYSTEM Full, **PoolAccount Read**, lab user nothing | **DENY read-data for your account's SID** → denied to process and user token |
| `labuser-only/DOC-203-audit-sample.txt` | DOC-203, tenant A (F03) | never created | never created |
| `tenant-b/DOC-204-legal-hold.txt` | DOC-204, tenant B | both may read (the *policy* must refuse) | inherits |
| root | — | inheritance removed; Administrators, SYSTEM, the person running the setup Full; pool and lab user `RX` on the root only | same, without the lab accounts |

## What the full ACL demo needs

Two **different** Windows accounts: the identity the application runs as (IIS application pool,
e.g. `IIS APPPOOL\AccessOps Intranet`) and the signed-in lab user (e.g. `LAB\dana.ortiz`), with
neither of them a local administrator on the file server (Administrators membership or inherited
rights would let one identity read both files and void the evidence).

## Why local mode is different (observed 2026-11-09)

- This machine has no second local account the sample may use (other accounts belong to people;
  their passwords are not lab material), and the sample creates no accounts.
- First attempt: separate the process token from the signed-in token by **logon type** (deny
  `NETWORK` S-1-5-2 on `pool-only`, deny `INTERACTIVE` S-1-5-4 on `labuser-only`). Result: inside the
  scope the token was `ITGLLC2\matte`, level `Impersonation`, **with the INTERACTIVE group** —
  NTLM loopback on one machine hands the server a token of the caller's own interactive logon
  session, so both tokens hit the same deny and DOC-201 was denied before *and* inside. Logon-type
  ACLs cannot stand in for a second account on localhost.
- Final local mode: `labuser-only` readable, `pool-only` denied to the account. The screen then shows
  the allowed read and the ACL denial as distinct outcomes (both verified), but not the flip between
  outside and inside the scope — that needs lab mode.

icacls after `Setup-Fixtures.ps1` (local mode, this machine):

```
...\confidential               ITGLLC2\matte:(OI)(CI)(F)  NT AUTHORITY\SYSTEM:(OI)(CI)(F)  BUILTIN\Administrators:(OI)(CI)(F)
...\confidential\pool-only     ITGLLC2\matte:(OI)(CI)(DENY)(RD)  ITGLLC2\matte:(I)(OI)(CI)(F)  SYSTEM:(I)(OI)(CI)(F)  Administrators:(I)(OI)(CI)(F)
...\confidential\labuser-only  ITGLLC2\matte:(I)(OI)(CI)(F)  SYSTEM:(I)(OI)(CI)(F)  Administrators:(I)(OI)(CI)(F)
...\confidential\tenant-b      (inherited from the root)
```
