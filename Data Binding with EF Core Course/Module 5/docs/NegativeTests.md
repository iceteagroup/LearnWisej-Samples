# Negative tests: at least five, against `TicketValidator` alone

**Deliverable:** At least five negative test cases.

## Where they live

`SupportDesk.Tests/TicketValidatorTests.cs` — 13 tests against `TicketValidator` directly, no `TicketEditorForm`,
no `Page`, no session, no database (`TicketValidator` has no reference to `SupportDesk.Data` or `Wisej.Web` at
all). `SupportDesk.Tests/DuplicateNumberTests.cs` adds the one negative case that *can only* be proven against
the database: the duplicate-number `DbUpdateException`.

## The five the lab guide names, and which layer each exercises

| # | Test | Breaks | Layer exercised |
|---|---|---|---|
| 1 | `Empty_title_is_reported_against_Title` | `Title = ""` | Domain — `[Required]` on `TicketEditModel.Title` |
| 2 | `Title_over_180_characters_is_reported_against_Title` | `Title` = 181 chars | Domain — `[StringLength(SupportDeskContext.TitleMaxLength)]` |
| 3 | `Missing_customer_is_reported_against_CustomerId` | `CustomerId = null` | Domain — `[Required]` on `int?` |
| 4 | `Missing_category_is_reported_against_CategoryId` | `CategoryId = null` | Domain — `[Required]` on `int?` |
| 5 | `Closed_ticket_with_a_future_due_date_is_reported_against_DueDate` | `Status = Closed`, `DueDate` = +7 days | Domain — the hand-written cross-field rule, not an attribute |

Eight more round the set out: `Whitespace_only_title_is_reported_against_Title` (proves `[Required]` treats
whitespace as empty, not merely null/empty-string); `Title_at_exactly_180_characters_is_not_reported` (the
boundary itself must pass — an off-by-one bug would fail this, not the over-length test); three tests proving
the cross-field rule is *strictly* future and *only* for `Closed` (today, a past date, an open ticket with a
future date — all three must **not** be reported, so a validator that fires too eagerly fails these);
`Every_broken_rule_is_reported_at_once_not_just_the_first` (asserts exactly 3 messages when 3 rules break,
proving `validateAllProperties: true` is doing its job); `Description_over_4000_characters_is_reported_against_Description`;
and `Valid_model_produces_no_messages` (the positive case every negative test's "starting point" is copied from).

## The database negative case

`DuplicateNumberTests.SaveAsync_with_forceDuplicateNumber_throws_DbUpdateException_wrapping_the_UNIQUE_constraint`
inserts a ticket through `TicketCommandService.SaveAsync(model, TimeSpan.Zero, forceDuplicateNumber: true)` —
the same lab prop `chkDuplicateNumber` arms — and asserts the exception type, the inner `SqliteException` type,
and that its message contains `UNIQUE constraint failed: Tickets.Number`. This is the one negative case that
exercises the **database layer**, not the domain layer: `TicketValidator` cannot see it coming (see
[`DuplicateNumberHandling.md`](DuplicateNumberHandling.md)).

## Student review question — the first one

*"If you removed every DataAnnotation from `TicketEditModel` but kept them on the `Ticket` entity, which of
your five negative tests would still fail before `SaveChangesAsync`, and why?"*

None of them — see the README's Self-check answers for the full reasoning, but in short: every one of the
five tests above calls `TicketValidator.Validate` directly, and `TicketValidator` inspects only
`TicketEditModel`'s own attributes through `Validator.TryValidateObject`. The entity's annotations are mapping
instructions EF Core reads at model-building time; they are never consulted by `Validator.TryValidateObject`
against a *different* object (`TicketEditModel`), and EF Core does not run a DataAnnotations pass of its own
before `SaveChangesAsync` either. All five tests would fail (produce zero messages where they expect one or
more) — which is exactly the point the lesson is making: annotations on the entity protect the column, not the
screen.

## Evidence

- `dotnet build SupportDesk.slnx -nologo -v q` — 0 warnings, 0 errors.
- `dotnet test SupportDesk.Tests -nologo -v q` — **68 passed**, 0 failed: the 52 carried over from Modules 1–4
  unchanged, 13 new in `TicketValidatorTests.cs`, 3 new in `DuplicateNumberTests.cs`.
