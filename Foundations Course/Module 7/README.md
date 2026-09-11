# WisejTrainingApp · Wisej.NET Foundations · Module 7

The modernized dashboard from **Module 7 · Theming & UI Modernization**, as the walkthrough video builds it:
a header with the app title, module label and a theme selector; a left navigation with one consistent active
state; and a Dashboard with metric cards, grouped commands and recent activity. The Module 5 ticket screen
and dialog are reused as they were — theming changes the look, not what the app does.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Foundations Course/Module 7/WisejTrainingApp"
dotnet run -f net10.0 --urls http://localhost:5087
```

Then open <http://localhost:5087>. (Visual Studio: open `WisejTrainingApp.slnx`, press F5.)

## What to try

| Action | What you should see |
|---|---|
| Pick a theme in **Theme** (Bootstrap-4, BootstrapDark-4, Blue-1, Classic-2) | the whole app restyles; *Current theme: …* updates; the change is logged in Recent activity |
| Click each nav button | the page swaps and the clicked button stays highlighted |
| **New Ticket** on the Dashboard, or New / Edit on Tickets | the Module 5 dialog, validation and save work unchanged; the cards update |

## Where things live

| File | What it's for |
|---|---|
| `MainWindow.cs` | Navigation, `SetActiveButton`, `cboTheme_SelectedIndexChanged` (`Application.LoadTheme`) |
| `Views/DashboardView.cs` | Metric cards, grouped commands, recent activity |
| `Views/TicketsView.cs` | The Module 5 ticket grid with New / Edit |
| `Views/CustomersView.cs`, `ReportsView.cs`, `SettingsView.cs` | Plain pages |
| `Dialogs/TicketDialog.cs`, `Models/Ticket.cs`, `Services/TicketService.cs` | Unchanged from Module 5 |
