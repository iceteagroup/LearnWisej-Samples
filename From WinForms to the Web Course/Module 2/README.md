# OrderDesk.Web · From WinForms to the Web · Module 2

Local lab build for **Module 2 · Project Conversion and Wisej.NET Startup**. A clean **Wisej.NET 4 shell** is created
on the migration branch (the WinForms original **LegacyOrderDesk** stays in the solution untouched), the first form
(`OrdersForm` + its `EditOrderDialog`) is copied in, `System.Windows.Forms` becomes `Wisej.Web`, and the compiler
errors are categorized, fixed and rebuilt (15 → 24 → 10 → 0) until the form runs in the browser. App.config's
connection string moves to `Web.config`.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/From WinForms to the Web Course/Module 2/OrderDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5602
```

Then open <http://localhost:5602>. (Visual Studio: open `OrderDesk.slnx`, set `OrderDesk.Web` as the startup project, F5.
The solution also contains `LegacyOrderDesk`, the WinForms original, which runs on Windows only.)

## What to try

| Action | What you should see |
|---|---|
| Page load | `Program.Main` shows the ported **Orders** form: menu File/View/Reports/Help, the grid with 1042 Northwind Traders $4,820.00 Open first, the Order detail group and the status bar `Ready · 5 orders` |
| Double-click a row | `EditOrderDialog` opens modally; **Save** closes it, `MessageBox.Show("Saved.")` still works, the grid reloads. The result arrives in the `ShowDialog` callback and the caller disposes the dialog |
| **New Order** | The same dialog for a new Northwind order |
| **View › Open orders only / All orders** | Filters the grid through the reused `OrderService.Search` |
| **Print Invoice**, **Export to Excel**, **Attach file…**, **File › Settings…** | Not ported yet (desktop-only operations): a "not available in the web version yet" message. They are the open items in `docs/CompilerErrorLog.md` |

## Where things live

```
Module 2/
├─ OrderDesk.slnx                   both projects
├─ LegacyOrderDesk/                 the WinForms original, untouched
└─ OrderDesk.Web/                   the Wisej.NET 4 shell (net10.0-windows;net10.0)
   ├─ Default.html / Default.json / Program.cs / Startup.cs / Web.config   the shell files
   ├─ OrderDesk.Web.csproj          Microsoft.NET.Sdk.Web + PackageReference Wisej-4 4.1.0
   ├─ OrdersForm.cs / .Designer.cs  the ported form: still a Form; MenuBar/StatusBar substitutions
   ├─ Dialogs/EditOrderDialog.cs    ported unchanged (the caller changed: ShowDialog callback)
   ├─ Domain/                       the reused business logic
   ├─ Legacy/AppState.cs            the desktop statics the ported form still reads (Module 4)
   ├─ Views/Ui.cs                   toast helper
   └─ docs/                         the lab deliverables
```

## Deliverables

1. **The Wisej.NET shell** (project file, package ref, startup files): [`OrderDesk.Web/docs/ShellAnatomy.md`](OrderDesk.Web/docs/ShellAnatomy.md)
2. **One form moved and building, errors categorized**: [`OrderDesk.Web/docs/CompilerErrorLog.md`](OrderDesk.Web/docs/CompilerErrorLog.md); the form is [`OrdersForm.cs`](OrderDesk.Web/OrdersForm.cs) + [`OrdersForm.Designer.cs`](OrderDesk.Web/OrdersForm.Designer.cs); why it stays a `Form`: [`docs/FormVsPage.md`](OrderDesk.Web/docs/FormVsPage.md)
3. **Migration log updated**: [`OrderDesk.Web/docs/migration-log.md`](OrderDesk.Web/docs/migration-log.md)

## Self-check answers (lab guide + video)

- **Which business logic was reused as-is?** `Domain/` again, verbatim, and the form's logic too: every handler in
  `OrdersForm.cs` (`ReloadGrid` → `OrderService.Search`, `newOrderButton_Click` → `OrderService.Save`, the
  `EditOrderDialog` validation) is the same C# it was on the desktop. The namespace swap touched `using` lines and
  designer types, not behaviour.
- **Which desktop boundary was replaced with a web-safe pattern?** Startup (`Application.Run` → `Default.json` +
  `Program.Main` + `Startup.cs`) and configuration (`App.config` → `Web.config`). The other boundaries
  (`RegistrySettings`, `InvoicePrinter`, `ExcelExport`, `OpenFileDialog`, `SettingsForm`) are stubbed and listed as
  open items for Modules 4 and 6.
- **How is per-user state kept out of static fields?** Not yet, on purpose: `AppState.CurrentUser / CurrentFilter /
  LastSearch` compile unchanged and are on the open-items list (Module 4). The WinForms `LoginForm` that filled the
  static was not copied.
- **What was tested before calling the migrated feature complete?** Builds on both target frameworks; the form opened in
  the browser with the five orders, the menu, the status bar and the detail panel; double-click → edit → Save
  round-trips through the callback. Compiling was explicitly *not* the finish line.
- **A form compiles but won't launch — where do you look first?** `Default.json`: is `"startup"` (or `"mainWindow"`)
  the right `Type.Method, Assembly` / `Type, Assembly`? Then `Program.Main`: does it set `Application.MainPage` or
  `Show()` a form? Then the browser console with `"debug": true`.
- **Where do connection strings move when App.config is gone?** To the web host's configuration: `Web.config`
  `<connectionStrings>` (or `appsettings.json`), read from the content root, not `<exe>.config` next to the binary.

## Runtime facts

- There is no `AddWisej()` in Wisej-4 4.1.0 (the video shows one); `app.UseWisej()` is the only middleware registration.
- A `Form` shown with `Show()` floats in the browser and is disposed by Wisej.NET when it closes; a `Form` shown with `ShowDialog(callback)` is disposed by the caller. `ShowDialog` never blocks.
- The projects multi-target `net10.0-windows;net10.0`, so `dotnet run` needs `-f net10.0` (or `-f net10.0-windows`).
