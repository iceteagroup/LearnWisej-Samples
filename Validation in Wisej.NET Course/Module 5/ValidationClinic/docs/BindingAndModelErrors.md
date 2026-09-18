# Data-Bound Validation and Model Errors

The form binds to an annotated model, commits pending edits before validation and maps model errors to fields. A separate dialog demonstrates automatic `IDataErrorInfo` feedback and switching the provider source.

## Decisions

- The missing step is committing pending edits before model validation. EndEdit completes the binding edit; this implementation also calls WriteValue and uses OnPropertyChanged to cover Enter-to-save.

- Without validateAllProperties: true, StringLength, EmailAddress and Range are skipped. Required alone does not enforce them.

- Annotations and ContactValidator can be invoked by an import service. Attributes do not execute themselves; a non-UI caller must explicitly run both validators.

## Evidence to reproduce

| Action | Evidence |
|---|---|
| Load valid values; set Age to 130; Save | The Integer field rule passes, but Range rejects the bound model and the trace stops at the model stage. |
| Enter a name of 81 characters; Save | StringLength produces a Name field error; 80 is allowed. |
| Edit email and immediately press Enter | EndEdit and explicit binding WriteValue run before the model checks; the typed value is checked. |
| Cancel; open Data-bound errors; blank Name; Check model | The bound ErrorProvider reads IDataErrorInfo. This dialog never calls SetError or saves. |
| Switch source in Data-bound errors | BindToDataAndErrors receives the new BindingSource and an empty member in one call. |

Every failed validation stage leaves the repository write count unchanged. A simulated write failure also
leaves saved contacts unchanged, retains the input, and is recoverable by clicking Save again.
These are reproduction instructions; the course README separately records which checks were executed.
