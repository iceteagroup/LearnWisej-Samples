# Dashboard events — properties, events, handlers and methods on the System Dashboard

The lesson "Properties, events and Designer code" describes a control with four ideas: a **property** is a
setting, an **event** is something that happens, an **event handler** is the method that runs when it happens,
a **method** is a command you call. This is the Module 2 screen, `DashboardWindow`, read through those four ideas.

## 1. The vocabulary, applied to this screen

| Concept | On the dashboard | Where it is written |
|---|---|---|
| Property | `lblTitle.Text = "System Dashboard"`, `lblStatus.Text = "Status: Idle"`, `lblStatus.ForeColor`, `btnStart.Text = "Start"`, `chkSimulateOutage.Checked` | Designer file for the starting values; `DashboardWindow.cs` when a handler changes them |
| Event | `btnStart.Click`, `btnStop.Click`, `btnReset.Click`, `btnRefresh.Click`, `chkSimulateOutage.CheckedChanged`, `DashboardWindow.Load` | wired in `DashboardWindow.Designer.cs` (`this.btnStart.Click += new System.EventHandler(this.btnStart_Click);`) |
| Event handler | `btnStart_Click`, `btnStop_Click`, `btnReset_Click`, `btnRefresh_Click`, `chkSimulateOutage_CheckedChanged`, `DashboardWindow_Load` | `DashboardWindow.cs` |
| Method | `lstEventLog.Items.Add(…)`, `lstEventLog.Items.Clear()`, `monitor.CheckAll()`, `AddLog(…)`, `ShowServices(…)`, `SetStatus(…)` | `DashboardWindow.cs` and `Services/ServiceMonitor.cs` |

The "Where your code goes" card shows the same three lines on screen (`lblVocabulary`) so the words stay next
to the controls they describe.

## 2. What each handler does

| Button | Handler | Screen | Log line |
|---|---|---|---|
| Start | `btnStart_Click` | `monitor.Start()`; all three indicators **Online** (green); `lblStatus` = *Status: Running* (green) | `Dashboard started.` |
| Stop | `btnStop_Click` | `monitor.Stop()`; all three **Offline** (red); `lblStatus` = *Status: Stopped* (red) | `Dashboard stopped.` |
| Reset | `btnReset_Click` | `lstEventLog.Items.Clear()` first, then `monitor.Reset()`, outage switch off, all **Offline**, `lblStatus` = *Status: Idle* (grey) | `Dashboard reset.` (the only line left) |
| Refresh | `btnRefresh_Click` | `monitor.CheckAll()` re-paints the indicators; status follows the monitor (Running when started) | `Status refreshed — Server 12 ms · Database 31 ms · API Service 18 ms` (numbers change each time) |
| Refresh with the outage switch on | `btnRefresh_Click` | `lblApiStatus` = *API Service: Degraded* (amber); `lblStatus` = *Status: Degraded* (amber) | `API Service check failed: timeout after 2000 ms` |

The four handlers have the same shape the lesson shows: change the monitor, paint the labels, set the status,
log one line. None of them talks to `lstEventLog` directly and none of them knows a colour value — that is
what the helpers are for.

## 3. The beginner lifecycle (what the log shows on load)

```
Program.Main                     → new DashboardWindow().Show()
DashboardWindow()                → InitializeComponent()   (DashboardWindow.Designer.cs builds the controls)
DashboardWindow_Load             → ready; Status: Idle, every service Offline
user clicks Start                → browser sends btnStart.Click → btnStart_Click runs on the server
btnStart_Click                   → monitor.Start(), ShowServices(), SetStatus(), AddLog()
request ends                     → Wisej.NET sends the changed Text / ForeColor / list items → browser updates
```

The three lifecycle lines are the first three entries in the event log every time the app starts, so the order
"startup → window created → controls built → Load → user events" is something you can see rather than remember.
A control is only usable after `InitializeComponent()` has run — which is why the handlers never run before Load.

## 4. The AddLog habit

```csharp
private void AddLog(string message)
{
    string time = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
    lstEventLog.Items.Add($"{time}  {message}");
    lstEventLog.SelectedIndex = lstEventLog.Items.Count - 1;
}
```

Every log line on the screen goes through this one method — the four button handlers, the checkbox handler,
the bottom-bar shortcuts and `Load`. Pulling the timestamp and the `Items.Add` call out of each handler is
what keeps `btnStart_Click` four lines long. If the log ever needs a different format (the lesson's
`"hh:mm tt"`, a date, a level), one method changes.

The same habit is applied twice more:

- `ShowServices(List<ServiceCheck>)` is the only code that knows which label shows which service and which
  colour each `ServiceState` gets.
- `SetStatus(string, StatusKind)` is the only code that colours `lblStatus`.

## 5. Safe event-handling habits (lesson §5) — and where each one is visible

| Habit | Where |
|---|---|
| Use event handlers for user actions like button clicks | the four `btn*_Click` handlers and `chkSimulateOutage_CheckedChanged` |
| Keep each handler easy to read | each handler is 4–8 lines, reads top to bottom: monitor → labels → status → log |
| Move repeated logic into helper methods such as `AddLog()` | `AddLog`, `ShowServices`, `SetStatus` |
| Don't edit Designer-generated layout code by hand | `DashboardWindow.Designer.cs` only creates controls, sets properties and wires events; no behaviour lives there |
| Don't hide slow database or API calls inside simple UI events | `ServiceMonitor` fakes its checks instantly; the "timeout after 2000 ms" is reported, not waited for. A real check would belong in a background job (Module 6) |
| Keep per-user state in the instance, not `static` | `private readonly ServiceMonitor monitor` is a field of the window; each browser session gets its own |

## Evidence

- On load: three lifecycle lines in the log; `lblStatus` grey *Status: Idle*; the three indicators red *Offline*.
- **Start**: all three indicators turn green *Online*, the status turns green *Status: Running*, the log gains
  `Dashboard started.`.
- **Refresh** (started, no outage): the log gains `Status refreshed — Server N ms · Database N ms · API Service N ms`
  with different numbers each click; indicators stay green.
- **Simulate API outage on Refresh** ticked, then **Refresh** (or the bottom-bar *Simulate API outage + Refresh*):
  `lblApiStatus` turns amber *API Service: Degraded*, `lblStatus` turns amber *Status: Degraded*, the log gains
  `API Service check failed: timeout after 2000 ms`. Server and Database stay as they were.
- Untick, then **Refresh** (or *Clear outage + Refresh*): `lblApiStatus` back to green *Online* (red *Offline* if the
  dashboard was never started), `lblStatus` back to *Status: Running* when started, and a fresh `Status refreshed — …` line.
- **Stop**: indicators red *Offline*, status red *Status: Stopped*, log `Dashboard stopped.`.
- **Reset**: the list empties and shows exactly one line, `Dashboard reset.`; status grey *Status: Idle*; the outage
  checkbox is cleared.
