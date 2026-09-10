# Screen classification — LegacyOrderDesk

**Lab 1 deliverable 2.** Every screen (and the desktop subsystems behind it) sorted into the four
verdicts of the lesson plus *remove*. The rule from the video: do **not** convert all the easy forms
first — that is how an Excel Interop workflow or a static user object becomes a late, expensive surprise.

## The four verdicts

| Verdict | Meaning (lesson) | LegacyOrderDesk items |
|---|---|---|
| **Direct-port** | standard controls, ordinary handlers, simple binding — no local dependencies | LoginForm · the business logic (`OrderService`, `OrderCalculator`, `OrderValidator`, `OrderStore`) |
| **Port-with-adaptation** | reusable, but needs Upload/Download, session state, theming or grid tuning | OrdersForm shell (MenuStrip/StatusStrip) · orders grid (volume) · EditOrderDialog (disposal, notification) · `AppState` statics · `UserPreferences` registry · `LocalExport` C:\Orders · App.config |
| **Redesign** | the workflow assumes the desktop — printers, Office, local file browsing | Print Invoice (`PrintDocument`) · Export to Excel (Excel Interop) |
| **Defer** | not impossible, just not needed to prove the migration | Help → About |
| *Remove* | has no meaning on a server | File → Exit (`Form.Close()` ended the process) |

## Screen by screen

### LoginForm — direct-port (S)
ComboBox + Button, no password, one handler. `System.Windows.Forms` → `Wisej.Web` and it compiles.
The only desktop dependency is the `UserPreferences.Get("LastUser")` default, which belongs to the
registry item (adapt). The *result* of the login — `AppState.CurrentUser = login.User` in
`Program.Main` — is the static-state item, not a LoginForm problem.

### OrdersForm — port-with-adaptation (M)
- **Shell**: `MenuStrip`/`ToolStripMenuItem`/`StatusStrip`/`ToolStripStatusLabel` have Wisej.NET
  equivalents (`MenuBar`/`MenuItem`/`StatusBar`/`StatusBarPanel`) but different names — a compiler pass
  (Module 2).
- **Grid**: `DataGridView` exists with the same columns; `ReloadGrid()` binds **all** rows from
  `OrderService.GetAll()` — 5,000 today, 200,000 in Module 5's test — every one a payload to the browser.
  Verdict adapt: `VirtualMode` + a server-side `OrderQuery` (Module 5).
- **Detail panel + SelectionChanged**: writes `AppState.CurrentOrder` / `CurrentCustomer` — static-state.
- **Buttons**: New Order (opens the dialog, fine) · Print Invoice (redesign) · Export to Excel (redesign,
  with the `LocalExport` fallback: adapt).
- `MessageBox.Show("Saved.")` after a save: works, but blocks for an informational message — `Toast`
  in Module 3. The validation MessageBox in the dialog may stay modal.

### EditOrderDialog — port-with-adaptation (S)
Three ComboBoxes, Save/Cancel, `DialogResult`. `ShowDialog` works the same way in Wisej.NET; what
changes is the lifetime: the WinForms process exit used to clean up the never-disposed dialog, the
server does not — `using (var dlg = new EditOrderDialog(...))` (Module 3).

### Print Invoice — redesign (M)
`PrintDocument.Print()` sends the page to the default printer **of the machine running the code**.
On the server that is no printer at all. The drawing code becomes a server-side PDF shown in a
`PdfViewer` or downloaded (Module 6).

### Export to Excel — redesign (M)
`new Excel.Application()` needs Excel installed and an interactive user session; on a server it throws
`COMException 0x80040154` (REGDB_E_CLASSNOTREG) or hangs. Microsoft does not support server-side Office
Automation. A managed `.xlsx` writer + `Application.Download` (Module 6). The fallback `LocalExport`
CSV is adapt: its formatting half is reused today as the body of a download (this module's slice).

### `AppState` statics — adapt (S), the multi-user risk
`CurrentUser`, `CurrentCustomer`, `ActiveFilter`, `CurrentOrder` are per-user values in static fields.
Correct on a desktop (one process, one user), wrong on a server (one process, every session). Module 4
moves them into `Application.Session`. `Countries` (immutable lookup) may stay static.

### `UserPreferences` (HKCU) — adapt (S)
Reads/writes the registry of the machine running the code under the account running the code: on the
server that is the wrong machine and the wrong user. Per-user server profile in Module 4.

### App.config — adapt (XS)
`ExportFolder=C:\Orders`, `ReportPrinter=(default)`, `LAN-SQL01` connection string → `Web.config`,
read by an `AppConfig` helper (Module 2).

### File → Exit — remove · Help → About — defer

## Evidence (what the running app shows)

- The **Verdict** column of the workbook grid is coloured per verdict (direct-port green, adapt blue,
  redesign amber, defer purple, remove grey); the summary line on load and after **Replay assessment**
  reads `direct-port 2 · adapt 7 · redesign 2 · defer 1 · remove 1`.
- **Export (desktop way) ✕** demonstrates the two redesign/adapt boundaries live:
  `✖ fail  COMException 0x80040154  Retrieving the COM class factory …` and
  `✖ fail  would write C:\Orders\out.csv on the SERVER  …`, each followed by a `★ log` finding naming the verdict.
- **Second session** demonstrates the static-state finding: `AppState.CurrentUser = kelly` then
  `read back … the same static instance for every session in this process`; a second browser tab logs
  `✖ fail  Legacy/AppState.CurrentUser  already "kelly" in this NEW session — set by session <id> (ANOTHER tab)` on load.
