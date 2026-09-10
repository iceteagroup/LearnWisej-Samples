# Production Architecture Deep Dive · lab samples

One runnable Wisej.NET 4 application per module, built from the course's lesson guide, applied-concepts
guide, lab guide and walkthrough video. The app under study is the course's **TicketOps Console**.
Every sample follows the same layout: the module's screen on the left, an **Activity trace ·
UI → Service → Data** on the right (every click logged as it crosses a boundary), and a bottom button
bar that exercises the success path, a progress path, at least one failure path and the error path
with its recovery. Each folder has its own `README.md` (what to click, deliverables, self-check answers)
and a `docs/` folder with the lab deliverables.

Requirements already on this machine: .NET 10 SDK and the `Wisej-4` 4.1.0 NuGet package. Nothing is deployed anywhere.

| Module | Folder | What it builds | Run |
|---|---|---|---|
| 1 · Production Architecture & Project Structure | `Module 1` | the junior app refactored into `Views/Controls/Services/Domain/Data/Infrastructure/Resources/Diagnostics`, `ITicketService` + fake `TicketService`, thin handlers, every path visible, architecture note | `dotnet run -f net10.0 --urls http://localhost:5101` |
| 2 · Startup, Configuration, Session State & Lifetime | `Module 2` | session-scoped `SessionContext`, diagnostics page (global vs per-session values), static-state audit | `http://localhost:5102` |
| 3 · Responsive Layouts, Client Profiles & Composition | `Module 3` | responsive `TicketWorkspace` UserControl (desktop / tablet / phone), `ClientProfiles.json`, reusable `SearchBar` | `http://localhost:5103` |
| 4 · Data Binding, DataGridView & Data Workflows | `Module 4` | Work Order grid: observable model, `BindingSource`, search/filter, master-detail editor, dirty tracking, formatted columns | `http://localhost:5104` |
| 5 · Validation, Error UX & Safe Save Pipelines | `Module 5` | validation rules, `SaveCommand`, field errors + summary panel, safe save pipeline, test cases | `http://localhost:5105` |
| 6 · Modal Workflows, Dialog Results & Transactional UI | `Module 6` | `ApprovalDialog` with a typed `ApprovalDialogResult`, `ApprovalService`, test cases, workflow diagram | `http://localhost:5106` |
| 7 · Background Tasks, Real-Time Updates & Sync | `Module 7` | background CSV import (`Application.StartTask` + `Application.Update`), progress + log panel, cancellation, per-row errors | `http://localhost:5107` |
| 8 · Services, DI & Testable UI | `Module 8` | `Application.Services` registration (fake + production profiles), five injected services, presenter, lifetime table | `http://localhost:5108` |
| 9 · JavaScript Integration & Widget Interop | `Module 9` | Ctrl+K focuses the global search, server-confirmed clipboard copy, security note per interop point | `http://localhost:5109` |
| 10 · Theming, Resources, Localization & Modernization | `Module 10` | light/dark theme switch, `StatusChip` UserControl, two cultures, culture-aware formatting, modernization checklist | `http://localhost:5110` |
| 11 · Security, Authentication & Safe Server Boundaries | `Module 11` | login gate, permission service, service-level authorization, safe HTML policy, audit log, security checklist | `http://localhost:5111` |
| 12 · Deployment, Diagnostics, Load Balancing & Capstone | `Module 12` | `HealthCheck.json`, diagnostics page, deployment checklist, release/rollback notes, demo script | `http://localhost:5112` |

Run any module from its `TicketOps` project folder. The projects multi-target `net10.0-windows` and
`net10.0`, so `dotnet run` needs a framework (`-f net10.0`, or `-f net10.0-windows`), e.g.

```bash
cd "D:/Projects/LearnWisej-Samples/Production Architecture Deep Dive/Module 4/TicketOps"
dotnet run -f net10.0 --urls http://localhost:5104
```

or open the `TicketOps.slnx` in the module folder with Visual Studio and press F5.

## `_template`

The scaffold every module was built from (the production folder structure with the shared
`ILog`/`ActivityLog`, the `ActivityTracePanel` diagnostics card, the `StatusBanner` control, `Strings`
and the per-session `AppComposition`), plus `COOKBOOK.md`: the conventions verified while building and
running these samples (handler shape, results vs exceptions, the trace format, the Wisej.NET APIs each
module relies on and which of them were verified at runtime). Read it before writing a new sample.
