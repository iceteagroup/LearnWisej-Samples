# Deliverable — the theme switch UI

*Module 10 deliverable · TicketOps Console · `Views/OperationsDashboard` header, `Infrastructure/ThemeSwitcher.cs`, `Services/SessionContext.cs`*

## What the operator sees

The dashboard header has a **Theme** picker (`Light · Bootstrap-4` / `Dark · BootstrapDark-4`). Picking a
theme re-skins the whole console — cards, grid, buttons, combo boxes, the chips and any dialog opened
afterwards — without rebuilding a control or reloading data. The footer line reads
*Theme: BootstrapDark-4 — applied to every screen.*

## What the code does

```csharp
// OperationsDashboard — the handler names a theme, nothing else
private void comboTheme_SelectedIndexChanged(object sender, EventArgs e)
{
    string themeName = comboTheme.SelectedIndex == 1 ? ThemeSwitcher.Dark : ThemeSwitcher.Light;
    _themes.Apply(themeName);
    labelFooter.Text = _localization.Format("Message.ThemeApplied", themeName);
}

// ThemeSwitcher — the only file that knows "Bootstrap-4"
public void Apply(string themeName)
{
    Application.Theme = BuildSessionTheme(themeName);   // re-renders every open window of this session
    Remember(themeName);                                // SessionContext + Application.Session
}
```

| Piece | Role |
|---|---|
| `ThemeSwitcher.Light` / `.Dark` | the two theme names, in one place (the lesson's "never scatter theme names") |
| `Application.Theme = theme` | the lesson's runtime switch: every open window of the session re-renders against the new theme |
| `BuildSessionTheme(name)` | reads the built-in theme JSON from `Wisej.Framework.dll`'s embedded resources (`Wisej.Platform.Themes.<name>.theme`), merges `Themes/TicketOps.mixin.theme` into it (`colors` + `appearances`) and wraps it in a `ClientTheme`. Built themes are cached per session, never in a static |
| `SessionContext.Theme` | the per-session memory of the choice (created per session by `AppComposition`) |
| `Application.Session.TicketOpsTheme` | the framework's per-session bag, mirrored for the lesson's `OnLoad` restore pattern |
| `ThemeSwitcher.RestoreSaved()` | called from `OperationsDashboard_Load` before the first paint: nothing saved → keep `Default.json`'s `"theme": "Bootstrap-4"`; something saved and different → re-apply |

## Why per session

`Application.LoadTheme(name)` also switches themes at runtime, but it is **process-wide**: every open
session re-skins. That is right for a tenant-wide rebrand and wrong for an operator's personal dark mode,
so the switch assigns `Application.Theme` for the current session only. A second browser tab keeps its own
theme.

## Why nothing else had to change

Neither screen sets a `BackColor` or `ForeColor`, fonts are `new Font("default", …)` — the theme's font —
and the chips colour themselves through theme states. So the switch is one line of behaviour and zero
lines of restyling. The one remaining literal palette (`StatusBanner`) is listed as a documented exception
in `docs/ModernizationChecklist.md`.

## Check it in the running app

1. **Theme → Dark · BootstrapDark-4**: the whole window is dark; footer *Theme: BootstrapDark-4 — applied to every screen.*
2. **Open detail…**: the dialog is painted in the current theme, its chip too.
3. Browser refresh (F5): the session survives and the theme stays.
