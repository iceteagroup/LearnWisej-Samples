# The profiling workflow

The order of operations for every trace in this course. Steps 1–4 are the ones people skip, and
skipping them is why most traces cannot answer the question they were collected for.

## 1. Prepare the environment

- **Release**, no debugger attached (`dotnet run -c Release -f net10.0 --urls http://localhost:5802`,
  or Visual Studio's Release configuration). The app header says which build it is running under; if it
  says *Debug (measure in Release)*, stop.
- Reset the dataset to the baseline row counts. Deleting `WisejPerfLab/App_Data/` and restarting
  reseeds deterministically — 50,000 tickets, 3,200 customer nodes, the same rows every time.
- Close every browser tab except the app. Each tab is a session, and a session that is polling in the
  background will appear in your trace.
- Stop other local servers. The other modules of this course all bind their own port; they still
  compete for CPU.

## 2. Warm up

Click **Warm up (discarded run)**. It runs all three scenarios once and marks them in the PERF log as
discarded. On this machine the first run of each scenario costs roughly twice the median — JIT, the
first query plan, the first layout of each screen. Profiling a cold run measures the runtime starting
up, and the hot path will be full of JIT frames that never run again.

## 3. Collect one scenario, once

`Debug > Performance Profiler` (**Alt+F2**) → tick **CPU Usage** and nothing else → **Start** → click
**Refresh** exactly once → **Stop** as soon as the screen settles.

One tool. One scenario. One click. Two tools at once distort each other, and a recording that contains
three different actions produces a call tree in which none of them is legible.

## 4. Narrow the report before you read it

In the report, drag a selection on the **summary timeline** around the click itself. Everything outside
it — idle time, the browser reconnecting, the session heartbeat — is noise that flattens the
percentages of the thing you care about.

## 5. Read the call tree top down

Enable **Show Hot Path**. The top frames are ASP.NET Core and Wisej.NET dispatch; keep opening until
the first function in the `WisejPerfLab` namespace. That is where your code begins and where your
responsibility begins. Record:

- **Total CPU** — this function and everything it calls,
- **Self CPU** — this function alone,
- and, from **caller/callee**, who calls it and how many times.

Then check the same reading in the **flame graph** and in the **functions view sorted by self CPU**.
Three views that disagree mean the selection is wrong, not that the profiler is.

## 6. Follow up with Instrumentation

CPU Usage samples: it tells you where the CPU was, and cannot tell you how many times a function ran or
how long the user waited during a wait. Collect the same scenario again with **Instrumentation** and
compare — see [`CpuVsInstrumentation.md`](CpuVsInstrumentation.md).

## 7. Name and note the trace

Save it as `PP-M02_<Scenario><Action>_<rows>_<Build>_<timestamp>.diagsession` and write the note beside
it. **Trace note for the last run** in the app builds that note from what the app knows — scenario,
build, dataset, tool, budget, the last runs and their spread. See [`TraceNotes.md`](TraceNotes.md).

## 8. Repeat three times, then write the suspected cause

Take the median of three runs — **Run the three scenarios ×3** does it and prints the spread as a
percentage of the median. On this machine the spread is 10–25 %: a change smaller than that is not a
finding. Only then write down the suspected root cause, which bucket it is in, and what evidence would
refute it: [`RootCause.md`](RootCause.md).

## What this module does not change

No optimisation happens in Module 2. The app in this folder is the Module 1 app plus the instruments
that make its traces checkable: named stages inside each scenario, a call counter, the trace note, and
the noise floor. The fixing starts in Module 3, and it starts from the evidence collected here.
