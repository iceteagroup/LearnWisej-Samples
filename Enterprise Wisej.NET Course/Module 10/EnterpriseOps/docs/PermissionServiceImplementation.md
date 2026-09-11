# Permission service implementation — EnterpriseOps

Deliverable 3 of Module 10. One service answers "may this caller do this?", and it is the answer to the review
question **where is authorization enforced?**

## The contracts

`Security/SecurityArchitecturePatterns.cs` — the lesson's listing, as the shape the sample implements:

```csharp
public enum Permission
{
    ViewWorkOrders, EditWorkOrders, ApproveWorkOrders,
    ExportData, AdminDiagnostics,
    ViewAuditLog, ApproveExport      // added by the lab
}

public interface IPermissionService
{
    void Demand(CommandContext context, Permission permission, string resourceTenantId = null);
    bool Has(CommandContext context, Permission permission, string resourceTenantId = null);
}

public interface IRolePermissionStore
{
    bool UserHasPermission(string userId, string tenantId, Permission permission);
}

public interface ITenantGuard
{
    void DemandTenant(CommandContext context, string resourceTenantId);
}
```

(The lesson writes the optional parameter as `string?`; the project compiles with nullable annotations disabled,
so it is `string resourceTenantId = null`. Same signature, same behaviour.)

## The implementation

`Security/PermissionService.cs`. The order of the two checks is the design:

1. **Tenant first.** When a resource tenant is given, `TenantGuard.DemandTenant` compares it with the tenant on
   the context — which came from the `tid` claim, not from the UI. A cross-tenant attempt never reaches the role
   store, so a user who holds a role in tenant A gets nothing in tenant B.
2. **Then roles.** `RolePermissionStore.UserHasPermission(userId, tenantId, permission)` resolves membership in
   that tenant and consults the matrix.
3. **Then it throws**, naming the missing permission: `Missing permission: ExportData`.

### Demand vs Has

| | `Demand` | `Has` |
|---|---|---|
| Called by | services, before executing | screens, to adapt themselves |
| On refusal | throws `UnauthorizedAccessException` | returns `false` |
| Audit entry | **always**, granted or denied | **never** |

The lesson writes `Has` as `try { Demand(…); return true; } catch { return false; }`. The sample keeps the
behaviour and routes both through a private `Evaluate(…, bool probe)`, because a screen asking "should I show
this button?" has attempted nothing: if probes wrote entries, a real denial would drown in them.

### Why the audit lives here

The audit entry for a permission decision is written where the decision is taken. A service cannot succeed
without one, and it cannot record a denial it never noticed. The command's own entry (what actually changed) is
written by the service, next to the change, in the same operation — see `WorkOrderService.ApproveAsync`.

## How services use it

```csharp
public async Task<ExportResult> RequestExportAsync(CommandContext context)
{
    _permissions.Demand(context, Permission.ExportData, context.TenantId);
    …
}
```

Three properties worth noting:

- the method takes a `CommandContext`, which can only come from `SessionContext.BeginCommand()`, which throws
  when nobody is signed in — an unauthenticated caller cannot even construct the argument;
- it takes **no** flag from the screen about what the user is allowed to do;
- the check runs before a single row is read, so a refusal leaks nothing, not even a row count.

## Failure paths the services handle

| Path | Where it is refused | Audit row |
|---|---|---|
| Technician clicks a wrongly-enabled **Export data** | `PermissionService.Demand` → role store | `ExportData · DENIED · missing permission for roles Technician` |
| Fabrikam user approves a contoso work order | `TenantGuard.DemandTenant`, before the role store | `ApproveWorkOrders · DENIED · cross-tenant: session 'fabrikam' requested 'contoso'` |
| Admin approves their own pending export | `ExportService`, after the permission passed | `ApproveExport · DENIED · separation of duties` |
| Account whose group maps to no role | every `Demand` | `SignIn · DENIED · claims mapped to no role` |

## Which line proves the caller is allowed to be here?

For every service method in this sample, the first non-trivial statement. `LoadQueueAsync` starts with
`Demand(ViewWorkOrders)`; `ApproveAsync` with `Demand(ApproveWorkOrders, order.TenantId)`; `RequestExportAsync`
with `Demand(ExportData, context.TenantId)`. `AuditQueryService.QueryAsync` is the one deliberate exception: it
uses `Has(ViewAuditLog)` to choose the **scope** of the query rather than to refuse it, and the tenant filter in
`AuditLog.Snapshot` is unconditional.

## Evidence in the running app

- The server log (`ActivityTrace` → `System.Diagnostics.Trace`) shows the layers in order for every click:
  `Service: ExportService.RequestExportAsync — Demand(ExportData…)`, `Security: TenantGuard ok`,
  `Security: PermissionService.Demand(ExportData) DENIED …`.
- **Export data** is offered to every signed-in user. A Technician's click reaches the same service and is
  refused, with the denial audited: an enabled button cannot bypass a service rule.
- Nothing in `UI/AuditLogPage.cs` decides a permission. The only security-shaped lines in the file are the two
  `Has` calls in `ApplyPermissionsToUi`, and they only hide buttons.
