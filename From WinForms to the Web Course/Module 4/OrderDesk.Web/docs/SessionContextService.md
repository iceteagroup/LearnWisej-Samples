# Session context service — static user state → session-aware services (lab steps 3 + 4)

Lab steps covered: *create `UserSessionContext`* and *move current-user / current-company / current-filter values into
session context*. Deliverable 1 of the storyboard: "static user state → session services — a typed context behind
`Application.Session`".

## Before: five statics, one slot per server

```csharp
// Legacy/AppState.cs — copied as-is from LegacyOrderDesk
public static class AppState
{
    public static string CurrentUser;                 // ✕ one slot for the whole server — the second sign-in overwrites the first
    public static string CurrentCompany = "Acme";     // ✕ per-user state in a static: shared by every session
    public static Customer CurrentCustomer;           // ✕ sam's selection silently replaces kelly's
    public static OrderStatus? CurrentFilter;         // ✕ a filter changed in tab B re-filters tab A on its next refresh
    public static string LastSearch;                  // ✕ per-user workflow state

    // ✓ immutable, user-independent lookup data — a static like this stays safe on the server
    public static readonly string[] Countries = { "US", "UK", "CA", "DE" };
}
```

`LoginForm.okButton_Click` wrote `AppState.CurrentUser`; `OrdersForm` read and wrote `CurrentCustomer` and
`CurrentFilter`. That code is correct on the desktop (one process, one user) and wrong on the server — see
`TwoSessionTest.md` for the live corruption.

## The quick fix, and why it is only a quick fix

```csharp
// ✓ works: Application.Session is kept per browser session by Wisej.NET
Application.Session.CurrentFilter = filter;          // dynamic — no compile-time member, a typo is a new key
var filter = Application.Session.CurrentFilter;      // null when never set; callers must know the storage
```

Redirecting every static to `Application.Session.X` fixes the leak, but the members are untyped, discoverable only by
grepping, scattered over every form, and logout has to remember every key. Good enough to stop the bleeding, not good
enough to ship.

## The pattern: a typed context behind `Application.Session`

Two files in `Services/`:

**`UserSessionContext`** — plain data, no Wisej reference (testable, serializable into a distributed store later):

```csharp
public sealed class UserSessionContext
{
    public string UserName { get; set; }            // was AppState.CurrentUser
    public string DisplayName { get; set; }
    public string Company { get; set; }             // was AppState.CurrentCompany
    public int? CurrentCustomerId { get; set; }     // was AppState.CurrentCustomer (the id, resolved through CustomerService)
    public OrderStatus? CurrentFilter { get; set; } // was AppState.CurrentFilter
    public string LastSearch { get; set; }          // was AppState.LastSearch
    public string Culture { get; set; }             // Application.CurrentCulture at sign-in
    public DateTime? SignedInAt { get; set; }
    public bool IsSignedIn => !string.IsNullOrEmpty(UserName);
}
```

**`SessionContext`** — the one class that knows where the context lives:

```csharp
public static class SessionContext
{
    private const string Key = "UserContext";

    public static UserSessionContext Current
    {
        get
        {
            dynamic session = Application.Session;                      // per browser session
            UserSessionContext context = session.UserContext as UserSessionContext;
            if (context == null)
            {
                context = new UserSessionContext();                     // created lazily: anonymous until sign-in
                session.UserContext = context;
            }
            return context;
        }
    }

    public static bool IsSignedIn => Current.IsSignedIn;

    public static void Reset()                                          // logout / timeout: only THIS session
    {
        Application.Session[Key] = null;
    }
}
```

Callers write `SessionContext.Current.CurrentFilter = filter` exactly as they used to write
`AppState.CurrentFilter = filter` — the edit is mechanical (`MainPage.WriteSignIn`, `WriteFilter`, `WriteCustomer`
show the ✕ and ✓ lines side by side):

```csharp
// MainPage.WriteSignIn — the ✓ branch
var context = SessionContext.Current;
context.UserName = user;
context.DisplayName = char.ToUpperInvariant(user[0]) + user.Substring(1);
context.Company = company;
context.CurrentCustomerId = customerId;
context.CurrentFilter = filter;
context.LastSearch = null;
context.Culture = Application.CurrentCulture?.Name;
context.SignedInAt = DateTime.Now;
```

## What moved, what stayed

| LegacyOrderDesk | OrderDesk.Web | Verdict |
|---|---|---|
| `AppState.CurrentUser` | `SessionContext.Current.UserName` (+ `DisplayName`, `SignedInAt`) | moved |
| `AppState.CurrentCompany` | `SessionContext.Current.Company` | moved |
| `AppState.CurrentCustomer` (a `Customer` object) | `SessionContext.Current.CurrentCustomerId` (an id; the object is looked up through `CustomerService.Find`) | moved, and de-referenced so the session holds no shared mutable object |
| `AppState.CurrentFilter` | `SessionContext.Current.CurrentFilter` | moved |
| `AppState.LastSearch` | `SessionContext.Current.LastSearch` | moved |
| — | `SessionContext.Current.Culture` | new: the browser's negotiated culture, a per-session value the desktop never had |
| `AppState.Countries` | stays a `static readonly` array | kept (immutable, user-independent) |
| `InMemoryOrderRepository.Shared` | stays static | kept (plays the database; one store for all sessions is the point) |

`Legacy/AppState.cs` stays in the project marked ✕ so the console can run the Orders screen on the old store and show
the corruption live; nothing on the migrated path reads it.

## Why the store is invisible to callers

* **One place to change.** `Application.Session` is an in-process bag. If the app is ever load-balanced without sticky
  sessions or needs the context in a Redis/SQL session store, `SessionContext.Current` is the only code that changes;
  `UserSessionContext` is already plain, serializable data.
* **Discoverable members.** IntelliSense lists `CurrentFilter`; a `dynamic` bag lists nothing and silently returns
  `null` for a misspelled key.
* **One place to initialise defaults** (the lazy `new UserSessionContext()`) **and one place to clear on logout**
  (`Reset()`), instead of every form remembering which keys it wrote.
* **Testable services.** A service that takes a `UserSessionContext` can be unit-tested without a Wisej session.
* The samples keep their own tiny user model in the session instead of ASP.NET authentication (`Application.UserIdentity`
  exists; Module 7 adds real server-side auth on top of this context).

## `Reset()` on logout — and only this session

`SessionCleanup.Run` (see `SessionCleanup.md`) ends with `SessionContext.Reset()`. The next `Current` access creates a
fresh, anonymous context. Compare the legacy store: the console's *Sign out* in **Legacy statics** mode has to blank
`AppState.*`, and the trace says why that is a bug in itself —
`⚠ boundary AppState cleared  ✕ the statics are one slot — this sign-out signed out EVERY session on the server`.

## Evidence (in the running app)

* Card B runs the Orders screen on either store: **Legacy statics** (`radioLegacy`) or **UserContext (session)**
  (`radioContext`). The trace announces the switch:
  `← JS→.NET mode  UserContext — the screen reads/writes SessionContext.Current  (✓ one context per browser session)`.
* *Sign in as kelly* on the context store logs
  `• server UserContext.Current  → session xxxxxxxx · kelly · Acme · Northwind Traders · filter Open · culture en-US   ← only this session`,
  versus `• server AppState.CurrentUser (static)  = kelly   ← one slot: EVERY session on the server now reads kelly` on the
  legacy store.
* The **stores** label shows `static AppState (one slot per server)` and `UserContext.Current (session xxxxxxxx)` side by
  side, plus `this page last wrote`, so the difference is visible without a second tab — and provable with one
  (`TwoSessionTest.md`).
