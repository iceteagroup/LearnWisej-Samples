# Peer code-review checklist — what a reviewer looks at in this project

Lesson s47 §2 lists five review points; the **Code Review** screen (`Views/CodeReviewView.cs`) shows them in
`chkReviewItems` and, for the selected point, where a reviewer looks in this project (`lblWhereToLook`).
Tick each point once you have looked; `lblStatus` turns green at 5 / 5. This note is the long form.

## 1. The five review points (s47 §2)

### Naming — do classes, controls and methods clearly describe their job?

| Look at | What you should find |
|---|---|
| The field lists at the bottom of `Window1.Designer.cs` and every `Views/*.Designer.cs`, `Dialogs/TicketDialog.Designer.cs` | Prefix + purpose on every control with behaviour: `btnCreateTicket`, `btnEditTicket`, `btnDeleteTicket`, `btnRefresh`, `dgvTickets`, `lblStatus`, `lblSelection`, `txtTitle`, `txtCustomer`, `cboStatus`, `cboPriority`, `txtAssignedTo`, `lblValidation`, `chkRequired`, `lblPackageStatus`, `btnStartJob`, `btnCancelJob`, `chkSimulateError`, `progressBar`, `lstJobLog`, `cboTheme`. Card-title labels are `label…Card` (`labelTicketsCard`) because they carry no behaviour. Nothing is `button1` or `label2`. |
| Method names in `Window1.cs`, `TicketsView.cs`, `TicketDialog.cs`, `JobsView.cs` | Verbs that say what happens: `NavigateTo`, `SetActiveButton`, `AddActivity`, `SetStatus`, `TicketsChanged`, `RefreshTicketGrid`, `ConfigureGrid`, `UpdateSelectionHint`, `ValidateForm`, `LoadTicket`, `ReadTicket`, `FocusFirstInvalid`, `SetJobRunning`, `LogJob`, `LogError`, `UpdatePackageStatus`. |
| Class names | `TicketService` / `TicketRepository` / `TicketValidator` / `CustomerService` say which layer they are; `HelpdeskView` / `IHelpdeskShell` say what they abstract; each screen is `<Name>View`. |
| The one naming difference from the lesson | The lesson writes `cmbPriority`; this project uses `cboPriority` everywhere (the course's other modules use `cbo` too). Consistent within the project is what matters. |

### Separation of concerns — is ticket logic in a service/validator instead of scattered across UI events?

| Look at | What you should find |
|---|---|
| `Services/TicketValidator.cs` | Every rule about a ticket, and nothing else: Title required and ≤ `MaxTitleLength` (80), Customer required, Status in `Statuses`, Priority in `Priorities`, Closed needs `AssignedTo`. It takes a `Ticket`, not a `TextBox`, so it can be unit-tested. |
| `Services/TicketService.cs` | CRUD + `GetSummary` + `GetTicketsForCustomer`. `Guard()` runs the validator again before `Add`/`Update` — the dialog is the first line of defence, never the only one. |
| `Services/TicketRepository.cs` | The only class holding the `List<Ticket>`; it assigns Ids in `Add`, throws on `Update` of an unknown Id, returns a copy from `GetAll`. |
| `Views/TicketsView.cs` handlers | Each is the lab's shape and nothing more: (guard) → open dialog → `await ShowDialogAsync()` → service call → `RefreshTicketGrid()` → `ShowStatus` + `Shell.AddActivity` + `Shell.TicketsChanged()`. Grep the file for `Status ==` or `IsNullOrWhiteSpace` — there is none. |
| `Dialogs/TicketDialog.cs` | `ValidateForm()` delegates to `validator.Validate(ReadTicket())`; the dialog does not know `TicketService` exists. |
| `Window1.cs` | No `using WisejTrainingApp.Models` — the shell never handles a ticket. |

### Navigation — can the reviewer follow how each page opens?

| Look at | What you should find |
|---|---|
| `Window1.BuildScreens()` | Eight `Register(name, button, view)` lines — the complete list of screens in one place. Adding a screen is one line plus a nav button, not a new handler. |
| `Window1.NavButton_Click` | One handler for all eight buttons; the screen name is in `button.Tag`. |
| `Window1.NavigateTo(string)` | The single place a screen is shown: remove the current view from `pnlContent`, add the new one with `Dock = Fill`, highlight the button, call `view.ActivateScreen()`, log, set the status bar. Unknown names get a red status and a log line instead of an exception. |
| `HelpdeskView.ActivateScreen()` | What each screen refreshes when it opens: `TicketsView` → `RefreshTicketGrid()`, `DashboardView` → `RefreshSummary()`, `CustomersView` → `RefreshCustomerGrid()`, the notes screens select their first item. |
| Cross-screen navigation | Only through the shell: `DashboardView.btnOpenTickets_Click` → `Shell.NavigateTo("Tickets")`. No view holds a reference to another view. |
| The Dashboard's `lstActivity` | Every `NavigateTo` writes `NavigateTo("<name>") → <View> shown in pnlContent`, so the reviewer can replay the navigation from the log. |

### Usability — are buttons placed where users expect them, with clear labels and feedback?

| Look at | What you should find |
|---|---|
| Command placement | Commands sit at the top-right of each card in the same order everywhere: primary (`Create ticket`, `Start job`, `Add customer`, `Check all`), secondary (`Edit`, `Cancel`, `Fill a valid sample`), danger (`Delete`), neutral (`Refresh`, `Reset`, `Clear`). |
| Spacing | The numbers are written once in the `Window1` class comment and used by every Designer: header 64, nav 220, status bar 36, content padding 24, card gap 16, card padding 24, control gap 8, nav buttons 188×44. |
| Feedback after every action | Each screen's `lblStatus` (green / amber / red through `HelpdeskView.ShowStatus`), the shell's status bar, and one activity line. Success: "Ticket created successfully." Guard: "Select a ticket in the grid first, then click Edit." Cancel: "Create cancelled — nothing changed." |
| Labels | Required fields are marked `*` in `TicketDialog`; `lblAssignedToField` says when it becomes required; `lblSelection` under the grid says what is selected and what Edit/Delete will do; `lblHint` prints the data flow. |
| Confirmation before a destructive action | `btnDeleteTicket_Click` → `MessageBox.ShowAsync(…, MessageBoxButtons.YesNo, MessageBoxIcon.Question)`; No keeps the ticket and says so. |
| Disabled while busy | `JobsView.SetJobRunning(true)` disables `btnStartJob` and `chkSimulateError`, enables `btnCancelJob`; the `finally` block always restores them. |

### Deployment — are configuration, logging, secrets, themes and release notes checked before review?

| Look at | What you should find |
|---|---|
| `Views/DeploymentView.cs` | The nine required checks from s42 §4 in `chkRequired`, `lblPackageStatus` NOT READY until 9 / 9, `lblDeploymentNotes` with this project's specifics. |
| `Web.config` | `Wisej.LicenseKey` is empty on purpose — the key comes from the host, never from source. |
| `Default.json` | `"startup"`, `"theme": "Bootstrap-4"`, `"debug": true` (to be `false` for release). `Startup.cs` never serves `*.json`. |
| Logging | `Window1.AddActivity` (timestamped, per session) and `JobsView.LogJob / LogError`; no secrets or personal data in any line. `docs/NextSteps.md` names the production sink. |
| `docs/DeploymentChecklist.md`, `docs/ReadinessNote.md` | The long-form checklist and the readiness note for submission. |

## 2. Habits (s47 §1) — what each one looks like here

| Habit | What it looks like in this capstone |
|---|---|
| Clear control names | `btnCreateTicket`, `dgvTickets`, `lblStatus`, `txtTitle`, `cboPriority` — see the Naming section; the Designer field lists are the checklist. |
| Small event handlers | `btnCreateTicket_Click` is ten lines: dialog → service → refresh → status → log. `btnStartJob_Click` is longer only because it is the try/catch/finally the lesson asks for; the work is in `DescribeStep`, `SetJobRunning`, `LogJob`, `LogError`. |
| Reusable validation | `TicketValidator.Validate` is used by `TicketDialog.ValidateForm` for both New and Edit and by `TicketService.Guard` before storing. `CustomerService.ValidateCustomer` follows the same pattern. |
| Intentional refresh | `RefreshTicketGrid()` is called only after a successful Add / Update / Delete, from Refresh, and from `ActivateScreen`; `Shell.TicketsChanged()` recomputes the Dashboard cards at the same moments. A cancelled dialog refreshes nothing and says so. |
| Safe errors | `JobsView`: the user sees "The digest could not be sent. Support has the details in the job log."; `LogError` writes the exception type, message and location to `lstJobLog`. `Window1.cboTheme_SelectedIndexChanged`: "The theme could not be applied…" on screen, the exception in the activity log. |

## 3. Before you submit

- [ ] Every control with behaviour has a prefix + purpose name (grep the Designer files for `button1`, `label1`, `textBox1` — expect none).
- [ ] No ticket rule outside `Services/TicketValidator.cs`.
- [ ] `NavigateTo` is the only place `pnlContent.Controls` changes.
- [ ] Every handler ends in `lblStatus` + `Shell.AddActivity`.
- [ ] `Default.json` `"debug"` and `Web.config` license key are host-provided for release (see `docs/DeploymentChecklist.md`).
- [ ] The Code Review screen shows 5 / 5 and the Deployment screen shows READY when you take the screenshots.

## Evidence

- **Code Review screen**: `chkReviewItems` lists the five points as "Focus — question"; selecting one fills
  `lblFocusTitle` ("Naming — where a reviewer looks in this project") and `lblWhereToLook`; ticking one writes
  `Code review: "Naming" reviewed` to the Dashboard activity log and moves `lblStatus` from amber
  `● n of 5 review points covered` to green `● all 5 review points covered — ready to submit` at 5 / 5.
  **Reset** clears the ticks and logs `Code review checklist reset`.
- The habits card (`lstHabits`) lists the five habits with the names they map to in this project.
- **Tickets screen**, Edit with nothing selected: `lblStatus` amber "Select a ticket in the grid first, then
  click Edit." and the log line `btnEditTicket_Click → no row selected → guard message` — the guard is
  visible, not a null reference.
