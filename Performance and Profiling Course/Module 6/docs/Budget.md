# Budget

An acceptance threshold per scenario, plus the memory threshold, plus — next to each — the tool that
would prove or disprove it. The thresholds live in code as well
(`WisejPerfLab/Shell/PerfBudget.cs`), so the status line says *over* or *within* after every run
instead of only printing a number. A budget that lives in a document nobody opens is a number nobody
checks.

| Scenario | Threshold | Measured now | Which tool decides it | Why this number |
|---|---:|---:|---|---|
| `Dashboard/Refresh` | **250 ms** | 476 ms | CPU Usage | The dashboard is the landing screen; a refresh that takes longer than a quarter of a second reads as a stall rather than a redraw. |
| `Tickets/Search` | **300 ms** | 444 ms | Database, then .NET Object Allocation | A search is a deliberate action, so a slightly longer wait is acceptable — but it must stay inside the time it takes the user to move the mouse to the grid. |
| `Tickets/Redraw` | **150 ms** | — | .NET Object Allocation | A redraw does no database work at all. Anything it costs is pure server-side rebuilding. |
| `Tickets/Export` | **400 ms** | 522 ms | File I/O, then .NET Async | The export may take longer than this in total, but it may not hold the request thread: the threshold is what the **click** is allowed to cost before the work moves to the background. |
| `Customers/LoadTree` | **150 ms** | 312 ms | .NET Object Allocation + the browser network panel | Opening a tab must feel instant. The server time is only half the story; the payload is the other half. |
| `Customers/ExpandNode` | **100 ms** | 188 ms | .NET Object Allocation + the browser network panel | Expanding a node is a direct manipulation: above about a tenth of a second it stops feeling connected to the click. |
| Retained per idle session | **25 MB** | measured in Module 4 | Memory Usage (snapshot comparison) | Capacity is per session. 25 MB × 150 sessions is a number a single instance can hold; 60 MB × 150 is not. Module 7 turns this threshold into `maxSessions`. |

## These are this sample's numbers

The course lesson quotes a budget of 800 ms for the dashboard refresh and 900 ms for the ticket
search, measured against SQL Server on the author's machine, where every statement costs a network
hop. This sample runs an **in-process SQLite** database on a developer laptop, where the same work is
roughly twice as fast — the N+1 in `Tickets/Search` costs about 0.05 ms per statement here and about
0.3 ms there. Keeping the course's thresholds would have made the naive implementation pass, and a
budget that the code you are about to fix already meets teaches nothing.

So the thresholds above were written from **what the screen owes the user**, and then checked against
the baseline. That is the right order, and it is the deliverable: writing your own budget from your
own baseline is what Module 1 asks for. If your machine is faster or slower than this one, write
different numbers — and say, as this table does, which tool you would use to settle each one.

## The rule that makes a budget useful

Each threshold names a tool. If you cannot say which tool would show you whether a scenario is inside
its budget, the threshold is a wish, not a budget. That mapping is the whole of Module 2:

- time in your own code → **CPU Usage**, then **Instrumentation** for call counts
- objects and strings → **.NET Object Allocation**
- memory that never goes away → **Memory Usage**, two snapshots
- statements and their duration → **Database**
- waiting rather than working → **.NET Async** and **File I/O**
- bytes to the browser → the browser **network panel**, not a .NET tool at all
