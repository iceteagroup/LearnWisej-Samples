# CPU Usage and Instrumentation: two traces of the same scenario

Both are collected from `Debug > Performance Profiler` (Alt+F2), on the same warm scenario, in Release.
They answer different questions, and the difference between them is the answer to the only question
that matters first: **is this screen working, or waiting?**

| | CPU Usage | Instrumentation |
|---|---|---|
| How it works | samples the call stacks periodically | records entry and exit of every call |
| What it reports | where the CPU was, in proportion | wall-clock time **and call counts** per function |
| Overhead | low; safe on a realistic scenario | high; distorts short functions, changes the shape |
| Call counts | no | yes |
| Time spent waiting | barely visible — no CPU is being used | visible as elapsed time with no CPU |
| Use it for | "which of my code is burning CPU?" | "how often does this run, and how long did the user wait?" |

## The comparison that identifies the bucket

For one scenario, put the two numbers next to each other:

- **CPU time ≈ wall-clock time** → the process is *working*. The fix is to do less work: fewer objects,
  fewer strings, fewer calls, less materialisation. Modules 3, 4 and 5.
- **CPU time ≪ wall-clock time** → the process is *waiting*. No amount of tightening your loops will
  help; the fix is in the query, the file, the network call or the lock. Module 6.

## What this sample shows

The app already reports its own wall clock per stage, which is the same reading Instrumentation gives
you at a coarser grain. For a warm `Dashboard/Refresh` of 50,000 tickets:

| Stage | Wall clock | CPU or wait? |
|---|---:|---|
| query + materialise | 894 ms cold, ~430 ms warm | mixed: a few ms of SQLite query, the rest CPU in EF Core materialisation and the GC |
| format rows (50,000 calls) | 60 ms | CPU, entirely |
| rebuild KPI panel | 4 ms | CPU, entirely |
| rebuild chart | 10 ms | CPU, entirely |

So `Dashboard/Refresh` is a **working** scenario: almost all of its wall clock is CPU inside this
process. That is why the module's tool is CPU Usage.

`Tickets/Search` looks different on the same instruments:

| Stage | Wall clock | Statements |
|---|---:|---:|
| query + per-row customer lookup | ~900 ms cold, ~400 ms warm | **5,001** |
| project display rows | 3 ms | — |
| bind to the grid | 29 ms | — |

Also mostly CPU here, because SQLite runs in this process — but against SQL Server the same 5,001
statements would be 5,001 network round-trips and the scenario would be dominated by *waiting*. This
is the honest limitation of the sample, and it is worth knowing which of your findings would change
with a real database server: the **statement count** is a property of your code, the **cost per
statement** is a property of the environment.

## Collecting the pair

1. Warm up. Release. No debugger.
2. Alt+F2 → **CPU Usage** only → Start → one click → Stop. Save as
   `PP-M02_DashboardRefresh_50000_Release_<timestamp>.diagsession`.
3. Alt+F2 → **Instrumentation** only → Start → one click → Stop. Save with the same name plus `_instr`.
4. In the Instrumentation report, find `TicketFormatter.FormatRow` and record its **call count**
   (50,000) and elapsed time; find `DashboardService.LoadAllTickets` and record its elapsed time.
5. Write, in one sentence: how much of the scenario was CPU and how much was waiting.

Never compare an Instrumentation number with a CPU Usage number as if they were the same measurement,
and never quote an Instrumentation *duration* as the cost of the scenario: the instrumentation itself
adds time to every call, and the more calls, the more it adds.
