# Profile notes · six profiles, what each one changes, and why "behaviour, not widths"

**Files:** `ClientProfiles.json` (project root, `Content` with `CopyToOutputDirectory=PreserveNewest`, so it lands
next to the assembly), `MainPage.cs` (`ApplyProfile`), `Shell/NavigationRail.cs`, `Shell/TicketEditor.cs`,
`Dialogs/TicketEditorForm.cs`.

## 1. How the file is matched

```json
{
  "profiles": [
    { "name": "Phone",              "device": "Mobile",  "landscape": false },
    { "name": "Phone (Landscape)",  "device": "Mobile",  "landscape": true },
    { "name": "Tablet",             "device": "Tablet",  "landscape": false },
    { "name": "Tablet (Landscape)", "device": "Tablet",  "landscape": true },
    { "name": "Small Desktop",      "device": "Desktop", "maxWidth": 1024 },
    { "name": "Desktop",            "device": "Desktop" }
  ]
}
```

- The list is read **top to bottom**; the **first** profile whose rules all hold becomes
  `Application.ActiveProfile`. Nothing merges. No match means `ClientProfile.Default` ("Default"), which the
  console treats as Desktop.
- Rule keys: `device`, `landscape`, `minWidth` / `maxWidth` (browser window, re-evaluated while the window is
  dragged), `minScreenWidth` / `maxScreenWidth`, `userAgent`. Device and user agent only change on a reload,
  which is why the lab says to refresh after every device change.
- **Order is configuration.** The broad `Desktop` rule must stay last. The first five names match the framework's
  embedded defaults, so they override them; `Desktop` is added.

## 2. The mechanism

| Step | Where | What |
|---|---|---|
| Subscribe once | `MainPage()` | `Application.ResponsiveProfileChanged += Application_ResponsiveProfileChanged;` |
| Apply at startup | `MainPage()` | `ApplyProfile(Application.ActiveProfile?.Name)` |
| Route the event | `Application_ResponsiveProfileChanged` | `ApplyProfile(e.CurrentProfile?.Name)` |
| Show and log | `ApplyProfile` | `lblProfile` = `Profile: Phone · 390 px`; one line in the profile log (`Desktop → Phone`) |
| Unsubscribe | `Dispose(bool)` in `MainPage.Designer.cs` | both `Application.*` events; the reusable dialog is disposed too |

## 3. What changes per profile

| Region | Phone | Phone (Landscape) | Tablet | Tablet (Landscape) | Small Desktop | Desktop |
|---|---|---|---|---|---|---|
| **Toolbar** | icon-only, ☰ Menu, title "AdaptiveOps" | same | icon-only | icon-only | icon-only | labels |
| **Navigation** | hidden, sections in the ☰ menu | hidden | icon-only rail, 64 px | icon-only rail, 64 px | full rail, 220 px | full rail, 220 px |
| **Details** | panel hidden; the editor moves into `TicketEditorForm` and opens (maximized) when a row is selected | dialog, centred | docked **under** the grid, 320 px, scrolls | docked under the grid | docked right, 300 px | docked right, 340 px |
| **Metrics** | 2 × 2 | 2 × 2 | 4 × 1 | 4 × 1 | 4 × 1 | 4 × 1 |
| **Grid** | hides Owner, Due | hides Owner, Due | all columns | all columns | all columns | all columns |

Why these are behaviour and not CSS dimensions:

- A media query cannot hide a server-side control or open a Form. `Visible = false` on the server removes the
  rail and the details panel from the layout and from the round trips.
- The phone gets a different way to edit, not a smaller copy of the panel: the same editor instance reappears as
  a modal `Form`, with its own Close button and message line, so validation feedback is visible inside the modal.
- The tablet re-dock is a different composition: the grid gets the full width, the editor a scrolling card.
- Icon-only is a different command bar: the label survives as the tooltip and the accessible name.

## 4. Designer or code?

| Change | In Visual Studio | Here |
|---|---|---|
| `navigationPanel.Visible`, `detailsPanel.Visible` false on Phone | designer `Visible` per profile | assigned in `ApplyProfile` (the sample is hand-written) |
| toolbar `Display = Icon` | designer `Display` per profile | same |
| `detailsPanel.Dock = Bottom`, `Height`, `Width` | designer `Dock` / `Size` per profile | same |
| `colOwner.Visible`, `colDue.Visible` | designer values on the columns | same |
| **editor in a modal Form, opened once, reused** | **code** | `ResponsiveProfileChanged` → `ApplyProfile` → `OpenEditorDialog` |
| **☰ Menu with the rail's sections** | code | `btnMenu_Click` builds a `ContextMenu` from `NavigationRail.Sections` |
| **metrics 4 × 1 / 2 × 2** | code (or designer `ColumnCount` / `RowCount` plus cell positions) | `LayoutMetrics` |
| **profile log line, status label text** | code | `ApplyProfile` |

## 5. Idempotency: a phone rotating ten times

`ApplyProfile` computes the target state from the profile and **assigns** it; it never toggles. The two places
where applying twice could hurt are guarded: `HostEditorInDialog()` / `HostEditorInPanel()` return when the editor
is already in the target host, and `OpenEditorDialog()` returns when the dialog is already visible. The dialog is
created once and reused after `Close()`. Rotating four times leaves one editor, in the dialog, with the same values.

## 6. Testing without a phone

- **Width rules**: drag a desktop browser from 1440 to 1000 px and back: `Desktop → Small Desktop → Desktop`.
- **Device rules**: devtools device toolbar → iPhone 12 Pro → **refresh** → `Phone`; rotate → `Phone (Landscape)`
  (a rotation is a resize, no refresh needed); iPad → refresh → `Tablet`.
