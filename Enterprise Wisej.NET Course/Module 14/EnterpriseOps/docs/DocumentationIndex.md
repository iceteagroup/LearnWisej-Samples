# Project documentation index — EnterpriseOps Command Center

**Deliverable:** Project documentation index · **Module 14** · last verified 2026-09-10 · owner ben.tech

A project that will be maintained with AI assistance needs documentation a **tool** can retrieve, not only a
person can browse. This file is the index for the person; [`index.json`](index.json) is the same index in an
MCP `resources/list` shape for the tool, and both are checked against the files on disk by the running app.

Write for the reader who has never met the team. If a note relies on knowledge that exists only in someone's
head, it will be wrong in a year.

## The index

| Document | Purpose | Path | Module | Last verified |
|---|---|---|---|---|
| AI prompt library | The project-rules header every AI session starts with, and the task prompts that build on it. | [`docs/PromptLibrary.md`](PromptLibrary.md) | 14 | 2026-09-10 |
| Generated-code review checklist | The ten questions asked of every change, generated or not, and what stops a merge. | [`docs/GeneratedCodeReviewChecklist.md`](GeneratedCodeReviewChecklist.md) | 14 | 2026-09-10 |
| AI-assisted review patterns | What goes into an AI session on this project, and what every answer is asked on the way out. | [`docs/AIAssistedReviewPatterns.md`](AIAssistedReviewPatterns.md) | 14 | 2026-09-10 |
| AI usage notes | Which generated code was accepted, by whom and why — including what was rejected. | [`docs/AIUsageNotes.md`](AIUsageNotes.md) | 14 | 2026-09-10 |
| Project documentation index (Markdown) | This file: every document with its purpose and verification date. | [`docs/DocumentationIndex.md`](DocumentationIndex.md) | 14 | 2026-09-10 |
| Project documentation index (MCP resources) | The machine-readable index a documentation endpoint would serve as `resources/list`. | [`docs/index.json`](index.json) | 14 | 2026-09-10 |
| Capstone demo script | The seven-stop production architecture review, each stop pairing a working screen with a failure path. | [`docs/CapstoneDemoScript.md`](CapstoneDemoScript.md) | 14 | 2026-09-10 |
| Architecture defense deck outline | Twelve slides: one claim, one piece of evidence and one risk each. | [`docs/DefenseDeckOutline.md`](DefenseDeckOutline.md) | 14 | 2026-09-10 |
| Production readiness statement | What is ready, the four questions answered, the open items with owners, the sign-off. | [`docs/ProductionReadiness.md`](ProductionReadiness.md) | 14 | 2026-09-10 |
| Capstone architecture diagram | One picture of the capstone: the session, the layers a command crosses, the package that ships with it. | [`docs/capstone-package.svg`](capstone-package.svg) | 14 | 2026-09-10 |

## What each entry has to carry

| Field | Why a tool (and a new maintainer) needs it |
|---|---|
| `id` | a stable handle that survives a rename of the file |
| `title` | what to call it in an answer |
| `purpose` | one sentence, so a retrieval can decide whether this is the right document without reading it |
| `path` / `uri` | where it is; the app verifies this resolves |
| `mimeType` | how to render it |
| `module` | which part of the system it belongs to, so an answer can be scoped |
| `lastVerified` | how much to trust it; supplied by a person, never generated |
| `decisions` | the decisions it records — not its headings. This is the field that makes an index worth retrieving |

Architecture decision records are the most valuable entries, because they explain **why** a design exists and
a tool can cite them when asked to change it. Runbooks, the permission matrix, the failure-path matrix and
the configuration table follow.

## Documents this build does not include

The capstone sample carries the Module 14 deliverables. The full project index would also list the documents
the earlier modules produced — ADR-001 (solution structure), the coding standards, the migration inventory,
the risk matrix, the permission matrix, the error-mapping table, the performance budget and the release
runbook — each with the same fields. One of them is deliberately still missing and is tracked as open item 1
of the readiness statement:

| Document | Status | Owner |
|---|---|---|
| `docs/SecurityReviewSignOff.md` | not written — the review happened, the signed record did not | ana.ops |

**Fail: missing document** on the Capstone Review screen adds exactly that entry to the index so a reviewer
can see what an incomplete index costs: the path stops resolving, the endpoint would answer 404, and the
package verification refuses to pass.

## Keeping it true

1. A new document is not finished until it has an index entry (prompt P4 drafts one; a person supplies the date).
2. The verification date is renewed when the document is re-read, not when it is edited.
3. `✓ Verify package` in the app fails if a required deliverable is missing from the index, or if any listed
   path does not resolve. Run it before submitting.

## Evidence

Capstone Review → **Documentation index**: `docs/index.json` read back with every field above, and a
**Resolves** column that says whether the file is really there. The trace line
`Docs: resources/list → {"resources":[…]}` is the answer a documentation MCP endpoint would return for this
project, printed so the shape can be checked rather than assumed.
