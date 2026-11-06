# AccessOps · Security & Auth Patterns · Module 1

Local lab build for **Module 1 · The Wisej.NET Security Model**. The AccessOps shell as the
walkthrough shows it - the blue app bar, the five areas, the employee directory and the
"server said" strip - plus the written threat model every later module is measured against.
The login page is a placeholder: it checks no credential and says so.

## Run it

```bash
cd "Security and Auth Patterns Course/Module 1/AccessOps"
dotnet run -f net10.0 --urls http://localhost:6401
```

Open <http://localhost:6401>. In Visual Studio, open `AccessOps.slnx` and press F5. The first run
creates `Module 1/AccessOpsData/accessops.db` (outside the web root) from
`AccessOps/Data/seed-employees.json`. Delete the folder to start again.

## The screen

- **Sign in** - `txtUserName`, `txtPassword`, `btnSignIn` and the status line (`lblStatus`).
- **Shell** - `AccessOps · Employees` in the app bar, the area strip *Departments · Employees ·
  Documents · Approvals · Admin console*, the wire note on the right
  (`Directory projection · name, department, role, manager`), the directory grid, the action strip
  (*Open record*, *Download contract*, *Approve request*) and the `SERVER SAID` strip.

## What to try

| Action | Expected result |
|---|---|
| Click **Sign in** with empty fields | `Enter your user name and password.` |
| Enter `<b>x</b>` as the user name and any password | `That user name and password combination is not valid.` - the markup is shown as text, the reason and the name are not |
| Enter `dana.ruiz` and any password | The shell opens on Employees. Nobody is authenticated; there is no identity in the session yet |
| Read the status strip | `Directory loaded · 6 rows, 4 fields each` |
| Select Kwame Osei, click **Open record** | `Record 4187 opened · Kwame Osei · Finance · Manager — directory fields only` |
| Click **Approvals** (or Departments, Documents, Admin console) | `Nothing loaded for this session` - empty screens the later modules fill |
| In the browser console, run the snippet below | Four values per row: name, department, role, manager. No id, no salary band, no review note |
| `fetch('/Default.json')` from the page | 404. Also 404 for `/appsettings.json` and `/Data/seed-employees.json` |

```js
// what the browser holds for row 4 of the employee grid
const reg = qx.core.ObjectRegistry.getRegistry(); let m;
for (const k in reg) if (reg[k] && reg[k].classname === 'wisej.web.datagrid.DataModel') m = reg[k];
JSON.stringify(m.getRowData(3).data);  // {"1":"Kwame Osei","2":"Finance","3":"Manager","4":"A. Lindqvist"}
```

## Lab tasks → where in the code / docs

| Deliverable | Where |
|---|---|
| AccessOps solution with a placeholder login page | [AccessOps.slnx](AccessOps.slnx), [LoginPage.cs](AccessOps/Pages/LoginPage.cs) - success, validation and generic failure paths |
| Empty screens for departments, employees, documents, approvals, admin console | [MainPage.cs](AccessOps/Pages/MainPage.cs), [EmptyAreaView.cs](AccessOps/Pages/EmptyAreaView.cs) |
| The directory projection (what the server lets leave) | [EmployeeDirectoryService.cs](AccessOps/Services/EmployeeDirectoryService.cs), [EmployeesView.cs](AccessOps/Pages/EmployeesView.cs) |
| `Default.json` cannot be downloaded | [Startup.cs](AccessOps/Startup.cs) - `app.UseWhen` around `app.UseFileServer()` |
| Trust boundary diagram | [docs/TrustBoundaries.md](docs/TrustBoundaries.md) (Mermaid) |
| Asset and entry point inventory | [docs/AssetAndEntryPointInventory.md](docs/AssetAndEntryPointInventory.md) |
| Threat table with likelihood, impact and mitigation | [docs/ThreatTable.md](docs/ThreatTable.md) |
| Security control placement map | [docs/ControlPlacementMap.md](docs/ControlPlacementMap.md) |

## Verified in the browser pane

Run with the `accessops-m1` launch entry at 1400x900: all three login paths, the shell, the
status texts above, the empty areas, the grid's client data model (4 values per row) and the 404s
for `.json` paths. A deliberate experiment - one extra column bound to the id with
`Visible = false` - put `"5":"4187"` into every client row, which is the lesson's point; the
experiment was removed again. No browser console errors other than the 404s the checks requested.

## Answers to the review questions

1. **Hidden salary column.** The browser holds the salary value of every loaded row in the grid's
   data model; `Visible = false` only stops it being drawn (verified above with the id column). The
   change that removes it from the wire is in the query: bind a projection that has no salary
   property and add no column for it - `EmployeeDirectoryService.GetDirectory`.
2. **Two people on one server.** They share everything static: the process, singletons such as
   `AccessOpsDatabase` and `EmployeeDirectoryService` (which hold no user data), and any cache.
   They do not share `Application.Session`, pages or controls. For one to see the other's current
   department, that department would have to be stored somewhere shared - a static "current user",
   a singleton field, or a cache keyed without the user.
3. **Arguing against the top threat (T5, pay data in the widget state).** It would be wasted effort
   if no screen ever bound a record type containing pay data - for example if pay lived in a
   separate system AccessOps never queries. As long as the `employees` table carries salary bands,
   one `SELECT *` bound to a grid re-creates the leak, so the projection rule stays.

## Where this sample differs from the lesson

- **Wisej.NET version.** The course teaches the Wisej.NET 4.2 `Passkey` and `OAuthClient`
  components in Modules 2 and 4. These samples target the released **Wisej-4 4.1.7**, which has
  neither, so passkeys are not implemented and external sign-in uses ASP.NET Core's OpenID Connect
  handler (Module 4). Module 1 itself uses nothing from 4.2.
- **Status text.** The video's status reads `5 rows, 4 fields each`; the sample has six employees
  (Anna Lindqvist, the managers' manager, is a real row so `A. Lindqvist` has someone to point to)
  and reports `6 rows, 4 fields each`, counted from the grid. The video's wire note lists
  `id, name, department, role`; the browser does not receive the id (no column), so the note reads
  `name, department, role, manager`.
- **The leaking run state.** The run scene first shows the grid bound to `SELECT *` with a hidden
  salary column. The sample does not ship that bad version as a mode; the README snippet and the
  recorded experiment show the same evidence on the real grid.
- **The forged approve scene** (Dana's approve refused by `ApprovalService`) previews Module 3.
  Module 1 has no identity and no approvals, so *Approve request* and *Download contract* are
  disabled here; the refusal is built and tested in Module 3.
- **Placeholder login.** The lab asks that the handler only sets the status line; the Module 2
  starting point says the placeholder opens the console. The sample does both: the status line
  reports the outcome, and a well-formed entry opens the shell with nobody signed in.
- **The pay-band call** (`_pay.MayViewBands(me)`) in the code scene needs an identity and a
  permission no lab in this course defines; salary bands simply never leave the server in these
  samples.
