# Five-minute capstone demo

1. Open intake and Save with untouched Name and Email. Point to the field errors, summary and zero writes.
2. Load valid values, set Age to 130, Save. Show field success followed by the model rejection in the trace.
3. Set Age back to 30; set end before start. Show both date errors; fix the order.
4. Cancel. In the grid, replace an email with missing-at-sign and Tab. Correct it. Type not-a-date in Closed date,
   observe friendly conversion feedback, then Escape or enter a valid date.
5. Set a row to Closed without a date, then open intake, load valid values and Save. The grid stage blocks it.
6. Cancel, repair the grid date, reopen intake and load valid values. Choose Fail next write and Save.
   Explain the system message and retained input. Retry: every stage passes and the write count becomes one.
7. Open a new intake and try Ada Lovelace. Contrast the Name message with the prior system failure.
