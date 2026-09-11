# Form or Page? — why the port stays a `Form` in this module

The lesson's namespace-replacement pattern keeps the class declaration familiar:

```csharp
// Before
using System.Windows.Forms;
// After
using Wisej.Web;
public partial class OrdersForm : Form { public OrdersForm() { InitializeComponent(); } }
```

The walkthrough video goes one step further in the same scene and shows

```csharp
public partial class OrdersPage : Page   // was : Form
```

**Both are valid Wisej.NET.** This module's sample keeps `OrdersForm : Form` on purpose; the video's `Page` variant
is one line away. This note says what each choice means so the decision is deliberate, not accidental.

## What the port did (this module)

`OrdersForm.cs` and `OrdersForm.Designer.cs` were copied from `LegacyOrderDesk`, `System.Windows.Forms` became
`Wisej.Web`, `namespace LegacyOrderDesk` became `namespace OrderDesk` — and **nothing else about the class changed**:
it is still `public partial class OrdersForm : Form`, `ClientSize = 716×372`, `MinimumSize`, `StartPosition =
CenterScreen`, a `MenuBar` docked top, a `StatusBar` docked bottom, the grid and the detail `GroupBox` anchored as before.
The compiler-driven edits were control substitutions inside the designer and the commented desktop calls (see
`CompilerErrorLog.md`). `Program.Main` opens it with `new OrdersForm().Show()`: a **floating window** in the browser,
with a title bar, movable and resizable, closable from its own `File › Exit`.

Why keep it a `Form` in Module 2:

1. **The diff stays honest.** The deliverable is "one form moved and building". Changing the base class would mix a
   design decision into the mechanical port and hide which errors the *namespace swap* produced.
2. **The Designer opens it as-is.** A Wisej.NET `Form` has the same designer surface a WinForms `Form` had — the
   `.Designer.cs` the video shows opening "intact" is the unchanged one.
3. **`ShowDialog` semantics are the next lesson.** `EditOrderDialog` is a `Form` shown modally from `OrdersForm`; the
   caller/owner relationship (`dialog.ShowDialog(this, callback)`) is easier to reason about when the owner is a form too.
4. **It is what a team really gets on day one.** Every ported screen starts as a `Form`; promoting the *main* screen to a
   `Page` (or hosting the forms in a `Desktop`) is the Module 3 navigation decision, not a Module 2 side effect.

## How it becomes a `Page` — in one line

```csharp
public partial class OrdersPage : Page   // was: OrdersForm : Form
```

and, instead of showing it, make it *the* page for the session:

```csharp
// Program.cs (startup)                       // or, with no Program.cs at all, in Default.json:
Application.MainPage = new OrdersPage();      // "mainWindow": "OrderDesk.OrdersPage, OrderDesk"
```

Things that stop mattering on a `Page` (the compiler or the designer will tell you): `ClientSize` / `MinimumSize` /
`StartPosition` / `Text` as a window title (a page fills the browser viewport and has no title bar), `FormClosing` /
`FormClosed` / `Close()` (a page is not closed; the session ends or `Application.MainPage` is replaced), `ShowDialog`
on the page itself. The `MenuBar`, `StatusBar`, grid, anchors and every handler stay exactly the same.

## The trade-offs

| | `OrdersForm : Form` (this module) | `OrdersPage : Page` (the video) |
|---|---|---|
| Looks like | a window over a page or desktop: title bar, move, resize, minimize/maximize, close | the whole browser tab, no chrome |
| Fits | MDI-style apps, tool windows, multi-window dashboards, a `Desktop` with a taskbar | one screen at a time, navigation by swapping `Application.MainPage` or by containers inside the page (`TabControl`, panels, `SplitContainer`) |
| Resize | the *user* resizes the window; `Dock`/`Anchor` do the rest | the *browser* resizes the page (`Application.BrowserSizeChanged`, responsive profiles in Module 7) |
| Lifetime | created and disposed per open; `Show()` disposes on close, `ShowDialog()` leaves disposal to the caller | one per session; replaced, not closed |
| Modal children | `ShowDialog(this, callback)` with the form as owner | `ShowDialog(callback)` from the page — same non-blocking rule |
| Startup | `Program.Main` → `new OrdersForm().Show()` or `Application.MainPage = new MainPage()` that hosts it | `Application.MainPage = new OrdersPage()` or `"mainWindow"` in `Default.json` |
| Cost of the choice | a floating window inside a browser tab can feel like a desktop app in a box; users may not expect a title bar | you lose the window as a unit of work — several open "forms" become several pages or a container layout you design (Module 3) |

Rule of thumb for the course: **keep every ported screen a `Form` until the navigation model is decided** (Module 3:
`MenuBar` / `ToolBar` shell with three screens), then promote the main screen to a `Page` and leave editors and
previews as `Form`s. The one-line change is cheap in either direction; the decision that is not cheap is where the
menu, the status bar and the "current screen" live once there is more than one.
