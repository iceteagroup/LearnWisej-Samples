# OperationsConsole · Mastering the Control Library · Module 7

Local lab build for **Module 7 · Custom Widgets, Extensions, Theming, and Capstone** — the last
module, and the capstone hand-in. It follows the lesson guide, the lab / exam guide and the
walkthrough video: the **Widgets** section is no longer a placeholder but a real
`Wisej.Web.Widget` named `ratingWidget` wrapping a small star-rating JavaScript library, with the
bridge traced in both directions, every rule on the server, theming through packaged CSS, and the
capstone documentation.

The course is cumulative: this folder is the whole Operations Console as it stands after module 7 —
the Module 1 shell and `Shell/` contract, the Module 2 editor, the Module 3 frame, the Module 4
explorer, the Module 5 grid, the Module 6 dashboard, and this module's widget screen.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Mastering the Control Library Course/Module 7/OperationsConsole"
dotnet run -f net10.0 --urls http://localhost:5707
```

Then open <http://localhost:5707> and click **Widgets**. (Visual Studio: open
`OperationsConsole.slnx` in this folder, press F5.) Open the browser console — the lab's acceptance
criterion is that it stays clean.

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try

Open the **Widgets** section. The command row across the top of the page drives every path; the
**Event log** card on the right is the message trace, in the same format as the Application
Integration samples.

| Action | Path | What you should see |
|---|---|---|
| Open **Widgets** | ready state | Five grey stars, caption "Northwind Traders · not rated"; log `WidgetsPage ready · InitScript from embedded resource OperationsConsole.wwwroot.rating-init.js` and `ratingWidget packages: rating.css → rating.js …` |
| Hover the stars | client only | The stars preview in gold. **No server round trip** — this is the browser-only work the Widget exists for |
| **Click the 4th star** | success | `← JS→.NET ratingChanged {"value":4}` then `→ .NET→JS setSaved 4`; the stars stay gold with a **✓ Saved 4/5** badge; a green Toast top-right; green status "Rating 4/5 saved for Northwind Traders."; `Record: CUST-1042` in the diagnostic strip |
| **Read client state** | server → client (awaited) | `→ .NET→JS getState() (CallAsync — waiting for the browser)` then `← JS→.NET getState → value 4 · savedValue 4 · saved True · max 5 · theme bootstrap · narrow False`, printed in the bridge card |
| **Push 5 from server** | server → client (silent) | `→ .NET→JS Options.value 5 (via update(options, old) — applied silently)`; the widget shows 5 and **no** `ratingChanged` comes back; the card says it is shown but not saved |
| **Re-run init()** | re-initialisation | `→ .NET→JS reinit()`; the stars redraw once, same value, browser console still clean (the adapter tears the old library down first) |
| **Theme → Material-3** | theming | The whole console *and* the stars restyle (purple stars, rounder badge); `→ .NET→JS Options.theme material (rating.css variant — no colour crosses the bridge)`. Press again to go back to Bootstrap-4 |
| **Send malformed payload** | failure 1 | `→ .NET→JS sendRawPayload "seven"`, then `← JS→.NET ratingChanged {"value":"seven"}`, then `✗ payload rejected — "seven" is not a rating — expected a whole number between 1 and 5.`; amber status, an AlertBox, **nothing stored**, stars still clickable |
| **Send out-of-range payload** | failure 2 | Same shape with `{"value":9}` → `✗ payload rejected — 9 is outside the 1–5 scale.` |
| Tick **Simulate service failure**, click a star | failure 3 | `← JS→.NET ratingChanged {"value":3}` → `✗ RatingService.Save(3) failed — InvalidOperationException` → `→ .NET→JS ratingClearSaved (the value was not stored)`; red status, a friendly AlertBox, the saved badge disappears and the widget stays editable |
| Untick it, click a star | recovery | Saves normally, green status, badge back |
| **Refresh** in the shell | command | `WidgetsPage.RefreshSection() → re-read the rating and re-pushed Options to ratingWidget` |
| Resize the browser under 601 px | responsive | `profile changed → Phone (…)` and `→ .NET→JS Options.profile Phone → .is-narrow (smaller stars)`; the stars shrink to 26 px and the caption wraps |
| In DevTools: `app.getWidget("ratingWidget").widget` | debugging | The `RatingWidget` library instance, exactly where `docs/WidgetContract.md` says it lives. `rating.js` and `rating-init.js` both appear by name in the **Sources** panel |

## Where things live

```
OperationsConsole/
├─ Sections/WidgetsPage.cs / .Designer.cs   THIS MODULE — the widget screen: ratingWidget, the
│                                           WidgetEvent handler, the command row, the bridge card
├─ Widgets/RatingInitScript.cs              loads the InitScript from the embedded resource
│                                           (file fallback for the edit + F5 loop)
├─ wwwroot/
│  ├─ rating.js                             the library: global RatingWidget, stars, gestures,
│  │                                        a "change" event. No colours, no rules, no Wisej
│  ├─ rating.css                            EVERY colour and size, as CSS variables; theme
│  │                                        variants, the .saved state, the narrow-profile rule
│  └─ rating-init.js                        the adapter (InitScript, embedded): init / update /
│                                           setSaved / getState / reinit / sendRawPayload
├─ Models/RatingModel.cs                    customer, value, saved value, saved-at, save count
├─ Services/RatingService.cs                EVERY rule: TryNormalize (1..5, whole number),
│                                           Save (throws when SimulateFailure), Reload
├─ docs/
│  ├─ WidgetDecision.md                     which native controls were considered and why none fit
│  ├─ WidgetContract.md                     packages + load order, options, events, payload,
│  │                                        server handling, theming, debugging notes
│  ├─ CapstoneNotes.md                      the workbook template, one block per capstone screen
│  ├─ DemoScript.md                         the five-minute demo across all six screens
│  └─ ControlSelection.md                   (Module 1) the control family per area
├─ MainPage.cs / .Designer.cs               the shell (Modules 1 / 3) — untouched by this module
├─ Shell/                                   IConsoleShell, ConsoleLog, ISection — untouched
├─ ClientProfiles.json                      Phone ≤ 600 px, Tablet ≤ 1024 px, Desktop
└─ OperationsConsole.csproj                 + <EmbeddedResource> for wwwroot/rating-init.js
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| Decision note: native controls considered, why none fits, which extension type | `docs/WidgetDecision.md` |
| A `Widget` named `ratingWidget` with `rating.js` and `rating.css` in `Packages`, **in load order** | `Sections/WidgetsPage.Designer.cs` → the `ratingWidget` block (`rating-css` then `rating-js`) |
| `InitScript` whose `init` creates the client object inside `this.container`, captures `var me = this`, safe to run again | `wwwroot/rating-init.js` → `this.init` (step 1 `var me = this`, step 2 `_teardownRating()`, step 3 the child element); loaded by `Widgets/RatingInitScript.cs`, set in `WidgetsPage.ConfigureRatingWidget()` |
| Client callback calls `me.fireWidgetEvent("ratingChanged", { value })` | `wwwroot/rating-init.js` → `this._onRatingChange` (init step 5) |
| Server handles `WidgetEvent` in `ratingWidget_WidgetEvent`, checks `e.Type`, validates and normalises `e.Data` to 1–5, saves through a `RatingService` | `Sections/WidgetsPage.cs` → `ratingWidget_WidgetEvent` + `Services/RatingService.TryNormalize` / `Save` |
| Confirm back with a server-to-client call, expose `setSaved`, show a Toast / AlertBox | `WidgetsPage.SaveRating()` → `ratingWidget.Call("setSaved", value)` + `Toast`; `await ratingWidget.CallAsync("getState")` in `btnReadClientState_Click`; `this.setSaved` in `rating-init.js` |
| Theme through packaged CSS / the `StyleSheet` extender, responsive properties for the narrow profile, an accessible label | `wwwroot/rating.css` (variables + `.opc-rating--material` / `.is-narrow` + media query); `WidgetsPage.ApplyConsoleStyleSheet()`; `WidgetsPage.Application_ResponsiveProfileChanged`; `ratingWidget.AccessibleName` / `AccessibleDescription` in the designer |
| Document the contract, then `CapstoneNotes.md` and the 5-minute demo script | `docs/WidgetContract.md`, `docs/CapstoneNotes.md`, `docs/DemoScript.md` |
| Show every path: valid click saves; malformed / out-of-range rejected with nothing saved; failed save shows a friendly error and leaves the widget editable | `btnSendMalformed_Click` / `btnSendOutOfRange_Click` → `Call("sendRawPayload", …)` → `RejectPayload()`; `chkSimulateServiceFailure_CheckedChanged` → the `catch` in `SaveRating()` |
| Review & run with the browser console open | Both client files end with `//# sourceURL=`; `app.getWidget("ratingWidget")` is the registry entry |

## Self-check answers

### From the lab / exam guide

- **What would happen if a user edited the `ratingChanged` payload in the browser's developer tools
  before it reached your handler, and which line of your code stops it?**
  Nothing would be stored. The payload arrives as `WidgetEventArgs.Data`, a dynamic object built
  from whatever JSON the browser sent — a string, a number out of range, `null`, or no `value` field
  at all. The line that stops it is
  `if (!_ratings.TryNormalize(raw, out int value, out string error))` in
  `ratingWidget_WidgetEvent`: nothing below it runs until the value is a whole number inside 1–5,
  and `RatingService.Save` refuses out-of-range values a second time (defence in depth) in case a
  future caller forgets. Press **Send malformed payload** to watch it happen. The general rule from
  the reading: a browser payload is user input, and it is validated on arrival like any other.

- **Your widget renders blank after the Widgets tab is hidden and shown again: which three things do
  you check, in which order, and why is the startup HTML not the fix?**
  (1) **The browser console** — the Wisej.NET documentation names it first for `Widget` problems;
  an exception thrown inside `init` leaves an empty container and nothing else to see.
  (2) **Package loading**: did `/wwwroot/rating.css` and `/wwwroot/rating.js` both return 200, and
  did they load in that order? A missing global (`RatingWidget is not defined`) means the order or
  the path is wrong.
  (3) **Re-initialisation**: the framework re-creates the client widget when it is re-shown, so
  `init` runs again — if it appends a second host element, or reuses a library object whose DOM was
  thrown away, the result is a blank or duplicated widget. This adapter calls `_teardownRating()`
  at the top of `init`, so the second run destroys the previous library, unhooks its event, removes
  its host element and rebuilds. Press **Re-run init()** to prove it.
  Moving the script into `Default.html` is not the fix because a `<script>` there runs **once, at
  page load, before the Wisej.NET widget exists** — it cannot re-run when the widget is re-created,
  and it has no `this.container` to build into. `Packages`, `InitScript` (embedded resource) and
  control-level `Call` are the patterns that live with the widget's lifecycle.

- **Which rule in your widget could you move to JavaScript as an optimisation, and which one must
  never leave the server?**
  Movable: the hover preview and the arrow-key navigation (already client-side), and a check that
  ignores a click on the star that is already selected — it would save a round trip and the server
  behaves identically if the check is missing. Never movable: **a rating is a whole number from 1 to
  5**, which customer it belongs to, and the decision that a value was stored. Those live in
  `RatingService`; the browser copy of a rule can be edited by anyone with developer tools, so a
  client-side rule is only ever an *optimisation on top of* the server rule, never the only copy.

### From the lesson's checkpoints

- **Native controls, widgets, components, extender providers and icon packs — what is the
  difference?** A **Control** implements the server side *and* its browser widget, so it gets the
  designer, theming, events and server state for free. A **Widget** is the general-purpose client
  container for third-party or custom JavaScript UI inside a Wisej.NET container (what this module
  builds). A **Component** may live only on the server, or on both, and has no visual surface of its
  own (`Timer`). An **Extender Provider** adds properties and behaviour to *existing* controls —
  `ToolTip`, `ErrorProvider`, `StyleSheet`. An **Icon Pack** packages visual resources for the
  project and the designer. Naming the type first is what stops "reusable" from being confused with
  "new native control".
- **Where is the boundary between the JavaScript and the server?** JavaScript does rendering,
  gestures and animation; the server owns rules, state and identity. In this sample the boundary is
  literally three files: `rating.js` (no Wisej, no rules, no colours), `rating-init.js` (the only
  file that knows both), `RatingService.cs` (every rule).
- **Why is a colour in the JavaScript a problem?** Because `Application.LoadTheme`, the `StyleSheet`
  extender and the next developer all look somewhere else. Press **Theme → Material-3**: the stars
  follow the console because every colour is a CSS custom property in `rating.css` and the server
  only chooses the variant class.

## Known simplifications

- The rating store is in memory, per session (`RatingService`), and rates one fixed customer
  (`CUST-1042`, "Northwind Traders") so the screen stays about the bridge rather than about data.
- `ratingWidget` is a plain `Wisej.Web.Widget` configured on the page — the lab's shape. A
  production version would be a `Widget` subclass with typed properties and a `RatingChanged` .NET
  event (see the "improve in production" note in `docs/CapstoneNotes.md`).
- `Widget.GetResourceString(...)` is `protected` in Wisej.NET 4.1, so it is only usable from a
  `Widget` subclass; `Widgets/RatingInitScript.cs` reads the same embedded resource with ordinary
  reflection instead.
- The theme switch maps two themes (`Bootstrap-4` ⇄ `Material-3`) to two CSS variant classes by
  hand. A `Themes/*.mixin.theme` file would let the widget follow `Application.Theme` automatically.
