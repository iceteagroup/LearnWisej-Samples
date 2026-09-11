# Static-state audit — LegacyOrderDesk (lab steps 1 + 2)

Lab steps covered: *search for static fields and singleton state* and *classify each static as immutable shared data
or per-user state*. On the desktop every user ran their own `LegacyOrderDesk.exe`, so a `static` field was private to
that user. On the server every browser session shares one process, so the same field is one slot for everybody.

## The rule applied to every row

> **Would the value differ if two users opened the app at the same time?**
> Yes → it is not a safe global; it moves to the session (workflow state), to a server-side profile store (settings
> that must follow the user) or to browser storage (device-bound UI preferences).
> No, and it is never written after startup → it may stay static.

Static **methods** without static fields hold no state and need no verdict; only static **data** (and the HKCU-backed
settings, which are process-global state in disguise once the process is a server) is audited.

## How the list was found

```bash
grep -rn --include=*.cs --exclude=*.Designer.cs "static" LegacyOrderDesk
```

Designer files are excluded (their statics are `InitializeComponent` plumbing). `Settings/RegistrySettings.cs` was
added by hand: `Registry.CurrentUser` is not a `static` keyword hit, but on the server it resolves to the service
account's hive — one value for every visitor, which is exactly the property the audit is looking for.

## The audit

| Member | Declared in | Kind | Verdict | Why | Replacement |
|---|---|---|---|---|---|
| `AppState.CurrentUser` | `LegacyOrderDesk/AppState.cs` | per-user state | **Move to session** | Set by `LoginForm`; two users signed in at the same time need two different values. One static slot means the second sign-in overwrites the first for everyone. | `UserSessionContext.UserName` via `SessionContext.Current` (`Application.Session`). |
| `AppState.CurrentCompany` | `LegacyOrderDesk/AppState.cs` | per-user state | **Move to session** | kelly works for Acme, sam for Globex — differs per user, so it cannot be process-wide. | `UserSessionContext.Company`. |
| `AppState.CurrentCustomer` | `LegacyOrderDesk/AppState.cs` | per-user state | **Move to session** | Workflow state of one user's Orders screen. sam selecting Fabrikam silently replaces kelly's Northwind; passes every single-user test. | `UserSessionContext.CurrentCustomerId` (store the id, resolve through `CustomerService`). |
| `AppState.CurrentFilter` | `LegacyOrderDesk/AppState.cs` | per-user state | **Move to session** | A view filter is per screen, per user. Changing it in tab B re-filters tab A on its next refresh. | `UserSessionContext.CurrentFilter` (`OrderStatus?`). |
| `AppState.LastSearch` | `LegacyOrderDesk/AppState.cs` | per-user state | **Move to session** | Per-user workflow state; also a small privacy leak if another session can read what a user searched for. | `UserSessionContext.LastSearch`. |
| `AppState.Countries` | `LegacyOrderDesk/AppState.cs` | immutable lookup | **Keep static** | `readonly`, never written after startup, identical for every user. Two users opening the app see the same list — the rule says it may stay global. | None. (Would become a read-only collection if anything ever mutated it.) |
| `SampleData.Customers` | `LegacyOrderDesk/Domain/Services/SampleData.cs` | immutable lookup | **Keep static** | Reference data, user-independent, read-only in practice: `CustomerService` hands out clones so no caller can mutate the shared array. | None — but keep the clone-on-read discipline; a shared mutable object would be the same bug as `CurrentCustomer`. |
| `InMemoryOrderRepository.Shared` | `LegacyOrderDesk/Domain/Services/OrderService.cs` | per-process cache | **Keep static** | The lab's stand-in for the database: ONE store for every session is the intended semantics, and every access is under a lock, so it is thread-safe and user-independent. | None. It plays the database; the real system replaces it with the database. |
| `RegistrySettings.GridDensity` | `LegacyOrderDesk/Settings/RegistrySettings.cs` | registry (HKCU) | **Move to browser storage** | HKCU on the server is the service account's hive, shared by every visitor. Density is a pure UI preference that may differ per device. | `localStorage` via `Application.Eval`/`EvalAsync`; a per-user server profile may keep a roaming copy. |
| `RegistrySettings.ExportFolder` | `LegacyOrderDesk/Settings/RegistrySettings.cs` | registry (HKCU) | **Move to profile store** | `"C:\Orders"` is a path on the user's PC; the server cannot write there. Per user, must follow the user to any device, business-relevant → server-owned. | A per-user server profile (e.g. `App_Data/profiles/<user>.json`, or a database row) holding a folder name under `App_Data`; files reach the user via `Application.Download`. |
| `RegistrySettings.WindowWidth / WindowHeight` | `LegacyOrderDesk/Settings/RegistrySettings.cs` | registry (HKCU) | **Remove** | The browser window belongs to the user; the server neither knows nor sets its size. | Remove. Responsive layout instead (Module 7). |

## Counts

| Verdict | Rows | Members |
|---|---|---|
| Keep static | 3 | `Countries`, `SampleData.Customers`, `InMemoryOrderRepository.Shared` |
| Move to session | 5 | `CurrentUser`, `CurrentCompany`, `CurrentCustomer`, `CurrentFilter`, `LastSearch` |
| Move to profile store | 1 | `ExportFolder` |
| Move to browser storage | 1 | `GridDensity` |
| Remove | 1 | `WindowWidth / WindowHeight` |
| **Total** | **11** | |

Three of eleven statics may stay — the lesson's warning in numbers: do not move every static into the session, only
the per-user ones. The five that move are exactly the five fields of `AppState` that `LoginForm` and `OrdersForm`
write; the running app reads them from `SessionContext.Current` (see `SessionContextService.md` and
`TwoSessionTest.md`).

Static classes that only hold methods (`InvoiceDocument`, `ExcelExport`, `InvoicePrinter`) keep no state, so nothing is shared between sessions; they are out of scope of the audit and are not counted. Their problem, where they have one, is the desktop boundary (Module 6), not shared state.
