# TicketOps · Production Architecture Deep Dive · Module 10

Local lab build for **Module 10 · Theming, Resources, Localization & UI Modernization**. It follows the
walkthrough video *Theme & localize the dashboard*: the **Operations Dashboard** gets a light/dark theme
switch, a reusable `StatusChip` UserControl, captions in `Strings.resx` + `Strings.de.resx`, two cultures
(**en-US**, **de-DE**) switched at runtime, and culture-aware dates and currency
(`6/14/2026 · $1,850.00` ⇄ `14.06.2026 · 1.850,00 €`).

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Production Architecture Deep Dive\Module 10\TicketOps"
dotnet run -f net10.0 --urls http://localhost:5110
```

Then open <http://localhost:5110> (or open `TicketOps.slnx` in Visual Studio and press F5).

## The screens

- **Operations Dashboard**: Theme and Culture pickers, four KPI cards (one `StatusChip` each), the
  work-order grid (Due and Cost formatted for the culture), a detail strip for the selected order,
  **Open detail…** and **Next status**, and a status line. Picking *Italiano* shows the "not shipped"
  message; **Next status** on a finished order shows the domain rule.
- **Work order detail** (dialog): the same chip, captions and formatting, and **Next status** / **Close**.

## Deliverables (lab guide)

| # | Deliverable | Where |
|---|---|---|
| 1 | Theme switch UI | Theme picker in `Views/OperationsDashboard`, `Infrastructure/ThemeSwitcher.cs` — [`docs/ThemeSwitch.md`](TicketOps/docs/ThemeSwitch.md) |
| 2 | Status chip UserControl | `Controls/StatusChip.cs`, its appearance in `Themes/TicketOps.mixin.theme` — [`docs/StatusChip.md`](TicketOps/docs/StatusChip.md) |
| 3 | Resource files and localization notes | `Resources/Strings.resx`, `Resources/Strings.de.resx`, `Resources/Strings.cs`, `Services/LocalizationService.cs` — [`docs/LocalizationNotes.md`](TicketOps/docs/LocalizationNotes.md) |
| 4 | Modernization checklist applied to two screens | `Views/OperationsDashboard` and `Views/WorkOrderDetail` — [`docs/ModernizationChecklist.md`](TicketOps/docs/ModernizationChecklist.md) |
| — | Production-readiness note | [`docs/ProductionReadinessNote.md`](TicketOps/docs/ProductionReadinessNote.md) |

The video's picker lists `Classic-1` and `Material-3`; this build switches between `Bootstrap-4` and
`BootstrapDark-4`, the light/dark pair the lab asks for.

## Where things live

```
TicketOps/
├─ Views/                    OperationsDashboard, WorkOrderDetail
├─ Controls/                 StatusChip, StatusBanner
├─ Themes/TicketOps.mixin.theme   the "chip" appearance and chip-* colours
├─ Resources/                Strings.resx, Strings.de.resx, Strings.cs
├─ Services/                 SessionContext, LocalizationService, WorkOrderService
├─ Domain/                   WorkOrder (CanAdvance), DashboardSnapshot, OperationResult
├─ Data/                     IWorkOrderRepository + in-memory implementation
├─ Infrastructure/           ThemeSwitcher, ILog/ActivityLog, AppComposition
└─ docs/                     the deliverables
```
