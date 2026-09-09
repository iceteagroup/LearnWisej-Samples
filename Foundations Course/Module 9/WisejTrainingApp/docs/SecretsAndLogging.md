# Secrets and logging — unsafe vs safer, and how this project does it

## Unsafe practice → safer practice (lesson s41 §3)

| Unsafe practice | Safer practice | In this project |
|---|---|---|
| Hard-code a license key in a page or button event. | Use secure configuration or environment variables. | `Services/SecureConfig.cs` reads `WISEJ_LICENSE_KEY`, falls back to `Web.config` `Wisej.LicenseKey`, and hands out only a masked status. No handler contains a key. |
| Commit connection strings to Git. | Keep secrets outside source control; use deployment secrets. | `TRAINING_CONNECTION_STRING` is an environment variable; the `Web.config` in the repo has an empty `Wisej.LicenseKey` and no connection setting at all. |
| Display raw exception details to users. | Log details server-side and show a safe message. | The simulated `IOException` becomes `lblStatus` = *Packaging failed. Please contact the release manager.*; the exception type and message go to the log (redacted) and the stack trace only to `Trace`. |
| Share production credentials in screenshots. | Redact or avoid showing secrets in training materials. | The "Secrets" card shows `configured (masked ••••1234)` / `MISSING — set WISEJ_LICENSE_KEY`; a screenshot of it leaks nothing. |

## SecureConfig — read once, mask immediately

```csharp
public SecretStatus Describe(string environmentVariable, string appSettingKey)
{
    string value = Environment.GetEnvironmentVariable(environmentVariable);
    if (string.IsNullOrWhiteSpace(value))
        value = ReadAppSetting(appSettingKey);          // XmlDocument over Web.config — no System.Configuration
    if (string.IsNullOrWhiteSpace(value))
        return SecretStatus.Missing(environmentVariable);
    return SecretStatus.Configured(Mask(value), source); // "••••" + last 4 characters
}
```

- The raw `value` is a local variable that never leaves the method. `SecretStatus` has no property for it.
- `Mask("demo-key-1234")` → `••••1234`; anything four characters or shorter becomes `••••`.
- To see the *configured* branch, start the app with the variables set (values are fake):

  ```powershell
  $env:WISEJ_LICENSE_KEY = "training-demo-1234"
  $env:TRAINING_CONNECTION_STRING = "Server=localhost;Database=Training;User Id=lab;Password=not-a-real-one"
  dotnet run -f net10.0 --urls http://localhost:5089
  ```

  The card then reads `License key: configured (masked ••••1234)` and the log says which source answered.

## SafeLogger — timestamps, and nothing secret

```csharp
public string Log(string message)
{
    string line = $"{DateTime.Now:HH:mm:ss}  {Redact(message)}";
    _entries.Add(line);
    Trace.WriteLine("[ReleaseReview] " + line);   // the "server-side log"
    return line;                                    // the window shows this in lstLog
}
```

`Redact` masks `name=value` (or `name: value`) pairs whose name looks like a secret — `key`, `apikey`, `licensekey`,
`connectionstring`, `password`, `pwd`, `secret`, `token` — replacing the value with `•••[redacted]`. The window's
`AddLog` is the only way into the list, so every line is redacted; the **What NOT to do** button proves it by
logging a fake `licensekey=DEMO-…` line that arrives masked.

Lesson s42 §2 checklist, mapped to the log:

| Rule | Log line you will see |
|---|---|
| Log the start and finish of important workflows | `ReleaseReviewService.TryCreatePackage(role=…, env=…, target=…, version=…)` → `Deployment package reviewed.` |
| Log validation failures and deployment-review failures | `Review attempt → required checks incomplete (4 / 9)`, `Server refused (permission): Review is restricted.` |
| Log exceptions server-side with enough detail for a developer | `Packaging failed: IOException — Access to path '…' is denied (stack trace in Trace, not on screen)` |
| Don't log passwords, license keys or private user data | `SecureConfig → WISEJ_LICENSE_KEY: configured (masked ••••1234)` — never the value |
| Add timestamps so the order of events is clear | every line starts with `HH:mm:ss` |

## Authentication vs authorization (lesson s42 §1)

- **Authentication** — *who is the user?* `currentUser` (`Models/UserAccount.cs`: Name, Role) stands in for the
  identity a sign-in would provide. It is an instance field of the window, so each session has its own.
- **Authorization** — *what may this user do?* Two layers, on purpose:

  1. **UI gate** (`ApplyPermission()` in `ReleaseReviewWindow.cs`, the lesson snippet):
     ```csharp
     bool canReviewDeployment = currentUser.Role == "Admin" || currentUser.Role == "Team Lead";
     btnReviewPackage.Enabled = canReviewDeployment;
     lblPermission.Text = canReviewDeployment ? "Deployment review available." : "Review is restricted.";
     ```
  2. **Server check** (`ReleaseReviewService.TryCreatePackage`): `IsAllowed(req.Role)` is evaluated again from the
     request, and the service never looks at a control. *Never rely only on hiding a button.*

  **Try review as Support Agent (server check)** calls the service directly with `Role = "Support Agent"` whatever
  `cboRole` says: the answer is `Review is restricted.` with reason `permission`. A disabled button is a courtesy
  to the user; the service is the protection.

## Evidence

| Action | Screen | Log |
|---|---|---|
| App starts without the variables | Secrets card: `MISSING — set WISEJ_LICENSE_KEY` / `MISSING — set TRAINING_CONNECTION_STRING` in red | `SecureConfig → WISEJ_LICENSE_KEY: MISSING — set WISEJ_LICENSE_KEY` |
| App starts with the variables set | `configured (masked ••••1234)` in green | `… configured (masked ••••1234) (source: environment variable WISEJ_LICENSE_KEY)` |
| **What NOT to do** | status green *Unsafe pattern explained in the log — no secret was shown.* | three explanation lines, then `Redaction demo (fake value): licensekey=•••[redacted] → …` |
| **Try review as Support Agent** | status red *Review is restricted.* | `Server refused (permission): Review is restricted. — the action is protected server-side …` |
| **Simulate packaging error** + Create Review Package (9/9, Team Lead) | status red *Packaging failed. Please contact the release manager.* — no path, no stack trace | `Packaging failed: IOException — Access to path 'D:\deploy\ServiceDesk.zip' is denied (stack trace in Trace, not on screen)` |
