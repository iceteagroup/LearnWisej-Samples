# OrderDesk · From WinForms to the Web · Module 3

Local lab build for **Module 3 · Forms, Navigation, Layout & Modal Workflow**. It follows the walkthrough video
(`wisej-wf-forms-navigation-modal`) and the lab guide: the Wisej.NET shell gains **navigation** (left nav +
`MenuBar` + `StatusBar`) with three ported screens (Orders / Customers / Reports), the WinForms `EditOrderDialog`
is ported to a `Wisej.Web.Form` and shown with `ShowDialog()` **inside a using block**, and the informational
`MessageBox.Show("Saved.")` becomes a **Toast** while the validation MessageBox stays modal. The buttons make the
dialog-disposal problem visible (five leaked dialogs) and fix it.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine. The `LegacyOrderDesk`
WinForms "before" app lives in `../Module 1/LegacyOrderDesk`.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/WinForms to Web Course/Module 3/OrderDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5103
```

Then open <http://localhost:5103>. (Visual Studio: open `OrderDesk.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package. The page fits a
1400×760 viewport: shell 840 wide (nav 160 + pages 680) + trace 560.

## What to try

The Orders page opens with the five walkthrough orders first (1042 Northwind Traders $4,820.00 Open · 1041 Contoso Ltd
$1,290.50 Shipped · 1040 Fabrikam Inc $760.00 Open · 1039 Adventure Works $12,400.00 Invoiced · 1038 Globex Corp $3,090.00 Open).
The right-hand card is the migration trace; the status bar shows **Dialogs alive: n** live.

| Action | Path | What you should see |
|---|---|---|
| (startup) | success | `• server Program.Main`, `★ log MenuStrip → MenuBar · StatusStrip → StatusBar`, `★ log OrdersForm → OrdersPage : Panel`, `★ log EditOrderDialog → Wisej.Web.Form`, `• server OrderService.GetAll()  5000 orders → ordersGrid.DataSource (BindingList<Order> → List<Order>)`, `← JS→.NET navigate → Orders  startup`, `• server ShowPage(Orders)`; status bar "Ready · 5000 orders · multi-user web · session xxxxxxxx" |
| **Orders / Customers / Reports** (left nav, or **View** menu) | navigation | `← navigate → Customers  navCustomers.Click`, `• OrderService.Customers  12 customers → customersGrid.DataSource`, `★ TaxId never leaves the server …`, `• ShowPage(Customers)  CustomersPage.Visible = true · other pages hidden (same instances)`; going back never reloads — same instances |
| **Reports › Open orders by customer** (menu) | navigation | Reports page with the item selected; `← reportsList.SelectedIndexChanged  "Open orders by customer"`, `• ReportsPage  placeholder — generation is Module 6 (PDF / .xlsx / queue)` |
| **Edit selected ✓** (or double-click row 1042, or **Edit › Edit selected order…**) | success | `→ dlg.ShowDialog()  modal · this handler is suspended…`; the dialog **Edit Order 1042** opens. Change Status to `InProgress`, **Save**: `← btnSave.Click`, `• OrderValidator.Validate  valid`, `← dlg.ShowDialog() returned  DialogResult.OK · dlg.IsDisposed=False (inside the using block)`, `✓ _service.Save(dlg.Order)  Order 1042 · Dana · InProgress`, `→ Toast  "Order 1042 saved." …`, `• EditOrderDialog.Dispose()  … LiveInstances=0`, `✓ using block exit  dlg.IsDisposed=True · LiveInstances=0`. Toast bottom-right "Order 1042 saved.", green banner, grid shows 1042 as InProgress. **Cancel** instead: `DialogResult.Cancel`, still disposed, blue banner |
| **Edit selected ✓** on **1040 Fabrikam Inc** → Save with no Owner | validation stays modal | `• OrderValidator.Validate  Owner: Assign an owner before saving.`, `★ validation MessageBox stays modal …` and the modal **Cannot save** box; the dialog stays open |
| **New Order** (detail panel, or **Edit › New order…**) | success | `★ AppState.CurrentUser not ported  owner defaults to "Kelly" until Module 4 (UserContext)`; dialog **New Order** (Northwind Traders / Kelly / Open); Save → `✓ _service.Save(dlg.Order)  Order 1043 · Kelly · Open`, Toast "Order 1043 saved.", 1043 appears on top of the grid (the store is process-wide: it stays until the app restarts) |
| **Leak dialogs ✕** | failure | five `• dialog #n  ShowDialog() → Close() · IsDisposed=False · LiveInstances=n`, then `✖ Application.OpenForms.Count …`, `✖ Application.FindComponents(c => c is EditOrderDialog)  5 closed EditOrderDialog instances still registered in this session`, `✖ EditOrderDialog.LiveInstances  5 constructed, never disposed (process-wide counter)`, `★ closed dialogs are not disposed  ShowDialog keeps the instance for reuse — the CALLER must Dispose (using block)`; status ● alarm; red banner "✖ closed dialogs are not disposed — LiveInstances = 5 after 5 open/close cycles …"; status bar **Dialogs alive: 5** (click again → 10) |
| **Dispose fix ✓** | recovery | `• cleanup  disposing 5 leaked dialog(s) · 5 found with Application.FindComponents, 0 from the lab's own list`, `✓ leaked dialogs disposed  LiveInstances=0`, five `✓ using #n  ShowDialog() → Close() → Dispose() · IsDisposed=True · LiveInstances=0`, `✓ Application.FindComponents(…)  0 alive`, `★ using block frees the dialog every time …`; green banner; **Dialogs alive: 0** |
| **Saved via MessageBox ✕ → Toast ✓** | before / after | `✖ MessageBox.Show("Saved.")  blocking — this handler is suspended until OK is clicked…`; the modal **Saved.** box appears — click OK: `← MessageBox.Show returned  DialogResult.OK after <n> ms …`, then the Toast "Order 1042 saved." and `✓ Toast "Order 1042 saved."  non-blocking · handler continued immediately · AutoCloseDelay 3500 ms · BottomRight`, `★ notification policy …`; green banner quoting the milliseconds you waited |
| **Tab order / docking** | progress (Timer 700 ms) | `• TabOrderWalker.Collect(OrdersPage)  13 controls · docking: headerLabel Top 36 · detailPanel Right 220 · ordersGrid Fill`, then one highlighted control (amber overlay) per tick with `• TabIndex=0 ordersGrid (DataGridView)  Dock=Fill · 460×… @ (0,36) · TabStop=True`, `• TabIndex=1 detailPanel (Panel)  Dock=Right …`, `•   TabIndex=0 detailCustomer (TextBox)  Anchor=Top, Left …` … `•   TabIndex=5 exportButton (Button) … · disabled`, captions 6–9, `• TabIndex=2 headerLabel (Label)  Dock=Top`; ends with `★ tab order preserved …`, `★ anchors → docking …`, `✓ Tab order / docking  walk complete · 13 controls`, green banner. Click again while running to restart |
| **File › Exit** | finding | nothing closes; AlertBox top-right "On the web, Exit ends this session only — see Module 4 (Sign out)."; `★ Exit is a session action on the web …` |
| **Help › About** | kept modal | the About MessageBox; `• MessageBox.Show(About)  modal on purpose: the user opened it and dismisses it — not a status message` |
| **Clear** (trace card) | – | empties the trace |

## Where things live

```
Module 3/
├─ OrderDesk.slnx · .gitignore
├─ README.md
└─ OrderDesk.Web/
   ├─ Program.cs · Startup.cs · Default.html · Default.json · Web.config      the Module 2 shell, unchanged
   ├─ Properties/launchSettings.json                                          port 5103
   ├─ MainPage.cs · MainPage.Designer.cs      app bar · MenuBar · left nav · pageHost · banner · button bar · StatusBar · TracePanel
   ├─ Pages/
   │  ├─ ModulePage.cs                        Panel base: Trace + Title
   │  ├─ OrdersPage.cs · .Designer.cs         OrdersForm ported (grid + detail panel, using-block edit, Toast)
   │  ├─ CustomersPage.cs · .Designer.cs      customer grid — explicit columns, TaxId never bound
   │  └─ ReportsPage.cs · .Designer.cs        placeholder report list (Module 6)
   ├─ Dialogs/
   │  └─ EditOrderDialog.cs · .Designer.cs    the modal dialog as Wisej.Web.Form + LiveInstances counter + ResetState()
   ├─ Services/TabOrderWalker.cs              tab-order walk for the progress button
   ├─ Domain/                                 Order · OrderStore · OrderService · OrderValidator · OrderQuery · User (reused as-is)
   ├─ Shared/                                 TracePanel · Palette · Notify (Toast / AlertBox / Confirm)
   └─ docs/
      ├─ navigation-map.md · modal-dialog-disposal.md · notification-policy.md · before-after.md
      └─ migration-log.md
```

## Deliverables

1. **Ported navigation + list screen** — [`OrderDesk.Web/docs/navigation-map.md`](OrderDesk.Web/docs/navigation-map.md) (shell tree, WinForms → Wisej mapping, every entry point's trace lines)
2. **An edit dialog with disposal** — [`OrderDesk.Web/docs/modal-dialog-disposal.md`](OrderDesk.Web/docs/modal-dialog-disposal.md) (`Dialogs/EditOrderDialog.cs`, the using block in `Pages/OrdersPage.cs`, the leak/fix evidence)
3. **A Toast confirmation** — [`OrderDesk.Web/docs/notification-policy.md`](OrderDesk.Web/docs/notification-policy.md) (which MessageBoxes stay, `Shared/Notify.cs`)
4. **Before/after** — [`OrderDesk.Web/docs/before-after.md`](OrderDesk.Web/docs/before-after.md) (control by control; text stands in for screenshots)
5. **Running migration log** — [`OrderDesk.Web/docs/migration-log.md`](OrderDesk.Web/docs/migration-log.md) (Modules 1–3)

## Self-check answers

- **When should an informational "Saved" MessageBox be replaced?** When it does not require a blocking decision and an
  AlertBox/Toast improves the workflow. "Saved." asks nothing of the user, so it costs a click for no information the Toast
  cannot give — `Notify.Saved` replaces it. A validation failure or an "Are you sure?" still needs the user to act, so it stays modal.
- **What is the disposal concern for modal dialogs after migration?** Closed dialogs are not automatically disposed just
  because they close: Wisej.NET keeps a `ShowDialog` instance so it can be reused, and on a server the session (not a
  process exit) holds it. The caller must dispose it — `using (var dlg = new EditOrderDialog(order)) { … }` — or `ResetState()`
  and reuse it deliberately. The app shows five leaked instances (`LiveInstances = 5`, `FindComponents` = 5) and the fix bringing them back to 0.
- **Which layout features are useful when preserving a WinForms screen?** Docking, anchoring, containers, tab order and
  the client-profile preview (the knowledge-check answer key marks "Only absolute coordinates", which is the distractor — the
  lesson text says the opposite). Here Anchor became Dock, the `TabIndex` values were kept, and the walk button proves the order.
- **What is a good early UI strategy?** Parity first, controlled modernization second: the row → modal → save workflow was
  kept exactly; only the confirmation (Toast) and the disposal changed in this module.
- **What is the main outcome of Module 3?** Preserve the WinForms user workflow while adapting desktop UI patterns to browser behaviour.
- **Which activity matches the hands-on lab?** Port navigation and three screens, then replace one unnecessary MessageBox with a non-blocking notification.
- **Learning objectives** — map Form/UserControl/menu/toolbar/tab/MDI patterns to Wisej.NET equivalents (see `navigation-map.md`);
  use docking, anchoring, containers, document outline, tab order and designer profiles to preserve usability (`before-after.md`);
  use modal dialogs safely and dispose them when not reused (`modal-dialog-disposal.md`); replace confirmation MessageBoxes with
  AlertBox/Toast when no blocking response is needed (`notification-policy.md`).
- **True/false facts** — transient dialogs should be disposed when closed if not reused: **true**. Informational MessageBoxes are
  Toast/AlertBox candidates, decision dialogs may stay modal: **true**. Docking/anchoring/containers/document outline/client-profile
  preview help preserve layouts: **true**. Preserve business logic and workflow before modernizing: **true**. Standard controls move
  from `System.Windows.Forms` to `Wisej.Web`: **true** (properties still need compiler-guided review — e.g. `ClientSize`,
  `ISupportInitialize`, `MainMenuStrip` went away here). Static fields are a safe place for per-user state: **false** (Module 4).
  Registry and local paths must be replaced: **true** (Modules 4 and 6). First milestone = complete UI redesign: **false** — parity.
  A vertical slice should prove startup, navigation, data, modal workflow, files/reports and session context: **true**. Server-side
  Office COM is the recommended report path: **false**.
- **Pause & predict 1 — Why can repeatedly creating modal dialogs leak server-side objects?** Every `new EditOrderDialog()` is a
  server-side object graph (form, combos, buttons) registered in the browser's session; `ShowDialog` deliberately does not dispose
  it on close so it can be shown again. Without `Dispose`, each double-click adds another instance that lives as long as the session
  (or until the GC gets to unreferenced ones) — on a desktop the process exit hid this, on a server sessions live for hours and there
  are many of them. `Leak dialogs ✕` shows it; `Dispose fix ✓` shows the using block draining it.
- **Pause & predict 2 — Which confirmations deserve a block, and which deserve a Toast?** Block when the user must decide or must
  fix something before continuing: "Cannot save" validation, "Delete this order?", "Excel is not available — write CSV instead?".
  Toast (or AlertBox) when the message only informs: "Order 1042 saved.", "Exported.", "Signed in as kelly". The table in
  `notification-policy.md` classifies every MessageBox of the legacy app.

## Notes for the reviewer

Compile-checked against Wisej.Framework.xml but **not yet run** in this module (everything else follows verified shapes from the cookbook):

- `Form.ShowDialog()` blocking the handler and returning `DialogResult` — the trace lines before and after `dlg.ShowDialog()`
  in `Pages/OrdersPage.cs` prove it either way; if it did not block, the `← dlg.ShowDialog() returned` line would appear before
  the dialog is used and the fallback would be `ShowDialog((form, result) => …)`.
- `ShowDialog((form, result) => { })` + immediate `Close()` in the same handler (the leak/fix loops) and `Application.FindComponents`
  as the session registry count. If `FindComponents` reports 0 while `LiveInstances` is 5, the banner still shows the leak
  (`LiveInstances`) and `Dispose fix ✓` still drains it through the lab's own list of leaked instances.
- `Application.OpenForms.Count` for closed dialogs (expected to exclude them; it is logged as a non-detector).
- `MenuBar`/`MenuItem` and `StatusBar`/`StatusBarPanel` (`ShowPanels`, `AutoSize = Spring`) docked inside a Panel; `Button.PerformClick()`
  for the **Edit › New order…** menu; the semi-transparent highlight Panel (`Color.FromArgb(90, …)`) and `Control.Focus()` in the tab walk;
  `Toast` from `Shared/Notify.cs` (cookbook shape).
- `EditOrderDialog.LiveInstances` is process-wide: a second tab shares the number (by design; Module 4 explains). The `OrderStore`
  is process-wide too: saved edits (1042 → InProgress, a new 1043) persist until the app restarts.
- The `Cancel` button closes through its own `Click` handler (`DialogResult = Cancel; Close()`) rather than `Button.DialogResult`,
  to keep the close deterministic.
