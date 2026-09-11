# Architecture note: the layered visual system as built

For every visible decision this note names the rung of the course decision tree that owns it.
`docs/GroundingPack.md` turns it into answer rules an assistant can cite.

## The decision tree, applied in order

| # | Rung | Owns in this app |
|---|---|---|
| 1 | **Theme** (`Themes/AdaptiveOps.theme`) | identity of every standard control: colours, fonts, radius, focus frame, invalid state, grid header, tooltip, page background |
| 2 | **AppearanceKey** | business variants: `action-button`, `nav-item`, `metric-card`, `surface-card`, `rail-surface`, `compact-editor`, and the label appearances |
| 3 | **CssClass** (`Styles/AdaptiveOps.css`) | uppercase and letter-spacing on card titles, tabular numerals on card values |
| 4 | **CssStyle** | nothing |
| 5 | **Layout containers** | Dock (five regions), FlowLayoutPanel (toolbar, metric cards), TableLayoutPanel (editor form), FlexLayoutPanel (grid region) |
| 6 | **Responsive properties** | the per-profile values `ApplyProfile` assigns |
| 7 | **ResponsiveProfileChanged** | the details editor opens as a modal `DetailsDialog` on phone |
| 8 | **ClientProfiles.json** | the six profile definitions, narrow to broad |
| 9 | **Production governance** | this note, the checklist, the QA matrix, source control |

## Layer 1: Theme

- **Derived from a base theme.** `Themes/src/AdaptiveOps.overrides.json` is the source we edit (`inherit:
  "Bootstrap-4"`, `settings`, `colors`, `fonts`, `appearances`). `Themes/src/merge-theme.mjs` merges it over
  the embedded Bootstrap-4 theme into the shipped `AdaptiveOps.theme`. Never edit the merged file by hand.
- **Tokens**: `brandPrimary/Hover/Pressed`, `brandAccent`, `surface`, `surfaceAlt`, `surfaceRail`,
  `surfaceHover`, `borderSoft`, `textMain`, `textMuted`, `danger`, `dangerHover`, `dangerSurface`, `warning`,
  `success`, `focusFrame`, `focusShadow`, `invalid`. The Bootstrap system colour names point to the same
  values, so inherited appearances already wear the brand.
- **Fonts**: `default`, `defaultBold`, `heading`, `subheading`, `cardTitle`, `cardValue`, `mono`, referenced
  by name from appearances. No `Font` is set in code.
- **Settings**: `borderRadius 6`, `focusBorderSize 3`, `focusBlurRadius 0`.
- **Overridden base appearances**: `page`, `button`, `textbox` (with the `invalid` state),
  `table-header-cell`, `tooltip/atom`.

## Layer 2: AppearanceKey

| Appearance | Inherits | Used by |
|---|---|---|
| `action-button` | `button` | Save, Open details |
| `destructive-button` | `action-button` | defined for delete actions; not assigned in this build |
| `nav-item` | `button` | the five rail buttons; custom state `selected` |
| `surface-card` | `panel` | toolbar, grid and status cards, editor |
| `metric-card` | `surface-card` | the four metric cards |
| `rail-surface` | `panel` | the navigation rail |
| `compact-editor` | `textbox` | text editors on narrow profiles and in the phone dialog |
| `heading-label`, `subheading-label`, `muted-label`, `mono-label`, `metric-title`, `metric-value`, `status-label` | `textlabel` | every label |
| `validation-label` | `textlabel` | editor footer; state `error` |

## Layer 3: CssClass

| Class | Rule | Why CSS |
|---|---|---|
| `metric-card .metric-title` | uppercase, letter-spacing | not theme properties; app-specific content |
| `metric-card .metric-value` | tabular numerals | same |

Both rules use inherited text properties, so the stylesheet never touches a widget's internal DOM.

## Layer 4: CssStyle

No control carries a `CssStyle`.

## Layer 5: Layout containers

```
MainPage (Page)                              toolbar Top 56 · status Bottom 28 · navigation Left · details Right · workspace Fill
├─ toolbarPanel › toolbarCard › toolbarFlow  FlowLayoutPanel: title, Refresh
├─ navigationPanel › NavigationRail          five nav-item buttons; IconOnly switches Display
├─ workspacePanel (MinimumSize 320×240)
│  ├─ metricsFlow                            FlowLayoutPanel, wraps; the profile sets the row budget as Height (92 / 184)
│  │  └─ 4 × MetricCard                      140×84, Margin 0 0 8 8
│  └─ contentFlex                            FlexLayoutPanel › gridCard (Min 280×200) › gridHeader + DataGridView (Min 240×160)
├─ detailsPanel (Min 280, Max 420 wide; Bottom on portrait tablet: Min 240, Max 360 tall) › DetailsEditor
│  └─ TableLayoutPanel 2 columns (Absolute 84 · Percent 100); footer Save and validation label
└─ statusPanel › statusCard                  lblStatus Left · widthLabel Fill · lblProfile Right
```

Docking priority is the child order (the control added last docks first). `Padding` on the region panels
is the gap between regions, because Dock ignores `Margin`. Nothing sets `Bounds` or `Location` in response
to a size change.

## Layer 6: per-profile values (`ApplyProfile`)

| Value | Desktop | Small Desktop / Tablet (Landscape) | Tablet | Phone (Landscape) | Phone |
|---|---|---|---|---|---|
| rail | text, 220 | icon-only, 68 | icon-only, 68 | icon-only, 60 | icon-only, 60 |
| details | Right 340 | Right 300 | **Bottom 300** | hidden, dialog | hidden, dialog (maximized) |
| editor | normal | compact | compact | compact | compact |
| card rows | 1 | 2 | 2 | 1 | 2 |
| grid columns | 6 | 5 (no Owner) | 5 | 3 (Id, Title, Status) | 3 |
| toolbar | text and icon | text and icon | icon-only | icon-only | icon-only |
| app title | full | full | full | "Adaptive Ops" | "Adaptive Ops" |

`ApplyProfile(name)` returns early when `name == _lastProfile`, assigns final values, never reloads data,
and is called from the constructor and from `Application.ResponsiveProfileChanged` (`Dispose`
unsubscribes). A failure inside it is reported in the status bar and undone by re-applying the last good profile.

## Layer 7: ResponsiveProfileChanged behaviour

On phone profiles **Open details** creates a `DetailsDialog` (the same `DetailsEditor`, `Compact = true`,
maximized on phone) and shows it with the non-blocking `ShowDialog(callback)`. Saving in the dialog goes
through the same `TrySave` as the docked editor.

## Layer 8: `ClientProfiles.json`

`Phone`, `Phone (Landscape)`, `Tablet`, `Tablet (Landscape)`, `Small Desktop (maxWidth 1024)`,
`Desktop (minWidth 1025)`. First match wins, so the narrow entries come first. The framework's `Default`
profile is treated as Desktop by `ProfileNameOf`.

## Layer 9: Production governance

`docs/ProductionChecklist.md` lists the rules with their evidence; `docs/ResponsiveQAMatrix.md` is rerun
after every theme, CSS or profile change and after every Wisej.NET upgrade.

## Performance decisions

- `ApplyProfile` assigns a dozen properties inside one `SuspendLayout/ResumeLayout(true)` pair.
- The grid is re-filled from the repository only on load, Refresh and Save, never on a profile change.
