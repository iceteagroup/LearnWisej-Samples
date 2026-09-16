# Scenarios

A scenario is a named, repeatable sequence of user actions. Everything else in this course — the
baseline, the budget, every profiler trace, every before/after table — is attached to one of these
names. A measurement without a scenario name cannot be compared with anything.

Each scenario is wrapped in a `ScenarioProbe` scope whose `Scenario` and `UserAction` are exactly the
names in this table, so the PERF records, the budget and this document all use one vocabulary.

| Scenario | Where | The user action, exactly | Wrapped in |
|---|---|---|---|
| `Dashboard/Refresh` | Dashboard tab | click **Refresh** once, wait until the KPIs and the chart have repainted | `DashboardPage.RunRefresh` |
| `Tickets/Search` | Tickets tab | Status = *Open*, Rows = *5000*, click **Search tickets** once, wait until the grid has repainted | `TicketGridPage.RunSearch` |
| `Tickets/Redraw` | Tickets tab | after a search, click **Redraw** once — no query, the rows are rebuilt from what is already in memory | `TicketGridPage.btnRedraw_Click` |
| `Tickets/Export` | Tickets tab | after a search, click **Export CSV** once, wait until the file name appears | `TicketGridPage.btnExport_Click` |
| `Customers/LoadTree` | Customers tab | click **Load the customer tree** once, wait until the tree has drawn | `CustomerTreePage.LoadTree` |
| `Customers/ExpandNode` | Customers tab | click **Expand the first branch** once (or expand any node by hand) | `CustomerTreePage.treeView1_BeforeExpand` |

## What is inside the scope, and why

The probe scope goes around the **whole user action**: the query, the projection and the control
mutation. It is tempting to wrap the query alone — the number is smaller and easier to explain, and it
is the wrong number. On this machine the query behind `Dashboard/Refresh` takes about 90 ms; the user
waits for roughly half a second, because the other 400 ms are materialisation, formatting and control
work that a query timer never sees.

What the scope does **not** include is everything after the handler returns: serialising the update,
sending it, and the browser applying it. That is the fifth cost bucket, and it is measured in the
browser network panel, not by the probe. It is why `Customers/LoadTree` can report 312 ms on the
server while the screen takes visibly longer to settle — three thousand nodes have to reach the
browser as well.

## The environment is part of the scenario

Two runs of the same scenario are only comparable when these are also the same:

- build configuration (**Release** for every recorded measurement),
- dataset (50,000 tickets, 3,200 customer nodes — the app prints both in its header),
- warm-up (one discarded run of every scenario before the first recorded one),
- session count (one browser tab unless the scenario says otherwise),
- and the machine, with no profiler attached while the wall-clock numbers are taken.

`Baseline.md` records those for the numbers in it. `Budget.md` says what each scenario is allowed to
cost and which tool would prove or disprove it.
