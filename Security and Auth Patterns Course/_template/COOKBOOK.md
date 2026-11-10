# AccessOps cookbook - Wisej-4 4.1.7 / .NET 10

Everything here was established by building and running the AccessOps lab samples of the
**Security & Auth Patterns** course against the public `Wisej-4` / `Wisej-4-WebAuthn` **4.1.7**
packages from nuget.org on .NET 10 (Windows 11, workgroup PC, Kestrel), and reading the result back.
Each entry says how. "M*n*" names the module whose sample established it; "earlier edition" marks a
fact from the previous version of these samples (same packages, re-checked where noted).

## Structure and ports

- One solution per module: `Module N/AccessOps.slnx` with `AccessOps/` (`Microsoft.NET.Sdk.Web`,
  `net10.0`, namespace `AccessOps`), `PolicyHarness/` (console, no Wisej.NET), from M4
  `AccessOps.Tests/` (xUnit), from M5 `AccessOps.TestIdentityProvider/` (a local OpenID Connect test
  double - never deploy). Written lab deliverables in `Module N/docs/`.
- Ports: app **640N** (M1-M3 `http://localhost:640N`; M4-M7 `https://localhost:640N`), test IdP
  **641N** (M5 6415, M6 6416, M7 6417), the Development-only http listener **644N** (M4-M7). M7 runs
  one build twice: partner 6407 (+6447), intranet 6408.
- The curriculum forks after M1: intranet chain M2 → M3 (Windows sign-in, impersonation), partner
  chain M4 → M5 → M6 (own sign-in, OpenID Connect, passkeys); M7 = M6 with M3's route ported in, one
  process per route (`AccessOps:Route = Intranet | Partner`).
- Runtime data in `Module N/AccessOpsData/` beside the project folder, so outside the web root
  (`WebRootPath = "./"` is the project folder); the course `.gitignore` excludes it and `*.db*`.

## Packages

- `Wisej-4` **4.1.7** has **no** `Wisej.Web.Passkey`, `Wisej.Web.OAuthClient` or
  `IPasskeyCredentialStore` (Wisej.NET 4.2). Reflection over `Wisej.Framework.dll` 4.1.7.0:
  `GetType(...)` returns null for all three (earlier edition). Federation is ASP.NET Core's OpenID
  Connect handler; passkeys are the `Wisej-4-WebAuthn` 4.1.7 extension.
- `Microsoft.Data.Sqlite` **10.0.14**: `10.0.0` pulls `SQLitePCLRaw.lib.e_sqlite3` 2.1.11 (restore
  warning NU1903); 10.0.14 restores clean (earlier edition).
- `Microsoft.AspNetCore.Authentication.Negotiate` / `.OpenIdConnect` **10.0.14** run beside
  `app.UseWisej()` (M2, M5, M7 builds: 0 warnings).
- `GenerateAssemblyInfo=false` (the Wisej.NET template default) drops the generated `UserSecretsId`
  attribute: `dotnet user-secrets set` wrote the value but `WebApplication.CreateBuilder` never loaded
  it. `GenerateAssemblyInfo=true` fixes it (earlier edition; M5-M7 rely on it).
- `ForwardedHeadersOptions.KnownIPNetworks` (`System.Net.IPNetwork`) exists on .NET 10 and compiles
  without an obsolete warning; M7 clears `KnownProxies` and `KnownIPNetworks` before adding the
  configured ones (M7 build).

## Session and principal

- **Startup order (M1):** `Program.Main` runs before `ApplicationStart` on the first HTTP load; both
  see `Application.User` with AuthenticationType "(none)" and `IsAuthenticated` false on Kestrel
  without host authentication. Logged.
- **Rotating the session ID (M1, M4, M7):** verify on the click, park, `Application.Reload()`, then in
  `ApplicationRefresh` call `Application.RefreshSessionId()` before `Application.User = …`. Observed:
  the ID changed, `sessionStorage["Wisej.SessionId"]` adopted it, the principal and
  `Application.Session` survived, a later F5 kept the same ID. From a WebSocket click the server
  switched IDs but the browser never adopted it (the next request created a new session) - earlier
  edition, re-confirmed in M1.
- **The old ID is refused (M7):** in the Chromium browser the anonymous ID was saved, the user signed in
  (ID rotated), the old ID was written back into `sessionStorage["Wisej.SessionId"]` and the tab
  reloaded: the tab got a third, new ID and the anonymous sign-in page.
- `Application.StartTask` (M1): `Application.User` returns the session override and `IsWebSocket` is
  true when started from a click; `Application.Update(this, action)` from inside updates the UI.
- Raw `Task.Run` (M1): `Application.SessionId` empty, AuthenticationType "(none)", `IsWebSocket`
  false - but `Application.Update(component, action)` still updated the right session's UI and
  `Application.Download` inside it worked.
- `IHttpContextAccessor.HttpContext` (after `AddHttpContextAccessor()`) is non-null during Wisej
  WebSocket events and inside a raw `Task.Run` started from one (ASP.NET Core's AsyncLocal flows,
  Wisej's session does not) (M1).
- **Application principal on the intranet route (M7):** `ApplicationPrincipal.Establish` in
  `Program.Main` replaces the host's `WindowsIdentity` as `Application.User` for the session; later
  WebSocket events see the claims principal (no disposed Windows token), while the scoped read still
  takes the live request token from `IHttpContextAccessor`. Observed in the review (cases 1, 4, 5).
- `Application.Exit()` with `"autoReload": true` reloads the tab into a new session; without it the
  4.1.7 client calls `window.close()` (earlier edition). `await Application.CallAsync(js)` waits for a
  JS promise (a fetch); `Application.Exit()` afterwards works (M4).
- Each browser tab is its own session; `sessionStorage.clear(); location.reload()` starts a fresh
  one (M1). Navigating a tab to the same URL kept its session (M7: a signed-in partner tab re-attached
  after the partner route was switched off and on).

## Negotiate on Kestrel (intranet route)

- `AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate()` +
  `FallbackPolicy = DefaultPolicy` + `UseAuthentication`/`UseAuthorization` before `UseWisej()`
  challenges every Wisej request and static file; an anonymous request never reaches `Program.Main`
  (M2, logged).
- Kestrel Negotiate offers only `WWW-Authenticate: Negotiate`. NTLM costs 401 → 401 → 200 on every new
  connection; the 200 carries a `Negotiate <token>` header (redact it). Wisej's WebSocket upgrade
  `/ws.wx` goes through the same handshake (401, 401, 101) (M2, challenge trace).
- `Application.User.Identity` is a `WindowsIdentity` with AuthenticationType "NTLM" in `Program.Main`
  on a workgroup PC (M2); `new HostFaultPage().Show()` from `Program.Main` makes it the main page (M2).
- On a WebSocket event `Application.User` (without an app principal) is the identity captured on the
  page load; its token belongs to the connection that signed in - in the embedded browser `Name`/`User` threw
  `ObjectDisposedException` on the first click (M2). Map the SID once in `Program.Main`.
- Clients: the Chromium browser (Chromium) answers Negotiate for `http://localhost` silently; headless
  Chrome needs `--auth-server-allowlist=localhost`; `curl.exe --negotiate -u :` works; PowerShell 7
  `Invoke-WebRequest -UseDefaultCredentials` over http needs `-AllowUnencryptedAuthentication` (M2).
  A .NET `HttpClient` with `SocketsHttpHandler { Credentials = CredentialCache.DefaultCredentials }`
  got 200 from the intranet route of the same process; without credentials 401 `Negotiate` (M7, case 1).
- The Release publish ships `Microsoft.AspNetCore.Authentication.Negotiate.dll` and generates a
  `web.config` with `hostingModel="inprocess"` (M2, M7).

## Impersonation (`WindowsIdentity.RunImpersonated`)

- `IHttpContextAccessor.HttpContext` on a WebSocket event is the socket's upgrade request with a live
  `WindowsIdentity`; `RunImpersonated(identity.AccessToken, …)` works with it. In HTTP-only mode the
  ajax request's identity works the same way (M3).
- It reverts on return and on throw (ACL exception and `FileNotFoundException`): the after row equals
  the before row (M3 trace; M7 case 4 compares name and `ImpersonationLevel` before and after).
- On localhost the NTLM loopback token belongs to the caller's interactive logon session (same SID,
  INTERACTIVE group, level Impersonation): ACLs cannot separate the pool from the user on one machine;
  the "denied before, allowed inside" flip needs two accounts (M3).
- .NET 10 with `"impersonate": true`: `ImpersonationBegin` → `ThreadBegin` → `ImpersonationEnd` →
  `ThreadEnd` (`Handled=False`) fire for HTTP requests, WebSocket events and `StartTask`, WebSocket on
  and off; the effective identity stays the process (level None) (M3, logged). In a `StartTask` from an
  HTTP-only click `Application.User` is anonymous; from a WebSocket click it is still NTLM (M3).
- `Default.json` is read from the project folder (`WEBSITE_PATH`); editing it and restarting changes
  `impersonate` and `enableWebSocket` (M3).
- Re-running an `icacls` fixture script needs an owner `/reset` of the folder first; without it
  `Set-Content` truncated the denied file to 0 bytes and then failed (M3).
- A UNC path (`\\files.contoso.test\…`) is refused in the catalog before any scope when it is not an
  approved delegation target - no SMB timeout, 0 file calls (M7 case 5).

## Cookies, antiforgery, origin, ingress

- An origin guard placed before `UseWisej()` cannot use `HttpContext.WebSockets.IsWebSocketRequest`
  (false there: the WebSockets middleware is added later); check the `Upgrade: websocket` header (M4).
  M7 case 8: an upgrade with `Origin: https://evil.example` → 403.
- Endpoints mapped "before `UseWisej()`" run at the end of the pipeline (WebApplication appends the
  endpoint middleware last). In TestHost a terminal `app.Run` hides them; use `MapFallback("{**path}")` (M4).
- ASP.NET Core refuses to issue an antiforgery cookie with `SecurePolicy.Always` on a plain HTTP
  request; Chrome does accept the Secure auth cookie on `http://localhost` (M4).
- `POST /auth/cookie` without an antiforgery token → 400; `/auth/continue?returnUrl=https://evil.example/…`
  → 302 `/` (M7 case 8, real HTTP).
- **Cookies on localhost are shared across ports.** The M7 partner route received the M6 sample's
  `AccessOps.Antiforgery` cookie (protected with M6's key ring) and logged `The key {…} was not found in
  the key ring` before issuing a fresh token (M7 log). Harmless here; give samples distinct cookie names
  if they must run side by side.
- `Application.Cookies[name]` reads an HttpOnly cookie written by ASP.NET Core on the next page load;
  `Application.Cookies.Add(…, secure, httpOnly, Wisej.Base.Cookie.SameSiteMode.Strict)` works from a page
  load (earlier edition, M5).
- With `app.UseWhen(cx => !cx.Request.Path.Value.EndsWith(".json", …), a => a.UseFileServer())`:
  `/Default.json`, `/appsettings.json` and `/ingress.json` → 404 (M7 curl).
- **Ingress switch (M7):** a JSON file added with `AddJsonFile(…, reloadOnChange: true)` and read per
  request by the first middleware of the partner pipeline; writing `"PartnerRoute": "Off"` (temp file +
  `File.Move(overwrite)`) made the route answer 503 within **0.3 s**, without a restart; "On" brought it
  back. Existing Wisej sessions were not ended by it.
- The 4.1.7 client honours `"secure": true`: loaded over `http://localhost` it navigated itself to
  https (earlier edition). M7 writes it into the **published** Default.json only
  (`tools/Publish-Release.ps1`).

## OpenID Connect handler (partner route)

- With `ResponseMode = Query` the handler sends no `response_mode` parameter (M5); the default is
  `form_post`. Correlation and nonce cookies must be `Lax` (cross-site top-level callback); the hand-off
  cookie works as `Strict` (earlier edition).
- At Warning level the handler logs the `.AspNetCore.Correlation.<id>` cookie name; set
  `Microsoft.AspNetCore.Authentication` to Error for a clean log (M5).
- In tests the handler uses `ConfigurationManager`, not `Options.Configuration`: set a
  `StaticConfigurationManager` (M5).
- A forged callback `/signin-oidc?code=…&state=…` ends in `OnRemoteFailure` (`Unable to unprotect the
  message.State.`) → 302 `/Default.html?signin=failed`, 0 grants (M5; M7 case 10 over real HTTP).
- **Restart mid-flow, shared key ring (M7):** the tab waited at the provider's login form while the
  partner process was stopped and started again; the callback validated on the new process (keys
  persisted with `PersistKeysToFileSystem` + `SetApplicationName`), the hand-off came from the shared
  store, one grant, session rotated.
- **Restart mid-flow, different key ring (M7):** the same with `DataProtection:KeyPath` pointing at a
  new folder: `OnRemoteFailure` classified `state`, the fixed failure sentence, 0 grants. A key ring per
  node breaks every callback that lands on the other node.
- `ShowDialogAsync` buttons with a `DialogResult` do not close the dialog; call `Close()` (M4).

## WebAuthn extension (`Wisej-4-WebAuthn` 4.1.7)

- **Bug 1 - user verification:** `wisej.ext.WebAuthn.js` reads the UV flag from bit 1 instead of bit 2,
  so `AuthenticatorData.UserVerified` is false for real authenticators. Workaround: read UP / UV / BE
  from the flags byte of `AuthenticatorData.Base64` (`AuthenticatorFlags.Read`) (M6: virtual
  authenticator in headless Chrome + unit tests).
- **Bug 2 - ES256:** `WebAuthn.Validate` rejects genuine ES256 signatures whose r or s is shorter than
  32 bytes (about 1 in 128). Workaround: re-encode r and s as 33-byte DER integers before calling it
  (`Identity/Es256Signature.cs`); 2,000 signed assertions pass afterwards (M6 test).
- Since 2026-11-10 the course's Module 6 lesson, lab guide and video teach both workarounds
  (`AuthenticatorFlags.Read`, `Es256Signature.ForValidate`), so they are no longer a sample-vs-lesson
  difference; drop them when a fixed extension ships. Reported upstream as Standard-Extensions issue
  #3026 and fixed in the extension's source on 2026-11-10 (UV from bit 2, DER r/s padded, rp.id and
  "cross-platform" sent correctly, UTF-8 user handle, extension outputs after the COSE key accepted);
  the fix ships with a package version later than 4.1.7.
- `CredentialsResponse` and its parts have public setters (synthetic responses in tests);
  `PublicKey(json)` accepts `{CredentialID, Data{"1","3","-1","-2","-3"}}`; `System.JSON.Stringify`
  round-trips it; the counter after `CreateAsync` over the virtual authenticator is 1;
  `IsUserVerifyingPlatformAuthenticatorAvailableAsync` returned true with a virtual authenticator and in
  the embedded browser on a Windows Hello PC (M6).
- `WebAuthn.GetAsync` sends no RP ID: register and sign in on the same host (M6).
- M7 case 11 (software authenticator, real `PasskeyService`): valid → `Accepted`, replay →
  `ChallengeRejected`, account A's credential with account B's user handle → `NoMatchingKey`, deleted
  credential → `NoMatchingKey`; grants +1.

## Shared store across processes and nodes (M7)

- Two processes (the two routes) open one SQLite file. Eight parallel connections redeeming one handle
  with `DELETE … RETURNING` failed with `SQLite Error 5: 'database is locked'` until each connection
  ran `PRAGMA busy_timeout = 5000` (and the file was switched to `journal_mode = WAL`); then exactly one
  redeem won (test `F12_of_two_nodes_racing_for_one_handle_exactly_one_wins`, run 3x).
- `DELETE … RETURNING payload, expires_at` (Microsoft.Data.Sqlite 10.0.14) is a single-use consume:
  hand-off handles (hashed) and passkey challenges moved from process memory to the store this way; a
  second "node" (a new store instance) resumes a parked hand-off once and refuses a consumed one
  (tests + review case 12).
- `PersistKeysToFileSystem` without a protector logs `No XML encryptor configured. Key {…} may be
  persisted to storage in unencrypted form.` at startup - protect the ring in production.
- `Microsoft.Data.Sqlite` turns foreign keys on: seed parents first (earlier edition, all modules).

## Logging and correlation

- Over the WebSocket ASP.NET Core's scope is per connection (`RequestPath: /ws.wx`, one `TraceId` for
  many clicks): a per-click correlation ID must be your own `ILogger.BeginScope` (earlier edition).
- With `Logging:Console:IncludeScopes = true`, a **dictionary** scope state printed as
  ``System.Collections.Generic.Dictionary`2[…]`` by the simple console formatter; a message-template
  scope `BeginScope("Correlation={Correlation}", label)` printed `=> Correlation=rr-07-…` (M7).
- In Wisej async click handlers, log lines written **after** some `await`s carried no scope at all -
  not even a scope opened after the await (rollback steps after `await Task.Delay`, review case 8 after
  three `await`ed HTTP calls in one run but not in another). The audit rows kept their correlation ID
  because AccessOps passes it explicitly. Carry the correlation ID in your own objects across awaits
  (M7, observed; cause not isolated).

## Grid and UI gotchas

- A bound `DataGridView` sends only the properties that have a column; a hidden column (`Visible =
  false`) **is** serialized to the browser (earlier edition, read back from the client data model).
- `DataGridViewColumn.AllowHtml` renders fixed pill markup; `DataGridViewCellStyle` has no
  `SelectionBackColor`/`SelectionForeColor`; `CellBorderStyle = Horizontal` and
  `ShowColumnVisibilityMenu = false` exist (M1).
- `UserControl` has no `AcceptButton`; `TextBox.InputType.Type = TextBoxType.Password` works (M1).
- Default control `Margin` (3) misaligns the first button in a `FlowLayoutPanel` against siblings with
  `Margin(8,0,0,0)`; set `Margin = 0`. Three 28-px glyph labels in a 96-px FlowLayoutPanel wrapped until
  `Margin = 0` (M1).
- A one-row `TableLayoutPanel` of bordered cards clipped the cards' bottom border until a
  `RowStyle(SizeType.Percent, 100)` was added (M7 screenshot before/after).
- `MessageBox.Show` / `AlertBox.Show` default `allowHtml` to true: pass `allowHtml: false` whenever the
  text can contain input (earlier edition; M6-M7 use it). An `AlertBox` appears bottom-right and closes
  itself after a few seconds (M7 screenshot).
- A grid re-bound with `DataSource = …` loses its current row; set `CurrentCell` again (earlier edition).
- `#if DEBUG` around a page type and its only entry removes it from `dotnet publish -c Release` (M1:
  byte search of the dll). **But** string literals passed to a method whose body is `#if DEBUG` stay in
  the Release dll: the demo password `"Reports-demo-31"` passed to `DemoHash(hasher, "…")` was found in
  the M7 Release dll (UTF-16 search) although Release seeds no hash - the M4-M6 seeds use the same
  call shape (not re-checked there). M7 moved the literals inside the `#if DEBUG` block; the publish
  check then found 0 hits.

## Embedded-browser testing

- The embedded browser is small (screenshots ~314x202 for a 1400x900 viewport): read state with `get_page_text`,
  `find`, `read_page` and `javascript_tool`; use headless Chrome (CDP, 1400x900) for screenshots and
  `Input.dispatchMouseEvent` at an element's `getBoundingClientRect()` centre to click Wisej buttons
  (M7: the review's toolbar buttons were not returned by `find`).
- A background browser tab does not render a reloaded Wisej page until it is fronted (`tabs_select`) (M4).
- The embedded browser does not trust the HTTPS dev certificate (`ERR_CERT_AUTHORITY_INVALID`); M4-M7 offer the
  Development-only `AccessOps:LocalHttpVerification` listener on `http://localhost:644N` (refused outside
  Development); passkeys then need that origin in `WebAuthn:AllowedOrigins` and the IdP its redirect URI.
- The embedded browser has a five-server limit for `preview_start`; start extra servers with `dotnet run --no-build`
  and an explicit `--urls`. Two routes of one project: build once, run both with `--no-build`, or the
  second build fails on the locked dll (M7).
- Stopping a process by command-line match must exclude the shell that runs the match (the M7 run's
  pattern also matched the bash processes executing it).
