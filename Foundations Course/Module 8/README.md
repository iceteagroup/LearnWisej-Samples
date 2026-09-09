# WisejTrainingApp · Wisej.NET Foundations · Module 8

Local lab build for **Module 8 · Integrating a JavaScript widget**. One screen, `StatusPage`, hosts a
custom status gauge — a `Wisej.Web.Widget` named `widStatus` whose visual is `Widgets/statusGauge.js`,
whose look is `Widgets/statusGauge.css`, and whose data comes from a C# service through `Options` —
next to plain native controls: a "Server-side data" card that shows what C# holds right now (so the values
sent to the widget can be verified by eye), the course's usual event-log card, and a bottom bar with the
success path (Refresh), the C# rule recolouring the widget (Set Healthy / Warning / Critical), a failure +
recovery path (Simulate service error) and the lab's "re-test after a theme change" step.

The lesson's files are used verbatim: the JS and CSS in `Widgets/` are the two files printed in reading
s36 §4, and `StatusPage_Load` / `btnRefresh_Click` / `widStatus_WidgetEvent` are the C# from §5.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Foundations Course/Module 8/WisejTrainingApp"
dotnet run -f net10.0 --urls http://localhost:5088
```

Then open <http://localhost:5088>. (Visual Studio: open `WisejTrainingApp.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package. The project multi-targets
`net10.0-windows;net10.0`, so `dotnet run` needs `-f`. Run it **from the project folder**: the static file server
serves that folder (`/Widgets/statusGauge.css`) and `Application.MapPath("Widgets/statusGauge.js")` resolves there.

## What to try

| Action | Path | What you should see |
|---|---|---|
| Page load | wiring | a green gauge *System load — 42%*; the data card shows Open 12, Closed 47, load 42 %, status `ok`, and `lblSent` = `{ percent = 42, label = "System load", status = "ok" }`; the log shows Packages → InitScript (N chars) → Options → WidgetEvent, then what the browser does with them |
| **Refresh Server Data** | success | the fill animates to the next sample and pulses; `lblLoad` and `lblSent` show the same number — the native labels and the widget update from one C# call; log `Widget refreshed with server data`. The sequence is deterministic: 49, 55, 64 (amber), 75, 87 (red), 84 (amber), 53 (green), 58, then repeats |
| **Set Healthy / Set Warning / Set Critical** | C# rule | the gauge turns green / amber / red because `StatusService.StatusFor` said `ok` / `warn` / `error` for 35 / 72 / 93 %; log `Status set to warn by C# rule (load 72 %)` |
| Click the gauge | widget → C# event | log `Gauge clicked at 72%` — the JS fired `gaugeClick` with a tiny payload, `widStatus_WidgetEvent` handled it |
| **Simulate service error** | failure | `GetStatus()` throws once: `lblStatus` turns red *Could not refresh status. Please try again.*; the gauge, `lblLoad`, `lblSent` keep their last values; the exception detail is in the log only |
| **Refresh Server Data** again | recovery | works again — the flag reset itself after one failure |
| **Switch theme (Bootstrap-4 ⇄ Material-3)**, then **Refresh Server Data** | lab step 10 | native controls restyle live; the gauge keeps its scoped `lw-gauge-*` look and still updates and pulses |
| **Clear log** | – | empties the event log |

The right-hand card is the event log: every server-side decision with a timestamp, so the
C# → Options → browser and click → fireWidgetEvent → C# round trips are visible for each action.

## Where things live

```
Module 8/
├─ README.md                        this file
├─ WisejTrainingApp.slnx
└─ WisejTrainingApp/
   ├─ Program.cs                    Wisej.NET session entry point: Application.MainPage = new StatusPage()
   ├─ StatusPage.cs                 code-behind: StatusPage_Load (Packages / InitScript / Options / WidgetEvent),
   │                                btnRefresh_Click, UpdateWidget, widStatus_WidgetEvent, the rule buttons, helpers
   ├─ StatusPage.Designer.cs        Designer-generated layout (InitializeComponent) — widStatus + the cards + the bottom bar
   ├─ Widgets/
   │  ├─ statusGauge.js             THE widget: init / update / pulse / render, fires gaugeClick (lesson s36 §4, verbatim)
   │  └─ statusGauge.css            scoped styles: .lw-gauge-* (lesson s36 §4, verbatim); served as a static file
   ├─ Models/StatusInfo.cs          safe display data: Percent, Label, Status, Open, Closed, SystemLoad
   ├─ Services/StatusService.cs     C# owns the data and the rule: GetStatus, Refresh, SetHealthy/Warning/Critical,
   │                                StatusFor (60 / 85), FailNextCall; ApiKey + ConnectionString stay private here
   ├─ Startup.cs                    Kestrel host (app.UseWisej(), static files from the project folder — never *.json)
   ├─ Default.json / Default.html / Web.config / Properties/launchSettings.json (port 5088)
   └─ docs/
      ├─ WidgetIntegrationNotes.md  native vs Widget table, the four layers, C# ↔ JS mapping, runtime sequence, troubleshooting
      └─ SecurityBoundary.md        bad vs better patterns, what is sent / never sent, third-party checklist, where each rule lives
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| 1 · Open the project, run it once | `WisejTrainingApp.csproj` (Wisej-4 4.1.0, `net10.0-windows;net10.0`), `dotnet run -f net10.0 --urls http://localhost:5088` |
| 2 · Add a widget screen | `StatusPage` (`Wisej.Web.Page`, `Program.cs` sets it as `Application.MainPage`); the "Status widget" card sits next to native cards |
| 3 · Add a Widget control, name it clearly | `widStatus` in `StatusPage.Designer.cs` (`Wisej.Web.Widget`, 420 × 140); pointed at the JS/CSS in `StatusPage_Load` |
| 4 · Wrap the integration | small named files `Widgets/statusGauge.js` + `.css` (marked `<Content>` in the csproj), read once in `StatusPage_Load`; one `UpdateWidget(StatusInfo)` helper instead of script strings per button |
| 5 · Send safe display data | `widStatus.Options = new { percent = data.Percent, label = data.Label, status = data.Status }` — `StatusPage_Load` and `UpdateWidget` |
| 6 · Server-side data card | the "Server-side data" card: `lblOpen`, `lblClosed`, `lblLoad`, `lblStatusValue`, `lblSent`, `lblNeverSent`, `lblRule` — filled by `ShowServerData(StatusInfo)` |
| 7 · Refresh button | `btnRefresh_Click`: `statusService.Refresh()` → `GetStatus()` → `UpdateWidget(data)` → `AddLog("Widget refreshed with server data")` |
| 8 · Wire only needed events | `widStatus.WidgetEvent += widStatus_WidgetEvent`; one `if (e.Type == "gaugeClick")`; everything else is logged and ignored |
| 9 · Keep secrets out of the browser | `Services/StatusService.cs`: `ApiKey` and `ConnectionString` are private fields used only server-side; `StatusInfo` has no such properties; the card's "Never sent to the browser" list; `docs/SecurityBoundary.md` |
| 10 · Run & test, re-test after a theme change | `btnToggleTheme_Click` → `Application.LoadTheme("Material-3" / "Bootstrap-4")`, then Refresh again |

Lab code check (`labs.js` m8): the handler is `btnRefresh_Click`; it works with `widStatus` / `.Options` / `.Update()`;
the data comes from `statusService.GetStatus()`; the code-behind contains no credential, connection-string or
API-key text (those words only appear in `Services/StatusService.cs`, where the fake values are private fields);
it logs through `AddLog` and shows status through `lblStatus`.

## Self-check

**From s36 — "You're ready for the next reading"**

- **When native controls, when a Widget?** Native first — forms, buttons, lists, grids, dialogs are faster to build,
  themed, and already fit the Wisej.NET event model. A Widget only for a browser-side visual that no control provides
  neatly (chart, gauge, map, timeline). Never for a flourish with no business value. This page: one Widget, everything else native.
- **What does the widget's JavaScript file define?** `this.init(options)` — called once after the Packages loaded and the
  element exists: build markup inside `this.container`, cache elements, wire the click, draw. `this.update(options, old)` —
  called every time C# changes `Options` and calls `Update()`: redraw only what changed. Optional named helpers
  (`pulse`) the server can `Call()`. `var me = this;` so callbacks can still reach `me.fireWidgetEvent`.
- **How is it connected from C#?** `Packages` (the CSS/scripts, in load order) → `InitScript` (the JS file, read with
  `File.ReadAllText(Application.MapPath(…))`) → `Options` (an anonymous object, camel-cased on the client) →
  `Update()` runs `update`, `Call("pulse")` runs `pulse`, `WidgetEvent` receives `fireWidgetEvent` with `e.Type` and `e.Data`.
  Set the options before `Update()` / `Call()`: they travel as one batch, applied in order.

**From s37 — "You're ready for the lab"**

- **Where do the business rules live?** In C# — `StatusService.StatusFor` decides `ok` / `warn` / `error`; the JS only
  picks a colour class for the word it receives. No thresholds, ticket maths, permissions or database logic in JavaScript.
- **How is the integration wrapped?** Two small named files in `Widgets/`, read once at load; one `UpdateWidget`
  helper; no script strings built in button handlers. If a second screen needed the gauge, the helper would become a
  `StatusGauge : Widget` control.
- **What are the risks of third-party scripts, and the rule for secrets?** They can change page behaviour, break on
  update, conflict with styles, or leak whatever data they are given — so: trusted, local, versioned, documented,
  re-tested after theme/layout/browser changes; and never send API keys, passwords, connection strings or private
  internal data to the browser. Anything in `Options` reaches DevTools.

## Verified / unverified

Verified by `dotnet build -nologo -v q` (0 warnings, 0 errors) and by the cookbook's runtime facts from the
Application Integration Course samples (same framework build): `Wisej.Web.Widget` with `Packages` / `InitScript` /
`Options`, `Update()` running `this.update(options, old)`, `Call("pulse")` running `this.pulse()`, `fireWidgetEvent`
from a user click reaching `WidgetEvent`, `Application.LoadTheme(name)` live.

**Unverified until the reviewer runs it (this module was not started in a browser):**

- `Application.MapPath("Widgets/statusGauge.js")` + `File.ReadAllText` resolving to the project folder under
  `dotnet run` (the docs say "relative to the application's project directory"; `launchSettings.json` also sets `WEBSITE_PATH`).
  If it fails the page throws at load — see the troubleshooting table in `docs/WidgetIntegrationNotes.md`.
- `Widgets/statusGauge.css` being served at `/Widgets/statusGauge.css` (csproj `<Content>` + `Startup.cs` `UseFileServer()`).
- `widStatus.Update()` and `widStatus.Call("pulse")` on **this** widget (the calls are the cookbook's verified pattern;
  this page's JS is the lesson's file, not the one verified earlier).
- `e.Data.percent` — read with `Convert.ToInt32(e.Data.percent)` rather than the lesson's bare `int percent = e.Data.percent;`
  so the sample works whether the JSON number arrives as `int`, `long` or `double`.
- `Wisej.Web.Page` as `Application.MainPage` with the 1348 × 680 layout (the Application Integration Course uses the same pattern).
