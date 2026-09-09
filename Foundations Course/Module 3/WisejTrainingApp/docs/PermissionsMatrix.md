# Permissions matrix — roles, pages, and where the rule is enforced

The lesson introduces permissions as a *design concept*: no login system, just a role, so the app can say
"a Support Agent can view Settings but not save" before authentication exists. Later this grows into real
authentication and authorization — and the `PermissionService` calls stay where they are.

## 1. Role × page matrix (lesson s11 §5)

| Role | Dashboard | Tickets | Customers | Settings |
|---|---|---|---|---|
| Support Agent | View | Create / close | View | **View only** |
| Manager | View | Create / close | Create / edit | **Save settings** |

In code (`Services/PermissionService.cs`):

| Question | Method | Support Agent | Manager |
|---|---|---|---|
| May this role save settings? | `CanSaveSettings(role)` | false | true |
| May this role add / edit customers? | `CanEditCustomers(role)` | false | true |
| May this role create / close tickets? | `CanManageTickets(role)` | true | true |
| The matrix row as text | `DescribeRole(role)` | shown on the Settings page | shown on the Settings page |

## 2. Where the role lives

- `MainPage` has `private string currentRole = "Support Agent";` — an instance field, so every browser session
  has its own role (never `static`).
- `cboRole` in the header is the stand-in for login. Choosing **Manager** sets `currentRole`, logs
  `Role changed to Manager - permissions re-applied.`, calls `ApplyPermissions()` and re-runs `NavigateTo`
  for the page that is open, so the view is rebuilt for the new role.
- `ApplyPermissions()` in the shell updates `lblUser` ("Signed in as: …") and the nav-button tooltips.
  It deliberately does *not* hide pages: both roles may open all four — what changes is what they can do there.

## 3. How SettingsView enforces it — twice

The shell builds the view for the role, exactly like the lesson: `new SettingsView(currentRole, …)`.

**UI half (on load):**

```csharp
if (permissions.CanSaveSettings(currentRole))
{
    btnSaveSettings.Enabled = true;
    lblPermissionNote.Text = currentRole + " can save settings.";
}
else
{
    btnSaveSettings.Enabled = false;                                 // Support Agent: view-only
    lblPermissionNote.Text = "Settings are view-only for this role.";
}
```

**Server half (on save):** both `btnSaveSettings_Click` and `btnTryServerSave_Click` go through one
`SaveSettings()` that asks again, this time with the role the shell says is current:

```csharp
if (!permissions.CanSaveSettings(shell.CurrentRole))
{
    shell.Log($"Permission denied: {role} cannot save settings.", LogKind.Error);
    AlertBox.Show("Permission denied: …", MessageBoxIcon.Warning, …);
    return;
}
shell.Log($"Settings saved by {role}.");
```

`CustomersView` does the same for **Add Customer** with `CanEditCustomers` — disabled for a Support Agent,
checked again inside `btnAddCustomer_Click`.

## 4. Why the server check matters

A disabled button is a courtesy to the user, not a security boundary. Everything the browser shows can be
changed in DevTools — re-enable the button, fire the click — and the server would receive a perfectly
ordinary `Click` event. The rule has to live where the action happens: in the C# handler, on the server.

That is what the **Try to save anyway (server check)** button demonstrates. It is never disabled and it calls
the same `SaveSettings()` as the real button. As a Support Agent it is refused with
`Permission denied: Support Agent cannot save settings.` in the status bar and the activity list — the decision
was made by `PermissionService`, not by the greyed-out button.

Two consequences for later modules:

- when real authentication arrives, the *identity* changes (a login instead of a ComboBox) but the checks
  (`CanSaveSettings`, `CanEditCustomers`) do not move;
- the check reads `shell.CurrentRole` at the moment of the action, not a value captured when the view was
  built, so a role change after the page opened is honoured.

## Evidence

| Step | Role | What the running app shows |
|---|---|---|
| Open **Settings** | Support Agent | `Save Settings` greyed out, note *Settings are view-only for this role.* (amber), role card lists `Settings     View only` |
| Click **Try to save anyway (server check)** | Support Agent | toast + red status `… - Permission denied: Support Agent cannot save settings. (via Try to save anyway)`, note turns red *Refused by the server: …* |
| Switch `cboRole` to **Manager** | Manager | status `… - Role changed to Manager - permissions re-applied.`, header *Signed in as: Manager*, Settings rebuilt with `Save Settings` enabled and note *Manager can save settings.* |
| Click **Save Settings** | Manager | green status `… - Settings saved by Manager. (company = "ServiceDesk Inc.", default priority = Medium, email notifications = on)`, toast *Settings saved by Manager.* |
| Open **Customers** | Support Agent | `Add Customer` disabled, note *Customers are view-only for this role (Support Agent).*; as Manager it is enabled and adding logs `Customer #7 Litware added by Manager.` |
| Open **Dashboard** | either | every line above in the Recent activity list, in order |
