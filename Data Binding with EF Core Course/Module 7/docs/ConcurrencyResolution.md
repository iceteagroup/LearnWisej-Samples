# Concurrency resolution

The first three Module 7 deliverables: the `RowVersion` round trip, the reproducible conflict, and the
`ConflictDialog` with Reload and Overwrite.

## The round trip

`TicketEditModel` gains a hidden field:

```csharp
public byte[] RowVersion { get; set; } = Array.Empty<byte>();
```

No control binds to it, `TicketValidator` never looks at it — it is plumbing, not a business rule.
`TicketCommandService.LoadEditModelAsync` fills it from the no-tracking read
(`TicketCommandService.ToModel`), so the token the editor's operator actually saw travels with the model
for as long as the dialog is open.

`SaveAsync`, for an **existing** ticket, restores it before any other field is mapped:

```csharp
ticket = await db.Tickets.SingleOrDefaultAsync(t => t.Id == model.Id, token)
    ?? throw new TicketNotFoundException(model.Id);

var originalToken = overwriteOriginalRowVersion ?? model.RowVersion;
if (originalToken is { Length: > 0 })
    db.Entry(ticket).Property(t => t.RowVersion).OriginalValue = originalToken;
```

`ticket` is a **fresh** tracked read — always current, by itself never in conflict with anything. The
`OriginalValue` line is the whole trick: it tells EF Core "compare the row against *this* token, not the
one you just read", so `SaveChangesAsync` renders

```
UPDATE "Tickets" SET "Priority" = @p0, "RowVersion" = @p1, "Status" = @p2, "UpdatedAt" = @p3
WHERE "Id" = @p4 AND "RowVersion" = @p5 RETURNING 1;
```

with `@p5` = the stale token. If another session's write already replaced the row's `RowVersion`, this
statement affects zero rows and `SaveChangesAsync` throws `DbUpdateConcurrencyException`. Skip the
`OriginalValue` line and the WHERE clause compares the fresh read against itself — always true — and no
conflict is ever detected; see the first Student review question below and `docs/Checklists.md`.

`overwriteOriginalRowVersion` is the same parameter used for the Overwrite resolution (below): when it is
supplied, it — the database's *current* token, read when the conflict was built — replaces `model.RowVersion`
as the value forced into `OriginalValue`, so the retry's WHERE clause matches the row as it stands right now.

## Reproducing the conflict

**One session (page button).** Click **Edit ticket**, leave the dialog open, click **Simulate: another
operator changes it** on the page — `TicketCommandService.SimulateAnotherOperatorChangeAsync` updates the
same row (`Status = Closed`, `Priority = High`) through a brand-new context, stamping a fresh `RowVersion`.
Click **Save** in the still-open editor: its `model.RowVersion` is now stale, and the conflict dialog opens.

**Two browser sessions.** Open the app in two tabs (or a second private window — a different Wisej.NET
session). In both, **Edit ticket** on the same row. Save the first — an ordinary, successful save. Save the
second: its `RowVersion` is now stale for the same reason, and it gets the conflict dialog instead.

## The conflict list

`TicketCommandService.SaveAsync` catches the exception **inside its own `await using` scope**, while its
context is still open:

```csharp
catch (DbUpdateConcurrencyException ex)
{
    var conflicts = await _conflictResolution.BuildConflictListAsync(ex, token);
    ex.Data["ConflictSet"] = conflicts;
    throw;
}
```

This has to happen here, not in the editor. `DbUpdateConcurrencyException.Entries` are `EntityEntry`
objects that belong to the context that threw; `GetDatabaseValuesAsync` issues a fresh query through that
same context. `await using var db = …` disposes that context the moment `SaveAsync`'s stack frame
unwinds — **before** the exception reaches `TicketEditorForm`'s catch block — so by the time the editor
sees the exception, a naive `ex.Entries[0].GetDatabaseValuesAsync()` would throw
`ObjectDisposedException`. `ConflictResolution.BuildConflictListAsync` runs first, turns everything it needs
into plain data (`ConflictSet`/`ConflictField` — no live entities), and that data rides on the exception's
own `Data` dictionary. The editor's `catch (DbUpdateConcurrencyException ex)` — which must sit **above**
`catch (DbUpdateException ex)` because the first type derives from the second, exactly as the lab guide asks —
just reads it back out:

```csharp
var conflicts = ex.Data["ConflictSet"] as ConflictSet ?? new ConflictSet(false, null, Array.Empty<ConflictField>());
```

`BuildConflictListAsync` walks `ex.Entries`; a `null` from `GetDatabaseValuesAsync` means the row was
deleted by another user (`ConflictSet.DeletedByAnotherUser`). Otherwise it compares `CurrentValues` against
the database's values for every mapped property except `Id` and `UpdatedAt` (pure noise — it changes on
every save and never explains the conflict) and keeps `RowVersion` under a relabelled, hex-formatted row —
not noise, it is the proof the conflict happened, and `ConflictSet.DatabaseRowVersion` is what Overwrite needs.

**A documented nuance.** `ConflictField.OriginalValue` is EF Core's own `OriginalValues` for the property.
Because `SaveAsync` always tracks a **fresh** read (never the editor's own stale copy), for an ordinary
field this is "what this save attempt's own read saw" — which, since the conflicting write already landed
before that read ran, is the same as the database's current value. Only `RowVersion`'s `OriginalValue`
differs on purpose: `SaveAsync` forces it to the stale token the editor carried in. A true "what the
operator started editing from" for an ordinary field would need `TicketEditModel` to keep its own pristine
snapshot, which it does not — out of scope for this lab. `SupportDesk.Tests/ConcurrencyAndTransactionsTests.cs`
asserts this exact shape rather than a nicer-sounding one that is not what the code does.

## The dialog: Reload and Overwrite

`ConflictDialog` (`Form`, `CenterParent`) lists every `ConflictField` in a `DataGridView` (Field / Your
value / Database value / Original value), explains what happened in a label, and offers three buttons:

- **Reload** — always offered. `ShowConflictDialogAsync` re-runs `LoadEditorAsync()`: new lookups, the
  fresh `Ticket` row, the new `RowVersion`. The dialog's own `DialogResult` is `OK` either way; the actual
  decision is read from `ConflictDialog.Choice`. The **editor stays open** with the fresh values, and the
  trace says so (`Reload complete — the editor now shows the database's values and the new RowVersion;
  still open`).
- **Overwrite** — visible only when `ConflictResolution.CanOverwrite(role)` is true (`role == UserRole.Supervisor`).
  `TicketEditorForm.OverwriteAsync` calls `Commands.SaveAsync(model, latency, forceDuplicateNumber: false,
  conflicts.DatabaseRowVersion)` — the database's *current* token becomes `OriginalValue`, so this retry is
  expected to succeed even though someone else changed the row in between. The **editor closes with OK**
  after a successful Overwrite, and the parent grid refreshes.
- **Cancel** — the editor stays open, exactly as it was; nothing is saved.

`cboRole` on the page (Agent / Supervisor, Agent by default) is passed into `TicketEditorForm`'s
constructor and from there into every `ConflictDialog` it opens — never guessed inside the dialog. This is
a policy decision, never an automatic retry: see the lesson's own "Common mistake: silently overwriting
another user's change". An ordinary Agent only ever sees Reload and Cancel.

**Deliberate deviation from the lesson text.** `assets/courses/ef-core-binding/lessons/ef7s1.html` also
names a third resolution, **Merge** ("keeps some values from each side, then saves on top of the current
version"). The Module 7 lab guide's own required deliverables and instructor acceptance criteria — and the
task given to this build — name only **Reload** and **Overwrite** as the dialog's buttons. Building Merge
as a per-field picker was out of scope here; `ConflictField` already carries every value a Merge UI would
need (Your / Database / Original), so a future module could add it without touching `ConflictResolution`.

## Trace, verbatim (from `SupportDesk.Tests.ConcurrencyAndTransactionsTests.Prints_the_trace_for_one_reproduced_conflict`)

```
Command: UPDATE "Tickets" SET "Priority" = @p0, "RowVersion" = @p1, "Status" = @p2, "UpdatedAt" = @p3 WHERE "Id" = @p4 AND "RowVersion" = @p5 RETURNING 1; (0.1 ms)
Note: SaveAsync: caught DbUpdateConcurrencyException — 0 rows matched the WHERE clause · 3 field(s) differ
conflict Priority: yours 'Low' · database 'High' · original 'High'
conflict RowVersion (concurrency token): yours '0x0DF6508CA8AF3A4A810F3E4E93221D78' · database '0xB028F781A942DC45B179D21F650BA89A' · original '0x234AC8FC3F77304FB56AA717719F48AD'
conflict Status: yours 'Resolved' · database 'Closed' · original 'Closed'
```

Note that `Priority` shows up as a conflict even though the test only edited `Status` — the operator's edit
model still carries whatever `Priority` it loaded, and `SimulateAnotherOperatorChangeAsync` changed
`Priority` too, so the save attempt disagrees with the database on both fields at once. This is realistic:
a conflict is not "the one field you touched", it is every field where your save and the database's current
row disagree.

## Evidence

- `SupportDesk.Tests/ConcurrencyAndTransactionsTests.cs`:
  - `SaveAsync_with_a_stale_RowVersion_throws_DbUpdateConcurrencyException` — the round trip really throws,
    through `TicketCommandService`, not raw EF Core.
  - `SaveAsync_without_a_stale_RowVersion_does_not_throw_when_nothing_else_changed_the_row` — the ordinary
    path is unaffected.
  - `SaveAsync_conflict_attaches_a_ConflictSet_listing_the_differing_fields_with_your_database_and_original_values`
    — `BuildConflictListAsync`'s field list, including the RowVersion/UpdatedAt handling.
  - `BuildConflictListAsync_reports_the_row_as_deleted_when_it_is_removed_between_the_tracked_read_and_SaveChangesAsync`
    — the deleted-row branch, driven directly against EF Core (see the test's own remarks for why
    `TicketCommandService.SaveAsync` cannot reach this exact race — its own fresh read already throws
    `TicketNotFoundException` first for the far more common "deleted before I even tried to save" case).
  - `LoadEditModelAsync_after_a_conflict_returns_the_current_database_values_the_way_the_dialogs_Reload_does`
    and `SaveAsync_overwrite_with_the_databases_RowVersion_wins_and_produces_a_new_token` — Reload and
    Overwrite.
  - `Prints_the_trace_for_one_reproduced_conflict` — the verbatim trace quoted above.
- Two-session reproduction: not run by these tests (there is no second browser here) — verify in the
  Browser pane per "Verified / unverified" in the README.
