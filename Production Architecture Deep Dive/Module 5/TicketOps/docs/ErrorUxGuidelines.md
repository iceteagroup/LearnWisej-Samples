# Error UX guidelines

*Module 5 deliverable · TicketOps Console*

Validation serves two audiences. This document is about the first one — the user — and how the Work Order editor
answers three questions whenever a save does not go through: **which field**, **what is expected**, and
**can I fix it myself**. The second audience (the data) is covered in [`SavePipeline.md`](SavePipeline.md).

## 1. Pick the channel by what the user can do about it

| The problem… | Channel | Control in this app | Example |
|---|---|---|---|
| belongs to one field the user can fix in place | **field error** — glyph + tooltip beside the control | `errorProvider.SetError(control, message)` | "Title is required." on `textTitle` |
| spans several fields or describes the workflow | **summary error** — a line in the summary panel | `panelSummary` / `labelSummaryList` | "Cannot move a New order to Completed." |
| is any error at all | **also** a line in the summary panel | same | the panel lists every problem, field or not, so nothing hides behind a single glyph |
| is the outcome of the whole action | **status line** (`StatusBanner.SetStatus`) | top-right "● …" | "● 3 problems found — nothing was saved" |
| is an unexpected failure the user cannot fix | **banner + toast** with a safe sentence | `StatusBanner.ShowBanner` + `AlertBox.Show(…, TopRight, 4000)` | "The work order could not be saved. Your changes are still here — try again or contact support." |

Rules of thumb:

- One mechanism per channel, used the same way on every editor. `ShowValidation` is the only method that paints
  errors; every path (pre-check, server validate, server rules) goes through it.
- **Clear before you paint.** `ClearErrors()` (`errorProvider.Clear()` + hide the panel) runs before every display,
  so a fixed field never keeps a stale glyph.
- **Show them all at once.** The validator collects; the panel lists. The user never plays whack-a-mole
  (TC-10: three problems, one round-trip).
- A summary panel that lists field errors too is not redundancy: the glyph tells *where*, the panel tells *how many*
  and *what*, and it is the only place a workflow error can appear.

## 2. Write messages the user can act on

| Good | Why | Bad |
|---|---|---|
| Title is required. | names the field's expectation | Validation failed. |
| Cost must be between $0 and $10,000. | states the bound they need | Invalid value. |
| Due date can't be in the past. | plain language, no jargon | DueDate < DateTime.Today |
| Closed work orders cannot be edited. Ask a Supervisor to reopen it. | says what to do next | Operation not permitted. |
| Only a Supervisor may set a cost above $2,500. | explains the rule instead of hiding the field | (silently ignoring the value) |

Conventions used in `WorkOrderValidator` and `WorkOrderRules`:

- Full sentences, one line, ending with a period. Messages start with the noun ("Title", "Cost", "Due date"), not with "Error:".
- Numbers are formatted for people (`$10,000`, not `10000.00`). Field captions in the panel use the screen's words
  (`Due date`, `Cost`, `Hours`), mapped from the command's property names in `CaptionForField`.
- Never blame the user ("you entered an invalid…"); describe the expectation.

## 3. Honest to the user, private about internals

When the store refuses the commit, the log and the user get two different things:

| Goes to `ILog.Error` (the server log) | Goes to the screen |
|---|---|
| the exception type and message (host names, table names, the stack), `tx#n rolled back` | `Strings.SaveFailed` — one calm sentence that promises the edits are still there and says what to do |

Why it matters: host names, table names and stack traces are a security leak and give the user nothing they can act
on. `Strings.cs` is the only source of user-facing failure text; handlers never show `ex.Message`.

## 4. Keep the user's work

After a failed save the editor must be exactly as the user left it:

- `ReportFailure` changes the status line, the banner and shows a toast — it never resets a field. The message says
  so: "Your changes are still here".
- Recovery is the same **Save** button: it sends the very same values again, and the success path takes over —
  glyphs cleared, panel hidden, status "● Work order #2002 saved.".

## 5. Do not confuse hints with guards

A disabled button, a hidden Cost field, a spin box `Maximum` — all are UX hints. In this app the cost spin box
deliberately allows 1,000,000 and the hours spin box 100,000, so the reviewer can see the *rule* reject the value,
not the control. The role rule makes the same point: as a Technician, a cost of 9,500 passes every field check
(it is in range) and is still rejected by the server, and the summary panel explains why in plain words.

## Evidence

- Clear the title, **Save**: red glyph beside Title (hover: "Title is required."), panel "1 problem needs attention · • Title — Title is required.", status **● 1 problem found — nothing was saved**.
- Set 1,200 estimated hours, **Save**: glyph beside Estimated hours; panel "• Hours — Hours must be between 0 and 999."
- Select closed #2006, change the title, **Save**: no glyph anywhere (every field is valid); panel "• Closed work orders cannot be edited. Ask a Supervisor to reopen it."
- As a Technician, set the cost of #2002 to 9,500, **Save**: no glyph; panel "• Only a Supervisor may set a cost above $2,500."
- A failed commit: no glyph, no panel; red banner + toast with `Strings.SaveFailed`, status **● Save failed — your changes are still here**; the detail is in the server log only.
