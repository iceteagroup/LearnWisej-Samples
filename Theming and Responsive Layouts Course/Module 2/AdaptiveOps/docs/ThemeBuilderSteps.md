# Deliverable · ThemeBuilderSteps — the Theme Builder session, as the walkthrough video shows it

This is the order of operations in the Wisej.NET Theme Builder that produces `Themes/AdaptiveOps.theme`. The sample
in this folder edited the JSON by hand (the file is a plain-text copy of the base theme plus the changes below), so
the result is the same file the Theme Builder would save; the steps are recorded here because the *order* is the
lesson: base theme → tokens → appearances → custom appearance → load it in the app.

## The main window (what the four tools are for)

| Tool | What you do with it |
|---|---|
| **Tree** (left) | the theme file itself: `colors`, `images`, `fonts`, `appearances`; inside an appearance the `states`, `components`, `styles`, `properties`. Add, rename, delete and **move** elements here — moving a state changes its order, and order is behaviour |
| **Property grid** (middle) | edits the selected element as typed fields: colour pickers for colour values, font choosers, a top · right · bottom · left group for spacing values such as `padding` |
| **Preview** (right) | real widgets rendered with the theme. With **Live update** on, every edit is visible immediately; switch it off while entering a group of related values, switch it back on to check the combined result |
| **Last Clicked** | click any widget in the preview and the Theme Builder reports its appearance path — e.g. `panel/captionbar`, `table-header-cell`, `tabview/page/button`, `tooltip/atom`. Jump to that path in the tree and edit exactly what the user sees. **Discover first, then edit** |

## Step 1 · New theme from a base theme

1. *File → New from theme…*, pick **Bootstrap-4** (the base theme recorded in Module 1's VisualInventory).
2. Name it **AdaptiveOps**. The tree now holds every appearance, component, state, image and font of the base — you
   never start from an empty file.
3. *Save as* into the project's **Themes** folder. The framework resolves the theme *name* to `/Themes/<name>.theme`,
   so the file is `Themes/AdaptiveOps.theme` (the lab text says `AdaptiveOps.Theme.json`; same JSON, and the
   `.theme` extension is what the loader looks for).
4. Commit the untouched copy first, so every later change is a readable diff against the inherited structure.

## Step 2 · Tokens first — named colours and fonts (Live update on)

In the tree, right-click **colors → Add color** and enter, one by one:

```
brandPrimary  #2454A6      brandAccent  #F59E0B     surface   #FFFFFF     surfaceAlt  #F6F7FB
textMain      #222222      textMuted    #677085     danger    #B42318     warning     #B54708
success       #027A48      focusFrame   #2454A6     (+ derived: brandPrimaryHover #1F4A92, brandPrimaryPressed #183B75,
                                                     surfaceHover #E6EAF2, focusShadow rgba(36,84,166,0.35))
```

- `danger`, `warning`, `success`, `focusFrame`, `focusShadow` already exist in Bootstrap-4: **edit their values**
  rather than adding duplicates. Watch the preview: buttons, tabs, focus rings and error borders move at once because
  the inherited appearances already reference those names.
- The lesson suggests typing `brandPrimary` as the *value* of `focusFrame`. In this framework build a token whose value
  is another token's name does not resolve (`GetColor` returns `Color.Empty`), so give `focusFrame` the same hex value
  instead — see `ThemeNotes.md` §3.
- Then re-point the base tokens the brand should drive (`primary`, `window`, `windowText`, `buttonFace`, `buttonText`,
  `tabFace`, `tabText`, `tabSelectedText`, `windowFrame` …) to the same values. Turn **Live update off** while typing
  this group, on again to see the whole preview take the palette.
- **fonts → Add font**: `heading` (Segoe UI stack, 18, bold) and `mono` (Consolas, Menlo, monospace, 12). Fonts are
  objects `{ size, family[], bold }` in the property grid. Leave `default` at 13 regular.
- **settings → borderRadius** 4 → 6.

## Step 3 · Appearances second — Last Clicked, then edit, tokens only

For each target, click it in the preview, read **Last Clicked**, jump to the path, and set values by *token name*:

| Click in the preview | Last Clicked path | Edit |
|---|---|---|
| a button | `button` › states › `default` | styles: backgroundColor `surfaceAlt`, border width 1 solid `windowFrame`, radius `$borderRadius`; properties: textColor `textMain` |
| the same button, hovered | `button` › states › `hovered` | styles: backgroundColor `surfaceHover` (replaces the literal `#D1E0E5`) |
| a panel's caption bar | `panel` › components › `captionbar` › `default` | styles: backgroundColor `brandPrimary`; properties: textColor `white`, font `defaultBold` |
| a tab button | `tabview` › components › `page` › components › `button` | `default`: backgroundColor `surfaceAlt`, textColor `textMuted`; `checked`: backgroundColor `surface`, textColor `brandPrimary` |
| a grid column header | `table-header-cell` › `default` | styles: backgroundColor `surfaceAlt`, color `windowFrame`; properties: textColor `textMuted`; `hovered`: `surfaceHover`; `sorted`: textColor `brandAccent` |
| the editor, then its invalid state | `textbox` › states › `invalid` | styles: color `danger`, width 1 (keep `invalid` as the **last** state) |
| a tooltip | `tooltip` › components › `atom` › `default` | styles: backgroundColor `textMain`; properties: textColor `surface` |

Rule enforced while editing: **no literal hex value inside an appearance where a token exists.** A lint over the
edited appearances of the saved file finds none (the only literal left in an edited appearance is Bootstrap-4's own
`table/resize-line` rgba, untouched by the lab).

## Step 4 · Custom appearances last — `action-button` by inheritance

1. Right-click **appearances → Add appearance**, name it `action-button`.
2. In the property grid set **inherit = button**. The preview widget with `appearance: action-button` immediately looks
   like a button: everything is inherited.
3. Add three states, in this order: **default**, **hovered**, **pressed**.
   - default: styles backgroundColor `brandPrimary`, color `brandPrimary`; properties textColor `white`
   - hovered: styles backgroundColor `brandPrimaryHover`
   - pressed: styles backgroundColor `brandPrimaryPressed`, transform `translate(1px, 1px)`
4. Verify in the preview: hover, hold the mouse (pressed wins because it is *below* hovered), Tab to it (the focus
   ring is the `button` one — `focused` is not in this appearance), disable it (opacity 0.5 — also from `button`).
5. Drag `pressed` above `hovered` once, hold the mouse, watch the hovered colour win; drag it back. That is the
   state-order lesson in one gesture. Keep `default` first.

The console's other variants were added the same way: `metric-card` and `metric-strip` (inherit `panel`),
`heading-label`, `card-title`, `overline-label`, `muted-label`, `mono-label`, `status-label`, `banner-label` (inherit
`textlabel`, the Label's appearance key), `trace-list` (inherit `list`). `metric-strip` and `status-label` carry custom
states (`danger` / `warning` / `success`, `ok` / `warn` / `error`) that code selects with `Control.States`.

## Step 5 · Load it in the console

1. `Default.json`: `"theme": "AdaptiveOps"` (and `Web.config`: `<add key="Wisej.DefaultTheme" value="AdaptiveOps"/>`).
   Changing the theme shown in the Visual Studio designer does **not** change what the browser receives.
2. `AdaptiveOps.csproj`: `Themes\*.theme` as `Content`, `CopyToOutputDirectory = PreserveNewest` (both target
   frameworks) — the loader reads `/Themes` next to the running application.
3. `MainPage.cs`: `btnApplyTheme_Click` reads `Application.Theme.Name` into the status label, sets
   `btnSave.AppearanceKey = "action-button"`, inside `try/catch`; `MainPage_Load` reports the name too and shows a
   banner when the running theme is not `AdaptiveOps` (file missing or malformed → the framework falls back).
4. Remove every `BackColor`, `ForeColor`, `Font` that reached a look the theme now owns (Module 1's cards, strips,
   title fonts, status colours, banner colours, trace font — all gone; see `MainPage.Designer.cs`).
5. Run, compare the browser with the preview screenshots (`ThemePreviewScreenshots.md`), check the browser console
   for errors, and confirm the status label reports **AdaptiveOps**.

## Evidence

- `Themes/AdaptiveOps.theme`: `"name": "AdaptiveOps"`, 88 colours (the 14 palette entries first), 6 fonts, 95
  appearances including `action-button` (`"inherit": "button"`, states default › hovered › pressed) — `git diff`
  against a fresh Bootstrap-4 dump shows only the edits listed above.
- In the running console: `Apply theme` → Token inspector tab lists every token with its resolved value; `Walk
  states` → the trace resolves each path of the table in Step 3 plus the inherited/overridden states of
  `action-button`; `Base theme ⇄` → the same console on Bootstrap-4, for the before/after screenshot.
