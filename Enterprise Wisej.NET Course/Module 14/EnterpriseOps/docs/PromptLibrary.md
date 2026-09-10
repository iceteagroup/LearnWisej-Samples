# AI prompt library — EnterpriseOps Command Center

**Deliverable:** AI prompt library · **Module 14** · last verified 2026-09-10 · owner ana.ops

The quality of generated code follows the quality of the context it was given. Every AI session on this
project starts from the same header, so the whole team starts from the same baseline and a reviewer knows what
the model was told before reading what it produced.

The app reads this file at run time (`PromptLibraryService`, Capstone Review → **AI prompt library**): each
`###` section below is one prompt, and the placeholder `<<prompt header>>` is expanded into the header when a
prompt is shown. A prompt that does not pull in the header is flagged in the screen — that is the point of
having the library at all.

## Prompt header

Pasted first, in every session, before anything is asked:

```
You are working on a Wisej.NET enterprise application. Follow these rules:
1. Keep UI event handlers thin.
2. Put workflow, validation, authorization, persistence, and integration logic in services.
3. Treat JavaScript as enhancement only.
4. Preserve session and tenant safety.
5. Do not use APIs unless they exist in the official Wisej.NET documentation.
6. Include failure paths and review notes.
```

Rule 5 is the boundary the rest of the module is built on. The project keeps a catalog of the documented
Wisej.NET surface (generated from `Wisej.Framework.xml`, the XML documentation shipped in the `Wisej-4`
package), and checklist rule **Q1** rejects any platform member that is not in it. An assistant that invents
`grid.EnableSmartVirtualScroll()` is not caught by a compiler — that method would only fail at review.

#### Project context to attach

Attach with the header when the task touches architecture:

- the solution tree (`UI/ Domain/ Services/ Data/ Security/ Diagnostics/ docs/`) and ADR-001;
- `docs/DocumentationIndex.md` — or point an MCP documentation endpoint at `docs/index.json`;
- the reference screen (`UI/CommandCenterDashboard.cs`) as the shape a screen is expected to have;
- `docs/GeneratedCodeReviewChecklist.md`, so the model knows the questions its output will be asked.

---

## Task prompts

### P1 · Draft a service from a command definition

- **Purpose:** turn a written command (name, inputs, rules, failure modes) into a service skeleton the team can review.

```
<<prompt header>>

Task: write the application service for this command.

Command: ApproveWorkOrderCommand { WorkOrderId, ExpectedVersion, Note }
Rules:
 - the caller's tenant and user come from CommandContext, never from the parameters;
 - the permission ApproveWorkOrder is checked in the service before anything is read or written;
 - the write carries ExpectedVersion and fails visibly when the row moved on;
 - an audit line is written whether the command was allowed or refused;
 - the return type is a typed result, never an entity.

Return the service class only. For every Wisej.NET or .NET API you use, name the documented member you are
relying on. If you are not certain a member exists, say so instead of using it.
```

### P2 · Generate test cases from a failure-path matrix

- **Purpose:** turn the failure paths a screen claims to handle into a list of cases a reviewer can tick off.

```
<<prompt header>>

Task: list the test cases for this command, one line each: input, expected result, expected audit line.

Cover at least: the happy path, permission denied, cross-tenant request, stale version, a status that cannot
be approved, and an unexpected exception. Say for each case whether it can be tested without the UI.
```

### P3 · Review a change against the project checklist

- **Purpose:** a second opinion before a human review — never instead of one.

```
<<prompt header>>

Task: review the change below against these questions, answering each with the line number or "not present":

Q1 Did it use a Wisej.NET API that is not in the official documentation?
Q2 Does it store user, tenant, selected entity or workflow state in a static field?
Q3 Is authorization enforced in the service?
Q4 Are HTML-capable paths reviewed?
Q5 Are background tasks tied unsafely to controls or sessions?
Q6 Can the code be tested without the UI?

Do not rewrite the code. Report findings only.
```

### P4 · Write the documentation entry for a new document

- **Purpose:** keep `docs/index.json` complete, so a maintainer (or a documentation MCP endpoint) can find it.

```
<<prompt header>>

Task: write the index entry for the document below, as one JSON object with the fields
id, title, purpose, path, module, lastVerified, uri, mimeType, decisions.

"purpose" is one sentence, written for someone who has never met this team. "decisions" lists the decisions
the document records, not its headings. Do not invent a lastVerified date — use the one I give you.
```

### P5 · Draft a runbook step from a diagnostic

- **Purpose:** turn something the team learned at 2 a.m. into a step the next person can follow.

```
<<prompt header>>

Task: write one runbook step for this symptom.

Format: symptom → what to check first → the command or screen that shows it → the decision → the rollback.
Name the screen and the probe in this application (Command Center → Diagnostics · health probes). Do not
suggest a tool the project does not already use.
```

### P6 · Draft the migration inventory of a legacy screen

- **Purpose:** the boring, mechanical part of a port, done fast and checked slowly.

```
<<prompt header>>

Task: list what this WinForms screen contains: controls, event handlers, direct database access, static
state, and anything that touches the file system or the registry.

Group by "port as is", "must move into a service", and "will not work on the web". Do not propose Wisej.NET
replacements for controls you cannot name from the documentation.
```

## How this library is used

1. Paste the header. Never skip it, even for a one-line change.
2. Paste the task prompt and the real project context (files, not descriptions of files).
3. Read the answer as a draft from an unknown contributor.
4. Run `docs/GeneratedCodeReviewChecklist.md` over it — in the Capstone Review screen, **Review generated code**.
5. Record the decision in `docs/AIUsageNotes.md`: what was accepted, by whom, and why.

## Evidence

Open the app (Command Center → **Capstone review →** → **AI prompt library**): every prompt in this file is
listed with its purpose, and the pane on the right shows it expanded — the header first, then the task. The
status line above the list says whether the selected prompt pulls the header in.
