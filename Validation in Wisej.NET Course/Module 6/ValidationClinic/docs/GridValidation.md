# DataGridView Cell Validation and Row Errors

The contacts grid rejects uncommitted bad email/age values, explains conversion failures, marks Closed rows without a date and checks every row before saving the working copy.

## Decisions

- Replacing a previously valid grid email with an @-less value demonstrates why reading cell.Value checks stale data. FormattedValue contains the edit still in progress.

- The age handler uses TryParse and rejects non-numbers. DataError remains necessary for other conversion and commit failures, such as not-a-date in ClosedDate.

- The Closed rule appears on the row and date cell, with a visible summary. Correcting or changing status removes its messages without erasing unrelated conversion errors.

## Evidence to reproduce

| Action | Evidence |
|---|---|
| Double-click a grid email, type missing-at-sign, then Tab | CellValidating reads FormattedValue, refuses the edit and puts a message on the cell and in the summary. |
| Type abc, -1 or 121 in Age | The cell rejects each value; 0 and 120 pass. Escape cancels an invalid edit. |
| Set Status to Closed with no closed date | The row header and date cell both show the rule, and the visible summary explains it. |
| Type not-a-date in Closed date | DataError suppresses the conversion exception and logs technical details on the server. |
| Correct the date; Save grid | Every non-placeholder row is rechecked, including annotations and shared business rules, then saved together. |
| Start a new row but leave required fields blank; Save grid | The partially entered contact is validated; only the untouched placeholder is skipped. |

Every failed validation stage leaves the repository write count unchanged. A simulated write failure also
leaves saved contacts unchanged, retains the input, and is recoverable by clicking Save again.
These are reproduction instructions; the course README separately records which checks were executed.
