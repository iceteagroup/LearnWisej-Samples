# Custom Validation Methods, Services and Rules

A UI-independent `ContactValidator`, a `MinimumAgeValidationRule` and paired start/end date feedback separate field shape from business meaning.

## Decisions

- The custom rule gives feedback when the picker is validated. The service protects callers without controls. This sample shares the age calculation between both.

- Clearing all errors after showing date errors removes those icons. The save sequence maps model errors first, then applies the date feedback.

- Call ContactValidator.Validate with a plain ContactEditModel. The capstone test project includes that test without creating a Wisej session.

## Evidence to reproduce

| Action | Evidence |
|---|---|
| Load valid values; move end date before start date | Both date pickers show the same message; focus can move to either picker to correct it. |
| Set birth date to tomorrow or less than 18 years ago | The custom age rule rejects it. The shared service repeats the age policy when invoked without a UI. |
| Use a birth date exactly 18 years ago | The boundary is accepted. |
| Correct the date order; Save | Both date messages clear; field and business stages pass before the write. |

Every failed validation stage leaves the repository write count unchanged. A simulated write failure also
leaves saved contacts unchanged, retains the input, and is recoverable by clicking Save again.
These are reproduction instructions; the course README separately records which checks were executed.
