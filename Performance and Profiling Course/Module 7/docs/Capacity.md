# Capacity model

Every number here was measured in this application, in Release, on one machine, with the dataset in
`Baseline.md`. The arithmetic is shown so that someone who disagrees with a threshold can see exactly
which measurement to argue with.

## The inputs

| Input | Value | Where it came from |
|---|---:|---|
| Retained per idle session (managed heap) | **6.0 MB** | Module 4, snapshot A→B on a session that had run one search and returned to idle |
| Budgeted per session | **25 MB** | `Budget.md` — the measured 6 MB plus room for the screens a session may open during a shift |
| `Dashboard/Refresh` | **113 ms** | Module 3, median of three warm runs |
| `Tickets/Search` | **66 ms** | Module 6, median of three warm runs |
| `Tickets/Export` (background task) | **171 ms** | Module 6, 5,000 rows |
| `Customers/LoadTree` / `ExpandNode` | 62 ms / 9 ms | Module 5 |
| Instance memory | 8 GB, **5 GB usable** | the target instance; the rest is the OS, the page cache and other processes |
| Headroom kept | **30 %** | for spikes, garbage collection and the operating system |
| Expected peak concurrency | **120 sessions** | the support team's shift plan: 100 agents plus a margin |

**Which sessions were idle and which were active:** the 6 MB figure is an *idle* session — one that has
a page, three tab screens, a PERF buffer and the rows of its last search, and is doing nothing. An
active session adds the transient cost of whatever scenario it is running, and a session that has an
export in flight holds the rows of that export until it finishes. The model budgets every session at
the idle figure plus headroom, and treats concurrency of *active* scenarios separately below.

## Memory — how many sessions fit

```
usable memory                      5.0 GB  = 5,120 MB
budget per session                   25 MB
                                   ------
sessions before headroom            204
headroom kept                        30 %
                                   ------
maxSessions                         143   ->  configured as 150 (rounded to the operational number)
```

Against the **measured** 6 MB rather than the budgeted 25 MB, the same instance would hold about 600
sessions. The gap between 143 and 600 is deliberate: the budget is what a session is *allowed* to cost,
and it is what protects the instance when someone adds a screen that holds more. The alternative —
sizing on today's measurement — means every new feature silently reduces capacity.

## CPU — does 150 sessions have enough processor?

The expensive interactive scenario is the dashboard refresh at 113 ms of mostly-CPU work:

```
one core serves                     1000 / 113   ≈  8.8 refreshes per second
peak scenario rate (150 sessions,
each refreshing once a minute)      150 / 60     =  2.5 refreshes per second
```

Two and a half refreshes per second against nearly nine per second per core is comfortable, and the
machine has more than one core. The searches (66 ms) and tree expands (9 ms) are cheaper still. The
export is 171 ms on a background thread — it does not compete for the request path, but it does compete
for the thread pool and for the database, which is why concurrent exports are the untested risk in
`Module 6/docs/ExportRework.md`.

**So memory is the binding constraint, not CPU.** That is worth stating explicitly, because it decides
what to do when the instance runs out: add memory or add instances, not faster cores.

## The thresholds

`HealthCheck.json`, with the sentence that justifies each line:

```json
{
  "enabled": true,
  "maxMemory": 70,
  "maxSessions": 150,
  "maxCPU": 85,
  "returnCode": 503,
  "retryAfter": 10
}
```

| Setting | Value | Why this number |
|---|---:|---|
| `maxSessions` | 150 | 5 GB usable ÷ 25 MB budgeted per session, minus 30 % headroom = 143, rounded to the operational figure of 150. Memory is the binding constraint, so this is the threshold that will actually fire. |
| `maxMemory` | 70 % | Above 70 % of the instance's memory the garbage collector spends visibly more time and the operating system starts trimming the working set. It is a second line of defence in case a session costs more than its budget. |
| `maxCPU` | 85 % | Sustained CPU above 85 % leaves nothing for the spikes that the measurements show are normal — the cold first run of a scenario costs twice the median. |
| `returnCode` | 503 | Service Unavailable is what a load balancer understands as "healthy server, no capacity right now". A 500 would suggest the instance is broken and should be taken out of rotation. |
| `retryAfter` | 10 s | Long enough that the balancer does not hammer the instance, short enough that a machine which frees a session is used again within one page load. |

## Verifying the behaviour

The **Capacity** tab drives it, and it is worth watching once:

1. **Ask healthcheck.wx now** → `200 OK`, `available — 1 session(s), memory 0 %, CPU 0 %`.
2. **Drive it past the session limit** — this sets `HealthCheck.MaxSessions` to the number of sessions
   that exist right now.
3. Ask again → **`503`**, `Retry-After: 10`, and the body says exactly why:
   `503 — sessions 1 >= maxSessions 1, retry after 10 s`.
4. **The existing session keeps working.** Every button on every tab still responds; the refusal only
   affects the balancer's answer for *new* users.
5. **Restore the configured limit** → `200 OK` again.

Observed on this machine: memory reads 0–2 % (the process working set is small against the machine's
memory) and CPU 0–3 % at rest, so `maxSessions` is the threshold that fires — which is what the model
predicted.

## What this model does not cover

- **Unmanaged memory.** The numbers are managed heap. The process working set is larger; for a real
  capacity decision, measure the working set per session on the target instance.
- **The database.** One SQLite file in process. A shared database server is a capacity constraint of its
  own, and two instances behind a load balancer share it.
- **Concurrent exports.** Not tested beyond one at a time per session.
- **Session lifetime.** 150 *concurrent* sessions assumes sessions end. `Default.json` sets
  `sessionTimeout` to 1800 s here so a lab session survives a coffee break; in production that number
  multiplies the session count and belongs in this model.
