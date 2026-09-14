# WisejPerfLab — final performance report

One document covering all six modules: scenario, environment, baseline evidence, root cause, change,
after evidence, remaining risks. A colleague who has never opened a profiler should be able to follow
it.

## Environment (unchanged throughout)

Release, no debugger, Kestrel, one browser tab, SQLite in process, **50,000 tickets and 3,200 customer
nodes**, warm-up run discarded, medians of three. Full details in `Baseline.md`; the thresholds and
their justification in `Budget.md`.

## The headline

| Scenario | Baseline (Module 1) | Now (Module 7) | Budget | Change |
|---|---:|---:|---:|---|
| `Dashboard/Refresh` | 476 ms | **113 ms** | 250 ms | **4.2× faster**, within budget |
| `Tickets/Search` | 444 ms | **66 ms** | 300 ms | **6.7× faster**, within budget |
| `Customers/LoadTree` | 312 ms | **62 ms** | 150 ms | **5× faster**, within budget |
| `Customers/ExpandNode` | 188 ms | **9 ms** | 100 ms | **20× faster**, within budget |
| `Tickets/Export` | 522 ms, request thread blocked | **171 ms**, background | 400 ms | within budget, and it no longer holds a thread |
| Retained per 50 detail-form visits | **+76 MB** | **+1.1 MB** | 25 MB per session | the leak is gone |
| SQL statements per ticket page | 5,001 | **1** | — | |
| SQL statements per tree load | 3,201 | **3** | — | |

Every one of those differences is far outside the 10–25 % noise floor the baseline measured.

## Module by module

### 1 — The model, the baseline, the budget

Three named scenarios, a `ScenarioProbe` around each, a dataset that is identical on every run, a
discarded warm-up run and medians of three. No optimisation. **Deliverables:** `Scenarios.md`,
`Baseline.md`, `Budget.md`.

The baseline's most useful output was not a number but a shape: the refresh issued **one** statement and
still cost 476 ms; the search issued **5,001**; the tree issued **3,201** to draw itself. Three
different problems, three different tools.

### 2 — The profiling workflow

Release, warm up, one tool, one scenario, one click; narrow the timeline; read the hot path down to the
first frame in your own namespace; follow up with Instrumentation for call counts; name the trace and
write the note beside it; three runs and a median.

**The finding that mattered:** the per-row formatter and the control rebuild — the two things the code
made most visible — were **7 %** of the refresh. 92 % was materialising 50,000 entities to count three
of their properties. Fixing the obvious suspects would have bought nothing.

**Deliverables:** `ProfilingWorkflow.md`, `HotPath.md`, `CpuVsInstrumentation.md`, `TraceNotes.md`,
`RootCause.md`.

### 3 — The hot path

`DashboardSnapshot` of display-ready values, built once in the service from four aggregate queries; the
KPI cards built once and updated in place; a guard so a second click cannot queue a second refresh.
**476 ms → 113 ms**, and the hot path changed shape rather than shrinking — there is no materialisation
frame left to find. **Cost:** a staleness window (shown on screen as `as of 11:31:45`), four statements
instead of one, one provider-specific SQL string. **Deliverables:** `BeforeAfter.md`, `SnapshotDesign.md`.

### 4 — Memory

Fifty open/close rounds of the ticket detail form retained **+76 MB** and left **50** handlers on a
static event. `Dispose(bool)` now unsubscribes the bus, stops and disposes the timer, nulls the binding
source, disposes the image and drops the buffer: **+1.1 MB, 0 subscribers**. The grid rows are projected
once per search instead of on every redraw.

**The line to remember:** *forms created 50 / disposed 50* — in both runs. Disposal ran all along;
collection did not, because a static event and a running timer still referenced the disposed forms.
**Deliverables:** `MemoryEvidence.md`, `DisposalChecklist.md`, `SessionMemoryBudget.md`.

### 5 — Large surfaces

The grid is virtual: a count query sets `RowCount`, and `CellValueNeeded` is answered from a page cache
(no query per cell, no formatting in the handler, no lock). The tree loads the eight regions with a
count placeholder per branch and fetches children on expand. **990 ms → 125 ms** and **510 ms → 62 ms**.

**The finding that mattered:** Wisej.NET's grid and tree already virtualise their *rendering* — the
browser held nine `TreeNode` widgets for a 3,200-node tree. The large surface was costing the **server**,
not the browser. **Deliverable:** `LargeSurfaces.md`.

### 6 — Waiting

One `AsNoTracking` projection query per page — joined, narrowed, ordered and paged in the database —
replacing 201 statements; the filter and sort columns indexed. The export moved into
`Application.StartTask`, writing through one `StreamWriter` with progress every ten percent, a cancel
path and `IsDisposed` checks. **201 statements → 1**, **522 ms → 171 ms**, and no request thread held.
**Deliverables:** `QueryTrace.md`, `ExportRework.md`.

### 7 — Capacity

The measurements become thresholds: 5 GB usable ÷ 25 MB budgeted per session, minus 30 % headroom,
= 143 → **`maxSessions` 150**. `HealthCheck.json` ships with the application; the health URL answers
`503` with `Retry-After: 10` when the instance is full, while existing sessions keep working.
**Deliverables:** `Capacity.md`, `Deployment.md`, `Monitoring.md`, this report.

## Remaining risks

| Risk | Why it is open | What would close it |
|---|---|---|
| **The database is SQLite, in process** | statement counts are real, milliseconds are flattering; the N+1 fix is worth far more against a database server | re-measure `Tickets/Search`, `Dashboard/Refresh` and the export against the production database |
| **Concurrent exports untested** | one export per session is enforced; several sessions exporting at once was never run | run the export from several browsers while an interactive session measures its own scenarios |
| **Multi-session capacity untested** | every measurement here is one session; 150 was derived, not observed | a load test that holds 150 sessions and repeats the three scenarios |
| **Unmanaged memory not modelled** | the 6 MB is managed heap; the working set is larger | measure the working set per session on the target instance |
| **Dashboard staleness** | the snapshot is true at `GeneratedAt`; nothing caches it yet, so the window is one refresh — the moment anyone adds caching it becomes a decision | state the cache lifetime in `Budget.md` if caching is added |
| **`sessionTimeout` is 1800 s here** | a lab convenience; session lifetime multiplies concurrent sessions | set the production value and re-derive `maxSessions` |
| **One provider-specific statement** | the average-age aggregate uses SQLite's `julianday()` | rewrite for the production database and re-measure |

## The monitoring plan

`Monitoring.md`: which counters, which thresholds, which alerts, and — for each of the six fixes — the
specific signal that would show it regressing. The short version: alert on the **p95 of the `PERF end`
records against the budget**, on **statements per page above 1**, on **retained MB per idle session
climbing across a shift**, and on **any health-check refusal**.

## How to check this report

Nothing in it has to be taken on trust. Run any module folder, click **Warm up (discarded run)** and
then **Run the three scenarios ×3**, and the app prints its own medians, its own spread and its own
verdict against the budget. Module 1 is the before; this folder is the after; the modules in between
each hold one change and the evidence for it.
