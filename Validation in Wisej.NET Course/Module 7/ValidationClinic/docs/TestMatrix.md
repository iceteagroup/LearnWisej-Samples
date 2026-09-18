# ValidationClinic test matrix

Use Load valid values before each independent case. Verify the repository write count after every Save.

| Rule | Required / invalid | Valid / boundary | Result |
|---|---|---|---|
| Name | empty or whitespace; 81 characters | Maria Chen; 80 characters | Required stops fields; StringLength stops model |
| Email | empty; maria.example.com | maria@example.com | Required then Email; grid checks the typed value |
| Phone | empty; 123 | 2125550123 | Required then ten-digit Telephone mask |
| Customer code | X; CUS-123; CUS-12345 | CUS-0000; CUS-9999 | Regex enforces CUS plus exactly four digits |
| Credit limit | money | blank (optional); 0; 100.50 in en-US | Currency checks shape using the current culture |
| Age | abc; -1; 121 | 0; 120 | Integer handles shape; Range/model and grid handle limits |
| Birth date | future; day before eighteenth birthday | eighteenth birthday | Shared age calculation; February 29 becomes February 28 in a non-leap anniversary year |
| Dates | missing date in model; start after end | start equals end; start before end | Both fields marked; equality is accepted |
| Closed row | Closed without date | Open without date; Closed with date | Row, date cell and summary agree |
| Conversion | not-a-date in Closed date | an ISO date such as 2026-09-18 | DataError suppresses and logs failure; correction clears it |
| Duplicate name | Ada Lovelace, case-insensitive and trimmed | Maria Chen | Duplicate maps to Name, no partial grid write |
| Repository failure | Fail next write then valid Save | retry the same Save | First attempt preserves data; retry writes once |
| Pending edits | edit email and immediately press Enter | new valid address | Bound model sees current input before validation |
| New row | start a row but omit required values | leave placeholder untouched | Partial row checked, untouched placeholder skipped |
| Recovery | invalid Name, then Cancel | reopen dialog | Cancel always works without writing |

The automated tests cover the model/service/repository rows. The UI paths also need browser testing:
the presence of an error icon alone is not proof that an edit or a write was rejected.
