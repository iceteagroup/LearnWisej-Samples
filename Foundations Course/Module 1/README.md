# WisejTrainingApp · Wisej.NET Foundations · Module 1

Local lab build for **Module 1 · Getting started on Wisej.NET**. It is the lab's one-screen app: a prompt label,
a textbox, a button and a result label — the full *design → name → handle event → run* cycle — plus the course's
usual event-log card and an "Inspect files" card for lab step 7 (what `Program.cs`, the code-behind, the
`.Designer.cs` file and the config files are for).

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
| Type a name, click **Say Hello** (or press Enter) | success | `lblResult` shows *Hello, &lt;name&gt;!*, the status turns green, the log shows `btnGreet_Click → txtName.Text = "…" → lblResult.Text = "Hello, …!"` |
| **Try a blank name (validation)** | failure | `lblResult` shows *Please enter a name.*, the status turns amber, the log shows the `IsNullOrWhiteSpace` guard firing, focus returns to the textbox |
| **Fill a sample name and greet** | recovery | `"  Ada  "` is put in the box on purpose; the greeting has no spaces — `Trim()` |
| **Inspect project files** | lab step 7 | nine files listed with ✓ and a one-line purpose; the path label shows the folder the app runs from; the log counts `9/9 files found` |
| **Clear log** | – | empties the event log |

The right-hand card is the event log: every server-side decision with a timestamp, so the browser → server →
handler → browser round trip is visible for each click.

## Where things live

```
WisejTrainingApp/
├─ Program.cs                Wisej.NET session entry point: new Window1().Show()
├─ Window1.cs                code-behind: btnGreet_Click, txtName_KeyDown, InspectFiles, AddLog/SetStatus helpers
├─ Window1.Designer.cs       Designer-generated layout (InitializeComponent) — the four lab controls + the cards
├─ Startup.cs                Kestrel host (app.UseWisej(), static files from the project folder)
├─ Default.json / Default.html / Web.config / Properties/launchSettings.json
└─ docs/
   ├─ ProjectStructure.md    lab step 7 — every file and its job, the lifecycle
   └─ FirstAppNotes.md       the design → name → handle event → run cycle, the handler explained, evidence
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| 1 · Create the project | `WisejTrainingApp.csproj` (Wisej-4 4.1.0, `net10.0-windows;net10.0`), `WisejTrainingApp.slnx` |
| 2 · Open the Designer | `Window1.Designer.cs` — `InitializeComponent()` is what the Designer generates |
| 3 · Add controls | `lblPrompt`, `txtName`, `btnGreet`, `lblResult` in `Window1.Designer.cs` |
| 4 · Name them clearly | same four names, exactly as the lab guide asks |
| 5 · Create the event | `this.btnGreet.Click += new System.EventHandler(this.btnGreet_Click);` → `btnGreet_Click` in `Window1.cs` |
| 6 · Run locally | `dotnet run -f net10.0 --urls http://localhost:5081` / F5 |
| 7 · Inspect files | the "Inspect files" card (`InspectFiles()` in `Window1.cs`) and `docs/ProjectStructure.md` |

Lab code check (`labs.js` m1): the handler is `btnGreet_Click`, reads `txtName.Text`, updates `lblResult.Text`,
uses `Trim()` and `IsNullOrWhiteSpace`, and lives in `Window1.cs`, not the Designer file.

## Self-check

- **What does Wisej.NET do when the button is clicked?** The browser sends the `Click` event to the server; `btnGreet_Click`
  runs in C#; when it returns, Wisej.NET sends only the changed properties (`lblResult.Text`, `lblStatus.Text`, the new
  log item) back and the browser updates — no HTML, CSS or JavaScript was written for it.
- **Which file must you not edit by hand?** `Window1.Designer.cs`. The Designer rewrites it; your code goes in `Window1.cs`.
- **Where does the app start?** `Program.Main`, named in `Default.json` (`"startup": "WisejTrainingApp.Program.Main, WisejTrainingApp"`).
- **Why validate with `Trim()` + `IsNullOrWhiteSpace`?** So `"   "` counts as empty, `"  Ada  "` greets *Ada*, and the handler
  returns early with a message instead of greeting nobody.

## Verified / unverified

Built and run in the browser on this machine (Wisej-4 4.1.0, .NET 10): every path in the table above.
