# Base theme choice and the layer-ownership note

Two lab deliverables: the **base theme** chosen for the Adaptive Operations Console with the reason, and
the **note explaining which layer owns each visual decision** in the Module 1 shell. The desktop
screenshot this note accompanies is taken by the learner at 1440 × 900.

## Base theme: Bootstrap-4

Selected for the running application, not only in the designer:

- `Default.json` → `"theme": "Bootstrap-4"` (the Kestrel / `dotnet run` target).
- `Web.config` → `<add key="Wisej.DefaultTheme" value="Bootstrap-4"/>` (the `net10.0-windows` / IIS target).

Why Bootstrap-4 (recorded before any colour or font was touched):

1. **It is the base Module 2 inherits from.** Its appearances are conventional (`button` with
   default / hovered / focused / pressed / disabled / checked, `textbox` with `invalid`,
   `table-header-cell`, `tabview`, `tooltip/atom`, `panel` with `captionbar`), so Module 2 changes
   colours, not structure.
2. **Its metrics suit a dense business console.** 30–36 px buttons, 28 px editors, compact rows. Material
   adds elevation and larger touch targets; Fluent's accent look is an identity Module 2 should decide;
   the Blue/Classic/Vista families carry gradients the course would have to remove.
3. **It is neutral about colour.** Grey on white with one accent, so the "before" screenshot shows a
   shell whose identity is still the base theme's.
4. **Web-conventional**, so Module 3's scoped stylesheet stays limited to the app-specific parts.

A light theme was preferred over BootstrapDark-4; a dark variant is Module 3's session-only theme.

## Which layer owns each visual decision

The layers as the lesson names them: **theme** (standard-control identity, tokens, states), **control
property** (`Dock`, `Size`, `MinimumSize`, `Padding`, `Anchor`, `Visible`, `AppearanceKey`, and, when
misused, `BackColor` / `ForeColor` / `Font`), **CssClass**, **CssStyle**, **layout container**. The rule
the shell follows: theme first for standard controls, containers for geometry, no CSS in Module 1.

| Region | Decision | Owned by |
|---|---|---|
| Page | the five regions and their order (Top, Bottom, Left, Right, Fill) | layout container: Dock, ordered by the `Controls.Add` sequence (added last = docked first) |
| Page | light grey ground behind the cards | control property today (`this.BackColor`) → theme token `surfaceAlt` in Module 2 |
| Page | look of every `Button`, `TextBox`, `ComboBox`, `DateTimePicker`, `DataGridView` | theme (Bootstrap-4 appearances and states); nothing set on them |
| Toolbar | height 56 at the top, 8 px gap to the page edge | layout container (Dock=Top) + `Padding` on the region (Dock ignores `Margin`) |
| Toolbar | white card with a border | control property today (`BackColor`, `BorderStyle`) → theme `panel` / `surface` |
| Toolbar | Refresh button colours and states | theme (`button`); Module 2 gives the primary command `AppearanceKey = "action-button"` |
| Navigation | width 220 on the left, between toolbar and status | layout container (Dock=Left) |
| Navigation | buttons stretch with the rail | layout container (`Anchor = Top\|Left\|Right`) |
| Navigation | visible on a phone or not | responsive property + `ResponsiveProfileChanged`, not decided in Module 1 |
| Workspace | fills what is left, never below 320 × 240 | layout container (Dock=Fill) + `MinimumSize` |
| Workspace | four metric cards in a row above the grid | layout container (four slots docked Left; Module 5 replaces them with a wrapping flow) |
| Workspace | coloured 4 px strip on each card | layout container for the geometry; control property today (`BackColor`) → theme tokens |
| Workspace | card surface, border, radius | control property today → `CssClass` `metric-card` in Module 3 |
| Workspace | grid header, rows, selection | theme (`table`, `table-header-cell`, `table-row`) |
| Details | width 340 on the right | layout container (Dock=Right) |
| Details | two-column form, notes grow, Save pinned to the bottom | layout container (`Location` + `Anchor`); Module 5 uses a `TableLayoutPanel` |
| Details | editor borders, focus ring, invalid look | theme (`textbox`, `combobox` appearances and states) |
| Status | height 28 at the bottom; status text left, width label filling the rest | layout container (Dock) |
| Status | the width text | control property (`Text`) written by `ReportWidth()`, behaviour, not styling |

Not used in Module 1: **CssClass** and **CssStyle** (nothing is app-specific enough yet; the metric card is
Module 3's first candidate), **AppearanceKey** (the custom theme comes first), and **Resize code that sets
`Bounds`** (`MainPage_Resize` only reports the width).

The `BackColor`, `ForeColor` and `Font` values that do exist are on `Panel`s and `Label`s (card surfaces,
metric strips, captions, headings) and are the Module 2 token values written as literals. No standard
interactive control carries any of them. Module 2 deletes them by giving the theme `surface`,
`surfaceAlt`, `textMuted`, `heading` and `mono` and a `panel` appearance.
