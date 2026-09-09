# Control naming — every control on the System Dashboard, and why it is called what it is

The lesson "Using the Wisej.NET Designer" says clear names are a developer habit: don't leave controls as
`button1`, `label1` or `textBox1` once the page gets real behaviour, because the event code reads through them.
This is the naming pass for `DashboardWindow`, control by control, in the order the Designer created them.

## 1. The lab controls (the ones the lesson and lab guide name)

| Weak name (Designer default) | Better name | Type | Why |
|---|---|---|---|
| `label1` | `lblTitle` | `Label` | It is the big *System Dashboard* heading. `lbl` says "label", `Title` says which one. |
| `label2` | `lblStatus` | `Label` | It shows the current status (*Idle / Running / Stopped / Degraded*); every handler sets `lblStatus.Text`. |
| `panel1` | `pnlServices` | `Panel` | It only groups the three service indicators; `pnl` + what it contains. |
| `label3` | `lblServerStatus` | `Label` | *Server: Online / Offline*. Named after the service it reports, not its position. |
| `label4` | `lblDatabaseStatus` | `Label` | *Database: Online / Offline*. |
| `label5` | `lblApiStatus` | `Label` | *API Service: Online / Offline / Degraded*. |
| `button1` | `btnStart` | `Button` | The Start button; its handler is therefore `btnStart_Click` — the name tells you what clicking it does. |
| `button2` | `btnStop` | `Button` | Stop → `btnStop_Click`. |
| `button3` | `btnReset` | `Button` | Reset → `btnReset_Click`. |
| `button4` | `btnRefresh` | `Button` | Refresh → `btnRefresh_Click`. |
| `listBox1` | `lstEventLog` | `ListBox` | It stores event messages; `lst` says "list", `EventLog` says what is in it. |
| `checkBox1` | `chkSimulateOutage` | `CheckBox` | The failure switch. `chk` + what ticking it does; the handler `chkSimulateOutage_CheckedChanged` reads as a sentence. |

## 2. The course's supporting controls

| Weak name | Better name | Type | Why |
|---|---|---|---|
| `panel2` | `panelDashboard` | `Panel` | The white card that holds the lab controls. `panelX` rather than `pnlX` marks it as course chrome, not a lab control. |
| `label6` | `labelDashboardCard` | `Label` | The card's small title line. |
| `label7` | `lblHint` | `Label` | Monospace reminder of the handler names. |
| `panel3` | `panelCode` | `Panel` | The "Where your code goes" card. |
| `label8` | `labelCodeCard` | `Label` | Its title line. |
| `label9`, `label10`, `label11` | `lblFileCodeBehind`, `lblFileDesigner`, `lblFileProgram` | `Label` | One row per file the lesson lists; the name says which file. |
| `label12` | `lblVocabulary` | `Label` | The property / event / method lines. |
| `panel4` | `panelLog` | `Panel` | The Event Log card. |
| `label13` | `labelLogCard` | `Label` | Its title line. |
| `label14` | `labelLogFooter` | `Label` | The one-line round-trip reminder under the list. |
| `panel5` | `panelActions` | `Panel` | The bottom bar. |
| `button5` | `btnSimulateOutage` | `Button` | Failure path shortcut: ticks the checkbox and runs `btnRefresh_Click`. |
| `button6` | `btnRecover` | `Button` | Recovery shortcut: clears the checkbox and runs `btnRefresh_Click`. |

Prefixes used: `lbl` label, `btn` button, `pnl` panel that groups lab controls, `lst` list box, `chk` check box;
`panelX` / `labelX` for the course's card chrome that no handler ever touches.

## 3. Which file each name appears in

| Name | `DashboardWindow.Designer.cs` | `DashboardWindow.cs` | `Program.cs` |
|---|---|---|---|
| `DashboardWindow` | `partial class DashboardWindow`, `this.Name = "DashboardWindow"` | `partial class DashboardWindow : Form`, `DashboardWindow_Load` | `new DashboardWindow().Show()` |
| `lblTitle` | created, `Text = "System Dashboard"`, font 20 bold | – (never changes) | – |
| `lblStatus` | created, `Text = "Status: Idle"`, grey | `SetStatus` changes `Text` and `ForeColor` | – |
| `pnlServices` | created, holds the three indicators | – | – |
| `lblServerStatus`, `lblDatabaseStatus`, `lblApiStatus` | created, `Text = "…: Offline"`, red | `ShowServices` changes `Text` and `ForeColor` | – |
| `btnStart`, `btnStop`, `btnReset`, `btnRefresh` | created, `Text`, `Click += …_Click` | `btnStart_Click`, `btnStop_Click`, `btnReset_Click`, `btnRefresh_Click` | – |
| `chkSimulateOutage` | created, `Text`, `CheckedChanged += …` | `chkSimulateOutage_CheckedChanged`; `Checked` read in `btnRefresh_Click`, set in Reset and the bottom bar | – |
| `lstEventLog` | created, monospace font | `AddLog` (`Items.Add`), `btnReset_Click` (`Items.Clear`) | – |
| `btnSimulateOutage`, `btnRecover` | created, `Click += …` | `btnSimulateOutage_Click`, `btnRecover_Click` | – |
| `panelDashboard`, `panelCode`, `panelLog`, `panelActions`, `label*Card`, `labelLogFooter`, `lblHint`, `lblFile*`, `lblVocabulary` | created and styled | – | – |

Rule of thumb visible in the table: a control's name is **declared** and **styled** in the Designer file and
**used** in the code-behind. If a name shows up in `DashboardWindow.cs`, it needs to be a name you can read
in a sentence — `lblStatus.Text = "Status: Running"` reads; `label2.Text = "Status: Running"` does not.

## Evidence

- Open `DashboardWindow.Designer.cs`: every `this.X = new Wisej.Web.…()` line in `InitializeComponent()` uses a
  name from the tables above; there is no `button1`, `label1`, `panel1` or `listBox1` anywhere in the project
  (`grep -rn "button1\|label1\|listBox1\|panel1" *.cs` finds nothing).
- Run the app: hover each of the four buttons — the tooltip names its handler (`btnStart_Click: …`), and the
  `lblHint` line under the checkbox lists all four handler names. The first log lines after Load list the controls
  `InitializeComponent()` built, by name.
- Rename any lab control in the Designer file without renaming it in `DashboardWindow.cs` and `dotnet build`
  fails with `CS0103: The name 'lblStatus' does not exist` — the two files agree on names or nothing compiles.
