# WisejPerfLab · Performance & Profiling · Module 7

Lab build for **Module 7 · Scale, Health Checks, Load Balancing and Final Tuning** — the capstone. The
application is the finished one: every scenario inside its budget, the leak gone, and now a **Capacity**
screen that turns the measurements into thresholds and shows the instance answering a load balancer.

- [`Capacity.md`](docs/Capacity.md) — the model and its arithmetic
- [`Deployment.md`](docs/Deployment.md) — two instances, session affinity, the WebSocket path
- [`Monitoring.md`](docs/Monitoring.md) — counters, alerts, and what each fix regressing looks like
- [`FinalReport.md`](docs/FinalReport.md) — all six modules in one document

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Performance and Profiling Course/Module 7/WisejPerfLab"
dotnet run -c Release -f net10.0 --urls http://localhost:5807
```

The health URL a load balancer would poll is <http://localhost:5807/healthcheck.wx>.

## What to click

| Action | What you should see |
|---|---|
| **Capacity** tab | `sessions 1   memory 0 %   CPU 0 %   managed heap 40.7 MB`, the `HealthCheck.json` values, and the capacity model with every number sourced to the module that measured it |
| **Ask healthcheck.wx now** | `healthcheck.wx → 200 OK` and `available — 1 session(s), memory 0 %, CPU 0 %` |
| **Drive it past the session limit** | the answer turns red: `503 (Retry-After: 10)` — `sessions 1 >= maxSessions 1` |
| **now click anything else** | the running session is untouched — every tab still works. That is the whole point of a health check |
| open a **second browser tab** while over the limit | a real load balancer sends it elsewhere; with only this instance, `/healthcheck.wx` is the thing to watch — `curl -i http://localhost:5807/healthcheck.wx` returns `503` and `Retry-After: 10` |
| **Restore the configured limit** | `200 OK` again |
| **Run the three scenarios ×3** | every median inside its budget: refresh ~113 ms, search ~66 ms, tree expand ~9 ms |
| **Warm up**, **Memory snapshot A** → **×50** → **Snapshot B** | `heap +1.1 MB   subscribers 0` |

```bash
curl -i http://localhost:5807/healthcheck.wx
```

## What changed since Module 6

```
HealthCheck.json               new — enabled, maxMemory, maxSessions, maxCPU, returnCode, retryAfter
Health/CapacityModel.cs        new — the measured inputs and the arithmetic, in code
Health/HealthPolicy.cs         new — HealthCheck.IsServerAvailable, the reason for the last answer,
                               and the lab override that refuses new sessions
Health/ServerLoad.cs           new — memory % and CPU % (Wisej's own accessors are not public)
Pages/CapacityPage.cs          new — the Capacity tab
MainPage.cs / .Designer.cs     + the fourth tab
Shell/PerfBudget.cs            + Capacity/HealthCheck, budget 20 ms — a health check must be free
Startup.cs                     + HealthPolicy.Apply() and the /healthcheck.wx endpoint
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| Collect the inputs: retained MB per idle session, CPU per scenario, peak concurrency, which sessions were idle | [`Capacity.md`](docs/Capacity.md); `Health/CapacityModel.cs` |
| Show the arithmetic and state the headroom | [`Capacity.md`](docs/Capacity.md) — memory ÷ budget − headroom, and the CPU sanity check |
| `HealthCheck.json` with `enabled`, `maxMemory`, `maxSessions`, `maxCPU`, `returnCode` 503, `retryAfter`, one sentence per threshold | `WisejPerfLab/HealthCheck.json`; the table in [`Capacity.md`](docs/Capacity.md) |
| Drive the instance past its limits; confirm a new session gets 503 with Retry-After while an existing session keeps working | **Drive it past the session limit**, then `curl -i …/healthcheck.wx`; `Health/HealthPolicy.cs` |
| `Deployment.md` for two instances: affinity, the WebSocket upgrade, what changes if it cannot pass, production settings | [`Deployment.md`](docs/Deployment.md) |
| Show the healthy case, the over-limit case and the no-WebSocket case, and what the user sees in each | the table in [`Deployment.md`](docs/Deployment.md) |
| The final report: scenario, environment, baseline, root cause, change, after evidence, remaining risks | [`FinalReport.md`](docs/FinalReport.md) |
| The monitoring plan: counters, thresholds, alerts, and the PERF records that reveal each fix regressing | [`Monitoring.md`](docs/Monitoring.md) |

## Self-check answers

**Why is a health check better than a bigger instance?** It is not — it is what you do *while* the
bigger instance is being ordered. A health check converts "everyone gets slower" into "existing users
keep their speed and new users go elsewhere", which is the failure mode people can live with.

**Why 503 and not 500?** 503 means "healthy server, no capacity now"; a balancer retries after
`Retry-After` and keeps the instance in rotation. 500 says the instance is broken, and some balancers
will take it out permanently.

**Which constraint actually binds here?** Memory. 150 sessions × 25 MB against 5 GB usable is the
threshold that fires; the CPU arithmetic (2.5 refreshes per second needed against ~8.8 per core) has
room to spare. That decides what to buy when it runs out.

**Why does the health check have its own budget (20 ms)?** Because a load balancer asks it every few
seconds, from every instance. A health check that does real work makes the instance less healthy every
time it is asked.

## Known simplifications

- Wisej.NET's built-in `healthcheck.wx` handler belongs to the classic System.Web pipeline; under
  Kestrel it answered `200` whatever the limits were (verified on Wisej-4 4.1.0), so this sample serves
  the URL from its own middleware using the same configuration and the same availability function. See
  the last section of [`Deployment.md`](docs/Deployment.md).
- `ServerLoad` computes memory and CPU percentages itself, because `HealthCheck.GetMemoryUsed()` and
  `GetCPULoad()` are not public. On this machine they read 0–3 %, so `maxSessions` is the threshold that
  fires.
- 150 sessions is **derived**, not observed. A load test that actually holds 150 sessions is the open
  item at the top of the remaining risks in [`FinalReport.md`](docs/FinalReport.md).
