# The Wisej.NET `impersonate` setting, `Application.StartTask`, HTTP and WebSocket (Module 3 deliverable 4)

**What is built here and what is not.** This sample targets `net10.0` only. The .NET Framework
pieces — `"impersonate": true` doing real impersonation, `Application.UserIdentity`,
`using (identity.Impersonate())`, `Net48ScopedRead.cs` — exist only in the `net48` build of
Wisej-4 and in .NET Framework (facts sheet, reflection). They are documented below for the .NET
Framework sandbox of the lab, **not built and not run here**. What *was* run here is the same
switch on .NET 10 (section C), which shows that the events fire and nothing is impersonated.

## A · The four switches on .NET Framework (they sound alike and do different things)

| Switch | Where | What it does |
|---|---|---|
| Windows Authentication role service | IIS (server) | lets IIS run the Negotiate/NTLM handshake at all |
| `<authentication mode="Windows" />` | System.Web `web.config` | ASP.NET takes the IIS Windows user as the request user (`LogonUserIdentity`) |
| `<identity impersonate="false" />` | System.Web `web.config` | **keep false**: `true` would impersonate the user on every ASP.NET request thread |
| `"impersonate": true` | Wisej `Default.json` (`Configuration.Impersonate`, default `false`, read once, cached process-wide) | Wisej raises `ImpersonationBegin`/`ImpersonationEnd` around each request thread and each `Application.StartTask` task; on net48 the default handling impersonates `Application.UserIdentity` and reverts |

The pack's `net48/windows-auth.config`:

```xml
<configuration>
  <system.web>
    <authentication mode="Windows" />
    <identity impersonate="false" />
    <authorization>
      <deny users="?" />
    </authorization>
  </system.web>
</configuration>
```

`net48/wisej-impersonate.json` (merged into `Default.json` of the sandbox only): `{ "impersonate": true }`.

The pack's scoped read for .NET Framework (`net48/Net48ScopedRead.cs`), with the identity taken from
`Application.UserIdentity` (the request's logon identity; net48 build only):

```csharp
internal static class Net48ScopedRead
{
    // Call after the application policy resolves an authorized server-side path.
    internal static byte[] Read(WindowsIdentity identity, string approvedPath)
    {
        if (identity == null || !identity.IsAuthenticated)
            throw new UnauthorizedAccessException();

        using (identity.Impersonate())
        {
            return File.ReadAllBytes(approvedPath);
        }
    }
}
```

Observer handlers, attached **once at start-up** (static, process-wide events), `Handled` left
false so Wisej cleans up what it started:

```csharp
Application.ImpersonationBegin += (s, e) => Log("ImpersonationBegin", WindowsIdentity.GetCurrent());
Application.ImpersonationEnd   += (s, e) => Log("ImpersonationEnd",   WindowsIdentity.GetCurrent());
```

Order (Wisej source, both builds; confirmed on net10 in section C): `ImpersonationBegin` is raised
**before** `ThreadBegin`, and `ImpersonationEnd` **before** `ThreadEnd` — around the request thread,
not "right after ThreadBegin" as older docs say.

## B · The comparison table for the .NET Framework sandbox (expected — fill from your run)

Each cell in a **fresh session**. "Effective" = `WindowsIdentity.GetCurrent().Name` /
`ImpersonationLevel` inside the handler or task body. Pool = `IIS APPPOOL\<pool>`, user = `LAB\dana.ortiz`.

| `impersonate` | Transport (`enableWebSocket`) | Code path | Effective identity (net48, expected) | Events |
|---|---|---|---|---|
| false | WebSocket | click handler | pool · None | none |
| false | WebSocket | `Application.StartTask` body | pool · None | none |
| false | HTTP only (`false`) | click handler | pool · None | none |
| false | HTTP only | `Application.StartTask` body | pool · None | none |
| **true** | WebSocket | click handler | **user · Impersonation** | Begin → ThreadBegin … End → ThreadEnd |
| **true** | WebSocket | `Application.StartTask` body | **user · Impersonation** (Begin/End around the task) | yes, per task |
| **true** | HTTP only | click handler | **user · Impersonation** | yes, per request |
| **true** | HTTP only | `Application.StartTask` body | **user · Impersonation** | yes, per task |
| any | any | raw `Task.Run` body | pool · None | **none** — `Task.Run` gets nothing |
| true, a handler sets `e.Handled = true` | any | click | pool · None (default suppressed) | yes |

Over WebSocket the `UserIdentity` Wisej impersonates is the one captured for the session's socket;
its token lifetime belongs to IIS — record what you see rather than relying on it (lesson).

## C · Observed here: the same switch on .NET 10 (2026-11-09)

`Default.json` temporarily set to `"impersonate": true` (and once also `"enableWebSocket": false`),
Debug build, `Identity/ImpersonationObserver.cs` attached once in `Startup.cs`, click on DOC-201 (the
click runs the scoped read and, in Debug, an `Application.StartTask` observer). Then restored to the
shipped `Default.json` (no `impersonate` key = false).

| `impersonate` | Transport | Code path | Effective identity in the handler/task | Events observed |
|---|---|---|---|---|
| true | WebSocket | page loads and resource requests | `ITGLLC2\matte` level=None (process) | `ImpersonationBegin` → `ThreadBegin` → `ImpersonationEnd` → `ThreadEnd`, `Handled=False`, every request |
| true | WebSocket | click (scoped read) | process, level=None — except **inside** `RunImpersonated`: level=Impersonation | same order, thread 14, transport WebSocket |
| true | WebSocket | `Application.StartTask` body | process, level=None; `Application.User` = NTLM, `IsAuthenticated` True | its own Begin/ThreadBegin/End/ThreadEnd on another thread (9), transport WebSocket |
| true | HTTP only | click (scoped read) | process, level=None outside the scope; the scoped read worked (`Read 18 KB`) with the ajax request's token | Begin/ThreadBegin/End/ThreadEnd per request |
| true | HTTP only | `Application.StartTask` body | process, level=None; **`Application.User` anonymous** (`(none)`, `IsAuthenticated` False) — the request that started it had ended | events fire |
| false (shipped) | WebSocket | everything | process, level=None | none raised |

So on .NET 10: the events fire (in the documented "around the thread" order) and **nothing is
impersonated by default**; modern .NET has no `WindowsIdentity.Impersonate()` to open in Begin and
close in End, so the read needs its own `RunImpersonated` scope (lesson review question 2).

Log excerpt (logger `AccessOps.ImpersonationObserver`):

```
ImpersonationBegin thread=14 transport=WebSocket handled=False effective=ITGLLC2\matte level=None
ThreadBegin        thread=14 transport=WebSocket handled=-     effective=ITGLLC2\matte level=None
ImpersonationBegin thread=9  transport=WebSocket handled=False effective=ITGLLC2\matte level=None   <- StartTask
ThreadBegin        thread=9  transport=WebSocket handled=-     effective=ITGLLC2\matte level=None
ImpersonationEnd   thread=9  ...
ThreadEnd          thread=9  ...
identity-trace doc=DOC-201 step=Inside ... level=Impersonation outcome=Read 18 KB
ImpersonationEnd   thread=14 transport=WebSocket handled=False effective=ITGLLC2\matte level=None
ThreadEnd          thread=14 ...
```

## D · The modern request-endpoint path (lab step 4)

The sample's reader runs in the Wisej click with the **current request's** `HttpContext`
(`IHttpContextAccessor`): over WebSocket that is the socket's upgrade request, authenticated by the
same Negotiate handshake (observed `/ws.wx 401 → 401 → 101`), and its token was live for every read
here. `Application.User` on that click is *not* usable for this: it is the page-load identity and was
already disposed in the browser pane (`ObjectDisposedException`, Module 2 README). The alternative
the lesson names — an authorized ASP.NET Core endpoint mapped before `app.UseWisej()` that streams
the file — keeps the token's lifetime equal to one HTTP request; use it when the read must not
depend on the socket.
