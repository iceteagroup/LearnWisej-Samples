# Settings relocation — registry → profile store / browser storage / database

Deliverable 2 of the storyboard: "registry settings relocated — database, profile store, or browser storage".
LegacyOrderDesk kept its settings in `HKCU\Software\LegacyOrderDesk` (`GridDensity`, `ExportFolder`, `WindowWidth`,
`WindowHeight`). `Registry.CurrentUser` in server code is the registry of the **server** machine, opened as the
**service account** that runs Kestrel — one hive shared by every visitor, and no hive at all on Linux. Card C of the
console (**Settings storage · registry → server profile → browser · session cleanup**) runs all three destinations on
the same setting so the difference is visible.

## The decision table

The lesson's question per setting: must it **roam** (follow the user to any device), be **audited** or queried,
be **centrally managed**, or is it **device-bound**?

| Destination | Roam? | Audited / queryable? | Centrally managed? | Device-bound? | Use for | In this sample |
|---|---|---|---|---|---|---|
| **Database table** (`UserSettings(UserId, Key, Value, ChangedBy, ChangedOn)`) | yes | **yes** — history, reports, admin edits | yes | no | business settings, anything compliance asks about, anything an admin sets for a user | not built — `UserProfileStore` is the step before it; the code shape is the same (`Load(user)` / `Save(user, profile)`) with a repository behind it |
| **Server user-profile store** (`App_Data/profiles/<user>.json`) | yes | no (a file, no history) | partly (the server owns it) | no | per-user settings the server code needs to read, without audit requirements | `Services/UserProfileStore.cs` — `GridDensity` (roaming copy) and `ExportFolder` |
| **Browser storage** (`localStorage`) | **no** — another device starts empty | no | no — the user can clear it | **yes** | pure UI preferences that may legitimately differ per device (density, collapsed panels, last tab) | `Services/BrowserPreferences.cs` — `localStorage['orderdesk.density']` |
| **Config per user** (`Web.config` / appsettings) | n/a | no | yes | no | only if operationally acceptable — a deploy per settings change | not used |
| ~~Registry (HKCU)~~ | no — it is the server's hive | no | no | bound to the **server** | nothing | `Legacy/RegistrySettings.cs` ✕, kept to show the failure |

Applied to LegacyOrderDesk's four keys:

| Setting | Question | Answer | Goes to |
|---|---|---|---|
| `GridDensity` | Does it hurt if my phone and my desk show different densities? | No — a UI preference, device-bound is fine | **browser storage** (the profile store keeps a roaming copy so a new device starts with the user's usual value) |
| `ExportFolder` | Server code must read it; it must follow the user; it names a place the server owns | Roams, server-owned, business-relevant | **profile store** (`App_Data/<ExportFolder>/<user>`); a database row if exports must be audited |
| `WindowWidth / WindowHeight` | The browser window belongs to the user | Not a setting any more | **remove** (responsive layout, Module 7) |

## The three buttons

All three save the density chosen in `comboDensity` (Comfortable · Compact · Dense). The **values** label above the
buttons shows the last answer of each store plus `density to save  <density> · signed in as <user>` (it refreshes on
every button press).

### `buttonRegistry` — *Legacy registry* (the failure) ✕

`Legacy.RegistrySettings.Load()` → set `GridDensity` → `Save()` → `Load()` again, exactly as `SettingsForm` did.

* Trace: `← JS→.NET settings.registry  GridDensity = Compact → HKCU\Software\LegacyOrderDesk` ·
  `⚠ boundary RegistrySettings.Save  HKCU on <MACHINE> = the hive of "<account>" (the account running Kestrel), not kelly's desktop`
  (or `not the visitor's desktop` when nobody is signed in) ·
  `• server RegistrySettings.Load  GridDensity = Compact · ExportFolder = C:\Orders — the same answer for every session`.
* Values line: `registry (HKCU)   HKCU of <account>@<MACHINE>: GridDensity = Compact · ExportFolder = C:\Orders`.
* Red banner: `✕ Registry: the write succeeded — into HKCU of "<account>" on <MACHINE>. That is whose registry this really is on a server: the service account's, one hive shared by every visitor. kelly saves Compact, sam reads Compact. ExportFolder C:\Orders is a disk the user cannot see. On Linux/containers the same call throws PlatformNotSupportedException.`
* Status `● registry = the server account's hive, shared by all` (red).

The write **succeeds** on a Windows developer PC — that is the point: nothing throws, the setting simply lands in the
wrong place. (`<account>` / `<MACHINE>` are `Environment.UserName` / `Environment.MachineName`; on your PC that is you,
and the key really is written to your `HKCU\Software\LegacyOrderDesk`. Delete it afterwards if you like.) Press it in
the second session too: the same value comes back for sam. Two other outcomes are caught and explained:

* `PlatformNotSupportedException` (Linux / container): trace `⚠ boundary RegistrySettings.Save  PlatformNotSupportedException: Linux … has no registry`, red banner
  `✕ Registry: PlatformNotSupportedException — <OS> has no registry at all. Even on Windows HKCU would be the service account's hive, shared by every visitor. Use the profile store or browser storage instead →`, status `registry: not supported on this server`.
* Any other exception (a locked-down service account that may not write HKCU): red banner
  `✕ Registry: <ExceptionType> — the service account may not even be allowed to write HKCU (<message>). Either way the server's registry is the wrong place for a user's settings.`, status `registry write failed on the server`.

### `buttonProfile` — *Profile store* (recovery, server-owned) ✓

`UserProfileStore.Load(user)` → set `GridDensity` → `Save(user, profile)` → `Load(user)` again. Requires a signed-in
user because a profile belongs to a user — which is exactly what HKCU could not express:

* Not signed in: trace `• server UserProfileStore  no signed-in user — a profile belongs to a user`, amber banner
  `Sign in first — a profile belongs to a user, which is exactly what HKCU could not express on the server.`, status
  `profile store needs a signed-in user`.
* Signed in as kelly: trace `← JS→.NET settings.profile  GridDensity = Compact for kelly` ·
  `• server UserProfileStore.Save  App_Data\profiles\kelly.json · <n> bytes (System.Text.Json)` ·
  `• server UserProfileStore.Load  GridDensity = Compact · ExportFolder = exports → App_Data\exports\kelly`.
  Values line: `profile store     App_Data\profiles\kelly.json: GridDensity = Compact · exports → App_Data\exports\kelly`.
  Green banner: `✓ Profile store: kelly's settings live in App_Data\profiles\kelly.json on the server — per user, they follow the user to any device and server code can read them. ExportFolder is now a folder under App_Data (App_Data\exports\kelly), not C:\Orders: exports are staged there and reach the browser through Application.Download. Settings that must be audited or queried go one step further, into a database table.`
  Status `● profile saved for kelly`.

The file is `App_Data/profiles/kelly.json` under the project folder (`Application.StartupPath`), written with
`System.Text.Json`; the user name is reduced to a safe file name. sam gets `sam.json`. `ExportFolder` defaults to
`exports` — a **folder name under App_Data**, never a client path; Module 6 stages exports there and streams them with
`Application.Download`.

### `buttonBrowser` — *Browser storage* (recovery, device-bound) ✓

`BrowserPreferences.SaveDensity(density)` runs `Application.Eval("localStorage.setItem('orderdesk.density', 'Compact')")`;
`await BrowserPreferences.ReadDensityAsync()` runs `Application.EvalAsync("localStorage.getItem('orderdesk.density')")`
— an **expression**, never a `return` statement — and the browser answers.

* Trace: `← JS→.NET settings.browser  GridDensity = Compact → localStorage` ·
  `→ .NET→JS Application.Eval  localStorage.setItem('orderdesk.density', 'Compact')` ·
  `← JS→.NET Application.EvalAsync  localStorage.getItem('orderdesk.density') → "Compact"`.
* Values line: `browser storage   localStorage['orderdesk.density'] = "Compact" (this browser profile only)`.
* Green banner: `✓ Browser storage: localStorage['orderdesk.density'] = "Compact" — stored in THIS browser profile on THIS device. The second session in the same browser reads the same value, another device starts empty, and the user can clear it at any time. Right for a UI preference like density; never for business settings, which stay server-owned.`
* Status `● density Compact stored in the browser`.

No sign-in needed: the value belongs to the device, not to a user. The second tab (same browser profile) reads the same
value; an incognito window or another machine reads `null` — the trace shows `→ null`.

## Linux / container note

`Microsoft.Win32.Registry` compiles for `net10.0` but **throws `PlatformNotSupportedException` at run time** on Linux —
there is no registry to open. `buttonRegistry` catches that case specifically (see above). The profile store and
browser storage are platform-neutral: `Path.Combine(Application.StartupPath, "App_Data", "profiles")` yields
`/app/App_Data/profiles/kelly.json` in a container, and `localStorage` lives in the browser regardless of the server OS.
The project builds for both `net10.0-windows` and `net10.0` so this can be checked.

## Evidence

Press the three buttons in a row with the same density and read the values label: three stores, three different
answers to "whose setting is this?" — the server account's (✕), kelly's on the server (✓ roaming), this browser's
(✓ device-bound). Then press *Legacy registry* in the second session (sam): it reads kelly's value back — the shared
hive in one line.
