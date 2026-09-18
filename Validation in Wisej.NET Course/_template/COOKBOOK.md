# ValidationClinic cookbook - Wisej-4 4.1.0 / .NET 10

## Structure

Each module is standalone: solution, designer/code-behind pairs, Models, Services, Validation (from Module 3)
and docs. Do not link runtime code to a sibling module. Keep the two target frameworks and explicit CLI `-f`.
Use ports 5901-5907; the template is 5900. The repository and working copy belong to MainPage, never a static.

## Validation ordering

`Validating` cancels; `Validated` tidies accepted values and never saves. `EnableAllowFocusChange` keeps
cross-field correction possible. Cancel has `CausesValidation = false`; Save explicitly calls ValidateChildren.
An ErrorProvider message alone does not reject data. Pair it with e.Cancel, or return false from a rule.

The extender's `Validating` event runs BEFORE its rules. Do not interpret e.Cancel there as the final rule result.
For icon-backed summaries, register control.Validating handlers after SetValidationRules to refresh from the
finished provider messages. A custom IErrorProvider receives actual SetError calls directly.
Every field rule array is fresh. Telephone has an explicit 0000000000 mask; Regex uses ValidateExpression.

Wisej DateTimePicker.Value is a non-nullable DateTime in this framework. The domain's dates are nullable so a
service can reject missing dates from non-UI callers. The custom age rule and service share an anniversary
comparison; the boundary policy treats February 28 as the anniversary of February 29 in a non-leap year.

## Binding and persistence

Use a BindingSource for the model. EndEdit completes its edit; OnPropertyChanged and explicit Binding.WriteValue
also commit control values before model checks, including Enter-to-save. Run TryValidateObject with
validateAllProperties: true. Keep unmapped model errors in the visible summary.

The IDataErrorInfo comparison is a separate dialog: mixing automatic model errors with manual SetError calls
on one provider obscures which mechanism owns the message. BindToDataAndErrors changes source/member together.

Snapshot returns copies. Repository.SaveAll builds a new list, checks duplicates and simulated failure, then
replaces persisted state. A failure cannot partly save grid edits. The repository is a stand-in, not a complete
service boundary: any real import/API caller must run model and business validation as well.

## Grid event traps

Always inspect CellValidating.FormattedValue. EndEdit must succeed before Save validates committed rows.
Check non-placeholder rows even if they currently have no icons; an untouched field might never have validated.
Check model annotations and the shared validator as well as cell and row rules.

Guard events while replacing the grid BindingSource: rebinding can validate an old current cell while rows
are being rebuilt. Never enumerate row.ErrorText inside DataError: reading a source error can itself raise
DataError and recursively re-enter the handler. Set the friendly message directly, log the exception, and
refresh the full summary from normal editing events. Do not erase a conversion error when clearing a different rule.

## Evidence

Both frameworks compile, domain tests run in Module 7, and browser verification caught the rebinding and
error-enumeration traps above. See the course README for verification scope and each module's What to try table.
