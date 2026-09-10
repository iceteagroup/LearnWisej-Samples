# Background import notes — the six-step pattern in TicketOps Live

## The steps, mapped to the code

| Step (lesson / walkthrough) | Where | What it does |
|---|---|---|
| 1 · Adjust the UI before the work starts | `startImportButton_Click` | Start disabled, Cancel enabled, bar reset, JobId shown — these changes travel back with the click's own response |
| 2 · Create cancellation state | `startImportButton_Click` | `_cts = new CancellationTokenSource()` in an **instance** field (one per session) |
| 3 · Start the work in the session context | `startImportButton_Click` | `Application.StartTask(() => RunImport(token, failAt87))` — the handler returns immediately |
| 4 · Run the work in small steps | `RunImport` | one record per iteration, `token.ThrowIfCancellationRequested()` between records |
| 5 · Push at a controlled interval | `RunImport` | the controls change on **every** record, `Application.Update(this)` runs on every **10th** |
| 6 · Finish consistently | `RunImport` `finally` | Start re-enabled, Cancel disabled, summary logged, final state pushed once with `Application.Update(this, () => …)` |

## The cadence choice

The server-side model is exact at all times (`recordsImportedLabel`, `importProgressBar.Value`, `elapsedLabel` are
set on every record), but the browser is only told every 10 records. 200 records × 25 ms = 5 s of work → 20 progress
pushes plus the final one. Pushing every record would cost 200 WebSocket frames for a bar the eye cannot follow at
40 updates per second. The final state is never throttled: whatever happened, `finally` pushes it immediately.

## Direct update vs. context callback — both shapes are in the sample

```csharp
// inside the loop (task thread, context kept by StartTask): direct changes, then push
importProgressBar.Value = i / 2;
if (i % 10 == 0) Application.Update(this);

// in finally: several changes applied in context and pushed in ONE flush
Application.Update(this, () =>
{
    startImportButton.Enabled = true;
    cancelImportButton.Enabled = false;
    ShowBanner(…);
});
```

The direct form is what the lesson shows for a page-owned, single-session job. The callback form is the one to reuse
when the code that changes the UI does not run on a `StartTask` thread (a hub event, a service timer): capture
`Application.Current` in a request and use `Application.Update(context, () => …)`.

## Why the blocking version is wrong

`blockingImportButton_Click` runs 60 records inside the handler. The request stays open for ≈1.9 s, so:

- no push is possible — `Application.Update` has nothing to deliver *ahead of* a response that is still being built;
- the user cannot click Cancel — the browser is waiting for the response;
- the server thread is busy for the whole duration;
- everything arrives at once: the two trace lines are stamped 1860 ms apart but land together.

## Evidence (verified in the browser)

- Completed job: `Job J-E6B613: 200 records in 6.21 s — completed · 21 pushes (final state, finally block)`; trace shows
  `→ push … 10/200 · 5%` … `200/200 · 100%`, one line per 10 records, ≈310 ms apart.
- Cancelled job: Cancel clicked at record ~70 → `cancelled after record 71`, 8 pushes, Start enabled again.
- Failed job: checkbox on → `FAILED on record 87: InvalidOperationException caught inside the task`, 86 records, 9 pushes,
  red banner with the JobId, server console `[TicketOpsLive] … job J-18846F import failed for client … InvalidOperationException: Simulated malformed record #87 …`.
- Blocking job: `60 model changes → 0 pushes, ONE response after 1860 ms`.
