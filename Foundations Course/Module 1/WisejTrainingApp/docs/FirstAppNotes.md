# First app notes — the design → name → handle event → run cycle

Module 1's two readings build up to two worked handlers. The first is one screen: type a name, click a button,
a label greets you. The second saves a ticket through a service. Small as they are, together they exercise the
whole Wisej.NET mental model from the lesson — **screens, controls, properties and events** — plus the habit
the readings keep coming back to: *the click stays readable, the service owns the rules*.

## 1. Design (the Designer builds the screen)

`Window1.Designer.cs` is what the Designer writes when you drag controls from the Toolbox. Four controls carry
the lesson's first example; the rest are the course's usual cards (event log, solution structure, ticket card,
bottom bar):

| Control | Type | Properties set in the Designer |
|---|---|---|
| `lblTitle` | `Label` | `Text = "Hello, Wisej.NET"`, `Font` 20 bold |
| `txtName` | `TextBox` | `Watermark`, `Font`, `KeyDown += txtName_KeyDown` |
| `btnSayHello` | `Button` | `Text = "Say Hello"`, `Click += btnSayHello_Click` |
| `lblStatus` | `Label` | `Font` 13 bold, light background, `TextAlign = MiddleLeft` |

## 2. Name (clear names are a developer habit)

Both Module 1 readings and the walkthrough video use `lblTitle`, `txtName`, `btnSayHello`, `lblStatus` — the
"Getting Started" lesson prints the handler in full with exactly those names — so this sample uses them. The
lab guide (`labs/m1.json` in the course repo) still asks for `lblPrompt` / `btnGreet` / `lblResult`; it is the
odd one out and should be brought in line with the readings and the video. What matters either way is that no
control is still called `button1` or `label2` once it has behaviour. The sample also keeps a `lblPrompt`
caption above the textbox and a separate `lblRunState` traffic light; neither is part of the lesson's four.

## 3. Handle the event (code-behind, not the Designer file)

Double-clicking `btnSayHello` in the Designer creates the handler in `Window1.cs`. It reads like a short story:
get the input, validate, update the UI.

```csharp
private void btnSayHello_Click(object sender, EventArgs e)
{
    string name = txtName.Text.Trim();

    if (string.IsNullOrWhiteSpace(name))
    {
        lblStatus.Text = "Please enter a name.";
        SetRunState("validation: a name is required", StatusKind.Warn);
        AddLog("btnSayHello_Click → txtName is blank → IsNullOrWhiteSpace guard → …");
        txtName.Focus();
        return;
    }

    lblStatus.Text = $"Hello, {name}!";
    SetRunState($"greeted {name}", StatusKind.Ok);
    AddLog($"btnSayHello_Click → txtName.Text = \"{name}\" → lblStatus.Text = \"Hello, {name}!\"");
}
```

Habits from the lesson that are visible here:

- **Trim first, then guard.** `"  Ada  "` becomes `Ada`; blank or whitespace input is rejected with a message, not an exception.
- **Return early.** The guard exits before anything is updated, so a failed validation never half-updates the screen.
- **Helpers keep handlers short.** `AddLog` and `SetRunState` are the only places that know how the log and the run-state label work.
- **Enter = click.** `txtName_KeyDown` calls `btnSayHello_Click`; the logic exists once.

## 4. Separate the business logic (the lesson's second handler)

The habits table says it plainly: avoid *"200 lines in btnSave_Click"*, prefer *"call `ValidateInput()` and
`ticketService.Save(ticket)`"*. That is the whole ticket card:

```csharp
private void btnSaveTicket_Click(object sender, EventArgs e)
{
    if (!ValidateInput())
    {
        return;
    }

    Ticket ticket = ReadTicketFromScreen();
    ticketService.Save(ticket);
    lblStatus.Text = "Ticket saved.";
    // … then log and refresh the list
}
```

The handler never assigns the id and never touches storage — `Services/TicketService.cs` does both, and
`Models/Ticket.cs` is a plain data class with no behaviour. That is the split the readings ask for: replace
the service's `List<Ticket>` with a database in a later module and `Window1.cs` does not change.

## 5. Run (locally, in the browser)

`dotnet run -f net10.0 --urls http://localhost:5081` (or F5 in Visual Studio) builds the project, starts
Kestrel, and the app opens in the browser. Every click travels browser → server → C# handler → changed
properties → browser; the event log on the right shows that round trip for each action.

## Evidence

| Action | Event log line | Screen |
|---|---|---|
| Type `Ada`, click **Say Hello** (or press Enter) | `btnSayHello_Click → txtName.Text = "Ada" → lblStatus.Text = "Hello, Ada!"` | `lblStatus` = *Hello, Ada!*, run state green "greeted Ada" |
| **Try a blank name (validation)** | `btnSayHello_Click → txtName is blank → IsNullOrWhiteSpace guard → lblStatus = "Please enter a name."` | `lblStatus` = *Please enter a name.*, run state amber, focus back in `txtName` |
| **Fill a sample name and greet** | `txtName = "  Ada  " (spaces on purpose)` then the success line | greeting without the spaces — `Trim()` did its job |
| Fill the ticket fields, click **Save Ticket** | `btnSaveTicket_Click → ReadTicketFromScreen() → ticketService.Save(ticket) → #1 "…"` | `lblStatus` = *Ticket saved.*, the row appears with the id the service assigned |
| **Save an empty ticket (validation)** | `btnSaveTicket_Click → ValidateInput() = false → returned before the service was called` | `lblStatus` = *A ticket needs a title.*, nothing reaches the service |
