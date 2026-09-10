# RuntimeThemeNotes — startup theme, global switch, session-only switch (Module 3)

## Three places a theme can change

| Where | Code | Who sees it | Use it for |
|---|---|---|---|
| **Startup** | `Default.json` → `"theme": "AdaptiveOps"` and `Web.config` → `Wisej.DefaultTheme = AdaptiveOps`; the file is `Themes/AdaptiveOps.theme` in the project folder | every session, before the app runs | stable branding: versioned, reviewed, identical for every user. The theme shown in the Visual Studio designer changes nothing at runtime. |
| **Global at runtime** | `Application.LoadTheme("MaterialDark-4")` — toolbar button *Global: MaterialDark-4*; `Application.LoadTheme("AdaptiveOps")` — *Back to AdaptiveOps* and *Reset* | **every session** on this server, on its next request | controlled previews, an operator-wide switch, hot reload of the theme file after a designer edit |
| **Session-only at runtime** | `var dark = new ClientTheme("AdaptiveOps-Dark", Application.Theme); dark.Colors["surface"] = "#1E2430"; … ; Application.Theme = dark;` — *Session-only dark* | **this session only** | personalization (night shift wants dark), tenant accents, demos |

What must never happen: `Application.Theme.Colors.surface = "#1E2430"` on the theme that `LoadTheme`
installed. That mutates the shared object in place and ships dark mode to everyone. The console never
does it: `ApplySessionDarkTheme` builds a *copy* (`new ClientTheme(name, baseTheme)`), changes 27
colour tokens on the copy, and assigns the copy. The line that guarantees the boundary is the
constructor call — the copy is a new object, the shared theme is not referenced again except to read
its `surface` token for the trace.

Only *tokens* are changed on the copy. Every appearance in the theme (`card`, `metric-card`,
`action-button`, `status-label`, the Bootstrap-4 ones) refers to colours by name, so the dark copy
inherits all states and all appearances unchanged and simply resolves the names differently.

## Remembering the choice

`Application.Session["ThemeChoice"] = "dark"` (`RememberThemeChoice`). `Application.Session` is a
per-session dynamic store; on page load `RestoreSessionTheme` reads it and re-applies the copy, so a
browser reload keeps the dark session dark while a *new* session (second browser, private window)
starts on the shared theme. *Back to AdaptiveOps*, *Global* and *Reset* clear the key
(`ForgetThemeChoice`), because after those the session follows the shared theme again.

## Failure handling: never half-themed

`SwitchGlobalTheme` and `ApplySessionDarkTheme` wrap the change in `try/catch`, keep a reference to the
theme the session had, and put it back (`Application.Theme = before`) if the change throws or if the
active theme after the call is not the one requested. *Style miss* exercises this with
`LoadTheme("Nope")`: the trace shows the exception type and message and then
`• server theme restored for this session: AdaptiveOps`.

## A side effect worth knowing

After a **global** swap to a stock theme, the console's own appearances (`card`, `metric-card`,
`action-button`, `muted-label`, `status-label`, `banner-label`, `metric-title`, `metric-value`) do not
exist in that theme, so the controls that ask for them fall back to plain widgets: transparent cards,
default text. The trace says so (`• server note: MaterialDark-4 has no action-button / card / … appearances`).
That is the theme owning those decisions, not a bug — and it is the argument for shipping app
appearances *in* the branded theme (or as a `Themes/*.mixin.theme` that merges into whichever theme is
active) rather than patching them from code.

## What the trace showed

- **Session-only dark** →
  `→ render Application.Theme = new ClientTheme("AdaptiveOps-Dark", Application.Theme) (Session-only dark): 27 colour tokens changed ON THE COPY (surface #1E2430, surfaceAlt #141A24, …) — THIS SESSION ONLY`
  `• server proof: shared theme 'AdaptiveOps' Colors.surface was #FFFFFF and is still #FFFFFF; a second tab keeps the light theme. Choice stored in Application.Session["ThemeChoice"] = dark`
  Status bar: `theme: AdaptiveOps-Dark · this session only`. A second tab opened at the same URL renders
  light and its own trace says `• server session theme choice: none stored`.
- **Reload the dark tab (F5)** → `• server session theme choice restored from Application.Session: dark`
  and the page comes back dark.
- **Global: MaterialDark-4** →
  `→ render Application.LoadTheme("MaterialDark-4") (Global: MaterialDark-4): GLOBAL — the shared theme changed from AdaptiveOps to MaterialDark-4; every session on this server follows on its next request`
  The second tab turns dark on its next round-trip (click anything in it). Status bar of both:
  `theme: MaterialDark-4 · shared (every session)`.
- **Back to AdaptiveOps** → `→ render Application.LoadTheme("AdaptiveOps") (Back to AdaptiveOps): GLOBAL — … from MaterialDark-4 to AdaptiveOps …`; both tabs light again.
- **Style miss** → `• server LoadTheme("Nope") FAILED (Style miss): <exception>` then
  `• server theme restored for this session: AdaptiveOps (never left half-themed)`.

## Facts to confirm at runtime (from the framework docs, not yet observed in this build)

- `new ClientTheme(name, Application.Theme)` + `Application.Theme = copy` renders the copy for this
  session only — the in-process check confirmed that mutating the copy leaves the shared object's tokens
  untouched (`copy surface=#1E2430 shared surface=#FFFFFF`); the browser rendering is what the reviewer checks.
- `Application.LoadTheme("Nope")` throws (or leaves `Application.Theme.Name` ≠ "Nope") — the code handles both.
- `Application.Session` is a `Wisej.Core.DynamicObject` at runtime (indexer / `Contains` / `Delete` used);
  if it were not, `RememberThemeChoice` is a no-op and the trace shows `none stored` after a reload.

## Evidence

Two browser sessions side by side (a normal and a private window, or two browsers) on
`http://localhost:5503`: switch *Session-only dark* in one — the other stays light; click *Global:
MaterialDark-4* in either — both change; *Back to AdaptiveOps* — both return. Screenshots taken by the
learner: `screenshots/m3-two-sessions.png` (dark left, light right), plus the desktop/phone pairs listed
in `CssDecisions.md`.
