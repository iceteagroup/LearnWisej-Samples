# VisualInventory.md · Adaptive Operations Console

The inventory the theme, layout and profile work of Modules 2–7 is planned from. Every row has an
**Owned by** column naming one of the layers: **theme**, **control property**, **CssClass**,
**CssStyle** or **layout container**. Base theme: **Bootstrap-4** (reason in
[`VisualLayers.md`](VisualLayers.md)).

## 1 · Controls that need a custom appearance

| Control (name in the shell) | Module 1 look | What the theme must own | Owned by (target) |
|---|---|---|---|
| Command buttons: `btnRefresh`, `btnSave` | Bootstrap-4 `button` | an `action-button` appearance (inherits `button`) in `brandPrimary` with its own hovered / pressed / focused / disabled states, selected with `AppearanceKey` | theme + control property (`AppearanceKey`) |
| Navigation buttons: `btnNavDashboard … btnNavHelp` | Bootstrap-4 `button` | a `nav-button` appearance with a selected state | theme |
| Metric cards: `cardOpen`, `cardOverdue`, `cardMine`, `cardClosed` (+ 4 px strips) | `Panel` with `BackColor = White`, `BorderStyle = Solid` | card surface, border, radius, shadow → `metric-card` class (Module 3); strip colours → tokens | CssClass + theme tokens |
| Region cards: `toolbarCard`, `navigationCard`, `gridCard`, `detailsCard`, `statusCard` | `Panel` with `BackColor = White` | `panel` appearance with the `surface` token | theme |
| Tabs (none yet) | — | `tabview` with a brand-coloured selected page | theme |
| Grid header: `gridTickets` | Bootstrap-4 `table-header-cell` | header background `surfaceAlt`, `textMuted`, selected / hovered rows | theme |
| Invalid editor: `txtTitle`, `txtOwner`, `cboPriority`, `cboStatus`, `dtpDue`, `txtNotes` | Bootstrap-4 `textbox` / `combobox` | `invalid` state with a `danger` border and `focusFrame` | theme (state) |
| Tooltips | Bootstrap-4 `tooltip/atom` | `surface` / `textMain`, default font, radius from settings | theme |

## 2 · Colour and font tokens

| Token | Role | Module 1 value | Owned by |
|---|---|---|---|
| `brandPrimary` | primary commands, selected navigation, "Open" strip | #2454A6 on `stripOpen` | theme |
| `brandAccent` | highlights, badges | not used yet (#F59E0B planned) | theme |
| `surface` | cards, editors, toolbar | `Color.White` on the region and metric cards | theme |
| `surfaceAlt` | page background, grid header | #EEF2F7 on `MainPage.BackColor` | theme |
| `textMain` | body text, metric values | Bootstrap-4 default | theme |
| `textMuted` | captions (`lblNavTitle`, the four metric titles, `lblDetailsSubtitle`) | #677085 | theme |
| `danger` | invalid editor border, "Overdue" strip | #B42318 on `stripOverdue` | theme |
| `warning` | "Assigned to me" strip | #B54708 on `stripMine` | theme |
| `success` | "Closed this week" strip | #027A48 on `stripClosed` | theme |
| `focusFrame` | keyboard focus ring | Bootstrap-4 focus decorator | theme |
| `default` font | body text, editors, grid, buttons | Bootstrap-4 `default` | theme |
| `heading` font | app title, card headings, metric values | control `Font` properties today | theme |
| `mono` font | `widthLabel` | `new Font("monospace", 9F)` | theme |

## 3 · Layout regions

The five regions are `Panel`s docked on `MainPage`; each holds one white card docked Fill, and the
region's `Padding` is the gap between neighbours (Dock ignores `Margin`). The `Controls.Add` block at the
end of `InitializeComponent()` adds workspace, details, navigation, status, toolbar; the control added
last is docked first.

| Region | Dock | Size | Contains | Owned by |
|---|---|---|---|---|
| `toolbarPanel` | Top | height 56 | app title, Refresh | layout container |
| `statusPanel` | Bottom | height 28 | `lblStatus` (Left), `widthLabel` (Fill) | layout container |
| `navigationPanel` | Left | width 220 (lab: 240) | five navigation buttons, `Anchor = Top\|Left\|Right` | layout container |
| `detailsPanel` | Right | width 340 (lab: 360) | the details editor laid out with `Location` + `Anchor` | layout container |
| `workspacePanel` | Fill | `MinimumSize` 320 × 240 | `metricsPanel` (Top, four cards) and `gridCard` (Fill, `gridTickets`) | layout container |

## 4 · Breakpoints

In Module 1 nothing changes across these widths: docking answers every desktop width and profiles are not
wired yet.

| Breakpoint | Width × height | Profile (target) | Target behaviour | Owned by |
|---|---|---|---|---|
| Desktop | 1440 × 900 | Desktop | rail and details visible, four cards in a row | layout container |
| Small desktop | 1024 × 768 | `Small Desktop` (maxWidth 1024) | no horizontal overflow, narrower details | responsive property |
| Tablet portrait | 768 × 1024 | `Tablet` | details under the grid | responsive property + `ResponsiveProfileChanged` |
| Tablet landscape | 1024 × 768 | `Tablet (Landscape)` | command bar fits, touch spacing | responsive property |
| Phone portrait | 390 × 844 | `Phone` | rail behind a menu button, details as a modal, icon-only buttons | responsive property + `ResponsiveProfileChanged` |
| Phone landscape | 844 × 390 | `Phone (Landscape)` | key controls reachable | responsive property |

`widthLabel` in the status bar is the number to compare against this table in every later module.

## 5 · What the shell does and does not set

The lab asks that the shell sets no `BackColor`, `ForeColor`, `Font` or `CssStyle` and that no Resize code
sets `Bounds`.

| Property | Occurrences | Verdict |
|---|---|---|
| `CssStyle`, `CssClass`, `AppearanceKey` | 0 | met |
| `Bounds` | 0 | met: `MainPage_Resize` only calls `ReportWidth()` |
| `BackColor` | 14 | page background, the white cards and the four strips; none on a region panel or an interactive control |
| `ForeColor` | 6 | muted caption labels only |
| `Font` | 13 | headings, metric values and `widthLabel` |

The five region panels set only `Dock`, `Size`, `Padding`, `MinimumSize` and `Name`. Every standard
interactive control (buttons, editors, grid) carries no colour or font, so swapping the theme in
`Default.json` restyles all of them. The remaining literals are the Module 2 token values; Module 2 moves
them into the theme.
