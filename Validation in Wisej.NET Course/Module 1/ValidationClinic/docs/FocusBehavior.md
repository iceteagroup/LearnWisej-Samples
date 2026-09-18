# Validation Fundamentals

The customer intake form introduces `Validating`, `Validated`, `e.Cancel`, `AutoValidate`, a working Cancel button and the `ValidateChildren()` save guard.

## Decisions

- An icon alone does not reject input. Without e.Cancel, ValidateChildren can return true and Save can continue.

- Leaving an editor runs its field handler; Save runs ValidateChildren for fields the user never visited, such as an untouched email.

- EnableAllowFocusChange lets the user reach another field; EnablePreventFocusChange holds focus. Both reject an invalid Save. Cancel uses CausesValidation = false in either mode.

## Evidence to reproduce

| Action | Evidence |
|---|---|
| Open customer intake; enter a name but leave email untouched; Save | The untouched email is checked, an error appears, and the repository write count stays at zero. |
| Type an @-less email and leave the field | An error is shown; focus may move because this sample chooses EnableAllowFocusChange. Save still refuses. |
| Type a padded name and leave it | Validated trims only accepted text; it never writes to the repository. |
| Cancel while name is invalid | The dialog closes and the repository remains unchanged. |

Every failed validation stage leaves the repository write count unchanged. A simulated write failure also
leaves saved contacts unchanged, retains the input, and is recoverable by clicking Save again.
These are reproduction instructions; the course README separately records which checks were executed.
