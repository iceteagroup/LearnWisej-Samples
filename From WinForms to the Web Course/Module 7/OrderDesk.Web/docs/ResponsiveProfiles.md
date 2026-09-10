# Responsive profiles — the three layouts the Orders screen supports (and nothing else is claimed)

`OrdersForm` was designed for one desktop resolution. On the web the same screen opens on a 27" monitor, a tablet and
a phone. Wisej.NET client profiles let one form carry different property values per profile; this module keeps the
mapping **in code** (`Services/ResponsiveLayout.cs`) so it is readable, testable and shows up in the trace — the
designer's per-profile property values (`Control.ResponsiveProfiles`) produce the same runtime effect.

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

- Page load: `ApplyProfile(Application.ActiveProfile, "Application.ActiveProfile")` — the profile Wisej.NET matched
  from the first browser size report; `ResponsiveLayout.FromProfile` maps a profile name (or, for an unknown name,
  the browser width) to one of the three layouts.
- `Application.ResponsiveProfileChanged += …` in `Load`, `-=` in `Dispose` (it is a static, per-session event; the
  page must unsubscribe). The handler traces `previous → current · browser W×H` and applies the layout.
- **Simulate desktop / tablet / phone** call the same `Apply(kind)` the event calls — what the buttons show is
  exactly what a resized browser gets. They exist so the three layouts can be reviewed without resizing.

## How it was tested — and what is claimed

Tested: the three simulate buttons (each layout renders, the grid keeps its five rows, the detail text reflows, the
phone ComboBox runs all four actions), and resizing the browser window across 600 / 1024 px, which produces the
`← JS→.NET ResponsiveProfileChanged Desktop → Tablet · Tablet · browser 900×…` trace line followed by the same
`ResponsiveLayout.Apply` line the simulate button produces.

**Not tested, therefore not claimed:** a real phone browser, touch scrolling of the grid, landscape phones, the
Security / Dashboard / Readiness tabs at phone width (they are the lab console, not the migrated screen). The
readiness checklist item 7 says exactly this: "tested = what docs/ResponsiveProfiles.md lists, nothing more".

## Evidence (in the running app, Orders tab)

- Page load in a wide browser: `labelProfile` = `Application.ActiveProfile: Desktop · browser 1348×… → Desktop`;
  trace `→ .NET→JS ResponsiveLayout.Apply Desktop ← Application.ActiveProfile (Desktop) · desktop: grid + detail
  side by side · toolbar icon + full text · grid 380×118`.
- **Simulate tablet**: trace `← JS→.NET simulate Tablet — the same Apply a real Tablet profile triggers` and
  `→ .NET→JS ResponsiveLayout.Apply Tablet · tablet: detail below the grid · toolbar icon + short text · grid 602×66`;
  the toolbar reads *New · Print · Export · Refresh*, the detail is one line under the grid.
- **Simulate phone**: `… phone: detail hidden · toolbar hidden · actions in one ComboBox · grid 602×84`; the toolbar
  is gone, the *Actions…* dropdown is full width; choosing *New Order* there traces
  `← JS→.NET comboActions (phone) "New Order" — the ComboBox replaces the toolbar on ≤600 px` and saves order 1043.
- **Simulate desktop**: back to the side-by-side layout.
- Resize the browser below 1025 px and reload (or wait for the event): the `ResponsiveProfileChanged` trace line.
