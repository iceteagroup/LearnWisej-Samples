# CapstoneNotes.md — Operations Console

**Mastering the Control Library · capstone hand-in.**
Written from the Candidate Workbook template: *controls used · why these controls · data size
assumptions · validation and feedback choices · layout choices · reusable code extracted · one
thing I would improve in production*, one block per capstone screen, plus the performance, theming
and AI-grounding note the capstone brief asks for.

The Operations Console is one Wisej.NET 4 application (`OperationsConsole`, .NET 10, port 5707)
grown across seven labs. This module's own work — the `ratingWidget` screen — is the last block;
the earlier blocks describe what modules 1–6 build, which is what the five-minute demo walks
through (see `DemoScript.md`).

---

## Screen 1 · Application shell (Modules 1 and 3)

- **Controls used.** `Page` with docked areas — `commandPanel` (Top), `statusPanel` (Bottom),
  `navigationPanel` (Left), `pnlEventLog` (Right), `contentPanel` (Fill) — replaced from Module 3 by
  `ToolBar` + `StatusBar` + `SplitContainer splitMain` + `TabControl tabDetail`. `Label`,
  `Button`, `CheckBox`, `ListBox`.
- **Why these controls.** Docking and a splitter are native, themed, keyboard-reachable and
  resizable; a hand-built frame would reimplement all four. `SplitContainer.Panel1Collapsed` is the
  narrow-profile answer in one property instead of a JavaScript layout.
- **Data size assumptions.** Six sections, fixed. The Event log is a `ListBox` that grows for one
  session; a production console would cap it (a ring buffer of ~500 lines) or move it to a
  `DataGridView` in virtual mode.
- **Validation and feedback.** One status area with three levels (green / amber / red) plus a
  diagnostic strip (selected control, selected record, active profile, last refresh). Every action
  is logged with a timestamp.
- **Layout choices.** Dock order is deliberate: docking is applied from the **last** `Controls.Add`
  to the first, so the `Fill` control is added first and the Top/Bottom bars last.
- **Reusable code extracted.** `Shell/IConsoleShell` + `Shell/ConsoleLog` + `Shell/ISection`: no
  section ever holds a reference to `MainPage`, and no section talks to another section.
- **Improve in production.** Persist the Event log per user and give it a filter; make the section
  catalog data-driven so a new section is a row, not a button.

## Screen 2 · Validated customer editor (Module 2)

- **Controls used.** `CustomerEditor` `UserControl`: `TextBox`, `ComboBox`
  (`DropDownStyle = DropDownList`, `ValueMember` ≠ `DisplayMember`), `DateTimePicker`,
  `NumericUpDown`, `ErrorProvider`, `ToolTip`, Save / Reset / Validate buttons.
- **Why these controls.** The editor matches the control to the **value type**, not to the screen:
  a date gets a date picker with `MinDate`/`MaxDate`, a bounded number gets `NumericUpDown` with
  `Minimum`/`Maximum`/`Increment`, a closed list gets a non-editable `ComboBox`. Every impossible
  value is then impossible to type.
- **Data size assumptions.** One record at a time; the lookup lists are small enough to bind whole.
- **Validation and feedback.** `ErrorProvider` next to the field, never a modal; `Validating` sets
  the error and lets focus move on; `btnSave.ShowLoader` for the busy state; `Toast` on success and
  `AlertBox` on failure, both top-right so they never cover the buttons.
- **Layout choices.** Label/editor pairs anchored `Top|Left|Right` so the editor grows with the card.
- **Reusable code extracted.** The whole editor is a `UserControl` with a small public surface
  (`Customer` in/out, `Saved` event) — the reuse question the labs keep asking.
- **Improve in production.** Move the validation rules into the service so a batch import enforces
  the same ones, and drive the `ErrorProvider` from the returned rule results.

## Screen 3 · Document explorer (Module 4)

- **Controls used.** `TreeView categoryTree` (lazy, `AfterExpand` + placeholder nodes),
  `ListView documentList` (`View.Details`, `VirtualMode`), a shared `ImageList`,
  `DocumentDetailControl`.
- **Why these controls.** A tree expresses containment; a details list expresses a homogeneous set
  with columns. Both are native and both have a large-data strategy built in.
- **Data size assumptions.** A few hundred documents generated in memory, but the code is written
  for far more: nodes load children only when expanded, and the list retrieves items through
  `RetrieveVirtualItem` / `CacheVirtualItems`, so the number of items on the wire is the number of
  visible rows, not the number of rows in the store.
- **Validation and feedback.** `ShowLoader` while a page is fetched; an honest empty state; the
  selected record's **stable ID** goes to `ConsoleLog.Record(...)` — identity never comes from a row
  index.
- **Layout choices.** Tree left, list centre, detail right, all inside the Module 3 splitter.
- **Reusable code extracted.** `DocumentService` (paging + generation) and `DocumentDetailControl`.
- **Improve in production.** Cache pages with an eviction policy and cancel in-flight fetches when
  the selection changes.

## Screen 4 · Orders `DataGridView` (Module 5)

- **Controls used.** `ordersGrid` with `AutoGenerateColumns = false` and explicit columns, a
  `BindingSource ordersSource`, an HTML status badge via `CellFormatting` + `AllowHtml`, a
  `DataGridViewButtonColumn` command column, a custom cell `Editor`, a filter strip and a status
  strip; virtual mode with `OrderCache`.
- **Why these controls.** The grid is the screen's whole job, so every column is declared —
  header, width, format, alignment, read-only — instead of being inferred from the model.
- **Data size assumptions.** A few thousand rows bound directly; the virtual-mode path
  (`RowCount` + `CellValueNeeded` + `DataRead`) is there for the "ten times bigger" question, and
  the demo switches between the two so the difference is visible.
- **Validation and feedback.** `CellValidating` rejects a bad cell and says why; `ShowLoader`
  during a fetch; the row count in the status strip.
- **Layout choices.** Filter strip Top, status strip Bottom, grid Fill, `AutoSizeColumnsMode.Fill`.
- **Reusable code extracted.** `OrderService`, `OrderCache`, and the badge formatter.
- **Improve in production.** Push sorting, filtering and paging into the query rather than into the
  cache, and encode every value that reaches an `AllowHtml` cell (the sample already does).

## Screen 5 · Dashboard and content (Module 6)

- **Controls used.** `ChartJS chartTickets` (`Wisej-4-ChartJS`), `ProgressBar progressCompletion`,
  `PdfViewer pdfPreview`, `HtmlPanel`, `Upload`.
- **Why these controls.** Each one is the native answer to a content type: a chart for a trend, a
  bar for a percentage, a PDF viewer for a document, an HTML panel for constrained markup, an
  upload for a file. None of them needed custom JavaScript — which is exactly the point the module
  makes before Module 7 introduces one.
- **Data size assumptions.** A dashboard summarises: dozens of points, not thousands. Aggregation
  happens in `DashboardService`, never in the chart.
- **Validation and feedback.** `Upload.AllowedFileTypes` / `MaxFileSize` reject client-side, and the
  `Error` event explains why; the server re-checks what arrives.
- **Layout choices.** One `RefreshDashboard(model)` method repaints every tile from one model, so
  the screen can never show two different moments in time.
- **Reusable code extracted.** `DashboardService` and the single-model refresh method.
- **Improve in production.** Cache the aggregate and refresh on a timer rather than on every visit;
  encode everything that reaches the `HtmlPanel`.

## Screen 6 · Custom widget (Module 7 — this lab)

- **Controls used.** `Wisej.Web.Widget ratingWidget` + `wwwroot/rating.js` + `wwwroot/rating.css` +
  `wwwroot/rating-init.js` (embedded `InitScript`), `Wisej.Web.StyleSheet` extender, `Toast`,
  `AlertBox`, a command row of `Button`s and one `CheckBox`.
- **Why these controls.** Four native controls were weighed first and the reasoning is written down
  in `WidgetDecision.md`. The gesture — hover preview, one-click commit, a purely visual saved
  state — is browser-only work, which is what `Widget` exists for.
- **Data size assumptions.** One value per customer. The interesting size question here is not rows
  but **messages**: one event per click, one confirmation back, nothing polled, nothing streamed.
- **Validation and feedback.** The payload is validated on arrival
  (`RatingService.TryNormalize`) before anything else touches it; the store refuses out-of-range
  values again; `Toast` on success, `AlertBox` + red status on failure, amber status on rejection —
  and in every failure case the widget stays editable.
- **Layout choices.** The page is self-contained and docked `Fill` (command row Top, cards in an
  `AutoScroll` panel) so it works both in the Module 1 `contentPanel` and in the Module 3 `TabPage`.
- **Reusable code extracted.** `Services/RatingService`, `Models/RatingModel`,
  `Widgets/RatingInitScript` (resource loading), and the adapter itself — `rating.js` has no
  dependency on this application at all.
- **Improve in production.** Give the widget its own `Widget` subclass with typed properties
  (`Value`, `Max`, `Caption`) and a `RatingChanged` .NET event, so consumers never touch `Options`
  or a magic string; add a `Themes/*.mixin.theme` file so the variant follows `Application.Theme`
  automatically instead of being mapped in C#.

---

## Performance, theming and AI grounding

**Performance.** Every screen states its size assumption above and uses the matching strategy: bind
small sets whole, page or virtualise large ones, aggregate before charting, and send one message
per gesture across the widget bridge. The two things that would break first at ten times the data
are the unbounded Event log and the Orders cache's eviction policy — both are named, not hidden.

**Theming.** One theme drives everything: `Application.LoadTheme` restyles the native controls, and
the custom widget follows because its colours live in a packaged stylesheet with a variant class the
server selects, not in JavaScript. The `StyleSheet` extender is the application-level override
layer. Responsive behaviour comes from `ClientProfiles.json` (Phone ≤ 600 px, Tablet ≤ 1024 px,
Desktop) via `Application.ResponsiveProfileChanged`, plus one CSS media query so the widget still
behaves outside this console.

**AI grounding.** The notes an assistant needs are next to the code, not in a wiki:
`docs/ControlSelection.md` (why each control family), `docs/WidgetDecision.md` (why no native
control fit), `docs/WidgetContract.md` (packages, options, events, payload, calls, debugging),
`docs/DemoScript.md` (the five-minute walkthrough) and this file. The rule an assistant should
repeat when asked "why this control?" is the course's own: **the simplest native control that
expresses the user's task, then an extender, then a `UserControl`, and a JavaScript widget only
when no native control fits** — and business rules never leave the server.

## Rubric self-assessment

| Area | Weight | Where it is demonstrated |
|---|---:|---|
| Control selection | 20% | `docs/ControlSelection.md`, `docs/WidgetDecision.md`; the alternatives table in each |
| Layout and reuse | 15% | Module 3 shell (`ToolBar`/`StatusBar`/`SplitContainer`/`TabControl`), `UserControl`s in every module, `Shell/` contract |
| Data controls | 20% | Lazy `TreeView` + virtual `ListView` (M4), explicit-column and virtual-mode `DataGridView` (M5), stable IDs through `ConsoleLog.Record` |
| Validation and feedback | 15% | `ErrorProvider` editor (M2), `TryNormalize` + Toast / AlertBox / status levels (M7), Event log everywhere |
| Custom widget or extension | 15% | `docs/WidgetContract.md`, `Sections/WidgetsPage.cs`, `wwwroot/rating*.js|css`, all three failure paths clickable |
| Polish and documentation | 15% | Theme switch, narrow profile, accessible labels, one `docs/` file per deliverable, README with a lab-step map |
