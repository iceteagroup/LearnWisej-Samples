# OperationsConsole · Mastering the Control Library · Module 7

Lab build for **Module 7 · Custom Widgets, Extensions, Theming, and Capstone**: the **Widgets** section is a
`Wisej.Web.Widget` named `ratingWidget` wrapping a small star-rating JavaScript library, with a message trace of the
bridge in both directions, every rule on the server, theming through packaged CSS, and the capstone documentation.
This folder is the whole Operations Console after module 7.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Mastering the Control Library Course/Module 7/OperationsConsole"
dotnet run -f net10.0 --urls http://localhost:5707
```

Then open <http://localhost:5707> and select **Widgets**. Keep the browser console open — it should stay clean.

## What to try

| Action | What you should see |
|---|---|
| Open **Widgets** | five grey stars for Northwind Traders, "no saved rating yet" |
| Hover the stars | a gold preview, no server round trip |
| Click the 4th star | message trace `← JS→.NET ratingChanged {"value":4}` then `→ .NET→JS setSaved 4`; the **✓ Saved 4/5** badge; a Toast; green status; StatusBar `Record: CUST-1042` |
| In the browser console: `app.getWidget("ratingWidget").fireWidgetEvent("ratingChanged", { value: "seven" })` (or `9`) | the payload is rejected: amber status, an AlertBox with the reason, nothing stored, stars still clickable |
| Tick **Simulate service failure**, click a star | `→ .NET→JS ratingClearSaved`, red status, a friendly AlertBox, the widget stays editable. Untick and click again to recover |
| Resize the browser under 601 px | the stars shrink (Phone profile → `.is-narrow`) |
| **Refresh** on the ToolBar | the rating is re-read and pushed into the widget |

## Where things live

```
OperationsConsole/
├─ Sections/WidgetsPage.cs / .Designer.cs   ratingWidget, the WidgetEvent handler, the message trace
├─ Widgets/RatingInitScript.cs              loads the InitScript from the embedded resource
├─ wwwroot/rating.js                        the library: stars, gestures, a "change" event — no colours, no rules
├─ wwwroot/rating.css                       every colour and size as CSS variables; theme variants, .saved, .is-narrow
├─ wwwroot/rating-init.js                   the adapter (embedded InitScript): init / update / setSaved
├─ Models/RatingModel.cs, Services/RatingService.cs   TryNormalize (1..5, whole number), Save
└─ docs/WidgetDecision.md, WidgetContract.md, CapstoneNotes.md, DemoScript.md
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| Decision note | `docs/WidgetDecision.md` |
| `ratingWidget` with `rating.js` and `rating.css` in `Packages`, in load order; `InitScript` whose `init` builds inside `this.container`, captures `var me = this`, safe to run again | `WidgetsPage.Designer.cs`; `wwwroot/rating-init.js` (`this.init`, `_teardownRating()`); `Widgets/RatingInitScript.cs` |
| `me.fireWidgetEvent("ratingChanged", { value })`; `ratingWidget_WidgetEvent` checks `e.Type`, validates `e.Data` to 1–5, saves through `RatingService` | `rating-init.js` `_onRatingChange`; `WidgetsPage.ratingWidget_WidgetEvent`; `RatingService.TryNormalize` / `Save` |
| `CallAsync("setSaved", value)`, `setSaved` in the InitScript, a Toast | `WidgetsPage.SaveRatingAsync()`; `this.setSaved` |
| Theme through packaged CSS / the `StyleSheet` extender, narrow profile, accessible label | `rating.css`; `ApplyConsoleStyleSheet()`; `Application_ResponsiveProfileChanged`; `AccessibleName` / `AccessibleDescription` |
| Contract, `CapstoneNotes.md`, demo script | `docs/WidgetContract.md`, `docs/CapstoneNotes.md`, `docs/DemoScript.md` |
| Show every path: valid click, malformed / out-of-range payload rejected, failed save | `RejectPayload()`; `chkSimulateServiceFailure` → `ShowSaveFailure()` |

## Self-check answers

- **A user edits the `ratingChanged` payload in developer tools — what happens, and which line stops it?** Nothing is
  stored. `if (!_ratings.TryNormalize(raw, out int value, out string error))` in `ratingWidget_WidgetEvent`: nothing
  below it runs until the value is a whole number from 1 to 5, and `RatingService.Save` checks again.
- **The widget is blank after the tab is hidden and shown again: what do you check, in which order?** (1) The browser
  console. (2) Package loading and order. (3) Re-initialisation: `init` runs again, so it must tear down the previous
  library first — this adapter does, in `_teardownRating()`. A script in `Default.html` runs once, before the widget
  exists, so it cannot be the fix.
- **Which rule could move to JavaScript, and which must never leave the server?** The hover preview, keyboard
  navigation, or ignoring a click on the already-selected star could. The 1–5 rule, the customer's identity and the
  decision that a value was stored must stay on the server.

## Known simplifications

- The rating store is in memory and rates one fixed customer (`CUST-1042`).
- `ratingWidget` is a plain `Wisej.Web.Widget` on the page, the lab's shape; production would use a subclass.
