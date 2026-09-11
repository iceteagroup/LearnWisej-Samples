# DemoScript.md — the five-minute Operations Console demo

The capstone's beats, timed to five minutes, across the six screens: **Editors · Layouts · Lists and Trees ·
DataGridView · Dashboard · Widgets**.

**Before you start:** run `dotnet run -f net10.0 --urls http://localhost:5707` from the `OperationsConsole` folder,
open the browser at a desktop width and open **DevTools → Console** (it must stay clean).

## 0:00 – 0:40 · Architecture

**Say:** "One Wisej.NET application. A `Page` for the frame, one `UserControl` per section, and a small shell contract
in between so no section knows about any other."

**Do:** point at the `ToolBar`, the `StatusBar`, the `SplitContainer` with navigation and a `TabControl`. Click two
sections; point at the StatusBar: status, profile, record count, selected record, last refresh.

## 0:40 – 1:25 · Editors and validation

**Do:** open **Editors**. Point at `NumericUpDown` (`Minimum`/`Maximum`), `DateTimePicker` (`MinDate`/`MaxDate`) and
the `ComboBox` whose stored key differs from its text. Type a malformed email and tab out: the `ErrorProvider` mark
appears and focus is not trapped. Fix it, press **Save**: loader, then a Toast.

## 1:25 – 2:10 · Tree and list navigation

**Do:** open **Lists and Trees**. Expand a category — children load on `AfterExpand`. Select Contoso ▸ Contracts: the
footer reads "8 of 312 items created". Select a document: the detail control fills and the stable ID appears in the
StatusBar.

## 2:10 – 2:55 · The grid and large data

**Do:** open **DataGridView**. Point at the explicit columns, the HTML status badge, the command column. Double-click
a due date and pick a day in the past: the service rejects it and the cell keeps its value. Tick **Virtual mode**:
4,000 rows, one page fetch per 200-row block.

## 2:55 – 3:30 · Dashboard and content

**Do:** open **Dashboard**. Chart, `ProgressBar`, PDF preview, HTML panel. Press **Refresh**: one
`RefreshDashboard(model)` repaints every tile. Choose an oversized file in **Upload report**: rejected in the browser
with a readable message.

## 3:30 – 4:35 · The custom widget

**Do — success:** open **Widgets**, click the 4th star. The message trace shows `← JS→.NET ratingChanged {"value":4}`
then `→ .NET→JS setSaved 4`; the stars show **✓ Saved 4/5**, a Toast confirms, the status is green.

**Do — the payload is not trusted:** in the browser console run
`app.getWidget("ratingWidget").fireWidgetEvent("ratingChanged", { value: "seven" })`. The server rejects it with a
clear message; nothing is stored and the stars are still clickable.

**Do — the save fails:** tick **Simulate service failure**, click a star: friendly AlertBox, red status, the saved
badge is dropped, the widget stays editable. Untick it and click again: saved.

**Say:** "There is not one colour in `rating.js`. They are all CSS variables in `rating.css`."

## 4:35 – 5:00 · What the AI assistant should answer

Show the `docs/` folder and say the rule: "Start with the simplest native control that expresses the user's task. Then
an extender. Then a `UserControl`. A `Widget` only when the work is browser-only — and every business rule stays on the
server."

## If you are running short

Drop, in this order: the upload rejection, the virtual-mode switch. Never drop the widget's failure paths.
