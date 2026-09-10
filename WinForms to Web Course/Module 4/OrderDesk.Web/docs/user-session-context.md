# UserSessionContext — the typed context behind `Application.Session` (lab steps 3–4)

## Why not just `Application.Session["CurrentCustomer"] = customer`?

That is the quick fix the video shows first and it already removes the bug: `Application.Session` is one
dynamic bag per browser session, so two tabs can no longer see each other's customer. But every caller
now knows the storage and the key string, nothing initialises defaults, and nothing clears the values on
logout. The lesson's pattern wraps it in a typed class with one static accessor.

## `Services/UserContext.cs`

```csharp
public sealed class UserContext
{
    public User User { get; set; }
    public Customer CurrentCustomer { get; set; }
    public OrderStatus? ActiveFilter { get; set; }
    public string LastSearch { get; set; }

    public static UserContext Current
    {
        get
        {
            dynamic s = Application.Session;          // one bag per session
            if (s.UserContext == null)
                s.UserContext = new UserContext();     // create on first use
            return (UserContext)s.UserContext;
        }
    }

    public static void Clear()                         // one place to forget everything
    {
        dynamic s = Application.Session;
        s.UserContext = null;
    }
}
```

- `Current` is `static` — but it is a static **accessor**, not a static **slot**: it resolves through
  `Application.Session`, which Wisej scopes to the calling session (also inside `Application.StartTask`,
  which runs in the session's application context — the `Use UserContext ✓` button reads it from a
  background task and still gets kelly's object).
- Reading a missing member of the dynamic bag returns `null`, so first use creates the object.
- `Handle` (`UserContext#xxxx`) is a lab prop: it lets the trace show that two sessions hold two
  different objects.
- `ActiveFilter` became `OrderStatus?` — the string in `AppState.ActiveFilter` was parsed everywhere.

## Callers never see the storage

| LegacyOrderDesk (static) | OrderDesk.Web (session) |
|---|---|
| `AppState.CurrentUser = login.User;` | `UserContext.Current.User = user;` |
| `AppState.CurrentCustomer = (Customer)cmbCustomer.SelectedItem;` | `UserContext.Current.CurrentCustomer = customer;` |
| `AppState.ActiveFilter = "Open";` | `UserContext.Current.ActiveFilter = OrderStatus.Open;` |
| `if (AppState.CurrentUser == null) …` | `if (!UserContext.Current.IsSignedIn) …` |
| *(nothing on exit — the process died)* | `UserContext.Clear()` on Sign out, `ApplicationExit`, `SessionTimeout` |

In this sample the page writes through `Services/StateStores.cs` (`IStateStore` with a
`LegacyStaticStore` and a `SessionContextStore`) so the **State store** toggle can run the same screen
against either storage. A real migration deletes `AppState` and the legacy store once the last caller
is gone.

## Sign-in became a card, not a modal

`LoginForm.ShowDialog()` before `Application.Run` has no equivalent: `Program.Main` runs once per session
and must return. The Sign-in card writes `UserContext.Current` and the rest of the page reads it; on a
page refresh the session (and the context) survives, which the trace shows as
`✓ ok UserContext.Current  UserContext#xxxx already holds kelly — the session survived the page reload`.

## Where else per-user state may live

- `Application.Session` / a typed context — this module; the right place for user, company,
  selection, filters, workflow position.
- A database row — when the value must roam, be audited or be centrally managed (settings, see
  `session-cleanup.md` and `static-state-audit.md`).
- Browser storage (`localStorage` via `Application.Eval`) — device-only UI preferences (a collapsed
  panel, a preferred column width); never business settings, never anything the server relies on.

## Evidence (what the running app shows)

- `Sign in` as kelly: `✓ ok UserContext.Current  UserContext#xxxx ← kelly · Northwind Traders · Open   ✓ Application.Session of <session>`.
- Toggle **Session context (UserContext) ✓**: the read-back shows `READ FROM UserContext (Application.Session)`
  and the location line `UserContext#xxxx · one object per SESSION`; the trace logs
  `← JS→.NET State store toggle  active store = UserContext (Application.Session)`.
- Changing the **Customer** combo in session mode: `✓ ok UserContext.Current.CurrentCustomer =  Fabrikam Inc   ✓ UserContext#xxxx only`.
- `Use UserContext ✓`: `✓ ok session A (kelly) reads  UserContext#aaaa · CurrentCustomer = Northwind Traders · ActiveFilter = Open   ✓ isolated — UserContext#aaaa ≠ UserContext#bbbb`
  and the green banner "kelly keeps Northwind Traders / Open — each session reads its own UserContext".
- `Sign out`: `✓ ok UserContext.Clear()  Application.Session.UserContext = null · session <id> forgot kelly — other sessions untouched`.
