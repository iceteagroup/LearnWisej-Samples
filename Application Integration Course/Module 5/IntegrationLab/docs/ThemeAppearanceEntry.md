# Deliverable 3 — Theme / appearance entry

`Themes/simplegauge.mixin.theme`

## The mixin

```json
{
  "name": "simplegauge",
  "colors": {
    "simplegauge-accent": "primary",
    "simplegauge-track":  "windowFrame"
  },
  "appearances": {
    "simplegauge": {
      "states": {
        "default": {
          "styles":     { "backgroundColor": "window", "width": 1, "style": "solid", "color": "windowFrame", "radius": 10 },
          "properties": { "textColor": "windowText", "padding": 8 }
        },
        "alarm":    { "styles": { "width": 2, "color": "#E0563B" } },
        "disabled": { "properties": { "opacity": 0.5 } }
      }
    }
  }
}
```

A **mixin** is a partial theme merged into whichever theme is active. Files named `*.mixin.theme`
in the application's `/Themes` folder are picked up automatically (`Application.LoadTheme(name)`
applies "the default theme mixins" when no explicit list is given). The csproj also copies the folder
to the output directory so a published build carries it.

## How the entry connects to the control

| Server | Client | Theme |
|---|---|---|
| `AppearanceKey = "simplegauge"` → `config.appearance = "simplegauge"` | `appearance: { init: "simplegauge", refine: true }` | `"appearances": { "simplegauge": … }` |
| `AddState("alarm")` when `Value >= Threshold` | qooxdoo state `alarm` on the widget | `"states": { "alarm": … }` |

`styles` become the widget's decorator (background, border, radius); `properties` are set on the
widget (`textColor`, `padding`). The client class reads `getTextColor()` for the needle and readout,
and `getPaddingTop()`… to place the vendor's element inside the padding.

## `@accent` and `@text-primary`

The lesson's theme excerpt uses `"value": "@accent"` and `"needle": "@text-primary"`. Wisej.NET
themes reference colours by **name** (no `@`), and the two colour roles map to real theme colours
that exist in every built-in theme:

| Course name | Here | Bootstrap-4 | Material-3 | FluentDark-5 |
|---|---|---|---|---|
| `@accent` (value arc) | `simplegauge-accent` → `primary` | `#007AFF` | `#00897B` | `#9d3fff` |
| `@text-primary` (needle, readout) | appearance `textColor` → `windowText` | `#212629` | `#3F3F3F` | `#F3F2F1` |
| track | `simplegauge-track` → `windowFrame` | `#CCCCCC` | `#E1E1E1` | `#3B3A39` |

`simplegauge-accent` and `simplegauge-track` are defined as aliases of theme colours so a product
theme can override just those two names without touching the control. The client class resolves one
alias level (`resolve("simplegauge-accent") → "primary" → "#007AFF"`) and falls back to `primary` /
`windowFrame` / a literal if the mixin is missing, so the gauge always draws.

## Theme switching

**Switch theme** calls `Application.LoadTheme("Material-3")`, then `"FluentDark-5"`, then back to
`"Bootstrap-4"`. Nothing in the control changes: the `simplegauge` appearance is resolved against
the new theme, the widget's `textColor` changes (the class listens to `changeTextColor` and to the
theme manager's `changeTheme`), the accent is re-read, and the vendor is repainted through its
`colors` option. Under FluentDark the tiles turn dark with light needles and a purple accent — the
same control, restyled, which is what "native-feeling" means in enterprise adoption.

## Why theme integration matters for enterprise adoption

A control that ignores the theme looks pasted in: it keeps its own white card in a dark product,
its own blue in a green brand, its own radius next to square buttons. Teams avoid such controls or
fork them per product. An appearance key makes the control **belong to the design system**: the
theme owner restyles it from one JSON entry, designers see the same styling in the Designer, dark
and light variants come for free, and the control can be reused across screens and products without
code changes.

## Evidence (running app)

- Bootstrap-4 (default): white tiles, 1 px `#CCCCCC` border, blue accent arc, dark needle.
- After **Switch theme → Material-3**: teal accent arc, lighter border.
- After **Switch theme → FluentDark-5**: dark tile background, light needle/readout, purple accent.
- A tile in alarm shows a 2 px red border (`alarm` state) until the reading falls below the threshold.
