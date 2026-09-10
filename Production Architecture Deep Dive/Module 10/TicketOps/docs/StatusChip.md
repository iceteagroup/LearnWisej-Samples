# Deliverable — the `StatusChip` UserControl

*Module 10 deliverable · TicketOps Console · `Controls/StatusChip.cs` + `.Designer.cs`, `Themes/TicketOps.mixin.theme`*

One control, every screen. The dashboard's four KPI cards, the detail strip under the grid and the
`WorkOrderDetail` dialog all show a work-order status with the same `StatusChip`. The chip is **set,
not styled**: a screen assigns `Status` and `Text`; the theme decides what that looks like.

## What the chip owns

| Member | What it does | Where the visual rule lives |
|---|---|---|
| `Status` (`WorkOrderStatus`) | swaps the theme **state** on the widget: `open`, `inprogress`, `blocked`, `done` (`AddState` / `RemoveState`) | `Themes/TicketOps.mixin.theme` → `appearances.chip.states.*` |
| `Text` (override) | the caption of the inner label — always a resource string (`ILocalizationService.StatusText`) | `Resources/Strings.resx` / `Strings.de.resx` (`Status.*`) |
| `Show(status, text)` | both in one call, the shape every screen uses | — |
| `AppearanceKey = "chip"` | joins the theme system: background, radius and text colour come from the appearance | the mixin |
| size 132 × 26, label docked, bold 9.5 pt `default` font | padding & radius fixed once | `StatusChip.Designer.cs` + `radius: 13` in the mixin |

There is **no** `BackColor`, `ForeColor` or hex value in `StatusChip.cs`. The file would pass the
lesson's grep for colour literals.

## The theme entry

```json
"colors": {
  "chip-open": "primary",   "chip-inprogress": "#B9770E",
  "chip-blocked": "danger", "chip-done": "success"
},
"appearances": {
  "chip": {
    "states": {
      "default":    { "styles": { "backgroundColor": "hotTrack", "radius": 13, "width": 0 }, "properties": { "textColor": "windowText" } },
      "open":       { "styles": { "backgroundColor": "rgba(0,122,255,0.14)" },  "properties": { "textColor": "chip-open" } },
      "inprogress": { "styles": { "backgroundColor": "rgba(185,119,14,0.16)" }, "properties": { "textColor": "chip-inprogress" } },
      "blocked":    { "styles": { "backgroundColor": "rgba(220,52,68,0.14)" },  "properties": { "textColor": "chip-blocked" } },
      "done":       { "styles": { "backgroundColor": "rgba(40,167,69,0.16)" },  "properties": { "textColor": "chip-done" } }
    }
  }
}
```

- The four `chip-*` names are the chip's **palette**: three of them are aliases of colours that exist in
  every Bootstrap theme (`primary`, `danger`, `success` — `#007AFF`, `#DC3444`, `#28A745`/`#188754`),
  so on **BootstrapDark-4** the done chip turns the darker green the theme owner chose. `chip-inprogress`
  is a literal because neither Bootstrap theme ships an amber token; a product theme overrides just
  that one name.
- The backgrounds are translucent tints, so they sit on a white card and on a `#212429` card alike.
- The label inside the chip has no colour of its own: qooxdoo's `textColor` is inheritable and the
  Bootstrap `textlabel` appearance does not set one, so the chip's `textColor` flows into it.
- A mixin (`*.mixin.theme` in `/Themes`) is merged into **whichever theme is active**, which is why the
  chip needs no per-theme code: switch theme, and the same four states resolve against the new palette.

## Where it is reused

| Screen | Use |
|---|---|
| `OperationsDashboard` — KPI cards | four chips, one per status, `chipOpen … chipDone` |
| `OperationsDashboard` — detail strip | `chipSelected` shows the selected order's status; re-colours as **Walk statuses** advances it |
| `WorkOrderDetail` (dialog) | `chipStatus` in the header — same control, same resources, same theme |

The applied-concepts guide shows the chip through `CssClass = "chip chip-" + status` with a stylesheet.
Wisej.NET's native equivalent — used here — is an **appearance key + theme states**: it needs no CSS
file, is visible in the Designer, and follows a theme switch automatically. `CssClass`/`CssStyle` stay
what the lesson says they are: the targeted exception, not the chip's home.

## Evidence (running app)

- On load: the four KPI chips read Open / In progress / Blocked / Done in blue / amber / red / green
  tints; the detail chip shows the first order's status.
- **Theme → Dark · BootstrapDark-4**: the cards and grid turn dark; the chips keep their tints, the
  done chip's text switches to the dark theme's `success` (#188754). The trace shows
  `[UI] … 5 StatusChips … re-skinned by the theme — no chip code ran`.
- **Culture → Deutsch**: the captions become Offen / In Bearbeitung / Blockiert / Erledigt; "In Bearbeitung"
  (the widest translation) fits the 132 px chip in both themes.
- **▶ Walk statuses**: the detail chip and the KPI counts change every 800 ms; only `Status`/`Text`
  are set by the handler.
