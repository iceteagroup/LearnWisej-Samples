# Architecture defense deck — outline

**Deliverable:** Architecture defense deck outline · **Module 14** · last verified 2026-09-10 · owner ana.ops

Twelve slides, thirty minutes, half of it questions. The deck does not demonstrate features — the demo script
does that. The deck answers *why the design is this shape* and *what happens when it breaks*.

Rule for every slide: **one claim, one piece of evidence, one thing that could go wrong.** A slide without
evidence is an opinion.

## Slide 1 · The claim

The EnterpriseOps Command Center is a multi-tenant field-service application that a team can evolve for
years: decisions are recorded, logic is reviewable without the Designer, every failure path is demonstrated,
and the deployment can be reversed.

*Evidence:* the diagram in `capstone-package.svg`. *Risk:* a claim this broad is only worth the eleven slides
behind it.

## Slide 2 · Solution structure and ADR-001

Folder-per-layer (`UI/ Domain/ Services/ Data/ Security/ Diagnostics/ docs/`), one project so the sample runs
with one `dotnet run`, mapping 1:1 onto the multi-project split.

*Evidence:* the solution tree; ADR-001. *Risk:* one project makes an accidental reference easy — the review
gate is what stops it.

## Slide 3 · Where state lives

`SessionContext` per browser session, in `Application.Session`. Statics hold immutable reference data only:
the permission matrix and the documented-API catalog.

*Evidence:* `Program.Main`, `Services/SessionContext.cs`, checklist rule Q2. *Risk:* one static field is
enough to undo it — which is why Q2 is a Reject, not a warning.

## Slide 4 · The command path

Handler → service → store, with a typed command in and a typed result out. Authorize, validate, write with
the expected version, audit, return.

*Evidence:* `WorkOrderService.ApproveAsync`, and the audit line in the footer during the demo. *Risk:* a screen that reaches the
store directly; the gate's Q3 and R10 look for exactly that.

## Slide 5 · Tenancy and authorization

The tenant comes from the session, never from the caller. Permissions are checked in the service. Denials are
audited.

*Evidence:* the cross-tenant guard in `ApproveAsync`; the Technician refusal in `ApproveAsync`; checklist R9.
*Risk:* a new endpoint that forgets the check — mitigated by the store's tenant-scoped `Query`.

## Slide 6 · Concurrency

Optimistic versioning. The command carries the version the user saw; the store refuses a stale write and the
user is told what happened in a sentence they can act on.

*Evidence:* the stale-version refusal in the demo (stop 3). *Risk:* last-write-wins creeping back in through a bulk
operation.

## Slide 7 · Failure paths as a deliverable

Every module of this course shipped at least one failure path and its recovery. They are demonstrated, not
described.

*Evidence:* the demo script, stops 3, 5 and 6. *Risk:* failure paths rot faster than features; they are on
the demo script so they are exercised every time the capstone is shown.

## Slide 8 · Operations

Diagnostics probes that report evidence rather than a green tick; correlation ids on every command and every
audit line; a health card the on-call engineer can read.

*Evidence:* **▶ Run health check**. *Risk:* probes that only test themselves — each one names what it counted.

## Slide 9 · Deployment and rollback

Configuration by environment, a health probe, a release with a rollback that is rehearsed rather than
theoretical.

*Evidence:* the release runbook (Module 12). *Risk:* the rollback that was never run — the open item on the
readiness statement.

## Slide 10 · AI-assisted development, with ownership

Prompt header in, no undocumented APIs out. Generated code is reviewed like junior code, and the decision is
recorded.

*Evidence:* PR #214 in the demo — eleven findings, nine blocking, then an accepted rev 2. *Risk:* the gate is
a reviewer's assistant, not a compiler; a Q1 finding says nobody has shown the API exists.

## Slide 11 · Documentation the tools can read

`docs/index.json` in an MCP `resources/list` shape next to a Markdown index for people. Every entry has a
purpose, a module and a verification date.

*Evidence:* the **Documentation index** tab and its **Resolves** column. *Risk:* an index that drifts —
the package verification fails when a listed path stops resolving.

## Slide 12 · What is not done

The open items from `ProductionReadiness.md`, read out with their owners, then the sign-off.

*Evidence:* the readiness statement. *Risk:* none — this is the slide that removes the risk of pretending.

## Questions to rehearse

- Where does state live, and what happens with two browsers?
- Who can call what, and how would you prove it in an audit?
- How is a bad release reversed, and when was that last rehearsed?
- What would the team do at two in the morning?
- Which generated code was accepted, and why?
- Was every platform-specific claim checked against the documentation?
- What would you change next, and what would you need to change it safely?

**A defense succeeds when every decision can be traced to a reason, every risk has a named mitigation, and
the team can say plainly what they would change next.**
