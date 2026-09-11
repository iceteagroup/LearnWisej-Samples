# Folder structure — TicketOps Console

*Module 1 deliverable*

```
TicketOps/
  Views/           pages, forms, screens          TicketEditor
  Controls/        reusable UserControls          StatusBanner
  Services/        ITicketService + workflows     ITicketService, TicketService
  Domain/          Ticket and business rules      Ticket, TicketDraft, OperationResult
  Data/            repositories, persistence      ITicketRepository, InMemoryTicketRepository, SeedData
  Infrastructure/  config, wiring, contracts      ILog, AppComposition
  Resources/       images, themes, strings        Strings
  Diagnostics/     logging & error tracking       ActivityLog
  docs/            lab deliverables
  Program.cs       session entry point → AppComposition.CreateMainView().Show()
  Startup.cs       Kestrel host, app.UseWisej()
```

## Folders, namespaces and what they may reference

| Folder | Namespace | May reference | Must NOT reference |
|---|---|---|---|
| `Views/`, `Controls/`, `Diagnostics/` | `TicketOps.Views` … | Services, Domain, Infrastructure, Resources, Wisej.Web | Data directly |
| `Services/` | `TicketOps.Services` | Domain, Data (interfaces), Infrastructure (`ILog`) | Wisej.Web, any control |
| `Domain/` | `TicketOps.Domain` | nothing but the BCL | Wisej.Web, Services, Data |
| `Data/` | `TicketOps.Data` | Domain, Infrastructure (`ILog`) | Wisej.Web, Services, Views |
| `Infrastructure/` | `TicketOps.Infrastructure` | everything (it composes the graph) | — |

No View references a Data type: `TicketEditor` takes `ITicketService` and `ILog` alone, and only
`AppComposition` knows that the service sits on `InMemoryTicketRepository`.

## Natural first assembly split

Pull `Domain/` and `Services/` (plus the `ITicketRepository` interface) into `TicketOps.Core`, a class
library with no Wisej.NET reference. The compiler then enforces the direction of the arrows.
