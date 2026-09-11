# TicketOps · Production Architecture Deep Dive · Module 9

Local lab build for **Module 9 · JavaScript Integration, Widget Interop & Client-Side Enhancements**.
It follows the walkthrough video *Add safe JavaScript interop*: a **Ctrl+K** shortcut that focuses the
global search from anywhere, and a **server-confirmed clipboard copy** of the selected work order's link.
The browser focuses and copies; the server validates, decides and records.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Production Architecture Deep Dive\Module 9\TicketOps"
dotnet run -f net10.0 --urls http://localhost:5109
```

Then open <http://localhost:5109> (or open `TicketOps.slnx` in Visual Studio and press F5). `localhost`
counts as a secure context, so `navigator.clipboard` is available.

## The screen

- **Search** (`searchBox`, a `GlobalSearchBox`) with the **Ctrl + K** hint: Ctrl+K focuses it from anywhere,
  Esc clears it, `?` shows the shortcut list.
- **Work-order grid**, the selected work order, and **Copy link**.
- **LINK**: the last link the server built, also the manual fallback when the browser refuses the copy.
- **AUDIT LOG**: the server-side record, written only after the browser confirmed the copy.

## Deliverables (lab guide)

| # | Deliverable | Where |
|---|---|---|
| 1 | Embedded script | `TicketOps/Platform/ticketops.interop.js`, embedded (`[assembly: WisejResources]`) and attached to the search box by the `JavaScript` extender in `Views/WorkOrdersView.Designer.cs` |
| 2 | C# method invoking client behavior | `Infrastructure/BrowserApi.CopyToClipboardAsync` (`Application.EvalAsync`, awaited), called from `WorkOrdersView.buttonCopyLink_Click` |
| 3 | Server callback handler | `Controls/GlobalSearchBox.ReportShortcut`, a `[WebMethod]` that whitelists its argument and raises `ShortcutPressed` |
| 4 | Security note for each interop point | [`docs/InteropSecurityNotes.md`](TicketOps/docs/InteropSecurityNotes.md) |
| — | Interop contract | [`docs/InteropContract.md`](TicketOps/docs/InteropContract.md) |
| — | Production-readiness note | [`docs/ProductionReadinessNote.md`](TicketOps/docs/ProductionReadinessNote.md) |

The video's `MainPage.cs` is `Views/WorkOrdersView` here, and its `Scripts/` folder is `Platform/`, the
folder Wisej.NET bundles automatically. The video's `Application.Eval` is `Application.EvalAsync` so the
server can await the browser's answer before it writes the audit entry.

## Where things live

```
TicketOps/
├─ Views/WorkOrdersView            the screen + the JavaScript extender
├─ Controls/GlobalSearchBox.cs     TextBox + [WebMethod] ReportShortcut
├─ Controls/StatusBanner           status line + banner
├─ Platform/ticketops.interop.js   the client script
├─ Services/                       TicketLinkService, AuditLogService, WorkOrderService
├─ Domain/                         WorkOrder (CanShareLink), SessionUser, TicketLink, AuditEntry, OperationResult
├─ Data/                           IWorkOrderRepository + in-memory implementation
├─ Infrastructure/                 BrowserApi, SessionContext, ILog/ActivityLog, AppComposition
├─ Resources/Strings.cs            user-facing messages
└─ docs/                           the deliverables
```
