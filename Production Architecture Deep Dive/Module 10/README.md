# TicketOps · Production Architecture Deep Dive · Module 10

Local lab build for **Module 10 · Theming, Resources, Localization & UI Modernization**. It follows the
walkthrough video *Theme & localize the dashboard*: the TicketOps **Operations Dashboard** gets a
light/dark theme switch (`Bootstrap-4` / `BootstrapDark-4`) that re-skins every open window in one line,
a reusable `StatusChip` UserControl that is *set, not styled* (its colours are theme states in a mixin),
every caption in `Resources/Strings.resx` + `Strings.de.resx` behind a `ResourceManager`, two cultures
(**en-US**, **de-DE**) switched at runtime with `Application.CurrentCulture`, culture-aware dates,
numbers and currency (`6/14/2026 · $1,850.00` ⇄ `14.06.2026 · 1.850,00 €`) and the modernization
checklist applied to two screens — the dashboard and the `WorkOrderDetail` dialog. Every path (success,
progress, a domain rule, resource gaps, the unsupported culture, a data outage and its recovery) is
visible to the operator in the operator's language without leaking internals.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Production Architecture Deep Dive\Module 10\TicketOps"
dotnet run -f net10.0 --urls http://localhost:5110
```

Then open <http://localhost:5110>. (Visual Studio: open `TicketOps.slnx`, press F5 — the port is in
`Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.
`dotnet build -nologo -v q` passes with no warnings for both targets (`net10.0-windows`, `net10.0`);
the German satellite assembly lands in `bin/…/de/TicketOps.resources.dll` and `Themes/` is copied next to it.

## What to click in the Operations Dashboard

The left card is the themed, localized dashboard; the right card is the **Activity trace · UI → Service →
Data · theme & culture**: every click is logged as it crosses a boundary (`[UI]` → `[SVC]` → `[DATA]` /
`[DOMAIN]` → `[UI]`), and the theme/culture switches add what the session (`[SESSION]`) and the framework
(`[INFRA]`) did — so you can see that no handler chose a colour, a pattern or a sentence.

| Control | Path | What you should see |
|---|---|---|
| **Theme → Dark · BootstrapDark-4** (header picker) | success | `[UI] comboTheme_SelectedIndexChanged → ThemeSwitcher.Apply("BootstrapDark-4", sessionOnly: False)`, `[INFRA] ThemeSwitcher.Apply — Application.LoadTheme("BootstrapDark-4") · global …`, `[SESSION] ThemeSwitcher.Remember — SessionContext.Theme = … · Application.Session.TicketOpsTheme = …`, `[SESSION] Application.ThemeChanged → "BootstrapDark-4"`, `[UI] … 5 StatusChips … re-skinned by the theme — no chip code ran`; the whole window turns dark — cards, grid, buttons, trace and chips; footer **Theme: BootstrapDark-4 — applied to every screen.** |
| **This session only** ☑ then pick a theme | success (per session) | `[INFRA] ThemeSwitcher.MergeMixin — TicketOps.mixin.theme: 5 colour/appearance entries merged …`, `[INFRA] … Application.Theme = new ClientTheme("Bootstrap-4", embedded JSON + TicketOps.mixin.theme) · this session only`; a second browser tab keeps its own theme (unchecked, `LoadTheme` re-skins every tab) |
| **Culture → Deutsch (Deutschland)** | success | `[SVC] LocalizationService.SetCulture — de-DE accepted · SessionContext.Culture = de-DE · thread CurrentCulture/CurrentUICulture set`, `[SESSION] Application.CurrentCulture — = de-DE · request thread UI culture now de-DE`, `[SESSION] Application.CultureChanged → de-DE`, `[UI] ApplyLocalizedText — captions resolved for de-DE …`, `[UI] Bind — 10 rows · 4 KPI chips · formatted with de-DE: #2002 due 14.06.2026 · 1.850,00 €`; every caption is German (Betriebs-Übersicht, Offen / In Bearbeitung / Blockiert / Erledigt, Fällig am, Kosten …); footer **Sprache: Deutsch (Deutschland) — Beschriftungen, Datum und Währung folgen der Kultur.** |
| **Culture → Italiano (Italia)** | failure 1 (validation) | `[SVC] ⚠ LocalizationService.SetCulture — rejected: 'it-IT' is not shipped (supported: en-US, de-DE) → staying on de-DE`; orange banner **Die Sprache „it-IT“ ist in diesem Build nicht enthalten — es bleibt bei de-DE.**; the picker snaps back; status **● abgelehnt** |
| **Open detail…** (select a row first) | second screen | `[UI] buttonOpenDetail_Click → IWorkOrderService.FindAsync(#2002) → WorkOrderDetail dialog …`, `[UI] WorkOrderDetail.Load — #2002 shown · formatted for de-DE: 14.06.2026 · 1.850,00 € · theme "BootstrapDark-4" (no code of this dialog knows either)`; the dialog is painted in the current theme with the same chip and German captions; **Nächster Status** works there too |
| **Next status** | success + domain | `[SVC] WorkOrderService.AdvanceStatusAsync — #2002 → IWorkOrderRepository.FindAsync`, `[DOMAIN] WorkOrder.Advance — #2002 Open → InProgress`, `[DATA] … #2002 written · status InProgress`, `[UI] OK · Work order 2002 is now "In progress".`; the detail chip re-colours (amber) and the KPI counts move |
| **↻ Refresh dashboard** | success | `[SVC] GetDashboardAsync → IWorkOrderRepository.GetAllAsync()`, `[DATA] 10 rows`, `[SVC] 10 work orders · open 4 · in progress 3 · blocked 1 · done 2`, `[UI] Bind …`; footer **Dashboard refreshed.** |
| **▶ Walk statuses** | progress | a `Timer` advances the selected order one status per 800 ms through the same `AdvanceStatusAsync`; the thin progress bar and **● walking statuses n/3** advance; one `[SVC]`+`[DOMAIN]`+`[DATA]` group per tick; the detail chip goes blue → amber → red → green; ends with `walk complete: #2002 is Done` |
| **Advance a finished order** | failure 2 (domain rule) | `[DOMAIN] ⚠ WorkOrder.CanAdvance — #2006 rejected: Rule.DoneCannotAdvance (resolved for de-DE)`; banner **Ein erledigter Arbeitsauftrag kann nicht weitergeschaltet werden.** — the handler never knew the rule and never knew the language |
| **Resource gaps** (best in German) | failure 3 (resources) | `[SVC] ⚠ LocalizationService.Text — missing: key 'Status.Archived' has no value in de-DE nor in the neutral resources → fallback "[Status.Archived]"`, `[SVC] ⚠ … untranslated: key 'Button.Export' missing in de-DE → neutral (English) value "Export"`; banner **Ressourcenlücken sicher angezeigt … → "[Status.Archived]" · "Export"**; nothing throws, the screen stays readable |
| **Simulate data outage** | error path | `[DATA] ✖ outage: SELECT * FROM WorkOrders failed — timeout connecting to sql01:1433 …` stays in the trace; `[UI] ✖ … caught DataOutageException — user sees Strings.ActionFailed resolved for thread UI culture de-DE`; the operator sees only the red banner **Die Aktion konnte nicht abgeschlossen werden. Details stehen im Protokoll.** and a toast; status **● fehlgeschlagen** |
| **Recover the data store** (same button) | recovery | the repository answers again, the dashboard reloads, status **● bereit** |
| **Clear trace** | — | empties the right-hand card |

## Deliverables (lab guide)

| # | Deliverable | Where |
|---|---|---|
| 1 | Theme switch UI | header picker + **This session only** in `Views/OperationsDashboard`; `Infrastructure/ThemeSwitcher.cs` (`Application.LoadTheme` / `Application.Theme`), `Services/SessionContext.cs` — see [`docs/ThemeSwitch.md`](TicketOps/docs/ThemeSwitch.md) |
| 2 | Status chip UserControl | `Controls/StatusChip.cs` + `.Designer.cs`, its appearance in `Themes/TicketOps.mixin.theme` — see [`docs/StatusChip.md`](TicketOps/docs/StatusChip.md) |
| 3 | Resource files & localization notes | `Resources/Strings.resx`, `Resources/Strings.de.resx`, `Resources/Strings.cs` (typed accessor), `Services/LocalizationService.cs` — see [`docs/LocalizationNotes.md`](TicketOps/docs/LocalizationNotes.md) |
| 4 | UI modernization checklist applied to two screens | `Views/OperationsDashboard` and `Views/WorkOrderDetail` — see [`docs/ModernizationChecklist.md`](TicketOps/docs/ModernizationChecklist.md) |
| 5 | Every path visible without leaking internals | `OperationsDashboard.ShowResult` / `ReportFailure`, `LocalizationService.Text` fallbacks, the trace panel |
| 6 | Production-readiness note | [`docs/ProductionReadinessNote.md`](TicketOps/docs/ProductionReadinessNote.md) |

## Where things live

```
TicketOps/
├─ Views/
│  ├─ OperationsDashboard.cs        the screen: theme/culture pickers, KPI cards, grid, detail strip; ApplyLocalizedText / Bind / ShowResult / ReportFailure
│  ├─ OperationsDashboard.Designer.cs  GENERATED-style layout — no BackColor anywhere: the theme paints every surface
│  ├─ WorkOrderDetail.cs            the second screen (dialog): same chip, same resources, same culture formatting
│  └─ WorkOrderDetail.Designer.cs
├─ Controls/
│  ├─ StatusChip                    the reusable status pill: AppearanceKey "chip" + theme state per status; Text from resources
│  └─ StatusBanner                  the template's "● state" + banner (documented theme exception)
├─ Themes/TicketOps.mixin.theme     the "chip" appearance + chip-* colour aliases (primary / danger / success …), merged into the active theme
├─ Resources/
│  ├─ Strings.resx                  every user-visible string, neutral (English), keyed Area.Name
│  ├─ Strings.de.resx               the German values (Button.Export left out on purpose → traced fallback)
│  └─ Strings.cs                    typed accessor over the ResourceManager (Strings.ActionFailed, Strings.Get(key, culture))
├─ Services/
│  ├─ SessionContext.cs             this operator's theme + culture, one per session (never static)
│  ├─ ILocalizationService.cs / LocalizationService.cs   culture, Text(key) with traced fallbacks, FormatDate/Currency/Number — plain .NET
│  └─ IWorkOrderService.cs / WorkOrderService.cs         dashboard snapshot + AdvanceStatus (domain rule → localized result)
├─ Domain/
│  ├─ WorkOrder.cs                  record + rule (CanAdvance → resource KEY, Advance); WorkOrderStatus; no culture, no UI
│  ├─ DashboardSnapshot.cs          orders + counts per status
│  └─ OperationResult.cs            success / safe (already localized) explanation
├─ Data/
│  ├─ IWorkOrderRepository.cs       persistence contract
│  └─ InMemoryWorkOrderRepository.cs fake store seeded with the video's work orders; SimulateOutage throws like a driver
├─ Infrastructure/
│  ├─ ThemeSwitcher.cs              the only file that names a theme; LoadTheme (global) vs Application.Theme (session)
│  ├─ ILog.cs / ActivityLog.cs      cross-cutting logging (details stay here)
│  └─ AppComposition.cs             who gets what: one object graph per session, constructor injection, no statics
├─ Diagnostics/ActivityTracePanel   the live trace card (white BackColor removed: it follows the theme now)
├─ docs/                            the deliverables
├─ Program.cs                       Wisej.NET session entry point → AppComposition
└─ Startup.cs                       Kestrel host (app.UseWisej())
```

## Self-check answers (lesson guide)

- **Which styles are theme-level and which are genuine screen-level exceptions?**
  Theme-level: every surface colour, font, border and radius (nothing in `Views/` sets a colour), and
  the chip palette (`chip-open` → `primary`, `chip-blocked` → `danger`, `chip-done` → `success`, one
  amber literal) in `Themes/TicketOps.mixin.theme`. Screen-level exceptions, documented in
  [`docs/ModernizationChecklist.md`](TicketOps/docs/ModernizationChecklist.md): the shared `StatusBanner`'s
  five semantic state colours and the grey footer of the trace panel. No `CssStyle` anywhere.
- **Can you switch the whole app's look in one line without touching a handler?**
  Yes: `Application.LoadTheme("BootstrapDark-4")` inside `ThemeSwitcher.Apply`. The dashboard's handler
  only names a theme; the grid, cards, buttons, trace, chips and the `WorkOrderDetail` dialog re-render
  because none of them carried a colour of their own. The trace line *no chip code ran* is the proof.
- **Is every user-visible string and image a resource?**
  Every caption, column header, chip text, state, message and rule reason is a key in `Strings.resx`
  (46 keys) with a German value in `Strings.de.resx`; `Strings.cs` is the typed accessor and
  `ILocalizationService.Text(key)` the traced one. The console uses text glyphs from the theme font and
  no bitmap images; the note explains where an `ImageSource = "resource.wx/…"` icon would go.
- **What changes in German text, and does the layout survive it?**
  *In Bearbeitung* (vs *In progress*), *Übersicht aktualisieren* (vs *Refresh dashboard*),
  *Fällig am*, *Arbeitskosten*, plus `14.06.2026` and `1.850,00 €` from the same values. Checked in
  the real layout in both themes: the 132 px chip, the 190 px button and the 184 px checkbox absorb the
  widest strings without clipping.
- **Can a future redesign avoid editing every screen?**
  Yes. A rebrand edits the theme (or the four `chip-*` aliases); a new language adds `Strings.xx.resx`
  and one entry in `LocalizationService.SupportedCultureNames`; neither touches `Views/`. The per-session
  `SessionContext` means two operators can already run two themes (session-only mode) and two languages
  at once.
