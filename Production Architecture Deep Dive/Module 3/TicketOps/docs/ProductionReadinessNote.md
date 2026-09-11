# Production-readiness note — responsive Ticket Workspace

*Module 3 deliverable · lab step 9 · TicketOps Console*

## What is production-ready

| Check | Result | Evidence |
|---|---|---|
| Each container runs one layout engine, chosen by intent | ✔ | `docs/ResponsiveLayoutNotes.md` table; `TicketWorkspace.Designer.cs` has no `Location` inside a resizable region |
| Profile changes toggle existing panels; nothing is rebuilt | ✔ | `ApplyResponsiveProfile` → `ShowAllPanels` / `CollapseActivityToTab` / `ShowSingleTaskView` only set `Visible`, `Dock`, `Width`, `Padding`, fill weights and `Parent` |
| The handler is thin | ✔ | one `switch`, three named methods, a `default` that falls back to the desktop layout |
| Profiles are defined in one file and documented | ✔ | `ClientProfiles.json` (Phone ≤ 600, Tablet 601–1024, Desktop ≥ 1025), copied to `/bin` for both targets |
| Repeated UI is a UserControl with a small public surface | ✔ | `Controls/SearchBar`: `Query`, `Placeholder`, `ButtonText`, `SearchRequested`, `Clear()`; two instances (`searchTickets`, `searchActivity`); the inner TextBox/Button are private |
| The service knows nothing about profiles or controls | ✔ | `Services/TicketService.cs` takes no Wisej.NET type; desktop, tablet and phone call the same `SearchAsync`, `SaveAsync`, `CloseAsync`, `GetActivityAsync` |
| Every path is visible without leaking internals | ✔ | success (save, search, chips), validation (a 1-character search, a blank title), rule (close without hours), unexpected failure (details to `ILog`, `Strings.ActionFailed` on screen) |
| No per-user state in statics | ✔ | `AppComposition` per session; the only statics are constants, `SeedData`, `OperationResult.Ok/Fail` and `StatusBanner.ColorFor` |
| Session-level events are released | ✔ | `Application.ResponsiveProfileChanged` unsubscribed in `Dispose(bool)` of `TicketWorkspace` |
| Designer-friendly | ✔ | every Page/UserControl has a parameterless constructor and a `.Designer.cs` with the whole layout in `InitializeComponent()`; the workspace receives its services through `Attach(...)` or its real constructor |

## Before shipping

1. **Confirm the profile events at runtime**: resize across 600 / 1024 px and check that
   `Application.ResponsiveProfileChanged` fires with the names from `ClientProfiles.json` and not the
   framework's `Small Desktop`; if `Small Desktop` appears, add an entry with that name to the JSON so the
   override wins, or map it to `Tablet` in the switch.
2. **Real devices**: test a phone in portrait and landscape (the built-in `Phone (Landscape)` profile is
   overridden only if the JSON defines it) and a tablet with an external keyboard.
3. **Minimum sizes**: `MinimumSize` is set on the cards so the flex panels stop shrinking them; verify no
   horizontal scrollbar appears at 320 px.
4. **Sensitive state**: nothing in the workspace is secret. When a panel holds one, clear it in
   `ApplyResponsiveProfile` before hiding — hidden is not cleared.
5. **Accessibility**: the navigation items are Labels with `Cursor = Hand`; give them keyboard focus
   (`TabStop`) or turn them into Buttons before a11y review.
6. **Designer profile properties**: Visual Studio's Wisej Designer can store per-profile `Visible` / `Size` /
   `Dock` values in `Control.ResponsiveProfiles`; this sample expresses the same values in code so the
   decision is readable in one method. Pick one approach per project and keep it.
7. **Chips**: the six filter chips are built in `BuildChips()` from `TicketFilter.Chips`; if the set becomes
   user-configurable, move it behind `ITicketService`.

## Evidence

Resize the browser across 1024 px and 600 px: the status bar reads `Active profile: Tablet` / `Phone` and
the layout changes as described in `ProfileNotes.md`. Type one character in the ticket SearchBar and press
Enter: orange banner **Type at least 2 characters to search.**, the grid is unchanged. Select #1002 (no
hours logged) and press **Close**: orange banner **Log hours before closing.**
