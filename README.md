# LearnWisej Samples

Runnable lab samples for the [LearnWisej](https://www.learnwisej.net/) courses, the official
e-learning platform for [Wisej.NET](https://wisej.com/). Every course folder contains one complete
Wisej.NET 4 application per module, built from that module's lesson guide, lab guide and walkthrough
video, so you can open the lab, run the finished solution next to it, click through it and compare.

![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4) ![Wisej.NET 4.1](https://img.shields.io/badge/Wisej.NET-4.1.0-0078D4) ![Samples](https://img.shields.io/badge/runnable%20samples-116-2EA44F)

## Courses

| Course | Modules | Project | Ports | What you build |
|---|:-:|---|---|---|
| [Foundations Course](./Foundations%20Course) | 10 | `WisejTrainingApp` | 5081–5090 | Your first windows, designer, navigation shell, data binding, dialogs, background work, theming, a JavaScript widget, deployment, and a mini helpdesk capstone |
| [Mastering the Control Library Course](./Mastering%20the%20Control%20Library%20Course) | 7 | `OperationsConsole` | 5701–5707 | An Operations Console that grows module by module: editors and validation, containers and layouts, trees and lists, `DataGridView` mastery, charts and documents, a custom widget |
| [Theming and Responsive Layouts Course](./Theming%20and%20Responsive%20Layouts%20Course) | 7 | `AdaptiveOps` | 5501–5507 | An Adaptive Operations Console: theme JSON, Theme Builder, CSS and states, Dock/Anchor, Flow/Table/Flex layouts, `ClientProfiles.json`, accessibility and performance |
| [Validation in Wisej.NET Course](./Validation%20in%20Wisej.NET%20Course) | 7 | `ValidationClinic` | 5901–5907 | Field events, ErrorProvider feedback, the Validation extender, custom rules, bound model errors, grid validation and a complete Save pipeline |
| [Data Binding with EF Core Course](./Data%20Binding%20with%20EF%20Core%20Course) | 7 | `SupportDesk` | 5401–5407 | A Support Desk Data Console on EF Core 10 + SQLite: DI bridge and context lifetimes, modeling and migrations, `BindingSource` grids, CRUD editors, validation, async and performance, concurrency |
| [Real-Time Server Push Course](./Real-Time%20Server%20Push%20Course) | 7 | `TicketOpsLive` | 5301–5307 | TicketOps Live: WebSocket push vs polling, session ownership, progress without refresh, update cadence, a live ticket board, one hub feeding many sessions, production review |
| [Application Integration Course](./Application%20Integration%20Course) | 10 | `IntegrationLab` | 5071–5080 | JavaScript widgets in Wisej.NET: one-off `Widget`s, reusable widget classes, custom controls with theme mixins, `Call`/`CallAsync`/`EvalAsync`, events and contracts, data endpoints, complex data widgets, a heatmap capstone |
| [From WinForms to the Web Course](./From%20WinForms%20to%20the%20Web%20Course) | 7 | `OrderDesk.Web` | 5601–5607 | Migrating **LegacyOrderDesk** (WinForms) to the web: discovery and risk mapping, project conversion, forms and modal workflow, sessions and statics, grids and performance, files and reports, secure deployment |
| [Production Architecture Deep Dive](./Production%20Architecture%20Deep%20Dive) | 12 | `TicketOps` | 5101–5112 | The TicketOps Console built the production way: project structure, startup and session state, responsive composition, data workflows, safe save pipelines, modal transactions, background sync, DI, interop, localization, security, deployment |
| [Enterprise Wisej.NET Course](./Enterprise%20Wisej.NET%20Course) | 14 | `EnterpriseOps` | 5201–5214 | The EnterpriseOps Command Center, a multi-tenant field-service system: governance, migration strategy, tenancy and concurrency, EF Core transactions, high-volume UX, background pipelines, wizards, custom components, secure interop, SSO and audit, observability, cloud release engineering, PWA/offline, AI-assisted delivery |
| [Performance & Profiling Course](./Performance%20and%20Profiling%20Course) | 7 | `WisejPerfLab` | 5801–5807 | WisejPerfLab, a support desk over 50,000 tickets, made fast with evidence: scenarios and a budget, the Visual Studio profiling workflow, the CPU hot path, session leaks and allocations, a virtual grid and a lazy tree, one query per page and a non-blocking export, then a capacity model and a health check |
| [Localization in Wisej.NET Course](./Localization%20in%20Wisej.NET%20Course) | 7 | `GlobalDesk` | 6101–6107 | GlobalDesk taken from English to five languages: resource keys and culture-formatted values, designer localization and its German variant, shared translations and composed sentences, a runtime language switch on `CultureChanged`, right-to-left and pseudo-localization, one honest translation round, then a `LocalizationService` and a QA matrix |
| [Icons & Images Course](./Icons%20and%20Images%20Course) | 7 | `IconDesk` | 6201–6207 | IconDesk, every way Wisej.NET puts a picture on a control: the four image mechanisms behind an `IImage` readout, the raster and vector pipelines, the designer image selector and theme recolouring, two official icon packs compared, embedded resources and deployment overrides, your own icon pack as a class library, then icon fonts and a capstone |

Each course folder has its own `README.md` with a module-by-module table, and each module folder has
a `README.md` that tells you what to click, maps the lab steps to the code and answers the self-check
questions.

## Getting started

You need the [.NET 10 SDK](https://dotnet.microsoft.com/download) and access to the `Wisej-4` 4.1.0
NuGet package. The Data Binding course also uses EF Core 10 and the `dotnet-ef` tool, and the Control
Library course restores `Wisej-4-ChartJS` from Module 6 on; the Performance & Profiling course uses both,
EF Core 10 with the SQLite provider and `Wisej-4-ChartJS`, in every module. Nothing is deployed anywhere: every sample
runs on `localhost`.

Every module folder contains a `.slnx` solution and the project folder. Open the solution in Visual
Studio and press F5, or run it from the command line. The projects multi-target `net10.0-windows` and
`net10.0`, so `dotnet run` needs a framework:

```bash
cd "Foundations Course/Module 4/WisejTrainingApp"
dotnet run -f net10.0 --urls http://localhost:5084
```

Then open the printed URL in a browser. Each course README lists the port assigned to every module,
so several modules can run side by side.

Modules with unit tests (the Data Binding course and Validation capstone) run them with `dotnet test` from the module folder.

## How a sample is laid out

All 116 samples follow the same frame so that a learner who has finished one module knows where to
look in the next:

- **The lab screen** on the left, built with the control names the lab guide uses.
- **A live trace card** on the right that logs what the server did for every click: the event log, the
  server ⇄ client message trace, the EF Core lifetime and SQL trace, or the layer-by-layer activity trace,
  depending on the course.
- **A button bar** that exercises the success path, a progress path, at least one failure path and the
  recovery, so every anti-pattern the lesson warns about can be reproduced and then fixed on screen.
- **A `docs/` folder** with the lab deliverables written as Markdown: architecture notes, ADRs,
  checklists, migration logs, demo scripts.

Courses that build one application across modules (Control Library, Data Binding, Real-Time Push,
Theming, Validation, WinForms to the Web, Production Architecture, Enterprise, Performance & Profiling) are cumulative: each
`Module N` folder is the complete application as it stands at the end of module N, and the last module
is the finished capstone.

## The `_template` folders and the cookbooks

Every course has a `_template` folder holding the scaffold each module was built from, plus a
`COOKBOOK.md`. The cookbooks record the Wisej.NET conventions and API facts that were verified while
building and running the samples, together with the gotchas found along the way: how dialogs are
awaited, how background tasks push updates, what the theme JSON really looks like, where `StartPolling`
belongs, why `ComboBox.DisplayMember` needs properties, and so on. Read the cookbook of a course before
writing a new sample for it.

## Stand-ins instead of third-party packages

The courses ship no commercial or third-party libraries, so the samples carry small self-contained
stand-ins where a real product would go: JavaScript "vendor" gauges, knobs, charts, grids and heatmaps
for the Application Integration course, a minimal `.xlsx` writer and a single-page PDF writer for the
migration course, an in-memory order store seeded with the orders shown in the videos. The
[`orders.csv`](./orders.csv) at the repository root is the file used for the CSV upload labs. Swap in a
real library and only the adapter or service class changes.

## Repository map

```
LearnWisej-Samples/
├── <Course name>/
│   ├── README.md          course overview, module table, ports
│   ├── _template/         scaffold + COOKBOOK.md
│   └── Module N/
│       ├── README.md      what to click, lab steps → code, self-check answers
│       ├── <Project>.slnx
│       └── <Project>/     the runnable Wisej.NET application (+ docs/)
└── orders.csv             sample data for the upload labs
```

## Learn more

- [LearnWisej](https://www.learnwisej.net/), the courses these samples belong to
- [Wisej.NET documentation](https://wisej.com/documentation)
- [Wisej.NET training and support](https://docs.wisej.com/license/services/training)
