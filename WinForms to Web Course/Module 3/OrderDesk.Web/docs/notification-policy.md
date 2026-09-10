# Notification policy — which MessageBoxes stay (Module 3)

Rule from the lesson: **a MessageBox is for decisions. Information does not block.** Every
`MessageBox.Show` in LegacyOrderDesk was classified with that rule.

| Where (LegacyOrderDesk) | Text | Needs a decision? | Web replacement | In this sample |
|---|---|---|---|---|
| `OrdersForm.ordersGrid_CellDoubleClick` / `newOrderButton_Click` | `MessageBox.Show("Saved.")` | No — purely informational | **Toast** `Notify.Saved("Order 1042 saved.")` (BottomRight, auto-close 3.5 s, icon-ok) | replaced |
| `EditOrderDialog.Save_Click` | `"Cannot save"` + the `OrderValidator` messages | Yes — the user must fix the data before continuing | **stays modal** (`MessageBox.Show(…, "Cannot save", OK, Warning)`); Module 5 adds inline `ErrorProvider` messages next to it | kept |
| `OrdersForm.printInvoiceButton_Click` | `"Could not print: …"` | Yes — an operation failed and the user must know before retrying | `Notify.Error` (AlertBox TopRight) is acceptable because there is nothing to decide; the sample defers the whole feature to Module 6 | deferred (button disabled) |
| `OrdersForm.exportButton_Click` | `"Exported to …"` / `"Excel is not available … Wrote … instead."` | No / partly (a fallback happened) | `Notify.Info` / `Notify.Warning` (AlertBox) — the download itself replaces the path text | deferred (button disabled) |
| `OrdersForm.aboutMenuItem_Click` | About text | Not a decision, but user-requested | **stays modal** — the user opened it and dismisses it; not a status message | kept |
| `Notify.Confirm(…)` (`MessageBoxButtons.YesNo`) | any "Are you sure?" | Yes | **stays modal** | available in `Shared/Notify.cs` |

## The primitives (Shared/Notify.cs)

| Call | Control | Blocks the handler? | Where |
|---|---|---|---|
| `Notify.Saved(text)` | `Wisej.Web.Toast(text, "icon-ok")`, `AutoCloseDelay = 3500`, `Alignment = BottomRight` | No | bottom-right, like the video |
| `Notify.Info / Warning / Error(text)` | `AlertBox.Show(text, icon, alignment: TopRight, autoCloseDelay: 4000–6000)` | No | top-right so it never covers the buttons |
| `Notify.Confirm(text)` | `MessageBox.Show(text, caption, YesNo, Question)` | Yes — returns `DialogResult` | centred, modal mask |
| `MessageBox.Show("Saved.")` | the legacy call | Yes | kept only in the ✕ demo button |

Why the Toast and not an AlertBox for "Saved": both are non-blocking; the Toast is the lighter,
transient one and matches the video ("Order 1042 saved." with a green check). AlertBox is the
better fit when the message should stay until read (a warning about a fallback, an error).

## Modernize incrementally

The port kept the workflow first (row → modal dialog → save) and changed only the confirmation.
Nothing else about the dialog moved: same fields, same DialogResult, same validation rule. That is
the module's "parity first, controlled modernization second".

## Evidence

- **Saved via MessageBox ✕ → Toast ✓**: `✖ MessageBox.Show("Saved.")  blocking — this handler is suspended until OK is clicked (one extra click, no decision to make)`;
  the modal "Saved." box appears with its mask; after OK: `← MessageBox.Show returned  DialogResult.OK after <n> ms — the user paid a click for an informational message`;
  a Toast "Order 1042 saved." slides in bottom-right and fades after 3.5 s; `✓ Toast "Order 1042 saved."  non-blocking · handler continued immediately · AutoCloseDelay 3500 ms · BottomRight`;
  `★ notification policy  informational → Toast/AlertBox · decisions & validation failures → MessageBox stays modal`; green banner quoting the milliseconds waited.
- **Edit selected ✓ → Save**: no MessageBox at all — only the Toast (`→ Toast  "Order 1042 saved." …`).
- **Validation kept modal**: select order **1040 Fabrikam Inc** (it has no owner), Edit selected ✓, Save without choosing an Owner:
  `• OrderValidator.Validate  Owner: Assign an owner before saving.`, `★ validation MessageBox stays modal  a decision is required — the user must fix the data first`,
  and the modal "Cannot save" box; the dialog stays open until the owner is picked or Cancel is pressed.
- **File › Exit**: `Notify.Info` AlertBox top-right (non-blocking) instead of closing anything.
- **Help › About**: a modal MessageBox, by design.
