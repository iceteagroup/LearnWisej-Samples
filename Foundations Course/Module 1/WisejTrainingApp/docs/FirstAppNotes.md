# First app notes — the design → name → handle event → run cycle

The Module 1 lab is one screen: type a name, click a button, a label greets you. Small as it is, it
exercises the whole Wisej.NET mental model from the lesson — **screens, controls, properties and events**.

## 1. Design (the Designer builds the screen)

`Window1.Designer.cs` is what the Designer writes when you drag controls from the Toolbox. Four controls
matter for the lab; the rest are the course's usual cards (event log, inspect files, bottom bar):

| Control | Type | Properties set in the Designer |
|---|---|---|
| `lblPrompt` | `Label` | `Text = "Your name"` |
| `txtName` | `TextBox` | `Watermark`, `Font`, `KeyDown += txtName_KeyDown` |
| `btnGreet` | `Button` | `Text = "Say Hello"`, `Click += btnGreet_Click` |
| `lblResult` | `Label` | `Font` 13 bold, light background, `TextAlign = MiddleLeft` |

## 2. Name (clear names are a developer habit)

The lab guide names them `lblPrompt`, `txtName`, `btnGreet`, `lblResult`; the walkthrough video uses
`lblTitle`, `txtName`, `btnSayHello`, `lblStatus`. Both are fine; what matters is that no control is still
called `button1` or `label2` once it has behaviour. This sample uses the lab-guide names and keeps a
`lblTitle` and a `lblStatus` for the heading and the status line.

## 3. Handle the event (code-behind, not the Designer file)

Double-clicking `btnGreet` in the Designer creates the handler in `Window1.cs`. It reads like a short story:
get the input, validate, update the UI.

```csharp
private void btnGreet_Click(object sender, EventArgs e)
{
    string name = txtName.Text.Trim();

    if (string.IsNullOrWhiteSpace(name))
    {
        lblResult.Text = "Please enter a name.";
        SetStatus("validation: a name is required", StatusKind.Warn);
        AddLog("btnGreet_Click → txtName is blank → IsNullOrWhiteSpace guard → …");
        txtName.Focus();
        return;
    }

    lblResult.Text = $"Hello, {name}!";
    SetStatus($"greeted {name}", StatusKind.Ok);
    AddLog($"btnGreet_Click → txtName.Text = \"{name}\" → lblResult.Text = \"Hello, {name}!\"");
}
```

Habits from the lesson that are visible here:

- **Trim first, then guard.** `"  Ada  "` becomes `Ada`; blank or whitespace input is rejected with a message, not an exception.
- **Return early.** The guard exits before anything is updated, so a failed validation never half-updates the screen.
- **Helpers keep handlers short.** `AddLog` and `SetStatus` are the only places that know how the log and the status label work.
- **Enter = click.** `txtName_KeyDown` calls `btnGreet_Click`; the logic exists once.

## 4. Run (locally, in the browser)

`dotnet run -f net10.0 --urls http://localhost:5081` (or F5 in Visual Studio) builds the project, starts
Kestrel, and the app opens in the browser. Every click travels browser → server → C# handler → changed
properties → browser; the event log on the right shows that round trip for each action.

## Evidence

| Action | Event log line | Screen |
|---|---|---|
| Type `Ada`, click **Say Hello** (or press Enter) | `btnGreet_Click → txtName.Text = "Ada" → lblResult.Text = "Hello, Ada!"` | `lblResult` = *Hello, Ada!*, status green "greeted Ada" |
| **Try a blank name (validation)** | `btnGreet_Click → txtName is blank → IsNullOrWhiteSpace guard → lblResult = "Please enter a name."` | `lblResult` = *Please enter a name.*, status amber, focus back in `txtName` |
| **Fill a sample name and greet** | `txtName = "  Ada  " (spaces on purpose)` then the success line | greeting without the spaces — `Trim()` did its job |
