# WisejTrainingApp · Wisej.NET Foundations · Module 2

Local lab build for **Module 2 · Using the Wisej.NET Designer / Properties, events and Designer code**. It is
the lab's System Dashboard exactly as the walkthrough video builds it: a title and status label, a Panel with
three service indicators, Start / Stop / Reset / Refresh buttons and an Event Log — each button with one short
Click handler in `DashboardWindow.cs`, the repeated logging pulled into `AddLog()`, and the layout left to
`DashboardWindow.Designer.cs`. On top of the video it adds a failure switch (a simulated API outage on Refresh)
and its recovery, a "Where your code goes" card for lesson step 7, and the course's usual event-log card.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Foundations Course/Module 2/WisejTrainingApp"
dotnet run -f net10.0 --urls http://localhost:5082
```

Then open <http://localhost:5082>. (Visual Studio: open `WisejTrainingApp.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package. The project multi-targets
`net10.0-windows;net10.0`, so `dotnet run` needs `-f`.

## What to try

| Action | Path | What you should see |
|---|---|---|
| **Start** | success | all three indicators turn green *Online*, `lblStatus` turns green *Status: Running*, the log gains `Dashboard started.` |
| **Refresh** | success | the log gains `Status refreshed — Server 12 ms · Database 31 ms · API Service 18 ms` (different numbers each click); indicators re-painted from `ServiceMonitor.CheckAll()` |
| **Stop** | success | indicators red *Offline*, `lblStatus` red *Status: Stopped*, log `Dashboard stopped.` |
| **Reset** | success | the log empties, indicators *Offline*, `lblStatus` grey *Status: Idle*, the outage box is cleared, then one line: `Dashboard reset.` |
| Tick **Simulate API outage on Refresh**, click **Refresh** — or the bottom-bar **Simulate API outage + Refresh (failure)** | failure | `lblApiStatus` turns amber *API Service: Degraded*, `lblStatus` amber *Status: Degraded*, log `API Service check failed: timeout after 2000 ms`; Server and Database untouched |
| Untick, **Refresh** — or **Clear outage + Refresh (recovery)** | recovery | `lblApiStatus` back to green *Online* (red *Offline* if never started), `lblStatus` back to *Status: Running* when started, a fresh `Status refreshed — …` line |
| Tick / untick the checkbox on its own | – | `chkSimulateOutage_CheckedChanged` logs what the next Refresh will do |

The right-hand card is the Event Log: every handler, every server-side decision, with a `HH:mm:ss` timestamp
from `DateTime.Now`, so the browser → `Click` → handler → labels → browser round trip is visible for each click.
The first three lines after load are the beginner lifecycle (`Program.Main` → `InitializeComponent()` → `Load`).

## Where things live

```
WisejTrainingApp/
├─ Program.cs                       Wisej.NET session entry point: new DashboardWindow().Show()
├─ DashboardWindow.cs               code-behind: btnStart/Stop/Reset/Refresh_Click, chkSimulateOutage_CheckedChanged,
│                                   the bottom-bar shortcuts, AddLog / ShowServices / SetStatus helpers
├─ DashboardWindow.Designer.cs      Designer-generated layout (InitializeComponent) — lblTitle, lblStatus, pnlServices
│                                   + three indicators, four buttons, chkSimulateOutage, lstEventLog, the cards
├─ Models/ServiceCheck.cs           ServiceState (Offline / Online / Degraded) + one check result (name, state, latency, error)
├─ Services/ServiceMonitor.cs       the fake monitor: Start/Stop/Reset, SimulateApiOutage, CheckAll() with fake latencies
├─ Startup.cs                       Kestrel host (app.UseWisej(), static files from the project folder)
├─ Default.json / Default.html / Web.config / Properties/launchSettings.json (port 5082)
└─ docs/
   ├─ DashboardEvents.md            property / event / handler / method table, the lifecycle, the AddLog habit, safe habits, evidence
   └─ ControlNaming.md              weak name → better name for every control, which file each name lives in, evidence
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| 1 · Open the project | `WisejTrainingApp.csproj` (Wisej-4 4.1.0, `net10.0-windows;net10.0`), `WisejTrainingApp.slnx`; run once with the command above |
| 2 · Create the page | `DashboardWindow` (a `Form`) — `DashboardWindow.cs` + `DashboardWindow.Designer.cs`; `Program.cs` shows it |
| 3 · Title & status | `lblTitle` (*System Dashboard*, 20 bold) and `lblStatus` (*Status: Idle*) in `DashboardWindow.Designer.cs` |
| 4 · Service indicators | `pnlServices` holding `lblServerStatus`, `lblDatabaseStatus`, `lblApiStatus` (all *Offline*, red) |
| 5 · Action buttons | `btnStart`, `btnStop`, `btnReset`, `btnRefresh` — `Text` Start / Stop / Reset / Refresh |
| 6 · Event log | `lstEventLog` (`ListBox`, monospace) in the Event Log card |
| 7 · Name clearly | every name above; the full naming pass is `docs/ControlNaming.md` |
| 8 · Wire & run | `this.btnStart.Click += new System.EventHandler(this.btnStart_Click);` (Designer file) → `btnStart_Click` and the other three handlers in `DashboardWindow.cs`, each updating labels and calling `AddLog(...)` |

Lab code check (`labs.js` m2): `DashboardWindow.cs` has `btnStart_Click` and the other `_Click` handlers, sets
`lblStatus.Text`, handles `btnStop` / `btnReset` / `btnRefresh`, writes to the log through `AddLog` /
`lstEventLog`, and timestamps with `DateTime.Now`.

## Self-check

Lesson s6 — *Completion check*:

- **Open a page in the Designer and place a Label, TextBox/ListBox, Button and Panel.** All four kinds are on this
  screen: `Label` (`lblTitle`, `lblStatus`, the three indicators), `ListBox` (`lstEventLog`), `Button` (four of them),
  `Panel` (`pnlServices`, plus the cards). `DashboardWindow.Designer.cs` is what the Designer writes when you do that.
- **Rename them clearly.** No control is left as `button1` / `label1`; `docs/ControlNaming.md` lists the weak name,
  the better name and the reason for each one.
- **Change visible properties such as `Text`, `BackColor`, `Font` or `Size`.** In the Designer file: `Text` on every
  label and button, `Font` (20 bold title, 11 bold status, monospace log), `BackColor` on the cards and `pnlServices`,
  `Size` / `Location` on everything, `ForeColor` red / green / amber on the indicators.

Lesson s7 — *habits*:

- **Property, event, event handler, method?** `btnStart.Text = "Start"` is a property; `btnStart.Click` is an event;
  `btnStart_Click(object sender, EventArgs e)` is its handler; `lstEventLog.Items.Add(...)` is a method. The
  "Where your code goes" card shows these three lines; `docs/DashboardEvents.md` has the full table for the screen.
- **Which file owns what?** `DashboardWindow.cs` — handlers and helpers; `DashboardWindow.Designer.cs` — generated
  layout, don't edit by hand; `Program.cs` — startup (`new DashboardWindow().Show()`).
- **The beginner lifecycle?** `Program.Main` → `new DashboardWindow()` → `InitializeComponent()` builds the controls →
  `Load` → the user clicks → the handler runs. The first three log lines show it on every start.
- **Why the `AddLog()` helper?** So each handler stays four lines: the timestamp and `Items.Add` exist once.
  `ShowServices` and `SetStatus` apply the same idea to the labels and the status colour.
- **Safe event-handling habits?** Handlers for user actions only; each one short; repeated logic in helpers; the
  Designer file untouched; no slow database / API call inside a Click — `ServiceMonitor` answers instantly and the
  outage is *reported* ("timeout after 2000 ms"), not waited for; per-user state in an instance field, never `static`.

## Verified / unverified

Builds clean on this machine (`dotnet build -nologo -v q` → 0 warnings, 0 errors, both targets). Not run in the
browser by the author; the reviewer runs it. Everything used is on the cookbook's verified list (Panel `BorderStyle`,
Label `Font` / `ForeColor` / `TextAlign`, ListBox, Button `Click`) except one call to check at runtime:

- `Wisej.Web.CheckBox` — `Checked`, `Text`, `ToolTipText` and the `CheckedChanged` event (compiles against
  Wisej.Framework 4.1.0; not on the cookbook's verified list). Expected: ticking the box logs a line, and setting
  `Checked` from code (Reset, the bottom-bar shortcuts) both updates the box and fires `CheckedChanged` once.
