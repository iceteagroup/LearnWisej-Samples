# TicketOps Live cookbook — Real-Time Apps with Server Push (Wisej-4 4.1.0, .NET 10)

The conventions every module sample of this course follows. Facts marked **(verified)** were executed in
the browser while building Module 1 (or the Application Integration / Foundations samples, same framework
build). Facts marked **(unverified)** come from the Wisej.NET XML docs; implement them, make sure
`dotnet build` passes, and say so in your report so the reviewer can check them at runtime.

## Project layout (copy `_template`)

```
Module N/
  TicketOpsLive.slnx                  (from _template)
  .gitignore                          (from _template)
  README.md                           (what it shows, how to run, what to click, lab steps → code, self-check answers)
  TicketOpsLive/
    TicketOpsLive.csproj              (from _template; TargetFrameworks = net10.0-windows;net10.0 — keep it)
    Program.cs  Startup.cs  Default.html  Default.json  Web.config
    Properties/launchSettings.json    (port: Module N → http://localhost:530N, i.e. 5301 … 5307)
    MainPage.cs + MainPage.Designer.cs   the page the course names (a Wisej.Web.Page; Application.MainPage = new MainPage())
    Models/  Services/                as the module needs them (Ticket, TicketStatus, TicketChangedEventArgs, TicketHub, ImportJob …)
    docs/                             the lab deliverables as Markdown
```

- Namespace is always `TicketOpsLive` (the lab: "Create a Wisej.NET Web Page Application named TicketOpsLive. Add a main page named MainPage.").
- `Program.Main(NameValueCollection args)` is the session entry point (`Default.json` → `"startup"`): `Application.MainPage = new MainPage();`.
- Run: `dotnet run -f net10.0 --urls http://localhost:530N` from the project folder. Build with `dotnet build -nologo -v q` and fix every error (warning CS7022 is already silenced).
- Do not run the app yourself and do not start servers; the reviewer runs it in the browser.
- Control names exactly as the lab guide asks (`statusPanel`, `clockLabel`, `startImportButton`, `importProgressBar`, `liveModeCheckBox`, `cadenceComboBox`, `ticketsGrid`, `notificationsList`, …). Never `button1`.

## Real-time runtime facts — **verified in Module 1** unless marked otherwise

- **`Application.StartTask(Action)`** runs the action on a background thread **with the session context kept**: the
  task can read `Application.ClientId` / `Application.SessionId` / `Application.IsWebSocket` and change the page's
  controls directly. Nothing reaches the browser until the task calls `Application.Update(this)`.
- **`Application.Update(this)`** (the Page is an `IWisejComponent`) pushes every pending control change in one flush.
  The 5-step loop of the walkthrough (`label.Text = …; Application.Update(this); Thread.Sleep(600);`) works exactly as shown.
- **`Application.Update(this, () => { … })`** executes the callback in the session context and then pushes once —
  the cleanest shape for loops: compute off the request thread, then apply all UI changes inside the callback.
  Use the callback form whenever code may run **outside** a `StartTask` thread (hub events, timers of a global
  service): capture `IWisejComponent context = Application.Current;` while in context (Load / a click) and call
  `Application.Update(context, () => …)` later **(the Current/context part is unverified — the XML docs document it; verify it builds)**.
- **`Application.IsWebSocket` is `false` during `MainPage_Load`** — the socket opens right after the first response —
  and `true` on every later event. Consequence **(verified)**: `Application.StartPolling(1000)` placed in Load starts
  real HTTP polling (one `POST app.wx` per second, "Wisej: Poll request." in the console) that keeps running after the
  socket connects. Do not put it in Load. Pattern used by the samples:
  ```csharp
  private int _pushers; private bool _polling;
  void BeginPush() { _pushers++; if (Application.IsWebSocket || _polling) return; Application.StartPolling(1000); _polling = true; }
  void EndPush()   { if (_pushers > 0) _pushers--; if (_pushers > 0 || !_polling) return; Application.EndPolling(); _polling = false; }
  ```
  call `BeginPush()` where the out-of-bound work starts and `EndPush()` in its `finally`.
- **Fallback demo (verified):** `"enableWebSocket": false` in `Default.json` makes `IsWebSocket` stay `false`; with
  `StartPolling(1000)` the heartbeat pushes still arrive (delivered by the polls, ~1 s late) and `EndPolling()` stops
  the polls. Mention this switch in the README so the reviewer can see the fallback.
- **Cadence (verified numbers):** 100 model changes 10 ms apart → 100 pushes take ~1570 ms; the same 100 changes pushed
  every 10th → 10 pushes, same wall time. Push final state immediately, throttle visible progress.
- Exceptions inside the task must be caught **inside** the task: `try { … } catch (Exception ex) { log; safe message }
  finally { re-enable buttons; Application.Update(this); }` — verified: the UI recovers and the buttons come back.
- Stop conditions: a `volatile bool _running` **instance** field + `this.IsDisposed` in the loop condition, and
  `this.Disposed += (s, e) => _running = false;`. Starting twice must be refused by the flag (verified).
- `Application.ClientId` (GUID string), `Application.SessionId` (string), `Application.Title` — all fine from the task thread.
- **`Application.ClientId` identifies the BROWSER, not the session (verified in Module 2):** two tabs of the same
  Chrome report the same `ClientId` and different `SessionId`s. Key registries, hubs and per-tab bookkeeping on
  `Application.SessionId`; show `ClientId` as "the browser".
- **`Application.Update(context, () => …)` from a foreign thread works (verified in Module 2):** a global service
  raised its event on a thread-pool thread (`Task.Run`), the subscriber captured `IWisejComponent _context =
  Application.Current` on Load and the callback updated the right tab's controls and pushed them. `Application.Current`
  is an `IWisejComponent`. Inside such a callback `Application.IsWebSocket` may read `false` even though the push arrives.
- `Application.Session` is a `dynamic` bag per session (`Application.Session.Counter = 0;`, a missing member reads `null`) — **verified**.
- `Application.Browser` returns `Wisej.Core.ClientBrowser`: `Type` (string, "Chrome"), `Version` (**int**, 152), `OS` ("Windows"),
  `Device` ("Desktop") — **verified**; `UserAgent`, `Language`, `ScreenSize`, `TimezoneId`, `IPAddress`, `TabId` exist per the XML docs.
- `Application.ApplicationExit` (EventHandler) **fires on `Application.Exit()` (verified)**: the handler ran, wrote to the
  server console and unsubscribed; `Application.Exit()` also closes the browser tab. `Application.SessionTimeout`
  (`HandledEventHandler`) fires about two minutes before the idle timeout dialog (verified, "is about to time out" logged);
  `Application.RunInContext(context, action)` executes without pushing **(unverified at runtime)**.
- `Application.GetInstance<T>(ref SessionReference<T>, Func<T>)` returns a session-static singleton **(unverified)**.
- `Wisej.Web.Timer(components)` with `Interval`, `Tick`, `Start()/Stop()`, `Enabled` — a Component with no visual surface;
  its Tick runs as a normal request, so changes made in Tick are returned with that request, no Update() needed
  (**verified in Module 4**: one `← request refreshTimer_Tick` per tick). **Assigning `Interval` on a running timer
  reprograms it live** (verified: 1 s → 250 ms, ticks 250 ms apart afterwards). The timer lives in the browser: a
  background tab throttles it to ≥ 1 s (Chrome), so cadence demos need the tab in front.
- **Global hub fan-out must run OFF the publisher's request thread (verified in Module 6):** raising the event
  synchronously on the publishing session's request thread made the other session's `Application.Update(context, …)`
  callback run while the publisher's context was ambient, and that session's pending control changes were flushed
  into the publisher's response (the publisher's trace and list showed the other tab's lines). Raise with
  `Task.Run(() => foreach handler …)` — like Module 2's `SessionRegistry` — and every subscriber restores its own
  context cleanly. `AlertBox.Show` inside such a callback pops in the right tab.
- `Wisej.Web.ProgressBar.Value` (0..100), `ListBox.Items.Add/Insert(0, …)/RemoveAt(0)/Clear()`, `ListBox.SelectedIndex`,
  `Label.Text/ForeColor/BackColor/Visible`, `Button.Enabled`, `Panel.BorderStyle = Wisej.Web.BorderStyle.Solid` — all verified.
- `AlertBox.Show(text, MessageBoxIcon.Warning, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000)` — always TopRight.
- Fonts: `new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold)`, monospace `new System.Drawing.Font("monospace", 9F)`.
- Data binding (from the Foundations cookbook, signatures checked by reflection, **unverified at runtime**):
  `var source = new Wisej.Web.BindingSource(); source.DataSource = new BindingList<Ticket>(); grid.AutoGenerateColumns = false;
  grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Title", HeaderText = "Title", Name = "colTitle", Width = 220 });
  grid.DataSource = source; source.ResetBindings(false); grid.CurrentRow?.DataBoundItem as Ticket; grid.Rows[i].Selected = true;
  grid.CurrentCell = grid.Rows[i].Cells[0]; grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect; column.DefaultCellStyle.Format = "HH:mm:ss";`
- **Grid binding (verified in Module 5):** `BindingList<Ticket>` → `Wisej.Web.BindingSource` → `DataGridView` with
  `AutoGenerateColumns = false` and `DataGridViewTextBoxColumn { DataPropertyName, HeaderText, Width }` works; a column
  bound to a get-only property (`Marker`) renders; `DefaultCellStyle.Format = "HH:mm:ss"` formats a `DateTime`;
  `Insert(0, …)` + `ResetBindings(false)` adds a row at the top without rebinding; `Rows[i].Selected = true` +
  `CurrentCell = Rows[i].Cells[0]` restores the selection (the highlighted row stays on the user's ticket while rows
  arrive above it). **Gotcha:** `ResetBindings(false)` raises `DataGridView.SelectionChanged` synchronously on the
  server, twice, pointing at whatever now sits at the old row index — mute your handler with a flag around the reset
  and restore the selection afterwards (`ResetBindingsQuietly()` in Module 5).
- **`Wisej.Web.TabControl` / `TabPage` (verified in Module 7):** `TabPages.Add(page)`, `SelectedTab`,
  `SelectedIndexChanged` all work. Two gotchas: a `TabPage.Text` containing `&` treats it as a **mnemonic**
  ("Health & config" rendered as "Health <u> </u>config") — use `·` instead; and a tab's controls are created
  **lazily**, so they are absent from `qx.core.ObjectRegistry` until that tab has been shown once (select the tab,
  wait, then look them up when driving from the console).
- **`Wisej.Web.CheckedListBox` (verified in Module 7):** `CheckOnClick`, `Items.Add`, `CheckedItems.Count` and
  `AfterItemCheck(ItemCheckEventArgs e)` with `e.Index` / `e.NewValue` compile and render.
- **`Application.Configuration` (verified in Module 7):** `SessionTimeout`, `PollingInterval`, `EnableWebSocket`,
  `Debug`, `MaxSessions`, `ThemeName` all read (180 / 1000 / True / True / **-1 = unbounded** / Bootstrap-4).
  `Application.StartupPath` resolved to the **project folder** under `dotnet run`, so a config file kept next to
  `Default.json` with `CopyToOutputDirectory=Never` is still readable at runtime.
- Console logging: `Console.Error.WriteLine("[TicketOpsLive] …")` is the "server log" of these samples (the preview shows it).
- **Idle timeout (verified in Module 3):** with the default `Default.json` an idle page shows Wisej's built-in
  "Session Timeout — your session will expire in 2:00" dialog after about two minutes without a request; a
  WebSocket push alone does not count as activity. `sessionTimeout` in `Default.json` (seconds) and the
  `Application.SessionTimeout` event (set `e.Handled = true` to suppress the dialog) are the knobs — Module 7 reviews them.
- **Client-side checkbox (verified):** in the browser console `checkBox.setValue(true)` does NOT reach the server;
  `checkBox.execute()` toggles it and syncs. A disabled button's `execute()` does nothing (so "start twice" tests hit the
  `Enabled = false` guard first, not the flag).

## Threading rule of thumb

Two threads can touch the same page: the request thread (clicks, timer ticks) and the task thread. Keep the task
side small and predictable: compute first, then apply the control changes inside `Application.Update(this, () => …)`
(one flush) or right before `Application.Update(this)`. Never mutate a bound list from a thread that is not in the
session context; never store page/control references in a static or global service — subscribe with a handler and
unsubscribe on `Disposed` / `ApplicationExit`.

## UI conventions used by every sample (so the samples feel like one course)

- `MainPage : Page`, `Size = 1348×700`, light grey background `Color.FromArgb(238,242,247)`, white cards (`Panel`,
  `BorderStyle.Solid`), card titles `"default" 14F Bold`, captions `"monospace" 9F Bold`, logs `"monospace" 9F`.
- A **status strip** on top (`statusPanel`, 1288×92) whenever the module has live status: clock / connection / activity / load.
- Right-hand card **"Server → Browser · live push trace"** (`panelTrace` + `listTrace`, monospace): every push, request and
  server decision logged through one helper, capped at 400 lines, last item selected:
  ```csharp
  private void AddTrace(TraceDirection direction, string name, string payload)   // Push → "→ push    ", Request → "← request ", Server → "• server  "
  { listTrace.Items.Add($"{DateTime.Now:HH:mm:ss.fff}  {prefix} {name,-30} {payload}"); while (listTrace.Items.Count > 400) listTrace.Items.RemoveAt(0); listTrace.SelectedIndex = listTrace.Items.Count - 1; }
  ```
- A status label (`labelStatus`, "● …") green `Color.FromArgb(31,157,87)` / amber `Color.FromArgb(232,161,60)` / red
  `Color.FromArgb(224,86,59)`, a banner label (`labelBanner`, hidden until needed; error `253,236,234`/`178,59,39`, warn
  `255,244,229`/`146,64,14`, info `230,240,251`/`21,79,143`) and a monospace `labelState` starting with
  `SERVER STATE (authoritative · this session only)` that prints the instance fields (running flags, counters, IsWebSocket, ClientId, SessionId).
- Bottom bar (`panelActions`, 1288×44, buttons 36 px high) that exercises the **success path**, the **progress path**
  (task / timer), at least one **failure path** (simulated exception inside the task, rejected input, wrong-thread demo)
  and the **recovery** (start again / resync). `Clear trace` anchored right.
- Designer-style `MainPage.Designer.cs` with `InitializeComponent()` so the file opens in the Wisej Designer; code-behind in
  `MainPage.cs` with `#region` blocks per path. Short handlers that call helpers/services.
- Every out-of-bound loop: bounded or stoppable, `IsDisposed` checked, exceptions caught inside, `finally` restores the UI.

## Docs (`docs/`)

Write each lab deliverable as its own Markdown file named after what the lab asks for (`ArchitectureNote.md`,
`UpdateMechanisms.md`, `SessionInspectorNotes.md`, `CleanupRules.md`, `BackgroundImportNotes.md`, `CadencePolicy.md`,
`LiveBindingNotes.md`, `TicketHubDesign.md`, `ProductionChecklist.md`, `DemoScript.md`, …). Include a short **Evidence**
section describing what the running app shows for each path (trace lines). The README carries a **What to try** table
(button → path → what you should see), a **Lab tasks → where in the code** table, and a **Self-check** section answering
the lesson's reflection / checkpoint questions.

## Browser notes for the reviewer

- The Browser pane must be visible while testing; a hidden tab does not flush the qooxdoo queues.
- Mouse clicks on the scaled-down page are unreliable; drive buttons from the console:
  `const reg=qx.core.ObjectRegistry.getRegistry(); const byName={}; for (const k in reg){const w=reg[k]; if (w && w.classname==='wisej.web.Button') byName[w.getName()]=w;} byName.startButton.execute();`
- `document.body.innerText` read from inside the same script is stale; use the page-text tool after the actions.
- **A background/hidden tab does not render pushed label and list changes** (the qooxdoo queues flush on
  requestAnimationFrame, which is paused; the DataGridView still repaints, which is confusing). Front the tab and run
  `qx.html.Element.flush(); qx.ui.core.queue.Manager.flush();` before reading, or the page text shows stale counters
  while the server (and the grid) already moved on. Verified in Module 6 with two tabs.
- Tabs opened from the pane share one browser profile → same `Application.ClientId`, distinct `SessionId`s.
- Driving a `ComboBox` from the console: `cb.setSelectedIndex(n)` changes only the client; use
  `const list = cb.getChildControl('list'); list.setSelection([list.getChildren()[n]]);` — that reaches the server.
  A `CheckBox` toggles with `.execute()`; a disabled `Button` ignores `.execute()`.
- The console logs `Wisej: Poll request.` for every fallback poll and `Wisej: WebSocket Request n` for socket traffic.
