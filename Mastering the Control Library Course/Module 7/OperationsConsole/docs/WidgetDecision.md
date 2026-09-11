# Widget decision note

**Module 7 · Custom Widgets, Extensions, Theming, and Capstone — lab task 3**
*"Write a one-paragraph decision note: which native controls you considered for the rating or indicator, why none
fits, and which extension type you are building."*

## The decision

The Operations Console needs a **customer satisfaction rating** on the Widgets screen: five stars, click or arrow-key
to pick a value from 1 to 5, a visible "saved" state once the server has stored it. I went through the native library
first. `TrackBar` gives a 1–5 integer with keyboard support but reads as a volume slider, not a judgement, and has no
place to show "this is the value we stored". A `NumericUpDown` or a `ComboBox`/`RadioButton` group records the same
number but turns a one-click gesture into a form field. `ProgressBar` displays a level but is read-only. A
`UserControl` holding five `Button`s with star images would stay entirely native — the honest runner-up — but it costs
themed images, hand-written hover and keyboard handling, and a server round trip for every hover preview. The gesture
(hover preview, click, keyboard, a visual "saved" state) is **browser-only work**, so the extension type is a
**`Widget`**: the general-purpose client container for a small JavaScript library (`wwwroot/rating.js`) inside a
Wisej.NET container. Not a **Control** (one screen, not a reusable product component), not a **Component** (it has a
visual surface), not an **Extender Provider** (it does not add a property to existing controls), not an **Icon Pack**.
And `Widget` keeps what matters on the server: the rule "a rating is a whole number from 1 to 5" lives in
`Services/RatingService.cs`, the payload is validated on arrival, and the colours come from a packaged stylesheet.

## Alternatives

| Considered | Why it was not used |
|---|---|
| `TrackBar` | Right value type, but the affordance is "adjust a level"; no saved-state affordance. |
| `NumericUpDown` | Turns a one-click gesture into typing. |
| `ComboBox` / `RadioButton` group | Same objection, plus five labels for one value. |
| `ProgressBar` | Read-only — an indicator, not an input. |
| `UserControl` with five star `Button`s | The closest native answer; rejected for the hover round trips and themed images. |
| **`Widget` + `rating.js` + `rating.css`** | **Chosen.** Rendering and gestures in JavaScript, every rule and all state on the server. |

## Evidence (what the running app shows)

- The Widgets section shows a five-star control; hovering previews without a server round trip, clicking commits.
- The message trace shows one message per click in each direction: `← JS→.NET ratingChanged {"value":4}` then
  `→ .NET→JS setSaved 4`.
- A payload typed in the browser console (`"seven"`, `9`) is refused by the server: the rule is not in the JavaScript.
