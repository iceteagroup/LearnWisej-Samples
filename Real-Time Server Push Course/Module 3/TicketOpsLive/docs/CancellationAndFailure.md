# Cancellation and failure — how the Import Monitor stays consistent

## Cooperative cancellation

Nobody aborts a thread. The click handler only signals the token (`_cts.Cancel()`); the loop decides when to stop
by calling `token.ThrowIfCancellationRequested()` **between** two records, never in the middle of one. The
`OperationCanceledException` is caught inside the task, the status becomes *Import cancelled by user.*, the log gets
"Cancelled at HH:mm:ss after record n", and `finally` restores the buttons. Records 1…n are in; nothing is half-written.

The page going away is a cancellation too: `this.Disposed += (s, e) => RequestCancel("page disposed")`. The loop stops
at its next record and the `finally` block skips the UI work because `IsDisposed` is true. This is the lesson's rule:
if the work is only meaningful for that page, cancel it when the page disappears; if it must outlive the page, move it
to a service (Module 6).

`_cts` is an **instance** field. A static `CancellationTokenSource` would let one user cancel another user's import.

## Error reporting — what the user sees vs. what the log has

| Audience | Gets | From |
|---|---|---|
| The user | *Import failed. Review the server log.* + a banner with the JobId | `catch (Exception)` in `RunImport` |
| Support / developer | the full exception with JobId, client id, session id, time | `LogError(job.JobId, "import", ex)` → `Console.Error` (replace with the logging framework in production) |
| The trace card | `FAILED on record 87: InvalidOperationException caught inside the task → server log, safe message` | `AddTrace` |

No stack trace ever reaches the browser. The simulated failure is a realistic one — record 87 has a bad
`priority` value — and it is armed by a checkbox so the reviewer can reproduce it on demand.

## JobId correlation (extension challenge)

`Services/ImportJob.cs` gives every import a short id (`J-3F9A2C`) at construction. The id is in:

- every import-log line (`[J-3F9A2C] Imported 50 records`),
- every trace line the job writes,
- the server console entry of a failure,
- the summary line the lab asks for (`Job J-3F9A2C: 200 records in 6.21 s — completed`),
- the SERVER STATE label (last three finished jobs).

A support engineer can therefore connect what the user saw with what the server logged — the capstone reuses this.

## The finally block is the contract

Whatever the outcome (completed, cancelled, failed), `finally` runs once and:

1. disposes the token source and clears `_importRunning`,
2. moves the job to the per-session history and bumps the outcome counter,
3. if the page is still alive, applies the UI reset in the session context and pushes it **once**
   (`Application.Update(this, () => …)`), catching `ObjectDisposedException` in case the page vanished in between.

## Evidence (verified in the browser)

- Cancel at record ~70: `_cts.Cancel() → job J-DE5219 sees the token at its next record (≤ 25 ms)` → `cancelled after record 71` → `Job J-DE5219: 71 records in 2.24 s — cancelled · 8 pushes`; the next Start ran a new job.
- Failure at 87: `Job J-18846F: 86 records in 2.73 s — failed · 9 pushes`; the log shows "Failed at 12:20:44 on record 87"; Start enabled again; a later Start completed 200 records.
- Starting twice: the button is disabled while a job runs; the `_importRunning` guard refuses a second start if a click gets through.
