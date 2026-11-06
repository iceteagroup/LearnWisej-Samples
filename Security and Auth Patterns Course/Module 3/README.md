# AccessOps · Security & Auth Patterns · Module 3

Local lab build for **Module 3 · Authorization: Roles, Claims and Permissions**. The approvals
screen from the walkthrough - *AccessOps · Approvals*, the navigation built from the permission set
(`N of 5 entries · from SessionContext.Permissions`), the request grid with its Action column
(*Approve*, *Approve · not your report*, *— no approval rights*, *Approved*), the refusal banner and
the `STATUS` strip - on top of services that decide on the server, and an xUnit project that
proves it without a browser.

> **Wisej.NET version.** These samples target the released **Wisej-4 4.1.7**. Module 2's passkey
> sign-in (a Wisej.NET 4.2 component) is not part of them; nothing in this module depends on it.

## Run it

```bash
cd "Security and Auth Patterns Course/Module 3/AccessOps"
dotnet run -f net10.0 --urls http://localhost:6403
```

```bash
cd "Security and Auth Patterns Course/Module 3"
dotnet test
```

Demo accounts (password in [Data/seed-credentials.json](AccessOps/Data/seed-credentials.json)):
`d.ortiz` (Operations employee), `m.keller` (Operations manager of Dana Ortiz, Priya Nair and Ines
Duarte), `s.reyes` (Administrator), plus Module 2's `r.almeida` and `k.osei`. Sign in as each in its
own tab - every tab is its own session.

## What to try

| Who | Action | Expected result |
|---|---|---|
| `d.ortiz` | Read the navigation | *Dashboard · Documents · Approvals* - `3 of 5 entries` |
| `d.ortiz` | Documents | Operations documents only (`scope: Operations only · filtered in the query`) |
| `d.ortiz` | Approvals | The Operations queue, every Action `— no approval rights`; status *Own department only · no approval rights* |
| `m.keller` | Approvals | `2 in your reports · 1 outside them`; REQ-4115 reads *Approve · not your report* |
| `m.keller` | Click the REQ-4115 action cell anyway | The service refuses: *You are not allowed to do that…*, status *Approval refused — logged*, and a `DENIED ApproveRequest … REQ-4115` line in the server log |
| `m.keller` | Click *Approve* on REQ-4118 | *REQ-4118 approved · recorded in the server log*; the row reads *Approved* |
| `s.reyes` | Read the navigation, open Admin console | *Dashboard · Documents · Employees · Admin console*; every account with roles, claims and the set they resolve to; Documents shows every department |
| `s.reyes` | Select an account, **Force re-resolve**, then click any area in that account's session | *Permissions re-resolved by an administrator: …* |
| anyone | `dotnet test` | 15 passing tests, including the forged request |

## Lab tasks → where in the code

| Deliverable | Where |
|---|---|
| `Permission` enumeration (the seven actions) | [Permissions.cs](AccessOps/Services/Permissions.cs) |
| Role → permission map in configuration | `Permissions:Roles` in [appsettings.json](AccessOps/appsettings.json) |
| `PermissionService.ResolveFor` - roles, claim grants and denials (denial wins), immutable sorted set | [PermissionService.cs](AccessOps/Services/PermissionService.cs), claims in the `employee_claims` table |
| Set, department and direct reports attached to the `SessionContext` once at sign-in | `BuildContext` in [LoginService.cs](AccessOps/Services/LoginService.cs), [SessionContext.cs](AccessOps/Services/SessionContext.cs) |
| `Authorize.Require` - reads the session, logs user/permission/target, throws `NotAuthorizedException` | [Authorize.cs](AccessOps/Services/Authorize.cs) |
| Guard as the first line of every `DocumentService` and `ApprovalService` method | [DocumentService.cs](AccessOps/Services/DocumentService.cs), [ApprovalService.cs](AccessOps/Services/ApprovalService.cs) |
| Department predicate composed into the document query | `DocumentRepository.List` |
| `GetForApproverAsync(requestId, approverId)` returning null outside the manager's reports | `ApprovalRepository` |
| UI from the same set: admin console only with `ManageUsers`, upload hidden without `UploadDocument`, Approve explained | [ConsolePage.cs](AccessOps/Pages/ConsolePage.cs), [DocumentsView.cs](AccessOps/Pages/DocumentsView.cs), [ApprovalsView.cs](AccessOps/Pages/ApprovalsView.cs) |
| "Force re-resolve" for a named employee | [PermissionRefreshService.cs](AccessOps/Services/PermissionRefreshService.cs), [AdminView.cs](AccessOps/Pages/AdminView.cs) |
| Negative test with a forged request | [AccessOps.Tests/AuthorizationTests.cs](AccessOps.Tests/AuthorizationTests.cs); results in [docs/AuthorizationEvidence.md](docs/AuthorizationEvidence.md) |

## Verified

- `dotnet test`: 15/15 passing (list in [docs/AuthorizationEvidence.md](docs/AuthorizationEvidence.md)).
- Browser pane (`accessops-m3`, 1400x900): `d.ortiz`, `m.keller` and `s.reyes` signed in one after
  the other in the same tab; navigation counts 3/4/4; the out-of-scope click refused with the banner
  and the `DENIED` log line; REQ-4118 approved; the admin console lists 5 accounts; force re-resolve
  rebuilt the set at the session's next action. The generated SQL with the department predicate was
  read from `preview_logs`. No server errors.

## Answers to the review questions

1. **An auditor granted approval rights mid-session.** With this design the new permission takes
   effect at the next sign-in, or at the next action after an administrator presses *Force
   re-resolve* (the `PermissionsResolvedUtc` stamp is compared with the request). Resolving once is
   deliberate: no database round trip per click and no two answers inside one workflow. Making it
   immediate means checking a version stamp on every service call (or pushing a revocation to the
   session) - more load, and a stamp every server can read.
2. **Filtering in the grid's DataSource instead.** The screen would look the same, but every other
   department's rows would have left the database, been held in the process, possibly logged, and -
   as Module 1 showed - any of them bound to the grid would sit in the browser's data model. A paging
   or export change later would expose them; the count would leak their existence today.
3. **Service or repository for the manager check.** The service decides (it knows the caller); the
   repository applies the rule it is handed (`approverId` is an argument, so it stays testable and
   reusable). If the check lived only in a repository that read the session, the repository could no
   longer serve callers with other rights (a nightly job, an admin report). If it lived only in the
   page, every other caller would bypass it. "Both" duplicates the rule and lets the copies drift;
   here the predicate is in the query and the decision - and the refusal log - is in the service.

## Where this sample differs from the lesson

- **Shell layout.** Module 2's video showed a left navigation column, Module 3's video a top strip;
  the sample follows each module's video, so the console's navigation moves to the top here.
- **People and rows.** The video's admin sees five navigation entries including *Approvals*, but its
  permission set (`ViewAllDocuments, ManageUsers, ViewAuditTrail`) has nothing that opens the
  approval queue; the sample builds the navigation strictly from the set, so the admin sees four.
  The video's employee view lists two requests; the sample's department rule shows the employee the
  whole Operations queue (four rows), all marked *— no approval rights*.
- **REQ-4115.** In the video Sam Whitfield (Legal) is in Miriam's queue. Requests carry the department
  they are charged to: REQ-4115 is charged to Operations (so it is in her queue) while Sam reports to
  Legal (so it is *not your report*). The grid's Department column shows the requester's department,
  as in the video.
- **Clicking an out-of-scope row.** The video demonstrates the refusal with a forged call from a test;
  the sample has that test, and additionally lets a click on a row the UI marks *not your report*
  reach the service, which refuses it - the UI marking is a hint, the service is the rule.
- **Upload.** The upload button exists only for `UploadDocument` holders (`r.almeida`), and is
  disabled: the document store arrives in Module 5. The Module 5 starting point describes a naive
  upload already in place; these samples never ship that insecure version.
- **Audit.** Module 3 records refusals and approvals in the server log; the audit table is Module 6.
- **Passkeys (Wisej.NET 4.2).** Not implemented in any module; see Module 2.
