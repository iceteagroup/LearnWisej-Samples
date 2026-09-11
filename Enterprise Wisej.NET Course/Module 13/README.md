# EnterpriseOps · Enterprise Wisej.NET: Architecture to Cloud · Module 13

Local lab build for **Advanced Module 13 — Hybrid, PWA, Offline & Device-Aware Applications**. It follows
the walkthrough video: the EnterpriseOps Command Center grows a **Field Technician mode**
(`UI.FieldTechnicianPage`) with a device-aware layout, an offline work-order cache in a local store, a
local completion queue of `OfflineCommand`s with an explicit `SyncState`, a reconnect sync simulation that
replays the queue through the *same* server service the online screen uses, and a `SyncConflictPanel` that
shows both versions when the dispatcher changed a work order while the technician was offline.

Everything is in-memory. Nothing is deployed, no database, no network, no cloud account, no device.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Enterprise Wisej.NET Course\Module 13\EnterpriseOps"
dotnet run -f net10.0 --urls http://localhost:5213
```

Then open <http://localhost:5213>. (Visual Studio: open `EnterpriseOps.slnx`, press F5 — the port is in
`Properties/launchSettings.json`.) Requirements: .NET 10 SDK and the `Wisej-4` 4.1.0 NuGet package.

## The screen

```
EnterpriseOps — Field Technician mode
┌ device frame (380 / 620 / 860 px, from the detected device) ───────────┐
│ Field mode                                  [Go offline] [ ONLINE ]     │
│ WORK ORDERS — LOCAL CACHE (SQLITE) · 12 ROWS · DOWNLOADED 09:14         │
│ │ WO-1037 │ Valve yard — replace relief valve │ … │ Assigned │ … │      │
│ │ WO-1038 │ Substation B — inspect breaker    │ … │ Assigned │ … │      │
│ COMPLETION NOTES  [ seal replaced, torqued to spec ]                    │
│ [ Complete… ] [ Scan ]  ✓ ASSET-Pump-88121 matches …                    │
│ COMPLETION QUEUE — 2 PENDING                                            │
│ │ WO-1038 │ Completed 11:48 — breaker OK            │ PendingSync │     │
│ ┌ Sync conflict — WO-1037 (appears on conflict) ┐                       │
│ ▓▓▓▓░░  Offline — 2 completion(s) queued locally · cache: 12 work orders│
└─────────────────────────────────────────────────────────────────────────┘
```

The frame width, touch-target height and the `Site` / `v` columns follow `Application.Browser`
(phone 380 px / 44 px, tablet 620 px / 40 px, desktop 860 px / 34 px). To see the phone layout, open the
app on a phone or in the browser's device emulation and reload.

## What to click

| Control | Path | What you should see |
|---|---|---|
| **Complete…** *(online)* | success | Green toast "WO-1041 completed (v3)."; the grid's **Server status** turns `Completed`; nothing is queued. |
| **Complete…** *(offline)* | queued | Toast "No signal — WO-1038 is queued locally…". **Server status** still says `Assigned`, **On this device** says `Completed · pending sync`, and an amber `PendingSync` card appears in the queue. |
| **Complete…** *(empty notes)* | validation | Rejected before anything is queued: "Enter what you did before completing the work order." |
| **Scan** | device capability | Online, the scanned tag is validated on the server (the simulated tags rotate: one matches the site, others are refused). Offline, it is recorded for validation at sync. The tag is appended to the notes. |
| **Go offline / Go online** | reconnect sync | The pill flips `ONLINE` ⇄ `OFFLINE`. Going online with a non-empty queue replays it on a background task (`Application.StartTask` + `Application.Update`): one command per 700 ms, the progress bar advances, cards turn green `Synced`. While the device is offline the dispatcher (`ana.ops`) cancels WO-1037 on the server, so a completion of WO-1037 queued offline comes back as a **conflict**. |
| **Keep server — attach my notes** *(conflict panel)* | resolution | WO-1037 stays `Cancelled`, the technician's notes are attached and audited, the card turns grey `Rejected · kept server — notes preserved`, and any commands behind it resume. |
| **Apply my completion…** *(conflict panel)* | failure: product rule | `ben.tech` is a Technician: the server refuses with the `workorder.override` rule, audits the refusal, and the panel stays open. |

### The demo (reproduces the video)

1. **Complete…** on `WO-1041` while online (success).
2. **Go offline**.
3. Select `WO-1038`, type notes, **Complete…** (queued).
4. Select `WO-1037`, type `relief valve replaced, torqued to spec`, **Complete…** (queued).
5. **Go online**: `WO-1038` → `Synced`, then `WO-1037` → **Conflict**, both versions on screen.
6. **Apply my completion…**: refused, audited, panel stays open.
7. **Keep server — attach my notes**: resolved, nothing lost.

Service, queue and sync decisions are written to the server log (`System.Diagnostics.Trace`, e.g. the
Visual Studio Output window) with a layer prefix: `Service:`, `Data:`, `Security:`, `Device:`, `Job:`.

## Lab steps → where in the code

| Lab step / deliverable | Where |
|---|---|
| Open the project and run it once | `EnterpriseOps.slnx`, `Program.Main` → `Application.MainPage = new UI.FieldTechnicianPage()` |
| Design and prototype Field Technician mode | `UI/FieldTechnicianPage.cs` + `.Designer.cs` |
| Device-aware layout | `Hybrid/DeviceServices.cs` (`DeviceProfile`, `DeviceInfo`, `BrowserDeviceServices.Describe`) → `FieldTechnicianPage.ApplyDeviceLayout()` |
| Offline work-order cache | `Hybrid/LocalStore.cs` (`CachedWorkOrder`, `LocalStore`), filled by `WorkOrderService.GetAssignedAsync` → `FakeWorkOrderRepository.FindAssigned` |
| Local completion queue | `Hybrid/HybridOfflinePatterns.cs` (`OfflineCommand`, `SyncState`, `IOfflineCommandQueue`) + `Hybrid/OfflineCommandQueue.cs` |
| Reconnect sync simulation | `btnToggleConnection_Click` → `FieldTechnicianPage.StartSync()` → `Hybrid/SyncWorkflow.cs` |
| Conflict display | `UI/SyncConflictPanel.cs` + `.Designer.cs`, fed by `SyncWorkflow.SyncConflict` |
| **Deliverable 1** — Hybrid/offline architecture note | [`docs/HybridOfflineArchitectureNote.md`](EnterpriseOps/docs/HybridOfflineArchitectureNote.md) + [`hybrid-offline-architecture.svg`](EnterpriseOps/docs/hybrid-offline-architecture.svg) |
| **Deliverable 2** — Device service abstraction | [`docs/DeviceServiceAbstraction.md`](EnterpriseOps/docs/DeviceServiceAbstraction.md) |
| **Deliverable 3** — Offline queue model | [`docs/OfflineQueueModel.md`](EnterpriseOps/docs/OfflineQueueModel.md) + [`sync-state-machine.svg`](EnterpriseOps/docs/sync-state-machine.svg) |
| **Deliverable 4** — Sync conflict screen | [`docs/SyncConflictScreen.md`](EnterpriseOps/docs/SyncConflictScreen.md) |
| **Deliverable 5** — Field-mode usability checklist | [`docs/FieldModeUsabilityChecklist.md`](EnterpriseOps/docs/FieldModeUsabilityChecklist.md) |
| PWA shell (written, deliberately **not wired in**) | [`docs/PwaShellNote.md`](EnterpriseOps/docs/PwaShellNote.md), [`docs/pwa/`](EnterpriseOps/docs/pwa/) |
| Show every path | success · validation · queued offline · conflict · override refused; every handler has a `try/catch` that shows a message with a reference id |
| Production-readiness note | the "Security consequences" and "Evidence" sections of the architecture note |

## Where things live

```
EnterpriseOps/
├─ UI/          FieldTechnicianPage · SyncConflictPanel · OfflineCommandRow (one queue card)
├─ Hybrid/      HybridOfflinePatterns · DeviceServices · LocalStore · OfflineCommandQueue · SyncWorkflow
├─ Services/    WorkOrderService (the only code that changes a work order) · Commands · SessionContext · ActivityTrace (server log)
├─ Security/    PermissionService (server grants + CachedPermissionSet) · AuditLog (device time + server time)
├─ Domain/      WorkOrder (Version = concurrency token)
├─ Data/        FakeWorkOrderRepository · SeedData (48 work orders, 12 in ben.tech's field cache)
└─ docs/        the five deliverables (+ 2 SVGs, + the PWA shell)
```

## Student review questions

**What data is allowed offline?** Only the technician's own still-open work orders for one tenant —
twelve rows, decided on the server by `FakeWorkOrderRepository.FindAssigned(tenantId, user)`. The device
stores a projection (`CachedWorkOrder`), never the entity, plus the command queue and a permission
snapshot. Nothing else.

**How are permissions refreshed?** The device's `CachedPermissionSet` is replaced, never merged, on every
reconnect: `SyncWorkflow.Replay` refreshes it before the first command replays. The real check is
server-side in `WorkOrderService.Complete`, once per queued command, against `PermissionService`'s current
answer, so a revoked user's queued work comes back `Rejected` and audited instead of applied.

**What happens when local changes conflict with server changes?** The command's `BaseVersion` no longer
matches the row's `Version`, so `WorkOrderService.Complete` returns `CommandResult.Conflict` with the
server snapshot. The replay stops (later commands stay `PendingSync`) and `SyncConflictPanel` shows both
versions. **Keep server — attach my notes** preserves the field notes; **Apply my completion…** needs
`workorder.override` and is refused for a Technician. Every outcome is an explicit state and audited.

## Instructor acceptance criteria

| Criterion | How this sample satisfies it |
|---|---|
| Follows the course architecture baseline | Folder-per-layer namespaces, typed commands and results, per-session services created in the page constructor, no static user state. |
| UI event handlers remain thin | Each handler reads the selection, builds a typed command, calls a service and shows the result; `try/catch` with one `Fail()` reporter. |
| Service logic reviewable without the designer | The offline model is in `Hybrid/*.cs` and `Services/WorkOrderService.cs`; `SyncConflictPanel` only raises events. |
| At least one failure path is demonstrated | Empty-notes validation, the sync conflict, and the refused override. |
| State ownership, security, production behaviour | The architecture note's state-ownership and security tables. |

## Notes

* `Application.Update(this, …)` (two-argument form) pushes the replay steps from `Application.StartTask`.
* The video's queue control is a `DataRepeater` named `repQueue`; this sample renders the same cards with
  `OfflineCommandRow` user controls in a `FlowLayoutPanel` (`flpQueue`). Swapping in a `DataRepeater`
  changes only `RenderQueue()`.
* The video's file tree shows seven projects; this course keeps one project with folder-per-layer
  namespaces so the sample runs with a single `dotnet run`.
* `LocalStore.cs` is the in-memory stand-in for the video's `LocalStore.Sqlite.cs`.
