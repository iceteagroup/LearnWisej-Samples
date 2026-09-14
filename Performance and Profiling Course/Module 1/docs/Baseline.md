# Baseline

The numbers every later module is compared against. They were produced by the app in this folder, with
the lab controls it ships with, and they are the **medians of three runs after one discarded warm-up
run**.

## Environment

| | |
|---|---|
| Build | **Release**, `dotnet run -c Release -f net10.0 --urls http://localhost:5801`, no debugger, no profiler attached |
| Hosting | Kestrel, one process, one session |
| Database | SQLite, in process, `WisejPerfLab/App_Data/perflab.db` (about 60 MB after seeding) |
| Dataset | **50,000 tickets**, **3,200 customer nodes** — printed in the app header |
| Machine | Windows 11, .NET 10, developer laptop |
| Browser | one tab, Edge/Chromium |
| Warm-up | **Warm up (discarded run)** clicked once; its three records are marked in the PERF log and thrown away |
| Method | **Run the three scenarios ×3**, median reported by the app itself |

## Measured

| Scenario | Rows | SQL statements | Run 1 | Run 2 | Run 3 | **Median** | Budget | Verdict |
|---|---:|---:|---:|---:|---:|---:|---:|---|
| `Dashboard/Refresh` | 50,000 | 1 | 496 ms | 476 ms | 442 ms | **476 ms** | 250 ms | over |
| `Tickets/Search` | 5,000 | 5,001 | 536 ms | 444 ms | 384 ms | **444 ms** | 300 ms | over |
| `Customers/ExpandNode` | 20 | 3,201 | 188 ms | 239 ms | 129 ms | **188 ms** | 100 ms | over |
| `Customers/LoadTree` | 3,200 | 3,201 | 312 ms | — | — | **312 ms** | 150 ms | over |
| `Tickets/Export` | 5,000 | 1 | 522 ms | — | — | **522 ms** | 400 ms | over |

The cold first run of each scenario — the one the warm-up discards — was consistently about twice the
median: 880 ms, 1,067 ms, 205 ms. That difference is JIT, the first query plan and the first layout of
each screen. It is exactly why the warm-up run exists, and why a single unwarmed measurement proves
nothing.

## Memory

Not measured here. Module 4 takes the retained-memory baseline with the Memory Usage tool, because a
number for "megabytes retained per idle session" needs a snapshot comparison, not a stopwatch. The
budget below states the target the app has to meet by then.

## What the numbers are evidence of

- `Dashboard/Refresh` issues **one** statement and still costs 476 ms: the cost is in this process,
  not in the database. That points at CPU and allocation — Modules 2 and 3.
- `Tickets/Search` issues **5,001** statements for 5,000 rows. One per displayed row. That points at
  data access — Module 6 — and at the entities themselves, Module 5.
- `Customers/ExpandNode` issues **3,201** statements to open one branch, because the expand reloads the
  whole table. The tree was also built in full at load: 3,200 nodes for a tree the user opens two
  branches of — Module 5.
- `Tickets/Export` blocks the request thread on `.Result` and writes the file one line at a time, so
  the progress bar never moves while it runs — Module 6.

## Reproducing it

```bash
cd "Performance and Profiling Course/Module 1/WisejPerfLab"
dotnet run -c Release -f net10.0 --urls http://localhost:5801
```

Then: **Warm up (discarded run)** → **Run the three scenarios ×3** → read the three median lines at the
bottom of the PERF log. Delete `WisejPerfLab/App_Data/` first if you want the dataset rebuilt from
scratch; the seed is deterministic, so the rows come back identical.

Your numbers will not be these numbers. That is not a problem — a baseline is only ever compared with
itself on the same machine. What must match is the shape: one statement for the refresh, one per row
for the search, thousands for the tree.
