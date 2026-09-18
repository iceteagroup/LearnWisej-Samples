# Production Validation UX and Capstone

The capstone combines binding, field rules, annotations, business rules and the contacts grid in one ordered Save pipeline. `SummaryErrorProvider` keeps extender, icon and summary feedback together.

## Decisions

- The pending edit must be committed before ValidateChildren and model checks. The trace records binding, fields, annotations, business, grid and write in that order.

- SummaryErrorProvider stores one entry per control and replaces or removes it. Distinct message text avoids duplicate cross-field lines.

- DuplicateNameException is a correctable field problem. An unexpected exception stays in the server log and leaves the user a friendly retry message with their entries intact.

## Evidence to reproduce

| Action | Evidence |
|---|---|
| Open intake; Save with empty Name and Email | The field stage stops the pipeline; the icons and custom summary show the errors. |
| Load valid values; set Age to 130; Save | Field shape passes, then DataAnnotations blocks the model. No grid check or write follows. |
| Set start after end; Save | The business layer marks both dates and stops the pipeline. |
| Cancel; mark a grid row Closed without its date; reopen intake, Load valid values, Save | The grid stage blocks the entire save; the intake summary includes the row message. |
| Correct the grid row and intake; Save | One atomic repository replacement persists the contact and edited grid rows; the trace ends with write. |
| Create a new contact named Ada Lovelace; Save | The duplicate-name exception becomes a Name field error. No grid or contact changes are persisted. |

Every failed validation stage leaves the repository write count unchanged. A simulated write failure also
leaves saved contacts unchanged, retains the input, and is recoverable by clicking Save again.
These are reproduction instructions; the course README separately records which checks were executed.
