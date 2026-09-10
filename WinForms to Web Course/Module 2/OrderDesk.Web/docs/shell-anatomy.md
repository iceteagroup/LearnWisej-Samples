# Shell anatomy — what replaces `Application.Run`

Lab 2 deliverable: *the Wisej.NET shell — project file, package ref, startup files*. This is the
`OrderDesk.Web` project as it stands at the end of Module 2, file by file, with what each one
replaces from `LegacyOrderDesk`. The **Shell anatomy** card of the running app reads the same
files at runtime and shows their live values.

## Path B: a clean shell, forms moved in

The lesson names two routes: convert the WinForms project file in place (Path A) or create a new
Wisej.NET project and move forms and classes across (Path B). The samples take **Path B** on a
branch (`migration/m2-shell`, with the desktop build tagged as the backup): the template project is
known-good, so every error that appears is caused by the code you moved, and the original desktop
app keeps building.

## The files

| File | Replaces (WinForms) | Role | Live value the card shows |
|---|---|---|---|
| `OrderDesk.Web.csproj` | `LegacyOrderDesk.csproj` (`OutputType WinExe`, `UseWindowsForms`) | `Microsoft.NET.Sdk.Web`, `<TargetFrameworks>net10.0-windows;net10.0</TargetFrameworks>`, `AssemblyName OrderDesk`, one `PackageReference Wisej-4 4.1.0` | `Wisej-4 package · Wisej.Framework 4.1.x · net10.0[-windows] · Windows` |
| `Default.html` | the desktop has no equivalent — the window *was* the app | the browser entry point: one `<script src="wisej.wx">`, an empty `<body>`; Wisej.NET builds the UI from the server-side controls | `<script src="wisej.wx"> · N bytes · empty <body>` |
| `Default.json` | `[STAThread] static void Main()` deciding which form to run | startup settings: `"url": "Default.html"`, `"startup": "OrderDesk.Program.Main, OrderDesk"`, `"theme": "Bootstrap-4"` | `startup = OrderDesk.Program.Main, OrderDesk · theme Bootstrap-4 · url Default.html · (no mainWindow)` |
| `Program.cs` | `Application.EnableVisualStyles(); Application.Run(new OrdersForm());` | `static void Main(NameValueCollection args) { Application.MainPage = new MainPage(); }` — runs **once per session**, not once per process | `Program.Main(args) → Application.MainPage · session 1a2b3c4d · Kestrel app.UseWisej() · .NET 10.0.x` |
| `Startup.cs` | the exe owning the process and the message loop | the ASP.NET Core host: `WebApplication.CreateBuilder(...)`, `app.UseWisej()`, static files via `UseFileServer()` (never the `.json` files), `app.Run()` — Kestrel owns the process; Wisej.NET is middleware | — |
| `Web.config` | `App.config` (`appSettings`, `connectionStrings`) | `<appSettings>` (`Wisej.LicenseKey`, `Wisej.DefaultTheme`, `OrderDesk.StorageRoot`) and `<connectionStrings>` (`OrderDesk`) | `appSettings ×3 · connectionStrings ×1 (OrderDesk → (local)) · OrderDesk.StorageRoot = App_Data` |
| `Properties/launchSettings.json` | — | `applicationUrl http://localhost:5102`, `WEBSITE_PATH` so Visual Studio serves the project folder | — |
| the browser | the Windows desktop, the screen, the printer, the registry | `Application.Browser` (type, version, OS, viewport), `Application.SessionId`, `Application.SessionCount` | `Chrome 129 · Windows · 1400×760 · sessions 1` |

## `startup` vs `mainWindow` in `Default.json`

Two documented ways to name the first view:

```json
{ "url": "Default.html", "startup": "OrderDesk.Program.Main, OrderDesk", "theme": "Bootstrap-4" }
```

`startup` names a **static method** (`type.method, assembly`). Wisej.NET calls it once per new
session; the method decides what to show (`Application.MainPage = new MainPage();`), can read
`args` (the query string), subscribe to `Application.ApplicationExit`/`SessionTimeout` (Module 4),
pick a theme, or show a login first. It is the direct equivalent of the WinForms `Program.Main` and
what the samples use.

```json
{ "url": "Default.html", "mainWindow": "OrderDesk.Pages.OrdersPage, OrderDesk", "theme": "Bootstrap-4" }
```

`mainWindow` names a **type** (`type, assembly`) — a `Page`, `Form` or `Desktop` — that Wisej.NET
instantiates and shows for each session, with no code of yours in between. It is the shortest
route when the ported form *is* the whole app (the video shows this variant); it gives you no place
for per-session setup, so the samples keep `startup`.

## What is gone, and why

| WinForms startup | Web shell |
|---|---|
| `Application.EnableVisualStyles()` | nothing — the theme (`Default.json "theme"`, `Application.LoadTheme`) styles every control |
| `Application.SetCompatibleTextRenderingDefault(false)` | nothing — the browser renders text |
| `Application.Run(new OrdersForm())` | `Application.MainPage = new MainPage()` inside the `startup` method; Kestrel keeps the process alive |
| `[STAThread]` | nothing — requests run on the thread pool; a session's UI code runs one request at a time |
| `using (var login = new LoginForm()) { if (login.ShowDialog() != OK) return; AppState.CurrentUser = login.User; }` | a login page/dialog shown by `Program.Main`, storing the user in `Application.Session` — Module 4 |
| `Close()` → process exits | the session ends when the tab closes or times out (`Application.ApplicationExit`) |
| `App.config` next to the exe | `Web.config` in the web root (see `Services/AppConfig`) |

## Evidence (what the running app shows)

- On load the trace starts with `• Program.Main  Application.MainPage = new MainPage() · session ……`,
  `• Startup.cs  Kestrel owns the process · app.UseWisej() · no EnableVisualStyles, no Application.Run`,
  `• Default.json  "startup": "OrderDesk.Program.Main, OrderDesk" …`, then one `• <file>` line per shell part with the live value,
  and `★ shell  OrderDesk.Web = Wisej-4 4.1.0 · net10.0-windows;net10.0 · Path B …`.
- The **Shell anatomy** card shows the six rows; hover a value for the full text. The app bar's right side shows
  `session <8 chars> · net10.0[-windows] · Wisej.Framework <version>`.
- **Web.config lookup ✓** refreshes the Web.config row after reading the file.
