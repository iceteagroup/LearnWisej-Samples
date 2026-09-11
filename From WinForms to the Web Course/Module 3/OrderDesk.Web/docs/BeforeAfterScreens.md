# Lab step · Before / after screens

No screenshots are stored in the repository; this is the written record the lab step asks for, with the layout of
each screen as it renders. "Before" is LegacyOrderDesk on a 1024×768 desktop; "after" is OrderDesk.Web in the
browser at `http://localhost:5603`.

## 1 · The main shell

**Before — `OrdersForm` (716×372, fixed, centred on the screen)**

```
┌ LegacyOrderDesk — Orders ───────────────────────────────────────────── _ □ ✕ ┐
│ File   View   Reports   Help                                                  │  MenuStrip
├───────────────────────────────────────────────────────────────────────────────┤
│ ┌ ordersGrid (12,36) 460×300, Anchor TBLR ───────┐ ┌ Order 1042 ───────────┐ │
│ │ Order  Customer            Total      Status   │ │ Customer  Northwind…  │ │  GroupBox detailGroup
│ │ 1042   Northwind Traders   $4,820.00  Open     │ │ PO #      NW-88231    │ │  (484,36) 220×300
│ │ 1041   Contoso Ltd         $1,290.50  Shipped  │ │ Lines     2 items     │ │  Anchor Top|Bottom|Right
│ │ 1040   Fabrikam Inc          $760.00  Open     │ │                       │ │
│ │ 1039   Adventure Works    $12,400.00  Invoiced │ │ [ New Order        ]  │ │
│ │ 1038   Globex Corp         $3,090.00  Hold     │ │ [ Print Invoice    ]  │ │
│ │                                                │ │ [ Export to Excel  ]  │ │
│ │                                                │ │ [ Attach file…     ]  │ │
│ └────────────────────────────────────────────────┘ └───────────────────────┘ │
│ Ready · 5 orders · user kelly · single-user desktop                           │  StatusStrip
└───────────────────────────────────────────────────────────────────────────────┘
```

Window size restored from `HKCU\Software\LegacyOrderDesk`; the filter (View › Open orders only) lived in the static
`AppState.CurrentFilter`; every action was a button in the detail box; Settings was a modal Form.

**After — `AppShell` in the browser tab (MenuBar · ToolBar · screen host · StatusBar, all docked)**

```
┌ OrderDesk ────────────────────────────────────────────────────────────────────────────────────────────┐
│ View   Reports   Help                                                                                 │  MenuBar (Dock Top)
│ [Orders] [Customers] [Reports] │ [New Order] [Print Invoice] [Export]                                 │  ToolBar (Dock Top)
├───────────────────────────────────────────────────────────────────────────────────────────────────────┤
│ Orders · all                                                             ┃ Order 1042                 │  labelHeading (Dock Top)
│ Order  Customer            Total      Status                             ┃ Customer  Northwind Traders│  panelDetail (Dock Right, 220)
│ 1042   Northwind Traders   4,820.00   Open       ◄ double-click = edit   ┃ PO #      NW-88231         │
│ 1041   Contoso Ltd         1,290.50   Shipped                            ┃ Owner     dana             │
│ 1040   Fabrikam Inc          760.00   Open                               ┃ Lines     2 items · 14 u.  │
│ 1039   Adventure Works    12,400.00   Invoiced                           ┃ Total     4,820.00         │
│ 1038   Globex Corp         3,090.00   Hold                               ┃ [ Edit…            ]       │
│                                                                          ┃ [ New Order        ]       │
│   gridOrders (Dock Fill)                                                 ┃ [ Print Invoice    ]       │
│                                                                          ┃                            │
├───────────────────────────────────────────────────────────────────────────────────────────────────────┤
│ Orders · 5 orders · filter all                                                                        │  StatusBar (Dock Bottom)
└───────────────────────────────────────────────────────────────────────────────────────────────────────┘
```

No fixed size anywhere; the filter is a field of the screen (one per session); Settings, Exit and Attach file are
not ported yet; the three screens swap inside the same host. Totals are `N2` — the server does not pick the user's
currency symbol.

**Customers** (ToolBar › Customers): heading `Customers`, a docked grid with the 8 customers (Adventure Works …
Wide World Importers, Tailspin Toys showing `5 %`).

**Reports** (ToolBar › Reports): the same five orders on the left, a right panel with
`[ Print Invoice (PDF) ]` and `[ Export orders ⬇ ]`.

## 2 · The edit dialog

**Before — `EditOrderDialog` (WinForms, FixedDialog, CenterParent, 392×216)**

```
┌ Edit Order 1042 ───────────────────────── ✕ ┐
│ Customer  [ Northwind Traders          ▾ ]  │   TabIndex 0
│ Owner     [ dana                         ]  │   TabIndex 1
│ Status    [ Open                       ▾ ]  │   TabIndex 2
│ PO #      [ NW-88231                     ]  │   TabIndex 3
│                       [ Cancel ] [ Save ]   │   5 / 4 (AcceptButton)
└─────────────────────────────────────────────┘
      ↓ Save
┌ LegacyOrderDesk ───────┐
│  Saved.                │   ← MessageBox.Show("Saved.") — one more click, the whole window masked
│              [  OK  ]  │
└────────────────────────┘
```

**After — `Dialogs/EditOrderDialog` (Wisej.Web.Form, FormBorderStyle.Fixed, CenterParent, 392×224)**

```
┌ Edit Order 1042 ───────────────────────── ✕ ┐   modal over the page (ShowDialogAsync — the server is not waiting)
│ Customer  [ Northwind Traders          ▾ ]  │   same fields, same TabIndex, same AcceptButton/CancelButton
│ Owner     [ dana                         ]  │
│ Status    [ Open                       ▾ ]  │
│ PO #      [ NW-88231                     ]  │
│                       [ Cancel ] [ Save ]   │
└─────────────────────────────────────────────┘
      ↓ Save                                              ┌──────────────────────────┐
  dialog closes → OrderService.Save → grid reloads →      │ ✓ Order 1042 saved.      │  ← Toast, top-right, gone in 4 s
  Dispose (using block)                                   └──────────────────────────┘
```

The validation message (`Select a customer.`) is the one MessageBox that stays modal in the dialog.

## Evidence

The layouts above are what `Shell/AppShell.Designer.cs`, `Screens/*.Designer.cs` and
`Dialogs/EditOrderDialog.Designer.cs` produce; the WinForms originals are under `Legacy/WinForms/`. The
"Fixed layout vs Dock/Anchor" table in `NavigationPort.md` lists the exact coordinates that were replaced by
`Dock`/`Anchor`.
