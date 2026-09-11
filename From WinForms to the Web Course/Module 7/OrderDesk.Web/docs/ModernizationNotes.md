# Modernization notes — what changed on the Orders screen after parity, and what did not

Rule of the module: **never change visual design and business behaviour in the same step** — a bug hides where you
cannot see it. Modules 1–6 reached parity (same workflow, same totals, same files) with the desktop look. Module 7
touches only the shell of the migrated Orders screen; `Domain/` is byte-for-byte the Module 1 copy.

## The modernization toolkit, applied

| Toolkit item | LegacyOrderDesk (`OrdersForm`) | OrderDesk.Web Module 7 | Where |
|---|---|---|---|
| **Theme** | Windows visual styles, one look | `Default.json` `"theme": "Bootstrap-4"` — the house style, loaded for every session | `Default.json` |
| **Mixin** | n/a | `Themes/orderdesk.mixin.theme` (button and toolbar-button radius 14, an `orderdesk-accent` colour) merged over the active theme at startup — every `Themes/*.mixin.theme` file is applied by the framework | `Themes/`, csproj copies the folder to the output |
| **Label wrappers** | `Label` + `TextBox` pairs ("Search:") | dropped: the label text moved into the control | — |
| **Watermarks** | none | `textSearch.Watermark = "Search orders (customer, id, PO)…"`, `comboActions.Watermark = "Actions…"` | `MainPage.Designer.cs` |
| **Tool buttons** | four push buttons under the grid (New Order, Print Invoice, Export, Attach) | one `ToolBar` with four `ToolBarButton`s (`toolNew`, `toolPrint`, `toolExport`, `toolRefresh`); `ButtonClick` dispatches by `e.Button.Name`; on phones the toolbar collapses into a `ComboBox` | `toolBar_ButtonClick`, `comboActions_SelectedIndexChanged` |
| **AlertBox / Toast** | `MessageBox.Show("Order saved")` (blocking) | `Ui.Toast(...)` (`AlertBox`, top-right, auto-close) | `NewOrder()` |
| **Responsive properties** | fixed 1024×768 form | `ClientProfiles.json` + `ResponsiveLayout` (three layouts) | `docs/ResponsiveProfiles.md` |

## What did **not** change

- `OrderService.Save` / `CalculateOrderTotal` / `Search` / `InvoiceDocument.Build` — **New Order** still produces the
  1,280.00 Litware order of Module 1; the search box calls the same `OrderService.Search(OrderFilter)` the desktop used.
- The workflow order: search → select → act; Print still opens the server PDF in a `PdfViewer` (Module 1/6), Export
  still streams a file built on the server to the browser.
- The data: 1042 Northwind Traders 4,820.00 Open · 1041 Contoso Ltd 1,290.50 Shipped · 1040 Fabrikam Inc 760.00
  Open · 1039 Adventure Works 12,400.00 Invoiced · 1038 Globex Corp 3,090.00 Hold.

## Why `Application.LoadTheme` is a deployment decision, not a user preference

`LoadTheme` replaces the theme object shared by every session in the process: calling it from one browser restyles
every other tab as well. That is right for a house style and wrong for a "dark mode" toggle — a per-user preference
would be a per-control `AppearanceKey`/style or a per-session CSS class, not `LoadTheme`. The sample therefore sets
the theme in `Default.json` and does not offer a per-user theme switch.

## Evidence (in the running app)

- The page opens in Bootstrap-4 with the mixin's rounded buttons and toolbar buttons.
- **New Order** (toolbar): order 1043 for 1,280.00 at the top of the grid and a toast top-right — restyling changed no
  behaviour.
