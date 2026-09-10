# Production-readiness note — Module 9 interop

*Lab step 8: "add a short production-readiness note, then run and confirm the workflow behaves correctly."*

## Ready as built

- **Small, named surface.** Three interop calls (`attachSearchShortcuts`, `ReportShortcut`, `copyToClipboard`),
  each documented in [`InteropContract.md`](InteropContract.md) with its validation in
  [`InteropSecurityNotes.md`](InteropSecurityNotes.md). One embedded script, one extender, one `[WebMethod]`, one `EvalAsync` in one helper class.
- **Server stays the authority.** The id is re-loaded, the rule is the domain's, the URL is built and
  signed server-side, the audit entry is written only after the browser confirmed. A forged id, a
  confidential work order and a denied clipboard each stop at the right layer with a plain sentence.
- **Every path visible, nothing leaked.** `Strings.*` in the banner; exception types, host names and the
  browser's error text in the trace only; the trace panel HTML-encodes what it renders.
- **Timing-safe.** The script attaches through the `JavaScript` extender (after the widget exists), is
  idempotent, and never sits in `Default.html`.
- **Testable without a browser.** `TicketLinkService`, `WorkOrderService`, `AuditLogService` and
  `WorkOrder.CanShareLink` take no Wisej.NET type; `ClipboardOutcome.From` is a pure function over the
  browser's answer.

## Before shipping

| Item | Why | Where |
|---|---|---|
| Serve over **HTTPS** | `navigator.clipboard.writeText` is only available in a secure context (https or localhost); on plain http every copy takes the fallback path | reverse proxy / Kestrel certificate (Module 12) |
| Real signing key from configuration | `AppComposition.DemoLinkSigningKey` is a demo constant; production reads it from a secret store and rotates it | `Infrastructure/AppComposition.cs` (Module 8: `Application.Services`) |
| Real `SessionUser` from authentication | the technician is hard-coded per session for the lab; `IsManager` must come from the identity provider | Module 11 login gate |
| Persist the audit log | `AuditLogService` is in-memory per session; production appends to a table with the same `Record` contract | `Services/AuditLogService.cs` |
| Rate-limit / log WebMethod refusals | `ReportShortcut` already refuses unknown names; a production log should alert on repeated refusals from one session (a sign of tampering) | `Controls/GlobalSearchBox.cs` |
| Keep `Eval` strings out of screens | today the only `Eval` is in `BrowserApi`; a review rule ("no `Application.Eval` outside `Infrastructure/BrowserApi.cs`") keeps it that way | code review checklist |
| Accessibility of the `?` overlay | it is a plain `div` with `role="dialog"`; add focus management if it grows beyond a hint | `Platform/ticketops.interop.js` |
| Consider the no-JavaScript alternative for Ctrl+K | `Form.Accelerators` + the `Accelerator` event handle the key entirely on the server (one round trip per press, no script to review). The lab uses JavaScript to show the pattern; if the shortcut is the only script left in an app, the server-side accelerator is the smaller surface | `Views/WorkOrdersView.cs` |

## Run and confirm

`dotnet run -f net10.0 --urls http://localhost:5109`, then follow the **What to click** table in the
module README: success (Copy link), progress (Verify 6 links), failure ×2 (#9999, #2006), client-side
denial with recovery, data outage with recovery, and Ctrl+K / Esc / ? from the keyboard.
