# Redacted challenge trace (Module 2 deliverable 2)

Format, as the walkthrough's trace panel: **scheme and status only**. An `Authorization` value is a
credential and a `WWW-Authenticate` continuation carries a security token, so both are written as
`Negotiate [redacted]`; nothing else from a header is ever kept. The application writes these lines
itself (`AccessOps/Security/ChallengeTrace.cs`, logger `AccessOps.Security.ChallengeTrace`), for every
request that carries an `Authorization` header or receives a `WWW-Authenticate` header.

## Observed on this machine — 2026-11-09, Kestrel + Negotiate, `http://localhost:6402`

Client: the Claude desktop browser pane (Chromium), signed in to Windows as `ITGLLC2\matte`
(local account, workgroup). **The pane completed the challenge silently** — no prompt; Chromium
treats `localhost` as an intranet host and answers Negotiate with the logged-on user's credentials.

```
challenge GET / request=(no Authorization header) -> 401 offered=Negotiate identity=anonymous
challenge GET / request=Negotiate [redacted] -> 401 offered=Negotiate [redacted] identity=anonymous
challenge GET /Default.html request=Negotiate [redacted] -> 200 offered=Negotiate [redacted] identity=NTLM · ITGLLC2\ma•••e
challenge GET /resource.wx/qx.js request=(no Authorization header) -> 401 offered=Negotiate identity=anonymous
challenge GET /resource.wx/qx.js request=Negotiate [redacted] -> 401 offered=Negotiate [redacted] identity=anonymous
challenge GET /resource.wx/qx.js request=Negotiate [redacted] -> 200 offered=Negotiate [redacted] identity=NTLM · ITGLLC2\ma•••e
trace point=Startup · Program.Main transport=HTTP load ... authType=NTLM isAuthenticated=True appSubject=acct …7f3a · tenant A host=NTLM · ITGLLC2\ma•••e
challenge GET /ws.wx request=(no Authorization header) -> 401 offered=Negotiate identity=anonymous
challenge GET /ws.wx request=Negotiate [redacted] -> 401 offered=Negotiate [redacted] identity=anonymous
trace point=Click · WebSocket (Open) transport=WebSocket ... authType=NTLM isAuthenticated=True
```

| Request | Scheme sent | Status | Offered | Meaning |
|---|---|---|---|---|
| 1 `GET /` | none | **401** | `Negotiate` | host challenge (fixture **F01**, first half). `Program.Main` did not run. |
| 2 `GET /` | `Negotiate [redacted]` (SPNEGO-wrapped NTLM negotiate message) | 401 | `Negotiate [redacted]` | NTLM is a three-leg handshake: the server sends its challenge |
| 3 `GET /Default.html` | `Negotiate [redacted]` | **200** | `Negotiate [redacted]` (final server token) | authenticated: `NTLM · ITGLLC2\ma•••e` (F01, second half) |
| every new TCP connection (`qx.js`, `loader.svg`, …) | same three legs | 401, 401, 200 | | NTLM is per connection; later requests on an authenticated connection carry no header and are not traced |
| WebSocket upgrade `GET /ws.wx` | same legs | 401, 401, then the upgrade | | the browser pane authenticates the WebSocket upgrade too; Wisej used WebSocket (`transport=WebSocket` on the click) |

Facts from this run:

- Kestrel's Negotiate handler offers **only** `WWW-Authenticate: Negotiate` (IIS with the provider list
  Negotiate, NTLM sends two headers: `Negotiate` and `NTLM`).
- The 200 response also carries a `WWW-Authenticate: Negotiate <token>` (the server's final token).
  It is redacted like the others.
- `Application.User.Identity.AuthenticationType` inside Wisej.NET is `NTLM` (not `Negotiate`).

### Command-line clients (same server, same day)

```
PS> Invoke-WebRequest http://localhost:6402/                             -> 401, WWW-Authenticate: Negotiate
PS> Invoke-WebRequest http://localhost:6402/ -UseDefaultCredentials      -> refused by PowerShell 7 on http:
    "The cmdlet cannot protect plain text secrets sent over unencrypted connections"
PS> Invoke-WebRequest http://localhost:6402/ -UseDefaultCredentials -AllowUnencryptedAuthentication -> 200
$ curl.exe --negotiate -u : http://localhost:6402/                       -> 200   (curl 8.21.0, Schannel/SSPI)
$ curl.exe http://localhost:6402/                                         -> 401
$ curl.exe -D - -o NUL http://localhost:6402/                             -> HTTP/1.1 401 Unauthorized
                                                                             WWW-Authenticate: Negotiate
$ curl.exe --negotiate -u : http://localhost:6402/Default.json           -> 404 (authenticated, .json never served)
```

## Lab environment template (IIS, CONTOSO, Edge)

Fill from Edge DevTools → Network (Headers tab: copy the scheme word only) and the IIS log:

| Request | Scheme sent | Status | Offered | Identity (diagnostics view) |
|---|---|---|---|---|
| 1 `GET https://accessops.contoso.test/` | none | 401 | `Negotiate`, `NTLM` | — |
| 2 same URL | `Negotiate [redacted]` | 200 (Kerberos: one leg) or 401 → 200 (NTLM: two legs) | | `NTLM`/`Negotiate`/`Kerberos` · `True` · `CONTOSO\re•••1` |

A Negotiate header followed by 200 proves a host challenge followed by authentication — nothing
about Kerberos (`provider-evidence.md`).
