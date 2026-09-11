# RuntimeThemeNotes · startup theme and the session-only switch

## Three places a theme can change

| Where | Code | Who sees it | Use it for |
|---|---|---|---|
| **Startup** | `Default.json` → `"theme": "AdaptiveOps"`, `Web.config` → `Wisej.DefaultTheme`; the file is `Themes/AdaptiveOps.theme` | every session | stable branding, versioned and reviewed |
| **Session-only** | `var dark = new ClientTheme("AdaptiveOps-Dark", Application.Theme); …; Application.Theme = dark;` (`btnTheme_Click`) | this session only | personalization (the night shift wants dark), tenant accents |
| **Shared object edited in place / `Application.LoadTheme`** | `Application.Theme.Colors["surface"] = …` after `LoadTheme`, or `Application.LoadTheme(name)` | **every session** | not used by this console: it would ship dark mode to everyone |

## The toolbar switch

`btnTheme_Click` toggles this session between light and dark:

- **Dark**: `SetSessionTheme(true)` builds a copy of the current theme with `new ClientTheme("AdaptiveOps-Dark",
  current)`, changes 27 colour tokens on the copy (`surface`, `surfaceAlt`, `textMain`, the grid row colours
  …), keeps a reference to the light theme in `Application.Session`, and assigns the copy to
  `Application.Theme`. The shared theme object is never written to.
- **Light**: assigns the stored light theme back to `Application.Theme` for this session.
- The choice is stored in `Application.Session["DarkTheme"]`; `MainPage_Load` re-applies it, so a reload
  keeps a dark session dark while a new session starts on the shared theme.
- The status label confirms the result: `Theme: AdaptiveOps-Dark (this session)`.
- Failure: the switch is wrapped in `try/catch`; on an error the current theme stays, the status label and
  an `AlertBox` say `Theme not applied: …`.

Only tokens change on the copy. Every appearance refers to colours by name, so the dark copy keeps every
state and appearance and simply resolves the names differently.

## Where this differs from the lab text

The lab and the video load a second named theme file, `AdaptiveOps-Dark`. This build creates the dark theme
as an in-memory copy of the shared theme with dark tokens, so no second 190 KB theme file has to be kept in
sync. The session boundary (assign `Application.Theme`, never edit the shared object) is the same.

## Evidence

Open `http://localhost:5503` in two browser sessions (a normal and a private window). Click **Dark theme** in
one: that console goes dark and its status label reads `Theme: AdaptiveOps-Dark (this session)`; the other
stays light. Reload the dark one: it comes back dark. Screenshot taken by the learner:
`screenshots/m3-two-sessions.png` (dark left, light right).
