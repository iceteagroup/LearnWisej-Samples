# Deliverable · ThemeNotes — the AdaptiveOps theme file, its tokens, inheritance and state order

**File:** `AdaptiveOps/Themes/AdaptiveOps.theme` (194 KB, JSON). Selected at startup by `"theme": "AdaptiveOps"` in
`Default.json` and by `Wisej.DefaultTheme` in `Web.config`; reloaded at runtime by `Application.LoadTheme("AdaptiveOps")`
(name only — no path, no extension). The csproj ships it as `Content` copied to the output folder of both target
frameworks, because the framework loads a named theme "from a file in the application `/Themes` directory".

The lab text calls the file `AdaptiveOps.Theme.json`. The framework resolves a theme *name* to `/Themes/<name>.theme`,
which is also what the Theme Builder saves, so the file here is `AdaptiveOps.theme`; the content is the same JSON.

## 1. How the file was created — a full copy of the base theme, edited

The Theme Builder never starts from an empty file: *New from theme…* copies the whole base theme (every appearance,
component, state, image and font) and you edit the copy. This sample does exactly that by hand: the file **is the
complete Bootstrap-4 definition** (95 appearances, 95 images, 88 colours) with the AdaptiveOps changes applied on top.

Why not a one-line `"inherit": "Bootstrap-4"` at the top level? The framework XML documentation describes
inheritance only *inside* appearances (`"inherit": "button"` on an appearance, and "searches the parent and the
inherited appearances" for child components); a whole-theme inherit is not documented, and a sibling probe of
`ClientTheme` confirmed that a top-level `inherit` is not merged. The Theme Builder's own workflow — copy, then edit —
is therefore also the safe file format: **the saved copy is the whole theme; the application does not depend on the
base at runtime.** The price is a 194 KB file; the benefit is that every edit is a readable diff against the base.

## 2. Structure of the file

```json
{
  "name": "AdaptiveOps",
  "settings":    { "borderRadius": 6, "focusBlurRadius": 0, "focusBorderSize": 3 },
  "fonts":       { "default": {…}, "heading": {…}, "mono": {…}, "windowTitle": {…}, "defaultBold": {…}, "menu": {…} },
  "images":      { "baseUrl": "", "blank": "data:image/png;base64,…", "icon-info": "data:image/svg+xml;base64,…", … },
  "colors":      { "brandPrimary": "#2454A6", … 88 named colours … },
  "appearances": { "button": {…}, "action-button": {…}, "panel": {…}, "metric-card": {…}, … 95 appearances … },
  "stylesheet":  { "rules": [] }
}
```

Inside an appearance:

```
appearance
├─ "inherit": "<other appearance>"        optional: copy that appearance, then override what is listed here
├─ "states": {                             matched against the widget's current states, IN LIST ORDER
│    "default":  { "styles": {…}, "properties": {…} },
│    "hovered":  { … }, "focused": { … }, "pressed": { … }, "disabled": { … }, "invalid": { … }, …
│  }
└─ "components": {                         the inner widgets, each with its own states (and inherit)
     "captionbar": { "states": {…} }, "icon": { "inherit": "icon-light" }, …
   }
```

- **styles** are CSS-like values the theme turns into dynamic CSS classes — `backgroundColor`, `width`/`style`/`color`
  (the border), `radius`, `shadow*`, `transition`, `transform`. The theme owns them; the application cannot change
  them at runtime.
- **properties** are widget properties — `textColor`, `font`, `height`, `padding`, `opacity`, `cursor`, `icon`,
  `center`, `backgroundColor` on widgets that paint their own background (`textbox`, `page`, `table`). Some become
  inline style, some behaviour. An application property (`BackColor`, `ForeColor`, `Font`) overrides them — which is
  why this project sets none.
- A colour value is a **literal** (`#1F4A92`, `white`, `rgba(…)`) or the **name of a token** from `colors`.
  `"$borderRadius"` reads a `settings` value. Fonts are referenced by name in `properties.font`.
- Fonts are objects `{ "size", "family": [...], "bold" }` — the course pack's `["Segoe UI", 14]` shorthand is not the
  framework format.

## 3. The tokens — defined once, referenced by name

### Colours

| Token | Value | Why it exists | Used by (appearance / component [state]) |
|---|---|---|---|
| `brandPrimary` | `#2454A6` | the brand; the one line marketing changes | `action-button` [default], `panel/captionbar`, `metric-strip` [default], `tabview/page/button` [checked] text, `button` [checked] |
| `brandAccent` | `#F59E0B` | the accent; kept for highlights | `table-header-cell` [sorted] text (sort a grid column); Module 3 reuses it in the CssClass layer |
| `surface` | `#FFFFFF` | card / editor background | `metric-card`, `tabview/page`, `textbox`, table rows, `tooltip/atom` text, `trace-list` |
| `surfaceAlt` | `#F6F7FB` | the page and every secondary surface | `page`, `button` face, tab bar buttons, `table-header-cell`, odd table rows |
| `textMain` | `#222222` | body text | `root`, `page`, `button` text, `heading-label`, `card-title`, `tooltip/atom` background |
| `textMuted` | `#677085` | secondary text | `muted-label`, `overline-label`, `mono-label`, tab button text, `table-header-cell` text, `status-label` [default] |
| `danger` | `#B42318` | errors | `textbox` [invalid] border, `banner-label`, `metric-strip` [danger], `status-label` [error], `tooltip-error/atom` |
| `warning` | `#B54708` | warnings | `metric-strip` [warning], `status-label` [warn] |
| `success` | `#027A48` | success | `metric-strip` [success], `status-label` [ok] |
| `focusFrame` | `#2454A6` | the focus border | `textbox` / `combobox` / `list` [focused] border, tab button [focused] inner border |
| `brandPrimaryHover` | `#1F4A92` | derived shade, defined once | `action-button` [hovered] |
| `brandPrimaryPressed` | `#183B75` | derived shade, defined once | `action-button` [pressed] |
| `surfaceHover` | `#E6EAF2` | hover shade for neutral surfaces | `button` [hovered], `table-header-cell` [hovered], list items [hovered], selected table rows |
| `focusShadow` | `rgba(36,84,166,0.35)` | the 3 px focus ring around buttons | `button` [focused] (`shadowColor`), inherited by `action-button` |

**About `focusFrame` "following" `brandPrimary`.** The lesson suggests letting a named colour point at another named
colour. In this framework build that does not resolve: `Application.Theme.GetColor("focusFrame")` returns
`Color.Empty` when the value is the *name* `brandPrimary` (checked in-process with `new ClientTheme(name, json)` against
this very file), and a colour that does not resolve paints nothing. So `focusFrame` carries the **same literal value**
as `brandPrimary`, and the rebrand rule becomes "brandPrimary plus the tokens that intentionally equal it" — see §7.

The lesson's `action-button` example writes the hover and pressed shades as literals (`#1F4A92`, `#183B75`). That is
allowed (no token existed for them), but the derived shades here are tokens so a rebrand touches `colors` only.

### Fonts

| Token | Definition | Used by |
|---|---|---|
| `default` | `{ "size": 13, "family": ["Segoe UI","Roboto","Helvetica Neue","Arial"], "bold": false }` | `root` — every widget without its own font |
| `heading` | `{ "size": 18, "family": [same], "bold": true }` | `heading-label`: application title, the four metric values |
| `mono` | `{ "size": 12, "family": ["Consolas","Menlo","monospace"], "bold": false }` | `trace-list` (the live trace), `mono-label` (browser size) |
| `defaultBold` | kept from Bootstrap-4 (13, bold) | `panel/captionbar`, `card-title`, `overline-label`, `status-label`, `banner-label` |

Web fonts: the families above are system stacks, so nothing has to be deployed. A real web font would be referenced
from `Default.html` (or a `@font-face` in the theme `stylesheet`) and must ship with the application; a font the
browser cannot find falls back silently.

### Base tokens re-pointed to the palette

Bootstrap-4's own tokens are still referenced by the ~85 appearances the lab did not edit (menus, calendars,
scrollbars…). So that those pick the brand up too, the base tokens that *should* follow the palette carry the same
literal values (a token → token alias would not resolve, see above):

| Base token | Now | Base token | Now |
|---|---|---|---|
| `primary`, `highlight`, `activeCaption`, `switchOn`, `menuSelected` | `#2454A6` (= brandPrimary) | `invalid` | `#B42318` (= danger) |
| `window`, `tabSelected`, `table-row-background(-even)` | `#FFFFFF` (= surface) | `buttonFace`, `tabFace`, `toolbar`, `light`, `gray-100`, `table-row-background-odd` | `#F6F7FB` (= surfaceAlt) |
| `windowText`, `controlText`, `buttonText`, `table-row` | `#222222` (= textMain) | `tabText` | `#677085` (= textMuted) |
| `buttonHighlight`, `hotTrack`, `tabHighlight`, `table-row-background-selected/-focused` | `#E6EAF2` (= surfaceHover) | `windowFrame`, `table-row-line`, `table-column-line` | `#D9DEE7` (the border grey) |

`settings.borderRadius` went from 4 to 6: one value, every `"radius": "$borderRadius"` follows.

## 4. The appearances the lab restyles — with the JSON and the reason

### `button` (base appearance, tokens only)

```json
"button": {
  "states": {
    "default":  { "properties": { "opacity": 1, "center": true, "textColor": "textMain", "height": 36 },
                  "styles": { "backgroundColor": "surfaceAlt", "radius": "$borderRadius", "transition": "background-color 250ms",
                              "width": 1, "style": "solid", "color": "windowFrame" } },
    "hovered":  { "styles": { "backgroundColor": "surfaceHover" }, "properties": { "cursor": "pointer" } },
    "focused":  { "styles": { "shadowLength": 0, "shadowSpreadRadius": "$focusBorderSize", "shadowColor": "focusShadow", "shadowBlurRadius": "$focusBlurRadius" } },
    "disabled": { "properties": { "opacity": 0.5, "cursor": "default" } },
    "checked":  { "styles": { "backgroundColor": "brandPrimary", "color": "brandPrimary" }, "properties": { "textColor": "white" } },
    "borderNone": { … }, "borderDashed": { … }, "borderDotted": { … }, "borderDouble": { … }
  },
  "components": { "arrow": { … }, "icon": { … } }
}
```

Reason: Bootstrap-4's `hovered` was a literal `#D1E0E5`; it is now `surfaceHover`. The navigation rail and the
secondary toolbar commands use this appearance untouched by code.

### `action-button` — the semantic variant (Lab task 6)

```json
"action-button": {
  "inherit": "button",
  "states": {
    "default": { "styles": { "backgroundColor": "brandPrimary", "color": "brandPrimary" }, "properties": { "textColor": "white" } },
    "hovered": { "styles": { "backgroundColor": "brandPrimaryHover", "color": "brandPrimaryHover" } },
    "pressed": { "styles": { "backgroundColor": "brandPrimaryPressed", "color": "brandPrimaryPressed", "transform": "translate(1px, 1px)" } }
  }
}
```

- `inherit: button` copies the whole button appearance (states *and* the `arrow`/`icon` components).
- Only **default, hovered, pressed** are listed. **focused** (the `focusShadow` ring) and **disabled** (opacity 0.5)
  are not mentioned, so they come from `button` unchanged — the state walk in the app proves it:
  `GetColor("action-button","shadowColor","focused")` = `focusShadow`, `GetProperty<double>("action-button","opacity","disabled")` = 0.5,
  `height` = 36.
- `textColor` sits under `properties`, where the framework keeps it (the lesson's snippet shows it under `styles`;
  Bootstrap-4 itself uses `properties.textColor`, so the file follows the framework).
- A control gets the variant with `btnSave.AppearanceKey = "action-button"` (done in `btnApplyTheme_Click` and on
  Reset). No `BackColor` anywhere.

### `panel` and its `captionbar` component

```json
"panel": {
  "states": { "default": { "styles": { "width": 1, "style": "solid", "color": "windowFrame", "radius": "$borderRadius" } }, "borderNone": {…}, … },
  "components": {
    "captionbar": { "states": { "default": { "styles": { "backgroundColor": "brandPrimary", "radius": "$borderRadius" },
                                              "properties": { "textColor": "white", "font": "defaultBold" } },
                                "horizontal": { "properties": { "height": 28 } }, "vertical": {…} } },
    "close-button": {…}, "title": {…}, "icon": {…}, "pane": { "inherit": "scrollarea" }
  }
}
```

The trace card and the details card are Panels with `ShowHeader = true`, so `panel/captionbar` is visible in the
console (Last Clicked on the caption bar in the Theme Builder reports exactly that path).

### `metric-card` and `metric-strip` — the console's own panel variants

```json
"metric-card":  { "inherit": "panel",
                  "states": { "default": { "styles": { "backgroundColor": "surface", "width": 1, "style": "solid", "color": "windowFrame", "radius": "$borderRadius" } } } },
"metric-strip": { "inherit": "panel",
                  "states": { "default": { "styles": { "backgroundColor": "brandPrimary", "width": 0, "radius": 0 } },
                              "danger":  { "styles": { "backgroundColor": "danger" } },
                              "warning": { "styles": { "backgroundColor": "warning" } },
                              "success": { "styles": { "backgroundColor": "success" } } } }
```

Every card (toolbar, navigation, four metrics, trace, details, status) is `AppearanceKey = "metric-card"`: Module 1's
`BackColor = White` is gone. The 4 px strip on each metric card is a `metric-strip` whose colour is a **custom state**
set from code with `Control.States` (`stripOverdue.States = { "danger" }`): the accent is a token, chosen by state, not
a `BackColor`.

### `tabview` — tab buttons and page

```json
"tabview": { "components": {
  "page": { "states": { "default": { "properties": { "backgroundColor": "surface" } } },
            "components": { "button": { "states": {
                "default": { "styles": { "backgroundColor": "surfaceAlt", "radius": "$borderRadius", "width": 1, "color": "transparent", … },
                             "properties": { "textColor": "textMuted", "height": 40, … } },
                "disabled": {…},
                "focused":  { "styles": { "innerStyle": "dashed", "innerColor": "focusFrame" } },
                "hovered":  { "styles": { "width": 1, "color": "windowFrame" }, "properties": { "cursor": "pointer" } },
                "checked":  { "styles": { "backgroundColor": "surface", "backgroundRepeat": "no-repeat", "color": "windowFrame" },
                              "properties": { "textColor": "brandPrimary", "showClose": true } },
                "barTop": {…}, … } } } } } }
```

The workspace is a TabControl (Tickets · Token inspector), so the tab restyle is visible: muted tabs on `surfaceAlt`,
the selected tab on `surface` with `brandPrimary` text.

### `table` and the `table-header-cell` component (the grid header)

```json
"table":             { "states": { "default": { "styles": { "width": 1, "style": "solid", "color": "windowFrame" },
                                                "properties": { "indent": 20, "rowHeight": 32, "headerCellHeight": 32, "backgroundColor": "surface", "headerBackColor": "surfaceAlt" } } } },
"table-header-cell": { "states": {
    "default": { "styles": { "width": [0,1,0,0], "color": "windowFrame", "transition": "background-color 250ms", "backgroundSize": "18x", "backgroundColor": "surfaceAlt" },
                 "properties": { "padding": 5, "sortIcon": "", "textColor": "textMuted", "cursor": "default" } },
    "rightToLeft": {…},
    "hovered": { "styles": { "backgroundColor": "surfaceHover" }, "properties": { "cursor": "pointer" } },
    "sorted":  { "properties": { "sortIcon": "icon-sorted-descending", "textColor": "brandAccent" } },
    "sorted-sortedAscending": {…}, "borderNone": {…}, … } }
```

Both grids (tickets, token inspector) share it. Bootstrap-4's hovered header was the literal-valued `buttonHighlight`.

### `textbox` — the invalid state (the editor)

```json
"textbox": { "states": {
  "default":  { "styles": { "width": 1, "style": "solid", "color": "windowFrame", "radius": "$borderRadius" },
                "properties": { "height": 30, "padding": [0,2,0,4], "backgroundColor": "surface", "opacity": 1, "textColor": "textMain" } },
  "hovered":  {…},
  "focused":  { "styles": { "color": "focusFrame", "shadowColor": "focusShadow", "shadowSpreadRadius": "$focusBorderSize", "shadowLength": 0, "shadowBlurRadius": "$focusBlurRadius" } },
  "disabled": {…}, "multiline": {…}, "borderNone": {…}, "borderDashed": {…}, "borderDotted": {…}, "borderDouble": {…}, "celleditor": {…},
  "invalid":  { "styles": { "color": "danger", "width": 1 } }
} }
```

`invalid` is the **last** state so it wins over `focused` while the user is still in the field. Code only says
`txtTitle.Invalid = true; txtTitle.InvalidMessage = "Title is required."` — the red border is the theme's; the message
shows in the `tooltip-error` appearance, whose `atom` already uses `danger`.

### `tooltip` — the `atom` component

```json
"tooltip": { "states": { "default": { "properties": { "placeMethod": "widget", "position": "top-center", "maxWidth": 280, … } }, … },
             "components": { "atom":  { "states": { "default": { "styles": { "backgroundColor": "textMain", "radius": "$borderRadius" },
                                                                 "properties": { "padding": 10, "textColor": "surface", "icon": "icon-info", "show": "both" } } },
                                        "components": { "icon": { "inherit": "icon-light" } } },
                             "arrow": {…} } }
```

Every toolbar button, the tab pages, the Title editor and Save carry `ToolTipText`, so the tooltip is one hover away.

### Label variants, the trace list, page and root

```json
"heading-label":  { "inherit": "textlabel", "states": { "default": { "properties": { "opacity": 1, "font": "heading",     "textColor": "textMain"  } } } },
"card-title":     { "inherit": "textlabel", "states": { "default": { "properties": { "opacity": 1, "font": "defaultBold", "textColor": "textMain"  } } } },
"overline-label": { "inherit": "textlabel", "states": { "default": { "properties": { "opacity": 1, "font": "defaultBold", "textColor": "textMuted" } } } },
"muted-label":    { "inherit": "textlabel", "states": { "default": { "properties": { "opacity": 1, "font": "default",     "textColor": "textMuted" } } } },
"mono-label":     { "inherit": "textlabel", "states": { "default": { "properties": { "opacity": 1, "font": "mono",        "textColor": "textMuted" } } } },
"status-label":   { "inherit": "textlabel", "states": {
                      "default": { "properties": { "opacity": 1, "font": "defaultBold", "textColor": "textMuted" } },
                      "ok":      { "properties": { "textColor": "success" } },
                      "warn":    { "properties": { "textColor": "warning" } },
                      "error":   { "properties": { "textColor": "danger" } } } },
"banner-label":   { "inherit": "textlabel", "states": { "default": { "styles": { "backgroundColor": "danger", "radius": "$borderRadius" },
                                                                    "properties": { "opacity": 1, "font": "defaultBold", "textColor": "white" } } } },
"trace-list":     { "inherit": "list", "states": { "default": { "styles": { "width": 1, "style": "solid", "color": "windowFrame", "radius": "$borderRadius" },
                                                                "properties": { "opacity": 1, "backgroundColor": "surface", "textColor": "textMain", "font": "mono", "itemHeight": 22 } } } },
"page":           { "states": { "default": { "properties": { "backgroundColor": "surfaceAlt", "textColor": "textMain" } } }, … },
"root":           { "states": { "default": { "properties": { "font": "default", "textColor": "textMain" } } } }
```

`textlabel` is the appearance key of `Wisej.Web.Label` (read from the framework), `list` the key of `ListBox`. These
variants replace every `Font` and `ForeColor` Module 1 set in the Designer: the status label's three colours are now
three **custom states** (`lblStatus.States = { "error" }`), the banner's red is a theme style, the metric values use
the `heading` font, the trace the `mono` font.

## 5. Inheritance — what "inherit" copies and what you override

- `"inherit": "X"` copies appearance X — its states **and** its components — into the new appearance; the states you
  list are then applied on top, state by state. `action-button` therefore still has `arrow` and `icon` components and
  the `checked`, `borderNone`… states of `button` without repeating them.
- Override the *whole state* you touch: a listed state replaces that state's block. `metric-card` restates the full
  `default` block (border width/style/colour, radius, background) rather than only `backgroundColor`, so the result
  does not depend on merge semantics.
- Component inheritance uses paths: `"inherit": "menubar/item"`, `"inherit": "checkbox/icon"` (both in Bootstrap-4).
- Appearance paths in the API use the same slash: `GetColor("panel/captionbar", "backgroundColor")`,
  `GetColor("tooltip/atom", …)`, `GetColor("tabview/page/button", "textColor", "checked")`.
- Decision recorded: `action-button` overrides **only** default, hovered, pressed. Focused must stay the shared ring
  (`focusShadow`) for keyboard users, and disabled must stay the shared 50 % opacity, otherwise a disabled primary
  command would look clickable. Nothing else about a primary command differs from a button.

## 6. State order — order is behaviour

Matching states are applied **top to bottom, later wins**. The widget's current states (e.g. `hovered` + `pressed`
+ `focused` while the mouse is held on a focused button) are all matched; each matching block writes its values in
list order, so the lowest matching block decides.

| Appearance | Order in the file | Why this order |
|---|---|---|
| `button` | default › hovered › focused › disabled › checked › border* | `default` first so every other state overrides it; `disabled` after `hovered` so a disabled button does not light up on hover; `checked` after `hovered` so a checked toggle keeps `brandPrimary` under the mouse |
| `action-button` | default › hovered › pressed | `pressed` below `hovered`: while the mouse is held both match and `brandPrimaryPressed` + `translate(1px,1px)` win. Drag `pressed` above `hovered` in the Theme Builder tree and the user sees `brandPrimaryHover` while pressing — same colours, different behaviour |
| `textbox` | default › hovered › focused › disabled › multiline › border* › celleditor › invalid | `invalid` last: a focused invalid editor shows the `danger` border, not `focusFrame`. Move `invalid` above `focused` and the red border disappears the moment the user clicks into the field |
| `metric-strip` | default › danger › warning › success | only one custom state is set per strip, so order does not matter here; `default` first is still the rule |
| `status-label` | default › ok › warn › error | one custom state at a time (`lblStatus.States = {…}`), `default` first |

Rule from the lesson, kept everywhere: **`default` is the first state in every appearance.**

## 7. Theme vs CSS vs layout — the decisions of this module

| Need | Mechanism | Not this |
|---|---|---|
| identity of standard controls (buttons, tabs, grid header, editors, tooltips, panels) | the theme file | `BackColor`/`ForeColor`/`Font` per control, CSS against `.qx-*` DOM |
| a semantic variant (the primary command) | `AppearanceKey = "action-button"` with `inherit: button` | a second Button subclass, a `BackColor` |
| the four metric accent colours, the three status colours | custom theme states + tokens (`metric-strip`, `status-label`) | `Color.FromArgb` in `SetStatus` (Module 1 did this and said so) |
| the invalid editor | the `invalid` state of `textbox` + `Invalid = true` | a red `BackColor` when validation fails |
| a rebrand | edit `colors` (brandPrimary + the tokens that equal it: `focusFrame`, `primary`, `highlight`…) | search-and-replace across Designer files |
| where things are and how big | Dock / Anchor / Padding in `MainPage.Designer.cs` (Module 1) | the theme (it owns look, not layout) |
| app-specific one-offs (Module 3) | `CssClass` + `Styles/AdaptiveOps.css`, `CssStyle` for one computed value | theme appearances for non-widget layout chrome |

## 8. "The control ignores my edit" — check in this order

1. An **application property** on the control overrides the theme — `BackColor`, `ForeColor`, `Font`, `CssStyle`,
   `CssClass`. (This project: none.)
2. The **AppearanceKey** — is it the appearance you edited? `Base theme ⇄` shows what a key the theme does not define
   looks like: `GetColor("action-button", …)` = `Color.Empty` on Bootstrap-4 and Save renders as a bare widget.
3. The **state order** — and is the state actually set (`Invalid = true`, `States = {…}`)?
4. Did the running app **load the theme you edited** — `Default.json` `"theme"`, `Web.config`,
   `Application.Theme.Name` in the status bar? Editing the theme shown in the Visual Studio designer changes nothing
   in the browser.

## 9. One question the grounded AI assistant must answer from the reading

*"In the AdaptiveOps theme the action-button appearance lists default, hovered and pressed. A learner reports the
Save button shows no focus ring and stays fully opaque when disabled. Is that a bug in the theme?"* — Expected answer:
no — unless the learner added `focused`/`disabled` blocks to `action-button` or set `BackColor`/`ForeColor` on the
control; with `inherit: button` and those two states unlisted, focused and disabled come from `button`
(`focusShadow` ring, opacity 0.5). Check the four items of §8 in order.

## Evidence (what the running console shows)

- **Startup:** status bar right label `theme: AdaptiveOps (Themes/AdaptiveOps.theme · Default.json)`; status label
  `● theme AdaptiveOps` in the `success` colour (custom state `ok`); trace lines
  `• server Application.Theme.Name = "AdaptiveOps" (page load)` and the two `theme file … ✓` lines. The page background
  is `surfaceAlt`, cards are white with `windowFrame` borders and 6 px radius, the two headered cards show
  `brandPrimary` caption bars, metric values are 18 px bold, the trace is monospaced.
- **Apply theme:** Save and Apply theme turn `brandPrimary` with white text; hover = `#1F4A92`, hold = `#183B75` and
  a 1 px shift; Tab to Save = the shared focus ring; the Token inspector tab lists 18 tokens with *Defined as* =
  *Resolved* for every colour (e.g. `brandPrimary · #2454A6 · #2454A6 (@brandPrimary)`).
- **Walk states:** 22 trace lines, e.g. `→ theme action-button / shadowColor [focused] = #2454A6 α89 (@focusShadow)
  ← INHERITED from button — not overridden` and `→ theme textbox / color [invalid] = #B42318 (@danger)`.
- **Invalid ticket:** the Title editor gets a `danger` border and the error tooltip; banner in `danger`; status
  `● validation error` in `danger` (state `error`). Reset clears all three.
- **Missing theme:** trace shows what `Application.LoadTheme("Missing-Theme")` did (exception or no-op — logged
  either way), `GetColor("brandPrimry") = Color.Empty`, the console keeps running; Reset reloads AdaptiveOps.
- **Base theme ⇄:** the whole console flips to Bootstrap-4 (blue `#007AFF` caption bars, grey `#D1E0E5` button hover,
  4 px radius); every control that names a custom appearance (`action-button`, `metric-card`, the label variants)
  renders bare because Bootstrap-4 does not define them — the trace says so. Click again to return.
