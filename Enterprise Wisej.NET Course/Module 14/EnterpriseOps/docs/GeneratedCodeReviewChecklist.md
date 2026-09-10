# Generated-code review checklist — EnterpriseOps Command Center

**Deliverable:** Generated-code review checklist · **Module 14** · last verified 2026-09-10 · owner cara.admin

Generated code is a draft from an unknown contributor. It goes through the same review as any pull request,
with the same questions: where does state live, what can fail, what is secure, and what changes in production.

The checklist is short enough to apply to every change and specific enough to catch what this course has
taught. Six questions come from the module (Q1–Q6); four more (R7–R10) are the failures the earlier modules
demonstrated. **One Reject finding stops the change** — a warning is a conversation, not a veto.

The list is executable: `Services/GeneratedCodeReviewService.cs` runs it, and the Capstone Review screen
(**Review generated code**) reports every finding with its line, its evidence and the fix. Signing the result
is a separate permission (`ReviewGeneratedCode`) because a review is signed by a reviewer, not by the author.

## The checklist

| # | Question | Severity | What the gate looks for |
|---|---|---|---|
| Q1 | Did the model invent a Wisej.NET API? | Reject | a member called on a control-named field (`grid…`, `btn…`, `dgv…`, `txt…`, `lst…`, `pnl…`, `lbl…`, `cbo…`) or on a documented Wisej.NET type used statically (`Application`, `AlertBox`, `MessageBox`), where the member is not in the documented-API catalog |
| Q2 | Does any generated code store user, tenant, selected entity or workflow state in static fields? | Reject | a non-`readonly` static field, or a static property with a setter |
| Q3 | Is authorization enforced in services? | Reject | a write (`SaveChanges`, `.Update(`, `.Insert(`, `.Delete(`, a status assignment) or a mutating command method, with no permission check anywhere in the change |
| Q4 | Are HTML-capable paths reviewed? | Warning | `AllowHtml = true`, `innerHTML`, `.Html =`, or an `Eval(` built by concatenation |
| Q5 | Are background tasks tied unsafely to controls or sessions? | Reject | `Application.StartTask`, `Task.Run(`, `new Thread(`, a background job or a new timer, with no `IsDisposed` guard and no `Application.Update(` |
| Q6 | Can the code be tested without the UI? | Warning | a method that takes a Wisej.NET control **and** queries or persists in the same body |
| R7 | Is every failure visible — no exception swallowed? | Reject | a `catch` block that is empty, or that only returns |
| R8 | Is every disposable resource disposed? | Warning | `new SqlConnection/StreamReader/FileStream/HttpClient/…` with no `using` and nothing disposing it |
| R9 | Is client-supplied identity or scope re-established on the server, never trusted? | Reject | a tenant, user, role or permission assigned from something named for the client, the request, the arguments or the payload |
| R10 | Does the event handler stay thin — a service call, not the logic? | Warning | a `_Click` / `_Load` / `_Changed` / `_Tick` handler that persists or queries, or that runs to more than twelve statements |

## The documented-API catalog (Q1)

`Services/DocumentedApiCatalog.cs` holds every documented property, method and field of a `Wisej.Web`,
`Wisej.Core` or `Wisej.Base` type, plus every documented type name. It was **extracted from
`Wisej.Framework.xml`**, the XML documentation file shipped inside the `Wisej-4` 4.1.0 NuGet package — the
same source the documentation site is generated from — so "not in the documentation" means exactly that
rather than "not on a list somebody curated". Regenerating it for a new release is a one-line script; see
`README.md`.

The gate only asks the question where a human reviewer would: on calls that are clearly against the platform.
A call on `_service`, `_repository` or `_log` is project code and is not checked against the catalog.

A Q1 finding is not proof the API does not exist. It is proof that **nobody has shown that it does** — which
is the same thing at merge time.

## Applying it

1. Load or paste the change into Capstone Review → **Generated-code review**.
2. Press **Review generated code**. Every rule runs; every finding names its line and quotes it.
3. Read the verdict: `REJECTED` (a blocking finding), `ACCEPTED WITH WARNINGS`, or `ACCEPTED`.
4. Fix, or send back with the prompt header and the documentation links attached.
5. Press **Sign the decision** — recorded with the reviewer's name in the AI usage notes.

A finding you disagree with is still a finding: answer it in the decision record, do not delete it.

## Worked example — pull request #214

The walkthrough's generated draft (Capstone Review → **Load AI draft #214**) compiles and looks right in a
single-user demo. The checklist returns **eleven findings over 41 lines, nine of them blocking** — in the
order the screen lists them:

| Rule | Line | Severity | Finding |
|---|---|---|---|
| Q1 | 18 | Reject | `grid.EnableSmartVirtualScroll()` — not in the documentation |
| Q1 | 38 | Reject | `Application.RegisterBackgroundJob(…)` — not in the documentation |
| Q2 | 10 | Reject | `static WorkOrder _current;` — every tenant would share one "current" work order |
| Q2 | 11 | Reject | `static string _tenantId;` — the second user overwrites the first |
| Q2 | 12 | Reject | `static DataGridView _grid;` — a control of a session that may already be gone |
| Q3 | 26 | Reject | the change writes and contains no permission check |
| Q5 | 38 | Reject | a background job with no `IsDisposed` guard and no `Application.Update` |
| Q6 | 19 | Warning | `Bind` takes a `DataGridView` and queries the database in the same method |
| R7 | 31 | Reject | `catch (Exception) { }` — the incident is invisible |
| R9 | 17 | Reject | `_tenantId = tenantIdFromClient;` — the browser chose the tenant |
| R10 | 28 | Warning | `btnApprove_Click` persists inside the handler |

Every one of them would have compiled. Nine of them would have reached production.

**Rev 2** (**Load rev 2 (fixed)**) is the same feature as an instance service: session-scoped state,
authorization in the service, a visible failure path, and `DataGridView.VirtualMode` — cited from the
documentation. It is `ACCEPTED` with no finding.

## Evidence

Command Center → **Capstone review →** → **Generated-code review**:

- **Load AI draft #214** → **Review generated code** → red verdict
  `● REJECTED — 9 blocking finding(s) of 11 over 41 lines · 10 rules run`, eleven rows in the findings grid,
  and the trace on the right showing `Review: Q1 L18 [Reject] …` line by line.
- **Load rev 2 (fixed)** → **Review generated code** → green `● ACCEPTED — no checklist finding`.
- **Switch to ben.tech** on the Command Center, then review again → `Security: ReviewGeneratedCode denied`
  and an audited refusal: a Technician cannot sign a review.
