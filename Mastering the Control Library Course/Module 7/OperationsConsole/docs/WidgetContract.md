# `ratingWidget` — client/server contract

**Module 7 · lab task 8** — *"document the contract next to the code (packages and load order,
public properties, event names, payload format, server handling, debugging notes)."*

This is the file the next developer — or an AI assistant grounded on this project — should be
pointed at. Everything the server and the browser have agreed on is here; nothing else is part of
the contract, and anything not listed may change.

---

## 1. The three files and what each one is allowed to know

| File | Role | Must **not** contain |
|---|---|---|
| `wwwroot/rating.js` | The library. Renders stars, handles click / hover / arrow keys, raises its own `change` event. Knows nothing about Wisej.NET. | Business rules, colours, `fireWidgetEvent`, `this.container` |
| `wwwroot/rating.css` | Every colour and metric, as CSS custom properties. Theme variants, the `.saved` state, the narrow-profile rule. | Anything the server must decide |
| `wwwroot/rating-init.js` | The **adapter** (the Widget's `InitScript`). The only file that knows both worlds. | Business rules |
| `Sections/WidgetsPage.cs` | The server half: wiring, event handling, feedback. | DOM knowledge |
| `Services/RatingService.cs` | **Every rule.** Validation, normalisation, storage, the failure mode. | UI |

## 2. Packages and load order

```csharp
// Sections/WidgetsPage.Designer.cs
ratingWidget.Packages.Add(new Widget.Package { Name = "rating-css", Source = "wwwroot/rating.css" });
ratingWidget.Packages.Add(new Widget.Package { Name = "rating-js",  Source = "wwwroot/rating.js"  });
```

- Packages load **in list order**, once per page; Wisej.NET caches them by `Name`, so two widgets
  that declare the same names share one download.
- A **stylesheet is a package too**. It is declared first on purpose: the CSS is in place before
  `rating.js` creates a single element, so the stars never flash unstyled.
- `Source` is resolved against the site root. `Startup.cs` serves the **project folder**
  (`WebRootPath = "./"`), so `wwwroot/rating.js` is fetched as `/wwwroot/rating.js`.
- The **`InitScript` is not a package**: it is compiled into the assembly as an embedded resource
  (`OperationsConsole.wwwroot.rating-init.js`, see the `<EmbeddedResource>` entry in the csproj) and
  loaded by `Widgets/RatingInitScript.cs`. A `<script>` in `Default.html` would run *before* the
  Wisej.NET widget exists — the "initialisation timing" pitfall from the module reading.
  *Note:* `Widget.GetResourceString(...)` — the helper the cookbook mentions — is `protected`, so it
  is only reachable from a `Widget` **subclass**. This module uses a plain `Wisej.Web.Widget` on the
  page, so `RatingInitScript` reads the same resource with ordinary reflection.

## 3. Public server-side surface

| Member | Where | Meaning |
|---|---|---|
| `ratingWidget.Options.value` | `int` | The rating currently shown (0 = not rated). First-level field: assign it and call `Update()`. |
| `ratingWidget.Options.max` | `int` | Top of the scale. `RatingService.MaxRating`. |
| `ratingWidget.Options.label` | `string` | The caption under the stars (the customer name). |
| `ratingWidget.Options.saved` | `bool` | Whether the shown value is the stored value. |
| `ratingWidget.Options.theme` | `"bootstrap"` \| `"material"` | Which variant of `rating.css` to use. |
| `ratingWidget.Options.profile` | `"Phone"` \| `"Tablet"` \| `"Desktop"` | `Application.ActiveProfile.Name`. `Phone` → `.is-narrow`. |
| `ratingWidget.Options.name` | `string` | Registry key for the DevTools console (`app.getWidget("ratingWidget")`). |
| `ratingWidget.WiredEvents` | `string[]` | `{ "ratingChanged" }` — the only event the client may raise. |
| `ratingWidget.AccessibleName` | `string` | "Customer satisfaction rating" (+ `AccessibleDescription`). |

Options are **first-level fields only**. Changing a nested field would need `Update()` or
`Options.Notify("path")`; this widget has no nested options on purpose.

## 4. Events out — client → server

Exactly one:

```
ratingChanged   { "value": <number> }
```

| | |
|---|---|
| Raised by | `rating-init.js`, from the library's `change` callback (a real user gesture: click, Enter/Space, arrow keys) |
| Also raised by | `sendRawPayload(v)` — the deliberate "developer tools" simulation, **deferred** with `setTimeout(…, 0)` |
| Server handler | `WidgetsPage.ratingWidget_WidgetEvent(object sender, WidgetEventArgs e)` |
| Read as | `e.Type` (string), `e.Data.value` (dynamic — read by the **JavaScript** field name) |
| Trust level | **None.** `value` is whatever the browser sent: a number, a string, `null`, missing. |

Anything with a different `e.Type` is logged and ignored — the handler never assumes it was called
for its own event.

### Payload examples seen in the Event log

```
← JS→.NET ratingChanged {"value":4}          a real click
← JS→.NET ratingChanged {"value":"seven"}    the "Send malformed payload" button
← JS→.NET ratingChanged {"value":9}          the "Send out-of-range payload" button
```

## 5. Calls in — server → client

| Call | Client function | One-way / awaited | Used for |
|---|---|---|---|
| `ratingWidget.Call("setSaved", value)` | `setSaved(value)` | one-way, flushed with the response | Confirm a stored value; the widget gets the `.saved` class |
| `ratingWidget.Call("ratingClearSaved")` | `ratingClearSaved()` | one-way | A save failed — drop the saved state, keep the widget editable |
| `ratingWidget.Call("ratingSetBusy", false)` | `ratingSetBusy(busy)` | one-way | Leave the busy state after a rejection |
| `await ratingWidget.CallAsync("getState")` | `getState()` | awaited, returns a value | Read the browser's own view of the widget |
| `ratingWidget.Call("reinit")` | `reinit()` | one-way | Re-run `init` and prove it is idempotent |
| `ratingWidget.Call("sendRawPayload", v)` | `sendRawPayload(v)` | one-way | Teaching hook: make the client send a bad payload |
| `ratingWidget.Options.x = …; Update()` | `update(options, old)` | one-way | Push state in **silently** — no event bounces back |

`getState()` returns
`{ value, savedValue, saved, max, label, theme, narrow, hasInstance }`.

> **Naming rule.** The wrapper is a qooxdoo widget, so a function declared in the InitScript with
> the name of a framework method **replaces** it and breaks the control. Never define `getWidth`,
> `getHeight`, `getBounds`, `setValue`, `getValue`, `getName`, `resize`, `show`, `hide`, `destroy`.
> `dispose` is **wrapped**, never replaced. That is why the custom functions here are `setSaved`,
> `ratingClearSaved`, `ratingSetBusy`, `reinit`, `sendRawPayload`.

## 6. Server handling — the order that matters

`WidgetsPage.ratingWidget_WidgetEvent` does five things, in this order:

1. **Check the type.** `if (e.Type != "ratingChanged")` → log and return.
2. **Log what arrived**, in the shape it arrived in (`{"value":"seven"}`), before interpreting it.
3. **Validate and normalise** through `RatingService.TryNormalize(raw, out value, out error)`:
   `null` → rejected · a non-numeric string → rejected · a non-integer → rejected ·
   outside 1–5 → rejected. This is the line that stops a hand-edited payload.
4. **Save** through `RatingService.Save(value)`, which refuses out-of-range values again (defence in
   depth) and throws when the store is unavailable.
5. **Confirm and tell the user**: `Call("setSaved", value)`, a `Toast`, the status area, the card.
   On failure: `Call("ratingClearSaved")`, an `AlertBox`, a red status, and the widget stays
   editable — the user's click was not lost.

### What must never move to JavaScript

The 1–5 rule, the identity of the customer being rated, the stored value, and the decision that a
save succeeded. `rating.js` may keep a *rendering* clamp so it never draws six stars; that is
hygiene, not a rule, and the server re-checks anyway.

### What could move to JavaScript

The hover preview (already there), the keyboard navigation (already there), and — as a deliberate
optimisation — a client-side pre-check that ignores a click on a star it already shows. None of
those is the only copy of anything.

## 7. Theming

- Every colour and metric is a CSS custom property in `rating.css`; `rating.js` only adds and
  removes class names. That is what makes the widget follow `Application.LoadTheme`.
- The server maps the Wisej theme name to a variant class: `Bootstrap-4` → `.opc-rating--bootstrap`,
  `Material-3` → `.opc-rating--material` (`WidgetsPage.ThemeKey`). Press **Theme → Material-3** and
  the native controls and the stars restyle together.
- `Wisej.Web.StyleSheet` (`WidgetsPage.ApplyConsoleStyleSheet`) is the *application-level* override
  layer: it tags the widget with `opc-console-rating` and injects console-specific rules, so the
  console can adjust the widget without editing files that belong to the widget.
- **Never** a colour in `rating.js`: a colour written there cannot be reached by the theme, by the
  StyleSheet extender, or by the next developer looking in the obvious place.

## 8. Responsive

- Server-driven: `Application.ResponsiveProfileChanged` → `Options.profile` → the adapter toggles
  `.is-narrow` (26 px stars, wrapped caption) for the `Phone` profile from `ClientProfiles.json`.
- Also a plain `@media (max-width: 600px)` rule in `rating.css`, so the widget still behaves if it
  is ever used outside this console.

## 9. Accessibility

- `ratingWidget.AccessibleName = "Customer satisfaction rating"` and an `AccessibleDescription` on
  the container (Wisej.NET 4.1 has no `AccessibleRole`).
- Inside, `rating.js` gives the root `role="radiogroup"` with an `aria-label` that includes the
  value, each star `role="radio"` + `aria-checked` + `aria-label`, a roving `tabindex`, and
  arrow-key navigation with a visible `:focus-visible` outline.

## 10. Debugging notes

**Start in the browser console — the Wisej.NET documentation says so explicitly for `Widget`.**

| Symptom | Check, in this order |
|---|---|
| Blank widget | 1. Console errors. 2. Did both packages load? Network tab, `/wwwroot/rating.css` and `/wwwroot/rating.js`, 200. 3. Load **order** — CSS then JS. 4. Is `init` running? `app.getWidget("ratingWidget")` in the console. |
| Blank after the tab was hidden and shown | `init` ran again and left two host elements, or the old library was never destroyed. Press **Re-run init()**: this adapter tears the previous library down first (`_teardownRating`), so it is safe. |
| The event never reaches the server | Is the name in `WiredEvents`? Is the payload shape right? Was it fired **synchronously during `update()`** — those are dropped; defer with `setTimeout(…, 0)` (see `_fireDeferred`). |
| The widget is styled wrong | Look in `rating.css`, then at the `.opc-rating--*` variant class the server pushed, then at the StyleSheet extender rules. Never look in `rating.js`. |

**Where the objects live**

```js
app.getWidget("ratingWidget")            // the Wisej.NET widget wrapper (registry filled by init)
app.getWidget("ratingWidget").widget     // the RatingWidget library object
app.getWidget("ratingWidget").host       // the <div> the library renders into
app.getWidget("ratingWidget").container  // the element the FRAMEWORK owns — never replace it
```

Both `rating.js` and `rating-init.js` end with a `//# sourceURL=` line, so they appear by name in
the DevTools **Sources** panel and breakpoints can be set inside the injected InitScript.

## Evidence (what the running app shows)

| Path | What you see |
|---|---|
| Click the 4th star | `← JS→.NET ratingChanged {"value":4}` → `→ .NET→JS setSaved 4`; the stars go gold with a **✓ Saved 4/5** badge; a Toast top-right; green status. |
| **Read client state** | `→ .NET→JS getState() (CallAsync — waiting for the browser)` then `← JS→.NET getState → value 4 · savedValue 4 · saved True · max 5 · theme bootstrap · narrow False`, printed in the bridge card. |
| **Push 5 from server** | `→ .NET→JS Options.value 5 (via update(options, old) — applied silently)`; the widget shows 5 with **no** `ratingChanged` coming back. |
| **Send malformed payload** | `→ .NET→JS sendRawPayload "seven"` then `← JS→.NET ratingChanged {"value":"seven"}` then `✗ payload rejected — "seven" is not a rating…`; nothing is stored. |
| **Re-run init()** | The stars redraw once, the value is unchanged, and the browser console stays clean. |
| **Theme → Material-3** | Native controls and the stars restyle together; `→ .NET→JS Options.theme material`. |
