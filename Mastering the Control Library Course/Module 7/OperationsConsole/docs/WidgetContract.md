# `ratingWidget` — client/server contract

**Module 7 · lab task 8** — *"document the contract next to the code (packages and load order, public properties,
event names, payload format, server handling, debugging notes)."* Everything the server and the browser have agreed
on is here; anything not listed may change.

## 1. The files and what each one is allowed to know

| File | Role | Must **not** contain |
|---|---|---|
| `wwwroot/rating.js` | The library. Renders stars, handles click / hover / arrow keys, raises its own `change` event. | Business rules, colours, `fireWidgetEvent`, `this.container` |
| `wwwroot/rating.css` | Every colour and metric, as CSS custom properties; theme variants, `.saved`, the narrow rule. | Anything the server must decide |
| `wwwroot/rating-init.js` | The **adapter** (the Widget's `InitScript`). The only file that knows both worlds. | Business rules |
| `Sections/WidgetsPage.cs` | The server half: wiring, event handling, feedback. | DOM knowledge |
| `Services/RatingService.cs` | **Every rule.** Validation, normalisation, storage. | UI |

## 2. Packages and load order

```csharp
// Sections/WidgetsPage.Designer.cs
ratingWidget.Packages.Add(new Widget.Package { Name = "rating-css", Source = "wwwroot/rating.css" });
ratingWidget.Packages.Add(new Widget.Package { Name = "rating-js",  Source = "wwwroot/rating.js"  });
```

- Packages load **in list order**, once per page. The stylesheet is first, so the stars never flash unstyled.
- `Startup.cs` serves the project folder, so `wwwroot/rating.js` is fetched as `/wwwroot/rating.js`.
- The **`InitScript`** is an embedded resource (`OperationsConsole.wwwroot.rating-init.js`) loaded by
  `Widgets/RatingInitScript.cs`. A `<script>` in `Default.html` would run before the widget exists.
  `Widget.GetResourceString(...)` is `protected`, so the resource is read with reflection.

## 3. Public server-side surface

| Member | Type | Meaning |
|---|---|---|
| `ratingWidget.Options.value` | `int` | The rating shown (0 = not rated). Assign it and call `Update()`. |
| `ratingWidget.Options.max` | `int` | Top of the scale (`RatingService.MaxRating`). |
| `ratingWidget.Options.label` | `string` | The caption under the stars (the customer name). |
| `ratingWidget.Options.saved` | `bool` | Whether the shown value is the stored value. |
| `ratingWidget.Options.theme` | `"bootstrap"` \| `"material"` | Which variant of `rating.css` to use. |
| `ratingWidget.Options.profile` | `"Phone"` \| `"Tablet"` \| `"Desktop"` | `Application.ActiveProfile.Name`; `Phone` → `.is-narrow`. |
| `ratingWidget.Options.name` | `string` | Registry key for the browser console. |
| `ratingWidget.WiredEvents` | `string[]` | `{ "ratingChanged" }`. |
| `ratingWidget.AccessibleName` | `string` | "Customer satisfaction rating" (+ `AccessibleDescription`). |

Options are first-level fields only, so `Update()` reaches the client's `update(options, old)`.

## 4. Events out — client → server

```
ratingChanged   { "value": <number> }
```

| | |
|---|---|
| Raised by | `rating-init.js`, from the library's `change` callback (click, Enter/Space, arrow keys) |
| Server handler | `WidgetsPage.ratingWidget_WidgetEvent(object sender, WidgetEventArgs e)` |
| Read as | `e.Type`, `e.Data.value` (dynamic) |
| Trust level | **None.** `value` is whatever the browser sent. |

## 5. Calls in — server → client

| Call | Client function | Used for |
|---|---|---|
| `await ratingWidget.CallAsync("setSaved", value)` | `setSaved(value)` | Confirm a stored value; the widget gets the `.saved` class |
| `ratingWidget.Call("ratingClearSaved")` | `ratingClearSaved()` | A save failed — drop the saved state, keep the widget editable |
| `ratingWidget.Call("ratingSetBusy", false)` | `ratingSetBusy(busy)` | Leave the busy state after a rejection |
| `ratingWidget.Options.x = …; Update()` | `update(options, old)` | Push state in silently — no event comes back |

> **Naming rule.** A function declared in the InitScript with the name of a qooxdoo method (`getWidth`, `setValue`,
> `getValue`, `resize`, `show`, `hide`, `destroy` …) replaces it and breaks the control. `dispose` is wrapped, never replaced.

## 6. Server handling — the order that matters

1. **Check the type.** `if (e.Type != "ratingChanged") return;`
2. **Validate and normalise** through `RatingService.TryNormalize(raw, out value, out error)`: `null`, a non-numeric
   string, a non-integer or a value outside 1–5 is rejected. This is the line that stops a hand-edited payload.
3. **Save** through `RatingService.Save(value)`, which refuses out-of-range values again and throws when the store is
   unavailable.
4. **Confirm and tell the user**: `CallAsync("setSaved", value)`, a `Toast`, the status area, the card. On failure:
   `Call("ratingClearSaved")`, an `AlertBox`, a red status, and the widget stays editable.

The 1–5 rule, the customer being rated, the stored value and the decision that a save succeeded never move to
JavaScript. The hover preview and keyboard navigation are client-side on purpose.

## 7. Theming, responsive, accessibility

- Every colour and metric is a CSS custom property in `rating.css`; `rating.js` only adds and removes class names.
- `Wisej.Web.StyleSheet` (`WidgetsPage.ApplyConsoleStyleSheet`) is the application-level override layer.
- `Application.ResponsiveProfileChanged` → `Options.profile` → `.is-narrow` for `Phone`; plus a
  `@media (max-width: 600px)` rule in `rating.css`.
- `role="radiogroup"` / `role="radio"`, `aria-checked`, a roving `tabindex` and arrow-key navigation in `rating.js`.

## 8. Debugging notes

| Symptom | Check, in this order |
|---|---|
| Blank widget | 1. Browser console errors. 2. Did both packages load (`/wwwroot/rating.css`, `/wwwroot/rating.js`, 200)? 3. Load order. 4. Is `init` running — `app.getWidget("ratingWidget")`. |
| Blank after the tab was hidden and shown | `init` runs again; this adapter tears the previous library down first (`_teardownRating`). |
| The event never reaches the server | Is the name in `WiredEvents`? Was it fired synchronously during `update()`? Those are dropped. |
| Styled wrong | `rating.css`, then the `.opc-rating--*` variant, then the StyleSheet rules. Never `rating.js`. |

```js
app.getWidget("ratingWidget")            // the Wisej.NET widget wrapper
app.getWidget("ratingWidget").widget     // the RatingWidget library object
// test the server-side validation with a payload the widget would never send:
app.getWidget("ratingWidget").fireWidgetEvent("ratingChanged", { value: "seven" })
```

Both client files end with `//# sourceURL=`, so they appear by name in the DevTools Sources panel.

## Evidence (what the running app shows)

| Path | What you see |
|---|---|
| Click the 4th star | Message trace `← JS→.NET ratingChanged {"value":4}` then `→ .NET→JS setSaved 4`; the **✓ Saved 4/5** badge; a Toast; green status |
| A payload typed in the browser console (`"seven"`, `9`) | `← JS→.NET ratingChanged {"value":"seven"}`, amber status and an AlertBox with the reason; nothing is stored |
| **Simulate service failure**, then a click | `→ .NET→JS ratingClearSaved`, red status, an AlertBox; the stars stay editable |
