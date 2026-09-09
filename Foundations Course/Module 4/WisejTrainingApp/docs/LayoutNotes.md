# Layout notes — SplitContainer, Dock and AutoSize on the ticket screen (lab deliverable)

Lesson s17: a data-binding page needs two areas — a large record list and a detail editor — and three
layout tools arrange them without fighting the page. This is how `TicketsWindow.Designer.cs` uses them.

## 1. The SplitContainer (lesson s17 §1)

| Step | What the Designer did here | Why |
|---|---|---|
| 1 | Dropped a `SplitContainer` named `splitContainer1` on the form. | Panel1 / Panel2 for a master-detail screen. |
| 2 | `splitContainer1.Dock = DockStyle.Fill` | It stretches with the browser window (under the docked header). |
| 3 | `splitContainer1.Orientation = Orientation.Vertical` | Left panel and right panel. |
| 4 | `Panel1.Controls.Add(dgvTickets)` with `dgvTickets.Dock = DockStyle.Fill` | The ticket list belongs on the large side. |
| 5 | `Panel2.Controls.Add(grpTicketDetails)` — labels, text boxes, two combos, a date picker, the Save button. | The detail editor belongs on the side panel. |
| 6 | `splitContainer1.SplitterDistance = 760` | About 760 px for the grid, ~580 px for the details; drag the splitter at runtime to change it. |

```csharp
splitContainer1.Dock = DockStyle.Fill;
splitContainer1.Orientation = Orientation.Vertical;
splitContainer1.SplitterDistance = 760;
dgvTickets.Dock = DockStyle.Fill;          // ticket list fills the left panel
grpTicketDetails.Dock = DockStyle.Top;     // fixed-height editor at the top of the right panel
btnSaveTicket.Text = "Save Ticket";
```

`Panel1.Padding` / `Panel2.Padding` give the docked children the grey margin around the cards, so
no card is hard-positioned against the panel edge.

## 2. Dock (lesson s17 §2)

| Dock | Use it for | On this screen |
|---|---|---|
| `Fill` | The control that should use all available space. | `splitContainer1` in the form; `dgvTickets` in Panel1; `panelLog` (the event-log card) in Panel2. |
| `Top` | Header bars, titles, short toolbars. | `panelHeader` (page title + status label); `grpTicketDetails` in Panel2 — fixed height, the log takes the rest. |
| `Bottom` | Status bars or command areas. | `panelState` in Panel1 — the three-line "UI / business / persisted" card sits under the grid. |
| `Left` | Navigation panels, sidebars. | Not used here (that was the Module 3 shell). |
| `None` | Controls you position manually. | Every label, textbox, combo, date picker and button inside `grpTicketDetails`; `lblStatus` is anchored `Top | Right` instead. |

Order matters when several children of one parent are docked: the Designer adds the `Fill` control
first and the `Top` / `Bottom` controls after it, so the edges are taken first and `Fill` gets what is
left. In Panel2 that is `Controls.Add(panelLog)` then `Controls.Add(grpTicketDetails)`.

**Beginner rule applied:** only containers and the grid are `Dock = Fill`. The eleven input controls in
the details group keep normal positions. Nothing covers anything else, and the page still resizes:
the grid grows, the details keep their height, the log absorbs the rest.

## 3. AutoSize (lesson s17 §3)

| Control | AutoSize here | Why |
|---|---|---|
| Field captions (`lblTitleCaption`, `lblStatusCaption`, …) | `true` | Short labels fit their text and stay readable in any theme font. |
| `lblPageTitle`, `lblStatus`, `lblState`, card titles | `false` (fixed `Size`, `TextAlign`) | Their text changes at runtime; a fixed box keeps the header and the State card stable and lets `lblStatus` right-align. |
| Buttons | `false` | Fixed widths so the three command buttons line up (`Save Ticket` 120, `Refresh` 100, the validation button 280). |
| `grpTicketDetails` (GroupBox), the cards (Panel) | `false` | The details area must not jump while the user edits fields. |
| `dgvTickets` | `false` — `Dock = Fill` | The grid fills the space; it never resizes to its content height. |
| Grid columns | `AutoSizeColumnsMode = Fill` with `FillWeight` 8 / 34 / 18 / 13 / 10 / 17 | Six columns share the width; Title gets the most, Id the least. |

```csharp
dgvTickets.AutoGenerateColumns = false;
dgvTickets.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
dgvTickets.MultiSelect = false;
dgvTickets.ReadOnly = true;
dgvTickets.Dock = DockStyle.Fill;
dgvTickets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
```

## 4. Layout tools are not data (lesson s17 §4)

| Concern | Where it lives on this screen |
|---|---|
| Screen layout | `TicketsWindow.Designer.cs` — `splitContainer1`, Dock, AutoSize, `panelHeader`, `grpTicketDetails`, the two cards, the grid, the buttons. |
| Record data | `Models/Ticket.cs`. |
| Get / save logic | `Services/TicketService.cs`. |
| Current selection | `ticketsBindingSource.Current` (the `SelectedTicket` property) and the grid's selected row, kept in step by the BindingSource. |
| Saving changes | `btnSaveTicket_Click` in `TicketsWindow.cs`: service call, then `ResetBindings(false)`, then the status label. |

Drag the splitter or resize the browser: the layout changes, the ticket data, the selection and the
service do not. Nothing in the Designer file knows what a ticket is beyond the column
`DataPropertyName`s.

## Evidence

- Resize the browser window: the header stays 56 px tall across the top, the grid and the log grow and
  shrink, the details group keeps its 352 px, the State card stays under the grid. No control overlaps.
- Drag the splitter left or right: the grid's six columns re-share the width (`Fill` mode); the details
  group and the log card follow Panel2's width because they are docked and their inner cards are
  anchored `Left | Right`.
- The event log's second line on start-up names the layout: `InitializeComponent() built splitContainer1
  (Dock Fill, Vertical, SplitterDistance 760), dgvTickets, grpTicketDetails`.
- Change a theme font (Default.json `"theme"`) and the AutoSize captions still fit; the fixed-size
  status label still right-aligns.
