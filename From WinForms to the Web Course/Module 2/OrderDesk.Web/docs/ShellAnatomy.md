# Deliverable 1 · The Wisej.NET shell — file by file

The shell is the **Path B** result from the lesson: a clean Wisej.NET 4 project (`OrderDesk.Web`, `net10.0-windows;net10.0`,
`Wisej-4` 4.1.0) created on the migration branch, with forms moved *into* it. `LegacyOrderDesk` stays in the solution
untouched, so the desktop build is the reliable backup the video asks for. The same six rows are compiled into the app
as `Migration/ShellAnatomy.cs`; the console previews the **real file** of the running project for each row.

## The six files and what they replace

| File | Replaces (WinForms) | Role |
|---|---|---|
| `Default.html` | the .exe window frame | Browser entry page. An empty `<body>` and one `<script src="wisej.wx">` — the Wisej.NET client renders every control from the server's description. |
| `Default.json` | `Application.Run(new OrdersForm())` | Startup settings: `"url": "Default.html"`, `"startup": "OrderDesk.Program.Main, OrderDesk"`, `"theme": "Bootstrap-4"`, `"debug": true`. Never served to the browser (`.json` is excluded in `Startup.cs`). |
| `Program.cs` | `Program.Main` + `EnableVisualStyles` + `SetCompatibleTextRenderingDefault` | Server-side **session** entry point: `static void Main(NameValueCollection args)` runs once per browser session and sets `Application.MainPage = new MainPage();`. |
| `Startup.cs` | *(nothing — the .exe owned the process)* | The ASP.NET Core host: `WebApplication.CreateBuilder(new WebApplicationOptions { Args = args, WebRootPath = "./" })` → `app.UseWisej()` → `UseFileServer` for everything but `.json` → `app.Run()`. **Kestrel owns the process.** |
| `Web.config` | `App.config` | `<appSettings>` (`Wisej.LicenseKey`, `Wisej.DefaultTheme`, `OrderDesk.StorageRoot`) and `<connectionStrings>` (`OrderDesk`). Read with `System.Xml.Linq` by `Configuration/WebConfig.cs` — `System.Configuration.ConfigurationManager` is not referenced. |
| `OrderDesk.Web.csproj` | `LegacyOrderDesk.csproj` (`UseWindowsForms`) | `Microsoft.NET.Sdk.Web`, two target frameworks, one `PackageReference` (`Wisej-4` 4.1.0) that replaces the `System.Windows.Forms` assembly. `Default.json` / `Web.config` are `CopyToOutputDirectory = Never` — they belong to the content root, not to `bin`. |

Also in the shell but not "startup": `Properties/launchSettings.json` (port **5602**), `Domain/` (copied verbatim from
the desktop project), `Views/Ui.cs` + `Views/TracePanel.cs` (the course's console helpers).

## `Default.json`: `startup` versus `mainWindow`

Two ways to name the first thing the user sees:

```json
{ "url": "Default.html", "startup": "OrderDesk.Program.Main, OrderDesk", "theme": "Bootstrap-4", "debug": true }
```

```json
{ "url": "Default.html", "mainWindow": "OrderDesk.OrdersForm, OrderDesk", "theme": "Bootstrap-4", "debug": true }
```

- **`startup`** names a static method (`Type.Method, Assembly`) that Wisej.NET calls once per session. It is the
  direct heir of `Program.Main`: you keep a place to read the query string (`NameValueCollection args`), pick a
  theme, wire `Application.ApplicationExit` / `SessionTimeout`, and then decide what to show
  (`Application.MainPage = new MainPage()`, or `new OrdersForm().Show()`). The samples use this form because it
  mirrors the lesson's minimal startup example.
- **`mainWindow`** names a type (`Type, Assembly`) that Wisej.NET instantiates and shows for you — a `Page`, a
  `Desktop` or a `Form`. No `Program.cs` at all. This is what the walkthrough video shows
  (`"mainWindow": "OrderDesk.OrdersPage, OrderDesk"`) and it is the shortest possible shell.

Both are valid; use one. At runtime `Application.Configuration` exposes what was loaded: the console traces
`StartUp = OrderDesk.Program.Main, OrderDesk · MainWindow = (none) · ThemeName = Bootstrap-4 · Url = Default.html`.

## `Startup.cs`: the pipeline

```csharp
var builder = WebApplication.CreateBuilder(new WebApplicationOptions { Args = args, WebRootPath = "./" });
var app = builder.Build();
app.UseWisej();                                                   // the Wisej.NET middleware
app.UseWhen(ctx => !ctx.Request.Path.Value.EndsWith(".json", StringComparison.InvariantCulture),
            a => a.UseFileServer());                              // static files, never the .json configuration
app.Run();                                                        // Kestrel — this is the process now
```

- `WebRootPath = "./"` makes the project folder the web root, so `Default.html` and `wwwroot/…` are served from
  where they are; `Application.StartupPath` is that folder too (the console reads `Web.config` and the preview
  files from it).
- **There is no `AddWisej()` in Wisej-4 4.1.0.** The video's `builder.Services.AddWisej();` line does not compile
  against this package; `app.UseWisej()` is the only registration. The samples' `Startup.cs` says so in a comment.
- `Startup.cs` runs **once per process**; `Program.Main` runs **once per browser session**. That split is the whole
  difference between "one .exe per user" and "one server for everyone" — it is why Module 4 exists.

## Why `Application.Run` and `EnableVisualStyles` are gone

| WinForms line | Why it has no place on the web | What does the job now |
|---|---|---|
| `Application.EnableVisualStyles();` | Chose the Windows visual style for GDI rendering. The browser renders; the look is a theme. | `"theme": "Bootstrap-4"` in `Default.json` (or `Wisej.DefaultTheme` in `Web.config`); `Application.LoadTheme(...)` at runtime. |
| `Application.SetCompatibleTextRenderingDefault(false);` | GDI vs GDI+ text — a desktop rendering switch. | Nothing; deleted. |
| `[STAThread]` | COM apartment for the UI thread of one process. | Nothing; requests run on the ASP.NET thread pool. Deleted. |
| `using (var login = new LoginForm()) if (login.ShowDialog() != DialogResult.OK) return;` | A **blocking** modal before the main window; `ShowDialog` does not block in Wisej.NET (Module 3). | Sign-in becomes a page or a non-blocking dialog whose result arrives in a callback; the user lands in `Application.Session` (Module 4). |
| `Application.Run(new OrdersForm());` | Started the message loop that *was* the process, for *one* user. | `Application.MainPage = new MainPage();` per session, inside a process Kestrel started. |

The compiler flags each of these (`CS0117 'Application' does not contain a definition for 'Run'` …) — they are the
**Irrelevant styling** and **Unsupported desktop op** rows of `CompilerErrorLog.md`. The fix was to *not copy*
`LegacyOrderDesk/Program.cs` at all and let the shell's `Program.cs` take over.

## Evidence (in the running app)

- **Wisej.NET shell** card: six rows; selecting one shows the first lines of the actual file from
  `Application.StartupPath` and traces `→ .NET→JS ShellAnatomy.Preview Default.json · 6 lines · replaces Application.Run(new OrdersForm())`.
- Trace on load: `• server startup Default.json → OrderDesk.Program.Main → Application.MainPage = new MainPage()`,
  `• server Application.Configuration StartUp = OrderDesk.Program.Main, OrderDesk · MainWindow = (none) · ThemeName = Bootstrap-4 · Url = Default.html`,
  `• server Kestrel localhost:5602 · platform … · StartupPath = …\Module 2\OrderDesk.Web`,
  `• server Startup.cs app.UseWisej() — AddWisej() does not exist in Wisej-4 4.1.0, only UseWisej(); Kestrel owns the process, no Application.Run`.
- Status: `● shell running · OrdersForm ported · 0 compiler errors`.
