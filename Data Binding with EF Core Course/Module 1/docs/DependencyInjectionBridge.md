# The dependency injection bridge · Microsoft DI inside Wisej.NET

Deliverable 3 of the Module 1 lab: Microsoft's `IServiceProvider` exposed to Wisej.NET, and a query
service resolved from a Page.

## One line, one direction

`Startup.cs`, step 4, right after `builder.Build()`:

```csharp
Wisej.Web.Application.Services.AddService<IServiceProvider>(app.Services);
```

Wisej.NET has its own service registry (`Application.Services`). Registering Microsoft's provider as the
`IServiceProvider` service tells Wisej.NET where to look when a top-level container (Page, Form, Desktop)
carries `[Inject]` properties. From this line on, **every application service is registered with Microsoft
DI** — `builder.Services.AddTransient<TicketQueryService>()` — and nothing else goes into
`Application.Services`. Mixing the two containers is the mistake the lesson warns about: a service
registered in Wisej.NET's registry cannot see the factory registered in Microsoft's.

## Resolving on the page

```csharp
public partial class TicketBrowserPage : Page
{
    [Inject]
    private TicketQueryService TicketQueries { get; set; }
    …
}
```

Wisej.NET injects the property while the Page is constructed (`Wisej.Web.Page..ctor` →
`ServiceProvider.Inject`). The page never constructs a context, never sees the factory; it calls
`TicketQueries.CountTicketsAsync()` and the service does the unit of work.

`Application.Services.GetService<TicketQueryService>()` is the explicit alternative for objects that are not
top-level containers (a helper class, a dialog created in code); `Application.Services.Inject(target)`
injects into an existing object.

## Verified: lifetimes across the bridge

This was measured, not assumed. The first build registered the services as `Scoped` (the course text) and
the page failed to construct in Development:

```
Cannot resolve scoped service 'SupportDesk.Services.TicketQueryService' from root provider.
   at Microsoft.Extensions.DependencyInjection.ServiceProvider.GetService(Type serviceType)
   at Wisej.Services.ServiceProvider.GetService(Type serviceType, Stack`1 controlStack)
   at Wisej.Services.ServiceProvider.Inject(Object target, Stack`1 controlStack)
   at Wisej.Web.Page..ctor()
```

Wisej.NET asks the **root** Microsoft provider, and ASP.NET Core validates scopes in Development. There is
no per-session Microsoft scope, so:

| Lifetime | Works through `[Inject]`? | Use it for |
|---|---|---|
| Transient | yes | stateless services that hold only the factory — `TicketQueryService`, validators, command services |
| Singleton | yes | the factory itself, options, immutable reference data |
| Scoped | **no** in Development (root-provider validation); silently root-scoped in Production | nothing — a "scope" would be the whole application |

Transient is the right answer anyway: the services own no state, so a new instance per page costs one
allocation and can never leak anything between sessions.

## What the bridge does not change

- The Wisej.NET session model: one `TicketBrowserPage` per browser session, created by `Program.Main`.
- Thread affinity: the page can be reached from any thread after an `await`; the injected services are
  thread-safe because they are stateless.
- Ownership of the `DbContext`: still one per operation, created and disposed inside the service.

## Evidence

- Trace line at page load: `• session page created for session c85f34bf · TicketQueryService through
  [Inject] → resolved from Microsoft DI`.
- With the bridge line commented out, the same property is `null` and the page prints
  `NULL — the IServiceProvider bridge is missing` (the code guards for it on purpose).
- `Startup.cs` contains exactly one `Application.Services.AddService` call, and every other registration is
  `builder.Services.Add…`.
