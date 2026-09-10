# Deliverable 5 — Conflict-resolution dialog

*EnterpriseOps · Advanced Module 3 · `UI/ConflictDialog.cs` + `.Designer.cs`,
`Services/ConflictResolutionService.cs`, `Services/ConflictInfo.cs`*

> The rejection is where the design work is. A bare error message leaves the user with lost work and no idea why.

## What the dialog says

```
┌────────────────────────────────────────────────────────────────────────────────┐
│ Work order changed — choose how to continue                                  ✕ │
├────────────────────────────────────────────────────────────────────────────────┤
│ (!)  This work order changed while you were editing                            │
│      Session "ben.tech" saved a newer version while this tab was editing.      │
│      Your edit is based on a stale copy — nothing has been overwritten.        │
│                                                                                │
│  ┌──────────────────┬─────────────────────────────┬──────────┬──────────────┐  │
│  │                  │ Title                       │ Status   │ Version      │  │
│  │ YOUR EDIT        │ Repair loading dock pump —… │ InProg…  │ v7 (stale)   │  │
│  │ CURRENT — v8     │ Repair loading dock pump    │ Completed│ v8 · ben.tech│  │
│  └──────────────────┴─────────────────────────────┴──────────┴──────────────┘  │
│                                                                                │
│  [ Reload latest ]  [ Compare changes ]  [ Cancel ]                            │
│  correlation 8f3a21c4 — expected v7, found v8                                  │
└────────────────────────────────────────────────────────────────────────────────┘
```

Four things, in this order: **what happened**, **that nothing was lost**, **the evidence**, **the choice**.
The correlation id is at the bottom in monospace — the one piece of internal state it is safe to show, and the
string the user reads out when they call support.

## The three paths

| Button | `ConflictResolution` | What it does | Closes? |
|---|---|---|---|
| **Reload latest** | `Reload` | discards the local edits; the screen re-opens the record at its current version and says *"re-apply your edit on the latest version"* | yes, `DialogResult.OK` |
| **Compare changes** | `Compare` | reveals `dgvCompare` — Title, Status, Assigned to, Priority, Version — each row marked `differs` / `same`, so the edit can be merged by hand | **no** — comparing is how the user decides between the other two, not a third way of leaving |
| **Cancel** | `Cancel` | leaves the screen exactly as it is: the edit is still on screen, still based on the stale version, nothing saved and nothing lost | yes, `DialogResult.Cancel` |

Closing the window with ✕ leaves `Resolution` at its initial value, `Cancel` — the safe default.

## The dialog decides nothing

Every decision is in `ConflictResolutionService`:

| Dialog does | Service does |
|---|---|
| `dgvVersions.DataSource = _service.Summarize(_conflict)` | builds the two rows ("YOUR EDIT", "CURRENT — v8") |
| `dgvCompare.DataSource = _service.Compare(_conflict)` | builds the field-by-field comparison and traces how many fields differ |
| `Choose(resolution)` | `RecordChoice(context, conflict, choice)` → audit entry + trace line, with the correlation id |

That is what lets the three paths be reviewed without opening the designer, and reused by a batch save or an
import that hits the same conflict. Every handler in `ConflictDialog.cs` is four lines or fewer.

## Async, because Wisej dialogs do not block

```csharp
// WorkOrderEditorPage.ResolveConflictAsync
var dialog = new ConflictDialog(conflict, CurrentContext, _services.Conflicts, _trace);
DialogResult answer = await dialog.ShowDialogAsync();

if (dialog.Resolution == ConflictResolution.Reload)
    await ReloadLatestAsync();
```

There is no blocking `ShowDialog()` in a Wisej app: the handler is `async void`, the dialog is awaited, and the
pending UI changes are pushed with `Application.Update(this)` in the `finally` of the calling handler.

## The choice is recorded

```
Audit: conflict resolution recorded — #2002 expected v7, found v8 → Reload (correlation 8f3a21c4)
```

`AuditTrail` keeps it with the tenant, the user and the correlation id, so "who overwrote my edit" has an answer
months later — and so does "who was told about the conflict and chose to walk away".

## Evidence — what the running app shows

| Action | What you see |
|---|---|
| Save a stale edit | amber banner *"This work order changed while you were editing — correlation … — expected v7, found v8. Nothing was overwritten."*, then the dialog |
| Dialog opens | trace `UI → ConflictDialog opened for #2002 — correlation 8f3a21c4 — expected v7, found v8` |
| **Compare changes** | `Service: ConflictResolutionService.Compare(#2002) → 5 fields, 3 differ`; the panel appears; the button becomes **Hide comparison**; nothing is saved |
| **Reload latest** | audit line, dialog closes, editor rebinds at `v8`, green banner, queue refreshed |
| **Cancel** | audit line, dialog closes, the stale edit is still in `txtTitle`, status reads `conflict cancelled — your edit kept` |
| ✕ | same as Cancel — `Resolution` was never moved off its default |
