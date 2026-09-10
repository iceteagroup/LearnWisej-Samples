# Support Desk cookbook — Data Binding with EF Core (Wisej-4 4.1.0, EF Core 10.0.12, .NET 10)

The conventions every module sample of this course follows. Facts marked **(verified)** were executed on this
machine while building and running Module 1 (or the sibling course samples on the same framework build).
Facts marked **(unverified)** come from the Wisej.NET XML docs or the course text: implement them, make sure
`dotnet build` and `dotnet test` pass, and list them in your report so the reviewer can check them in the browser.

## The course in one sentence

The **Support Desk Data Console**: Customers, Agents, Categories, Tickets and TicketComments behind EF Core, with a
pageable ticket grid (Module 3), a modal ticket editor (Module 4), validation (Module 5), a measured optimisation
(Module 6) and optimistic concurrency + deployment (Module 7). The habit the course teaches: **forms, pages,
BindingSources and the selected row live for the session; a DbContext lives for one operation.**

Modules are cumulative: **Module N is a copy of Module N-1 plus the new lab.** Copy the previous module folder
(without `bin/`, `obj/`, `.vs/`, `App_Data/`), then extend it. Keep everything that worked; the bottom bar of the page
grows with each module (a later module may move earlier buttons into a "Module 1 · lifetimes" secondary row or a
collapsible panel, but never delete a working path).

## Project layout (Module 1 = `_template`)

```
Module N/
  SupportDesk.slnx                     four projects
  .gitignore                           (from _template)
  README.md                            what it shows, how to run, what to click, lab steps → code, self-check answers, verified/unverified
  docs/                                one Markdown file per lab deliverable (+ Evidence section)
  SupportDesk.Web/                     ASP.NET Core + Wisej.NET 4, TargetFrameworks = net10.0-windows;net10.0 — KEEP BOTH
    Startup.cs                         host: configuration → AddSupportDeskData → services → Build → IServiceProvider bridge → UseWisej
    Program.cs                         session entry (Default.json "startup"): Application.MainPage = new TicketBrowserPage();
    TicketBrowserPage.cs/.Designer.cs  the page the course names; later modules add TicketEditorForm(.Designer).cs, ConflictDialog(.Designer).cs
    SupportDeskDevelopmentDatabase.cs  Development-only startup: EnsureCreated (Module 1) → MigrateAsync + seed (Module 2+)
    appsettings.json                   ConnectionStrings:SupportDesk = Data Source=App_Data/supportdesk.db (relative → resolved by SupportDeskPaths)
    Properties/launchSettings.json     applicationUrl http://localhost:540N (Module N → 5401 … 5407), ASPNETCORE_ENVIRONMENT=Development
    Default.html / Default.json / Web.config
  SupportDesk.Data/                    net10.0 class library — the ONLY project with the provider (Microsoft.EntityFrameworkCore.Sqlite + Design)
    SupportDeskContext.cs  Entities/  Migrations/ (Module 2+)  SupportDeskDesignTimeFactory.cs  SupportDeskPaths.cs
    DependencyInjection/SupportDeskDataServiceCollectionExtensions.cs   AddSupportDeskData(connectionString, isDevelopment)
    Diagnostics/                       lab instruments: QueryTrace (AsyncLocal sink), QueryTraceInterceptor, DevelopmentOutageSwitch/OutageInterceptor
  SupportDesk.Services/                net10.0 — TicketQueryService, (TicketCommandService, TicketEditModel, TicketValidator, records …)
  SupportDesk.Tests/                   xunit + SQLite in memory (Support/SqliteTestFactory) — services tested without the UI
```

- Namespaces: `SupportDesk.Web`, `SupportDesk.Data`, `SupportDesk.Services`, `SupportDesk.Tests`. Web project has
  `Nullable`/`ImplicitUsings` **disabled** (Wisej designer style, explicit `using`s); the libraries have both enabled.
- Run: `dotnet run -f net10.0 --urls http://localhost:540N` from `SupportDesk.Web`. Build with `dotnet build SupportDesk.slnx -nologo -v q`
  and fix every error and warning; run `dotnet test SupportDesk.Tests -nologo -v q`.
- Do not start the web app yourself and do not open browsers; the reviewer runs it in the Browser pane. You **may** run
  `dotnet build`, `dotnet test`, `dotnet ef …` (dotnet-ef 10.0.12 is installed globally) and small console checks.
- Control names exactly as the lab guide asks (`countButton`, `statusLabel`, `btnSeed`, `ticketsDataGridView`,
  `ticketBindingSource`, `statusComboBox`, `customerComboBox`, `searchButton`, `nextPageButton`, `prevPageButton`,
  `txtTitle`/`titleTextBox` as the module's lab names them, `btnSave`, `btnCancel`, `btnDelete`, `errorProvider`,
  `validationSummaryLabel`, …). Never `button1`.
- Database: SQLite file `SupportDesk.Web/App_Data/supportdesk.db` (gitignored). The design-time factory, the app and the
  tests share `SupportDeskPaths`. Deleting the file resets the module. `App_Data/**` is excluded from the project items.

## EF Core + Wisej.NET facts — **verified in Module 1**

- `Wisej.Web.Application.Services.AddService<IServiceProvider>(app.Services)` right after `builder.Build()` makes `[Inject]`
  (`Wisej.Services.Inject`) on a `Page`/`Form` resolve through Microsoft DI. Wisej.NET asks the **root** provider, so
  application services must be **Transient** or **Singleton**: a `Scoped` registration throws
  *Cannot resolve scoped service … from root provider* at page construction in Development. `IDbContextFactory<T>` itself is a singleton and injects fine.
- `[Inject] private TicketQueryService TicketQueries { get; set; }` is injected in the `Page`/`Form` **constructor**
  (`Wisej.Web.Page..ctor` → `ServiceProvider.Inject`), so it is available in `Load` and in handlers. Guard for `null` in
  the Load trace anyway (it prints "the IServiceProvider bridge is missing").
- Lifetime rule as code: `await using var db = await _dbFactory.CreateDbContextAsync(token); …` inside the service method;
  the context is disposed before the method returns. The page never sees a context.
- `async void` handlers are fine; after an `await` the continuation may run on any thread and may still change controls.
  Call `Application.Update(this)` in `finally` to push the final state (the response for the original request may already be gone).
- The loading guard is a page field: `if (_loading) return; try { _loading = true; SetBusy(true); … } finally { _loading = false; SetBusy(false); Application.Update(this); }`.
  Wisej.NET **does** deliver a second click while an awaited handler is pending — the guard is what drops it (verified with the "Count ×3 rapid" button).
- Two concurrent operations on one `DbContext` throw `InvalidOperationException` "A second operation was started on this
  context instance before a previous operation completed" (the anti-pattern demo needs a few rounds on SQLite because
  a COUNT takes 0.1 ms).
- `QueryTrace.Begin(sink)` (AsyncLocal) + `QueryTraceInterceptor : DbCommandInterceptor` + the context constructor/Dispose
  report → the page shows `◦ context #n created`, `→ SQL … (ms)`, `◦ context #n disposed (k tracked)`. The scope flows
  through the async chain into the service and interceptor; it is per operation, never static per user. Keep it in every module —
  it is how the reviewer sees statement counts (Module 3: exactly 2 statements per search; Module 6: before/after).
- `OutageInterceptor : DbConnectionInterceptor` throws `DatabaseUnavailableException` while `DevelopmentOutageSwitch.IsDown`
  (a singleton lab prop) — the failure path of every module ("Break the database" / "Restore").
- SQLite specifics: `EnableSensitiveDataLogging()` + `EnableDetailedErrors()` behind `isDevelopment`; SQL text is
  `SELECT COUNT(*) FROM "Tickets" AS "t"`; paging renders as `LIMIT @p OFFSET @p` (not OFFSET/FETCH — say so in the README).
- Tests: `SqliteTestFactory : IDbContextFactory<SupportDeskContext>` over one open `Data Source=:memory:` connection,
  `EnsureCreated()` once; every `CreateDbContext()` hands out a fresh context. Module 2+: `EnsureCreated` is fine for the
  tests even when the app uses migrations (same model). A `RowVersion` column on SQLite is **not** auto-generated by the
  database — see the Module 2/7 notes below.

## EF Core facts you will need later — **unverified at runtime here**, verified in EF Core docs / other projects

- **RowVersion on SQLite.** `IsRowVersion()` marks the property as a concurrency token with `ValueGeneratedOnAddOrUpdate`,
  but SQLite has no `rowversion` type — the column is `BLOB` and nothing fills it. Make the token real: keep
  `b.Property(x => x.RowVersion).IsRowVersion()` for the mapping (the migration then shows the column) **and** set the value
  yourself in `SupportDeskContext.SaveChangesAsync`/`SaveChanges` override: for every `Added`/`Modified` `Ticket` entry,
  `entry.Property(x => x.RowVersion).CurrentValue = Guid.NewGuid().ToByteArray()` (or an incrementing 8-byte counter).
  Because the property is a concurrency token, EF adds `WHERE "RowVersion" = @original` to the UPDATE/DELETE, and a stale
  original → 0 rows → `DbUpdateConcurrencyException`. Document the SQL Server difference (`rowversion` generated by the server,
  nothing to write by hand) in `docs/`. Alternative that also works: `.IsConcurrencyToken()` + `ValueGeneratedNever()` with the same override. Whichever you choose, **test it** in the Tests project: save with a stale token → `DbUpdateConcurrencyException`.
- `db.Entry(ticket).Property(t => t.RowVersion).OriginalValue = model.RowVersion;` restores the token read by the editor before `SaveChangesAsync` (Module 4/7).
- `DbUpdateConcurrencyException.Entries[i].GetDatabaseValuesAsync()` returns `null` when the row was deleted; `entry.ReloadAsync()` refreshes; `entry.OriginalValues.SetValues(databaseValues)` then save again = **Overwrite**.
- Migrations: `dotnet ef migrations add InitialCreate --project SupportDesk.Data --startup-project SupportDesk.Web --framework net10.0`
  (run from the module folder; `--framework` is required because the Web project multi-targets). The design-time factory
  uses `SupportDeskPaths.DevelopmentDatabaseFile()`, so `dotnet ef` never starts Kestrel.
  `dotnet ef migrations script --idempotent --project SupportDesk.Data --startup-project SupportDesk.Web --framework net10.0 --output artifacts/sql/supportdesk_migrations.sql`.
  Commit `Migrations/*.cs` + the snapshot. In Development the host runs `await db.Database.MigrateAsync()` at start (replacing EnsureCreated) and then seeds.
- SQLite migration limits: no `ALTER COLUMN`, and `DeleteBehavior` renders as `ON DELETE RESTRICT/SET NULL/CASCADE` in
  `CREATE TABLE`; foreign keys are enforced (Microsoft.Data.Sqlite turns `PRAGMA foreign_keys` on by default), so
  `Restrict` really refuses the delete with `SqliteException` (SQLITE_CONSTRAINT_FOREIGNKEY) wrapped in `DbUpdateException`.
- Unique index on `Ticket.Number` (Module 5's "duplicate ticket number" path): `b.HasIndex(x => x.Number).IsUnique()`; the failed
  INSERT surfaces as `DbUpdateException` with inner `SqliteException` "UNIQUE constraint failed: Tickets.Number" (SQLITE_CONSTRAINT_UNIQUE, error code 19, extended 2067).
- `EF.Functions.Like` / `Contains` translate to `LIKE`/`instr`; `StartsWith` → `LIKE 'x%'` (index-friendly). Decimal columns
  on SQLite are stored as TEXT — avoid `decimal` in ORDER BY, or say so.
- Logging (Module 6): `options.LogTo(text => …, LogLevel.Information)` or `builder.Logging` + category
  `Microsoft.EntityFrameworkCore.Database.Command` at `Information` in `appsettings.Development.json`; keep
  `EnableSensitiveDataLogging` inside `if (isDevelopment)`. The QueryTrace already shows count + ms per operation — use it for the before/after numbers, LogTo for the "real" log.
- `AsNoTrackingWithIdentityResolution()` exists; `Include(t => t.Comments.OrderByDescending(c => c.CreatedAt).Take(5))` is a filtered Include; `await db.Entry(ticket).Collection(t => t.Comments).LoadAsync()` is explicit loading. There is **no** lazy loading unless `UseLazyLoadingProxies` is added — do not add it; the "naive" Module 6 branch must instead load tracked entities with `Include`s or issue one query per row on purpose (N+1 by explicit per-row `FirstAsync`) so the trace shows the statement count grow.

## Wisej.NET facts — **verified in sibling course samples** on this machine (same 4.1.0 build)

- Dialogs: Wisej.NET has no blocking `ShowDialog()` result. Use `DialogResult r = await dialog.ShowDialogAsync();` in an
  `async void` handler, or `dialog.ShowDialog((form, result) => { … })` (callback, returns immediately). Set
  `this.DialogResult = DialogResult.OK; Close();` inside the dialog (the `Form` has `DialogResult`).
  `await MessageBox.ShowAsync("Delete ticket T-1004?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question)` returns `DialogResult`;
  `MessageBox.Show(text, caption, buttons, icon)` is fire-and-forget.
- Toasts: `AlertBox.Show(text, MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000)` — always TopRight.
- Data binding: `var source = new Wisej.Web.BindingSource(); source.DataSource = list; grid.AutoGenerateColumns = false;
  grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTitle", DataPropertyName = "Title", HeaderText = "Title", Width = 220 });
  grid.DataSource = source;` — set the **DataSource before** `DataBindings.Add`, otherwise Wisej.NET throws
  *Cannot bind to the property or column Title on the DataSource* (verified). `txt.DataBindings.Add("Text", source, nameof(Model.Title), true, DataSourceUpdateMode.OnValidation)`;
  `cbo.DataBindings.Add("SelectedValue", source, nameof(Model.CustomerId), true, DataSourceUpdateMode.OnPropertyChanged)`;
  `chk.DataBindings.Add("Checked", …, DataSourceUpdateMode.OnPropertyChanged)`; `dtp.DataBindings.Add("Value", …, DataSourceUpdateMode.OnValidation)` —
  binding a `DateTime?` to `DateTimePicker.Value` (non-nullable) needs the formatting-enabled overload and a null placeholder; safer: bind `dtp.Checked`/`Value` by hand in code or use `ShowCheckBox = true` and copy in `SaveAsync` (say what you did) **(unverified)**.
  `source.EndEdit()`, `source.CancelEdit()`, `source.ResetBindings(false)`, `source.Current`, `source.CurrentChanged` exist.
  Records with positional constructor (`TicketListItem`) bind read-only to the grid fine — the grid only reads properties.
- **Editor binding facts — verified in the browser on Module 4** (each one cost a crash or a silent no-op):
  1. A `BindingSource` whose `DataSource` is still null throws *Cannot bind to the property or column Title on the DataSource*
     at the first `DataBindings.Add` in `InitializeComponent`. Set `editBindingSource.DataSource = typeof(TicketEditModel)` in the
     Designer right after creating the BindingSource (the WinForms-designer trick); assign the instance later.
  2. `ComboBox` `SelectedValue` bindings must be added **after** the ComboBox has its lookup `DataSource` **and** the BindingSource
     has its current item — bindings added earlier neither push the model value into the control nor write the selection back.
     Keep TextBox/CheckBox bindings in the Designer; add the lookup bindings in a `BindLookupControls()` called once at the end of
     `LoadEditorAsync` (after `editBindingSource.DataSource = model`).
  3. `SelectedValue` needs a `ValueMember`: bound to a plain `List<string>` the control shows its first row and ignores the model.
     Wrap strings in `NamedValue(Value, Name)` rows (`DisplayMember = "Name"`, `ValueMember = "Value"`).
  4. An exception escaping an `async void` `Load` handler (or any handler) becomes the Wisej.NET **Application Error** dialog and the
     session is stuck — every async handler needs its own try/catch with a friendly message (`AlertBox`) and a decided
     `DialogResult`. Loading a row that another session deleted must be a normal path (`SingleOrDefaultAsync` → not-found → toast + OK).
  5. A `Binding.Format`/`Parse` pair (sentinel row ↔ null) works for a ComboBox `SelectedValue` binding once rule 2 is respected.
  8. A **modal** dialog (`ShowDialogAsync`) blocks every control on the page behind it (verified on Module 7): a lab prop that must act *while* the editor is open has to live inside the dialog (Module 7's `btnLabSimulateChange`), and a two-session demo needs a second browser tab.
  9. Driving a Wisej ComboBox from the console (`setSelectedIndex`) repaints the client but does **not** raise the server-side selection change the binding listens to; real clicks on the open button and the list item do (verified on Modules 4 and 7).
  7. A **disabled** button delivers no click at all (verified on Modules 3 and 5): "Save disabled while invalid" means Save can never be what paints the first validation icons — paint them live on field changes, and let lab/demo buttons that pre-fill a form run the validator once on open.
  6. Reviewer driving tip: the accessibility refs of the dialog's ComboBox "open" buttons are listed in **reverse** visual order; the
     Wisej widgets can also be driven from the console — `qx.core.ObjectRegistry.getRegistry()`, filter by `getName()`,
     `setSelectedIndex(i)` on `wisej.web.ComboBox`; `form_input` only fills TextBoxes.
- `DataGridView`: `SelectionMode = DataGridViewSelectionMode.FullRowSelect; MultiSelect = false; ReadOnly = true;
  AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;` `grid.CurrentRow?.DataBoundItem as TicketListItem`;
  `column.DefaultCellStyle.Format = "yyyy-MM-dd"`; `DataGridViewCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight`;
  `CellFormatting` event (`DataGridViewCellFormattingEventArgs`: `Value`, `FormattingApplied`); `CellDoubleClick`/`CellClick` (`e.RowIndex`); `column.AllowHtml = true`.
- `ComboBox`: `DropDownStyle = ComboBoxStyle.DropDownList; DisplayMember = "Name"; ValueMember = "Id"; DataSource = list;`
  `SelectedValue` is `object` (cast `as int?` fails for boxed int — use `cbo.SelectedValue is int id ? id : (int?)null`).
  An "All" entry: a lookup item with `Id = 0`/`null`-marker mapped to a null criterion in code.
- `DateTimePicker`: `Format = DateTimePickerFormat.Short; ShowCheckBox = true; Checked` (unchecked = no date) — **verified in Module 3**: with `ShowCheckBox = true` the picker starts **ticked**, so set `Checked = false` explicitly in the Designer for optional dates (or the first search is filtered); `MinDate/MaxDate`.
- `ErrorProvider`: a component — `new Wisej.Web.ErrorProvider(this.components)` or `new ErrorProvider()` + `ContainerControl = this`;
  `errorProvider.SetError(txtTitle, "Title is required."); errorProvider.SetError(txtTitle, "")` clears one; `errorProvider.Clear()` clears all (verified in Control Library samples).
- Fonts: `new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold)`, monospace `new System.Drawing.Font("monospace", 9F)`.
  Labels support `AllowHtml = true` (Module 1 renders the lifetimes table as HTML). `Padding` is `Wisej.Web.Padding`.
- `Application.SessionId`, `Application.ClientId`, `Application.Update(this)`, `Application.StartTask(...)` (keeps the
  session context; nothing reaches the browser until `Application.Update`). Two browser tabs = two sessions (each has its own page).
- Console logging: `Console.Error.WriteLine("[SupportDesk] …")` is the "server log" (the preview shows it). Production would use `ILogger<T>` — the Web project can inject `ILogger<TicketEditorForm>` through the same bridge.
- Idle sessions: Wisej.NET shows a **Session Timeout** dialog ("Your session will expire in …") after the default idle timeout when the pane sits unused for a few minutes; pressing OK prolongs it and clicks made while the dialog is up are swallowed — dismiss it first (verified on Module 2). Set `"sessionTimeout"` in Default.json only if a module needs longer idle time.
- Browser-pane reviewer notes (verified on Module 1): mouse clicks on the scaled page are unreliable; buttons are driven from the console with
  `qx.core.ObjectRegistry.getRegistry()` → `wisej.web.Button` by `getName()` → `.execute()`. While the pane is **hidden**,
  `innerText` and the page-text tool keep returning the state at load time even after the buttons ran — the **screenshot** (or zoom)
  shows the real state; show the pane or take a screenshot to read results.

## UI conventions (so the seven samples look like one console)

- `TicketBrowserPage : Page`, `Size = 1348×680` (grow the height when the module needs more; `AutoScroll = true`),
  background `Color.FromArgb(238,242,247)`, white cards (`Panel`, `BorderStyle.Solid`), card titles `"default" 14F Bold` / `12F Bold`,
  lead text grey `Color.FromArgb(90,107,125)`, monospace 9F for traces and state.
- **Left card(s)**: the lab's controls (the grid, the filters, the buttons the lab names) + `labelState` ("● idle / ● ok / ● fault":
  green `31,157,87`, amber `232,161,60`, red `224,86,59`, neutral `90,107,125`) + `labelBanner` (friendly error: back `253,236,234`, fore `178,59,39`; hidden until needed).
- **Right card** `panelTrace` + `listTrace` (monospace ListBox) titled **"Server ⇄ Database · EF Core lifetime & SQL trace"**, footer legend
  `• server   ◦ context created / disposed   → SQL sent to the database (ms)   ← result back in the handler`. One `AddTrace(Glyph, label, text)` helper:
  `HH:mm:ss.fff  {•|◦|→|←} {label,-12} {text}`, last item selected, capped at 400 lines. Every handler logs what it decided (guard hits, catches, dialog results).
- **Bottom bar** `panelActions` (1288×44, buttons 36 px high): the **success path**, the **progress path** (slow/rapid → guard), at least one
  **failure path** (outage switch, constraint violation, stale token, deleted row) and the **recovery**; `Clear trace` anchored right (x 1178, 110 wide).
  Keep the Module 1 buttons (slow count, rapid, break, restore, anti-pattern) reachable — a second row `panelActions2` under the first is fine.
- Designer-style `*.Designer.cs` with `InitializeComponent()` (explicit `Location`/`Size`, `SuspendLayout`/`ResumeLayout`, fields at the bottom)
  so the files open in the Wisej Designer; code-behind in `*.cs` with `#region` blocks per path; short handlers calling services.
- Dialog forms (`TicketEditorForm`, `ConflictDialog`): `Form`, `StartPosition = FormStartPosition.CenterParent`, `ShowInTaskbar = false`,
  fixed size, `AcceptButton`/`CancelButton` set, `Text` = "Add ticket" / "Edit ticket T-1004"; they also log into the parent trace through an
  `Action<string> trace` passed in the constructor (the dialog has no trace list of its own).

## Docs (`docs/`) and README

One Markdown file per lab deliverable, named after what the lab asks for (`ModelAndDeleteBehaviours.md`, `MigrationAndSeed.md`,
`TicketBrowserBinding.md`, `PagingInTheDatabase.md`, `EditorBinding.md`, `SaveDeleteFlows.md`, `ValidationLayers.md`,
`NegativeTests.md`, `BeforeAfterMeasurements.md`, `RelatedDataDecisions.md`, `ConcurrencyResolution.md`, `DeploymentNotes.md`,
`CapstoneReview.md`, …), each with a short **Evidence** section describing what the running app shows (trace lines) and which test proves it.
The README carries: **Run it** (exact command + port), **What to click** (button → path → what you should see), **Deliverables** table,
**Where things live** tree, **Lab steps → where in the code** table, **Self-check answers** (the lesson's checkpoint questions, from
`assets/courses/ef-core-binding/lessons/efNs2.html` "Knowledge check"/"Reflect" and the lab guide's self-check), and **Verified / unverified**.
Never point learners at snippet/pattern files — the samples are the reference. Never mention the SCORM pack or `code_snippets/`.

## Course sources (read them before building a module)

- Plan: `D:/Projects/Netlify/WWW-LearnWisej/scripts/course-plans/ef-core-binding.json` (objectives, lab objective, deliverables per module).
- Lab guide steps: `D:/Projects/Netlify/WWW-LearnWisej/assets/courses/ef-core-binding/labs/mN.json`; code checks `labs.js`.
- Lesson prose: `assets/courses/ef-core-binding/lessons/efNs1.html` (concepts, code) and `efNs2.html` (practice, checks).
- Walkthrough video captions/scenes: `D:/Projects/Netlify/WWW-LearnWisej/scripts/walkthrough/ef-core-binding/mN/app.jsx` (`CAPS`) and `content.json`.
- Original lab text: `C:/Users/matte/Desktop/Wisej.NET Learning/Wisej_NET_Intermediate_Data_Binding_with_EF_Core_Course_Pack_SCORM/course_materials/labs/lab_0N.md`
  and `modules/module_0N.md`; reference shapes in `code_snippets/` (SQL Server flavoured — adapt to SQLite, never copy the `(localdb)` string).
