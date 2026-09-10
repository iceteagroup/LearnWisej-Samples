# Modernization notes — what changed on the Orders screen after parity, and what did not

Rule of the module: **never change visual design and business behaviour in the same step** — a bug hides where you
cannot see it. Modules 1–6 reached parity (same workflow, same totals, same files) with the desktop look. Module 7
touches only the shell of the migrated Orders screen; `Domain/` is byte-for-byte the Module 1 copy.

## The modernization toolkit, applied

| Toolkit item | LegacyOrderDesk (`OrdersForm`) | OrderDesk.Web Module 7 | Where |
|---|---|---|---|
| **Theme** | Windows visual styles, one look | `Default.json` `"theme": "Bootstrap-4"`; `ThemeService.Apply("Material-3" / "FluentDark-5")` restyles every session live | `Services/ThemeService.cs`, buttons **Bootstrap-4 / Material-3 / FluentDark-5** |
| **Mixin** | n/a | `Themes/orderdesk.mixin.theme` (button and toolbar-button radius 14, an `orderdesk-accent` colour) merged over whichever theme is active; `ThemeService.ApplyMixin("orderdesk")` = `Application.LoadTheme(current, new[] { "orderdesk" })` | button **Apply orderdesk mixin**; `ButtonRadius()` reads the merged value back (`Application.Theme.GetStyle<object>("button", "radius", "default")`) |
| **Label wrappers** | `Label` + `TextBox` pairs ("Search:") | dropped: the label text moved into the control | — |
| **Watermarks** | none | `textSearch.Watermark = "Search orders (customer, id, PO)…"`, `comboActions.Watermark = "Actions…"` | `MainPage.Designer.cs` |
| **Tool buttons** | four push buttons under the grid (New Order, Print Invoice, Export, Attach) | one `ToolBar` with four `ToolBarButton`s (`toolNew`, `toolPrint`, `toolExport`, `toolRefresh`); `ButtonClick` dispatches by `e.Button.Name`; on phones the toolbar collapses into a `ComboBox` | `toolBar_ButtonClick`, `comboActions_SelectedIndexChanged` |
| **AlertBox / Toast** | `MessageBox.Show("Order saved")` (blocking) | `Ui.Toast(...)` (`AlertBox`, top-right, auto-close) — Module 3's rule, kept | `NewOrder()` |
| **Responsive properties** | fixed 1024×768 form | `ClientProfiles.json` + `ResponsiveLayout` (three layouts) | `docs/ResponsiveProfiles.md` |

## What did **not** change

- `OrderService.Save` / `CalculateOrderTotal` / `Search` / `InvoiceDocument.Build` — **New Order** still produces the
  1,280.00 Litware order of Module 1; the search box calls the same `OrderService.Search(OrderFilter)` the desktop used.
- The workflow order: search → select → act; Print still opens the server PDF in a `PdfViewer` (Module 1/6), Export
  still streams bytes through `Application.Download`.
- The data: 1042 Northwind Traders 4,820.00 Open · 1041 Contoso Ltd 1,290.50 Shipped · 1040 Fabrikam Inc 760.00
  Open · 1039 Adventure Works 12,400.00 Invoiced · 1038 Globex Corp 3,090.00 Hold.

## Why `Application.LoadTheme` is a deployment decision, not a user preference

`LoadTheme` replaces the theme object shared by every session in the process: pressing **Material-3** in one browser
restyles the other tab as well (open a second session and look). That is right for a house style and wrong for a
"dark mode" toggle — a per-user preference would be a per-control `AppearanceKey`/style or a per-session CSS class,
not `LoadTheme`. The console makes that visible on purpose.

## Evidence (in the running app, Orders tab)

- Page load: trace `• server theme Bootstrap-4 · button radius = <value> (Themes/orderdesk.mixin.theme merged at startup)`
  and `labelTheme` = `theme Bootstrap-4 · button radius <value> · mixin orderdesk.mixin.theme`.
- **Material-3**: trace `← JS→.NET theme Material-3`, `• server Application.LoadTheme Bootstrap-4 → Material-3 · button
  radius <before> → <after>`, `→ .NET→JS theme every session in this process is restyled — zero lines of form code
  changed`; the whole page (both columns, the second tab too) changes look; audit row `theme Bootstrap-4 → Material-3`.
- **Apply orderdesk mixin**: trace `• server Application.LoadTheme(mixins) Material-3 + [orderdesk] · button radius
  <before> → 14 (Themes/orderdesk.mixin.theme says 14)`; buttons and toolbar buttons get 14 px corners.
- **New Order** after any theme change: trace `• server OrderService.Save order 1043 · CalculateOrderTotal = 1,280.00
  (business logic reused, unchanged since Module 1)` — the proof that restyling changed no behaviour.
- The toolbar: trace `← JS→.NET toolBar.ButtonClick toolNew "New Order"`; the toast appears top-right instead of a
  modal MessageBox.
