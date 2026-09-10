# Deliverable — the theme switch UI

*Module 10 deliverable · TicketOps Console · `Views/OperationsDashboard` header, `Infrastructure/ThemeSwitcher.cs`, `Services/SessionContext.cs`*

## What the operator sees

The dashboard header has a **Theme** picker (`Light · Bootstrap-4` / `Dark · BootstrapDark-4`) and a
**This session only** checkbox. Picking a theme re-skins the whole console — cards, grid, buttons,
combo boxes, the trace card, the chips and any dialog opened afterwards — without rebuilding a control
or reloading data. The footer line reads *Theme: BootstrapDark-4 — applied to every screen.*

## What the code does

```csharp
// OperationsDashboard — the handler names a theme, nothing else
private void comboTheme_SelectedIndexChanged(object sender, EventArgs e)
{
    string themeName = comboTheme.SelectedIndex == 1 ? ThemeSwitcher.Dark : ThemeSwitcher.Light;
    _themes.Apply(themeName, checkSessionOnly.Checked);
    labelFooter.Text = _localization.Format("Message.ThemeApplied", themeName);
}

// ThemeSwitcher — the only file that knows "Bootstrap-4"
public void Apply(string themeName, bool sessionOnly)
{
    if (sessionOnly) Application.Theme = BuildSessionTheme(themeName);   // this session
    else             Application.LoadTheme(themeName);                   // every session
    Remember(themeName);                                                 // SessionContext + Application.Session
}
```

| Piece | Role |
|---|---|
| `ThemeSwitcher.Light` / `.Dark` | the two theme names, in one place (the lesson's "never scatter theme names") |
| `Application.LoadTheme(name)` | **global** switch: the framework swaps the theme for every session of the process and merges the `/Themes/*.mixin.theme` files — the chip appearance included |
| `Application.Theme = new ClientTheme(name, json)` | **session-only** switch: the built-in theme JSON is read from `Wisej.Framework.dll`'s embedded resources (`Wisej.Platform.Themes.<name>.theme`), our mixin is merged into it (`colors` + `appearances`, `System.Text.Json`), and the result is assigned to *this* session. The framework's own mixin step (`ClientTheme.GetInstance`, `ApplyThemeMixins`) is internal, so the sample does that one step itself. Built themes are cached per session (`_sessionThemes`), never in a static |
| `SessionContext.Theme` | the per-session memory of the choice (created per session by `AppComposition`) |
| `Application.Session.TicketOpsTheme` | the framework's per-session bag, mirrored for the lesson's `OnLoad` restore pattern; wrapped in `try/catch` and traced |
| `ThemeSwitcher.RestoreSaved()` | called from `OperationsDashboard_Load` before the first paint: nothing saved → keep `Default.json`'s `"theme": "Bootstrap-4"`; something saved and different → re-apply |
| `Application.ThemeChanged` | subscribed by the dashboard: `[SESSION] Application.ThemeChanged → "BootstrapDark-4" — every open window re-renders …` |

## Global or per session?

`Application.LoadTheme` is what the course brief asks for and what the Integration course verified, but
it is **process-wide**: open the console in a second browser tab and switch to Dark in the first — the
second tab turns dark too (on its next request). That is right for a tenant-wide skin and wrong for an
operator's personal dark mode. The **This session only** checkbox shows the alternative: with it
checked, the second tab keeps its theme. The trace makes the difference explicit:

```
[INFRA] ThemeSwitcher.Apply — Application.LoadTheme("BootstrapDark-4") · global: every session of this process re-skins, no control was rebuilt
[INFRA] ThemeSwitcher.MergeMixin — TicketOps.mixin.theme: 5 colour/appearance entries merged into the session theme
[INFRA] ThemeSwitcher.Apply — Application.Theme = new ClientTheme("BootstrapDark-4", embedded JSON + TicketOps.mixin.theme) · this session only …
```

## Why nothing else had to change

Neither screen sets a `BackColor` or `ForeColor` (the template's `238,242,247` form and white cards were
removed in this module), fonts are `new Font("default", …)` — the theme's font — and the chips colour
themselves through theme states. So the switch is one line of behaviour and zero lines of restyling:
exactly the separation the lesson describes. The two remaining literal palettes (`StatusBanner`,
the trace footer) are listed as documented exceptions in `docs/ModernizationChecklist.md`.

## Evidence (running app)

1. Load → trace `[SESSION] ThemeSwitcher.RestoreSaved — no saved theme for this session → Default.json theme "Bootstrap-4"`.
2. **Theme → Dark · BootstrapDark-4** → `[UI] … → ThemeSwitcher.Apply("BootstrapDark-4", sessionOnly: False)`,
   `[INFRA] … Application.LoadTheme("BootstrapDark-4") · global …`, `[SESSION] ThemeSwitcher.Remember — SessionContext.Theme = "BootstrapDark-4" · Application.Session.TicketOpsTheme = …`,
   `[SESSION] Application.ThemeChanged → "BootstrapDark-4"`, `[UI] … 5 StatusChips … re-skinned by the theme — no chip code ran`;
   the whole window is dark; footer *Theme: BootstrapDark-4 — applied to every screen.*
3. Tick **This session only**, pick **Light** → `[INFRA] ThemeSwitcher.MergeMixin — … merged …`,
   `[INFRA] … Application.Theme = new ClientTheme("Bootstrap-4", …) · this session only`; a second tab (opened dark in step 2) stays dark.
4. **Open detail…** in either theme → the dialog is painted in the current theme; its chip too.
5. Browser refresh (F5) → the session survives, the theme stays; the trace shows `RestoreSaved — saved theme "…" already active`.
