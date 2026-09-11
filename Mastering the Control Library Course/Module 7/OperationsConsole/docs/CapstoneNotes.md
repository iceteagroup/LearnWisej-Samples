# CapstoneNotes.md — Operations Console

**Mastering the Control Library · capstone hand-in.** One block per capstone screen — controls used, why, data size
assumptions, validation and feedback, layout, reusable code, one production improvement — plus the performance,
theming and AI-grounding note. The five-minute walkthrough is in `DemoScript.md`.

## Screen 1 · Application shell (Modules 1 and 3)

- **Controls used.** Module 1: a `Page` with docked `commandPanel` (Top), `statusPanel` (Bottom), `navigationPanel`
  (Left), `contentPanel` (Fill). From Module 3: `ToolBar` + `StatusBar` + `SplitContainer splitMain` + `TabControl
  tabDetail`, a `ContextMenu` on the navigation list.
- **Why these controls.** Docking and a splitter are native, themed, keyboard-reachable and resizable;
  `SplitContainer.Panel1Collapsed` is the narrow-profile answer in one property.
- **Data size assumptions.** Six sections, fixed.
- **Validation and feedback.** One status area with three levels (green / amber / red) plus a diagnostic strip
  (selected control, selected record, active profile, last refresh).
- **Layout choices.** Docking is applied from the last `Controls.Add` to the first, so the `Fill` control is added
  first and the bars last.
- **Reusable code extracted.** `Shell/IConsoleShell` + `Shell/ShellStatus` + `Shell/ISection`: no section holds a
  reference to `MainPage` or talks to another section.
- **Improve in production.** Make the section catalog data-driven so a new section is a row, not code.

## Screen 2 · Validated customer editor (Module 2)

- **Controls used.** `CustomerEditor` `UserControl`: `TextBox`, `ComboBox` (`DropDownList`, key ≠ display text),
  `DateTimePicker`, `NumericUpDown`, `ErrorProvider`, `ToolTip`, Save / Reset / Validate.
- **Why.** The control matches the value type, so impossible values cannot be typed.
- **Data size.** One record at a time.
- **Validation and feedback.** `ErrorProvider` next to the field, never a modal; `btnSave.ShowLoader` for the busy
  state; `Toast` on success, `AlertBox` on failure.
- **Reusable code.** The editor is a `UserControl` with a small public surface.
- **Improve in production.** Move the rules into the service so an import enforces the same ones.

## Screen 3 · Document explorer (Module 4)

- **Controls used.** Lazy `TreeView categoryTree`, virtual-mode `ListView documentList`, a shared `ImageList`,
  `DocumentDetailControl`.
- **Why.** A tree for containment, a details list for a homogeneous set; both have a large-data strategy built in.
- **Data size.** A few hundred documents; nodes load children on expand and the list creates items only for visible rows.
- **Validation and feedback.** `ShowLoader` while a page loads, an empty-category message, and the stable ID of the
  selected document in the diagnostic strip.
- **Reusable code.** `DocumentService`, `DocumentDetailControl`.
- **Improve in production.** A windowed page cache filled from `CacheVirtualItems`.

## Screen 4 · Orders `DataGridView` (Module 5)

- **Controls used.** `ordersGrid` with explicit columns, `BindingSource ordersSource`, an HTML status badge via
  `CellFormatting` + `AllowHtml`, a command column, a `MonthCalendar` cell editor, a filter strip and a status strip;
  virtual mode with `OrderCache`.
- **Data size.** 4,000 rows; the bound path is capped, the virtual path serves everything page by page.
- **Validation and feedback.** A rejected due-date edit puts the stored value back and says why; `ShowLoader` during a load.
- **Reusable code.** `OrderService`, `OrderCache`.
- **Improve in production.** Push filtering and paging into the query.

## Screen 5 · Dashboard and content (Module 6)

- **Controls used.** `ChartJS chartTickets`, `ProgressBar progressCompletion`, `PdfViewer pdfPreview`, `HtmlPanel`, `Upload`.
- **Why.** Each is the native answer to a content type; none needs custom JavaScript.
- **Data size.** Aggregates only: six months, twelve numbers.
- **Validation and feedback.** `Upload.AllowedFileTypes` / `MaxFileSize` reject in the browser and the server re-checks.
- **Layout.** One `RefreshDashboard(model)` repaints every tile from one model.
- **Improve in production.** Cache the aggregate.

## Screen 6 · Custom widget (Module 7)

- **Controls used.** `Wisej.Web.Widget ratingWidget` + `rating.js` + `rating.css` + the embedded `rating-init.js`,
  the `StyleSheet` extender, `Toast`, `AlertBox`, a message trace `ListBox`.
- **Why.** Native controls were weighed first (`WidgetDecision.md`); the gesture is browser-only work.
- **Data size.** One value per customer; one event per click, one confirmation back.
- **Validation and feedback.** The payload is validated on arrival (`RatingService.TryNormalize`); the store refuses
  out-of-range values again; in every failure case the widget stays editable.
- **Reusable code.** `RatingService`, `RatingModel`, `RatingInitScript`; `rating.js` has no dependency on this app.
- **Improve in production.** A `Widget` subclass with typed properties and a `RatingChanged` .NET event.

## Performance, theming and AI grounding

**Performance.** Bind small sets whole, page or virtualise large ones, aggregate before charting, one message per
gesture across the widget bridge.

**Theming.** `Application.LoadTheme` restyles the native controls; the widget's colours live in a packaged stylesheet
with a variant class the server selects; the `StyleSheet` extender is the override layer; `ClientProfiles.json` drives
the narrow profile.

**AI grounding.** `docs/ControlSelection.md`, `docs/WidgetDecision.md`, `docs/WidgetContract.md`, `docs/DemoScript.md`
and this file. The rule: **the simplest native control first, then an extender, then a `UserControl`, and a JavaScript
widget only when no native control fits** — and business rules never leave the server.

## Rubric self-assessment

| Area | Weight | Where it is demonstrated |
|---|---:|---|
| Control selection | 20% | `docs/ControlSelection.md`, `docs/WidgetDecision.md` |
| Layout and reuse | 15% | the Module 3 shell, `RecordHeader` / `StatusStrip`, `CustomerEditor`, `DocumentDetailControl` |
| Data controls | 20% | lazy `TreeView` + virtual `ListView` (M4), explicit-column and virtual `DataGridView` (M5) |
| Validation and feedback | 15% | `ErrorProvider` editor (M2), `TryNormalize` + Toast / AlertBox / status levels (M7) |
| Custom widget or extension | 15% | `docs/WidgetContract.md`, `Sections/WidgetsPage.cs`, `wwwroot/rating*` |
| Polish and documentation | 15% | narrow profile, accessible labels, one `docs/` file per deliverable |
