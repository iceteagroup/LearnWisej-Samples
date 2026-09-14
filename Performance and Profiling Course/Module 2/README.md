# WisejPerfLab · Performance & Profiling · Module 2

Lab build for **Module 2 · Visual Studio Profiling Workflow for Wisej.NET**. The application is the
Module 1 application — nothing is optimised here — plus the instruments that make a profiler report
**checkable**:

- **named stages** inside every scenario (`ScenarioProbe.Stage`), so the PERF log says which phase of
  the refresh the time went to;
- a **call counter** (`CallCounter`), so the app reports the call count an Instrumentation trace gives
  you and a sampling trace cannot;
- **Trace note for the last run**, which writes the note that belongs beside the `.diagsession`;
- the **noise floor**: after three runs the app prints the spread as a percentage of the median.

What those instruments found is in `docs/`: [`ProfilingWorkflow.md`](docs/ProfilingWorkflow.md) ·
[`HotPath.md`](docs/HotPath.md) · [`CpuVsInstrumentation.md`](docs/CpuVsInstrumentation.md) ·
[`TraceNotes.md`](docs/TraceNotes.md) · [`RootCause.md`](docs/RootCause.md), beside Module 1's
`Scenarios.md`, `Baseline.md` and `Budget.md`.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Performance and Profiling Course/Module 2/WisejPerfLab"
dotnet run -c Release -f net10.0 --urls http://localhost:5802
```

Release, because every recorded measurement in this course is taken in Release. The header says which
build you are on.

## What to click

| Action | What you should see |
|---|---|
| **Warm up (discarded run)** | each scenario runs once and its stages are listed under it |
| Dashboard → **Refresh** | `stage Refresh / query + materialise 894 ms`, `format rows 60 ms`, `rebuild KPI panel 4 ms`, `rebuild chart 10 ms`, then `calls TicketFormatter.FormatRow x50,000` |
| **Run the three scenarios ×3** | three medians, each followed by `spread … = N % of the median — a smaller difference is not a finding` |
| **Trace note for the last run** | a dialog with the file name, the environment, the tool, the budget, the measured numbers and the eight collection steps |
| **Break the database** → Dashboard → **Refresh** | `PERF end Dashboard/Refresh elapsedMs=62 failed=DatabaseUnavailableException` — the failing trace, short because the scenario stopped at the connection |

The headline finding is in the stage list, and it is not the one the code invites you to expect: the
per-row formatter and the control rebuild together are **7 %** of the refresh. See
[`HotPath.md`](docs/HotPath.md).

## Collecting the real traces

The app cannot collect a CPU Usage trace for you — that is Visual Studio's job, and doing it is the lab.
[`ProfilingWorkflow.md`](docs/ProfilingWorkflow.md) has the eight steps; the short version:

1. Release, no debugger, dataset reset, one browser tab, warm up.
2. Alt+F2 → **CPU Usage** only → Start → one **Refresh** click → Stop.
3. Narrow the summary timeline to the click; **Show Hot Path**; scroll past the ASP.NET Core and
   Wisej.NET dispatch frames to the first `WisejPerfLab` function; record total and self CPU.
4. Caller/callee on it; then check that the flame graph and the functions view agree.
5. Repeat with **Instrumentation** for call counts and wall-clock time.
6. Save both under the naming convention and write the note beside each.
7. Three runs, median, spread.
8. Write the suspected root cause and what would refute it.

## What changed since Module 1

```
Diagnostics/CallCounter.cs        new — per-scenario call counts, reset by the probe scope
Diagnostics/ScenarioProbe.cs      + Stage(name), + call-count records on dispose
Diagnostics/PerfRecord.cs         + the "stage" and "calls" line formats
Diagnostics/PerfLogBuffer.cs      + Samples(), + SpreadPercent()  — the noise floor
Forms/TraceNoteForm.cs            new — the note dialog
MainPage.cs / .Designer.cs        + "Trace note for the last run", + the spread line after the medians
Pages/DashboardPage.cs            + four stages around the phases of the refresh
Pages/TicketGridPage.cs           + three stages around the phases of the search
Pages/CustomerTreePage.cs         + two stages around the phases of the tree load
Services/TicketFormatter.cs       + CallCounter.Count("TicketFormatter.FormatRow")
```

No service, no query and no screen behaviour changed. The scenarios cost what they cost in Module 1.

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| Prepare the environment, reset the dataset, warm up without recording | delete `WisejPerfLab/App_Data/`, restart, **Warm up (discarded run)** |
| Alt+F2, CPU Usage only, one click, stop | Visual Studio; [`ProfilingWorkflow.md`](docs/ProfilingWorkflow.md) |
| Narrow the timeline, Show Hot Path, find the first function in your namespace | [`HotPath.md`](docs/HotPath.md) |
| Caller/callee: who calls it, how many times | the `calls …` records; confirm in the trace |
| Instrumentation follow-up: call count, wall clock, CPU versus waiting | [`CpuVsInstrumentation.md`](docs/CpuVsInstrumentation.md) |
| Save both traces with module, scenario, build, dataset and timestamp in the name | **Trace note for the last run**; [`TraceNotes.md`](docs/TraceNotes.md) |
| Record what the trace looks like when the scenario fails | **Break the database**, then rerun |
| Three runs, median, then the suspected root cause and what would refute it | **Run the three scenarios ×3**; [`RootCause.md`](docs/RootCause.md) |

## Self-check answers

**Why Release?** A Debug build skips inlining and other optimisations, and a debugger changes both the
timing and the call tree. A Debug measurement measures a program you are not shipping.

**Why narrow the timeline?** Percentages in a call tree are percentages *of the selection*. If the
selection contains four seconds of idling, a 400 ms click is 10 % of the report and nothing looks hot.

**Why is a large Total CPU with a tiny Self CPU not a finding?** It is a route, not a cause: everything
expensive is somewhere below it. Keep opening the hot path until self CPU rises.

**What does CPU time far below wall-clock time mean?** Waiting — a database round-trip, a file, a lock,
an HTTP call. Tightening loops will not help; that is Module 6.

**Why the median of three?** Because the spread here is 10–25 % of the median. A single before and a
single after can differ by that much with nothing changed at all.

## Known simplifications

- `.diagsession` files are not committed (`.gitignore` excludes them). The traces in this course are
  yours to collect; the docs say what to look for, not what your machine will print.
- `CallCounter` and `ScenarioProbe.Stage` are lab instruments. They cost roughly 50 ns per call and a
  few microseconds per stage — negligible here, and the first thing to remove before recording a trace
  you intend to quote to the millisecond.
