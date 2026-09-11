# Deliverable 1 — the `StatusTimeline` UserControl

`EnterpriseOps.Controls.StatusTimeline` — the reusable "what happened to this record, in order" component
of the EnterpriseOps application framework.

Files: `Controls/StatusTimeline.cs`, `Controls/StatusTimeline.Designer.cs`, `Controls/TimelineItem.cs`.

## Why a UserControl and not an inherited control

Wisej.NET offers two ways to build a reusable component out of existing controls.

| | Compose (`UserControl`) | Inherit (subclass a built-in control) |
|---|---|---|
| When | the component is a **new thing made of several parts** | the component is a **specialised version of one control** |
| Example here | `StatusTimeline`: a header, a scrolling list of rows, and a dot / time / status / message layout per row | a grid that always shows the tenant's standard columns |
| Cost | you own the layout and the theming of the parts | you inherit every feature of the base for free |

A timeline is not a specialised `ListBox` — it is a header plus rows plus a per-row layout — so it composes.
Mixing the two (inheriting a `Panel` and then hiding half of its surface) produces components that are hard
to theme and hard to explain, which is why the sample does not do it.

## The public API

Everything a screen developer sees in the Properties window and in IntelliSense, and nothing else.

| Member | Type | Default | What it is for |
|---|---|---|---|
| `Items` | `IList<TimelineItem>` | empty | The entries, oldest first. A **live** collection: add, remove or replace and the control re-renders itself. |
| `SelectedItem` | `TimelineItem` | `null` | The highlighted entry. Setting it moves the highlight **without** raising `ItemSelected` — "the user clicked" and "the screen restored a selection" are different things. |
| `SelectedIndex` | `int` | `-1` | Its position in `Items`. |
| `Caption` | `string` | `"STATUS TIMELINE"` | The small uppercase caption. Upper-cased by the setter so five screens cannot disagree. |
| `TimeFormat` | `string` | `"MMM dd, HH:mm"` | A .NET date format string. Validated in the setter: a bad format throws `ArgumentException` naming the property, not a `FormatException` deep inside a render loop. |
| `EmptyText` | `string` | `"No history to show."` | What an empty component says instead of showing nothing. |
| `SampleMode` | `bool` | `false` | Design-time sample data (see below). |
| `SetItems(params TimelineItem[])` / `SetItems(IEnumerable<TimelineItem>)` | method | | Replace everything in one call, one render. The walkthrough's API. |
| `Clear()` / `ClearSelection()` | method | | |
| `ItemSelected` | `EventHandler<TimelineItemEventArgs>` | | The user clicked an entry. Carries the item and its index. |
| `SelectionCleared` | `EventHandler` | | The highlight was dropped because the entries were replaced. |

Every property carries `[Category]`, `[Description]` and — where the Designer needs to know what not to
serialize — `[DefaultValue]`. That is the only documentation most users of the component will ever read.

## What the component is *not* allowed to know

`TimelineItem` is three fields and an enum: `At`, `Status`, `Message`, `Severity` (+ an untyped `Tag` the
screen can use to find its own object again). It is not a domain entity and it never will be.

`WorkOrderHistoryEntry` (the entity) has `Actor` and `WorkOrderId`; `HistoryEntryView` (the service
projection) drops `Actor`; `WorkOrderHistoryPage.ToTimelineItem` maps the projection into `TimelineItem`.
Three narrowings, on purpose: the component **physically cannot** leak the actor, the tenant or the version
of a work order, because it was never given them.

The severity → colour map lives in one private method (`StatusTimeline.ColorFor`). A screen says
"this entry is critical", never "this entry is `#c0392b`" — which is how five screens stay consistent and
how a theme change happens in one file.

## Design-time sample mode

The Designer creates the control with no services and no data. `OnCreateControl` turns `SampleMode` on when
`DesignMode` is true, and sample mode renders a fixed four-entry timeline (Created → Assigned → On hold →
Escalated) plus the amber `DESIGN-TIME SAMPLE` badge. It calls nothing and cannot throw.

`SampleMode` is also a public property, so a screen can switch it on at run time to show exactly what the
Designer shows.

## Evidence — what the running app shows

| Do this | You should see |
|---|---|
| Open `http://localhost:5208` | The left card shows work order 2002 with four entries: grey *Created*, blue *Assigned*, amber *On hold*, red *Escalated* — the four entries the walkthrough video shows. |
| Click any entry | The row highlights and the status bar shows the entry's status and message (the screen's `ItemSelected` handler). |
| Open `UI/WorkOrderHistoryPage.cs` in the Wisej Designer | The timeline renders the fixed sample with the amber `DESIGN-TIME SAMPLE` badge; no service is called. |
