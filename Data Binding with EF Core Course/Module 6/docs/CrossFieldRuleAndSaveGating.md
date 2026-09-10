# Cross-field rule label, Save disabled while invalid

**Deliverable:** Summary label for cross-field rules and Save disabled while invalid.

## `validationSummaryLabel`

A `Wisej.Web.Label` (`TicketEditorForm.Designer.cs`), positioned under the due-date row and above the
button row, red-ish (`ForeColor` `178,59,39` on `BackColor` `253,236,234`, the same failure palette
`labelBanner` uses elsewhere in this course) when it is carrying messages. `ShowValidation` joins *every*
message — field-level and the cross-field rule alike — into it:

```csharp
this.validationSummaryLabel.Text = string.Join("   ·   ", messages.Select(m => m.Message));
this.validationSummaryLabel.Visible = true;
```

so a message with no control to attach an icon to is never silently dropped — the closed/future-due-date rule
does report `DueDate` (which has a control), but the label is where every message lands regardless, which is
what makes it the right home for a rule that reported no field at all (see the README's Self-check answers for
when that would be the better choice).

## The neutral pre-touch state

A blank "Add ticket" is *always* invalid at the moment it opens (no title, no customer, no category) — but
five red icons before a single keystroke would be noise, not feedback. `LoadEditorAsync` computes validity
without painting (`Validator.Validate(model)`, no `ShowValidation` call) and, only for a brand-new ticket that
is not yet valid, shows one neutral sentence instead of the raw validator text:

```csharp
this.validationSummaryLabel.Text = "Fill in Title, Customer and Category.";
this.validationSummaryLabel.ForeColor = /* neutral grey */;
this.validationSummaryLabel.BackColor = this.BackColor;
```

Editing an existing ticket (which is expected to already be valid) or a lab-scenario preset that happens to be
valid (the duplicate-number scenario: every annotation passes, only the database will object) shows no label
at all until Save is pressed or a field changes. The `_wireLiveValidation` field is what enforces the rule
"never paint before Load has finished" — see [`ValidationLayers.md`](ValidationLayers.md) and the
`TicketEditorForm` class remarks for the full reasoning.

## Live validation

Once `LoadEditorAsync` finishes (`_wireLiveValidation = true`), every subsequent field change re-runs the
validator and repaints, through two Designer-wired handlers:

- `Field_Changed` — `txtTitle`/`txtDescription.Validated`, `cboCustomer`/`cboCategory`/`cboStatus.SelectedValueChanged`.
  By the time these fire, the control's own `DataBindings.Add` entry has already pushed the new value into the
  model (`OnValidation` commits on `Validating`→`Validated`; `OnPropertyChanged` commits immediately).
- `DueDate_Changed` — `dtpDueDate.ValueChanged` and `.Validated`. `dtpDueDate` is not bound through
  `DataBindings.Add` (see the course cookbook and the `TicketEditorForm` remarks), so this handler copies
  `Checked`/`Value` into the model by hand, *then* validates — the one control where "the model already
  reflects the control" cannot be assumed.

Both funnel into `RunValidation(paint: true)`, which calls `Validator.Validate`, `ShowValidation`, and sets
`btnSave.Enabled = messages.Count == 0`.

## Save disabled while invalid — and while saving

`RunValidation` is the single place `btnSave.Enabled` is ever set:

```csharp
this.btnSave.Enabled = valid && !_saving;
```

`SaveAsync`'s `finally` calls `RunValidation(paint: false)` (after `_saving = false`) instead of unconditionally
setting `Enabled = true` the way Module 4 did — so after a caught `DbUpdateException` (the model was valid,
the *database* refused it) Save comes back enabled, letting the operator retry without re-typing anything;
after a validation failure it would already be disabled from the live-validation pass moments earlier.
`DeleteAsync`'s `finally` does the same.

## Evidence

- `dotnet build` — 0 warnings, 0 errors.
- `SupportDesk.Tests/TicketValidatorTests.cs` — `Closed_ticket_with_a_future_due_date_is_reported_against_DueDate`
  and its three neighbouring tests (today's date, a past date, an open ticket with a future date) prove the
  cross-field rule fires exactly when the lesson says it should — strictly future, and only for `Closed`.
- `RunValidation`'s `this.btnSave.Enabled = valid && !_saving` line and `SaveAsync`'s early `return` on a
  non-empty `messages` list are exercised indirectly by every `TicketValidatorTests` case (the same
  `Validate` call `RunValidation` makes) and directly provable only by a running dialog — see the README's
  "Verified / unverified" for what the browser reviewer confirms (the button's actual enabled/disabled state,
  the label's actual visibility and color).
