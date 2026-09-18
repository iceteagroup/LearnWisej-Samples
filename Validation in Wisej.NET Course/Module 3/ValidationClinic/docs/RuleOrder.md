# Built-in Rules and the Validation Extender

The Validation extender replaces the repeated field handlers with reusable Required, Email, Integer, Telephone, Currency and Regex rule arrays.

## Decisions

- Required belongs first so an empty email is explained as missing. Some optional format rules accept empty values, so rule behavior must be verified rather than assuming every format rule rejects blanks.

- The extender owns the event hooks and repetition. The application still chooses required fields, rule order, masks, wording and display targets.

- A two-field date rule must run on Save and after either date changes; a rule on just one field can miss edits to the other.

## Evidence to reproduce

| Action | Evidence |
|---|---|
| Blank email; Save | Required is first, so the message is Email is required. |
| Type maria.example.com into email; Tab | The Email rule reports the format problem; a valid address clears it. |
| Use abc for age or X for customer code | Integer and Regex rules reject the values before a write. |
| Use 2125550123 as phone and 100.50 as credit limit | Telephone uses an explicit ten-digit mask; the currency rule accepts a formatted number for the current culture. |

Every failed validation stage leaves the repository write count unchanged. A simulated write failure also
leaves saved contacts unchanged, retains the input, and is recoverable by clicking Save again.
These are reproduction instructions; the course README separately records which checks were executed.
