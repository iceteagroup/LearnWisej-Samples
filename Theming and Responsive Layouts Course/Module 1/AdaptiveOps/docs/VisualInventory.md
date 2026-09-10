# Deliverable 3 · VisualInventory.md — Adaptive Operations Console

The inventory the theme, layout and profile work of Modules 2–7 is planned from. Every row carries an
**Owned by** column naming one of the five layers the lesson defines: **theme** (appearance / state /
token), **control property** (`BackColor`, `Font`, `Dock`, `MinimumSize`, `AppearanceKey`, responsive
properties …), **CssClass** (scoped application stylesheet), **CssStyle** (one-off inline value) or
**layout container** (Dock / Anchor / the layout panels). "Module 1" describes the shell as it runs today
(`MainPage.Designer.cs`); "Target" is where the course moves the decision.

Base theme: **Bootstrap-4**, selected for the running application in `Default.json` (`"theme":
"Bootstrap-4"`) and, for the Windows/IIS target, in `Web.config` (`Wisej.DefaultTheme`). The reason for
the choice is recorded in [`VisualLayers.md`](VisualLayers.md) § Base theme.

## 1 · Controls that need a custom appearance

Everything in this table is drawn by the **base theme** in Module 1: no control in the shell sets an
`AppearanceKey`, a `CssClass` or a `CssStyle` (grep evidence in §5). The table records what Module 2
will have to define in `Themes/AdaptiveOps.theme`.

| Control (name in the shell) | Module 1 look comes from | Appearance / state the theme must own | Owned by (target) |
|---|---|---|---|
| Command buttons — `btnRefresh`, `btnSimulateLoad`, `btnInvalidTicket`, `btnReset`, `btnClearTrace`, `btnSave` | Bootstrap-4 `button` (default, hovered, focused, pressed, disabled) | a semantic `action-button` appearance (inherits `button`) painting `brandPrimary` with its own hovered / pressed / focused / disabled states; selected with `AppearanceKey = "action-button"` on the primary commands only | theme (appearance) + control property (`AppearanceKey`) |
| Navigation rail buttons — `btnNavDashboard … btnNavHelp` | Bootstrap-4 `button`, `Anchor = Top\|Left\|Right` | a `nav-button` appearance with a `checked`/selected state; the rail becomes a `Shell/NavigationRail` UserControl later | theme (appearance); geometry stays with the layout container |
| Metric-card panels — `cardOpen`, `cardOverdue`, `cardMine`, `cardClosed` (+ `stripOpen…` 4-px strips) | `Panel` `BackColor = White`, `BorderStyle = Solid`; strip `BackColor` = the token colour | card surface (`surface` token), border, radius and shadow: an app-specific look with no standard-control equivalent → a scoped `metric-card` class in `Styles/AdaptiveOps.css` (Module 3); the strip colour becomes a token reference | CssClass (chrome) + theme (colour tokens) |
| Region cards — `toolbarCard`, `navigationCard`, `gridCard`, `traceCard`, `detailsCard`, `statusCard` | `Panel` `BackColor = White`, `BorderStyle = Solid` | `panel` appearance (`surface` background, border colour) so the white comes from the theme, not from 9 `BackColor` lines | theme (`panel` appearance) |
| Tabs (none in Module 1; Module 4/5 adds a `TabControl` to the details area on tablet) | — | `tabview` (`bar`, `pane`, `page` components) with a `checked` page state in the brand colour | theme (appearance) |
| Grid header — `gridTickets` (`DataGridView`, 6 columns, `AutoSizeColumnsMode = Fill`) | Bootstrap-4 `table` / `table-header-cell` / `table-row` | `table-header-cell` (background `surfaceAlt`, font `heading`-derived, `textMuted`), `table-row` `selected` / `hovered` states | theme (appearance) |
| Invalid editor — `txtTitle`, `txtOwner`, `cboPriority`, `cboStatus`, `dtpDue`, `txtNotes` | Bootstrap-4 `textbox` / `combobox`; Module 1 reports validation with the banner + `AlertBox`, the editors themselves never change look | `textbox` `invalid` state (border `danger`, focus frame `focusFrame`) applied when the server rejects a field; same for `combobox` and the date picker | theme (state) |
| Tooltips — `ToolTipText` on the four lab buttons | Bootstrap-4 `tooltip` / `atom` | `tooltip` appearance: `surface`/`textMain`, `default` font, radius from `settings.borderRadius` | theme (appearance) |
| Trace list — `listTrace` (`ListBox`, `Font = monospace 9`) | Bootstrap-4 `listview`/list item + control `Font` | `mono` font token referenced from a `trace-list` appearance or class | theme (font token) |
| Status label — `lblStatus` (`● ready / loading / validation error`) | `ForeColor` switched in `SetStatus()` between three hard-coded colours | `success` / `warning` / `danger` tokens read with `Application.Theme.GetColor(...)` or three states on a `status-label` appearance | theme (colour tokens) |
| Banner — `bannerPanel` / `lblBanner` | `BackColor` #FDECEA, `ForeColor` #B23B27, `Font` default 9 bold; docked Top, hidden by default | `danger` tint / `danger` text from tokens, `heading`-derived font; visibility stays a docked `Visible` toggle | theme (tokens) + layout container (Dock) |

## 2 · Colour and font tokens

The ten colours and three fonts the AdaptiveOps theme defines in Module 2 (`colors` / `fonts` sections of
`Themes/AdaptiveOps.theme`, inheriting Bootstrap-4). "Module 1 value" is the literal the shell uses today,
where it uses one; "Owned by" is the layer that owns the value once the token exists.

| Token | Role in the console | Module 1 value (where it appears today) | Owned by |
|---|---|---|---|
| `brandPrimary` | primary commands, selected navigation item, "Open" metric strip | `Color.FromArgb(36, 84, 166)` = #2454A6 on `stripOpen` | theme (colour) |
| `brandAccent` | highlights, badges, the accent on the app title | not used yet (#F59E0B planned) | theme (colour) |
| `surface` | card backgrounds, editors, toolbar | `Color.White` on the 9 region/metric cards | theme (colour) |
| `surfaceAlt` | page background behind the cards, grid header | `Color.FromArgb(238, 242, 247)` = #EEF2F7 on `MainPage.BackColor` (Module 2 picks the final tint, cookbook proposes #F6F7FB) | theme (colour) |
| `textMain` | body text, values in the metric cards | Bootstrap-4 default text colour (nothing overrides it) | theme (colour) |
| `textMuted` | captions: `lblNavTitle`, the four `lbl*Title`, `lblProgress`, `lblDetailsSubtitle`, `lblDetailsHint`, `lblTheme` | `Color.FromArgb(103, 112, 133)` = #677085 (9 `ForeColor` lines) | theme (colour) |
| `danger` | validation banner, `● validation error`, "Overdue" strip, `invalid` editor border | `Color.FromArgb(180, 35, 24)` = #B42318 on `stripOverdue` and in `SetStatus(Error)`; banner tint #FDECEA / text #B23B27 | theme (colour) |
| `warning` | `● loading`, "Assigned to me" strip | `Color.FromArgb(181, 71, 8)` = #B54708 on `stripMine` and in `SetStatus(Warn)` | theme (colour) |
| `success` | `● ready`, "Closed this week" strip | `Color.FromArgb(2, 122, 72)` = #027A48 on `stripClosed` and in `SetStatus(Normal)`; the Designer's initial `lblStatus.ForeColor` is #1F9D57 (replaced on first `SetStatus`) | theme (colour) |
| `focusFrame` | keyboard focus ring on every focusable control | Bootstrap-4 focus decorator (`settings.focusBorderSize`) | theme (colour + settings) |
| `default` font | all body text, editors, grid cells, buttons | Bootstrap-4 `default` font; the shell only *scales* it: `new Font("default", 8F/9F/10F/11F/12F/14F/22F, Bold)` on 15 labels | theme (font) |
| `heading` font | app title (`lblAppTitle` 14 bold), card titles (`lblWorkspaceTitle` 11, `lblDetailsTitle` 12, `lblTraceTitle` 10), metric values (`lbl*Value` 22 bold) | control `Font` properties today | theme (font) |
| `mono` font | trace list (`listTrace`), width label (`lblBrowserWidth`) | `new Font("monospace", 9F)` on both | theme (font) |

## 3 · Layout regions

The five regions are transparent `Panel`s docked on `MainPage`; each contains one white "card" `Panel`
with `Dock = Fill`, and the region's `Padding` is the gap between neighbours (Dock ignores `Margin`).
**Dock order = child order**: the `Controls.Add` block at the end of `InitializeComponent()` adds
`workspacePanel, detailsPanel, navigationPanel, statusPanel, toolbarPanel`, and the control added last is
docked first — so the toolbar takes the top edge, then status the bottom, navigation the left, details the
right, and the workspace fills what is left. `VerifyShell()` checks this at startup and traces `• server
dock order verified: Top, Bottom, Left, Right, Fill`.

| Region (control) | Dock | Size | Padding (gap) | Contains | Owned by |
|---|---|---|---|---|---|
| Toolbar — `toolbarPanel` | Top | height **56** | 8, 8, 8, 4 | `toolbarCard`: app title (250 × 30 at 12,7), five buttons laid out left to right at x = 272 / 376 / 500 / 620 / 708 (30 px high), `lblProgress` anchored Top\|Left\|Right | layout container (Dock + Location/Anchor inside the card) |
| Status bar — `statusPanel` | Bottom | height **28** | 8, 0, 8, 4 | `statusCard` with three docked labels: `lblStatus` Left 200, `lblTheme` Right 300, `lblBrowserWidth` Fill | layout container (Dock) |
| Navigation rail — `navigationPanel` | Left | width **220** (lab text: 240) | 8, 4, 0, 4 | `navigationCard`: `lblNavTitle` + five 36-px buttons at y = 40 / 84 / 128 / 172 / 216, each `Anchor = Top\|Left\|Right` (186 wide) | layout container (Dock + Anchor) |
| Details panel — `detailsPanel` | Right | width **340** (lab text: 360) | 0, 4, 8, 4 | `detailsCard` (332 × 588): the editor laid out with `Location` + `Anchor` — full-width `txtTitle`, two 146-px columns for Priority/Status and Owner/Due (right column anchored Top\|Right), `txtNotes` anchored on all four sides, `btnSave` + `lblDetailsHint` anchored Bottom | layout container (Dock + Anchor; Module 5 replaces the inner layout with a `TableLayoutPanel`) |
| Workspace — `workspacePanel` | Fill | `MinimumSize` **320 × 240** | 8, 4, 8, 4 | children in dock order: `metricsPanel` Top 84 (four `slot*` panels Left 192 each, `Padding` right 8, card Fill, 4-px strip Top) → `bannerPanel` Top 38, `Visible = false` → `tracePanel` Bottom 176 (`Padding` top 8) → `gridCard` Fill (`lblWorkspaceTitle` Top 26 + `gridTickets` Fill) | layout container (Dock at every level) |

Design size of the page: 1348 × 680 (`MainPage.Size`); at that size the workspace is 788 × 596 wide
before its own padding. The `Size` values on docked panels only matter in the docked dimension (height
for Top/Bottom, width for Left/Right); the other dimension is computed by the engine.

## 4 · Breakpoints

The lesson's breakpoints, mapped to the client profiles Module 6 will name in `ClientProfiles.json`
(the framework's built-in file already defines `Phone`, `Phone (Landscape)`, `Tablet`, `Tablet
(Landscape)`, `Small Desktop` with `maxWidth 1024`; anything else is the `Default` profile). In Module 1
**nothing changes across these widths**: docking answers every desktop width, and the profile mechanism
is not wired yet.

| Breakpoint | Width × height | Profile (target) | What the console does there (target) | Module 1 behaviour | Owned by |
|---|---|---|---|---|---|
| Desktop | 1440 × 900 | `Default` (treated as Desktop) | rail + details visible, four metric cards in a row | full shell, this is the screenshot width | layout container (Dock) |
| Small desktop | 1024 × 768 | `Small Desktop` (`device Desktop`, `maxWidth 1024`) | no horizontal overflow; details narrower or collapsible | full shell; `workspacePanel` = 1024 − 220 − 340 = 464 px (448 inside its own padding), above its 320 minimum | layout container today → responsive property (`detailsPanel.Visible/Size` per profile) |
| Tablet portrait | 768 × 1024 | `Tablet` | details stacked below the grid or moved to a tab | `workspacePanel` would be 768 − 560 = 208 px < `MinimumSize` 320 → the page scrolls horizontally | responsive property + `ResponsiveProfileChanged` (Module 6) |
| Tablet landscape | 1024 × 768 | `Tablet (Landscape)` | command bar fits, touch spacing | same as small desktop | responsive property |
| Phone portrait | 390 × 844 | `Phone` (`device Mobile`, `landscape false`) | rail hidden behind a menu button, grid and details stacked, icon-only buttons | desktop shell only: 220 + 340 + 320 = 880 px > 390 → horizontal overflow, documented as desktop-only in Module 1 | responsive property (`Visible`, `Display = Icon`) + `ResponsiveProfileChanged` |
| Phone landscape | 844 × 390 | `Phone (Landscape)` | key controls still reachable, status bar may hide | same overflow; toolbar and status still docked | responsive property |

The status-bar label `Browser W × H px · Device` (from `Application.Browser.Size` / `.Device`) is the
number to compare against this table in every later module; the trace also logs the page and workspace
size on every `Application.BrowserSizeChanged` and `Page.Resize`.

## 5 · What the shell does and does not set (acceptance-criterion audit)

The lab's criterion: *no control in the shell sets `BackColor`, `ForeColor`, `Font` or `CssStyle`, and no
Resize handler sets `Bounds`.* Audited with `grep` over `MainPage.cs` and `MainPage.Designer.cs`
(2026-09-10):

| Property | Occurrences | Where | Verdict |
|---|---|---|---|
| `CssStyle` | **0** | — | met: nothing in the shell uses inline styles |
| `CssClass` | **0** | — | met: no custom CSS in Module 1 (Module 3 adds `Styles/AdaptiveOps.css`) |
| `AppearanceKey` | **0** | — | every control uses its default Bootstrap-4 appearance |
| `Bounds` | **0** assignments | the word appears twice in `MainPage.cs`, both in comments ("It never sets Bounds", "no Bounds involved") | met: `MainPage_Resize` only calls `ReportWidth("Page.Resize")` |
| `BackColor` | **16** (Designer) | `MainPage` page background #EEF2F7; the 9 white cards (`toolbarCard`, `navigationCard`, `cardOpen/Overdue/Mine/Closed`, `gridCard`, `traceCard`, `detailsCard`, `statusCard`); the 4 metric strips; `lblBanner` | **not met literally** — but none is on a region panel or a standard interactive control (see below) |
| `ForeColor` | **11** (Designer) + **1** (`SetStatus` in `MainPage.cs`) | 9 muted captions (#677085), `lblBanner`, `lblStatus` (initial + the three-way switch) | not met literally — labels only |
| `Font` | **17** (Designer) | 15 labels scaled from the theme's `default` family (8–22 pt, bold), `listTrace` and `lblBrowserWidth` in `monospace` 9 | not met literally — labels and the trace list only |

What **is** clean, and why it is the part that matters:

- The **five region panels** (`toolbarPanel`, `statusPanel`, `navigationPanel`, `detailsPanel`,
  `workspacePanel`) set only `Dock`, `Size`, `Padding`, `MinimumSize` and `Name` — no colour, font or
  style at all; they are pure layout containers.
- **Every standard interactive control** — the 11 `Button`s, the `TextBox`es, `ComboBox`es,
  `DateTimePicker`, `DataGridView` and its columns — carries no `BackColor`, `ForeColor` or `Font`. Their
  look is 100 % Bootstrap-4, which is what the lesson's "theme owns the identity of standard controls"
  rule protects. Swapping the theme name in `Default.json` restyles all of them.
- The `BackColor` / `ForeColor` / `Font` lines that do exist are **card chrome and typography** on
  `Panel`s and `Label`s, and their literals are exactly the Module 2 token values (§2). They are the
  Module 1 stand-in for tokens that do not exist yet; `SetStatus()` carries the comment that Module 2
  moves the three status colours into `success` / `warning` / `danger`. The audit therefore records them
  as *debt owned by the theme*, not as design decisions of the shell.

## Evidence

What the running app shows for this deliverable (all text is produced by `MainPage.cs`; sizes depend on
the browser window):

- **Regions and dock order** — first trace line on load: `• server shell built: toolbar Dock=Top 1348×56 ·
  navigation Dock=Left 220×596 · details Dock=Right 340×596 · status Dock=Bottom 1348×28 · workspace
  Dock=Fill 788×596 min 320×240` (`DescribeShell()`), followed by `• server dock order verified: Top,
  Bottom, Left, Right, Fill (child order in MainPage.Designer.cs)` (`VerifyShell()`). Reordering the five
  `Controls.Add` lines makes the second line read `• server DOCK ORDER WRONG …`.
- **Base theme** — `• server base theme: Bootstrap-4 (selected in Default.json; no custom theme, no CSS
  in Module 1)` and the status bar's right label `theme: Bootstrap-4` (`Application.Theme.Name`).
- **Breakpoints / width label** — the middle status label `Browser 1440 × 900 px · Desktop`, updated on
  every resize; two trace lines per resize, `← client resize (Application.BrowserSizeChanged): browser
  1024×768 · page 1024×768 · workspace 448×…` and the same for `(Page.Resize)`. At 768 and 390 px the
  browser shows a horizontal scrollbar — the documented Module 1 limit.
- **Tokens in use** — the four strips on the metric cards are brand blue / red / orange / green, the
  captions are grey, the status dot changes colour between `● ready` (green), `● loading` (orange) and
  `● validation error` (red) when Simulate load / Invalid ticket are clicked.
- **Screenshot** — taken by the learner at 1440 × 900 as `docs/screenshots/desktop-1440.png` (not
  committed here).
