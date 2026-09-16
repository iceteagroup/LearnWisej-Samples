# Suspected root cause — `Dashboard/Refresh`

Written after the traces, before any code is touched. One paragraph, one bucket, one falsification
test. Module 3 either confirms it or throws it away.

## The claim

`Dashboard/Refresh` takes 476 ms (median of three, warm, Release, 50,000 tickets) against a 250 ms
budget, and the cost is **CPU and allocation inside this process**, not the database and not the
browser. The scenario issues exactly **one** SQL statement, and the stage breakdown puts 92 % of the
time in *query + materialise* — of which the SQLite query itself is a few milliseconds. The rest is EF
Core materialising 50,000 tracked entities, each carrying two long text columns that no KPI uses, so
that three integers and fourteen bar values can be computed in memory. The per-row formatter
(`TicketFormatter.FormatRow`, 50,000 calls) and the KPI control rebuild together account for about 7 %:
they are real, they are worth removing, and removing them would not bring the scenario inside budget.

**Bucket:** CPU in your own code, with allocation as its companion — buckets 1 and 3 of the five.
Not blocking/waiting (there is nothing to wait for), not data access (one statement), not payload (the
update is three labels and a small chart).

## The evidence that supports it

| Evidence | Source |
|---|---|
| one statement per refresh | the app's own statement count; confirm in a **Database** trace |
| 92 % of the time in *query + materialise* | `ScenarioProbe.Stage` records in the PERF log |
| hot path leaves your code into EF Core materialisation | **CPU Usage**, Show Hot Path, timeline narrowed to the click |
| 50,000 calls of `FormatRow`, one caller | the app's call counter; confirm with **Instrumentation** + caller/callee |
| CPU time ≈ wall-clock time | comparing the CPU Usage and Instrumentation traces |
| noise floor 10–25 % | **Run the three scenarios ×3**, spread line |

## What would refute it

- A **Database** trace showing the single statement taking most of the 476 ms. Then the cost is in the
  database, the fix is an index or a narrower query, and materialisation is innocent.
- A **CPU Usage** trace whose hot path stays inside `TicketFormatter` or inside control construction
  rather than the EF Core pipeline. Then the obvious suspects were the right ones after all.
- Dropping the ticket count to 5,000 and seeing the time fall by much less than 90 %. That would mean
  the cost is fixed per refresh rather than per row, and the row count is not the driver.
- A **.NET Async** trace showing a wait. There is no await on this path, so there should be none — if
  there is, something is blocking that the code does not admit to.

## The predicted fix, and its prediction

Compute the three KPI values and the chart series **in the service, in the database**, and hand the page
a small snapshot of display-ready strings. The 50,000 entities then never exist, the formatter is never
called, and the KPI labels are assigned rather than rebuilt.

Prediction: `Dashboard/Refresh` lands **well under 100 ms**, a change far larger than the 10–25 % noise
floor, and the new hot path is no longer in materialisation but in the request dispatch itself — the
signature of a handler with nothing left to do. If the measurement after the fix is only 10 % better,
this root cause was wrong.

Module 3 reruns the identical scenario, on the same dataset, build and warm-up, and records the answer
in `Module 3/docs/BeforeAfter.md`.
