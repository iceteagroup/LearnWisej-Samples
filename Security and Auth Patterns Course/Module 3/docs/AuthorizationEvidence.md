# Service-level authorization - evidence

## The negative tests (`dotnet test` from `Module 3/`)

Run on 2026-11-06 against Wisej-4 4.1.7 / .NET 10, 15 tests, all green:

```
Passed AuthorizationTests.Forged_approve_from_a_non_manager_is_refused_and_nothing_changes
Passed AuthorizationTests.A_manager_cannot_approve_a_request_outside_their_reports
Passed AuthorizationTests.A_manager_can_approve_a_request_from_a_direct_report
Passed AuthorizationTests.An_unknown_request_id_is_refused_exactly_like_an_out_of_scope_one
Passed AuthorizationTests.The_queue_marks_what_the_manager_may_approve
Passed AuthorizationTests.An_employee_sees_only_their_department_and_the_filter_is_in_the_sql
Passed AuthorizationTests.An_administrator_with_ViewAllDocuments_sees_every_department
Passed AuthorizationTests.An_administrator_without_SubmitRequest_or_ApproveRequest_cannot_list_approvals
Passed AuthorizationTests.Without_a_signed_in_session_every_service_refuses
Passed PermissionServiceTests.Roles_expand_through_the_configured_map
Passed PermissionServiceTests.A_denial_wins_over_a_role_and_over_a_grant
Passed PermissionServiceTests.A_grant_from_the_identity_store_adds_a_permission
Passed PermissionServiceTests.The_resolved_set_is_sorted_and_has_no_way_to_add
Passed ReResolveTests.A_granted_permission_takes_effect_at_the_next_action_after_force_re_resolve
Passed ReResolveTests.Only_a_ManageUsers_holder_can_ask_for_a_re_resolve
Total tests: 15
```

The forged-request test builds a `SessionContext` for `d.ortiz` (no `ApproveRequest`), calls
`ApprovalService.ApproveAsync(4115, "ok")` directly - no screen, no button - and asserts three
things: `NotAuthorizedException`, the request still `Pending` **in the database** (the assertion
that rules out a guard that throws after the write), and a `DENIED ApproveRequest` log line naming
employee 3081 and `REQ-4115`.

## The department filter is in the query

Server log from the browser check, signed in as `m.keller`:

```
info: AccessOps.Services.DocumentRepository[0]
      Document query: SELECT id, title, department, classification, size_bytes, updated_utc FROM documents
      WHERE department = $department ORDER BY updated_utc DESC, id [department=Operations]
```

The value is a parameter (`$department`), taken from the `SessionContext` by `DocumentService` and
handed to the repository as an argument. An administrator (`ViewAllDocuments`) runs the same query
without the `WHERE`.

## The refusal, seen from the server

Browser check, `m.keller` clicking the action cell of REQ-4115 (Sam Whitfield, Legal - not her
report). The screen showed one sentence; the log has the detail:

```
warn: AccessOps.Services.Authorize[0]
      DENIED ApproveRequest for employee 3050 (m.keller, Operations) on REQ-4115:
      request is outside the approver's reports (or does not exist)
info: AccessOps.Services.ApprovalService[0]
      Request 4118 approved by employee 3050
```

## The three permission sets (admin console, browser check)

| Account | Roles | Claims | Resolves to |
|---|---|---|---|
| d.ortiz | Employee | - | SubmitRequest, ViewDepartmentDocuments |
| m.keller | Employee, Manager | - | ApproveRequest, SubmitRequest, ViewDepartmentDocuments |
| s.reyes | Administrator | - | ManageUsers, ViewAllDocuments, ViewAuditTrail |
| r.almeida | Employee | grant UploadDocument | SubmitRequest, UploadDocument, ViewDepartmentDocuments |
| k.osei | Employee, Manager | deny ManageUsers | ApproveRequest, SubmitRequest, ViewDepartmentDocuments |
