# Trace notes

A `.diagsession` with no note is a file you will delete in three weeks because you cannot remember what
it was. Two things fix that: a file name that carries the facts, and a note beside it that carries the
rest.

## The naming convention

```
PP-M<module>_<Scenario><Action>_<rows>_<Build>_<yyyyMMdd-HHmm>.diagsession
```

```
PP-M02_DashboardRefresh_50000_Release_20260914-1123.diagsession
PP-M02_DashboardRefresh_50000_Release_20260914-1126_instr.diagsession
PP-M02_TicketsSearch_5000_Release_20260914-1131.diagsession
PP-M02_DashboardRefresh_50000_Release_20260914-1140_outage.diagsession
```

Module, scenario, dataset, build, when — and a suffix when the trace is a variant (`_instr` for the
Instrumentation follow-up, `_outage` for the failure path). Traces are **not** committed to this
repository; `.gitignore` excludes `*.diagsession`. Collect your own.

## The note

**Trace note for the last run** in the app writes it for you, from what the app knows. It goes in a
`.md` or `.txt` file beside the trace, under the same name. This one was produced by the button after a
warm `Dashboard/Refresh`:

```
// Trace note — save beside the .diagsession under the same name
// File:     PP-M02_DashboardRefresh_50000_Release_20260914-1123.diagsession
// Module:   02 — Visual Studio profiling workflow
// Scenario: Dashboard/Refresh   rows=50,000
// Build:    Release, x64, no debugger, warm
// Hosting:  Kestrel, local SQLite (App_Data/perflab.db)
// Dataset:  50,000 tickets, 3,200 customer nodes
// Browser:  one tab, one session
// Tool:     CPU Usage
// Budget:   < 250 ms
// Measured: last run 476 ms, median of 3 = 476 ms, spread 11 %
```

The two lines that are most often missing are the last two. **Tool** matters because the same scenario
looks completely different under CPU Usage and under Database. **Budget** matters because a trace is
evidence in an argument about whether something is fast enough, and six months later nobody remembers
what "fast enough" meant.

## The failure-path trace

Collect one of these too, for the same scenario. **Break the database**, then run the refresh:

```
PERF start  Dashboard/Refresh
PERF end    Dashboard/Refresh elapsedMs=62 failed=DatabaseUnavailableException
```

What the failing trace looks like, and why it is worth having:

- It is **short** — the scenario ends at the first connection attempt, so the call tree stops at
  `LoadAllTickets` and never reaches materialisation. A trace that is suspiciously cheap is often a
  trace of a scenario that failed.
- The failure is **visible in the app**, not silent: red banner, fault status, and the PERF end record
  still written with its elapsed time. If a failed run wrote no record at all, the median of three
  would quietly become the median of two.
- Comparing it with the successful trace is the cleanest demonstration of where the time in the
  successful one actually goes: everything the failing run skipped.

## The three numbers that go with every trace

1. the **median of three** warm runs, not a single run;
2. the **spread** of those runs as a percentage of the median — the app prints it after
   **Run the three scenarios ×3**; on this machine it is 10–25 %;
3. the **budget** the scenario is being held to.

Without the spread, the next module's "30 % faster" cannot be distinguished from a rerun.
