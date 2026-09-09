# Widget integration notes — native controls vs a JavaScript Widget, and how the two sides line up

The Module 8 lab adds one custom visual — a status gauge — to a screen that is otherwise plain
Wisej.NET: labels, buttons, a list box. The gauge is a `Wisej.Web.Widget` named `widStatus`; its
visual is a JavaScript file, its look is a CSS file, and its data is an `Options` object that C# fills
from a service. This note records the decisions and the wiring.

## 1. Native control or Widget? (the decision table)

| Need on this screen | Choice | Why |
|---|---|---|
| Open / closed / load / status values, captions | Native `Label` | Fast to build, themed, no JavaScript to maintain — and they double as the verification card |
| Refresh, Set Healthy / Warning / Critical, Simulate error, theme switch | Native `Button` | Normal C# `Click` handlers; nothing special to draw |
| Event log | Native `ListBox` | Selection, scrolling and fonts come for free |
| A coloured gauge with a fill animation and a pulse | **`Widget`** | A browser-side visual that no built-in control provides neatly; ~50 lines of JS + 10 lines of CSS |
| A ticket list or table (Modules 4, 7, 10) | `DataGridView` | Works directly with C# data binding and selection events |
| A flashy flourish with no business value | Avoid | Maintenance risk for nothing |

Rule of thumb from the lesson: **native controls are the default; the Widget is for special visuals.**
One Widget on this page, everything else native.

## 2. How the Widget fits (the four layers)

```
Wisej.NET Page   (StatusPage.cs / .Designer.cs)   →  C# controls and layout: cards, labels, buttons, log
Widget wrapper   (widStatus : Wisej.Web.Widget)   →  loads CSS (Packages), runs the JS (InitScript), exposes Options
JavaScript UI    (Widgets/statusGauge.js + .css)  →  draws the gauge: init / update / pulse / render
Server events    (widStatus_WidgetEvent, buttons) →  refresh, status and data updates handled in C#
```

C# services still own the data (`Services/StatusService.cs`); the Widget owns a small browser-side
visual. The bridge is narrow on purpose: three display values go down, one small event comes up.

## 3. The C# ↔ JavaScript mapping

| C# (StatusPage.cs) | JavaScript (statusGauge.js) | What it does |
|---|---|---|
| `widStatus.Packages.Add(new Widget.Package { Name = "statusGauge-css", Source = "Widgets/statusGauge.css" })` | — | The stylesheet loads before the InitScript runs, once per session; more entries load in list order (a library before the plugin that needs it) |
| `widStatus.InitScript = File.ReadAllText(Application.MapPath("Widgets/statusGauge.js"))` | the whole file | Runs once in the browser with `this` = the client-side widget; it must define `this.init` and `this.update` |
| `widStatus.Options = new { percent = …, label = …, status = … }` | `options.percent`, `options.label`, `options.status` | Serialized to JSON; property names are **camel-cased** on the client (`Percent` would become `percent`, so this sample already uses lower-case names) |
| first render | `this.init(options)` | Builds the markup inside `this.container`, caches the elements, wires the click, draws once |
| `widStatus.Update()` after changing `Options` | `this.update(options, old)` | Redraws only if `percent`, `label` or `status` changed |
| `widStatus.Call("pulse")` | `this.pulse()` | Restarts the CSS animation — a named helper the server can call |
| `widStatus.WidgetEvent += widStatus_WidgetEvent` … `e.Type == "gaugeClick"`, `e.Data.percent` | `me.fireWidgetEvent('gaugeClick', { percent })` | The one event that supports a workflow; the payload is tiny |

Order matters on the way down: set `Options`, then `Update()`, then `Call()`. Wisej.NET sends them to
the browser as one batch, applied in order — `UpdateWidget(StatusInfo)` in `StatusPage.cs` does exactly
that and nothing else talks to the widget after load.

Three names to remember inside the JS file: `this.container` (the element reserved for our markup —
create children inside it, never replace the widget's own root), `options` (the plain object C#
assigned), `this.fireWidgetEvent(name, data)` (the way back to C#). And `var me = this;` at the top of
`init`: inside the click listener `this` is the DOM element, so `this.fireWidgetEvent` would be
undefined there — `me` is the captured widget.

## 4. What happens at runtime (lesson §6, as this page shows it)

1. The page renders and the browser creates the widget's element (`widStatus`, 420 × 140, inside the "Status widget" card).
2. Wisej.NET loads every entry in `Packages`, in order — here `Widgets/statusGauge.css`, fetched from the project folder as a static file.
3. It runs the `InitScript`, which defines `init`, `update`, `pulse` and `render` on the widget.
4. It calls `init(options)` with the serialized `Options` — the gauge appears at the load the service returned (42 %, "System load", `ok` → green).
5. Each time C# sets `Options` and calls `Update()`, the browser runs `update(options, old)`; `Call("pulse")` runs `pulse()` right after.
6. A click in the browser fires `gaugeClick`; C# handles it in `widStatus_WidgetEvent` like any button click and logs `Gauge clicked at 42%`.

The event log on the page prints steps 2–4 at load and steps 5–6 on every action, with timestamps.

## 5. If the widget stays blank (troubleshooting)

| Symptom | Likely cause | Where to look |
|---|---|---|
| Console error naming an undefined symbol at load time | `Packages` order — a plugin loaded before the library it needs | Reorder the `Packages` list; the CSS-only case here cannot hit this |
| `fireWidgetEvent is not a function` inside a callback | Lost `this` — the listener used `this` instead of the captured `me` | Top of `init`: `var me = this;` and use `me` in every callback |
| Empty container, no error | `init` never got the option it needed, or the InitScript was empty | Log `options` at the top of `init`; check `widStatus.InitScript.Length` in the event log (this page logs it at load) |
| 404 for `Widgets/statusGauge.css` in the Network tab | The file is not served — not marked `<Content>`, or the static file server is not rooted at the project folder | `WisejTrainingApp.csproj` (`<Content Include="Widgets\…">`), `Startup.cs` (`UseFileServer()` with `WebRootPath = "./"`) |
| `FileNotFoundException` at load | `Application.MapPath("Widgets/statusGauge.js")` did not resolve to the project folder | Run from the project folder (`dotnet run` there) or set `WEBSITE_PATH` as `launchSettings.json` does |
| Gauge does not move after a theme switch | It should — the CSS is scoped to `lw-gauge-*`; if it does not, a theme rule is overriding the container | Re-test with **Refresh Server Data** after **Switch theme**; DevTools → Elements → `.lw-gauge-fill` |

## Evidence

| Action | Event log | Screen |
|---|---|---|
| Page load | `StatusPage_Load → Packages += statusGauge-css …`, `… InitScript = File.ReadAllText(Application.MapPath("Widgets/statusGauge.js")) → N chars`, `… Options = { percent = 42, label = "System load", status = "ok" } → WidgetEvent += …`, `browser: loads the CSS → runs the InitScript … → init(options) → the gauge appears` | a green gauge at 42 % labelled *System load — 42%*; the data card shows Open 12, Closed 47, System load 42 %, Status `ok`, and the same Options text in `lblSent` |
| **Refresh Server Data** | `Widget refreshed with server data` | the fill animates to 49 % (then 55, 64 → amber, 75, 87 → red, 84 → amber, 53 → green, 58 …), pulses once, and `lblLoad` / `lblSent` show the same number — one C# call fed both |
| **Set Healthy / Warning / Critical** | `Status set to ok by C# rule (load 35 %)` / `… warn … 72 %` / `… error … 93 %` | the gauge recolours green / amber / red; `lblStatusValue` shows the same word in the same colour |
| Click the gauge | `Gauge clicked at 72%` (whatever it shows) | `lblStatus` confirms the browser → C# round trip and compares the number with `lblLoad` |
| **Switch theme**, then **Refresh Server Data** | `Application.LoadTheme("Material-3") …`, then `Widget refreshed with server data` | buttons, labels and the list box restyle; the gauge keeps its own look and still updates and pulses |
