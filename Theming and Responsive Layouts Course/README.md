# Theming and Responsive Layouts Course · lab samples

One runnable Wisej.NET 4 application per module, built from the course's lesson guide, lab guide
and walkthrough video. Every sample is a stage of the same **Adaptive Operations Console**
(project `AdaptiveOps`): a toolbar, a navigation rail, a workspace with metric cards and a ticket
grid, a details editor and a status bar. Each screen shows only what that module's lab guide and
video build. Each folder has its own `README.md` (what to check, self-check answers) and a `docs/`
folder with the lab deliverables written as Markdown.

Requirements already on this machine: .NET 10 SDK and the `Wisej-4` 4.1.0 NuGet package. Nothing is deployed anywhere.

| Module | Folder | What it builds | Run |
|---|---|---|---|
| 1 · The Wisej.NET Visual System | `Module 1` | the console shell from five docked Panels, base theme chosen in `Default.json`, browser-width status label, `VisualInventory.md` and the "which layer owns what" note | `dotnet run -f net10.0 --urls http://localhost:5501` |
| 2 · Theme Builder and Theme JSON Internals | `Module 2` | `Themes/AdaptiveOps.theme` (tokens, fonts, appearances, states, inheritance) selected for the running app, Theme Builder steps | `http://localhost:5502` |
| 3 · CSS, CssClass, CssStyle, States, Runtime Themes | `Module 3` | appearance, `CssClass` with a scoped `Styles/AdaptiveOps.css`, one computed `CssStyle`, custom `stale` state, session-only dark theme | `http://localhost:5503` |
| 4 · Layout Fundamentals and Shell Composition | `Module 4` | shell split into UserControls composed by Dock only, Anchor, AutoSize, AutoScroll and MinimumSize | `http://localhost:5504` |
| 5 · Flow, Table and Flex Layouts | `Module 5` | a wrapping filter bar (`FlowLayoutPanel`), an aligned editor (`TableLayoutPanel`), a proportional dashboard (`FlexLayoutPanel`), fluent markup API | `http://localhost:5505` |
| 6 · ClientProfile and Responsive Properties | `Module 6` | custom `ClientProfiles.json`, `Application.ActiveProfile`, `ResponsiveProfileChanged` and an `ApplyProfile` that changes behaviour per profile (icon-only rail, details as dialog, stacked cards) | `http://localhost:5506` |
| 7 · Capstone, Accessibility, Performance, Governance | `Module 7` | the finished console across every profile: theme, CSS and profiles, accessibility review, QA matrix, architecture note, grounding pack and production checklist | `http://localhost:5507` |

Run any module from its `AdaptiveOps` project folder. The projects multi-target `net10.0-windows` and `net10.0`, so `dotnet run` needs a framework (`-f net10.0`, or `-f net10.0-windows`), e.g.

```bash
cd "D:/Projects/LearnWisej-Samples/Theming and Responsive Layouts Course/Module 3/AdaptiveOps"
dotnet run -f net10.0 --urls http://localhost:5503
```

or open the `AdaptiveOps.slnx` in the module folder with Visual Studio and press F5.

Testing profiles without a phone: resize the browser window (Small Desktop ≤ 1024 px) or use the
browser devtools device emulation (Mobile / Tablet device type and orientation); the profile is
re-evaluated on the next request or resize.

## `_template`

The scaffold every module was built from, plus `COOKBOOK.md`: the theme JSON structure as the
framework really reads it (fonts as `{size, family[], bold}` objects, `styles` vs `properties`,
state order, appearance keys such as `table-header-cell` and `tooltip/atom`), where theme files
and `ClientProfiles.json` live, `CssClass` / `CssStyle` / `AppearanceKey` / custom states, the
layout engines and their extended properties, the client-profile events, and the pack snippet
names that do not exist in Wisej.NET (`TableLayoutColumnStyle`, `PlaceholderText`). Read it before
writing a new sample.
