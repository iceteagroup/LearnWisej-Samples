# Deliverable 3 · Notification review — every MessageBox in LegacyOrderDesk

Rule from the lesson: **keep it modal when a decision is required** (the answer changes what happens next, or the
user must fix input before continuing); **make it non-blocking when nothing is decided** (an AlertBox or Toast
saves a click and does not mask the page). Modernize incrementally: the workflow is preserved first, the
notifications are improved second — none of these swaps changes the order of operations.

Wisej.NET shapes used: `MessageBox.Show(text, caption, OK, icon)` (blocking in the browser, the server does not
wait), `await MessageBox.ShowAsync(…, YesNo, …)` for an awaited decision, `Ui.Toast(text, icon)` =
`AlertBox.Show(text, icon, alignment: TopRight, autoCloseDelay: 4000)` for information.

## The seven MessageBox calls in LegacyOrderDesk

| # | Where (desktop) | Call | Kind | Decision | OrderDesk.Web | Reason |
|---|---|---|---|---|---|---|
| 1 | `LoginForm.signInButton_Click` | `MessageBox.Show("Enter a user name.", …, OK, Warning)` | validation | **keep modal** | stays a modal validation message when the login screen is ported (Module 4: sign-in against the session) | the user cannot continue without fixing the input; an inline `ErrorProvider` message is the other acceptable form (Module 5) |
| 2 | `EditOrderDialog.saveButton_Click` | `MessageBox.Show("Select a customer.", "Validation", OK, Warning)` | validation | **keep modal** | unchanged in `Dialogs/EditOrderDialog.cs` | Save must not proceed; the dialog stays open behind the message. Reachable only when the combo has no selection (the dialog pre-selects the order's customer) |
| 3 | `OrdersForm.ordersGrid_CellDoubleClick` | `MessageBox.Show("Saved.", "LegacyOrderDesk")` | information | **Toast** | `Ui.Toast($"Order {saved.Id} saved.")` in `OrdersScreen.EditOrder` | nothing to decide; one extra click and the whole page masked for a confirmation the grid already shows |
| 4 | `OrdersForm.exportButton_Click` | `MessageBox.Show("Exported to " + path, …)` | information | **Toast** | `Ui.Toast("orders.csv sent to the browser.")` in `ReportsScreen.Export` | the browser's own download UI is the confirmation; there is no server path to show |
| 5 | `OrdersForm.exportButton_Click` (catch) | `MessageBox.Show("Export failed: " + ex.Message, …, OK, Error)` | error | **keep modal** | keep `MessageBox.Show(…, MessageBoxIcon.Error)` where the export can fail (Module 6's report queue reports failures per job) | an error the user must acknowledge before retrying; not reachable in this module — the in-memory CSV cannot fail the way Interop did |
| 6 | `OrdersForm.attachButton_Click` | `MessageBox.Show("Attached to " + folder, …)` | information | **Toast** | not ported in this module (no Attach button until Module 6, which confirms the upload with a Toast) | nothing to decide; the local folder does not exist on the server anyway |
| 7 | `OrdersForm.aboutMenuItem_Click` | `MessageBox.Show("LegacyOrderDesk 3.2 — …", "About")` | information | **Toast** | `Ui.Toast("OrderDesk.Web — LegacyOrderDesk 3.2 migrated to Wisej.NET.")` in `AppShell.menuHelpAbout_Click` | nothing to decide; a modal About box is a desktop habit, not a requirement |

Totals: 7 calls · 3 keep modal (2 validation, 1 error) · 4 Toast (information). The lab asks for **one** informational
MessageBox replaced; #3 is that one — it is the "Saved." the storyboard shows — and #4, #6, #7 follow the same rule.

## What stays blocking, and why it is still cheap

A modal `MessageBox` / `MessageBox.ShowAsync` in Wisej.NET blocks the **browser**, not the server: `ShowAsync`
returns the request, the box is rendered client-side, and the answer comes back as an ordinary event that resumes
the `async` handler.

## Evidence (in the running app)

- **Save** in the edit dialog (double-click 1042 → Save): a Toast in the top-right corner, `Order 1042 saved.`,
  auto-closing after 4 s; no OK button anywhere; the grid is already updated behind it.
- **Help › About** (`menuHelpAbout`), ToolBar **Export** (`toolExport`): Toasts.
- The validation message (#2) is the one MessageBox left in the ported dialog (`Dialogs/EditOrderDialog.cs`,
  `saveButton_Click`).
