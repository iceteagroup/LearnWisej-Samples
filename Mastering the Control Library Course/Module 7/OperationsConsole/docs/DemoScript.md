# DemoScript.md — the five-minute Operations Console demo

**Module 7 · lab task 8 and the capstone hand-in.** The capstone brief's seven beats, timed to five
minutes, across the six required screens: **Editors · Layouts · Lists and Trees · DataGridView ·
Dashboard · Widgets**.

**Before you start:** run `dotnet run -f net10.0 --urls http://localhost:5707` from the
`OperationsConsole` folder, open the browser at a desktop width, open **DevTools → Console** (the
acceptance criterion is that it stays clean), and leave the **Event log** card visible — it is the
narration. Do a dry run once: the timing below assumes you never hunt for a button.

---

## 0:00 – 0:40 · Architecture (capstone beat 1)

**Say:** "One Wisej.NET application. A `Page` for the frame, one `UserControl` per section, and a
tiny shell contract in between so no section knows about any other."

**Do:**
- Point at the frame: `ToolBar` on top, `StatusBar` at the bottom, `SplitContainer` with navigation
  in `Panel1` and a `TabControl` in `Panel2`, and the Event log card on the right.
- Click one section, then another. Point at the Event log: every navigation is one line.
- Point at the diagnostic strip: selected control, selected record, active profile, last refresh.

**The one sentence to land:** "Sections talk to the shell through `ConsoleLog` — four methods —
and never to each other. That is why any module could be built on its own."

## 0:40 – 1:25 · Editors and validation (beat 2)

**Say:** "The editor matches the control to the value type, so impossible values are impossible to
type."

**Do:**
- Open **Editors**. Point at `NumericUpDown` with `Minimum`/`Maximum`, the `DateTimePicker` with
  `MinDate`/`MaxDate`, the `ComboBox` whose `ValueMember` is a key and whose `DisplayMember` is
  text.
- Clear the email, tab out: the `ErrorProvider` marker appears next to the field — **focus is not
  trapped**. Fix it; the marker clears.
- Press **Save**: the loader shows, then a Toast top-right.

**The one sentence:** "Errors sit next to the field and never block the user; the modal is reserved
for a real confirmation."

## 1:25 – 2:10 · Tree and list navigation (beat 3)

**Say:** "A tree for containment, a details list for a homogeneous set — and both have a large-data
strategy."

**Do:**
- Open **Lists and Trees**. Expand a category: the placeholder node is replaced on `AfterExpand` —
  children load only when they are needed.
- Select a document: the detail control fills, and the **stable ID** appears in the diagnostic
  strip. Say the words "not the row index".
- Scroll the list fast: `RetrieveVirtualItem` / `CacheVirtualItems` fetch only what is on screen.

**The one sentence:** "Nothing on the wire that is not on screen."

## 2:10 – 2:55 · The grid and large data (beat 4)

**Say:** "Every column is declared: header, width, format, alignment, read-only. Nothing is
inferred."

**Do:**
- Open **DataGridView**. Point at the currency column's format, the HTML status badge, the command
  column.
- Edit a cell and type something invalid: `CellValidating` rejects it and says why.
- Switch to the virtual-mode dataset: `RowCount` + `CellValueNeeded` + `DataRead`, same screen,
  thousands of rows, unchanged scrolling.

**The one sentence:** "If the data got ten times bigger, this screen would not change — only which
of the two paths it uses."

## 2:55 – 3:30 · Dashboard and content (beat 5)

**Say:** "Five content types, five native controls, zero custom JavaScript — which is what makes the
next screen a deliberate decision rather than a habit."

**Do:**
- Open **Dashboard**. The ChartJS trend, the `ProgressBar`, the PDF preview, the HTML panel.
- Press **Refresh**: one `RefreshDashboard(model)` repaints every tile from one model, so the tiles
  can never show two different moments.
- Drop an oversized file on the `Upload`: it is rejected client-side with a readable message.

## 3:30 – 4:35 · The custom widget, both directions (beat 6) — the heart of the demo

**Say:** "This is the one screen where no native control fit. The reasoning is in
`docs/WidgetDecision.md`; the contract is in `docs/WidgetContract.md`."

**Do — success path (15 s):**
- Open **Widgets**. Click the 4th star.
- Read the Event log out loud:
  `← JS→.NET ratingChanged {"value":4}` … `→ .NET→JS setSaved 4`.
- The stars go gold, the **✓ Saved 4/5** badge appears, a Toast confirms, the status area is green.

**Do — server → client (10 s):**
- Press **Read client state**: `await ratingWidget.CallAsync("getState")` comes back with the
  browser's own view of the widget, printed in the bridge card.
- Press **Push 5 from server**: `Options.value = 5` + `Update()` — the widget changes and **no**
  `ratingChanged` comes back, because the adapter applies a server-driven change silently.

**Do — the payload is not trusted (15 s):**
- Press **Send malformed payload**. The client fires `ratingChanged {"value":"seven"}` — exactly
  what a user with developer tools can type.
- The Event log shows it arriving and then `✗ payload rejected`. Nothing is stored, the stars are
  still clickable.
- (Optional, if you have the seconds: **Send out-of-range payload** → `9` rejected the same way.)

**Do — the save fails (10 s):**
- Tick **Simulate service failure**, click a star: friendly `AlertBox`, red status, the saved badge
  is dropped — and the widget is still editable.
- Untick it, click a star again: saved. That is the recovery.

**Do — theming (10 s):**
- Press **Theme → Material-3**. The native controls *and* the stars restyle together.
- **The line to say:** "There is not one colour in `rating.js`. They are all CSS variables in
  `rating.css`, and the server only tells the widget which variant to wear."

**Do — the console (5 s):**
- Show DevTools: no errors. Type `app.getWidget("ratingWidget").widget` — the library instance is
  exactly where the contract says it is.

## 4:35 – 5:00 · What the AI assistant should answer (beat 7)

**Say:** "If someone asks my assistant why a control was chosen, it should not guess — it should
quote these notes."

**Do:** show the `docs/` folder and say the rule out loud:

> "Start with the simplest **native control** that expresses the user's task — it already has
> design-time support, theming, events and server state. Then an **extender**. Then a
> **`UserControl`** for a reusable server-side composition. A **`Widget`** only when the work is
> genuinely browser-only, and even then every business rule stays on the server and the payload is
> validated on arrival."

Then name the file for each screen: `ControlSelection.md`, `EditorDecisions.md`, `LayoutNotes.md`,
`ExplorerNotes.md`, `GridDecisions.md`, `DashboardNotes.md`, `WidgetDecision.md`,
`WidgetContract.md`, `CapstoneNotes.md`.

---

## If you are running short

Drop, in this order: the out-of-range payload (0:10), the `Upload` rejection (0:10), the
virtual-mode switch on the grid (0:15). **Never** drop the widget's failure paths or the theme
switch — they carry two of the six rubric areas between them.

## Rubric coverage of this script

| Beat | Rubric area | Weight |
|---|---|---:|
| 0:00 architecture, 0:40 editors | Control selection | 20% |
| 0:00 frame, 1:25 explorer layout | Layout and reuse | 15% |
| 1:25 tree/list, 2:10 grid | Data controls | 20% |
| 0:40 ErrorProvider, 3:30 rejection + failure | Validation and feedback | 15% |
| 3:30 the whole widget beat | Custom widget or extension | 15% |
| 4:15 theme switch, 4:35 the docs | Polish and documentation | 15% |

## Evidence (what a reviewer should see)

Running the script end to end produces, in the Event log, at least these lines in this order:
`✓ showing Widgets` → `← JS→.NET ratingChanged {"value":4}` → `→ .NET→JS setSaved 4` →
`→ .NET→JS getState() (CallAsync — waiting for the browser)` → `← JS→.NET getState → value 4 …` →
`→ .NET→JS Options.value 5 (via update(options, old) — applied silently)` →
`← JS→.NET ratingChanged {"value":"seven"}` → `✗ payload rejected — …` →
`✗ RatingService.Save(3) failed — InvalidOperationException` →
`→ .NET→JS Options.theme material (rating.css variant — no colour crosses the bridge)`.
