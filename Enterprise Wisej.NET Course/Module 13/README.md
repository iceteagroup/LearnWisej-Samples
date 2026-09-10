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
`Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK and the `Wisej-4` 4.1.0 NuGet package.
`dotnet build -nologo -v q` passes with **0 warnings, 0 errors** on both `net10.0` and `net10.0-windows`.

## The screen

```
┌ EnterpriseOps — Field Technician mode ───────── contoso · ben.tech (Technician) · session ab12cd34 ┐
│ ┌ device frame (380 / 620 / 860 px) ───────────────────┐  ┌ Server · live activity trace ───────┐ │
│ │ Field mode · Desktop                       [ONLINE]  │  │ 09:14:22.118  Device: device dete…  │ │
│ │ WORK ORDERS — LOCAL CACHE (SQLITE) · 12 ROWS · 09:14 │  │ 09:14:22.140  Security: permission… │ │
│ │ ┌──────────────────────────────────────────────────┐ │  │ 09:14:22.161  Data: repository.Fin… │ │
│ │ │ WO-1037 │ Valve yard — replace relief valve │ …  │ │  │ 09:14:22.163  Device: local store:… │ │
│ │ │ WO-1038 │ Substation B — inspect breaker    │ …  │ │  │ …                                   │ │
│ │ │ WO-1041 │ Pump station 7 — replace seal     │ …  │ │  │                                     │ │
│ │ └──────────────────────────────────────────────────┘ │  │                                     │ │
│ │ COMPLETION NOTES  [ seal replaced, torqued to spec ] │  │                                     │ │
│ │ [ Complete… ] [ Scan ]  ✓ ASSET-Pump-88121 matches…  │  │                                     │ │
│ │ COMPLETION QUEUE — 2 PENDING OF 3 · 1 942 BYTES      │  │                                     │ │
│ │ │ WO-1041 │ Completed 11:04 — seal replaced │Synced│ │  │                                     │ │
│ │ │ WO-1038 │ Completed 11:48 — breaker OK  │Pending│ │  │                                     │ │
│ │ ┌ Sync conflict — WO-1037 (appears on conflict) ───┐ │  │                                     │ │
│ │ └──────────────────────────────────────────────────┘ │  │                                     │ │
│ │ ▓▓▓▓▓░░░  Connection restored — permissions refreshed│  │ ● ready · connected                 │ │
│ └──────────────────────────────────────────────────────┘  └─────────────────────────────────────┘ │
│ [Go offline] [▶ Sync now] [Dispatcher cancels the selected WO] [Revoke ben.tech's permission] …    │
│ [✖ Anti-pattern: device writes straight to the store] [Recover: reconcile + re-provision] banner   │
└────────────────────────────────────────────────────────────────────────────────────────────────────┘
```

## What to click

Field actions, inside the device frame:

| Button | Path | What you should see |
|---|---|---|
| **Complete…** *(online)* | success | `UI → complete WO-1041 clicked (device holds v2)`, `Device: IDeviceServices.IsOnlineAsync() → True`, `UI → online → WorkOrderService.CompleteAsync directly`, then `Data: repository.Update WO-1041 v2 → v3 · Completed`. Green toast; nothing is queued. |
| **Complete…** *(offline)* | **the queued path** | `Device: queue.Enqueue CompleteWorkOrder WO-1038 (base v2) → PendingSync · 1 pending · payload … bytes` and `UI → offline → the work order row is untouched`. The grid's **Server status** still says `Assigned`; **On this device** says `Completed · pending sync`; an amber card appears in the queue. |
| **Complete…** *(empty notes)* | validation | Rejected locally before anything is queued: amber banner "Completion notes are required — the office cannot audit \"done\"." |
| **Scan** | device capability | `Device: IDeviceServices.ScanDocumentAsync() → "ASSET-Pump-88121"`. Online it is validated on the server — the four simulated tags rotate, so one matches the selected work order's site, others are refused as belonging elsewhere, and the fourth is refused as "not an asset tag". Offline it is recorded for validation at sync. |

Bottom bar:

| Button | Path | What you should see |
|---|---|---|
| **Go offline / Go online** | reconnect simulation | The header pill flips `ONLINE` ⇄ `OFFLINE`. Going online with a non-empty queue starts the replay automatically. |
| **▶ Sync now** | **progress** | Background replay (`Application.StartTask`): one command per 700 ms, the progress bar advances, cards turn green `Synced` one at a time, each step pushed with `Application.Update`. |
| **Dispatcher cancels the selected WO** | conflict setup | `ana.ops` cancels the row on the server: `Data: repository.Update WO-1037 v2 → v3 · Cancelled by ana.ops — the device still holds v2`. |
| **Revoke ben.tech's permission** | **failure: stale permissions** | The server drops `workorder.complete`; the device's snapshot is unchanged. Sync then rejects every queued command: `Security: ben.tech lacks workorder.complete → denied`, cards go grey `Rejected`, and the audit log gets a `Rejected` entry. |
| **✖ Anti-pattern: device writes straight to the store** | **failure: the boundary bypassed** | Four red trace lines: no permission check, no version check, the dispatcher's `Cancelled v3` overwritten, and `audit log still has N entries` — support has no record it happened. |
| **Recover: reconcile + re-provision device** | **recovery** | `cara.admin` replays the correction through `WorkOrderService` (now audited), then the local store is **wiped** and the device re-downloads its cache and a fresh permission snapshot. |
| **Simulate device** (Phone / Tablet / Desktop) | device-aware layout | The frame becomes 380 / 620 / 860 px, touch targets 44 / 40 / 34 px, and the `Site` + `v` columns and the scan result disappear on a phone. Trace: `UI → layout → Phone: frame 380px · touch targets 44px · context columns hidden`. |
| **Clear trace** | — | empties the right-hand card |

On the conflict panel (appears inside the frame when the replay hits one):

| Button | Path | What you should see |
|---|---|---|
| **Keep server — attach my notes** | resolution / recovery | The work order stays `Cancelled`; the technician's notes are appended and audited; the card turns grey `Rejected · kept server — notes preserved`; the replay resumes for anything still pending. |
| **Apply my completion…** | **failure: product rule** | `ben.tech` is a Technician: the server refuses with `workorder.override required (Manager/Admin)`, the refusal is audited, and the panel stays open with the real reason on screen. |

### The five-minute demo (reproduces the video)

1. **Complete…** on `WO-1041` while online → success path.
2. Select `WO-1037` → **Dispatcher cancels the selected WO** (the office moves on).
3. **Go offline**.
4. Select `WO-1038`, type notes, **Complete…** → queued.
5. Select `WO-1037`, type `relief valve replaced, torqued to spec`, **Complete…** → queued.
6. **Go online** → the replay runs: `WO-1038` `Synced`, then `WO-1037` → **Conflict**, both versions on screen.
7. **Apply my completion…** → refused, audited, panel stays open.
8. **Keep server — attach my notes** → resolved, nothing lost.

## Lab steps → where in the code

| Lab step / deliverable | Where |
|---|---|
| Open the project and run it once | `EnterpriseOps.slnx`, `Program.Main` → `Application.MainPage = new UI.FieldTechnicianPage()` |
| Design and prototype Field Technician mode | `UI/FieldTechnicianPage.cs` + `.Designer.cs` |
| Device-aware layout | `Hybrid/DeviceServices.cs` (`DeviceProfile`, `DeviceInfo`, `BrowserDeviceServices.Describe`) → `FieldTechnicianPage.ApplyDeviceLayout()`, `cboDevice_SelectedIndexChanged` |
| Offline work-order cache | `Hybrid/LocalStore.cs` (`CachedWorkOrder`, `LocalStore`), filled by `WorkOrderService.GetAssignedAsync` → `FakeWorkOrderRepository.FindAssigned` |
| Local completion queue | `Hybrid/HybridOfflinePatterns.cs` (`OfflineCommand`, `SyncState`, `IOfflineCommandQueue`) + `Hybrid/OfflineCommandQueue.cs` |
| Reconnect sync simulation | `Hybrid/SyncWorkflow.cs`, driven by `FieldTechnicianPage.StartSync()` (`Application.StartTask` + `Application.Update`) |
| Conflict display | `UI/SyncConflictPanel.cs` + `.Designer.cs`, fed by `SyncWorkflow.SyncConflict` |
| **Deliverable 1** — Hybrid/offline architecture note | [`EnterpriseOps/docs/HybridOfflineArchitectureNote.md`](EnterpriseOps/docs/HybridOfflineArchitectureNote.md) + [`hybrid-offline-architecture.svg`](EnterpriseOps/docs/hybrid-offline-architecture.svg) |
| **Deliverable 2** — Device service abstraction | [`EnterpriseOps/docs/DeviceServiceAbstraction.md`](EnterpriseOps/docs/DeviceServiceAbstraction.md) |
| **Deliverable 3** — Offline queue model | [`EnterpriseOps/docs/OfflineQueueModel.md`](EnterpriseOps/docs/OfflineQueueModel.md) + [`sync-state-machine.svg`](EnterpriseOps/docs/sync-state-machine.svg) |
| **Deliverable 4** — Sync conflict screen | [`EnterpriseOps/docs/SyncConflictScreen.md`](EnterpriseOps/docs/SyncConflictScreen.md) |
| **Deliverable 5** — Field-mode usability checklist | [`EnterpriseOps/docs/FieldModeUsabilityChecklist.md`](EnterpriseOps/docs/FieldModeUsabilityChecklist.md) |
| PWA shell (written, deliberately **not wired in**) | [`EnterpriseOps/docs/PwaShellNote.md`](EnterpriseOps/docs/PwaShellNote.md), [`docs/pwa/manifest.webmanifest`](EnterpriseOps/docs/pwa/manifest.webmanifest), [`docs/pwa/service-worker.js`](EnterpriseOps/docs/pwa/service-worker.js) |
| Show every path | success · validation · queued-offline · conflict · permission-denied · anti-pattern · recovery, all on the bottom bar and all traced |
| Production-readiness note | the "Security consequences" and "Evidence" sections of the architecture note |

## Where things live

```
Module 13/
├─ EnterpriseOps.slnx
└─ EnterpriseOps/
   ├─ UI/
   │  ├─ FieldTechnicianPage.cs / .Designer.cs   the screen: thin handlers, device-aware layout
   │  ├─ SyncConflictPanel.cs / .Designer.cs     both versions + the two resolutions (deliverable 4)
   │  └─ OfflineCommandRow.cs / .Designer.cs     one queue card, coloured by SyncState
   ├─ Hybrid/
   │  ├─ HybridOfflinePatterns.cs                SyncState · OfflineCommand · IDeviceServices · IOfflineCommandQueue
   │  ├─ DeviceServices.cs                       DeviceProfile · DeviceInfo · BrowserDeviceServices
   │  ├─ LocalStore.cs                           CachedWorkOrder + the device's SQLite stand-in (encrypted, wipeable)
   │  ├─ OfflineCommandQueue.cs                  LocalOfflineCommandQueue : IOfflineCommandQueue
   │  └─ SyncWorkflow.cs                         the synchronization boundary + the two resolutions
   ├─ Services/
   │  ├─ WorkOrderService.cs                     the ONLY code that changes a work order
   │  ├─ Commands.cs                             CompleteWorkOrderCommand · WorkOrderSnapshot · CommandResult
   │  ├─ SessionContext.cs                       per-session identity + CommandContext (correlation ids)
   │  └─ ActivityTrace.cs                        IActivityTrace + the layer prefixes
   ├─ Security/
   │  ├─ PermissionService.cs                    server grants + CachedPermissionSet (the device snapshot)
   │  └─ AuditLog.cs                             append-only, device time + server time
   ├─ Domain/WorkOrder.cs                        the entity, with Version as the concurrency token
   ├─ Data/
   │  ├─ FakeWorkOrderRepository.cs              in-memory system of record, optimistic concurrency
   │  └─ SeedData.cs                             48 work orders, 12 of them ben.tech's field cache
   ├─ docs/                                      the five deliverables (+ 2 SVGs, + the PWA shell)
   ├─ Program.cs  Startup.cs  Default.html  Default.json  Web.config
   └─ Properties/launchSettings.json             port 5213
```

## Student review questions, answered against this sample

**What data is allowed offline?**
Only the technician's own still-open work orders, for one tenant — twelve rows, decided on the **server**
by `FakeWorkOrderRepository.FindAssigned(tenantId, user)`, not by the device asking for what it likes. The
device stores a projection (`CachedWorkOrder`), never the domain entity, so no customer record, no
pricing, no other technician's work and no closed history leaves the server. Alongside it the device holds
exactly two more things: the command queue and a permission snapshot. Everything else stays connected —
offline is a product decision taken per workflow, and every extra cached entity is more data to lose, more
sync cases to test and more conflicts to resolve. See the trace lines
`Data: repository.FindAssigned(contoso, ben.tech) → 12 rows (of 48 in the store)` and
`Device: cache scope: ben.tech's own open assignments for contoso only`.

**How are permissions refreshed?**
The device holds a `CachedPermissionSet` with the time it was issued; it is **replaced, never merged**, on
every reconnect. `SyncWorkflow.Replay` refreshes it as step 1 of 2, *before* the first command replays —
`Job: reconnect 1/2 — permission snapshot refreshed BEFORE replay: …`. That snapshot is a convenience for
the UI only: the real check happens server-side inside `WorkOrderService.Complete`, once per queued
command, against `PermissionService`'s current answer. Press **Revoke ben.tech's permission** while the
device is offline and then sync: the queued completions come back `Rejected` with
`Security: ben.tech lacks workorder.complete → denied` and an audit entry each. A revoked user's queued
work is never applied, and the device is told why.

**What happens when local changes conflict with server changes?**
The command's `BaseVersion` no longer matches the row's `Version`, so `WorkOrderService.Complete` returns
`CommandResult.Conflict` with the server's snapshot attached instead of throwing or overwriting. The
replay **stops** — commands behind it stay `PendingSync`, so order is preserved — and `SyncConflictPanel`
shows both versions with who changed what and when. The technician chooses: **Keep server — attach my
notes** leaves the status as the dispatcher set it and appends the field notes (nothing is lost), or
**Apply my completion…**, which needs `workorder.override` and is therefore refused for a Technician —
audited, with the rule shown on screen rather than hidden behind a disabled button. Whatever happens the
command ends in an explicit state and the audit log holds both versions.

## Instructor acceptance criteria, answered

| Criterion | How this sample satisfies it |
|---|---|
| Follows the course architecture baseline | Folder-per-layer (`UI` · `Domain` · `Services` · `Data` · `Security` · `Hybrid`), namespaces matching the folders, typed commands and results across every boundary, per-session services created in the page constructor, no static user state. |
| UI event handlers remain thin and explainable | Every handler reads the selection, builds a typed command, calls a service and reports. The longest one (`btnComplete_Click`) is one decision (`IsOnlineAsync`) and two calls. All are `try/catch` with a single `Fail()` reporter; nothing decides a rule. |
| Service-level logic can be reviewed without opening the designer | The whole offline model is in `Hybrid/*.cs` and `Services/WorkOrderService.cs`. `SyncConflictPanel` raises events, it does not resolve anything; `SyncWorkflow.KeepServerAsync` / `ApplyMineAsync` hold the resolutions. |
| At least one failure path is demonstrated | Five: a validation refusal (empty notes), an action taken offline that is **queued instead of failing**, a sync **conflict**, a **stale-permission rejection**, and the video's **anti-pattern** (direct write: no permission check, no version check, no audit). Plus two recoveries: conflict resolution and *Recover: reconcile + re-provision*. |
| The student can explain state ownership, security implications and production behaviour | The architecture note has a state-ownership table and a security table; every claim in the docs has an **Evidence** row naming the trace line or screen element that proves it; the live trace shows `UI →`, `Service:`, `Data:`, `Security:`, `Device:` and `Job:` decisions as they happen. |

## Verified / unverified

Verified while building (checked against `Wisej.Framework.dll` 4.1.0 by reflection, and inherited from
earlier course samples):

* `Application.StartTask(Action)` exists; **`Application.Update` takes `(IWisejComponent context, Action)`**
  — there is **no** single-argument `Application.Update(component)` overload in 4.1.0, so the cookbook's
  `Application.Update(this)` shape does not compile. This sample uses
  `Application.Update(this, () => { … })` (the form the framework's own XML docs show) inside a
  `Push(Action)` helper, guarded by `IsDisposed` and `catch (ObjectDisposedException)`.
* `Application.Browser` is a `Wisej.Core.ClientBrowser` with `Device` ("Mobile" / "Tablet" / "Desktop"),
  `Type`, `OS`, `Version`, `Size`, `ScreenSize`, `PixelRatio`, `IsDarkMode`, `UserAgent` — this closes the
  cookbook's *(unverified)* note about `Application.Browser` for device-aware layout. It is read once in
  `BrowserDeviceServices.Describe()`, inside a `try/catch` for the no-client case.
* Docking order: **the Fill control must be the first `Controls.Add`, Top/Bottom bars last** (Wisej docks
  from the last added to the first). Every panel in the designer follows it.
* `AlertBox.Show(text, MessageBoxIcon, alignment:, autoCloseDelay:)`, `Wisej.Web.FlowLayoutPanel`,
  `ProgressBar.BarColor`, `DataGridView` with `AutoGenerateColumns = false` + `DataPropertyName`, and
  `DataGridViewColumn.Visible` all exist and compile.

Relied on but **not executed** (the brief forbids running the app — please check these at runtime):

* `Application.Update(this, …)` actually pushing from inside `Application.StartTask` over the WebSocket
  during the sync replay (the pattern is the framework's documented one; the pacing is 700 ms, well above
  the 250 ms floor).
* `dgvCache.CurrentRow.Index` mapping 1:1 onto the bound `List<CachedWorkOrder>` position, and
  `BindingSource.ResetBindings(false)` repainting in-place changes to the bound objects.
* `FlowLayoutPanel` with `FlowDirection.TopDown`, `WrapContents = false` and `AutoScroll = true` scrolling
  the queue cards when more than about five are queued.
* Live re-layout when `pnlDevice.Width` changes (the Simulate switch) — anchored children are expected to
  follow.

Deliberate deviations from the walkthrough, for the reasons given:

* The video's queue control is a `Wisej.Web.DataRepeater` named `repQueue` bound through
  `ItemTemplate` + `DataRepeaterBindings`. Since this sample cannot be run before delivery, the queue is
  built from explicit `OfflineCommandRow` user controls inside a `FlowLayoutPanel` (`flpQueue`) — same
  card rendering and the same `SyncState` pills, without depending on an unverified designer-bound
  template. Swapping in a `DataRepeater` changes only `RenderQueue()`.
* The video's file tree shows seven projects; ADR-001 of this course keeps **one** project with
  folder-per-layer namespaces (`EnterpriseOps.Hybrid`, `EnterpriseOps.UI`, …) so the sample runs with a
  single `dotnet run`. The folders map 1:1 onto the multi-project split.
* `LocalStore.cs` is the in-memory stand-in for the video's `LocalStore.Sqlite.cs`. Same surface, no
  database dependency; `docs/OfflineQueueModel.md` says exactly what changes in a production build.
* The PWA manifest and service worker are written as deliverables under `docs/pwa/` and are **not
  registered** by `Default.html` — see `docs/PwaShellNote.md` for the three reasons.
