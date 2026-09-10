# migration-log.md — LegacyOrderDesk → OrderDesk.Web

One line per accepted decision or workaround. Later modules append to this file; the console's trace panel is the
live version of it.

## Module 1 · Migration discovery (2026-09-09)

- **Assessment first.** 14 items inventoried and classified (4 direct-port · 5 adapt · 3 redesign · 1 defer · 1 remove) before any namespace was replaced. See Module 1 `MigrationAssessmentWorkbook.md`.
- **Startup → `Default.json` + `Program.Main`.** `Application.EnableVisualStyles` / `Application.Run` have no equivalent; `Program.Main(NameValueCollection)` runs once per browser session and sets `Application.MainPage`. Kestrel (`Startup.cs`, `app.UseWisej()`) owns the process.
- **Business logic copied verbatim.** `Domain/` (Order, Customer, OrderService, CustomerService, InvoiceDocument, SampleData) has no UI or desktop dependency and moved unchanged. Totals are identical on both sides.
- **Static current user is a defect, not a style issue.** `AppState.CurrentUser` is one slot per server process; a second session overwrites it. Typed session context is Module 4's job.
- **Print Invoice → server PDF.** `PrintDocument` targets a printer attached to the server. The shared `InvoiceDocument` lines are written by a dependency-free `InvoicePdfWriter` and shown in a `PdfViewer` (+ Download).
- **Export to Excel → bytes + `Application.Download`.** Excel Interop is not supported in a server process and `C:\Orders\out.xlsx` is the user's disk. Module 6 upgrades to a managed .xlsx writer and a report queue.
- **Attach file deferred to Module 6.** `OpenFileDialog` + `C:\Orders\Attachments` cannot exist on the server. Replacement: `Upload` control + configured storage root.
- **Connection string → `Web.config`.** `App.config` is gone; settings live in the web host's configuration, read with `System.Xml.Linq` (Module 2).
- **Modal dialogs do not block in Wisej.NET.** `ShowDialog(callback)` / `ShowDialogAsync()`; the caller disposes the dialog when it closes (Module 3 rule).

## Module 5 · Data access, grids, validation & performance (2026-09-10)

- **Production-sized test data.** `OrderStore.Large` seeds 200,000 orders once per process (`Lazy<T>`, deterministic). The walkthrough's five orders stay first. `InMemoryOrderRepository.Shared` (Module 1) is untouched.
- **"Load every row" measured, not argued.** `Legacy/DesktopGridHabits.LoadWholeTable` (= `OrdersForm.ReloadGrid`) is kept and executed by the console: `GetOrders()` clones + sorts 200,000 orders per session. Recorded in the Performance card and in `GridPerformanceNotes.md`.
- **Query model added to the Domain.** `OrderQuery { Status, Text, CustomerId, SortBy, Descending, Skip, Take }`, `PagedResult<T>`, `OrderSummary`, `OrderQueryService` (`Page`, `Count`, `Summarize`). `OrderService`'s Module 1 API is unchanged; `OrderFilter` still works.
- **Filter + sort on the server, memoized.** `LargeOrderRepository.Query` orders once per filter+sort key and keeps the ordered set until the next write; pages are `Skip/Take` over it. Only the requested rows are cloned.
- **Virtual rows.** `DataGridView.VirtualMode = true`, `RowCount` from `Count`, `CellValueNeeded` served from a 50-row block cache; column sorting disabled (`SortMode = NotSortable`) because the grid only ever holds a page. Pattern in `VirtualRowPattern.md`.
- **Σ row on the server.** `Summarize(query)` → count + total in the footer. A grid summary row over a virtual grid would sum what the browser holds; the footer sums what the filter matches.
- **Default filter = Open.** The users' working set; "All" is a choice. The search box matches customer, id and PO number — the same three fields `OrderService.Search` always looked at.
- **Validation out of the form.** `OrderValidator.Validate(order) → ValidationResult { Errors[field], General }` in `Domain/`; the dialog shows messages with an `ErrorProvider`, a batch and a direct call use the same class. The desktop's single `MessageBox` rule is kept in `Legacy/` for comparison. Rules in `ValidationRules.md`.
- **Edit workflow.** `EditOrderDialog` (Customer, PO, Owner, Status, one line) → `ShowDialog(callback)` → validator on Save → `OrderService.Save` (CalculateOrderTotal reused) → toast → block cache cleared, grid re-counted. The dialog is disposed in the callback.
- **Live updates: not added.** Nothing on this screen benefits from server push; a Timer drives only the lab's batch and measurement paths. (Real-time is its own course.)
- **Performance notes.** `GridPerformanceNotes.md` holds the method and the table; the *Average of 10* interaction row is the number to quote.
