# Deliverables 2 + 5 · Base theme choice and the layer-ownership note

Two lab deliverables share this file: the **base theme** chosen for the Adaptive Operations Console
with the reason it was chosen, and the **note explaining which layer owns each visual decision** in the
Module 1 shell, region by region. The lab guide does not name a separate theme-choice document, so the
choice is recorded here (and referenced from `VisualInventory.md`). The desktop screenshot this note
accompanies is taken by the learner at 1440 × 900 as `docs/screenshots/desktop-1440.png`.

## Base theme: Bootstrap-4

**Configured where it counts — for the running application**, not only in the designer:

- `Default.json` → `"theme": "Bootstrap-4"` (the Kestrel / `dotnet run` target; `Default.json` is never
  served, `Startup.cs` excludes `*.json` from the static file server).
- `Web.config` → `<add key="Wisej.DefaultTheme" value="Bootstrap-4"/>` (the `net10.0-windows` / IIS target).
- Confirmed at runtime: `MainPage_Load` traces `• server base theme: Bootstrap-4 …` and the status bar's
  right-hand label shows `theme: Bootstrap-4`, both read from `Application.Theme.Name`. If the two
  disagreed with what you see, the theme would be "set in the designer but not selected for the running
  application" — the lesson's first suspect when a theme does not apply.

**Why Bootstrap-4** (recorded before any colour or font was touched, as the lab asks):

1. **It is the base Module 2 inherits from.** `Themes/AdaptiveOps.theme` will carry
   `"inherit": "Bootstrap-4"` and override only tokens and a handful of appearances. Bootstrap-4's
   appearances are the most conventional of the embedded set — a flat `button` with clear `default /
   hovered / focused / pressed / disabled / checked` states, a `textbox` with `focused` and `invalid`
   states, `table-header-cell` / `table-row`, `tabview`, `tooltip/atom`, `panel` with `captionbar` —
   so every appearance the inventory needs already exists with the state set the course teaches
   ("state order"). Inheriting a theme that already has `invalid` and `checked` means Module 2 changes
   colours, not structure.
2. **Its metrics suit a dense business console.** Buttons are 30–36 px, editors 28 px, rows compact;
   the shell's toolbar (56), status bar (28) and 36-px rail buttons were sized against these metrics.
   Material-3/-4 add elevation and larger touch targets that fight a data-dense grid, Fluent's
   accent-driven look is the identity Module 2 should *decide*, not inherit, and the Blue/Classic/Vista
   families carry gradients and images the course would spend Module 2 removing.
3. **It is neutral about colour.** Bootstrap-4's palette is grey-on-white with one accent, so nothing in
   the shell today looks "branded by accident"; the brand colour appears exactly where the inventory
   says a token will own it (the metric strips), and the desktop screenshot documents a shell whose
   identity is still the base theme's — the honest "before" picture Module 2 needs.
4. **Web-conventional, so the CSS layer stays small.** Its generated classes and radii match what a web
   designer expects from a business dashboard; Module 3's scoped `Styles/AdaptiveOps.css` can stay
   limited to the truly app-specific parts (metric cards, embedded HTML) instead of correcting the base.

A light theme was preferred over BootstrapDark-4 because the course's tablet and phone screenshots are
compared against a light Theme Builder reference; a dark variant is a later `ClientTheme` exercise
(Module 3, session-only theme).

## Which layer owns each visual decision in the shell

The five layers, as the lesson names them: **theme** (identity of standard controls, tokens, states,
metrics), **control property** (the server-side control model: `Dock`, `Size`, `MinimumSize`,
`Padding`, `Anchor`, `Visible`, `AppearanceKey`, responsive properties — and, when misused, `BackColor` /
`ForeColor` / `Font`), **CssClass** (scoped application stylesheet), **CssStyle** (one-off inline
value), **layout container** (the Dock / Anchor engine and the layout panels). The decision rule the shell
follows: theme first for standard controls, containers for geometry, control properties for what the
container needs to know, CSS not at all in Module 1.

### Page (`MainPage`)

| Decision | Owned by | Evidence in code |
|---|---|---|
| The five regions and their order (Top, Bottom, Left, Right, Fill) | layout container — Dock, ordered by the `Controls.Add` sequence | `this.Controls.Add(workspacePanel, detailsPanel, navigationPanel, statusPanel, toolbarPanel)` at the end of `InitializeComponent()`; added last = docked first |
| The page always fills the browser viewport, so `Resize` reports the browser size | layout container (framework behaviour of `Page`) + control property (`Resize` event) | `MainPage_Resize` → `ReportWidth("Page.Resize")`; no `Bounds` set |
| Light grey ground behind the cards (#EEF2F7) | control property today (`this.BackColor`) → theme token `surfaceAlt` in Module 2 | `MainPage.Designer.cs`, `this.BackColor = Color.FromArgb(238, 242, 247)` |
| Look of every `Button`, `TextBox`, `ComboBox`, `DateTimePicker`, `DataGridView`, `ListBox` | theme (Bootstrap-4 appearances and states) | no `AppearanceKey`, `BackColor`, `ForeColor`, `Font`, `CssClass` or `CssStyle` on any of them |

### Toolbar (`toolbarPanel`, Top 56)

| Decision | Owned by | Evidence |
|---|---|---|
| Height 56 and position at the top | layout container (Dock=Top, `Size.Height`) | `toolbarPanel.Dock = Top; Size = (1348, 56)` |
| 8-px gap to the page edges and 4 px to the regions below | control property feeding the container (`Padding`) | `toolbarPanel.Padding = (8, 8, 8, 4)`; Dock ignores `Margin`, so the gap is the parent's padding |
| White card with a thin border | control property today (`BackColor = White`, `BorderStyle = Solid`) → theme `panel` appearance / `surface` token | `toolbarCard` |
| Buttons left to right at fixed x, 30 px high; the progress label stretches with the window | layout container (`Location` + `Anchor Top\|Left\|Right` on `lblProgress`) | `btnRefresh` at 272,7 … `btnClearTrace` at 708,7; `lblProgress.Anchor` |
| Button colours, hover/pressed/disabled, corner radius, font | theme (`button` appearance) | nothing set on the five buttons; Module 2 gives the primary ones `AppearanceKey = "action-button"` |
| App title 14 pt bold, progress text muted | control property today (`Font`, `ForeColor`) → theme `heading` font and `textMuted` token | `lblAppTitle.Font`, `lblProgress.ForeColor` |
| Tooltips on the four lab buttons | theme (`tooltip/atom`) for the look; control property (`ToolTipText`) for the text | `btnRefresh.ToolTipText = "Reload the tickets …"` |

### Navigation rail (`navigationPanel`, Left 220)

| Decision | Owned by | Evidence |
|---|---|---|
| Width 220, left edge, below the toolbar | layout container (Dock=Left; docked after Top/Bottom so it sits between them) | `navigationPanel.Dock = Left; Size = (220, 596)` |
| Buttons fill the rail's width and keep 8-px gutters | layout container (`Anchor = Top\|Left\|Right`, 186 px wide at design time) | `btnNavDashboard.Anchor …`; five buttons at y = 40, 84, 128, 172, 216 |
| Button look and states | theme (`button`) | nothing set; a `nav-button` appearance with a `checked` state is Module 2's job |
| "NAVIGATION" caption small, bold, muted | control property today (`Font` 8 bold, `ForeColor` #677085) → theme `textMuted` | `lblNavTitle` |
| Whether the rail is visible on a phone | responsive property (per-profile `Visible`) + `ResponsiveProfileChanged` — **not decided in Module 1** | the rail is always visible; see `VisualInventory.md` §4 |

### Workspace (`workspacePanel`, Fill, MinimumSize 320 × 240)

| Decision | Owned by | Evidence |
|---|---|---|
| Takes whatever the four edges leave; never collapses below 320 × 240 | layout container (Dock=Fill) + control property (`MinimumSize`) | `workspacePanel.Dock = Fill; MinimumSize = (320, 240)`; `VerifyShell()` checks both |
| Metric cards on top, banner under them, trace at the bottom, grid in the middle | layout container (Dock inside the workspace; child order `gridCard, tracePanel, bannerPanel, metricsPanel` so metrics dock first) | the `workspacePanel.Controls.Add` block and its comment |
| Four metric cards of equal width, 8 px apart | layout container (four `slot*` panels Dock=Left, 192 px, `Padding` right 8; card Dock=Fill inside) — Module 5 replaces this with a `FlowLayoutPanel`/`FlexLayoutPanel` that wraps | `slotOpen … slotClosed` |
| The coloured 4-px strip on each card | layout container (a `Panel` Dock=Top, height 4) for the geometry; control property today (`BackColor`) → theme tokens `brandPrimary` / `danger` / `warning` / `success` | `stripOpen.BackColor = (36, 84, 166)` etc. |
| Card surface, border, corner radius | control property today (`BackColor = White`, `BorderStyle = Solid`) → **CssClass** `metric-card` in Module 3 (rounded corners and shadow are app-specific chrome with no standard-control equivalent) | `cardOpen.BorderStyle` |
| Metric caption 8 pt bold muted, value 22 pt bold | control property today (`Font`, `ForeColor`) → theme `textMuted` + `heading` font | `lblOpenTitle`, `lblOpenValue` |
| The validation banner appears and pushes the grid down, disappears and gives the space back | layout container (Dock=Top, a hidden docked control takes no space) + control property (`Visible`) | `bannerPanel.Visible = false` in the Designer; `ShowBanner()` / `HideBanner()` toggle only `Visible` |
| Banner colours and bold text | control property today → theme `danger` tint / text | `lblBanner.BackColor / ForeColor / Font` |
| Grid fills the card, columns share the width by weight | layout container (Dock=Fill) + control property (`AutoSizeColumnsMode = Fill`, `FillWeight`, `MinimumWidth` per column) | `gridTickets`, `colTitle.FillWeight = 240F` … |
| Grid header, row, selection, hover look | theme (`table`, `table-header-cell`, `table-row`) | nothing set on the grid or its columns |
| Trace card 176 px high at the bottom, list in monospace | layout container (Dock=Bottom, `Padding` top 8) for the geometry; control property today (`listTrace.Font = monospace 9`) → theme `mono` font | `tracePanel`, `listTrace` |

### Details panel (`detailsPanel`, Right 340)

| Decision | Owned by | Evidence |
|---|---|---|
| Width 340 on the right edge | layout container (Dock=Right) | `detailsPanel.Dock = Right; Size = (340, 596)` |
| Two-column form: full-width Title, Priority/Status and Owner/Due side by side, Notes grows with the card, Save pinned to the bottom | layout container (`Location` + `Anchor`: right column `Top\|Right`, `txtNotes` `Top\|Bottom\|Left\|Right`, `btnSave` / `lblDetailsHint` `Bottom`) — Module 5 replaces this with a `TableLayoutPanel` | `txtTitle.Anchor`, `cboStatus.Anchor`, `txtNotes.Anchor`, `btnSave.Anchor` |
| Editor borders, focus ring, drop-down arrows, the date picker, the placeholder text look | theme (`textbox`, `combobox`, date-picker appearances; `focusFrame` via `settings`) | no visual property on `txtTitle`, `cboPriority`, `cboStatus`, `txtOwner`, `dtpDue`, `txtNotes`; placeholders via `Watermark` (a control property that only supplies the text) |
| How an *invalid* editor looks | theme (`textbox` `invalid` state) — **not used in Module 1**: the shell reports rejection through the banner, the status label and an `AlertBox`, the editor itself does not change | `SaveTicket()` catch block |
| Title 12 pt bold, subtitle and hint muted | control property today (`Font`, `ForeColor`) → theme `heading` / `textMuted` | `lblDetailsTitle`, `lblDetailsSubtitle`, `lblDetailsHint` |
| Whether the panel is visible / stacked at 1024 or 768 px | responsive property + `ResponsiveProfileChanged` (Module 6) — **not decided in Module 1** | always visible today |

### Status bar (`statusPanel`, Bottom 28)

| Decision | Owned by | Evidence |
|---|---|---|
| Height 28 at the bottom | layout container (Dock=Bottom) | `statusPanel.Dock = Bottom; Size = (1348, 28)` |
| Three labels: status left, width in the middle, theme right | layout container (Dock inside `statusCard`: `lblStatus` Left 200, `lblTheme` Right 300, `lblBrowserWidth` Fill; child order `lblBrowserWidth, lblTheme, lblStatus` so the Fill label is docked last) | `statusCard.Controls.Add` block |
| The width text itself | control property (`Text`) written by code from `Application.Browser.Size` / `Device` — behaviour, not styling | `ReportWidth()` in `MainPage.cs`, called from the constructor, `Load`, `Application.BrowserSizeChanged`, `Page.Resize`; `try / catch` leaves `Browser size unavailable — …` |
| Status colour green / orange / red | control property today (`ForeColor` switched in `SetStatus()`) → theme tokens `success` / `warning` / `danger` (`Application.Theme.GetColor`) in Module 2 | `SetStatus()` and its comment |
| Width label in monospace, theme label muted | control property today (`Font`, `ForeColor`) → theme `mono` font / `textMuted` | `lblBrowserWidth.Font`, `lblTheme.ForeColor` |

### Layers deliberately *not* used in Module 1

- **CssClass** — 0 occurrences. Nothing in the shell is app-specific enough yet; the metric card is the
  first candidate (Module 3), because rounded corners and a shadow on a plain `Panel` have no theme
  appearance to hang on and are not a one-off value either.
- **CssStyle** — 0 occurrences. The only "computed one-off" the console will ever need is a percentage
  width in the phone layout (Module 6); until then there is no legitimate use.
- **Resize code that sets `Bounds`** — none. `MainPage_Resize` reports; Dock lays out. Every width the
  lab asks to test (1440, 1024, 768, 390) is answered by the same five Dock values, and the two failures
  at 768 / 390 (horizontal overflow) are profile decisions for Module 6, not a reason to compute sizes.
- **AppearanceKey** — 0 occurrences. Semantic variants (`action-button`, `nav-button`) need the custom
  theme first; selecting a key the base theme does not define would silently fall back.

### The honest debt

The 16 `BackColor`, 12 `ForeColor` and 17 `Font` lines listed in `VisualInventory.md` §5 are all on
`Panel`s and `Label`s — card chrome and typography — and their values are the Module 2 tokens written
as literals. They violate the letter of the acceptance criterion and were kept on purpose so the shell
reads as a dashboard in the "before" screenshot; the point of Module 2 is to delete every one of them by
giving the theme a `surface`, `surfaceAlt`, `textMuted`, `heading` and `mono` and a `panel` appearance,
and to watch the shell look the same afterwards. No standard interactive control carries any of them,
which is the part of the criterion that protects the theme contract.

## Evidence

- **Theme selected for the running app** — the trace's second line `• server base theme: Bootstrap-4
  (selected in Default.json; no custom theme, no CSS in Module 1)` and the status bar `theme:
  Bootstrap-4`. Editing `Default.json` to `"theme": "Material-3"` and restarting restyles every button,
  editor, the grid and the list without a code change — the Dock geometry and the card chrome stay.
- **Containers own geometry** — resize the window: the toolbar and status bar keep 56 / 28 px, the rail
  and details keep 220 / 340, the workspace absorbs the difference and the grid columns redistribute;
  the trace shows `← client resize (Page.Resize): browser … · page … · workspace …` with no server-side
  size assignment behind it.
- **Visibility is a docked toggle** — click **Invalid ticket**: the red banner appears above the grid and
  the grid shrinks by 38 px; click **Reset**: it disappears and the grid grows back. `ShowBanner()` and
  `HideBanner()` touch only `Visible` and `Text`.
- **Theme owns the interactive controls** — hover and press any toolbar or rail button, focus an editor,
  select a grid row: every state colour comes from Bootstrap-4; there is no code path that sets a colour
  on them.
- **Screenshot** — `docs/screenshots/desktop-1440.png`, taken by the learner at 1440 × 900 with the
  first-load trace visible.
