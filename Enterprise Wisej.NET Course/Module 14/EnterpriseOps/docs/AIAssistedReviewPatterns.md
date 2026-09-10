# AI-assisted review patterns — EnterpriseOps Command Center

**Deliverable:** AI usage notes (patterns) · **Module 14** · last verified 2026-09-10 · owner ana.ops

This is the document the walkthrough opens first: the pattern the team follows whenever an assistant writes
code for this project. Two halves — what goes **into** a session, and what every answer is asked on the way
out.

## Prompt header

```
You are working on a Wisej.NET enterprise application. Follow these rules:
1. Keep UI event handlers thin.
2. Put workflow, validation, authorization, persistence, and integration logic in services.
3. Treat JavaScript as enhancement only.
4. Preserve session and tenant safety.
5. Do not use APIs unless they exist in the official Wisej.NET documentation.
6. Include failure paths and review notes.
```

The prompt carries the architecture. Rule 5 makes the official documentation the boundary: **no undocumented
APIs**. The full library of task prompts that build on this header is `PromptLibrary.md`.

## Review questions

Asked of every answer, before it becomes a commit:

1. Did the model invent a Wisej.NET API?
2. Does any generated code store user, tenant, selected entity, or workflow state in static fields?
3. Is authorization enforced in services?
4. Are HTML-capable paths reviewed?
5. Are background tasks tied unsafely to controls or sessions?
6. Can the code be tested without the UI?

`GeneratedCodeReviewChecklist.md` turns these six into a runnable gate and adds four more the earlier modules
earned the hard way (swallowed exceptions, undisposed resources, trusted client input, fat handlers).

## Where AI helps, and where it does not

| Most valuable | Least valuable |
|---|---|
| a service skeleton from a written command definition | security logic |
| test cases from a failure-path matrix | concurrency |
| a migration inventory from a solution file | platform APIs the developer does not know well |
| a runbook turned into a checklist | anything where verification is slower than writing it |

The line is verification cost. Where the developer already knows what correct looks like, the assistant is a
fast typist. Where they do not, it is a confident stranger.

## The rule

**Accept a suggestion only when someone on the team can explain it without the tool present.**

Generated code is reviewed like junior code: checked, tested, defended. The acceleration is real; the
ownership does not move.

## Recording the decision

Every acceptance and every rejection is a recorded, defensible decision — `AIUsageNotes.md` and, in the
running app, Capstone Review → **AI usage notes**. The review question the module asks is *which generated
code was accepted and why*, and a record that only lists what was accepted answers half of it.

## Evidence

Capstone Review → **AI prompt library** shows this header expanded into every task prompt; → **Review
checklist** shows the six questions above as rules Q1–Q6 with the severity each carries.
