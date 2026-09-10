# Project structure — what each file is for

The lesson "Getting Started on Wisej.NET" says the Solution Explorer is not random files; it shows how the
app is organized, and it lists `Models/` and `Services/` as part of that picture. This is the Module 1
project, file by file, in the same order.

| File | What it is for | Who edits it |
|---|---|---|
| `Program.cs` | Startup code. `Program.Main(NameValueCollection args)` is the Wisej.NET session entry point named in `Default.json` (`"startup"`); it does `new Window1().Show()`. | You, rarely |
| `Window1.cs` | Code-behind: the handlers `btnSayHello_Click` and `btnSaveTicket_Click`, the `ValidateInput` / `ReadTicketFromScreen` helpers, the `AddLog` / `SetRunState` helpers and the file inspection. Behaviour lives here. | You |
| `Window1.Designer.cs` | Designer-generated layout: `InitializeComponent()` creates every control, sets `Name`, `Text`, `Location`, `Size`, `Font` and wires `Click += …`. | The Wisej.NET Designer — don't edit by hand |
| `Models/Ticket.cs` | A simple data class: `Id`, `Title`, `Customer`, `Status`. The shape of one record — no rules, no UI. | You |
| `Models/Customer.cs` | The second data class the lesson names under `Models/`. | You |
| `Services/TicketService.cs` | The service class: `Save()` assigns the id and stores the ticket. Business logic, away from the UI. | You |
| `Startup.cs` | The ASP.NET Core / Kestrel host: `app.UseWisej()` then a static file server for `Default.html` (never the `.json` config files). | You, rarely |
| `Default.json` | Wisej.NET configuration: the startup class, the theme (`Bootstrap-4`), `debug`. | You |
| `Default.html` | The page the browser loads; its only job is `<script src="wisej.wx">`, which boots the client. | You, rarely |
| `Web.config` | IIS-style app settings: `Wisej.LicenseKey`, `Wisej.DefaultTheme`. Never put real secrets in source control. | You |
| `WisejTrainingApp.csproj` | The project: `Wisej-4` package reference, `net10.0-windows;net10.0` targets. | You |
| `Properties/launchSettings.json` | The F5 profile: `http://localhost:5081`. | You |
| `*.resx` | Not present in this module: the Designer adds one when a control stores resources (images, InitScripts). | Designer |

## Why `Models/` and `Services/` exist in Module 1

Both readings ask for the split before you have written anything big. "What Wisej.NET Is" puts *service class*
in its key vocabulary and ends its example flow with a service call; "Getting Started" lists the two folders in
the structure table and then makes them a habit — *avoid* all save logic in the page, *prefer* a `TicketService`
that owns the rules. The ticket card on the screen is the smallest thing that makes that real: three files, one
four-line handler.

## The lifecycle you can watch in the event log

```
Program.Main                     → new Window1().Show()
Window1()                        → InitializeComponent()   (Window1.Designer.cs builds the controls)
Window1_Load                     → ready
user clicks Say Hello            → browser sends the event → btnSayHello_Click runs on the server
btnSayHello_Click                → reads txtName, validates, sets lblStatus
user clicks Save Ticket          → btnSaveTicket_Click → ValidateInput() → ReadTicketFromScreen()
                                 → ticketService.Save(ticket)  (the rule runs in the service)
request ends                     → Wisej.NET sends the changed properties back → the browser updates
```

## Evidence

- On load the event log shows the three lifecycle lines and `InspectFiles → 12/12 files found under Application.StartupPath`;
  the "Solution structure" card lists every file above with a ✓ and its purpose, and the path label shows the project folder.
- Delete or rename a file while the app runs and click **Inspect project files** again: that row turns to ✗ and the
  count drops — the check is real, not a hard-coded list of ticks.
