# Deliverable 1 · Ported navigation and list screen — MenuStrip/ToolStrip → MenuBar/ToolBar/Page

The main shell of LegacyOrderDesk (`OrdersForm`: MenuStrip, four action buttons, a StatusStrip, one grid) becomes
`Shell/AppShell` (a `UserControl` with a `MenuBar`, a `ToolBar`, a content host and a `StatusBar`) hosted by the
`MainPage : Page`. The three screens (`Screens/OrdersScreen`, `CustomersScreen`, `ReportsScreen`) are
`UserControl`s the shell creates once and swaps with `NavigateTo(string)`.

## Control mapping

| WinForms (`Legacy/WinForms/OrdersForm.Designer.cs`) | Wisej.NET (`Shell/AppShell.Designer.cs`) | Note |
|---|---|---|
| `Form OrdersForm` (ClientSize 716×372, CenterScreen, MainMenuStrip) | `Page MainPage` + `UserControl AppShell` | A Page fills the browser tab; there is no screen to centre on and no `MainMenuStrip` — the bar is docked |
| `MenuStrip menuStrip` | `MenuBar menuBar` (`Dock = Top`) | View · Reports · Help (File is not ported, see below) |
| `ToolStripMenuItem` | `MenuItem` (the View menu gained Orders / Customers / Reports) | `Text` with `&` mnemonics unchanged; `"-"` is a separator |
| `viewMenu.DropDownItems.AddRange(…)` | `menuView.MenuItems.AddRange(…)` | the only rename in the menu code |
| `menuStrip.Items.AddRange(…)` | `menuBar.MenuItems.AddRange(…)` | |
| `Button newOrderButton / printInvoiceButton / exportButton / attachButton` in the detail `GroupBox` | `ToolBar toolBar` (`Dock = Top`) with `ToolBarButton` Orders · Customers · Reports ‖ New Order · Print Invoice · Export; the Orders screen keeps its own buttons in the detail panel | Actions reachable from every screen live in the bar; screen-local ones stay on the screen |
| `StatusStrip statusStrip` + `ToolStripStatusLabel statusLabel` | `StatusBar statusBar` + `StatusBarPanel statusPanel` (`ShowPanels = true`, `AutoSize = Spring`) | a `StatusBar` shows its `Text` unless `ShowPanels` is on |
| `GroupBox detailGroup` (Anchor Top\|Bottom\|Right) | `Panel panelDetail` (`Dock = Right`, 220) | |
| `DataGridView ordersGrid` (Location 12,36 · Size 460×300 · Anchor TBLR, `DataSource` + `DataPropertyName`) | `DataGridView gridOrders` (`Dock = Fill`, rows added from `OrderService`, `Row.Tag = order`) | `"C2"` became `"N2"`: the server's culture is not the user's, so the server does not pick a currency symbol |
| modal `SettingsForm` (File › Settings…) | not ported (left out of the menu); becomes a per-user settings screen in Module 4 | HKCU on the server is the service account's registry |
| `Close()` (File › Exit) | not ported (left out of the menu); becomes sign-out in Module 4 | there is no process to end |
| `Button attachButton` | not ported; Module 6 replaces it with an `Upload` control | the user's disk is not reachable from the server |
| `MessageBox.Show(…, "About")` | `Ui.Toast(…)` | see `NotificationsReview.md` |
| MDI / `IsMdiContainer` | — (LegacyOrderDesk had none) | would become `TabControl` pages or a screen host like this one |
| `TabControl`, `UserControl`, `SplitContainer` | same names in `Wisej.Web` | direct-port |

## Screen hosting — `AppShell.NavigateTo`

```
NavigateTo("Customers")
  ├─ GetScreen: first time → new CustomersScreen { Dock = Fill, Visible = false }, subscribe StatusChanged, add to screenHost
  ├─ hide the current screen (Visible = false — never Dispose), show the target, BringToFront
  ├─ target.OnShown(first)   (Orders/Customers load their grid on the first show; Reports reloads every time)
  └─ StatusBar ← "Customers · 8 customers"
```

One instance per screen for the life of the shell, which is the life of the browser session: navigating back
to Orders reuses the grid (no second `OrderService.GetOrders`). On the desktop each screen was a Form the operating
system kept apart; here the shell is the only owner, and it has to be explicit about it.

## Tab order

Set explicitly, as on the desktop, in the designer files:

- `OrdersScreen`: `gridOrders` 0 → `buttonScreenEdit` 1 → `buttonScreenNewOrder` 2 → `buttonScreenPrint` 3.
- `EditOrderDialog`: `customerComboBox` 0 → `ownerTextBox` 1 → `statusComboBox` 2 → `poTextBox` 3 → `saveButton` 4 (`AcceptButton`) → `cancelButton` 5 (`CancelButton`).
- The `MenuBar` and `ToolBar` are reached with the keyboard through their own mnemonics (`&View`, …), not the tab chain.

## Fixed layout vs Dock/Anchor

| | Before (`OrdersForm`) | After (`OrdersScreen`) |
|---|---|---|
| Window | `ClientSize = 716×372`, `MinimumSize = 640×360`, designed for a 1024×768 desktop; size restored from HKCU | no size at all: the screen is `Dock = Fill` in the host, the host `Dock = Fill` in the shell, the shell `Dock = Fill` in the page (the whole browser tab) |
| Grid | absolute `Location (12,36)`, `Size 460×300`, `Anchor Top\|Bottom\|Left\|Right` | `Dock = Fill` |
| Detail | `GroupBox` at `(484,36)` 220×300, `Anchor Top\|Bottom\|Right` — pinned to an edge that only exists at 716 px | `Panel`, `Dock = Right`, `Width = 220`; the buttons inside are `Anchor Top\|Left\|Right` so a wider panel widens them |
| Heading / filter | none (the filter lived in a static) | `Label`, `Dock = Top`, shows the filter the View menu applied |
| Menu / status | docked by the designer | docked by the designer (`menuBar` Top, `toolBar` Top, `statusBar` Bottom) |

Dock order is the WinForms rule: docking is applied in reverse order of the `Controls` collection, so the
designer adds the `Fill` control first and the edge bars after it (`Controls.Add(screenHost); Add(toolBar);
Add(menuBar); Add(statusBar)`). Get it wrong and the toolbar sits under the grid.

## Designer best practice (the storyboard's four rules)

1. **Confirm in the designer**: layout, docking, anchoring, tab order, control hierarchy. Every `.Designer.cs` here
   is a plain `InitializeComponent()` with absolute `Location/Size`, `Dock`, `Anchor`, `TabIndex` and event hookups,
   so the Wisej Designer can open it.
2. **Keep `.Designer.cs` designer-owned.** The generated files contain only `InitializeComponent`, the component
   container, `Dispose(bool)` and the field declarations.
3. **Migration logic in partial classes / helpers**: `AppShell.cs` (navigation, screen cache), `OrdersScreen.cs`
   (`EditOrder`, the filter), `ScreenBase.cs` (what every screen shares).
4. **Document control replacements**: the mapping table above records every swap, and the WinForms originals sit
   under `Legacy/WinForms/` (excluded from compilation) for side-by-side reading.

## Evidence (in the running app)

- Page load: the shell opens on the Orders screen with the five orders; the StatusBar reads `Orders · 5 orders · filter all`.
- ToolBar **Customers** (`toolCustomers`) shows the 8 customers; ToolBar **Orders** again shows the same grid instance
  (no second `GetOrders`).
- Menu **View › Open orders only** (`menuViewOpenOnly`): the grid shows 2 orders and the heading reads
  `Orders · Open only`. **All orders** restores 5.
- Resize the browser tab: the grid and the detail panel are docked, so they follow the tab's width and height.
