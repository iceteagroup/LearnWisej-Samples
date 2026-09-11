# Responsive profiles — the three layouts the Orders screen supports (and nothing else is claimed)

`OrdersForm` was designed for one desktop resolution. On the web the same screen opens on a 27" monitor, a tablet and
a phone. Wisej.NET client profiles let one form carry different property values per profile; this module keeps the
mapping **in code** (`Services/ResponsiveLayout.cs`) so it is readable and testable — the designer's per-profile
property values (`Control.ResponsiveProfiles`) produce the same runtime effect.

## `ClientProfiles.json` (project root, copied to the output next to the assembly)

```json
{
  "profiles": [
    { "name": "Phone",   "maxWidth": 600 },
    { "name": "Tablet",  "minWidth": 601, "maxWidth": 1024 },
    { "name": "Desktop", "minWidth": 1025 }
  ]
}
```

Three named profiles by browser width, replacing the framework's embedded defaults (Phone, Phone (Landscape),
Tablet, Tablet (Landscape), Small Desktop, default). `Application.ActiveProfile` names the current one;
`Application.ResponsiveProfileChanged` fires when the browser crosses a boundary. `Startup.cs` never serves `.json`,
so the file is not downloadable.

## The three layouts (`ResponsiveLayout.Apply`)

| Profile | Grid | Detail panel | Toolbar | Actions ComboBox | Search box |
|---|---|---|---|---|---|
| **Desktop** ≥ 1025 px | 380 × 118, left | beside the grid (5 lines) | visible, full text: *New Order · Print Invoice · Export · Refresh* | hidden | 260 px |
| **Tablet** 601–1024 px | 602 × 66, full width | **below** the grid, one line (`FormatDetail` compresses it) | visible, short text: *New · Print · Export · Refresh* | hidden | 260 px |
| **Phone** ≤ 600 px | 602 × 84, full width | **hidden** | **hidden** | visible, full width — the four actions in one dropdown | full width |

Intentional choices, per the lesson: not every form must be phone-first. On a phone the order desk is a lookup
screen (search + list + the four actions); editing details belongs on a larger screen. The detail panel is hidden,
not squeezed.

## How it is wired

- Page load: `ApplyProfile(Application.ActiveProfile)` — the profile Wisej.NET matched from the first browser size
  report; `ResponsiveLayout.FromProfile` maps a profile name (or, for an unknown name, the browser width) to one of
  the three layouts.
- `Application.ResponsiveProfileChanged += …` in `Load`, `-=` in `Dispose` (it is a static event; the page must
  unsubscribe). The handler applies the layout of the new profile.

## How it was tested — and what is claimed

Tested: resizing the browser window across 600 / 1024 px — each layout renders, the grid keeps its five rows, the
detail text reflows, the phone ComboBox runs all four actions.

**Not tested, therefore not claimed:** a real phone browser, touch scrolling of the grid, landscape phones, the
Dashboard tab at phone width. `ReadinessChecklist.md` item 7 says exactly this.

## Evidence (in the running app, Orders tab)

- A wide browser: grid and detail side by side, toolbar *New Order · Print Invoice · Export · Refresh*.
- Narrow the window below 1025 px: the toolbar reads *New · Print · Export · Refresh* and the detail is one line under
  the grid.
- Below 601 px: the toolbar is gone and the *Actions…* dropdown is full width; choosing *New Order* there saves order
  1043.
