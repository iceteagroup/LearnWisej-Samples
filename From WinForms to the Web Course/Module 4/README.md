# OrderDesk.Web · From WinForms to the Web · Module 4

Local lab build for **Module 4 · Sessions, Statics, and Multi-User Safety**. It follows the lesson and the walkthrough
video: every static field and singleton of **LegacyOrderDesk** is found and classified (*keep static* vs *move to
session / profile store / browser storage*), the five per-user statics of `AppState` become a typed
`UserSessionContext` behind `Application.Session`, the registry settings are relocated, and the same Orders screen is
run on **both** stores with **two browser sessions** — so the corruption (✕ Legacy statics) and the isolation
(✓ UserContext) are both visible, on the same page, with the same two clicks. Logout and a simulated timeout run the
one cleanup routine the lesson asks for.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/From WinForms to the Web Course/Module 4/OrderDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5604
```

Then open <http://localhost:5604>. (Visual Studio: open `OrderDesk.slnx`, F5.) Use a browser window at least
1400 × 760 — the trace on the right is anchored and is sized against the first browser size the client reports.

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try

Session ids in the texts below (`a4f9c2e1`, `7e0d…`) stand for the first 8 characters of `Application.SessionId`;
`<account>@<MACHINE>` for `Environment.UserName` / `MachineName`. Trace lines are quoted without their timestamp.

| Action | Path | What you should see |
|---|---|---|
| Page load | success | Trace `• server startup Default.json → OrderDesk.Program.Main → Application.MainPage = new MainPage()` · `• server session browser session a4f9c2e1 · 1 session(s) share this process` · `• server lifecycle hooks Application.SessionTimeout + Application.ApplicationExit subscribed (released in Dispose)` · `← JS→.NET mode UserContext — the screen reads/writes SessionContext.Current  (✓ one context per browser session)`. Card A shows `11 statics · 3 may stay`; card B has **UserContext (session)** checked, grid `5 of 5 orders · filter All · all customers` with 1042 Northwind Traders 4,820.00 Open first; the stores label shows `(not signed in)` twice and `(nothing yet)`; card C values read `(not tried)` / `(not saved yet)` / `(not saved yet)` / `density to save   Comfortable · signed in as (nobody)`. Status `● audit loaded · not signed in` |
| `buttonAuditAll` / `buttonAuditKeep` / `buttonAuditSession` / `buttonAuditProfile` / `buttonAuditBrowser` (**All / Keep static / Session / Profile store / Browser storage**) | success (audit) | The audit grid filters by verdict: 11 · 3 · 5 · 1 · 1 rows (the *Remove* row only under All); the count label reads `11 statics · 3 may stay` or `3 of 11 statics`; trace `← JS→.NET audit.filter verdict = move to session → 5 rows`. Selecting a row (`gridAudit`) prints `Member · kind → verdict`, *Why* and *Replacement* underneath |
| `radioLegacy` (**Legacy statics**) | – | Trace `← JS→.NET mode Legacy statics — the screen reads/writes Legacy.AppState.*  (✕ one slot for the server)`; combos and grid re-read from the static slot; banner hidden |
| `radioContext` (**UserContext (session)**) | – | Trace `← JS→.NET mode UserContext — the screen reads/writes SessionContext.Current  (✓ one context per browser session)`; combos and grid re-read from this session's context; banner hidden |
| `buttonSignInKelly` (**Sign in as kelly**) on UserContext | success | Trace `← JS→.NET sign in kelly · Acme · Northwind Traders · filter Open` · `• server workspace App_Data\tmp\a4f9c2e1 created — temp files this session must release on logout/timeout` · `• server UserContext.Current → session a4f9c2e1 · kelly · Acme · Northwind Traders · filter Open · culture en-US   ← only this session` · `• server ✓ isolated filter unchanged = Open (UserContext.Current → session a4f9c2e1)`. `comboFilter` = Open, `comboCustomer` = Northwind Traders, grid `2 of 5 orders · filter Open · Northwind Traders` (1042 highlighted, 1040). Green banner `✓ Isolated: UserContext.Current → session a4f9c2e1 still says "kelly · Acme · Northwind Traders · filter Open". Open a second session, sign in as the other user and change its filter, then Re-read here: nothing changes.` Status `● signed in as kelly · UserContext (session)` |
| `buttonSignInSam` (**Sign in as sam**) on UserContext | success | Same shape for `sam · Globex · Fabrikam Inc · filter Invoiced`; grid `1 of 5 orders · filter Invoiced · Fabrikam Inc` (1039 Adventure Works); status `● signed in as sam · UserContext (session)` |
| `buttonSignInKelly` on Legacy statics | success (single-user, "for now") | Trace `• server AppState.CurrentUser (static) = kelly   ← one slot: EVERY session on the server now reads kelly` · `• server static slot = kelly · Acme · Northwind Traders · filter Open — matches this page (no second session has written yet)`. Green banner `✓ Static slot and this page agree ("kelly · Acme · Northwind Traders · filter Open") — for now. Open a second session, switch it to Legacy statics, sign in as the other user, then Re-read here.` Status `● signed in as kelly · Legacy.AppState (static)` |
| `comboFilter` / `comboCustomer` | success | Writes to the active store and re-filters: trace `← JS→.NET filter a4f9c2e1 sets filter = Shipped → UserContext (session)` (or `→ Legacy.AppState (static)`), `← JS→.NET customer a4f9c2e1 sets current customer = Contoso Ltd → …`; the count label follows (`1 of 5 orders · filter Shipped · Contoso Ltd`); the highlighted customer row changes |
| `buttonSecondSession` (**Open second session ↗**) | – | Trace `→ .NET→JS Application.Navigate same URL, target _blank → a second browser session in this process`; a new tab opens whose load trace says `2 session(s) share this process`; the stores label in both tabs reads `… · 2 live · …` after the next action |
| **The corruption test**: tab A `radioLegacy` → `buttonSignInKelly` → `buttonSecondSession`; tab B `radioLegacy` → `buttonSignInSam`; tab A `buttonReread` (**Re-read state**) | **failure** (static corruption) | Trace `← JS→.NET re-read state Legacy.AppState (static) — compare with what this page last wrote`. In tab A the combos jump to **Invoiced / Fabrikam Inc** and the grid to `1 of 5 orders · filter Invoiced · Fabrikam Inc` although kelly touched nothing. Red banner `✕ Static slot corrupted: this session (a4f9c2e1, kelly) last wrote "kelly · Acme · Northwind Traders · filter Open" but AppState now holds "sam · Globex · Fabrikam Inc · filter Invoiced" — another session overwrote the one slot the whole server shares, and the grid now shows the OTHER user's filter. Single-user testing never sees this: with one session the last writer is always you.` Trace `• server ✕ corrupted static filter = Invoiced ≠ this page wrote Open — kelly's selection silently replaced by sam`. Status `● static state overwritten by another session` (red) |
| **The isolation test**: tab A `radioContext` → `buttonSignInKelly`; tab B `buttonSignInSam`, `comboFilter` → Shipped; tab A `buttonReread` | **recovery** (UserContext) | Combos stay **Open / Northwind Traders**, grid stays `2 of 5 orders · filter Open · Northwind Traders`. Green banner `✓ Isolated: UserContext.Current → session a4f9c2e1 still says "kelly · Acme · Northwind Traders · filter Open" while the static slot meanwhile says "sam · Globex · Fabrikam Inc · filter Invoiced" — another session's writes never reached this context.` (the second half appears only if the static slot was written before; otherwise `. Open a second session, sign in as the other user and change its filter, then Re-read here: nothing changes.`). Trace `• server ✓ isolated filter unchanged = Open (UserContext.Current → session a4f9c2e1)`. Status `● signed in as kelly · isolated per session` |
| `buttonReread` before any sign-in through the active store | – | Amber banner `This session has not signed in through this store yet. Sign in here, open a second session, sign in as the other user there (or change its filter), then come back and Re-read.` |
| `comboDensity` (Comfortable · Compact · Dense) | – | Chooses the value the three settings buttons save; the `density to save` line updates on the next button press |
| `buttonRegistry` (**Legacy registry**) | **failure** (registry = the server's hive) | Trace `← JS→.NET settings.registry GridDensity = Comfortable → HKCU\Software\LegacyOrderDesk` · `⚠ boundary RegistrySettings.Save HKCU on <MACHINE> = the hive of "<account>" (the account running Kestrel), not kelly's desktop` (`not the visitor's desktop` when nobody is signed in) · `• server RegistrySettings.Load GridDensity = Comfortable · ExportFolder = C:\Orders — the same answer for every session`. Values line `registry (HKCU)   HKCU of <account>@<MACHINE>: GridDensity = Comfortable · ExportFolder = C:\Orders`. Red banner `✕ Registry: the write succeeded — into HKCU of "<account>" on <MACHINE>. That is whose registry this really is on a server: the service account's, one hive shared by every visitor. kelly saves Comfortable, sam reads Comfortable. ExportFolder C:\Orders is a disk the user cannot see. On Linux/containers the same call throws PlatformNotSupportedException.` Status `● registry = the server account's hive, shared by all` (red). Note: on your PC this really writes `HKCU\Software\LegacyOrderDesk`; press it in tab B too — sam reads kelly's value. On Linux the banner is `✕ Registry: PlatformNotSupportedException — <OS> has no registry at all. …` |
| `buttonProfile` (**Profile store**) while not signed in | – | Trace `• server UserProfileStore no signed-in user — a profile belongs to a user`; amber banner `Sign in first — a profile belongs to a user, which is exactly what HKCU could not express on the server.`; status `● profile store needs a signed-in user` |
| `buttonProfile` after `buttonSignInKelly` | recovery (server-owned, roaming) | Trace `← JS→.NET settings.profile GridDensity = Comfortable for kelly` · `• server UserProfileStore.Save App_Data\profiles\kelly.json · <n> bytes (System.Text.Json)` · `• server UserProfileStore.Load GridDensity = Comfortable · ExportFolder = exports → App_Data\exports\kelly`. Values line `profile store     App_Data\profiles\kelly.json: GridDensity = Comfortable · exports → App_Data\exports\kelly`. Green banner `✓ Profile store: kelly's settings live in App_Data\profiles\kelly.json on the server — per user, they follow the user to any device and server code can read them. ExportFolder is now a folder under App_Data (App_Data\exports\kelly), not C:\Orders: exports are staged there and reach the browser through Application.Download. Settings that must be audited or queried go one step further, into a database table.` Status `● profile saved for kelly`. The file exists under `OrderDesk.Web/App_Data/profiles/` |
| `buttonBrowser` (**Browser storage**) | recovery (device-bound) | Trace `← JS→.NET settings.browser GridDensity = Comfortable → localStorage` · `→ .NET→JS Application.Eval localStorage.setItem('orderdesk.density', 'Comfortable')` · `← JS→.NET Application.EvalAsync localStorage.getItem('orderdesk.density') → "Comfortable"`. Values line `browser storage   localStorage['orderdesk.density'] = "Comfortable" (this browser profile only)`. Green banner `✓ Browser storage: localStorage['orderdesk.density'] = "Comfortable" — stored in THIS browser profile on THIS device. The second session in the same browser reads the same value, another device starts empty, and the user can clear it at any time. Right for a UI preference like density; never for business settings, which stay server-owned.` Status `● density Comfortable stored in the browser` |
| `buttonSignOut` (**Sign out**) | recovery (cleanup) | Trace `← JS→.NET sign out kelly · UserContext (session)` then five `• server cleanup …` lines: `temp files: deleted <full path>\App_Data\tmp\a4f9c2e1 (1 file(s))` · `report jobs: cancel queued jobs keyed by this session (none running in the sample)` · `transactions: roll back any open unit of work (the in-memory repository holds none)` · `locks: release row/record locks held on behalf of this session (none in the sample)` · `UserContext: SessionContext.Reset() → session a4f9c2e1 is anonymous again (logout)`. Combos back to All / All customers, grid `5 of 5 orders · filter All · all customers`, status `● signed out (logout) · cleanup ran`. Green banner `✓ Signed out: SessionContext.Reset() cleared this session's context and the cleanup routine released the session's temp files; report jobs, open transactions and locks are released in the same routine — the one place both logout and timeout call.` On Legacy statics with a signed-in static user: extra trace `⚠ boundary AppState cleared ✕ the statics are one slot — this sign-out signed out EVERY session on the server` and the banner is amber, ending ` In Legacy statics mode this logout also blanked AppState — for EVERY session on the server.` |
| `buttonTimeout` (**Simulate timeout**) | **progress** (5 s countdown) | Trace `← JS→.NET simulate timeout 5 s countdown → the cleanup routine Application.SessionTimeout → ApplicationExit would run; the session itself stays alive`. `progressTimeout` appears and fills 20 % per second; status `● session times out in 5 s…` then `● session times out in 4 s… (a real timeout shows the built-in prolong dialog first)` … `1 s…` (amber). At zero: trace `• server SessionTimeout (simulated) cleanup must run BEFORE the session is destroyed — afterwards there is no session left to clean` + the five `cleanup` lines with `(timeout (simulated))`; status `● signed out (timeout (simulated)) · cleanup ran`; amber banner `⏱ Simulated timeout: the cleanup routine ran (temp files deleted, context reset; report jobs, transactions and locks logged) but the session was NOT ended. The real Application.SessionTimeout fires after the configured sessionTimeout, shows the built-in prolong dialog (this sample leaves Handled = false), and if nobody answers the session ends and Application.ApplicationExit runs this same routine.` A second press during the countdown is ignored |
| `buttonClear` (**Clear**) | – | Empties the trace |

The right-hand card is the **migration log · live trace**: every user action (`← JS→.NET`), every business-logic call
(`• server`), everything pushed to the browser (`→ .NET→JS`) and every desktop boundary hit and replaced (`⚠ boundary`).

## Where things live

```
Module 4/
└─ OrderDesk.Web/                     the Wisej.NET 4 app (net10.0-windows;net10.0), port 5604
   ├─ Program.cs / Startup.cs         session entry point (Application.MainPage) / Kestrel host (app.UseWisej())
   ├─ Default.html / Default.json / Web.config
   ├─ Domain/                         ✓ the reused business logic (Order, Customer, OrderService, CustomerService, InvoiceDocument, SampleData)
   ├─ Legacy/AppState.cs              ✕ the five per-user statics (+ ✓ Countries), kept so the corruption can be shown live
   ├─ Legacy/RegistrySettings.cs      ✕ HKCU\Software\LegacyOrderDesk — Registry.CurrentUser on the SERVER
   ├─ Migration/StaticStateAudit.cs   lab steps 1 + 2 as data (11 rows, five verdicts)
   ├─ Services/UserSessionContext.cs  ✓ the typed context: plain data, no Wisej reference
   ├─ Services/SessionContext.cs      ✓ Current (lazy, per browser session in Application.Session) + Reset()
   ├─ Services/UserProfileStore.cs    ✓ App_Data/profiles/<user>.json (System.Text.Json) — the roaming, server-owned store
   ├─ Services/BrowserPreferences.cs  ✓ localStorage['orderdesk.density'] via Application.Eval / EvalAsync
   ├─ Services/SessionCleanup.cs      ✓ the one routine logout, timeout and ApplicationExit share
   ├─ Views/TracePanel.cs, Ui.cs      shared console helpers
   ├─ MainPage.cs / .Designer.cs      the lab console (three cards + trace)
   ├─ App_Data/                       created at run time: profiles/<user>.json, tmp/<session>/report-job.txt
   └─ docs/                           the lab deliverables + migration-log.md
```

## Deliverables

| # | Deliverable | Document | Code |
|---|---|---|---|
| 1 | Static-state audit (lab steps 1 + 2) | [`OrderDesk.Web/docs/StaticStateAudit.md`](OrderDesk.Web/docs/StaticStateAudit.md) | [`Migration/StaticStateAudit.cs`](OrderDesk.Web/Migration/StaticStateAudit.cs) (card A) |
| 2 | Static user state → session services (lab steps 3 + 4) | [`OrderDesk.Web/docs/SessionContextService.md`](OrderDesk.Web/docs/SessionContextService.md) | [`Services/UserSessionContext.cs`](OrderDesk.Web/Services/UserSessionContext.cs), [`Services/SessionContext.cs`](OrderDesk.Web/Services/SessionContext.cs), [`Legacy/AppState.cs`](OrderDesk.Web/Legacy/AppState.cs) ✕ |
| 3 | Two-session isolation test (lab step 5) | [`OrderDesk.Web/docs/TwoSessionTest.md`](OrderDesk.Web/docs/TwoSessionTest.md) | `MainPage.cs` card B (`RefreshStores`) |
| 4 | Registry settings relocated | [`OrderDesk.Web/docs/SettingsRelocation.md`](OrderDesk.Web/docs/SettingsRelocation.md) | [`Services/UserProfileStore.cs`](OrderDesk.Web/Services/UserProfileStore.cs), [`Services/BrowserPreferences.cs`](OrderDesk.Web/Services/BrowserPreferences.cs), [`Legacy/RegistrySettings.cs`](OrderDesk.Web/Legacy/RegistrySettings.cs) ✕ |
| 5 | Cleanup requirements for timeout and logout (lab step 6) | [`OrderDesk.Web/docs/SessionCleanup.md`](OrderDesk.Web/docs/SessionCleanup.md) | [`Services/SessionCleanup.cs`](OrderDesk.Web/Services/SessionCleanup.cs), `MainPage.cs` lifecycle hooks |
| 6 | Migration log | [`OrderDesk.Web/docs/migration-log.md`](OrderDesk.Web/docs/migration-log.md) | the trace panel is the live version |

## Self-check answers (lab guide + storyboard)

- **Which business logic was reused as-is?** Everything under `Domain/`, unchanged since Module 1. Card B filters the
  grid through `OrderService.Search(new OrderFilter { Status, Text })` — the same call `OrdersForm` made; only *where
  the filter value comes from* changed (a static → the session context).
- **Which desktop boundary was replaced with a web-safe pattern?** The registry. `Registry.CurrentUser` in server code
  opens the server's hive as the service account (and throws `PlatformNotSupportedException` on Linux). `GridDensity`
  went to browser `localStorage` via `Application.Eval`/`EvalAsync`, `ExportFolder` to a per-user JSON profile under
  `App_Data`, `WindowWidth/Height` were removed. The decision table (roam? audited? centrally managed? device-bound?) is
  in `SettingsRelocation.md`.
- **How is per-user state kept out of static fields?** The five per-user statics of `AppState` became properties of
  `UserSessionContext`, one instance per browser session stored in `Application.Session` and reached only through
  `SessionContext.Current`. Callers do not know the storage; `Reset()` clears it on logout. The statics that were
  immutable and user-independent (`Countries`, `SampleData.Customers`, the locked repository) stayed static.
- **What was tested before calling the migrated feature complete?** The two-session test in `TwoSessionTest.md`, both
  ways: on the legacy store *Re-read* in tab A shows `✕ Static slot corrupted …` after tab B signs in as sam; on the
  context store the same clicks show `✓ Isolated …` and the combos do not move. Plus logout: signing out in one tab
  leaves the other tab's context intact (and, on the legacy store, visibly does not).
- **Pause & predict — Can this static stay: is it immutable and user-independent?** Ask *would the value differ if two
  users opened the app at the same time?* `AppState.Countries` (readonly, same for everyone): stays.
  `SampleData.Customers` (reference data, clone-on-read): stays. `InMemoryOrderRepository.Shared`: stays — one store
  for all sessions is the intended semantics and it is locked. `CurrentUser`, `CurrentCompany`, `CurrentCustomer`,
  `CurrentFilter`, `LastSearch`: differ per user → session. Static *methods* without fields hold no state and need no
  verdict. 3 of 11 stay.
- **Pause & predict — Should this setting roam, be audited, or stay on the device?** `GridDensity` may differ per
  device → browser storage (with a roaming copy in the profile so a new device starts sensibly). `ExportFolder` must
  roam and server code must read it → server profile store; if exports had to be audited → a database table.
  `WindowWidth/Height` → neither; the browser window belongs to the user. Anything HKCU held that a business process
  depends on is server-owned, never browser-only.

## Runtime facts

- `Application.Session` is a **dynamic bag** (also a `Dictionary<string, object>`): `dynamic s = Application.Session;
  s.UserContext = ctx;` — a member that was never set reads as `null`. `SessionContext.Current` wraps exactly that
  and creates the context lazily.
- `Application.SessionTimeout` and `Application.ApplicationExit` are **static events**: subscribe in the page
  constructor, **unsubscribe in `Dispose`** (`MainPage.Designer.cs` → `DetachApplicationEvents`), or the page is never
  collected. `SessionTimeout` carries `HandledEventArgs`; leaving `Handled = false` keeps the built-in prolong dialog.
  `ApplicationExit` runs with no UI to push to — log to the host, do not touch controls.
- `Application.Eval(js)` runs a statement in the browser; `await Application.EvalAsync(expression)` returns the
  value of an **expression** — never write `return …;` ("Illegal return statement"). Used for `localStorage`.
- `Microsoft.Win32.Registry` compiles for `net10.0` but **throws `PlatformNotSupportedException` at run time on
  Linux**; on Windows it writes the hive of the account running Kestrel on the server machine.
- `Application.Navigate(Application.Url, "_blank")` opens a second session in the same server process;
  `Application.SessionCount` counts them; `Application.SessionId` identifies this one.
- `Application.StartupPath` is the project folder under `dotnet run`, so `App_Data/profiles` and `App_Data/tmp` are
  created next to the sources; delete `App_Data/` to reset the sample.
- `Wisej.Web.Timer(components)` drives the countdown (`Interval = 1000`, `Tick`, `Start()`/`Stop()`); the page keeps
  responding while it runs.
- The projects multi-target `net10.0-windows;net10.0`, so `dotnet run` needs `-f net10.0` (or `-f net10.0-windows`).
