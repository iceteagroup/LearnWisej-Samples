# Deliverable 1 · Ported navigation and list screen — MenuStrip/ToolStrip → MenuBar/ToolBar/Page

The main shell of LegacyOrderDesk (`OrdersForm`: MenuStrip, four action buttons, a StatusStrip, one grid) becomes
`Shell/AppShell` (a `UserControl` with a `MenuBar`, a `ToolBar`, a content host and a `StatusBar`) hosted by the
`MainPage : Page`. The three screens (`Screens/OrdersScreen`, `CustomersScreen`, `ReportsScreen`) are
`UserControl`s the shell creates once and swaps with `NavigateTo(string)`.

## Control mapping

| WinForms (`Legacy/WinForms/OrdersForm.Designer.cs`) | Wisej.NET (`Shell/AppShell.Designer.cs`) | Note |
|---|---|---|
| `Form OrdersForm` (ClientSize 716×372, CenterScreen, MainMenuStrip) | `Page MainPage` + `UserControl AppShell` | A Page fills the browser tab; there is no screen to centre on and no `MainMenuStrip` — the bar is docked |
| `MenuStrip menuStrip` | `MenuBar menuBar` (`Dock = Top`) | same four top-level items File · View · Reports · Help |
| `ToolStripMenuItem` (×10) | `MenuItem` (×14: the View menu gained Orders / Customers / Reports) | `Text` with `&` mnemonics unchanged; `"-"` is a separator |
| `fileMenu.DropDownItems.AddRange(…)` | `menuFile.MenuItems.AddRange(…)` | the only rename in the menu code |
| `menuStrip.Items.AddRange(…)` | `menuBar.MenuItems.AddRange(…)` | |
| `Button newOrderButton / printInvoiceButton / exportButton / attachButton` in the detail `GroupBox` | `ToolBar toolBar` (`Dock = Top`) with `ToolBarButton` Orders · Customers · Reports ‖ New Order · Print Invoice · Export; the Orders screen keeps its own buttons in the detail panel | Actions reachable from every screen live in the bar; screen-local ones stay on the screen |
| `StatusStrip statusStrip` + `ToolStripStatusLabel statusLabel` | `StatusBar statusBar` + `StatusBarPanel statusPanel` (`ShowPanels = true`, `AutoSize = Spring`) | a `StatusBar` shows its `Text` unless `ShowPanels` is on |
| `GroupBox detailGroup` (Anchor Top\|Bottom\|Right) | `Panel panelDetail` (`Dock = Right`, 220) | |
| `DataGridView ordersGrid` (Location 12,36 · Size 460×300 · Anchor TBLR, `DataSource` + `DataPropertyName`) | `DataGridView gridOrders` (`Dock = Fill`, rows added from `OrderService`, `Row.Tag = order`) | `"C2"` became `"N2"`: the server's culture is not the user's, so the server does not pick a currency symbol |
| modal `SettingsForm` (File › Settings…) | not ported: a boundary trace + Toast; becomes a per-user settings screen in Module 4 | HKCU on the server is the service account's registry |
| `Close()` (File › Exit) | not ported: a boundary trace + Toast; becomes sign-out in Module 4 | there is no process to end |
| `MessageBox.Show(…, "About")` | `Ui.Toast(…)` | see `NotificationsReview.md` |
| MDI / `IsMdiContainer` | — (LegacyOrderDesk had none) | would become `TabControl` pages or a screen host like this one |
| `TabControl`, `UserControl`, `SplitContainer` | same names in `Wisej.Web` | direct-port |

## Screen hosting — `AppShell.NavigateTo`

```
NavigateTo("Customers")
  ├─ GetScreen: first time → new CustomersScreen { Dock = Fill, Visible = false }, subscribe Trace/StatusChanged, add to screenHost
  ├─ hide the current screen (Visible = false — never Dispose), show the target, BringToFront
  ├─ trace   ← JS→.NET navigate Customers → created CustomersScreen in screenHost (Dock = Fill)
  ├─ target.OnShown(first)   (Orders/Customers load their grid on the first show; Reports reloads every time)
  └─ StatusBar ← "Customers · 8 customers"
```

One instance per screen for the life of the shell, which is the life of the browser session: navigating back
to Orders reuses the grid (`… → reused OrdersScreen …` in the trace, no second `OrderService.GetOrders`). On the
desktop each screen was a Form the operating system kept apart; here the shell is the only owner, and it has to
be explicit about it.

Everything the shell and the screens do is raised as a `Trace` event (`Views/TraceEventArgs.cs`); `MainPage`
forwards it to the TracePanel. The shell does not reference the console — unsubscribe and it is the product.

## Tab order

Set explicitly, as on the desktop, in the designer files:

- `OrdersScreen`: `gridOrders` 0 → `buttonScreenEdit` 1 → `buttonScreenNewOrder` 2 → `buttonScreenPrint` 3 → `buttonScreenAttach` 4.
- `EditOrderDialog`: `customerComboBox` 0 → `ownerTextBox` 1 → `statusComboBox` 2 → `poTextBox` 3 → `saveButton` 4 (`AcceptButton`) → `cancelButton` 5 (`CancelButton`).
- The `MenuBar` and `ToolBar` are reached with the keyboard through their own mnemonics (`&File`, …), not the tab chain.

## Fixed layout vs Dock/Anchor

| | Before (`OrdersForm`) | After (`OrdersScreen`) |
|---|---|---|
| Window | `ClientSize = 716×372`, `MinimumSize = 640×360`, designed for a 1024×768 desktop; size restored from HKCU | no size at all: the screen is `Dock = Fill` in the host, the host `Dock = Fill` in the shell, the shell sized by its parent (616×396 in the console, the whole tab in the product) |
| Grid | absolute `Location (12,36)`, `Size 460×300`, `Anchor Top\|Bottom\|Left\|Right` | `Dock = Fill` |
| Detail | `GroupBox` at `(484,36)` 220×300, `Anchor Top\|Bottom\|Right` — pinned to an edge that only exists at 716 px | `Panel`, `Dock = Right`, `Width = 220`; the buttons inside are `Anchor Top\|Left\|Right` so a wider panel widens them |
| Heading / filter | none (the filter lived in a static) | `Label`, `Dock = Top`, shows the filter the View menu applied |
| Menu / status | docked by the designer | docked by the designer (`menuBar` Top, `toolBar` Top, `statusBar` Bottom) |

Dock order is the WinForms rule: docking is applied in reverse order of the `Controls` collection, so the
designer adds the `Fill` control first and the edge bars after it (`Controls.Add(screenHost); Add(toolBar);
Add(menuBar); Add(statusBar)`). Get it wrong and the toolbar sits under the grid.

Anchored controls are sized on the **server** against the browser size the client reported first: the console's
right column (trace + review card) is anchored; the left column stays absolute (x 30…670) so the shell always
has its 616 px.

## Designer best practice (the storyboard's four rules)

1. **Confirm in the designer**: layout, docking, anchoring, tab order, control hierarchy. Every `.Designer.cs` here
   is a plain `InitializeComponent()` with absolute `Location/Size`, `Dock`, `Anchor`, `TabIndex` and event hookups,
   so the Wisej Designer can open it.
2. **Keep `.Designer.cs` designer-owned.** The generated files contain only `InitializeComponent`, the component
   container and the field declarations. `EditOrderDialog.Dispose(bool)` moved out of the designer file into
   `EditOrderDialog.cs` because it carries migration logic (`DialogTracker`); the designer would regenerate it away.
3. **Migration logic in partial classes / helpers**: `AppShell.cs` (navigation, screen cache), `OrdersScreen.cs`
   (`EditOrder`, the filter), `ScreenBase.cs` (the shared `Trace` event), `MainPage.cs` (the console). The
   reusable-dialog release in `MainPage` uses the `Disposed` event instead of editing the generated `Dispose(bool)`.
4. **Document control replacements**: every swap is a `// ✓ was: …` comment on the line that changed
   (`AppShell.Designer.cs`, `OrdersScreen.Designer.cs`), and the WinForms originals sit next to them under
   `Legacy/WinForms/` (excluded from compilation) for side-by-side reading.

## Evidence (in the running app)

- Page load: trace `• server shell MenuStrip → MenuBar · buttons → ToolBar · StatusStrip → StatusBar · Form → screen host (Dock = Fill)`,
  then `← JS→.NET navigate Orders → created OrdersScreen in screenHost (Dock = Fill)` and
  `• server OrderService.GetOrders 5 orders (business logic reused, unchanged)`; the StatusBar reads
  `Orders · 5 orders · filter all · session xxxxxxxx`.
- ToolBar **Customers** (`toolCustomers`): `navigate Customers → created CustomersScreen …`,
  `• server CustomerService.GetCustomers 8 customers …`; ToolBar **Orders** again: `navigate Orders → reused OrdersScreen …`
  and no second `GetOrders`.
- Menu **View › Open orders only** (`menuViewOpenOnly`): `← JS→.NET View › filter status = Open (kept on this screen, not in a static)`,
  `• server OrderService.Search 2 orders · status = Open …`; the heading reads `Orders · Open only`. **All orders** restores 5.
- Menu **File › Settings…** / **File › Exit** (`menuFileSettings`, `menuFileExit`): a `⚠ boundary` line each and a warning Toast — the
  two desktop-only items are logged, not faked.
- Resize the browser tab: the shell keeps its place (absolute left column), the trace and the review card follow the
  right edge (anchored); inside the shell the grid and the detail panel are docked, so the console's 616 px and a
  full-width product page use the same designer file.
