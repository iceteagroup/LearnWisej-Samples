# WisejTrainingApp · Wisej.NET Foundations · Module 1

Local lab build for **Module 1 · How Wisej.NET Works**. The module has two readings, and this app is built
from both of them:

- **What Wisej.NET Is (and When to Use It)** — the browser / server / Designer / controls / events mental
  model, the example flow that ends in a *service* call, and the beginner habit: *don't put your business
  logic inside the button click*.
- **Getting Started on Wisej.NET** — create the project, open the Designer, run locally, **inspect the
  solution structure**, the good-habit table, and two worked event handlers.

So the screen has two halves. The top-left card is the lesson's first worked handler — `txtName`,
`btnSayHello`, `lblStatus` — and the bottom-right card is its second one, `btnSaveTicket_Click`, which stays
four lines long because `Models/Ticket.cs` holds the shape and `Services/TicketService.cs` holds the rule.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Foundations Course/Module 1/WisejTrainingApp"
dotnet run -f net10.0 --urls http://localhost:5081
```

Then open <http://localhost:5081>. (Visual Studio: open `WisejTrainingApp.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package. The project multi-targets
`net10.0-windows;net10.0`, so `dotnet run` needs `-f`.

## What to try

| Action | Path | What you should see |
|---|---|---|
| Type a name, click **Say Hello** (or press Enter) | success | `lblStatus` shows *Hello, &lt;name&gt;!*, the run state turns green, the log shows `btnSayHello_Click → txtName.Text = "…" → lblStatus.Text = "Hello, …!"` |
| **Try a blank name (validation)** | failure | `lblStatus` shows *Please enter a name.*, the run state turns amber, the log shows the `IsNullOrWhiteSpace` guard firing, focus returns to the textbox |
| **Fill a sample name and greet** | recovery | `"  Ada  "` is put in the box on purpose; the greeting has no spaces — `Trim()` |
| Fill the ticket title + customer, click **Save Ticket** | success | `lblStatus` shows *Ticket saved.*, the ticket appears in the list with the id the **service** assigned, the log shows `ReadTicketFromScreen() → ticketService.Save(ticket)` |
| **Save an empty ticket (validation)** | failure | `ValidateInput()` returns false and the handler returns **before the service is called** — the log says so in those words |
| **Inspect project files** | solution structure | twelve files listed with ✓ and a one-line purpose, `Models/` and `Services/` among them; the path label shows the folder the app runs from; the log counts `12/12 files found` |
| **Clear log** | – | empties the event log |

The top-right card is the event log: every server-side decision with a timestamp, so the browser → server →
handler → browser round trip is visible for each click.

## Where things live

```
WisejTrainingApp/
├─ Program.cs                Wisej.NET session entry point: new Window1().Show()
├─ Window1.cs                code-behind: btnSayHello_Click, btnSaveTicket_Click, ValidateInput,
│                            ReadTicketFromScreen, InspectFiles, AddLog/SetRunState helpers
├─ Window1.Designer.cs       Designer-generated layout (InitializeComponent) — the lesson's controls + the cards
├─ Models/
│  ├─ Ticket.cs              a simple data class — the shape of one record, no rules
│  └─ Customer.cs            the second data class the lesson names under Models/
├─ Services/
│  └─ TicketService.cs       the service class: assigns the id, stores the ticket, owns the rule
├─ Startup.cs                Kestrel host (app.UseWisej(), static files from the project folder)
├─ Default.json / Default.html / Web.config / Properties/launchSettings.json
└─ docs/
   ├─ ProjectStructure.md    "Inspect the solution structure" — every file and its job, the lifecycle
   └─ FirstAppNotes.md       the design → name → handle event → run cycle, the handlers explained, evidence
```

## Lesson → where in the code

| Reading | Section | Where |
|---|---|---|
| What Wisej.NET Is | mental model: screens, controls, properties, events | `Window1.Designer.cs` builds the controls; every handler in `Window1.cs` runs on the server |
| What Wisej.NET Is | example flow: click → read fields → **call a service** → status message | `btnSaveTicket_Click` in `Window1.cs` |
| What Wisej.NET Is | *"Don't put all your business logic inside the button click"* | `Services/TicketService.cs` owns `Save()`; the handler is four lines |
| Getting Started | 1 · project template, name `WisejTrainingApp` | `WisejTrainingApp.csproj`, `WisejTrainingApp.slnx` |
| Getting Started | 2 · open the Designer, rename controls, create Click code | `Window1.Designer.cs` — `InitializeComponent()` is what the Designer generates |
| Getting Started | 3 · run it locally | `dotnet run -f net10.0 --urls http://localhost:5081` / F5 |
| Getting Started | 4 · inspect the solution structure | the "Solution structure" card (`InspectFiles()`) and `docs/ProjectStructure.md` |
| Getting Started | 5 · good habits — clear names, short handlers, separate logic, organise files | control names throughout; `Models/` + `Services/` folders; `ValidateInput()` |
| Getting Started | 6 · `btnSayHello_Click` worked example | `btnSayHello_Click` in `Window1.cs`, verbatim in shape: `Trim()`, `IsNullOrWhiteSpace`, `lblStatus` |
| Getting Started | 6 · `btnSaveTicket_Click` worked example | `btnSaveTicket_Click` in `Window1.cs`: `ReadTicketFromScreen()` → `ticketService.Save(ticket)` → `lblStatus.Text = "Ticket saved."` |

## A note on the control names

The two Module 1 readings and the build walkthrough all use **`txtName` / `btnSayHello` / `lblStatus`**, so this
sample does too. The lab guide (`labs/m1.json` in the course repo) still asks for `lblPrompt` / `txtName` /
`btnGreet` / `lblResult` — that text is the odd one out and should be brought in line with the readings and the
video.

## Self-check

- **What does Wisej.NET do when the button is clicked?** The browser sends the `Click` event to the server;
  `btnSayHello_Click` runs in C#; when it returns, Wisej.NET sends only the changed properties
  (`lblStatus.Text`, the run-state label, the new log item) back and the browser updates — no HTML, CSS or
  JavaScript was written for it.
- **Which file must you not edit by hand?** `Window1.Designer.cs`. The Designer rewrites it; your code goes in `Window1.cs`.
- **Where does the app start?** `Program.Main`, named in `Default.json` (`"startup": "WisejTrainingApp.Program.Main, WisejTrainingApp"`).
- **Why validate with `Trim()` + `IsNullOrWhiteSpace`?** So `"   "` counts as empty, `"  Ada  "` greets *Ada*, and the handler
  returns early with a message instead of greeting nobody.
- **Why does `btnSaveTicket_Click` not assign the id?** Because that is a business rule, and rules live in
  `TicketService`. Swap the service's `List<Ticket>` for a database in a later module and the screen does not change.

## Verified / unverified

Built (`dotnet build`, 0 warnings, 0 errors, both targets) and run in the browser on this machine
(Wisej-4 4.1.0, .NET 10): the greeting success / validation / recovery paths, the ticket save and its
validation path, and `InspectFiles` reporting **12/12** files including `Models/` and `Services/`.
