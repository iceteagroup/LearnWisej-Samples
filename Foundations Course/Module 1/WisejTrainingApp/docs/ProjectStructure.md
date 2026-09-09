# Project structure — what each file is for (lab step 7)

The lesson "Getting started on Wisej.NET" says the Solution Explorer is not random files; it shows how the
app is organized. This is the Module 1 project, file by file.

| File | What it is for | Who edits it |
|---|---|---|
| `Program.cs` | Startup code. `Program.Main(NameValueCollection args)` is the Wisej.NET session entry point named in `Default.json` (`"startup"`); it does `new Window1().Show()`. | You, rarely |
| `Window1.cs` | Code-behind: the event handler `btnGreet_Click`, the `AddLog` / `SetStatus` helpers and the file inspection. Behaviour lives here. | You |
| `Window1.Designer.cs` | Designer-generated layout: `InitializeComponent()` creates every control, sets `Name`, `Text`, `Location`, `Size`, `Font` and wires `Click += …`. | The Wisej.NET Designer — don't edit by hand |
| `Startup.cs` | The ASP.NET Core / Kestrel host: `app.UseWisej()` then a static file server for `Default.html` (never the `.json` config files). | You, rarely |
| `Default.json` | Wisej.NET configuration: the startup class, the theme (`Bootstrap-4`), `debug`. | You |
| `Default.html` | The page the browser loads; its only job is `<script src="wisej.wx">`, which boots the client. | You, rarely |
| `Web.config` | IIS-style app settings: `Wisej.LicenseKey`, `Wisej.DefaultTheme`. Never put real secrets in source control. | You |
| `WisejTrainingApp.csproj` | The project: `Wisej-4` package reference, `net10.0-windows;net10.0` targets. | You |
| `Properties/launchSettings.json` | The F5 profile: `http://localhost:5081`. | You |
| `*.resx` | Not present in this module: the Designer adds one when a control stores resources (images, InitScripts). | Designer |
| `Models/`, `Services/` | Not present yet: Module 4 adds `Ticket.cs` and `TicketService.cs`. | You |

## The lifecycle you can watch in the event log

```
Program.Main                     → new Window1().Show()
Window1()                        → InitializeComponent()   (Window1.Designer.cs builds the controls)
Window1_Load                     → ready
user clicks Say Hello            → browser sends the event → btnGreet_Click runs on the server
btnGreet_Click                   → reads txtName, validates, sets lblResult
request ends                     → Wisej.NET sends the changed properties back → the browser updates
```

## Evidence

- On load the event log shows the three lifecycle lines and `InspectFiles → 9/9 files found under Application.StartupPath`;
  the "Inspect files" card lists every file above with a ✓ and its purpose, and the path label shows the project folder.
- Delete or rename a file while the app runs and click **Inspect project files** again: that row turns to ✗ and the
  count drops — the check is real, not a hard-coded list of ticks.
