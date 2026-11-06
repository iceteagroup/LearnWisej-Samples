# AccessOps · Security & Auth Patterns · Module 2

Local lab build for **Module 2 · Authentication and Session Identity**. The AccessOps sign-in
page from the walkthrough - *Sign in to AccessOps*, the two credential fields, **Sign in**,
*Remember this device for 14 days*, one message strip and the `SERVER` footer - in front of a
signed-in console that only exists once a `SessionContext` does.

> **Wisej.NET version.** The lesson also teaches passkey enrolment and sign-in with the
> `Wisej.Web.Passkey` component and an `IPasskeyCredentialStore`, which are new in Wisej.NET 4.2.
> These samples target the released **Wisej-4 4.1.7**, which has neither, so **passkeys are not
> implemented** and the login screen shows no passkey control. Everything else in the lab is.

## Run it

```bash
cd "Security and Auth Patterns Course/Module 2/AccessOps"
dotnet run -f net10.0 --urls http://localhost:6402
```

Open <http://localhost:6402>. The first run creates `Module 2/AccessOpsData/accessops.db` and
hashes the demo accounts from [Data/seed-credentials.json](AccessOps/Data/seed-credentials.json)
(`r.almeida` and `k.osei`; the demo password is in that file). Delete `AccessOpsData` to start
again - the lockout counters live in process memory and reset when the app restarts.

## The screen

- **Sign in** - `txtUserName`, `txtPassword`, `btnSignIn` (*Checking…* while the hash runs),
  `chkRemember`, the message strip and the footer, which reads
  `PasswordService · PBKDF2-SHA256 · 210,000 iterations` and, after a failure,
  `LoginThrottle · failures N of 5`.
- **Console** - `AccessOps · Console`, the navigation column (*Dashboard, Employees, Documents,
  Approvals, Admin console*), the three figures, *Documents visible to this session*, the
  signed-in name and **Sign out**. For an employee who manages nobody the admin entry is greyed and
  the note says why.

## What to try

| Action | Expected result |
|---|---|
| Five wrong passwords for `r.almeida` (one of them with the name mistyped) | The same sentence every time: *That user name and password combination is not valid.* The footer counts `failures N of 5` |
| Then the **correct** password | Still the same sentence. The audit row says `LockedOut` - refused before the hash is computed |
| Sign in as `k.osei` | The page reloads once and the console opens. `sessionStorage["Wisej.SessionId"]` is different from before the sign-in |
| Sign in with *Remember this device* ticked, then clear the tab's sessionStorage and reload | A new session opens straight on the console; audit row `RememberedDevice`. `document.cookie` is empty - the cookie is HttpOnly |
| **Sign out** | The tab reloads into a new session: *You have signed out. Sign in to continue.* Audit row `SignOutExplicit`; the device token is revoked |
| Leave the console idle past `sessionTimeout` (1200 s; set 60 to try it quickly) | No prolong dialog. The tab lands on the login page: *Your session ended after 20 minutes of inactivity.* Audit row `SignOutIdle` |

## Lab tasks → where in the code

| Deliverable | Where |
|---|---|
| Credential columns (algorithm, iterations, salt, hash - no password) | [AccessOpsDatabase.cs](AccessOps/Infrastructure/AccessOpsDatabase.cs) |
| `PasswordService` Hash / Verify, PBKDF2-SHA256, 210,000 iterations, `FixedTimeEquals` | [PasswordService.cs](AccessOps/Services/PasswordService.cs) |
| `SessionContext` and the typed accessor that throws when nobody is signed in | [SessionContext.cs](AccessOps/Services/SessionContext.cs), [WisejSession.cs](AccessOps/Infrastructure/WisejSession.cs) |
| `LoginThrottle` (per account and per address, 5 failures → 15 minutes) | [LoginThrottle.cs](AccessOps/Services/LoginThrottle.cs) |
| `LoginAuditService` (one row per attempt and per sign-out, true outcome) | [LoginAuditService.cs](AccessOps/Services/LoginAuditService.cs) |
| `LoginService.SignIn` - throttle, verify (dummy credential for unknown names), audit, `RefreshSessionId`, context | [LoginService.cs](AccessOps/Services/LoginService.cs) |
| Login page that only calls `LoginService`, one generic sentence | [LoginPage.cs](AccessOps/Pages/LoginPage.cs) |
| `sessionTimeout: 1200`, `Application.SessionTimeout` with `e.Handled = true` | [Default.json](AccessOps/Default.json), [Program.cs](AccessOps/Program.cs) |
| One Logout routine for the button and the timeout: audit, clear, `Application.Exit()` | [SignOutService.cs](AccessOps/Services/SignOutService.cs), [SignOutFlow.cs](AccessOps/Pages/SignOutFlow.cs) |
| Remember-me: hashed, expiring, revocable device token in a Secure, HttpOnly, SameSite=Strict cookie | [RememberedDeviceService.cs](AccessOps/Services/RememberedDeviceService.cs), `WritePendingDeviceCookie` in [Program.cs](AccessOps/Program.cs) |
| Login audit records | [docs/LoginAuditEvidence.md](docs/LoginAuditEvidence.md) - the real rows from the browser check |

## Verified in the browser pane

With `accessops-m2` at 1400x900: the five refusals and the lockout (audit rows 1-7 in
[docs/LoginAuditEvidence.md](docs/LoginAuditEvidence.md)), the session ID change across sign-in,
the remembered-device sign-in in a new session, explicit sign-out to the login page with its
sentence, and the idle timeout with `sessionTimeout` temporarily set to 60 (fired after 61 s, then
restored to 1200). No server errors in `preview_logs`.

## Notes for anyone extending the login

- **`RefreshSessionId()` must run on a page load in 4.1.7.** Called from the click handler, the
  browser never adopted the new ID and the next request started an empty session. The sign-in
  therefore parks the verified context, reloads, and on the page load renews the ID and only then
  stores the context. See `_template/COOKBOOK.md`.
- **`Application.Exit()` closes the tab unless `autoReload` is on.** Default.json sets
  `"autoReload": true`; the reason for the next page travels through `Application.Call` (an
  argument, not concatenated script) before the exit.
- **Unknown names cost as much as wrong passwords.** `LoginService` verifies
  `StoredCredential.Dummy` when the user name does not exist, so both paths run one 210,000-round
  PBKDF2 derivation; the lockout path runs none (the video's 0 ms bar), which tells an attacker
  only what the lockout already does.
- **Idle sign-out revokes the remembered device too.** Otherwise the timeout would sign the next
  person straight back in on a shared machine.
- The throttle lives in process memory: right for one server, wrong for a farm (Module 7).

## Answers to the review questions

1. **480 ms versus 6 ms.** The difference tells an attacker which user names exist without
   guessing a single password. Remove it by verifying a dummy credential of the same cost when the
   name is unknown (`StoredCredential.Dummy`), and by answering every failure through the same
   code path and sentence; the lockout refusal is the only fast path, and it reveals nothing that
   five failures did not.
2. **Deleting the password column now that passkeys exist.** (Not built here - see the version
   note.) Service accounts and scripted logins have no authenticator, and an employee who lost or
   replaced their device needs a recovery path. A phishing page can capture a typed password and
   replay it; it cannot obtain a passkey signature for the real origin, because the browser binds
   the ceremony to the site's origin.
3. **Logout without `Application.Exit()`.** The session stays alive until it times out: its pages
   and controls (with the documents they loaded) are still in server memory, and anyone holding the
   tab's session ID can keep using the session. The second is more serious in a portal of
   confidential documents, because it is a live, authenticated view rather than leftover memory.

## Where this sample differs from the lesson

- **Passkeys (lesson, lab, video).** The lesson teaches the Wisej.NET 4.2 `Passkey` component; the
  samples target the released 4.1.7, which has no passkey support. Not built: passkey enrolment
  after password sign-in (`RegisterAsync(new PasskeyUser(...), name)`), *Sign in with a passkey*
  (`AuthenticateAsync()`), the `Authenticating` handler with `PasskeyPrincipal`, the
  `AuthenticationFailed` message, and `AccessOpsPasskeyStore : IPasskeyCredentialStore` registered
  as a Shared service. Affected deliverable: *Password hashing service and passkey sign-in* (the
  password half is complete). WebAuthn was not hand-rolled.
- **Session ID renewal timing.** The lab says to call `RefreshSessionId()` inside
  `LoginService.SignIn` before storing the context. On 4.1.7 that call only works on a page load, so
  the click parks the verified context and the page load renews the ID and stores it - same order,
  one reload later.
- **Console warning banner.** The timeout scene shows a banner (*No activity for 20 minutes …*) on
  the console before the sign-out. The handler signs out immediately (no prolong, no banner), so
  the sample goes straight to the login page with the idle sentence.
- **Status strip on the console.** The console mock has no status strip; the sample keeps the
  `SERVER SAID` strip from Module 1 so every area can say what the server did.
- **Figures.** The mock's `7` open approvals and `312` documents are the seed's real counts here
  (3 and 7), and the document list shows every department - the gap Module 3 closes.
- **People.** The video signs in `r.almeida`; the lab asks for two seeded test employees, so
  `k.osei` (a manager, with the admin entry enabled) is the second.
