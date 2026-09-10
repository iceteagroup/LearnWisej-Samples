# Deliverable · Profile notes — six profiles, what each one changes, and why "behaviour, not widths"

**Files:** `ClientProfiles.json` (project root, `Content` · `CopyToOutputDirectory=PreserveNewest` in
`AdaptiveOps.csproj`, so it lands in `bin/Debug/net10.0/` and `bin/Debug/net10.0-windows/` next to the
assembly), `MainPage.cs` (`ApplyProfile` / `ApplyKind`), `Shell/NavigationRail.cs`, `Shell/TicketEditor.cs`,
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

- The framework reads the list **top to bottom** and the **first** profile whose rules all hold becomes
  `Application.ActiveProfile`. Nothing merges: a client matches one profile or none (`ClientProfile.Default`,
  name `"Default"`, which the console treats as Desktop).
- Rule keys: `device` (`Mobile` | `Tablet` | `Desktop`, a string or a regex), `landscape` (`true`/`false`),
  `minWidth` / `maxWidth` (browser window, CSS pixels — re-evaluated while the window is dragged),
  `minScreenWidth` / `maxScreenWidth` (physical screen), `userAgent` (string or regex). Screen size, device
  and user agent describe the client itself and only change on a reload (or after switching a devtools
  emulator, which is why the lab says *refresh after every device change*).
- **Order is configuration.** The two phone entries share `device: Mobile` and differ only in `landscape`;
  the tablet pair does the same. `Small Desktop` is a Desktop device with `maxWidth: 1024`, and `Desktop`
  (device Desktop, no width rule) is the broad rule that must stay last. Move `Desktop` to the top and it
  shadows `Small Desktop` and nothing warns you; move `Small Desktop` to the top and it shadows nothing but
  itself is still only matched by desktops ≤ 1024 px (see the README self-check).
- The framework's embedded defaults are the first five names. Because ours use the same names they
  **override** the defaults; `Desktop` is added. `Application.Browser.Profiles` is the merged, ordered list the
  framework actually matches, and the console logs it at startup:
  `• server client profiles in match order (first match wins): Phone → Phone (Landscape) → Tablet → Tablet (Landscape) → Small Desktop → Desktop`
  followed by one indented line per profile with its rule. If `Desktop` is missing from that line, the file
  was not picked up (wrong folder, not copied to `bin`).

## 2. The mechanism, in the order the lab asks for it

| Step | Where | What |
|---|---|---|
| Subscribe once | `MainPage()` constructor | `Application.ResponsiveProfileChanged += Application_ResponsiveProfileChanged;` |
| Apply at startup | `MainPage()` constructor | `ApplyProfile(Application.ActiveProfile, "constructor")` so the first render is already right for this client |
| Route the event | `Application_ResponsiveProfileChanged` | `ApplyProfile(e.CurrentProfile, "Application.ResponsiveProfileChanged", e.PreviousProfile)` |
| Show + log | `ApplyProfile` → `ApplyKind` | `lblProfile` = `profile: Phone · browser 390 × 844 px · Mobile`; one trace line `• server profile change (reason): "Desktop" → "Phone" · browser …` plus one `→ render profile "Phone": toolbar: … · navigation: … · details: … · metrics: … · grid: … · status: …` |
| Unsubscribe | `Dispose(bool)` in `MainPage.Designer.cs` | `Application.ResponsiveProfileChanged -= …` (and `BrowserSizeChanged`); the page also disposes the reusable dialog |
| Container event | `MainPage_ResponsiveProfileChanged` | the same event on the Page (containers and DataGridView/ListView get it too) is only traced — proof that one handler is enough |

`ApplyProfile(ClientProfile profile, string reason, ClientProfile previous = null)` resolves the profile's
`Name` to one of six behaviours (`ProfileKind`: Phone, PhoneLandscape, Tablet, TabletLandscape, SmallDesktop,
Desktop; `"Default"`/null → Desktop). An unknown name is logged and **ignored** — the previous layout stays.
`ApplyKind` then applies six regions, each inside its own try/catch (`ApplyRegion`), so one faulty region never
stops the others.

## 3. What changes per profile — and why each change is behaviour

| Region | Phone (390×844) | Phone (Landscape) (844×390) | Tablet (768×1024) | Tablet (Landscape) (1024×768) | Small Desktop (≤1024, desktop device) | Desktop |
|---|---|---|---|---|---|---|
| **Toolbar** (`toolbarFlow`, 8 command buttons) | icon-only (`Display.Icon`, 36 px, label → `ToolTipText`/`AccessibleName`) · **Menu button shown** · title hidden | icon-only · Menu button · short title "AdaptiveOps" | icon-only | icon-only | icon-only | labels (`Display.Both`, designer widths) |
| **Navigation** (`navigationPanel` + `NavigationRail`) | **hidden** — the five sections come back as a `ContextMenu` under Menu | hidden → Menu | icon-only rail, region 64 px | icon-only rail, 64 px | full rail, 220 px | full rail, 220 px |
| **Details** (`detailsPanel` + `TicketEditor`) | **panel hidden; the editor moves into `TicketEditorForm`** and opens (maximized) when a row is selected | dialog, centred 420×620 | **docked UNDER the grid** (`Dock.Bottom`, 320 px; the card scrolls, editor `MinimumSize` 440) | docked Right, **300 px** | docked Right, 300 px | docked Right, 340 px |
| **Metrics** (`metricsTable`, TableLayoutPanel) | **1×4** (cards stacked) | 4×1 | 4×1 | 4×1 | **2×2** | 4×1 |
| **Grid** (`gridTickets`) | hides **Owner, Due** | hides Owner, Due | hides Owner | all columns | all columns | all columns |
| **Status bar** | profile + size, theme label hidden, status 110 px; trace card 132 px | same | profile + size + theme | same | same | same |

Why these are *behaviour* and not CSS dimensions:

- **A media query cannot hide a server-side control or open a Form.** The rail, the details panel and the
  grid columns are server objects; `Visible = false` on the server removes them from the layout and from the
  round trips. A CSS `display:none` would leave the details editor alive and focusable off-screen and would
  still ship its data.
- **The phone gets a different way to edit, not a smaller copy of the panel.** Hiding the side panel is only
  acceptable because the same editor instance re-appears as a modal `Form` on row selection — a different
  interaction, with its own Close button and its own message line so validation feedback is visible inside
  the modal instead of in a banner hidden behind it.
- **The tablet re-dock is a different composition**, not a narrower one: `Dock.Right → Dock.Bottom` changes
  what the grid gets (full width) and what the editor gets (a scrolling card of fixed height).
- **Icon-only is a different command bar.** `Display.Icon` changes what is rendered; the label survives as
  the tooltip and the accessible name, which a CSS `font-size: 0` would not guarantee.
- **The metrics change shape** (4×1 → 2×2 → 1×4) by changing `ColumnCount`/`RowCount` and the cell positions
  of the TableLayoutPanel, never a card's `Bounds`.

The two things that *are* widths (details 340 → 300, rail 220 → 64) are consequences of the behaviour change
(a narrower editor still has to hold the same fields; an icon-only rail needs less room), not the point of it.

## 4. Designer or code?

The lesson's rule: **deterministic per-profile values belong in the Designer** (the responsive-profile
dropdown writes them into `Control.ResponsiveProfiles`; the framework applies them through
`IHasResponsiveProfiles.ChangeProfile`), **code is for what a value cannot express**. Mapping this sample to
that rule:

| Change | In Visual Studio it would be… | Here it is in `ApplyKind` because… |
|---|---|---|
| `navigationPanel.Visible`, `detailsPanel.Visible` false on Phone | a designer `Visible` value per profile | the sample is hand-written (no `.resx`); the region reads as one table |
| toolbar `Display = Icon` below Desktop | a designer `Display` value per profile | same |
| `detailsPanel.Dock = Bottom` + `Height` on Tablet; `Width` 300/340 | designer `Dock` / `Size` values per profile | same |
| `colOwner.Visible`, `colDue.Visible` | designer values on the `DataGridViewColumn` (columns have `ResponsiveProfiles` too) | same |
| **editor into a modal Form, opened once, reused** | **code — `ResponsiveProfileChanged`** | it is not a property value: it reparents a control and shows a dialog |
| **Menu button → ContextMenu with the rail's sections** | code | it creates UI from data (the rail's section list) |
| **metrics 4×1 / 2×2 / 1×4** | code (or three designer-set `ColumnCount`/`RowCount` values plus cell positions) | it re-computes cell positions |
| **trace line per change, status label text** | code | it is logging |

So the README's statement holds: the Designer would own every row marked "same"; `ApplyProfile` keeps them
here so the whole mapping is readable in one method — the trade-off the course calls out (values in code are
invisible at design time).

## 5. Idempotency — a phone rotating ten times

`ApplyKind` computes the target state for every region from the kind and **assigns** it; it never toggles.
Assigning `Visible = false` to a hidden panel or `Display.Icon` to an icon-only button is a no-op on the
client. The two places where "apply twice" could hurt are guarded explicitly:

- `HostEditorInDialog()` / `HostEditorInPanel()` return immediately when `ticketEditor.Parent` is already the
  target, so the editor is reparented at most once per direction and never loses what was typed.
- `OpenEditorDialog()` checks `_editorForm.Visible` and logs `editor dialog already open — reused, not opened
  again`; `ShowDialog` on a visible form would throw, and a second form would be a second editor.
- `TicketEditorForm` is created once (`_editorForm == null || IsDisposed`) and reused after `Close()`
  (Wisej.NET does not dispose a closed dialog); it is disposed with the page.

Rotating Phone ↔ Phone (Landscape) four times therefore leaves exactly one editor, in the dialog, with the
same field values, and the fourth state equals the first.

## 6. Testing without a phone

- **Width rules**: drag a desktop browser from 1440 to 1000 px — `Desktop → Small Desktop` fires
  `ResponsiveProfileChanged` on the resize; back to 1440 fires it again. The status label changes on each.
- **Device rules**: Chrome/Edge devtools → device toolbar → iPhone 12 Pro (390×844) → **refresh** → `Phone`;
  rotate → `Phone (Landscape)` (fires without a refresh: the rotation is a resize); iPad (768×1024) → refresh
  → `Tablet`. Device type and user agent are only re-read on a reload.
- **Without devtools**: the toolbar's Phone / Tablet / Desktop buttons call `ApplyProfile` with the framework's
  own `ClientProfile` object found by name in `Application.Browser.Profiles`, so every behaviour above can be
  seen in a desktop browser; the status label adds "(simulated)" and **Re-apply** returns to the real profile.

## Evidence (what the running app shows)

- **Startup (desktop, 1348 px)**: status bar `● ready` · `profile: Desktop · browser 1348 × 680 px · Desktop` ·
  `theme: Bootstrap-4`. Trace: `• server profile change (constructor): "Desktop" → "Desktop" …`,
  `→ render profile "Desktop": toolbar: labels · navigation: rail full 220 px · details: editor docked Right 340 px · metrics: cards 4×1 · grid: all columns · status: profile + size + theme`,
  then the six-profile list from `Application.Browser.Profiles`.
- **Resize to ≤ 1024 px**: `• server profile change (Application.ResponsiveProfileChanged): "Desktop" → "Small Desktop" · browser 1000×680 px …`,
  toolbar buttons become icons, details 300 px, metric cards 2×2; a second line
  `← client Page.ResponsiveProfileChanged too: Desktop → Small Desktop (container-level; nothing to do …)`.
- **Phone button**: rail disappears, ☰ Menu appears, cards stack, Owner/Due columns gone, details panel gone;
  since a ticket is selected the editor opens as a maximized dialog once
  (`→ render editor dialog opened (profile change with T-1031 selected): TicketEditorForm.ShowDialog, maximized`).
  Pressing Phone again logs the same profile change and `editor dialog already open — reused, not opened again`.
- **Menu (☰)**: a five-item context menu; picking *Reports* retitles the workspace and logs
  `← client navigation: Reports (rail hidden → phone menu)`.
- **Tablet button**: rail shrinks to icons (hover shows the label), details region moves under the grid and
  scrolls, Owner column hidden.
- **Save inside the dialog** with an empty title: the red line `✖ Title is required. Nothing was written.`
  appears inside the editor (the banner behind the modal also shows it); with a valid title the green line
  `✓ T-1042 saved on the server` and the top-right toast appear and the grid behind refreshes.
- **Desktop / Re-apply**: the dialog closes if open, the editor is back in the docked card with its values,
  `• server closing the editor dialog: the profile is no longer a phone (nothing typed is lost …)`.
