# Deliverable 2 — Architecture Decision Records

**The ADRs themselves live in [`EnterpriseOps/Architecture/`](../Architecture/), not in this folder.** That is
deliberate: an ADR is part of the codebase it governs, so it travels with the branch that changes the structure
and is reviewed in the same pull request. `docs/` holds the lab deliverables and points here.

| ADR | Title | Status | Owner | Decided | Review by | Record |
|---|---|---|---|---|---|---|
| ADR-001 | Solution structure | Accepted | Tech lead | 2026-09-09 | 2027-03-09 | [`Architecture/ADR-001-SolutionStructure.md`](../Architecture/ADR-001-SolutionStructure.md) |

## How an ADR is written here

Every record uses the shape of the `ArchitectureDecision` record in
[`Architecture/ArchitectureGovernancePatterns.cs`](../Architecture/ArchitectureGovernancePatterns.cs) — the
lesson's code, verbatim:

```csharp
public sealed record ArchitectureDecision(
    string Id, string Title, string Context, string Decision,
    string Consequences, string Owner, DateTime Date, DateTime? ReviewDate);
```

so the Markdown always carries: **Status · Owner · Date · Review date · Context · Options considered · Decision ·
Consequences · How to revisit**. `ReviewDate` is not decoration — a decision with no review date is a decision
nobody will ever revisit, which is how architecture becomes technical debt.

## Rules

1. One ADR per decision that is expensive to reverse. Adding a button is not one; choosing where workflow logic
   lives is.
2. Numbered, never renumbered: `ADR-001`, `ADR-002`, … A superseded ADR keeps its number and gets
   `Status: Superseded by ADR-0NN`; it is never deleted, because the reasoning is the value.
3. Written *before* the code merges, reviewed in the same pull request as the change it justifies.
4. Every ADR has an owner and a review date. The review-date sweep is a scheduled team activity, not a hope.
5. Short. If it takes more than a page, the decision was more than one decision.
