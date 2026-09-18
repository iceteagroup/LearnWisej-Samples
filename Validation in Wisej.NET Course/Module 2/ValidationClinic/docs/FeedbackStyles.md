# Programmatic Validation and ErrorProvider Feedback

The intake form adds pure `FieldValidation` results, one `ErrorProvider`, per-field clearing, a visible summary and an `InvalidMessage` comparison on the customer code.

## Decisions

- Clear removes every provider message, including an unrelated invalid email. SetError(txtName, "") removes only the corrected name.

- InvalidMessage is outside the provider collection. RefreshSummary reads the customer code explicitly; shared providers avoid needing that special case.

- Uniqueness is a save-time business/repository concern. A duplicate name is mapped to its field; a failed repository is a separate system message.

## Evidence to reproduce

| Action | Evidence |
|---|---|
| Load invalid values, then Save | Name, email and phone problems appear in the summary and beside their fields. |
| Correct email and Tab | Only the email message disappears; the remaining problems stay visible. |
| Leave customer code as X and leave its field | The editor shows InvalidMessage, with no provider icon for that field; the summary includes it explicitly. |
| Load valid values; Save | All field messages clear and the summary hides. |

Every failed validation stage leaves the repository write count unchanged. A simulated write failure also
leaves saved contacts unchanged, retains the input, and is recoverable by clicking Save again.
These are reproduction instructions; the course README separately records which checks were executed.
