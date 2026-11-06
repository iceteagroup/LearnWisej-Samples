# FieldOps · Hybrid & Native Packaging · Module 1

Lab build for **Module 1 · Hosting Models Beyond the Browser**. FieldOps as the walkthrough shows it
in the browser frame: the blue app bar **FieldOps · My work orders**, the action strip with
**Update status** and **Attach photo (upload)**, the grid Order · Site · Fault · Priority · Status
with the four orders WO-4181, WO-4184, WO-4190, WO-4193, and the status strip
**Connected · 4 open orders**. Behind it: the `WorkOrder` model, a `WorkOrderService` that owns the
transition rule, the work-order detail page with the status update flow, the `Device.Valid` gate,
and the written delivery decision. No client project yet — that arrives in Module 3 and changes
none of these screens.

## Run it

```bash
cd "Hybrid and Native Packaging Course/Module 1/FieldOps"
dotnet run -f net10.0 --urls http://localhost:6301
```

Open <http://localhost:6301> (signed in as Dana Whitfield; `?tech=marek` signs in Marek Sobczak).
In Visual Studio open `FieldOps.slnx` and press F5.

## What to try

| Action | Expected result |
|---|---|
| Open the app in a browser | Four open orders, **Attach photo (upload)** in the action strip, no **Take photo**: `Device.Valid` is false in a browser. |
| Select **WO-4190**, press **Update status** (or double-click the row) | The detail page: customer, site address, fault, priority, assigned technician, status chip, the three steps. |
| Choose **Completed** while the order is Assigned, press **Update status** | Amber: *WO-4190 must be Accepted before it can be Completed.* Nothing saved. |
| Choose **Accepted**, press **Update status** | Green: *Saved: WO-4190 is now Accepted.* |
| Choose **Accepted** on an order that is On site | Amber: *… An order only moves forward, so it cannot go back to Accepted.* |
| Open a second browser window, move WO-4190 to **On site** there, then press **Update status** in the first window | Red: *WO-4190 was changed somewhere else while you had it open. It is now On site…* — the server log carries the detail (`WorkOrderConflictException … version 2, now 3`). |
| Back arrow, then refresh the other window | Both lists show the same status: one server, one store. |
| Narrow the window below 600 px | The grid keeps Order · Site · Status (the compact list of the walkthrough). |
| `?tech=marek` | Marek's single order; Dana's orders never appear. |

## Lab tasks → where in the code

| Lab step / deliverable | Implementation |
|---|---|
| `WorkOrder` model (number, customer, site address, priority, status, assigned technician) | [Model/WorkOrder.cs](FieldOps/Model/WorkOrder.cs) |
| `WorkOrderService` — open orders per technician, `UpdateStatus` with the transition rule | [Services/WorkOrderService.cs](FieldOps/Services/WorkOrderService.cs) over [WorkOrderStore.cs](FieldOps/Services/WorkOrderStore.cs) |
| Work-order list bound to the service, filtered to the signed-in technician | [WorkOrdersPage.cs](FieldOps/WorkOrdersPage.cs) `LoadOrders` |
| Detail page + status flow Accepted → On site → Completed | [WorkOrderDetail.cs](FieldOps/WorkOrderDetail.cs) `btnSaveStatus_Click` |
| `Wisej-4-Hybrid` + `btnTakePhoto` only when `Device.Valid && Device.Media.CaptureSupported`, otherwise `uploadPhoto` | `WorkOrderDetail_Load`, [Capabilities.cs](FieldOps/Capabilities.cs), [PhotoActions.cs](FieldOps/PhotoActions.cs) |
| Show every path: confirmation, rejected transition in words, friendly failure + logged detail | `ShowMessage` tones in `WorkOrderDetail`; [Services/OpsLog.cs](FieldOps/Services/OpsLog.cs) |
| Delivery model comparison table | [docs/DeliveryModelComparison.md](docs/DeliveryModelComparison.md) |
| Delivery decision record (targets, hosting mode, costs of Local, build order, exit criteria) | [docs/DeliveryDecisionRecord.md](docs/DeliveryDecisionRecord.md) |
| Capability check design based on `Device.Valid` | [docs/CapabilityCheckDesign.md](docs/CapabilityCheckDesign.md) |
| Architecture diagram of one codebase and its clients | [docs/Architecture.md](docs/Architecture.md) (Mermaid) |

## What was verified, and how

- `dotnet build` (net10.0-windows and net10.0): 0 errors, 0 warnings, against `Wisej-4` 4.1.7 and
  `Wisej-4-Hybrid` 4.1.7.
- Browser pane (`fieldops-m1`, port 6301): the list matches the walkthrough's browser frame; the
  Upload path is the one shown (`Device.Valid` false); in one session WO-4190 was refused
  Assigned → Completed, saved Assigned → Accepted; a second session saw *Accepted* and moved it to
  On site; the first session's stale On site was refused with the conflict message and the server
  log shows `ERROR status update WO-4190 -> OnSite: WorkOrderConflictException …`; a backwards
  change was refused; at 380 px the compact list showed. No console errors, no server errors.
- Not exercised here: posting an actual image through the Upload control (the browser pane cannot
  drive a native file picker) and the **Take photo** path, which needs the Hybrid client — it is
  exercised from Module 3 on.

## Where this sample differs from the lesson

- **Sign-in is a stand-in.** The lab says "the signed-in technician"; the walkthrough shows no
  sign-in screen. The session takes the technician from `?tech=` (default Dana Whitfield) in
  `Program.Main`, so the list is genuinely filtered without adding a screen the video does not have.
- **Photo action on the list.** The walkthrough puts *Attach photo (upload)* / *Take photo* in the
  list's action strip; the lab puts the gate in the detail page's `Load`. Both are built, with the
  same gate; the list's button acts on the selected row.
- **Fault column.** The model carries `Fault` in addition to the lab's fields because the video's
  grid has a Fault column.
- **Status values.** The video shows *Assigned* as the first status; the lab lists the three steps a
  technician takes (Accepted, On site, Completed). Assigned is the dispatcher's starting state and
  cannot be chosen on the detail page.
- The walkthrough's offline banners (Remote-only and Local fallback) belong to the Hybrid client; in
  Module 1 they are design, written into the decision record. They become running code in Module 3.
