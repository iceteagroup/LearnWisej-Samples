# Forced-failure reversion trace — fixture F03 (Module 3 deliverable 3)

F03: an **approved** document whose file is missing. The policy allows it, the catalog resolves a
canonical path under the approved root, the read inside `WindowsIdentity.RunImpersonated` throws
`FileNotFoundException`, the exception escapes the scope, `ScopedDocumentReader` records the after
row in `finally`, and the caller (`DocumentsView.OpenDocument`) catches it and shows the plain
missing-file message. The denied-ACL case (DOC-202) uses a file that **exists**, so "not found"
never masquerades as "not allowed".

## Observed — 2026-11-09, Chromium browser, `http://localhost:6403`, click on DOC-203

App log (logger `AccessOps.IdentityTrace`, SID shortened, path is the server's):

```
identity-trace doc=DOC-203 step=Before appSubject=acct …7f3a subject=s-d16a4174a75e request=DEVBOX\dev requestSid=S-1-5-21-…-1007
  effective=DEVBOX\dev effectiveSid=S-1-5-21-…-1007 level=None logon=INTERACTIVE outcome=File missing
  detail=baseline open as process: FileNotFoundException: Could not find file '…\Module 3\AccessOpsData\confidential\labuser-only\DOC-203-audit-sample.txt'.
identity-trace doc=DOC-203 step=Inside ... effective=DEVBOX\dev level=Impersonation logon=INTERACTIVE outcome=FileNotFoundException
  detail=scoped read threw FileNotFoundException: Could not find file '…\labuser-only\DOC-203-audit-sample.txt'.
identity-trace doc=DOC-203 step=After  ... effective=DEVBOX\dev level=None logon=INTERACTIVE outcome=Reverted
  detail=same token as before: True
audit subject=s-d16a4174a75e action=scoped-read resource=A / DOC-203 decision=allowed correlation=ed81e9188198
audit subject=s-d16a4174a75e action=file.read  resource=DOC-203 decision=failed reason=missing correlation=ed81e9188198
```

Screen: trace rows `DOC-203 F03 · Before · File missing`, `DOC-203 F03 · Inside ·
DEVBOX\dev · Impersonation · FileNotFoundException`, `DOC-203 F03 · After · DEVBOX\dev ·
Reverted`; status line (amber) "DOC-203 could not be opened. The file is missing; the request was
logged." — no path, SID or exception text on screen.

Reading: the after row has `ImpersonationLevel = None` and the same SID and logon group as the before
row (`same token as before: True`), i.e. the thread is back on the process token although the
delegate threw. The same holds for the ACL exception (DOC-202) and for the normal return (DOC-201).

## In the lab environment

The before and after rows read `IIS APPPOOL\AccessOps Intranet` (level None) and the inside row
`LAB\dana.ortiz · Impersonation`: the reversion is then visible in the name too, not only in the level.
