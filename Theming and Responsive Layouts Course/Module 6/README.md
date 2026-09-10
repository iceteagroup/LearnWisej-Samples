# AdaptiveOps · Theming & Responsive Layouts Course · Module 6

Local lab build for **Module 6 · ClientProfile and Responsive Properties** — *profiles that change behaviour,
not just widths*. It follows the walkthrough video and the lab: a `ClientProfiles.json` in the project root
defines six profiles narrow → broad (Phone, Phone (Landscape), Tablet, Tablet (Landscape), Small Desktop,
Desktop); the framework matches them top to bottom into `Application.ActiveProfile`; the status bar names the
active profile; `Application.ResponsiveProfileChanged` routes every change through one `ApplyProfile` method
that hides the rail and opens the ticket editor as a modal `Form` on a phone, docks the details region under
the grid on a tablet, switches the toolbar and the rail to icon-only buttons, re-shapes the metric cards and
hides the grid's secondary columns — and logs every change with the previous and current profile and the
browser size. Toolbar buttons simulate the profiles so all of it can be seen in a desktop browser without
devtools.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Theming and Responsive Layouts Course/Module 6/AdaptiveOps"
dotnet run -f net10.0 --urls http://localhost:5506
```

Then open <http://localhost:5506>. (Visual Studio: open `AdaptiveOps.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try

| Action | Path | What you should see |
|---|---|---|
| Open the page in a wide desktop window | startup | status bar `● ready` · `profile: Desktop · browser 1348 × 680 px · Desktop` · `theme: Bootstrap-4`; trace: `• server profile change (constructor): "Desktop" → "Desktop" …`, `→ render profile "Desktop": toolbar: labels · navigation: rail full 220 px · details: editor docked Right 340 px · metrics: cards 4×1 · grid: all columns · status: …`, then `• server client profiles in match order (first match wins): Phone → Phone (Landscape) → Tablet → Tablet (Landscape) → Small Desktop → Desktop` with one rule line each |
| Drag the window to ≤ 1024 px and back | success (real event) | `• server profile change (Application.ResponsiveProfileChanged): "Desktop" → "Small Desktop" · browser 1000×680 px · screen … · Desktop`; toolbar buttons become icons, details region 300 px, metric cards 2×2; a second line `← client Page.ResponsiveProfileChanged too …`; dragging back logs `"Small Desktop" → "Desktop"` |
| Devtools device toolbar → iPhone 12 Pro → **refresh** | success (device rule) | `profile: Phone · browser 390 × 844 px · Mobile`; rail gone, ☰ **Menu** in the toolbar, cards stacked 1×4, grid shows Id · Title · Priority · Status only; tap a row → the editor opens as a **maximized dialog** with a Close button; rotate → `Phone (Landscape)` without a refresh, dialog centred |
| **Phone** / **Tablet** / **Desktop** toolbar buttons | success (simulated) | the same behaviours in a desktop window: `• server profile change (simulate button: ClientProfile "Phone" from Application.Browser.Profiles): "Desktop" → "Phone" …`; status bar adds "(simulated)" and `● simulating Phone`; because a ticket is selected the Phone button opens the editor dialog **once** — press it again and the trace says `editor dialog already open — reused, not opened again` |
| ☰ Menu (Phone) → *Reports* | success | a 5-item context menu; the workspace title becomes "Reports"; `← client navigation: Reports (rail hidden → phone menu)` |
| **Tablet** | success | rail shrinks to 64 px icons (hover: "Dashboard", "Tickets" …), details region moves **under the grid** (320 px, scrolls), Owner column hidden |
| Save in the dialog with an empty Title | failure (validation) | the red line `✖ Title is required. Nothing was written.` **inside the editor** (the page banner is behind the modal), toast top-right, grid unchanged; type a title → Save → green `✓ T-1042 saved on the server` and the grid behind refreshes |
| **Step profiles** | progress | a `Wisej.Web.Timer` applies Desktop → Small Desktop → Tablet → Phone → Tablet → Small Desktop → Desktop, one per 1.5 s; status counts `● stepping 3/7 · Tablet`; then `• server recovery (step-through finished): Application.ActiveProfile re-read → "Desktop"` |
| **Unknown** | failure 1 | `• server profile "Kiosk" not found (Unknown button); known profiles: Phone, Phone (Landscape), …; keeping "Desktop"`; banner `✖ Profile "Kiosk" does not exist in ClientProfiles.json …`; layout unchanged |
| **Faulty region** | failure 2 | `• server fault injected into the "metrics" region …`, then `• server region "metrics" failed: injected fault … — the remaining regions still apply`, `→ render profile "Desktop": toolbar: labels · … · metrics: FAILED · grid: all columns · …`, banner `✖ 1 region(s) failed …; the other 5 applied. Re-apply recovers.` |
| **Re-apply** | recovery | `• server recovery (Re-apply button): Application.ActiveProfile re-read → "Desktop"`; simulation ends, "(simulated)" and the banner disappear, the dialog closes if it was open and the editor is back in the docked card **with the values you typed** |
| Refresh | – | reload from the repository; never opens the phone dialog (only a user's row selection does) |
| ✕ in the trace card header | – | clears the trace |

The trace card ("Layout & theme · live trace") is the evidence: every profile change is one `• server profile
change …` line (previous → current, browser and screen size, device) and one `→ render profile …` line listing
what each of the six regions did.

### Designer vs. code — read this before comparing with Visual Studio

The lab assigns the deterministic per-profile values (rail and details `Visible` false on Phone, toolbar
`Display` icon-only on Phone and Tablet, details `Dock` Bottom on Tablet) in the **Designer's responsive-profile
dropdown**; Visual Studio stores them in `Control.ResponsiveProfiles` and the framework applies them. This
sample is written by hand (no `.resx`), so the same values are expressed in `MainPage.ApplyKind`, one region per
block, next to the things that genuinely need code (moving the editor into a modal Form, the ☰ menu, the
metrics re-shape, the trace). `docs/ProfileNotes.md` § 4 maps every row to "would be a designer value" or
"must be code".

## Where things live

```
Module 6/
├─ README.md                          this file
├─ AdaptiveOps.slnx
└─ AdaptiveOps/
   ├─ ClientProfiles.json             deliverable 1 — six profiles, narrow → broad (Content, copied to bin for both targets)
   ├─ AdaptiveOps.csproj              <Content Update="ClientProfiles.json" CopyToOutputDirectory=PreserveNewest>
   ├─ MainPage.cs                     ApplyProfile / ApplyKind (six regions, try/catch each), event wiring, lab paths, trace
   ├─ MainPage.Designer.cs            the shell: toolbar FlowLayoutPanel, regions, metrics TableLayoutPanel, grid, status bar; Dispose unsubscribes
   ├─ Shell/NavigationRail.cs (+ .Designer.cs)   the rail UserControl: SetMode(Full | IconOnly), SectionSelected, Sections
   ├─ Shell/TicketEditor.cs (+ .Designer.cs)     the editor UserControl: LoadTicket / ReadTicket / ShowMessage, Save + Close events
   ├─ Dialogs/TicketEditorForm.cs (+ .Designer.cs)  the reusable modal host: HostEditor(control)
   ├─ Models/Ticket.cs · TicketRepository.cs   the same 12 tickets and server-side validation as every module
   ├─ Default.html · Default.json · Web.config · Program.cs · Startup.cs
   ├─ Properties/launchSettings.json  http://localhost:5506
   └─ docs/
      ├─ ProfileNotes.md              every profile, its rule, what changes per region, why behaviour not widths, designer-vs-code, idempotency
      ├─ ResponsiveQAMatrix.md        profile × region × expected behaviour × how to test, event checks, known limits
      └─ LabNotes.md                  every hidden control with its mobile alternative; theme / CSS / layout / profile decisions
```

## Deliverables

1. **`ClientProfiles.json` with Phone, Phone Landscape, Tablet, Tablet Landscape, Small Desktop and Desktop in first-match order** — [`AdaptiveOps/ClientProfiles.json`](AdaptiveOps/ClientProfiles.json), explained in [`AdaptiveOps/docs/ProfileNotes.md`](AdaptiveOps/docs/ProfileNotes.md) § 1
2. **Responsive values that hide the navigation rail and the details panel on phone** — `ApplyKind` regions *navigation* and *details* in [`AdaptiveOps/MainPage.cs`](AdaptiveOps/MainPage.cs); mapping to the Designer in [`ProfileNotes.md`](AdaptiveOps/docs/ProfileNotes.md) § 4
3. **Icon-only toolbar `Display` and the re-docked details region on tablet** — `ApplyKind` regions *toolbar* and *details*, `NavigationRail.SetMode`
4. **`ResponsiveProfileChanged` handler that shows and logs the active profile name** — `Application_ResponsiveProfileChanged` → `ApplyProfile` → `lblProfile` + trace; unsubscribed in `MainPage.Designer.cs` `Dispose`
5. **Details editor opened as a modal form on phone through the profile handler** — `HostEditorInDialog` / `OpenEditorDialog` with [`Dialogs/TicketEditorForm.cs`](AdaptiveOps/Dialogs/TicketEditorForm.cs)
6. **Responsive QA matrix** — [`AdaptiveOps/docs/ResponsiveQAMatrix.md`](AdaptiveOps/docs/ResponsiveQAMatrix.md)
7. **Lab notes: every hidden control with its mobile alternative, designer vs. handler** — [`AdaptiveOps/docs/LabNotes.md`](AdaptiveOps/docs/LabNotes.md)

## Lab step → code map

| Lab step (tr6s2 / labs/m6.json) | Where |
|---|---|
| Open the project as Module 5 left it; no `ClientProfiles.json` yet | the Module 1 shell is the base; everything below is Module 6's addition |
| `ClientProfiles.json` in the application root, six profiles in first-match order; confirm the names | `AdaptiveOps/ClientProfiles.json`; the csproj `Content Update`; `MainPage.LogKnownProfiles()` prints `Application.Browser.Profiles` in order at startup |
| Rail UserControl and `pnlDetails` `Visible` false on both Phone profiles; `gridTickets` keeps its `MinimumSize` | `ApplyKind` → *navigation* (`navigationPanel.Visible = false`) and *details* (`detailsPanel.Visible = false`); `gridTickets.MinimumSize = 280×120` in the Designer, checked by `VerifyShell()` |
| Toolbar buttons `Display` icon-only on Phone and Tablet; `pnlDetails.Dock` Right → Bottom on the Tablet profiles | `ApplyKind` → *toolbar* (`Display.Icon`, width 36, `AccessibleName`) and *details* (`Dock = Bottom`, `Height = 320` on Tablet; Right 300 px on Tablet (Landscape)) |
| Subscribe once to `Application.ResponsiveProfileChanged`, `ApplyProfile` sets `lblProfile.Text` and writes one log line, called in the constructor with `ActiveProfile` and from the handler with `e.CurrentProfile`, unsubscribed in `Dispose` | `MainPage()` constructor; `Application_ResponsiveProfileChanged`; `ApplyProfile(ClientProfile, string, ClientProfile)`; `MainPage.Designer.cs` `Dispose(bool)` |
| On Phone, open the editor with `ShowDialog`, reuse an open form, lose nothing typed when returning to a wider profile | `HostEditorInDialog`, `OpenEditorDialog` (`_editorForm.Visible` guard, `ShowDialog` with the close callback), `HostEditorInPanel` (same `TicketEditor` instance reparented) |
| Fill in the responsive QA matrix | `docs/ResponsiveQAMatrix.md` |
| Show every path: drag 1440 → 390 and back, device emulation with a refresh, success toast and validation error inside the modal | the *What to try* table; `TicketEditor.ShowMessage` inside the editor + `AlertBox` toast |
| Review & run: compare with the matrix, capture the profile label, write the lab notes | `docs/LabNotes.md`; `lblProfile` is docked Fill in the status bar on every profile |

## Self-check answers (lab guide)

- **If Small Desktop were moved to the top of `ClientProfiles.json`, which of the other five profiles could
  still be matched, and what would a tablet in landscape see?**
  All five would still be matched, because `Small Desktop` only claims clients whose `device` is `Desktop`
  *and* whose browser width is at most 1024 px; a phone or a tablet never satisfies the device rule, so the
  evaluation falls through to Phone, Phone (Landscape), Tablet and Tablet (Landscape) exactly as before, and a
  desktop wider than 1024 px falls through to Desktop. Moving it up therefore changes nothing in this file —
  the dangerous move is the other one, putting the ruleless-by-width `Desktop` entry (or any entry with no
  rules at all) at the top, because a broad rule matches first and silently shadows everything below it. A
  tablet in landscape would still see `Tablet (Landscape)`: icon-only rail, details docked on the right at
  300 px, icon-only toolbar. The console shows the effective order at startup in the trace
  (`• server client profiles in match order …`) so a shadowing mistake is visible on the first run.

- **The tablet re-dock lives in the designer and the phone modal editor lives in code. What decides which
  mechanism owns each change, and what would go wrong if you swapped them?**
  Whether the change is a *value* or an *action*. `pnlDetails.Dock = Bottom` on the Tablet profiles is a
  deterministic property value that depends on nothing but the profile, so it belongs in the Designer's
  responsive-profile dropdown: it is serialized with the form, visible to the next developer in the property
  grid and applied by the framework through `Control.ResponsiveProfiles`. Opening the editor as a modal
  `Form` is not a value — it reparents a control, calls `ShowDialog`, must reuse an open form and must not fire
  on a refresh — so it can only be `ResponsiveProfileChanged` code. Swapping them: the re-dock in code would
  work but drift — the Designer would keep showing the desktop layout, and a typo in the profile-name check
  would skip the tablet silently; the modal editor in the Designer is impossible, and the nearest imitation (a
  second, designer-placed editor made visible on Phone) would produce two editors holding two copies of the
  ticket. In this hand-written sample the designer-type values are expressed in `ApplyKind` so the mapping
  reads as one table, and the README and `docs/ProfileNotes.md` § 4 say which rows Visual Studio would own.

- **A tester rotated a phone four times and your handler ran four times. Which state must be identical after
  the fourth run and after the first, and how does your code guarantee it?**
  Everything the handler touches: exactly one `TicketEditor` instance, hosted in exactly one
  `TicketEditorForm`, shown once, holding the values the tester typed; the rail hidden; the Menu button
  visible; the same grid columns; the same status text. The guarantee is that `ApplyKind` *assigns* target
  state instead of toggling it — `Visible = false`, `Display = Icon`, the metric grid shape — so a repeat is a
  no-op on the client; and the two operations that are not naturally idempotent are guarded explicitly:
  `HostEditorInDialog` returns when `ticketEditor.Parent` is already the form (no second reparent, nothing
  typed is lost), and `OpenEditorDialog` returns when `_editorForm.Visible` is already true (no second
  `ShowDialog`, which would also throw). The form itself is created once and reused after `Close()`; pressing
  the Phone button repeatedly shows the trace line `editor dialog already open — reused, not opened again`
  as the proof.
