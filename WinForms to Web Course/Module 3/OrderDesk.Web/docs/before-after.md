# Before / after — the ported screens, control by control (Module 3)

Text stands in for the lab's before/after screenshots. "Before" is `LegacyOrderDesk/OrdersForm.Designer.cs`
and `EditOrderDialog.cs` (Module 1 folder); "after" is this project.

## OrdersForm → OrdersPage

| Control | Before (WinForms) | After (Wisej.NET) | Why |
|---|---|---|---|
| the window | `OrdersForm : Form`, `ClientSize 720×341`, `MinimumSize 640×300`, `StartPosition CenterScreen`, `Text "LegacyOrderDesk — Orders · Kelly"` | `OrdersPage : ModulePage (Panel)`, `Dock = Fill` in `pageHost`, white card `BorderStyle.Solid` | a screen inside the shell, not a top-level window; size comes from the browser |
| `menuStrip` (`MenuStrip`, 5 `ToolStripMenuItem`) | on the form, `MainMenuStrip = menuStrip` | removed from the page → `MainPage.menuBar : MenuBar` with `MenuItem`s | one menu for the app, in the shell |
| `statusStrip` + `statusLabel` | on the form, `"Ready · N orders · single-user desktop"` | removed from the page → `MainPage.statusBar : StatusBar` with 3 `StatusBarPanel`s: `"Ready · 5000 orders · multi-user web · session xxxxxxxx"`, `"Dialogs alive: n"`, page name | status is app-wide; "single-user desktop" is no longer true |
| — | — | `headerLabel : Label`, `Dock Top 36`, blue `#1565d8`, white bold "Orders" | the page title the video shows; replaces the window caption |
| `ordersGrid` (`DataGridView`) | `Anchor Top\|Bottom\|Left\|Right`, `Location (0,27)`, `Size 496×292`, `DataSource = BindingList<Order>` | `Dock = Fill`, `DataSource = List<Order>` (`OrderService.GetAll()`), `BorderStyle None`, `TabIndex 0` | Dock instead of Anchor: fills whatever the browser gives; `BindingList` had no purpose without two-way binding |
| `colId / colCustomer / colTotal / colStatus` | `Order 70 · Customer Fill · Total 90 C2 right · Status 80` | `Order 70 · Customer Fill · Total 100 C2 right · Status 90` | identical column model (`Wisej.Web.DataGridViewTextBoxColumn`), widths nudged for the web font |
| `((ISupportInitialize)ordersGrid).BeginInit()/EndInit()` | present | removed | not needed by `Wisej.Web.DataGridView` |
| `ordersGrid_CellDoubleClick` | `new EditOrderDialog(…).ShowDialog(this)` → `MessageBox.Show("Saved.")` | `using (var dlg = new EditOrderDialog(…)) { if (dlg.ShowDialog() == OK) { Save; Notify.Saved; } }` | disposal + Toast (see the other two docs) |
| `detailPanel` (`Panel`) | `Anchor Top\|Bottom\|Right`, `Location (500,27)`, `Size 220×292`, `BorderStyle FixedSingle` | `Dock = Right`, `Width 220`, `BackColor #f4f6f9`, `BorderStyle None`, `TabIndex 1` | docks beside the grid at any width |
| `detailTitle / labelCustomer / labelPo / labelLines` | `AutoSize = true`, Segoe UI 9 bold title | `AutoSize = false` with explicit sizes, `"default"` font, muted colour `#5a6b7d` for captions | Wisej labels size explicitly in designer code; theme font instead of Segoe UI |
| `detailCustomer / detailPo / detailLines` (`TextBox`) | `ReadOnly`, `Width 192`, at y 58 / 106 / 154 | `ReadOnly`, `192×30`, at y 60 / 116 / 172, `TabIndex 0 / 1 / 2` | taller web inputs; TabIndex kept |
| `newOrderButton` | `192×26`, `Click → newOrderButton_Click` | `192×30`, `TabIndex 3`, same handler through the using block; Owner defaults to `"Kelly"` instead of `AppState.CurrentUser?.UserName` | `AppState` statics are not ported (Module 4) |
| `printInvoiceButton` | `192×26`, `InvoicePrinter.Print()` | `192×30`, `TabIndex 4`, **Enabled = false**, tooltip "PrintDocument prints on the SERVER — replaced by a PDF in Module 6" | desktop boundary, kept visible for parity |
| `exportButton` | `192×26`, Excel Interop → C:\Orders fallback | `192×30`, `TabIndex 5`, **Enabled = false**, tooltip "Excel Interop needs Excel on the SERVER — replaced by a managed .xlsx download in Module 6" | desktop boundary, kept visible for parity |
| `AppState.CurrentOrder = o; AppState.CurrentCustomer = o?.Customer;` in `ShowDetail()` | ✕ statics written on every selection | removed (comment left in place) | per-user state in statics is the Module 4 refactoring |

## EditOrderDialog (WinForms) → Dialogs/EditOrderDialog (Wisej.Web.Form)

| Control | Before | After | Why |
|---|---|---|---|
| the form | `Form`, `FixedDialog`, `CenterParent`, `ClientSize 380×260`, no designer file (controls built in the ctor) | `Wisej.Web.Form`, `FormBorderStyle.Fixed`, `StartPosition.CenterParent`, `Size 380×316`, `ShowInTaskbar = false`, `EditOrderDialog.Designer.cs` | designer-owned layout (video: keep `.Designer.cs` designer-owned; migration logic in the partial class) |
| `Customer / Owner / Status` combos | `ComboBox DropDownList`, width 340, built by `AddField(label, top, items, value)` | `comboCustomer / comboOwner / comboStatus`, `DropDownList`, `340×30`, `TabIndex 0 / 1 / 2`, filled in the ctor, `ResetState()` reloads them | same fields, same values (`OrderService.Customers`, `Owners`, `Enum.GetNames(OrderStatus)`) |
| captions | `Label AutoSize` "Customer" / "Owner" / "Status" | `Label` 340×18, bold 9, muted `#5a6b7d` | matches the video's field captions |
| `cancel` | `DialogResult = DialogResult.Cancel` (auto-close) | `btnCancel`, `TabIndex 4`, `Click → DialogResult = Cancel; Close()` | explicit close instead of relying on `Button.DialogResult` |
| `save` | `Click → Save_Click`, `AcceptButton` | `btnSave`, blue `#1565d8` / white, `TabIndex 3`, `AcceptButton`, `Click → btnSave_Click` | same validation, same `DialogResult.OK; Close()` |
| `Save_Click` validation | `MessageBox.Show(errors, "Cannot save", OK, Warning)` | unchanged (see `notification-policy.md`) | a decision is required — stays modal |
| lifetime | caller never disposed it | `Dispose(bool)` decrements `LiveInstances`; caller uses `using` | server-hosted: the caller owns the lifetime |

## Shell (new)

| Element | After | Replaces |
|---|---|---|
| `appBar` | blue bar, title "OrderDesk — Module 3 · Forms, Navigation, Layout & Modal Workflow", ● status | the window caption + the course look |
| `navPanel` | dark rail `#16142e`, "OrderDesk", Orders / Customers / Reports buttons (active = translucent blue, bold) | opening screens from the menu as separate Forms |
| `CustomersPage` | grid `Customer · Country · Tier · Credit limit`, footer "TaxId is sensitive and is not bound…" | the desktop customer Form (never ported TaxId) |
| `ReportsPage` | placeholder list of four reports + Module 6 note | Print Invoice / Export to Excel |
| `buttonBar` | the five lab buttons | — (lab prop) |
| `trace` | the migration trace | — (lab prop) |

## Evidence

- **Tab order / docking** walks the OrdersPage: one highlighted control every 0.7 s, in this order —
  `TabIndex=0 ordersGrid (DataGridView) Dock=Fill …`, `TabIndex=1 detailPanel (Panel) Dock=Right · 220×… …`,
  then inside it `  TabIndex=0 detailCustomer (TextBox) Anchor=Top, Left · 192×30 @ (…) · TabStop=True`, `  TabIndex=1 detailPo`, `  TabIndex=2 detailLines`,
  `  TabIndex=3 newOrderButton (Button)`, `  TabIndex=4 printInvoiceButton … · disabled`, `  TabIndex=5 exportButton … · disabled`,
  the four captions (`TabIndex=6..9`, `TabStop=False`), and finally `TabIndex=2 headerLabel (Label) Dock=Top`.
  Then `★ tab order preserved  TabIndex 0 ordersGrid → 1 detailPanel (0 Customer · 1 PO · 2 Lines · 3 New Order · 4 Print · 5 Export)`,
  `★ anchors → docking  OrdersForm anchored a 496×292 grid at a fixed 720×341 ClientSize; the page docks and fills whatever the browser gives`,
  `✓ Tab order / docking  walk complete · 13 controls`, and the green banner. Click the button again while it runs to restart the walk.
- Resize the browser pane: the grid and the detail panel follow (Dock), the trace keeps 560 px, nothing scrolls at 1400×760.
- The detail panel shows "Order 1042 · Northwind Traders · NW-88231 · 10 items · 1 line(s)" for the first row; selecting another row updates it and logs `← ordersGrid.SelectionChanged  Order 1041 · Contoso Ltd`.
