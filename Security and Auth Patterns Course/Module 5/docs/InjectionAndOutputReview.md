# Parameterised query and output encoding review - Module 5

Walked on 2026-11-06 over every `.cs` file in `AccessOps/` plus `Default.html`. Method: read each
`CommandText`, every place a user-supplied string reaches the browser, and every interop call;
then a text search for the risky patterns (`AllowHtml = true`, `MessageBox.Show`,
`Application.Eval`, `CommandText` built with `+` from a variable) - **no hits**.

## 1. SQL - every command is parameterised

| File | Command | User-supplied values | How they travel |
|---|---|---|---|
| `Infrastructure/AccessOpsDatabase.cs` | seed `INSERT`s, `CREATE TABLE` | none (seed files) | `$parameters` |
| `Services/EmployeeRepository.cs` | credential lookup, `Find`, `DirectReports` | user name typed at sign-in | `$user`, `$id` |
| `Services/LoginAuditService.cs` | `INSERT INTO login_audit`, `Latest` | user name tried (truncated to 64) | `$user`, `$n` |
| `Services/PermissionService.cs` | claims by employee | none | `$id` |
| `Services/DocumentService.cs` (`DocumentRepository`) | `List`, `Search`, `FindById`, `Insert`, seed files | department (from the session), search term, document id from a grid row, upload display name | `$department`, `$term`, `$id`, `$title` ... The only concatenation is of **constant** SQL fragments (`" WHERE department = $department"`), chosen by whether a department applies |
| `Services/ApprovalService.cs` (`ApprovalRepository`) | queue, `GetForApproverAsync`, `SaveAsync` | request id from a grid row | `$id`, `$approver`, `$department` |
| `Services/ExternalIdentityMapper.cs` | external login lookup, provisioning | issuer, subject, names, email from the validated ID token | parameters |
| `Services/AdminService.cs` | account list | none | - |

Proof by test: `A_hostile_search_term_is_data_not_sql` searches for
`' OR 1=1; DROP TABLE documents; --`, gets no rows, and the table is still there and still scoped.

## 2. Output - every user string is encoded

Wisej.NET encodes text sent to the browser unless `AllowHtml` is on or the text goes through
`MessageBox.Show` / `AlertBox` with their `allowHtml` default of **true** (checked by reflection on
4.1.7: `MessageBox.Show(..., allowHtml = true, ...)`).

| Where a user string is shown | Control | Encoded? |
|---|---|---|
| Upload display name in the notice strip | `Label` (`MessageStrip`) | yes - `AllowHtml` is never set anywhere in the project |
| Document titles in the grid | `DataGridView` cells | yes - no column sets `AllowHtml` |
| Document title in the share-link dialog | `Label` | yes |
| User names, display names, departments | `Label`, grid cells | yes |
| Toast after an upload | `Application.Call("showToast", text)` → `textContent` in Default.html | yes - passed as an argument, set as text |
| `MessageBox.Show` | not used anywhere | - |
| `Application.Eval` | not used anywhere (Module 2's sign-out hand-off used it; it was replaced by `Application.Call`) | - |

Browser check: a file named `<img src=x onerror="window.__xss=1">.pdf` was uploaded through the
real Upload control. The notice, the grid row and the toast all showed the name as characters;
`window.__xss` stayed 0 and no `img[src=x]` element was created.

## 3. Interop

| Call | Argument | Notes |
|---|---|---|
| `Application.Call("showToast", "Filed: " + name)` | the upload display name | a function that already exists on the page; the value is data |
| `Application.Call("accessOpsEnded", callback, word)` | one of six fixed words | Default.html accepts only those words |

## 4. Files and paths

The client's file name never becomes a path: `UploadGate.DisplayName` strips directories and
control characters (`..\..\etc/passwd.pdf` → `passwd.pdf`, unit test), and the stored name is 32
random hex characters checked again by `DocumentStore` before any file operation.
